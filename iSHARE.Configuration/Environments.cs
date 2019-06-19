namespace iSHARE.Configuration
{
    public static class Environments
    {
        public const string Test = "Test";
        public const string Live = "Live";
        public const string QaLive = "QaLive";
        public const string QaTest = "QaTest";
        public const string QaEnvironmentPrefix = "Qa";
        public const string Development = "Development";

        public class Variables
        {
            public const string AspNetCoreDetailedErrors = "ASPNETCORE_DETAILEDERRORS";
            public const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        }
    }
}
