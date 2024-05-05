using Xamarin.Essentials;

namespace App26
{
    public static class LocalDatabase
    {
        /// <summary>
        /// This method returns a bool value stored under the given key in the local storage. 
        /// If the value does not exist under the given key, the method returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBool(string key)
        {
            return Preferences.Get(key, false);
        }

        /// <summary>
        /// This method returns a string value stored under the given key in the local storage. 
        /// If the value does not exist under the given key, the method returns an empty string ("").
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return Preferences.Get(key, "");
        }

        /// <summary>
        /// his method stores a bool value in the local storage under the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void PutBool(string key, bool value)
        {
            Preferences.Set(key, value);
        }

        /// <summary>
        /// This method stores a string value in the local storage under the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void PutString(string key, string value)
        {
            Preferences.Set(key, value);
        }
    }
}