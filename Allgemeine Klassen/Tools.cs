using System;
using System.Collections.Generic;
using System.Data.Odbc;
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


        // Convertiert eine Zahl aus einem String
        public static Int32 ConvertStringToInt32(string Str)
        {
            try
            {

                Int32 Returnvalue = 0;

                Int32.TryParse(Str, out Returnvalue);

                return Returnvalue;


            }
            catch 
            {
                return 0;
            }


        }

        // ODBC Helper Klassen 
        // -------------------------------------------------------------------------------------------------------------
        public static void ODBCHelper(OdbcCommand command, string Tag, int value)
        {
            if (value == 0) { command.Parameters.Add(Tag, OdbcType.Int).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.Int).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, decimal value)
        {
            if (value == 0) { command.Parameters.Add(Tag, OdbcType.Decimal).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.Decimal).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, long value)
        {
            if (value == 0) { command.Parameters.Add(Tag, OdbcType.BigInt).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.BigInt).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, string value)
        {
            if (string.IsNullOrEmpty(value)) { command.Parameters.Add(Tag, OdbcType.VarChar).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.VarChar).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, DateTime value)
        {
            if (value == null || value == DateTime.MinValue) { command.Parameters.Add(Tag, OdbcType.Date).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.DateTime).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, DateTime value, bool istDatetimeWithTimecomponent)
        {
            if (value == null || value == DateTime.MinValue) { command.Parameters.Add(Tag, OdbcType.DateTime).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.DateTime).Value = value; }
        }

        public static void ODBCHelperForDatetime(OdbcCommand command, string Tag, DateTime value)
        {
            if (value == null || value == DateTime.MinValue) { command.Parameters.Add(Tag, OdbcType.DateTime).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.DateTime).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, Int16 value)
        {
            if (value == 0) { command.Parameters.Add(Tag, OdbcType.TinyInt).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.TinyInt).Value = value; }
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, double value)
        {
            if (value == 0) { command.Parameters.Add(Tag, OdbcType.Double).Value = DBNull.Value; }
            else { command.Parameters.Add(Tag, OdbcType.Double).Value = value; }
        }

        public static void ODBCHelperTinyTintNotNull(OdbcCommand command, string Tag, Int16 value)
        {
            command.Parameters.Add(Tag, OdbcType.TinyInt).Value = value;
        }

        public static void ODBCHelperZeroInsteadOfNull(OdbcCommand command, string Tag, Int32 value)
        {
            command.Parameters.Add(Tag, OdbcType.Int).Value = value;
        }

        public static void ODBCHelperZeroInsteadOfNull(OdbcCommand command, string Tag, decimal value)
        {
            command.Parameters.Add(Tag, OdbcType.Decimal).Value = value;
        }

        public static void ODBCHelperZeroInsteadOfNull(OdbcCommand command, string Tag, double value)
        {
            command.Parameters.Add(Tag, OdbcType.Double).Value = value;
        }

        public static void ODBCHelper(OdbcCommand command, string Tag, Boolean value)
        {
            command.Parameters.Add(Tag, OdbcType.Bit).Value = value;
        }
        // -------------------------------------------------------------------------------------------------------------


    }
}
