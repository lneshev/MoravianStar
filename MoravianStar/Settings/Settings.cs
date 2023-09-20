using System;

namespace MoravianStar.Settings
{
    public class Settings
    {
        public static Type DefaultDbContextType { get; set; }
        public static Type StringResourceTypeForEnums { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
    }
}