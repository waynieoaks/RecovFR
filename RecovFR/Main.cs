using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using Rage;

namespace RecovFR
{
    public static class EntryPoint

    {
        // Settings variables    
        public static string XMLpath = "Plugins\\RecovFR.xml";
        public static Boolean IsLoaded = false;
        public static Keys BackupKey { get; set; }
        public static Keys BackupModifierKey { get; set; }
        public static Keys RestoreKey { get; set; }
        public static Keys RestoreModifierKey { get; set; }
        public static Boolean AutoBackups { get; set; }
        public static Byte AutoBackupInt { get; set; }
        public static Boolean ShowNotifications { get; set; }
                
        // Object variables
        public static Vehicle GetVehicle { get; set; }

        // Vehicle variables
        public static Boolean InVeh { get; set; }
        public static String MyVeh { get; set; }
        public static Vector3 MyVehLoc { get; set; }
        public static float MyVehLocX { get; set; }
        public static float MyVehLocY { get; set; }
        public static float MyVehLocZ { get; set; }
        public static float MyVehLocH { get; set; }

        public static Int32 MyVehPriR { get; set; }
        public static Int32 MyVehPriG { get; set; }
        public static Int32 MyVehPriB { get; set; }

        public static Int32 MyVehSecR { get; set; }
        public static Int32 MyVehSecG { get; set; }
        public static Int32 MyVehSecB { get; set; }

        public static float  MyVehDirt { get; set; }
        public static String MyVehPlate { get; set; }
        public static LicensePlateStyle MyVehPlateStyle { get; set; }
        
        // Character variables
        public static Vector3 MyLoc { get; set; }
        public static float MyLocX { get; set; }
        public static float MyLocY { get; set; }
        public static float MyLocZ { get; set; }
        public static float MyLocH { get; set; }
        public static Int32 MyWanted { get; set; }
        public static Int32 MyHealth { get; set; }
        public static Int32 MyArmor { get; set; }

        // Word variables
        public static TimeSpan MyTime { get; set; }

        //Initialization of the plugin.
        public static void Main()
        {
            //This is saying that when our OnDuty status is changed, it calls for the code to call private static void OnOnDutyStateChangedHandler near the bottom of this page.
            //   Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;

            Game.LogTrivial("Loading RecovFR settings...");
            LoadValuesFromIniFile();

            //This will show in the RagePluginHook.log as "Example Plugin 1.0.0.0 has been initialised." 
            Game.LogTrivial("RecovFR " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");

            //We have to make sure they are actually on duty so the code can do its work, so we use an "if" statement.
            if (IsLoaded == false)
            {
                IsLoaded = true;
                //This shows a notification at the bottom left, above the minimap, of your screen when you go on duty, stating exactly what's in the quotation marks.
                Game.DisplayNotification("RecovFR " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has loaded.");

                GameFiber.StartNew(delegate ()
                {
                    while (true)
                    {
                        GameFiber.Yield();
                        if (Game.IsKeyDown(BackupKey) &&
                            (Game.IsKeyDownRightNow(BackupModifierKey) ||
                             BackupModifierKey == Keys.None))
                        {
                            DoBackup();
                            GameFiber.Sleep(1000);
                        }
                        if (Game.IsKeyDown(RestoreKey) &&
                            (Game.IsKeyDownRightNow(RestoreModifierKey) ||
                             RestoreModifierKey == Keys.None))
                        {
                            DoRestore();
                            GameFiber.Sleep(1000);
                        }
                    }
                });

                // Auto backup fiber
                if (AutoBackups == true && AutoBackupInt >= 1 && AutoBackupInt <= 60)
                {
                    Int32 time = ((AutoBackupInt * 1000) * 60);
                    string timeString = time.ToString();

                    GameFiber.StartNew(delegate
                    {
                        while (true)
                        {
                            Game.LogTrivial("RecovFR: AutoBackup initiated");
                            DoBackup();
                            GameFiber.Sleep(time);
                        }
                    });

                }

            }
        }

        private static void LoadValuesFromIniFile()
        {

            string path = "Plugins\\RecovFR.ini";
            InitializationFile ini = new InitializationFile(path);
            ini.Create();

            try
            {
                BackupKey = ini.ReadEnum<Keys>("Keybindings", "BackupKey", Keys.F12);
                BackupModifierKey = ini.ReadEnum<Keys>("Keybindings", "BackupModifierKey", Keys.ControlKey);
                RestoreKey = ini.ReadEnum<Keys>("Keybindings", "RestoreKey", Keys.F12);
                RestoreModifierKey = ini.ReadEnum<Keys>("Keybindings", "RestoreModifierKey", Keys.Menu);
                AutoBackups = ini.ReadBoolean("Features", "AutoBackups", false);
                AutoBackupInt = ini.ReadByte("Features", "AutoBackupInt", 60);
                ShowNotifications = ini.ReadBoolean("Features", "ShowNotifications", false);
                Game.LogTrivial("RecovFR: INI file read successfully.");
            }
            catch (Exception e)
            {
                BackupKey = Keys.F5;
                BackupModifierKey = Keys.ControlKey;
                RestoreKey = Keys.F5;
                RestoreModifierKey = Keys.Menu;
                AutoBackups = false;
                AutoBackupInt = 60;
                ShowNotifications = false;
                Game.LogTrivial("RecovFR: ~r~~h~Error~s~ reading INI file.");
                Game.LogTrivial(e.ToString());
                Game.DisplayNotification("RecovFR: ~r~~h~Error~s~ reading INI file, setting default values.");

            }

        }

        private static void DoBackup()
        {

            // Am I in a vehicle?
            if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
            {
                InVeh = true;
                try
                {
                    GetVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                } catch {
                    GetVehicle = null;
                }
            } else {
                InVeh = false;
                try
                {
                    GetVehicle = Game.LocalPlayer.Character.LastVehicle;
                } catch {
                    GetVehicle = null;
                }
            }

            // Get vehicle parameters
            if (GetVehicle == null)
            {
                MyVeh = "None";
                MyVehLocX = 0;
                MyVehLocY = 0;
                MyVehLocZ = 0;
                MyVehLocH = 0;
                MyVehPriR = 0;
                MyVehPriG = 0;
                MyVehPriB = 0;
                MyVehSecR = 0;
                MyVehSecG = 0;
                MyVehSecB = 0;
                MyVehDirt = 0;
                MyVehPlate = "";
            }  else {
                MyVeh = GetVehicle.Model.Name;
                MyVehLocX = GetVehicle.Position.X;
                MyVehLocY = GetVehicle.Position.Y;
                MyVehLocZ = GetVehicle.Position.Z;
                MyVehLocH = GetVehicle.Heading;
                MyVehPriR = GetVehicle.PrimaryColor.R;
                MyVehPriG = GetVehicle.PrimaryColor.G;
                MyVehPriB = GetVehicle.PrimaryColor.B;
                MyVehSecR = GetVehicle.SecondaryColor.R;
                MyVehSecG = GetVehicle.SecondaryColor.G;
                MyVehSecB = GetVehicle.SecondaryColor.B;
                MyVehDirt = GetVehicle.DirtLevel;
                MyVehPlate = GetVehicle.LicensePlate;
                MyVehPlateStyle = GetVehicle.LicensePlateStyle;
            }

            // Get character parameters
            MyLocX = Game.LocalPlayer.Character.Position.X;
            MyLocY = Game.LocalPlayer.Character.Position.Y;
            MyLocZ = Game.LocalPlayer.Character.Position.Z;
            MyLocH = Game.LocalPlayer.Character.Heading;
            MyWanted = Game.LocalPlayer.WantedLevel;
            MyHealth = Game.LocalPlayer.Character.Health;
            MyArmor = Game.LocalPlayer.Character.Armor;


            MyTime = World.TimeOfDay;

            // Now Write backup to XML file for later use
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            // XmlWriter xmlWriter = XmlWriter.Create(XMLpath);
            using (XmlWriter xmlWriter = XmlWriter.Create(XMLpath, xmlWriterSettings))
            {
                // Start the XML
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Backup");
                xmlWriter.WriteStartElement("BackedUp");
                xmlWriter.WriteString(DateTime.Now.ToString("F"));
                xmlWriter.WriteEndElement();
                
                // Vehicle elements
                xmlWriter.WriteStartElement("InVeh");
                xmlWriter.WriteString(InVeh.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVeh");
                xmlWriter.WriteString(MyVeh);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehLocX");
                xmlWriter.WriteString(MyVehLocX.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehLocY");
                xmlWriter.WriteString(MyVehLocY.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehLocZ");
                xmlWriter.WriteString(MyVehLocZ.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehLocH");
                xmlWriter.WriteString(MyVehLocH.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehPriR");
                xmlWriter.WriteString(MyVehPriR.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehPriG");
                xmlWriter.WriteString(MyVehPriG.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehPriB");
                xmlWriter.WriteString(MyVehPriB.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehSecR");
                xmlWriter.WriteString(MyVehSecR.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehSecG");
                xmlWriter.WriteString(MyVehSecG.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehSecB");
                xmlWriter.WriteString(MyVehSecB.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehDirt");
                xmlWriter.WriteString(MyVehDirt.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehPlate");
                xmlWriter.WriteString(MyVehPlate);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyVehPlateStyle");
                xmlWriter.WriteString(MyVehPlateStyle.ToString());
                xmlWriter.WriteEndElement();

                // Character elements
                xmlWriter.WriteStartElement("MyLocX");
                xmlWriter.WriteString(MyLocX.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyLocY");
                xmlWriter.WriteString(MyLocY.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyLocZ");
                xmlWriter.WriteString(MyLocZ.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyLocH");
                xmlWriter.WriteString(MyLocH.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyWanted");
                xmlWriter.WriteString(MyWanted.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyHealth");
                xmlWriter.WriteString(MyHealth.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("MyArmor");
                xmlWriter.WriteString(MyArmor.ToString());
                xmlWriter.WriteEndElement();

                // World elements
                xmlWriter.WriteStartElement("MyTime");
                xmlWriter.WriteString(MyTime.ToString());
                xmlWriter.WriteEndElement();
               
                // End the XML
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }

            Command_Notification("RecovFR: Backup complete...");
            return;
        }

        private static void DoRestore()
        {
            // Check if there is an xml file
            if (File.Exists(XMLpath))
            {
                // Read some XML

                XDocument xdocument = XDocument.Load(XMLpath);
                IEnumerable<XElement> backups = xdocument.Elements();
                foreach (var backup in backups)
                {
                    // Vehicle elements
                    InVeh = Boolean.Parse(backup.Element("InVeh").Value);
                    MyVeh = backup.Element("MyVeh").Value;
                    MyVehLocX = float.Parse(backup.Element("MyVehLocX").Value);
                    MyVehLocY = float.Parse(backup.Element("MyVehLocY").Value);
                    MyVehLocZ = float.Parse(backup.Element("MyVehLocZ").Value);
                    MyVehLocH = float.Parse(backup.Element("MyVehLocH").Value);
                    MyVehPriR = Int32.Parse(backup.Element("MyVehPriR").Value);
                    MyVehPriG = Int32.Parse(backup.Element("MyVehPriG").Value);
                    MyVehPriB = Int32.Parse(backup.Element("MyVehPriB").Value);
                    MyVehSecR = Int32.Parse(backup.Element("MyVehSecR").Value);
                    MyVehSecG = Int32.Parse(backup.Element("MyVehSecG").Value);
                    MyVehSecB = Int32.Parse(backup.Element("MyVehSecB").Value);
                    MyVehDirt = float.Parse(backup.Element("MyVehDirt").Value);
                    MyVehPlate = backup.Element("MyVehPlate").Value;

                    // Character Elements
                    MyLocX = float.Parse(backup.Element("MyLocX").Value);
                    MyLocY = float.Parse(backup.Element("MyLocY").Value);
                    MyLocZ = float.Parse(backup.Element("MyLocZ").Value);
                    MyLocH = float.Parse(backup.Element("MyLocH").Value);
                    MyWanted = Int32.Parse(backup.Element("MyWanted").Value);
                    MyHealth = Int32.Parse(backup.Element("MyHealth").Value);
                    MyArmor = Int32.Parse(backup.Element("MyArmor").Value);

                    // World Elements
                    MyTime = TimeSpan.Parse(backup.Element("MyTime").Value);
                }

                if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                {
                    //Get out vehicle to pevent taking it with us
                    GetVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    Game.LocalPlayer.Character.Tasks.LeaveVehicle(GetVehicle, LeaveVehicleFlags.WarpOut).WaitForCompletion();            
                }

                // Now lets run the restore
                GetVehicle = null;

                MyVehLoc = new Vector3(MyVehLocX, MyVehLocY, MyVehLocZ);
                if (MyVeh != "None")
                { 
                    try
                    {
                        GetVehicle = new Vehicle(MyVeh, MyVehLoc, MyVehLocH);
                    } catch (Exception e) {
                        InVeh = false;  // Vehicle not restored, so forget warp to vehicle
                        Game.LogTrivial("RevovFR: --------------------------------------");
                        Game.LogTrivial("RecovFR: Error restoring vehicle.");
                        Game.LogTrivial("Expected vehicle: " + MyVeh);
                        Game.LogTrivial(e.ToString());
                    }
                }

                // Try and set vehicle parameters
                if (MyVeh != "None") 
                {
                    try
                    {
                        GetVehicle.PrimaryColor = Color.FromArgb(MyVehPriR, MyVehPriG, MyVehPriB);
                        GetVehicle.SecondaryColor = Color.FromArgb(MyVehSecR, MyVehSecG, MyVehSecB);
                        GetVehicle.DirtLevel = MyVehDirt;
                        GetVehicle.LicensePlate = MyVehPlate;
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("RevovFR: --------------------------------------");
                        Game.LogTrivial("RecovFR: Error restoring vehicle perameters.");
                        Game.LogTrivial(e.ToString());
                    }
                }

                // Restore character
                MyLoc = new Vector3(MyLocX, MyLocY, MyLocZ);
                World.TeleportLocalPlayer(MyLoc, false); // Set to true if reports of falling through world
                Game.LocalPlayer.WantedLevel = MyWanted;
                Game.LocalPlayer.Character.Health = MyHealth;
                Game.LocalPlayer.Character.Armor = MyArmor;

                if (InVeh == true)
                {
                    Game.LocalPlayer.Character.Tasks.EnterVehicle(GetVehicle, -1, EnterVehicleFlags.WarpIn);
                }

                // Restore world
                World.TimeOfDay = MyTime;
                Command_Notification("RecovFR: Restore complete...");
                GameFiber.Sleep(1000); // Wait 1 second
                return;

            } else {
                // There is no XML file
                Game.DisplayNotification("RecovFR: Error: No backup file found.");
                return;
            }
        }
        private static void Command_Notification(string text)
        {
            if (ShowNotifications == true)
            {
                Game.DisplayNotification(text);
            }
        } 
    }
}
