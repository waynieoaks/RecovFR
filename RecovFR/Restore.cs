using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using Rage;
using Rage.Native;

namespace RecovFR
{
    class Restore
    {
        //Vehicle variables
        static String MyVeh, MyVehPlate, MyVehRadio;
        static float MyVehLocX, MyVehLocY, MyVehLocZ, MyVehLocH, MyVehDirt, MyVehBodyHealth, MyVehEngineHealth, MyVehFuelTankHealth;
        static Int16 MyVehPriR, MyVehPriG, MyVehPriB, MyVehSecR, MyVehSecG, MyVehSecB, MyVehRimR, MyVehRimG, MyVehRimB, MyVehLivery, MyVehTint, MyVehHealth;
        static short MyVehPlateStyle;

        // Character variables
        static float MyLocX, MyLocY, MyLocZ, MyLocH;
        static Int16 MyWanted, MyHealth, MyArmor;

        // Environment variables
        static TimeSpan MyTime;
        static Int32 MyWeather;
        static float MyPuddles, MyWindSpeed, MyWindDirection;

        public static void DoRestore()
        {
            // Check if there is an xml file
            if (File.Exists(EntryPoint.XMLpath))
            {
                // Read some XML
                XDocument xdocument = XDocument.Load(EntryPoint.XMLpath);
                IEnumerable<XElement> backups = xdocument.Elements();
                if (xdocument == null)
                {
                    Game.DisplayNotification("Error: RecovFR: Backup file missing data.");
                }
                    foreach (var backup in backups)
                {
                    // Vehicle elements
                    EntryPoint.InVehicle = Boolean.Parse(backup.Element("InVehicle").Value);
                    MyVeh = backup.Element("MyVehicle").Value;
                    MyVehLocX = float.Parse(backup.Element("MyVehLocX").Value);
                    MyVehLocY = float.Parse(backup.Element("MyVehLocY").Value);
                    MyVehLocZ = float.Parse(backup.Element("MyVehLocZ").Value);
                    MyVehLocH = float.Parse(backup.Element("MyVehLocH").Value);
                    MyVehPriR = Int16.Parse(backup.Element("MyVehPriR").Value);
                    MyVehPriG = Int16.Parse(backup.Element("MyVehPriG").Value);
                    MyVehPriB = Int16.Parse(backup.Element("MyVehPriB").Value);
                    MyVehSecR = Int16.Parse(backup.Element("MyVehSecR").Value);
                    MyVehSecG = Int16.Parse(backup.Element("MyVehSecG").Value);
                    MyVehSecB = Int16.Parse(backup.Element("MyVehSecB").Value);
                    MyVehRimR = Int16.Parse(backup.Element("MyVehRimR").Value);
                    MyVehRimG = Int16.Parse(backup.Element("MyVehRimG").Value);
                    MyVehRimB = Int16.Parse(backup.Element("MyVehRimB").Value);
                    MyVehDirt = float.Parse(backup.Element("MyVehDirt").Value);
                    MyVehLivery = Int16.Parse(backup.Element("MyVehLivery").Value);
                    MyVehTint = Int16.Parse(backup.Element("MyVehTint").Value);
                    MyVehPlate = backup.Element("MyVehPlate").Value;
                    MyVehPlateStyle = short.Parse(backup.Element("MyVehPlateStyle").Value);
                    MyVehHealth = Int16.Parse(backup.Element("MyVehHealth").Value);
                    MyVehBodyHealth = float.Parse(backup.Element("MyVehBodyHealth").Value);
                    MyVehEngineHealth = float.Parse(backup.Element("MyVehEngineHealth").Value);
                    MyVehFuelTankHealth = float.Parse(backup.Element("MyVehFuelTankHealth").Value);
                    MyVehRadio = backup.Element("MyVehRadio").Value;

                    // Character Elements
                    MyLocX = float.Parse(backup.Element("MyLocX").Value);
                    MyLocY = float.Parse(backup.Element("MyLocY").Value);
                    MyLocZ = float.Parse(backup.Element("MyLocZ").Value);
                    MyLocH = float.Parse(backup.Element("MyLocH").Value);
                    MyWanted = Int16.Parse(backup.Element("MyWanted").Value);
                    MyHealth = Int16.Parse(backup.Element("MyHealth").Value);
                    MyArmor = Int16.Parse(backup.Element("MyArmor").Value);

                    // World Elements
                    MyTime = TimeSpan.Parse(backup.Element("MyTime").Value);
                    MyWeather = Int32.Parse(backup.Element("MyWeather").Value);
                    MyPuddles = float.Parse(backup.Element("MyPuddles").Value);
                    MyWindSpeed = float.Parse(backup.Element("MyWindSpeed").Value);
                    MyWindDirection = float.Parse(backup.Element("MyWindDirection").Value);
                }

                if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                {
                    //Get out vehicle to pevent taking it with us
                    EntryPoint.GetVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    Game.LocalPlayer.Character.Tasks.LeaveVehicle(EntryPoint.GetVehicle, LeaveVehicleFlags.WarpOut).WaitForCompletion();
                }

                // Now lets run the restore
                EntryPoint.GetVehicle = null;

                Vector3 MyVehLoc = new Vector3(MyVehLocX, MyVehLocY, MyVehLocZ);
                if (MyVeh != "None")
                {

                    Lookups.LookupVehicles.TryGetValue(MyVeh, out String Vehresult);

                    if (Vehresult == null)
                    {
                        try
                        {
                            EntryPoint.GetVehicle = new Vehicle(MyVeh, MyVehLoc, MyVehLocH);
                        }
                        catch (Exception e)
                        {
                            EntryPoint.InVehicle = false;  // Vehicle not restored, so forget warp to vehicle
                            MyVeh = "None";
                            EntryPoint.Command_Notification("RecovFR: Error restoring vehicle, please send me your logs");
                            Game.LogTrivial("RevovFR: --------------------------------------");
                            Game.LogTrivial("RecovFR: Error restoring vehicle.");
                            Game.LogTrivial("Expected vehicle: " + MyVeh);
                            Game.LogTrivial(e.ToString());
                            
                        }
                    }
                    else
                    {
                        try
                        {
                            EntryPoint.GetVehicle = new Vehicle(Vehresult, MyVehLoc, MyVehLocH);
                        }
                        catch (Exception e)
                        {
                            EntryPoint.InVehicle = false;  // Vehicle not restored, so forget warp to vehicle
                            MyVeh = "None";
                            EntryPoint.Command_Notification("RecovFR: Error restoring vehicle, please send me your logs");
                            Game.LogTrivial("RevovFR: --------------------------------------");
                            Game.LogTrivial("RecovFR: Error restoring vehicle.");
                            Game.LogTrivial("Expected vehicle: " + MyVeh);
                            Game.LogTrivial(e.ToString());     
                        }
                    }
                }

                // Try and set vehicle parameters
                if (MyVeh != "None")
                {
                    try
                    {
                        EntryPoint.GetVehicle.PrimaryColor = Color.FromArgb(MyVehPriR, MyVehPriG, MyVehPriB);
                        EntryPoint.GetVehicle.SecondaryColor = Color.FromArgb(MyVehSecR, MyVehSecG, MyVehSecB);
                        EntryPoint.GetVehicle.RimColor = Color.FromArgb(MyVehRimR, MyVehRimG, MyVehRimB);
                        EntryPoint.GetVehicle.DirtLevel = MyVehDirt;
                        NativeFunction.Natives.SetVehicleLivery<int>(EntryPoint.GetVehicle, MyVehLivery);
                        NativeFunction.Natives.SetVehicleWindowTint<int>(EntryPoint.GetVehicle, MyVehTint);
                        EntryPoint.GetVehicle.LicensePlate = MyVehPlate;
                        NativeFunction.Natives.SetVehicleNumberPlateTextIndex<short>(EntryPoint.GetVehicle, MyVehPlateStyle);
                        EntryPoint.GetVehicle.Health = MyVehHealth;
                        NativeFunction.Natives.SetVehicleBodyHealth<float>(EntryPoint.GetVehicle, MyVehBodyHealth);
                        EntryPoint.GetVehicle.EngineHealth = MyVehEngineHealth;
                        EntryPoint.GetVehicle.FuelTankHealth = MyVehFuelTankHealth;
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("RevovFR: --------------------------------------");
                        Game.LogTrivial("RecovFR: Error restoring vehicle perameters.");
                        Game.LogTrivial(e.ToString());
                    }
                }

                // Restore character
                Vector3 MyLoc = new Vector3(MyLocX, MyLocY, MyLocZ);
                World.TeleportLocalPlayer(MyLoc, false); // Set to true if reports of falling through world
                Game.LocalPlayer.WantedLevel = MyWanted;
                Game.LocalPlayer.Character.Health = MyHealth;
                Game.LocalPlayer.Character.Armor = MyArmor;

                if (EntryPoint.InVehicle == true)
                {
                    Game.LocalPlayer.Character.Tasks.EnterVehicle(EntryPoint.GetVehicle, -1, EnterVehicleFlags.WarpIn).WaitForCompletion();
                    GameFiber.Sleep(2500);
                    
                    if (MyVehRadio != "") { NativeFunction.Natives.SetRadioToStationName(MyVehRadio); }
                    else { NativeFunction.Natives.SetRadioToStationName("OFF"); }
                }

                // Restore world
                World.TimeOfDay = MyTime;
                if (Lookups.LookupWeather.TryGetValue(MyWeather, out String Weathresult))
                     {
                        NativeFunction.Natives.SetWeatherTypeNow<string>(Weathresult);
                     } else
                     {
                         Game.DisplayNotification("Error: RecovFR: Could not recover weather");
                         Game.LogTrivial("RecovFR: Could not detect weather: " + MyWeather.ToString());
                     }
                    // NativeFunction.Natives.SetWeatherTypeNow<string>(MyWeather);
                NativeFunction.Natives.SetWindSpeed<float>(MyWindSpeed);
                NativeFunction.Natives.SetWindDirection<float>(MyWindDirection);
                World.WaterPuddlesIntensity = MyPuddles;

                //Finish up
                EntryPoint.Command_Notification("RecovFR: Restore complete...");
                GameFiber.Sleep(1000); // Wait 1 second
                return;
            }
            else
            {
                // There is no XML file
                Game.DisplayNotification("Error: RecovFR: Backup file not found.");
                return;
            }
        }

    }
}
