using System;
using System.Data.Odbc;


namespace TimeWorkUpdateLoader
{
    public class DaoVersion : Connector
    {
        public DateTime Releasedatum { get; set; }
        public string Version { get; set; }
        public int Aktiv { get; set; }

        public string Bemerkung { get; set; }



        public DaoVersion()
        {
        }



        // Initialisierungskonstruktor
        public DaoVersion(long id)
            : base(id)
        {
            string sql = "SELECT Version, Releasedatum, Aktiv, Bemerkung FROM Versionen WHERE id = " + "'" + id + "'";


            OdbcDataReader reader = database.AsReader(sql);
            while (reader.Read())
            {
                this.Id = id;

                if (!reader.IsDBNull(0)) { this.Version = reader.GetString(0); }

                if (!reader.IsDBNull(1)) { this.Releasedatum = reader.GetDate(1); }

                if (!reader.IsDBNull(2)) { this.Aktiv = reader.GetInt32(2); }

                if (!reader.IsDBNull(3)) { this.Bemerkung = reader.GetString(3); }

            }
        }


       

        // Prüft ob ein Dienstverhältnis bereits existiert
        public static DaoVersion GetNewestVersion()
        {

            string sql = "SELECT ID FROM Versionen " +
                      " WHERE Versionen.Aktiv = '" + 1 + "'";

            try
            {

                OdbcDataReader reader = database.AsReader(sql);
                while (reader.Read())
                {
                    DaoVersion version = new DaoVersion(reader.GetInt64(0));
                    return version;
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Auslesen der neuesten Version aus der Datenbank!\nFehler: " + ex.Message);
                Console.ReadKey(true);
                return null;

            }


        }




    }
}
