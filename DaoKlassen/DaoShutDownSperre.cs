using System;
using System.Data.Odbc;
using System.Linq;
using TimeWorkUpdateLoader.Allgemeine_Klassen;

namespace TimeWorkUpdateLoader
{
    public class DaoShutDownSperre:Connector
    {

        public Int64 UserID { get; set; }

        public DateTime BeginnZeit { get; set; }

        public DateTime EndeZeit { get; set; }

        public Int16 Aktiv { get; set; }




        // Leerkonstruktor
        public DaoShutDownSperre()
        {

        }


        // Initialisierungskonstruktor
        public DaoShutDownSperre(long id)
            : base(id)
        {

            string sql = "SELECT UserID, BeginnZeit, EndeZeit, Aktiv FROM shutdownsperre WHERE id = " + "'" + id + "'";


            OdbcDataReader reader = database.AsReader(sql);
            while (reader.Read())
            {
                this.Id = id;

                if (!reader.IsDBNull(0)) { this.UserID = reader.GetInt64(0); }

                if (!reader.IsDBNull(1)) { this.BeginnZeit = reader.GetDateTime(1); }

                if (!reader.IsDBNull(2)) { this.EndeZeit = reader.GetDateTime(2); }

                if (!reader.IsDBNull(3)) { this.Aktiv = reader.GetInt16(3); }



            }


        }








        // Erstellt einen neuen Datensatz oder mach ein Update
        public bool Manipulate()
        {

            try
            {

                string sql = "";
                string sqlID = "";
                OdbcCommand command = new OdbcCommand();

                // Ist es ein Update oder eine Create?
                if (!AlreadyExists(this.Id))
                {


                    // Create
                    sql = "INSERT INTO shutdownsperre (UserID, BeginnZeit, EndeZeit,Aktiv) Values (?,?,?,?)";
                    sqlID = "SELECT MAX(id) FROM shutdownsperre";

                    command.CommandText = sql;

                    Tools.ODBCHelper(command, "UserID", this.UserID);
                    Tools.ODBCHelper(command, "BeginnZeit", this.BeginnZeit);
                    Tools.ODBCHelper(command, "EndeZeit", this.EndeZeit);
                    Tools.ODBCHelperTinyTintNotNull(command, "Aktiv", this.Aktiv);


                }
                else
                {
                    // Update
                    sql = "UPDATE shutdownsperre  SET " +
                                              "UserID      = ?,  " +
                                              "BeginnZeit  = ?,  " +
                                              "EndeZeit    = ?,  " +
                                              "Aktiv       = ?   " +
                         " WHERE shutdownsperre.id = '" + this.Id + "'";

                    command.CommandText = sql;


                    Tools.ODBCHelper(command, "UserID", this.UserID);
                    Tools.ODBCHelper(command, "BeginnZeit", this.BeginnZeit);
                    Tools.ODBCHelper(command, "EndeZeit", this.EndeZeit);
                    Tools.ODBCHelperTinyTintNotNull(command, "Aktiv", this.Aktiv);

                }

                database.Execute(command);

                // Bei einem Create holen wir uns die aktuelle ID Ab 
                if (!string.IsNullOrEmpty(sqlID))
                {
                    OdbcDataReader reader = database.AsReader(sqlID);
                    while (reader.Read())
                    {
                        this.Id = reader.GetInt64(0);
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Sperrzeit kann nicht manipuliert werden!\nFehler:" + ex.Message);
                Console.ReadKey(true);
                return false;
            }


        }



        // Prüft ob ein Dienstverhältnis bereits existiert
        public static bool AlreadyExists(long ID)
        {

            string sql = "SELECT 1 FROM shutdownsperre " +
                      " WHERE shutdownsperre.ID = '" + ID + "'";

            try
            {
                OdbcDataReader reader = database.AsReader(sql);
                while (reader.Read())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: " + ex.Message);
                Console.ReadKey(true);
                return true;
            }
        }


        // Gibt es für den Benutzer eine Sperrzeit?
        public static DaoShutDownSperre UserHasLockTime(long UserId)
        {

            string sql = "SELECT ID FROM shutdownsperre       " +
                         " WHERE shutdownsperre.UserID      = '" + UserId + "' " +
                         "   AND shutdownsperre.BeginnZeit <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                         "   AND shutdownsperre.EndeZeit   >= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

            try
            {
                OdbcDataReader reader = database.AsReader(sql);
                while (reader.Read())
                {
                    return new DaoShutDownSperre(reader.GetInt64(0));
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: " + ex.Message);
                Console.ReadKey(true);
                return null;
            }
        }




        // Löscht einen Datensatz aus der Datenbank
        public bool Delete()
        {

            string sql = "DELETE FROM shutdownsperre WHERE shutdownsperre.id = '" + this.Id + "'";

            try
            {
                OdbcCommand command = new OdbcCommand(sql);


                database.Execute(command);

                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine("Fehler: " + ex.Message);
                Console.ReadKey(true);
                return false;
            }


        }




    }
}
