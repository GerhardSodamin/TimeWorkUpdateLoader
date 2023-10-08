using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeWorkUpdateLoader.Allgemeine_Klassen
{
    public class Tools
    {

        public static string CorrectPathMissingBackslash(string Path)
        {
            if (String.IsNullOrEmpty(Path)) { return ""; }

            if (Path.Substring(Path.Length - 1) == @"\") { return Path; }
            else { return Path + @"\"; }
        }

    }
}
