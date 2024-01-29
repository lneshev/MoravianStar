using System;
using System.Reflection;

namespace MoravianStar.Settings
{
    public class Settings
    {
        public static Type DefaultDbContextType { get; set; }
        public static Type StringResourceTypeForEnums { get; set; }
        public static Assembly AssemblyForEnums { get; set; }
    }
}