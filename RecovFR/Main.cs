using System;
using System.Windows.Forms;
using Rage;
using Rage.Native;

namespace RecovFR
{
    public static class EntryPoint

    {
        // Settings variables    
        public static string XMLpath = "Plugins\\RecovFR.xml";
    //    public static Boolean IsLoaded = false;
        public static Keys BackupKey { get; set; }
        public static Keys BackupModifierKey { get; set; }
        public static Keys RestoreKey { get; set; }
        public static Keys RestoreModifierKey { get; set; }
        public static Boolean AutoBackups { get; set; }
        public static Byte AutoBackupInt { get; set; }
        public static Boolean ShowNotifications { get; set; }
        public static Boolean PlayerGodMode { get; set; }
        public static Boolean VehicleGodMode { get; set; }
        public static Boolean FreezeWeather { get; set; }
        public static Boolean SnowOnGround { get; set; }
        public static Boolean FreezeTime { get; set; }
        public static Vehicle GetVehicle { get; set; }
        public static Boolean InVehicle { get; set; }
        public static Vector3 MyLoc { get; set; }

        //Initialization of the plugin.
        public static void Main()
        {

            Game.LogTrivial("Loading RecovFR settings...");
            LoadValuesFromIniFile();

            Game.LogTrivial("RecovFR " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");

            // Key binding fiber
            GameFiber.StartNew(delegate 
            {
                while (true)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(BackupKey) &&
                        (Game.IsKeyDownRightNow(BackupModifierKey) ||
                            BackupModifierKey == Keys.None))
                    {
                        Backup.DoBackup();
                        GameFiber.Sleep(1000);
                    }
                    if (Game.IsKeyDown(RestoreKey) &&
                        (Game.IsKeyDownRightNow(RestoreModifierKey) ||
                            RestoreModifierKey == Keys.None))
                    {
                        Restore.DoRestore();
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
                        GameFiber.Yield(); // May need to remove this
                        Game.LogTrivial("RecovFR: AutoBackup initiated");
                        Backup.DoBackup();
                        GameFiber.Sleep(time);
                    }
                });
            }

            //    // Try to apply startup options
            //GameFiber.StartNew(delegate 
            //{
            //    while (Game.IsLoading)
            //    {
            //        GameFiber.Sleep(2000); // Wait 2 seconds
            //    } 
            //        try
            //        { // Apply startup options
            //            GameFiber.Sleep(2000); // Wait 1 second
            //            if (PlayerGodMode == true) { NativeFunction.Natives.SetPlayerInvincible(Game.LocalPlayer, true); }
            //            if (FreezeWeather == true) { NativeFunction.Natives.SetWeatherTypeNowPersist(SetWeather); }
            //            if (SnowOnGround == true) { MemoryClasses.MemoryAccess.SetSnowRendered(true); }
            //            return;
            //        } 
            //        catch (Exception e) { EntryPoint.ErrorLogger(e, "Startup", "Error applying startup options"); }
            //}); 
        }

        private static void LoadValuesFromIniFile()
        {
            string path = "Plugins\\RecovFR.ini";
            InitializationFile ini = new InitializationFile(path);
            ini.Create();

            try
            {
                if (ini.DoesKeyExist("Keybindings", "BackupKey"))
                { 
                    BackupKey = ini.ReadEnum<Keys>("Keybindings", "BackupKey", Keys.F12);
                } else { 
                    ini.Write("Keybindings", "BackupKey", "F12"); 
                }
                if (ini.DoesKeyExist("Keybindings", "BackupModifierKey"))
                { 
                    BackupModifierKey = ini.ReadEnum<Keys>("Keybindings", "BackupModifierKey", Keys.ControlKey);
                } else { 
                    ini.Write("Keybindings", "BackupModifierKey", "ControlKey"); 
                }
                if (ini.DoesKeyExist("Keybindings", "RestoreKey"))
                {
                    RestoreKey = ini.ReadEnum<Keys>("Keybindings", "RestoreKey", Keys.F12);
                }
                else
                {
                    ini.Write("Keybindings", "RestoreKey", "F12");
                }
                if (ini.DoesKeyExist("Keybindings", "RestoreModifierKey"))
                {
                    RestoreModifierKey = ini.ReadEnum<Keys>("Keybindings", "RestoreModifierKey", Keys.Menu);
                }
                else
                {
                    ini.Write("Keybindings", "RestoreModifierKey", "ControlKey");
                }
                if (ini.DoesKeyExist("Features", "AutoBackups"))
                {
                    AutoBackups = ini.ReadBoolean("Features", "AutoBackups", false);
                }
                else
                {
                    ini.Write("Features", "AutoBackups", "false");
                }
                if (ini.DoesKeyExist("Features", "AutoBackupInt"))
                {
                    AutoBackupInt = ini.ReadByte("Features", "AutoBackupInt", 60);
                }
                else
                {
                    ini.Write("Features", "AutoBackupInt", "60");
                }
                if (ini.DoesKeyExist("Features", "ShowNotifications"))
                {
                    ShowNotifications = ini.ReadBoolean("Features", "ShowNotifications", false);
                }
                else
                {
                    ini.Write("Features", "ShowNotifications", "false");
                }
                if (ini.DoesKeyExist("RestoreOptions", "PlayerGodMode"))
                {
                    PlayerGodMode = ini.ReadBoolean("RestoreOptions", "PlayerGodMode", false);
                }
                else
                {
                    ini.Write("RestoreOptions", "PlayerGodMode", "False");
                }
                if (ini.DoesKeyExist("RestoreOptions", "VehicleGodMode"))
                {
                    VehicleGodMode = ini.ReadBoolean("RestoreOptions", "VehicleGodMode", false);
                }
                else
                {
                    ini.Write("RestoreOptions", "VehicleGodMode", "false");
                }
                if (ini.DoesKeyExist("RestoreOptions", "FreezeWeather"))
                {
                    FreezeWeather = ini.ReadBoolean("RestoreOptions", "FreezeWeather", false);
                }
                else
                {
                    ini.Write("RestoreOptions", "FreezeWeather", "false");
                }
                if (ini.DoesKeyExist("RestoreOptions", "SnowOnGround"))
                {
                    SnowOnGround = ini.ReadBoolean("RestoreOptions", "SnowOnGround", false);
                }
                else
                {
                    ini.Write("RestoreOptions", "SnowOnGround", "false");
                }
                if (ini.DoesKeyExist("RestoreOptions", "FreezeTime"))
                {
                    FreezeTime = ini.ReadBoolean("RestoreOptions", "FreezeTime", false);
                }
                else
                {
                    ini.Write("RestoreOptions", "FreezeTime", "false");
                }
                Game.LogTrivial("RecovFR: Settings initialisation complete.");
            }
            catch (Exception e)
            {
                ErrorLogger(e, "Initialisation", "Unable to read INI file.");
            }

        }
     
        public static void Command_Notification(string text)
        {
            if (ShowNotifications == true)
            {
                Game.DisplayNotification(text);
            }
        } 

        public static void ErrorLogger(Exception Err, String ErrMod, String ErrDesc)
        {
            Game.LogTrivial("RevovFR: --------------------------------------");
            Game.LogTrivial("RecovFR: Error during " + ErrMod);
            Game.LogTrivial("Decription: " + ErrDesc);
            Game.LogTrivial(Err.ToString());
            Game.DisplayNotification("~r~~h~RecovFR:~h~~s~ Error during " + ErrMod + ". Please send logs.");
        }
    }
}
