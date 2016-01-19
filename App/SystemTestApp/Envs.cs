namespace ThomsonReuters.Eikon.SystemTestApp
{
    static internal class Envs
    {
        static internal readonly string Local = "local";
        static internal readonly string Dev = "dev";
        static internal readonly string Alpha = "alpha";
        static internal readonly string Beta = "beta";
        static internal readonly string Prod = "prod";
    }

    static internal class DataCenters
    {
        internal const string Local = "local";
        internal const string Dev = "dev";
        internal const string Alpha = "alpha";
        internal const string Beta = "beta";
        internal const string HDCP = "hdcp";
        internal const string NTCP = "ntcp";
        internal const string DTCP = "dtcp";
        internal const string STCP = "stcp";
    }

    static internal class Env
    {
        internal static string CurrentEnv { get; private set; }
        internal static string DataCenter { get; private set; }
        internal static string Version { get; private set; }

        static Env()
        {
            string asEnv = System.Environment.GetEnvironmentVariable("AS_ENV") ?? "";

            DataCenter = asEnv;
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            switch (asEnv.ToLower())
            {
                case "dev":
                    CurrentEnv = Envs.Dev;
                    break;
                case "alpha":
                    CurrentEnv = Envs.Alpha;
                    break;
                case "ppe1":
                    CurrentEnv = Envs.Beta;
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    CurrentEnv = Envs.Prod;
                    break;
                case "local":
                case "appengine":
                case "temp":
                    CurrentEnv = Envs.Local;
                    break;
                default:
                    CurrentEnv = "Unknown";
                    break;
            }
        }
    }
}
