using System;
using System.Diagnostics;
using System.IO;


namespace TimeWorkUpdateLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Database datenbank = null;
                bool UpdateIsNecessary = false;
                string Installationspfad = "";
                string NetzwerkPfad = "";
                string OriginalPfad = "";
                string PfadNeueVersion = "";
                string CurrentVersion = "";
              
                // INI Einladen
                // --------------------------------------------------------------------------------------------------
                string path = @"C:\CRM\" + @"Config.ini";

                if (!File.Exists(path))
                {
                    Console.WriteLine(String.Format("Die Config.ini konnte unter {0} nicht gefunden werden!", path));
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }

                IniFile ini = new IniFile(path);
                // --------------------------------------------------------------------------------------------------


                // Installationspfad
                // --------------------------------------------------------------------------------------------------
                Installationspfad = ini.IniReadValue("System_Folders", "Installationspfad");

                if (String.IsNullOrEmpty(Installationspfad))
                {
                    Console.WriteLine("Installationspfad von TimeWork in der Config.ini ist leer!");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }

                OriginalPfad = Installationspfad + @"\AMS_e.exe";
             
                if (!File.Exists(OriginalPfad))
                {
                    Console.WriteLine("Das Startprogramm von TimeWork aus der Config.ini existiert nicht!\nPfad: " + OriginalPfad);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                // --------------------------------------------------------------------------------------------------

                // Aktuelle Version
                // --------------------------------------------------------------------------------------------------
                CurrentVersion = ini.IniReadValue("Version", "CurrentVersion");

                if (String.IsNullOrEmpty(CurrentVersion))
                {
                    Console.WriteLine("Aktuelle Version von TimeWork in der Config.ini nicht gefunden!");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                // --------------------------------------------------------------------------------------------------


                // Datenbank initialisieren
                // --------------------------------------------------------------------------------------------------
                try
                {
                    datenbank = Database.Instance;  // new Database(this._host , this._datenbankname , this._user , this._passwort );
                    Connector.Initialize(datenbank);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Datenbank Verbindungsproblem!\nFehler: " + ex.Message);
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                // --------------------------------------------------------------------------------------------------
               
                // Muss ein Update gemacht werden?
                // --------------------------------------------------------------------------------------------------
                DaoVersion version = DaoVersion.GetNewestVersion();
                if (version == null) 
                {
                    Console.WriteLine("Die aktuelle TimeWork - Version kann nicht aus der Datenbank gelesen werden!");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }

                // Bei Versionsunterschied machen wir ein Update
                if (version.Version != CurrentVersion) { UpdateIsNecessary = true; }
                // --------------------------------------------------------------------------------------------------



                // Updateprozess
                // --------------------------------------------------------------------------------------------------
                if (UpdateIsNecessary) 
                {

                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("       UPDATE IST NÖTIG!         ");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("                                 ");
                    Console.WriteLine("ALTE VERSION: " + CurrentVersion  );
                    Console.WriteLine("NEUE VERSION: " + version.Version );
                    Console.WriteLine("                                 ");

                    if (!string.IsNullOrEmpty(version.Bemerkung)) 
                    {
                        Console.WriteLine("BEMERKUNG: " + version.Bemerkung );
                        Console.WriteLine("                                ");
                    }

                    Console.WriteLine("Drücken sie j um das Update zu installieren!\nBeenden sie vorher umbedingt alle laufenden TimeWork Sessions!");
                    if (Console.ReadKey(true).Key != ConsoleKey.J) 
                    {
                        Console.WriteLine("Update wird abgebrochen!\nProgramm wird beendet");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }


                    NetzwerkPfad = ini.IniReadValue("System_Folders", "Netzwerkpfad");
                    
                    if (String.IsNullOrEmpty(NetzwerkPfad))
                    {
                        Console.WriteLine("Der Zielpfad für die neue Version ist leer!");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }

                    PfadNeueVersion = NetzwerkPfad + @"\AMS_e.exe";

                    if (!File.Exists(PfadNeueVersion))
                    {
                        Console.WriteLine("Der Zielpfad für die neue Version stimmt nicht!\nPfad: " + PfadNeueVersion + @"\AMS_e.exe");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }

                    // Altes File löschen
                    File.Delete(OriginalPfad);

                    // Neue Version kopieren
                    File.Copy(PfadNeueVersion, OriginalPfad, true);

                    ini.IniWriteValue("Version", "CurrentVersion", version.Version);

                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("       UPDATE IST FERTIG!        ");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine(" ");
                    Console.WriteLine("Drücken sie eine beliebige Taste um fortzufahren!");
                    Console.ReadKey(true);

                }
                // --------------------------------------------------------------------------------------------------



                // Programm Start
                // --------------------------------------------------------------------------------------------------
                Process firstProc = new Process();
                firstProc.StartInfo.FileName = Installationspfad + @"\AMS_e.exe";
                firstProc.EnableRaisingEvents = true;
                firstProc.Start();
                // --------------------------------------------------------------------------------------------------

                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Programm kann nicht gestartet werden!\nFehler:" + ex.Message);
                Console.ReadKey(true);
                Environment.Exit(0);
            }

        }
    }
}
