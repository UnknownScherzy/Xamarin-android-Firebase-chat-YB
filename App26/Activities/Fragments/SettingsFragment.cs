using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace App26
{
    public class SettingsFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.settings_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ImageView profilePhotoView = view.FindViewById<ImageView>(Resource.Id.profileImageSettings);
            profilePhotoView.SetImageBitmap(LocalDatabase.GetString(Constants.USER_PHOTO).DecodeImage());

            view.FindViewById<Button>(Resource.Id.signOutButton).Click += delegate
            {
                LocalDatabase.PutBool(Constants.IS_LOGGED, false);
                StartActivity(new Intent(Context, typeof(MainActivity)));
            };
        }
    }
}