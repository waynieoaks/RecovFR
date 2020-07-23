using System;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;


namespace RecovFR
{
    class Menu

    {
        public static MenuPool menuPool;
        public static UIMenu mainMenu;
        public static string UpdateValue;
        public static void BuildMenu()
        {
            menuPool = new MenuPool();
            mainMenu = new UIMenu("RecovFR", "~b~Restore options:");

            mainMenu.AddItem(new UIMenuCheckboxItem("Player God Mode", EntryPoint.PlayerGodMode));
            mainMenu.AddItem(new UIMenuCheckboxItem("Vehicle God Mode", EntryPoint.VehicleGodMode));
            mainMenu.AddItem(new UIMenuCheckboxItem("Freeze Weather", EntryPoint.FreezeWeather));
            mainMenu.AddItem(new UIMenuCheckboxItem("Snow on Ground", EntryPoint.SnowOnGround));
            mainMenu.AddItem(new UIMenuCheckboxItem("Freeze Time", EntryPoint.FreezeTime));
            mainMenu.AddItem(new UIMenuItem("Backup", "Select to run backup"));
            mainMenu.AddItem(new UIMenuItem("Restore", "Select to run restore"));
            mainMenu.RefreshIndex();

            mainMenu.OnCheckboxChange += MainMenuOnCheckboxChange;
            mainMenu.OnItemSelect += MainMenuOnItemSelect;
            menuPool.Add(mainMenu);
        }

        public static void MainMenuOnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked)
        {
            
            if (checkboxItem.Text == "Player God Mode") { UpdateValue = "PlayerGodMode"; EntryPoint.PlayerGodMode = Checked; }
            if (checkboxItem.Text == "Vehicle God Mode") { UpdateValue = "VehicleGodMode"; EntryPoint.VehicleGodMode = Checked; }
            if (checkboxItem.Text == "Freeze Weather") { UpdateValue = "FreezeWeather"; EntryPoint.FreezeWeather = Checked; }
            if (checkboxItem.Text == "Snow on Ground") { UpdateValue = "SnowOnGround"; EntryPoint.SnowOnGround = Checked; }
            if (checkboxItem.Text == "Freeze Time") { UpdateValue = "FreezeTime"; EntryPoint.FreezeTime = Checked; }
            
            InitializationFile ini = new InitializationFile(EntryPoint.INIpath);
            ini.Create();

            try
            {
                ini.Write("RestoreOptions", UpdateValue, Checked);

            }
            catch (Exception e)
            {
                EntryPoint.ErrorLogger(e, "MenuUpdate", "Unable to update INI file.");
            }

        }

        public static void MainMenuOnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {

            mainMenu.Visible = false;
            if (selectedItem.Text == "Backup") 
            {
                Backup.DoBackup();
            }
            if (selectedItem.Text == "Restore") 
            {
                Restore.DoRestore();
            }
        }

    }

}
