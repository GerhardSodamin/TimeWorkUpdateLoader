using System;
using System.Data.Odbc;
using System.IO;


namespace TimeWorkUpdateLoader
{
    public class Database
    {
        private OdbcConnection _connection;

        private string _host;
        private string _datenbankname;
        private string _user;
        private string _passwort;
        private string _driver;

        private static Database instance;



        public static Database Instance
        {
            get
            {
                // Gibt es überhaupt eine gültige Instanz?
                if (instance == null)
                {
                    instance = new Database();
                }

                // Wenn ja ist die Datenbankverdindung noch gültig oder muß sie erneuert werden?
                if (!instance.CheckConntection())
                {
                    instance = new Database();
                }

                return instance;
            }
        }


        private bool CheckConntection()
        {
            if (_connection.State == System.Data.ConnectionState.Broken
                || _connection.State == System.Data.ConnectionState.Closed)
            {
                return false;
            }

            return true;
        }


        private Database()
        {

            string connectionString = "";

            try
            {

                // Sind alle Ini Files vorhanden? 
                string path = @"C:\CRM\" + @"Config.ini";

                if (!File.Exists(path))
                {
                    Environment.Exit(0);
                }

                IniFile ini = new IniFile(path);

                this._host = ini.IniReadValue("Database_Connection", "Server");
                this._datenbankname = ini.IniReadValue("Database_Connection", "database");
                this._user = ini.IniReadValue("Database_Connection", "user");
                this._passwort = ini.IniReadValue("Database_Connection", "password");
                this._driver = ini.IniReadValue("Database_Connection", "treiber");

                if (String.IsNullOrEmpty(this._host) || String.IsNullOrEmpty(_datenbankname) || String.IsNullOrEmpty(_user))
                {
                    Environment.Exit(0);
                }

                if (string.IsNullOrEmpty(this._driver)) { this._driver = "{MySQL ODBC 8.0 ANSI Driver}"; }

                connectionString =
                    "Provider=MSDASQL;" +
                    "Driver=" + this._driver + ";" +
                    "Server=" + this._host + ";" +
                    "Database=" + this._datenbankname + ";" +
                    "User=" + this._user + ";" +
                    "Password=" + this._passwort + ";" +
                    "CharSet=latin1;" +
                    "Option=3;";


                _connection = new OdbcConnection(connectionString);
                _connection.Open();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message + "\n\n" +
                    connectionString);
            }
        }


        public OdbcDataReader AsReader(string sql)
        {
            try
            {
                OdbcCommand command = new OdbcCommand(sql, _connection);
                return command.ExecuteReader();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message + "\n\n" + sql);
            }
        }


        public void Execute(string sql)
        {
            OdbcCommand command = null;

            try
            {
                command = new OdbcCommand(sql, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message + "\n\n" + sql);
            }
            finally
            {
                command.Dispose();
            }
        }


        public void Execute(OdbcCommand command)
        {
            try
            {
                command.Connection = _connection;
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message + "\n\n" + command.CommandText);
            }
            finally
            {
                command.Dispose();
            }
        }


        public bool Exists(string sql)
        {
            OdbcCommand command = null;
            OdbcDataReader reader = null;

            try
            {
                command = new OdbcCommand(sql, _connection);
                reader = command.ExecuteReader();
                bool exist = reader.Read();

                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n\n" + sql);
            }
            finally
            {
                command.Dispose();
                reader.Close();
                reader.Dispose();
            }
        }


        public string GetString(string sql)
        {
            OdbcCommand command = new OdbcCommand(sql, _connection);
            OdbcDataReader reader = command.ExecuteReader();

            return (reader.Read() ? reader.GetString(0) : null);
        }

    }
}
