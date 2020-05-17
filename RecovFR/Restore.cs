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
        static String MyVehRadio ="";

        public static void DoRestore()
        {
            // Check if there is an xml file
            if (File.Exists(EntryPoint.XMLpath))
            {
                // Do a bit of housework
                if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                {
                    //Delete vehicle to pevent taking it with us
                    Game.LocalPlayer.Character.CurrentVehicle.Delete();
                } else
                {
                    try { Game.LocalPlayer.Character.LastVehicle.Delete(); } catch { }
                }

                // Initiate the XML file
                XDocument xdocument = XDocument.Load(EntryPoint.XMLpath);

                ////// RESTORE VEHICLE ///////////////////////////////////////////////////////////////
                IEnumerable<XElement> MyVehicleElements = xdocument.Descendants("MyVehicleElements");
                foreach (XElement GetElements in MyVehicleElements)
                {
                    EntryPoint.InVehicle = (Boolean)GetElements.Element("InVehicle");
                    Vector3 MyVehLoc = new Vector3((float)GetElements.Element("MyVehLocX"), 
                                                    (float)GetElements.Element("MyVehLocY"), 
                                                    (float)GetElements.Element("MyVehLocZ"));
                    
                    if ((string)GetElements.Element("MyVehicle") != "None")
                    {

                        Lookups.LookupVehicles.TryGetValue((string)GetElements.Element("MyVehicle"), out String Vehresult);

                        if (Vehresult == null)
                        {
                            try
                            {
                                EntryPoint.GetVehicle = new Vehicle((string)GetElements.Element("MyVehicle"), MyVehLoc, (float)GetElements.Element("MyVehLocH"));
                            }
                            catch (Exception e)
                            {
                                EntryPoint.InVehicle = false;  // Vehicle not restored, so forget warp to vehicle
                                EntryPoint.Command_Notification("RecovFR: Error restoring vehicle, please send me your logs");
                                Game.LogTrivial("RevovFR: --------------------------------------");
                                Game.LogTrivial("RecovFR: Error restoring vehicle.");
                                Game.LogTrivial("Expected vehicle: " + (string)GetElements.Element("MyVehicle"));
                                Game.LogTrivial(e.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                EntryPoint.GetVehicle = new Vehicle(Vehresult, MyVehLoc, (float)GetElements.Element("MyVehLocH"));
                            }
                            catch (Exception e)
                            {
                                EntryPoint.InVehicle = false;  // Vehicle not restored, so forget warp to vehicle
                                EntryPoint.Command_Notification("RecovFR: Error restoring vehicle, please send me your logs");
                                Game.LogTrivial("RevovFR: --------------------------------------");
                                Game.LogTrivial("RecovFR: Error restoring vehicle.");
                                Game.LogTrivial("Expected vehicle: " + (string)GetElements.Element("MyVehicle"));
                                Game.LogTrivial(e.ToString());
                            }
                        }
                    
                        // Try and set vehicle parameters
                        try
                        {
                            EntryPoint.GetVehicle.PrimaryColor = Color.FromArgb((Int16)GetElements.Element("MyVehPriR"),
                                                                                (Int16)GetElements.Element("MyVehPriG"),
                                                                                (Int16)GetElements.Element("MyVehPriB"));
                            EntryPoint.GetVehicle.SecondaryColor = Color.FromArgb((Int16)GetElements.Element("MyVehSecR"),
                                                                                (Int16)GetElements.Element("MyVehSecG"),
                                                                                (Int16)GetElements.Element("MyVehSecB"));
                            EntryPoint.GetVehicle.RimColor = Color.FromArgb((Int16)GetElements.Element("MyVehRimR"),
                                                                                (Int16)GetElements.Element("MyVehRimG"),
                                                                                (Int16)GetElements.Element("MyVehRimB"));
                            EntryPoint.GetVehicle.DirtLevel = (float)GetElements.Element("MyVehDirt");
                            NativeFunction.Natives.SetVehicleLivery<int>(EntryPoint.GetVehicle, (Int16)GetElements.Element("MyVehLivery"));
                            NativeFunction.Natives.SetVehicleWindowTint<int>(EntryPoint.GetVehicle, (Int16)GetElements.Element("MyVehTint"));
                            EntryPoint.GetVehicle.LicensePlate = (string)GetElements.Element("MyVehPlate");
                            NativeFunction.Natives.SetVehicleNumberPlateTextIndex<short>(EntryPoint.GetVehicle, (short)GetElements.Element("MyVehPlateStyle"));
                            EntryPoint.GetVehicle.Health = (Int16)GetElements.Element("MyVehHealth");
                            NativeFunction.Natives.SetVehicleBodyHealth<float>(EntryPoint.GetVehicle, (float)GetElements.Element("MyVehBodyHealth"));
                            EntryPoint.GetVehicle.EngineHealth = (float)GetElements.Element("MyVehEngineHealth");
                            EntryPoint.GetVehicle.FuelTankHealth = (float)GetElements.Element("MyVehFuelTankHealth");
                            MyVehRadio = (string)GetElements.Element("MyVehRadio"); //Get radio to set later
                        }
                        catch (Exception e)
                        {
                            Game.LogTrivial("RevovFR: --------------------------------------");
                            Game.LogTrivial("RecovFR: Error restoring vehicle perameters.");
                            Game.LogTrivial(e.ToString());
                        }
                    }
                }

                ////// RESTORE CHARACTER /////////////////////////////////////////////////////////
                IEnumerable<XElement> MyPedElements = xdocument.Descendants("MyPedElements");
                foreach (XElement GetElements in MyPedElements)
                {
                    Game.LocalPlayer.Model = (string)GetElements.Element("MyModel");
                    Vector3 MyLoc = new Vector3((float)GetElements.Element("MyLocX"),
                                                (float)GetElements.Element("MyLocY"),
                                                (float)GetElements.Element("MyLocZ"));
                    World.TeleportLocalPlayer(MyLoc, false); // Set to true if reports of falling through world
                    Game.LocalPlayer.WantedLevel = (Int16)GetElements.Element("MyWanted");
                    Game.LocalPlayer.Character.Health = (Int16)GetElements.Element("MyHealth");
                    Game.LocalPlayer.Character.Armor = (Int16)GetElements.Element("MyArmor");
                }

                if (EntryPoint.InVehicle == true)
                {
                    Game.LocalPlayer.Character.Tasks.EnterVehicle(EntryPoint.GetVehicle, -1, EnterVehicleFlags.WarpIn).WaitForCompletion();
                    GameFiber.Sleep(2500);

                    if (MyVehRadio != "") { NativeFunction.Natives.SetRadioToStationName(MyVehRadio); }
                    else { NativeFunction.Natives.SetRadioToStationName("OFF"); }
                }

                ////// RESTORE PROPS ////////////////////////////////////////////////////////////
                //IEnumerable<XElement> MyPropElements = xdocument.Descendants("MyPropElements");
                //foreach (XElement GetElements in MyPropElements)
                //{
                //    // MyArmor = (Int16)GetElements.Element("MyArmor");
                //}

                ////// RESTORE WEAPONS //////////////////////////////////////////////////////////
                //IEnumerable<XElement> MyWeaponElements = xdocument.Descendants("MyWeaponElements");
                //foreach (XElement GetElements in MyWeaponElements)
                //{
                //    // MyLocX = (float)GetElements.Element("MyLocX");
                //}


                ////// RESTORE WORLD ////////////////////////////////////////////////////////////
                IEnumerable<XElement> MyWorldElements = xdocument.Descendants("MyWorldElements");
                foreach (XElement GetElements in MyWorldElements)
                {
                    World.TimeOfDay = TimeSpan.Parse((string)GetElements.Element("MyTime"));
                    if (Lookups.LookupWeather.TryGetValue((Int32)GetElements.Element("MyWeather"), out string Weathresult))
                    {
                     //     NativeFunction.Natives.ClearWeatherTypePersist("Clear");
                          NativeFunction.Natives.SetWeatherTypeNow<string>("Clear");
                          NativeFunction.Natives.SetWeatherTypeNow<string>(Weathresult);
                    }
                    else
                    {
                        Game.DisplayNotification("Error: RecovFR: Could not recover weather");
                        Game.LogTrivial("RecovFR: Could not detect weather: " + (Int32)GetElements.Element("MyWeather"));
                    }
                    NativeFunction.Natives.SetWindSpeed<float>((float)GetElements.Element("MyWindSpeed"));
                    NativeFunction.Natives.SetWindDirection<float>((float)GetElements.Element("MyWindDirection"));
                    World.WaterPuddlesIntensity = (float)GetElements.Element("MyPuddles");
                }
             
                ////// FINISH UP ////////////////////////////////////////////////////////////
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
