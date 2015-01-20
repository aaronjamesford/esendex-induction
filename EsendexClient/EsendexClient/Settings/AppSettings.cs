using System.Web.Configuration;

namespace EsendexClient.Settings
{
    public static class AppSettings
    {
        public static string EsendexEndpoint { get { return WebConfigurationManager.AppSettings["EsendexEndpoint"]; } }
        public static string EsendexFormPostEndpoint { get { return WebConfigurationManager.AppSettings["EsendexFormPostEndpoint"]; } }
    }
}