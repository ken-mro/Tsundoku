using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsundoku.Repository;

public static class Constants
{
    private const string DATABSE_NAME = "Tsundoku.db3";
    private const string REVENUECAT_API_KEY_ANDROID = "PASTE-YOUR-API-KEY-HERE";
    private const string REVENUECAT_API_KEY_IOS = "PASTE-YOUR-API-KEY-HERE";

    public static string DatabaseName => DATABSE_NAME;
    public static string DataBasePath => Path.Combine(FileSystem.AppDataDirectory, DATABSE_NAME);
    public static string RevenueCatApiKeyAndroid => REVENUECAT_API_KEY_ANDROID;
    public static string RevenueCatApiKeyIos => REVENUECAT_API_KEY_IOS;
}
