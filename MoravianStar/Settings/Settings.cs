using Microsoft.EntityFrameworkCore;
using System;

namespace MoravianStar.Settings
{
    public class Settings
    {
        public static DbContext DefaultDbContext { get; set; }
        public static Type StringResourceTypeForEnums { get; set; }
    }
}