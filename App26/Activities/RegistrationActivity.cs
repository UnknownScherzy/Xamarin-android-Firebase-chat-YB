using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using App26.AppDataHelpers;
using Java.Util;
using System;

namespace App26.Resources
{
    [Activity(Label = "RegistrationActivity")]
    public class RegistrationActivity : Activity
    {
        private EditText _nameInput;
        private EditText _emailInput;
        private EditText _passwordInput;
        private EditText _confirmPasswordInput;
        private ImageView _imagePicker;

        private Bitmap _loadedImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_registration);
            InitInputs();
        }

        private void InitInputs()
        {
            _nameInput = FindViewById<EditText>(Resource.Id.editTextName);
            _emailInput = FindViewById<EditText>(Resource.Id.editTextEmail);
            _passwordInput = FindViewById<EditText>(Resource.Id.editTextPassword);
            _confirmPasswordInput = FindViewById<EditText>(Resource.Id.editTextPasswordConfirm);
            _imagePicker = FindViewById<ImageView>(Resource.Id.imagePicker);

            FindViewById<Button>(Resource.Id.backToLogInButton).Click += delegate
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
            };

            FindViewById<Button>(Resource.Id.signUpButton).Click += OnSignUpClick;
            _imagePicker.Click += OnImagePickerClick;
        }

        private void OnImagePickerClick(object sender, EventArgs eventArgs)
        {
            Intent intent = new(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            StartActivityForResult(intent, 0);
        }

        private void OnSignUpClick(object sender, EventArgs eventArgs)
        {
            if (ValidateInput())
            {
                MakeRegistrationAsync();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == 0 && data != null)
            {
                _loadedImage = BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(data.Data));
                _imagePicker.SetImageBitmap(_loadedImage);
            }
            else
            {
                Toast.MakeText(this, "Cannot selecet image", ToastLength.Short).Show();
            }
        }

        private async void MakeRegistrationAsync()
        {
            try
            {
                await FirebaseDataHelper.Database
                    .Collection(Constants.USERS_TABLE_ID)
                    .Add(CreateUser());
                Toast.MakeText(this, "Registration successful. Please log in", ToastLength.Long).Show();
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message, ToastLength.Long).Show();
            }
        }

        private HashMap CreateUser()
        {
            HashMap newUser = new();
            newUser.Put(Constants.USER_NAME, _nameInput.Text);
            newUser.Put(Constants.USER_EMAIL, _emailInput.Text);
            newUser.Put(Constants.USER_PASSWORD, _passwordInput.Text);
            newUser.Put(Constants.USER_PHOTO, _loadedImage.EncodeImage());

            return newUser;
        }

        private bool ValidateInput()
        {
            if (_nameInput.Text.Length == 0)
            {
                _nameInput.SetError("Name is required", null);
                return false;
            }
            if (_emailInput.Text.Length == 0)
            {
                _emailInput.SetError("Email is required", null);
                return false;
            }
            if (_passwordInput.Text.Length == 0)
            {
                _passwordInput.SetError("Password is required", null);
                return false;
            }
            if (_confirmPasswordInput.Text.Length == 0)
            {
                _confirmPasswordInput.SetError("Password confirmation is required", null);
                return false;
            }
            if (!_confirmPasswordInput.Text.Equals(_passwordInput.Text))
            {
                _confirmPasswordInput.SetError("Confirm password doesn't match password", null);
                return false;
            }
            if (_passwordInput.Text.Length < 5)
            {
                _passwordInput.SetError("Password must be at least 5 characters in length", null);
                return false;
            }

            return true;
        }
    }
}