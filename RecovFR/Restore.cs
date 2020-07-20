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
        static String MyVehRadio = "";
        
        public static void DoRestore()
        {
            EntryPoint.MyPed = Game.LocalPlayer.Character;

            try
            { // Catch all
                // Check if there is an xml file
                if (File.Exists(EntryPoint.XMLpath))
                {
                    try
                    { // Do a bit of housework

                        NativeFunction.Natives.CLEAR_ALL_PED_PROPS(EntryPoint.MyPed);

                        if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                        {
                            //Delete vehicle to pevent taking it with us
                            Game.LocalPlayer.Character.CurrentVehicle.Delete();
                        }
                        else
                        {
                            try { Game.LocalPlayer.Character.LastVehicle.Delete(); } catch { }
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error doing housework"); }

                        XDocument xdocument = XDocument.Load(EntryPoint.XMLpath);

                    try
                    { ////// RESTORE VEHICLE ///////////////////////////////////////////////////////////////
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
                                        EntryPoint.ErrorLogger(e, "Restore", "Error restoring vehicle");
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
                                        EntryPoint.ErrorLogger(e, "Restore", "Error restoring vehicle");
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
                                    if (EntryPoint.VehicleGodMode == true)
                                    {
                                        NativeFunction.Natives.SET_VEHICLE_FIXED(EntryPoint.GetVehicle);
                                        NativeFunction.Natives.SET_ENTITY_INVINCIBLE(EntryPoint.GetVehicle, true);
                                        NativeFunction.Natives.SET_VEHICLE_STRONG(EntryPoint.GetVehicle, true);
                                        NativeFunction.Natives.SET_VEHICLE_CAN_BREAK(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_VEHICLE_CAN_BE_VISIBLY_DAMAGED(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_VEHICLE_TYRES_CAN_BURST(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_VEHICLE_WHEELS_CAN_BREAK(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_ENTITY_CAN_BE_DAMAGED(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_ENTITY_ONLY_DAMAGED_BY_PLAYER(EntryPoint.GetVehicle, false);
                                        NativeFunction.Natives.SET_ENTITY_PROOFS(EntryPoint.GetVehicle, 1, 1, 1, 1, 1, 1, 1, 1);
                                    } else
                                    {
                                     //   EntryPoint.GetVehicle.IsInvincible = false;
                                    }
                                }
                                catch (Exception e)
                                {
                                    EntryPoint.ErrorLogger(e, "Restore", "Error restoring vehicle parameters");
                                }
                            }
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring vehicle"); }

                    try
                    { ////// RESTORE CHARACTER /////////////////////////////////////////////////////////
                        IEnumerable<XElement> MyPedElements = xdocument.Descendants("MyPedElements");
                        foreach (XElement GetElements in MyPedElements)
                        {
                         //   Game.LocalPlayer.Model = (string)GetElements.Element("MyModel");
                            EntryPoint.MyLoc = new Vector3((float)GetElements.Element("MyLocX"),
                                                        (float)GetElements.Element("MyLocY"),
                                                        (float)GetElements.Element("MyLocZ"));
                            World.TeleportLocalPlayer(EntryPoint.MyLoc, false); // Set to true if reports of falling through world
                            Game.LocalPlayer.WantedLevel = (Int16)GetElements.Element("MyWanted");
                            Game.LocalPlayer.Character.Health = (Int16)GetElements.Element("MyHealth");
                            Game.LocalPlayer.Character.Armor = (Int16)GetElements.Element("MyArmor");
                            if (((Boolean)GetElements.Element("MyInvincible") == true) || (EntryPoint.PlayerGodMode == true))
                            {
                                NativeFunction.Natives.SetPlayerInvincible(Game.LocalPlayer, true);
                            } else
                            {
                                NativeFunction.Natives.SetPlayerInvincible(Game.LocalPlayer, false);
                            }
                        }

                        if (EntryPoint.InVehicle == true)
                        {
                            Game.LocalPlayer.Character.Tasks.EnterVehicle(EntryPoint.GetVehicle, -1, EnterVehicleFlags.WarpIn).WaitForCompletion();
                            GameFiber.Sleep(2500);

                            if (MyVehRadio != "") { NativeFunction.Natives.SetRadioToStationName(MyVehRadio); }
                            else { NativeFunction.Natives.SetRadioToStationName("OFF"); }
                        }
                        else if (EntryPoint.GetVehicle != null)
                        {
                            // Need to get in and out of vehicle to save it again
                            Game.LocalPlayer.Character.Tasks.EnterVehicle(EntryPoint.GetVehicle, -1, EnterVehicleFlags.WarpIn).WaitForCompletion();
                            // Go back to original location
                            Game.LocalPlayer.Character.Tasks.LeaveVehicle(EntryPoint.GetVehicle, LeaveVehicleFlags.WarpOut).WaitForCompletion();
                            World.TeleportLocalPlayer(EntryPoint.MyLoc, false);
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring character"); }

                    try
                    { ////// RESTORE COMPONENTS ////////////////////////////////////////////////////////////
                        IEnumerable<XElement> MyComponentElements = xdocument.Descendants("MyComponentElements");
                        foreach (XElement GetElements in MyComponentElements.Elements())
                        {
                            int GetComponentId = Int16.Parse(GetElements.Attribute("ComponentId").Value);

                            NativeFunction.Natives.SET_PED_COMPONENT_VARIATION(EntryPoint.MyPed,
                                Int16.Parse(GetElements.Attribute("ComponentId").Value),
                                Int16.Parse(GetElements.Attribute("DrawableId").Value),
                                Int16.Parse(GetElements.Attribute("TextureId").Value),
                                Int16.Parse(GetElements.Attribute("PaleteId").Value));
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring character components"); }

                    try
                    { ////// RESTORE PROPS ////////////////////////////////////////////////////////////
                        IEnumerable<XElement> MyPropElements = xdocument.Descendants("MyPropElements");
                        foreach (XElement GetElements in MyPropElements.Elements())
                        {
                            int GetComponentId = Int16.Parse(GetElements.Attribute("ComponentId").Value);

                            NativeFunction.Natives.SET_PED_PROP_INDEX(EntryPoint.MyPed,
                                Int16.Parse(GetElements.Attribute("ComponentId").Value),
                                Int16.Parse(GetElements.Attribute("DrawableId").Value),
                                Int16.Parse(GetElements.Attribute("TextureId").Value),
                                Int16.Parse(GetElements.Attribute("PaleteId").Value));
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring character props"); }

                    try
                    { ////// RESTORE WEAPONS //////////////////////////////////////////////////////////
                        IEnumerable<XElement> MyWeaponElements = xdocument.Descendants("MyWeaponElements");
                        foreach (XElement GetElements in MyWeaponElements.Elements())
                        {
                            Game.LocalPlayer.Character.Inventory.GiveNewWeapon(GetElements.Name.ToString(), short.Parse(GetElements.Value), false);
                        }

                        ////// RESTORE WEAPON COMPONENTS /////////////////////////////////////////////////
                        IEnumerable<XElement> MyWeaponComponents = xdocument.Descendants("MyWeaponComponents");
                        foreach (XElement GetElements in MyWeaponComponents.Elements())
                        {
                            //Game.LocalPlayer.Character.Inventory.GiveNewWeapon(GetElements.Name.ToString(), short.Parse(GetElements.Value), false);
                            Game.LocalPlayer.Character.Inventory.AddComponentToWeapon(GetElements.Name.ToString(), GetElements.Value);
                        }
                        // restore weapon in hand
                        // Game.LocalPlayer.Character.Inventory.EquippedWeapon = 
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring weapons or components"); }

                    try
                    { ////// RESTORE WORLD ////////////////////////////////////////////////////////////
                        NativeFunction.Natives.ClearWeatherTypePersist();
                        IEnumerable<XElement> MyWorldElements = xdocument.Descendants("MyWorldElements");
                        foreach (XElement GetElements in MyWorldElements)
                        {
                            World.TimeOfDay = TimeSpan.Parse((string)GetElements.Element("MyTime"));
                            if (EntryPoint.FreezeTime == true)
                            {
                                NativeFunction.Natives.PauseClock(true);
                            } else
                            {
                                NativeFunction.Natives.PauseClock(false);
                            }
                                if (Lookups.LookupWeather.TryGetValue((Int32)GetElements.Element("MyWeather"), out string Weathresult))
                            {
                                NativeFunction.Natives.SetWeatherTypeNow<string>("Clear");
                                if (EntryPoint.FreezeWeather == true)
                                {
                                    NativeFunction.Natives.SetWeatherTypeNowPersist<string>(Weathresult);
                                } else
                                {
                                    NativeFunction.Natives.SetWeatherTypeNow<string>(Weathresult);
                                }
                            }
                            else
                            {
                                Game.DisplayNotification("Error: RecovFR: Could not recover weather");
                                Game.LogTrivial("RecovFR: Could not detect weather: " + (Int32)GetElements.Element("MyWeather"));
                            }
                            NativeFunction.Natives.SetWindSpeed<float>((float)GetElements.Element("MyWindSpeed"));
                            NativeFunction.Natives.SetWindDirection<float>((float)GetElements.Element("MyWindDirection"));
                            World.WaterPuddlesIntensity = (float)GetElements.Element("MyPuddles");
                            if (EntryPoint.SnowOnGround == true)
                            {
                                MemoryClasses.MemoryAccess.SetSnowRendered(true);
                            } else
                            {
                                MemoryClasses.MemoryAccess.SetSnowRendered(false);
                            }
                        }
                    }
                    catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Error restoring world"); }

                    ////// FINISH UP ////////////////////////////////////////////////////////////
                    EntryPoint.Command_Notification("RecovFR: Restore complete...");
                    GameFiber.Sleep(1000); // Wait 1 second
                    return;
                }
                else
                {
                    // There is no XML file
                    Game.DisplayNotification("~r~~h~RecovFR:~s~~h~ Error: Backup file not found.");
                    Game.LogTrivial("RevovFR: --------------------------------------");
                    Game.LogTrivial("RecovFR: Error during Restore.");
                    Game.LogTrivial("Decription: XML file could not be found." );
                    return;
                }
            } catch (Exception e) { EntryPoint.ErrorLogger(e, "Restore", "Someting went wrong (catch all)"); }
            return;
        }
    }
}
