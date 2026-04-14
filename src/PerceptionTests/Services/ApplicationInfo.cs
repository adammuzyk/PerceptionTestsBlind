using System.Reflection;

namespace PerceptionTests.Services
{
    public static class ApplicationInfo
    {
        public static string DisplayName => "Perception Tests";

        public static string Version => typeof(ApplicationInfo).Assembly.GetName().Version != null
            ? typeof(ApplicationInfo).Assembly.GetName().Version.ToString(3)
            : "unknown";
    }
}
