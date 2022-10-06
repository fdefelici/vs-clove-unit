using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLoveUnitTestAdapter.Core
{
    public class XVersion
    {
        public static XVersion FromSemVerString(string semver)
        {
            string[] semVerArray = semver.Split('.');
            if (semVerArray.Length != 3) return null;

            int major = int.Parse(semVerArray[0]);
            int minor = int.Parse(semVerArray[1]);
            int patch = int.Parse(semVerArray[2]);

            return new XVersion(major, minor, patch);
        }

        public XVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public bool HasSameMinor(XVersion other)
        {
            if (Major != other.Major) return false;
            if (Minor != other.Minor) return false;
            return true;
        }

        public string AsMinorString()
        {
            return $"{Major}.{Minor}.X";
        }

        public string AsString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
    }
}
