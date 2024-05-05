using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using App26.AppDataHelpers;
using App26.Resources;
using Firebase.Firestore;
using System;

namespace App26
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private EditText _loginEmail;
        private EditText _loginPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (LocalDatabase.GetBool(Constants.IS_LOGGED))
            {
                LoadDashboard();
            }

            InitInputs();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void InitInputs()
        {
            _loginEmail = FindViewById<EditText>(Resource.Id.loginEmail);
            _loginPassword = FindViewById<EditText>(Resource.Id.loginPassword);

            FindViewById<Button>(Resource.Id.signUpButton).Click += delegate
            {
                StartActivity(new Intent(this, typeof(RegistrationActivity)));
            };

            FindViewById<Button>(Resource.Id.logInButton).Click += OnLogInClick;
        }

        private void OnLogInClick(object sender, EventArgs eventArgs)
        {
            if (!ValidateInput())
            {
                return;
            }

            Authenticate();
        }

        private async void Authenticate()
        {
            FirebaseFirestore database = FirebaseDataHelper.Database;
            QuerySnapshot userSnapshot = await database.Collection(Constants.USERS_TABLE_ID)
                    .WhereEqualTo(Constants.USER_EMAIL, _loginEmail.Text)
                    .WhereEqualTo(Constants.USER_PASSWORD, _loginPassword.Text)
                    .Get()
                    .AsAsync<QuerySnapshot>();

            if (userSnapshot != null && userSnapshot.Documents.Count > 0)
            {
                CreateUserSession(userSnapshot.Documents[0]);
                LoadDashboard();
            }
            else
            {
                Toast.MakeText(this, "Authentication error", ToastLength.Long).Show();
            }
        }

        private void LoadDashboard()
        {
            Intent intent = new(this, typeof(DashboardActivity));
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
        }

        private void CreateUserSession(DocumentSnapshot documentSnapshot)
        {
            LocalDatabase.PutString(Constants.USER_ID, documentSnapshot.Id);
            LocalDatabase.PutString(Constants.USER_NAME, documentSnapshot.GetString(Constants.USER_NAME));
            LocalDatabase.PutString(Constants.USER_EMAIL, documentSnapshot.GetString(Constants.USER_EMAIL));
            LocalDatabase.PutString(Constants.USER_PHOTO, documentSnapshot.GetString(Constants.USER_PHOTO));
            LocalDatabase.PutBool(Constants.IS_LOGGED, true);
        }

        private bool ValidateInput()
        {
            if (_loginEmail.Text.Trim().Length == 0)
            {
                _loginEmail.SetError("Enter email", null);
                return false;
            }

            if (_loginPassword.Text.Trim().Length == 0)
            {
                _loginPassword.SetError("Enter password", null);
                return false;
            }

            return true;
        }
    }
}