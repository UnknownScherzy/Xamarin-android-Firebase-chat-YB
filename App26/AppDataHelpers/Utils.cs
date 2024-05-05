using Android.Graphics;
using Android.Util;
using System;
using System.Globalization;
using System.IO;

namespace App26
{
    public static class Utils
    {
        private const int DEFAULT_WIDTH = 150;


        /// <summary>
        /// This extension method takes a Bitmap object and encodes it as a Base64 string. 
        /// It first scales the bitmap to a default width of 150 pixels while preserving the aspect ratio, 
        /// then compresses it as a JPEG with a quality of 50%, and finally encodes the compressed bytes as a 
        /// Base64 string 
        /// If the input bitmap is null, it returns an empty string.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string EncodeImage(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return "";
            }

            int height = bitmap.Height * DEFAULT_WIDTH / bitmap.Width;

            Bitmap scaled = Bitmap.CreateScaledBitmap(bitmap, DEFAULT_WIDTH, height, false);
            MemoryStream output = new MemoryStream();

            scaled.Compress(Bitmap.CompressFormat.Jpeg, 50, output);
            byte[] bytes = output.ToArray();

            return Base64.EncodeToString(bytes, Base64Flags.Default);
        }

        /// <summary>
        /// This extension method takes a Base64 string and decodes it as a Bitmap object. 
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static Bitmap DecodeImage(this string encoded)
        {
            if (encoded == null)
            {
                encoded = "";
            }

            byte[] bytes = Base64.Decode(encoded, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        }

        public static string FormatDate(this DateTime date)
        {
            if (date == null)
            {
                return "";
            }

            return date.ToString("dd. MM. yyyy - HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static DateTime FormatStringToDate(this string date)
        {
            return DateTime.ParseExact(date, "dd. MM. yyyy - HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}