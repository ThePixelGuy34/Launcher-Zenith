using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML.Class
{
    public static class AppVersion
    {
        public const string Current = "0.93.PreReleaseDebug";
        public static bool IsPreRelease => Current.Contains("-");
        public static string BuildNumber => "139596034";
        public static string FullVersion => $"{Current}+{BuildNumber}";
    }
}
