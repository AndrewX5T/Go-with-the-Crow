using System;
using System.Collections.Generic;

namespace Go_with_the_Crow
{
    /// <summary>
    /// Yes, it's a singleton. I know.
    /// </summary>
    public class Globals
    {
        private static readonly Globals __instance = new Globals();
        private Globals() { }

        public static Globals Instance
        {
            get => __instance;
        }

        private const string _default_image_dir = "UploadedImages/";
        public static string DefaultImageDir => _default_image_dir;


        private const string _util_image_dir = "UserImages/";
        public static string UtilImageDir => _util_image_dir;
    }
}