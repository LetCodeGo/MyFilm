using System;
using System.IO;

namespace MyFilm
{
    public class CommonString
    {
        public static String AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.Create), "MyFilm");

        public static String DbIP = "127.0.0.1";
        public static String DbUserName = string.Empty;
        public static String DbPassword = string.Empty;
        public static String DbName = "myfilm";

        public static String WebSearchKeyWord = String.Empty;
        public static String RealOrFake4KDiskName = "REAL_OR_FAKE_4K";

        public readonly static String PipeName =
            "myfilm_pipe_{8AC5703C-D6F4-43F0-B625-11D27D2ADCF8}";
        public readonly static String MemoryMappedName =
            "myfilm_memory_mapped_{98516080-4D5E-4A73-AA2D-37CC7AACCA07}";

        public readonly static String SharedMemorySemaphoreReadName =
            "myfilm_SM_SR_{203FD686-411F-40A4-ACC3-DE3528E1EC32}";
        public readonly static String SharedMemorySemaphoreWriteName =
            "myfilm_SM_SW_{C039F665-E406-44D1-998D-73C9DA19CECE}";
        public readonly static String AppMutexName =
            "myfilm_app_mutex_{2D9D20B5-555B-49BA-A0C2-1CDCB2A255F7}";

        public readonly static string[] MediaExts = 
            new string[] { ".mkv", ".mp4", ".flv", ".ts", ".m2ts" };
    }
}
