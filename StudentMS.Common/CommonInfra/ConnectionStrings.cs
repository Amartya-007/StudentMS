using Microsoft.Extensions.Configuration;

namespace StudentMS.Common.CommonInfra
{
    public static class ConnectionStrings
    {
        private static IConfiguration? _config;

        public static void Initialize(IConfiguration config)
        {
            _config = config;
        }

        public static string MainDB =>
            _config?.GetConnectionString("MainDB") ?? string.Empty;
    }
}
