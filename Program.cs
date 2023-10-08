using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using TimeWorkUpdateLoader.Allgemeine_Klassen;
using TimeWorkUpdateLoader.Helper;

namespace TimeWorkUpdateLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {

            try
            {

                // Notwendige Variablen
                // ---------------------------------
                Database datenbank = null;
                bool UpdateIsNecessary = false;
                string Installationspfad = "";
                string NetzwerkPfad = "";
                string CurrentVersion = "";
                Process firstProc = null;
                // ---------------------------------


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


                // Aktuelle Version
                // --------------------------------------------------------------------------------------------------
                CurrentVersion = ini.IniReadValue("Version", "CurrentVersion");
                if (String.IsNullOrEmpty(CurrentVersion))
                {
                    Console.WriteLine("Aktuelle Programmversion von TimeWork in der Config.ini nicht gefunden!");
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


                // Installationspfad
                // ------------------------------------------------------------------------
                Installationspfad = Tools.CorrectPathMissingBackslash(ini.IniReadValue("System_Folders", "Installationspfad"));
                if (!Directory.Exists(Installationspfad))
                {
                    Console.WriteLine("Der Installationspfad für TimeWork " + Installationspfad + " existiert nicht!");
                    Console.WriteLine("Bitte kontrollieren sie die Config.INI");
                    Console.WriteLine("Programm wird beendet!");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
                // ------------------------------------------------------------------------


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

                    // Pfad für die neuen Files auslesen
                    // ------------------------------------------------------------------------
                    NetzwerkPfad = Tools.CorrectPathMissingBackslash(ini.IniReadValue("System_Folders", "Netzwerkpfad"));
                    if (!Directory.Exists(NetzwerkPfad)) 
                    {
                        Console.WriteLine("Der Netzwerkpfad für die neuen Files " + NetzwerkPfad + " existiert nicht!");
                        Console.WriteLine("Bitte kontrollieren sie die Config.INI");
                        Console.WriteLine("Programm wird beendet!");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }
                    // ------------------------------------------------------------------------


                    // Liste aller Files erstellen
                    // ------------------------------------------------------------------------
                    List<FileObject> Files = new List<FileObject>();
                    DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo(NetzwerkPfad);
                    foreach (FileInfo f in ParentDirectory.GetFiles())
                    {
                        if (f.Extension == ".txt") { continue; }

                        FileObject obj = new FileObject();
                        obj.SourceFile = f.FullName;
                        obj.DestinationFile = Installationspfad + f.Name;

                        Files.Add(obj);
                    }
                    // ------------------------------------------------------------------------


                    // User Infos
                    // ------------------------------------------------------------------------
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("       UPDATE IST NÖTIG!         ");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("                                 ");
                    Console.WriteLine("ALTE VERSION: " + CurrentVersion  );
                    Console.WriteLine("NEUE VERSION: " + version.Version );
                    Console.WriteLine("                                 ");
                    Console.WriteLine("NEUE FILES:                      ");

                    foreach (FileObject File in Files) 
                    {
                        Console.WriteLine(File.SourceFile);
                    }
                    Console.WriteLine("                                 ");

                    if (!string.IsNullOrEmpty(version.Bemerkung)) 
                    {
                        Console.WriteLine("BEMERKUNG: " + version.Bemerkung );
                        Console.WriteLine("                                ");
                    }

                    Console.WriteLine("Drücken sie j(a) um das Update zu installieren!\nBeenden sie vorher umbedingt alle laufenden TimeWork Sessions!");
                    if (Console.ReadKey(true).Key != ConsoleKey.J) 
                    {
                        Console.WriteLine("                                ");
                        Console.WriteLine("Das Programmupdate wird abgebrochen!!!");
                        Console.WriteLine("Drücken sie j(a) um die alte Version von TimeWork zu starten!\nDies wird ausdrücklich nicht empfohlen!!!");
                        if (Console.ReadKey(true).Key != ConsoleKey.J)
                        {
                            Environment.Exit(0);
                        }
                        else 
                        {
                            firstProc = new Process();
                            firstProc.StartInfo.FileName = Installationspfad + "AMS_e.exe";
                            firstProc.EnableRaisingEvents = true;
                            firstProc.Start();
                            Environment.Exit(0);
                        }
                            
                    }
                    // ------------------------------------------------------------------------


                    // File Aktionen
                    // ------------------------------------------------------------------------
                    Console.WriteLine("                                ");
                    Console.WriteLine("Update Start!                   ");

                    foreach (FileObject file in Files)
                    {
                        Console.WriteLine(" ");

                        if (File.Exists(file.DestinationFile)) 
                        {
                            Console.WriteLine("Altes File wird gelöscht: " + file.DestinationFile);
                            File.Delete(file.DestinationFile);
                        }

                        // Neue Version kopieren
                        Console.WriteLine("Kopie von: " + file.SourceFile);
                        Console.WriteLine("Kopie auf: " + file.DestinationFile);
                        File.Copy(file.SourceFile, file.DestinationFile, true);
                    }
                    // ------------------------------------------------------------------------


                    ini.IniWriteValue("Version", "CurrentVersion", version.Version);

                    Console.WriteLine("\n---------------------------------");
                    Console.WriteLine("       UPDATE IST FERTIG!        ");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine(" ");
                    Console.WriteLine("Drücken sie eine beliebige Taste um fortzufahren!");
                    Console.ReadKey(true);

                }
                // --------------------------------------------------------------------------------------------------

                // Programm Start
                // --------------------------------------------------------------------------------------------------
                firstProc = new Process();
                firstProc.StartInfo.FileName = Installationspfad + "AMS_e.exe";
                firstProc.EnableRaisingEvents = true;
                firstProc.Start();
                // --------------------------------------------------------------------------------------------------


            }
            catch (Exception ex)
            {
                Console.WriteLine("Programm kann nicht gestartet werden!\nFehler:" + ex.Message);
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            finally 
            {
                Environment.Exit(0);
            }



        }


    }
}
