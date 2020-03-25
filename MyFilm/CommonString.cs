using System;
using System.IO;

namespace MyFilm
{
    public class CommonString
    {
        // Web搜索关键字
        public static String WebSearchKeyWord = String.Empty;
        // 主界面标题
        public static String MainFormTitle = String.Empty;

        public static String MyFilmApplicationDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyFilm");

        public static LoginConfig.LoginConfigData LoginConfigData = null;

        public readonly static String RealOrFake4KDiskName = "REAL_OR_FAKE_4K";

        public readonly static String RSAKeyContainerName =
            "myfilm_RSA_KCN_{ADA91B04-A829-496A-BF8B-1A6478C833C4}";

        public readonly static String PipeName =
            "myfilm_pipe_{8AC5703C-D6F4-43F0-B625-11D27D2ADCF8}";
        public readonly static String MemoryMappedName =
            "myfilm_memory_mapped_{98516080-4D5E-4A73-AA2D-37CC7AACCA07}";

        public readonly static String SharedMemorySemaphoreReceiveReadName =
            "myfilm_SMS_RR_{203FD686-411F-40A4-ACC3-DE3528E1EC32}";
        public readonly static String SharedMemorySemaphoreReceiveWriteName =
            "myfilm_SMS_RW_{C039F665-E406-44D1-998D-73C9DA19CECE}";
        public readonly static String SharedMemorySemaphoreSendReadName =
            "myfilm_SMS_SR_{60249DE1-7C02-4148-93CD-6F3F49312CB3}";
        public readonly static String SharedMemorySemaphoreSendWriteName =
            "myfilm_SMS_SW_{CA1A831D-EAB3-4389-BF87-9EDC3D4865E8}";

        public readonly static String AppMutexName =
            "myfilm_app_mutex_{2D9D20B5-555B-49BA-A0C2-1CDCB2A255F7}";

        public readonly static string[] MediaExts =
            new string[] { ".mkv", ".mp4", ".flv", ".ts", ".m2ts" };

        public readonly static String CrawlURL =
            "https://digiraw.com/DVD-4K-Bluray-ripping-service/4K-UHD-ripping-service/the-real-or-fake-4K-list/";
    }
}
