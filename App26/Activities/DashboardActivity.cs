using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace App26
{
    [Android.App.Activity(Label = "DashboardActivity")]
    public class DashboardActivity : AppCompatActivity
    {
        private UsersFragment _usersFragment;
        private ChatsFragment _chatsFragment;
        private SettingsFragment _settingsFragment;
        private TextView _dashboardTitle;

        private ImageView _createGroupButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.dashboard);

            InitDashboard();

            _usersFragment = new UsersFragment(_createGroupButton);
            _chatsFragment = new ChatsFragment();
            _settingsFragment = new SettingsFragment();

            ChangeFragment("Chat list", _chatsFragment);
        }

        private void InitDashboard()
        {
            FindViewById<ImageView>(Resource.Id.userBottomMenu).SetImageBitmap(LocalDatabase.GetString(Constants.USER_PHOTO).DecodeImage());
            _dashboardTitle = FindViewById<TextView>(Resource.Id.fragmentTitle);
            _createGroupButton = FindViewById<ImageView>(Resource.Id.createGroupButton);

            FindViewById<View>(Resource.Id.usersBottomMenu).Click += delegate
            {
                ChangeFragment("Users", _usersFragment);
            };

            FindViewById<View>(Resource.Id.userBottomMenu).Click += delegate
            {
                ChangeFragment(LocalDatabase.GetString(Constants.USER_NAME), _settingsFragment);
            };

            FindViewById<View>(Resource.Id.chatsBottomMenu).Click += delegate
            {
                ChangeFragment("Chat list", _chatsFragment);
            };
        }

        private void ChangeFragment(string title, Fragment fragmentClass)
        {
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, fragmentClass).Commit();
            _createGroupButton.Visibility = ViewStates.Gone;
            _dashboardTitle.Text = title;
        }
    }
}