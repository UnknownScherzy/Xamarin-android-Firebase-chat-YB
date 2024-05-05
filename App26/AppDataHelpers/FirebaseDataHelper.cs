using Android.App;
using Firebase;
using Firebase.Firestore;

namespace App26.AppDataHelpers
{
    public static class FirebaseDataHelper
    {
        public static FirebaseFirestore Database
        {
            get
            {
                FirebaseApp application = FirebaseApp.InitializeApp(Application.Context);

                if (application == null)
                {
                    FirebaseOptions options = new FirebaseOptions.Builder()
                        .SetApplicationId("workingchat-dc0bf")
                        .SetDatabaseUrl("workingchat-dc0bf.appspot.com")
                        .SetApiKey("AIzaSyABb82JZvBWcxxuJ9jYYhIqOCEskK2ETS4")
                        .Build();
                    application = FirebaseApp.InitializeApp(Application.Context, options);
                }

                return FirebaseFirestore.GetInstance(application);
            }
        }
    }
}