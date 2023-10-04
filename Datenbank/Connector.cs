using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeWorkUpdateLoader
{
    public abstract class Connector
    {

        private static Database _database;


        protected static Database database { get { return _database; } }

        public long Id
        {
            get;
            set;
        }

        public Connector()
        {

        }
        public Connector(long id)
        {

        }


        public static string PLZSuche(string plz)
        {
            string sql = "Select ort from Postleitzahlen where plz = '" + plz + "'";
            return database.GetString(sql);
        }

        public static string OrtSuche(string plz)
        {
            string sql = "Select plz from Postleitzahlen where Ort = '" + plz + "'";
            return database.GetString(sql);
        }


        public static void Initialize(Database database)
        {
            _database = database;

        }
    }
}
