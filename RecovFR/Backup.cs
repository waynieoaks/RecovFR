using System;
using System.Collections.Generic;
using System.Xml;
using Rage;
using Rage.Native;

namespace RecovFR
{
    class Backup
    {
        public static void DoBackup()
        {
            Dictionary<string, string> XMLVehicle = new Dictionary<string, string>();
            Dictionary<string, string> XMLPed = new Dictionary<string, string>();
            Dictionary<string, string> XMLProp = new Dictionary<string, string>();
            Dictionary<string, string> XMLWeapon = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> XMLWeaponComps = new List<KeyValuePair<string, string>>();
            Dictionary<string, string> XMLWorld = new Dictionary<string, string>();
            try
            { // Catch all
                // Am I in a vehicle?
                if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                {
                    try
                    {
                        EntryPoint.GetVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                        EntryPoint.InVehicle = true;
                    }
                    catch
                    {
                        EntryPoint.GetVehicle = null;
                        EntryPoint.InVehicle = false;
                    }
                }
                else
                {
                    try
                    {
                        EntryPoint.GetVehicle = Game.LocalPlayer.Character.LastVehicle;
                        EntryPoint.InVehicle = false;
                    }
                    catch
                    {
                        EntryPoint.GetVehicle = null;
                        EntryPoint.InVehicle = false;
                    }
                }

                try
                {
                    // Get vehicle Elements
                    if (EntryPoint.InVehicle == false)
                    {
                        XMLVehicle.Add("InVehicle", "false");
                    }
                    else
                    {
                        XMLVehicle.Add("InVehicle", "true");
                    }

                    if (EntryPoint.GetVehicle == null)
                    {
                        XMLVehicle.Add("MyVehicle", "None");
                        XMLVehicle.Add("MyVehLocX", "0");
                        XMLVehicle.Add("MyVehLocY", "0");
                        XMLVehicle.Add("MyVehLocZ", "0");
                        XMLVehicle.Add("MyVehLocH", "0");
                        XMLVehicle.Add("MyVehPriR", "0");
                        XMLVehicle.Add("MyVehPriG", "0");
                        XMLVehicle.Add("MyVehPriB", "0");
                        XMLVehicle.Add("MyVehSecR", "0");
                        XMLVehicle.Add("MyVehSecG", "0");
                        XMLVehicle.Add("MyVehSecB", "0");
                        XMLVehicle.Add("MyVehRimR", "0");
                        XMLVehicle.Add("MyVehRimG", "0");
                        XMLVehicle.Add("MyVehRimB", "0");
                        XMLVehicle.Add("MyVehDirt", "0");
                        XMLVehicle.Add("MyVehLivery", "-1");
                        XMLVehicle.Add("MyVehTint", "-1");
                        XMLVehicle.Add("MyVehPlate", "");
                        XMLVehicle.Add("MyVehPlateStyle", "0");
                        XMLVehicle.Add("MyVehHealth", "0");
                        XMLVehicle.Add("MyVehBodyHealth", "0");
                        XMLVehicle.Add("MyVehEngineHealth", "0");
                        XMLVehicle.Add("MyVehFuelTankHealth", "0");
                        XMLVehicle.Add("MyVehRadio", "");
                    }
                    else
                    {
                        XMLVehicle.Add("MyVehicle", EntryPoint.GetVehicle.Model.Name.ToString());
                        XMLVehicle.Add("MyVehLocX", EntryPoint.GetVehicle.Position.X.ToString());
                        XMLVehicle.Add("MyVehLocY", EntryPoint.GetVehicle.Position.Y.ToString());
                        XMLVehicle.Add("MyVehLocZ", EntryPoint.GetVehicle.Position.Z.ToString());
                        XMLVehicle.Add("MyVehLocH", EntryPoint.GetVehicle.Heading.ToString());
                        XMLVehicle.Add("MyVehPriR", EntryPoint.GetVehicle.PrimaryColor.R.ToString());
                        XMLVehicle.Add("MyVehPriG", EntryPoint.GetVehicle.PrimaryColor.G.ToString());
                        XMLVehicle.Add("MyVehPriB", EntryPoint.GetVehicle.PrimaryColor.B.ToString());
                        XMLVehicle.Add("MyVehSecR", EntryPoint.GetVehicle.SecondaryColor.R.ToString());
                        XMLVehicle.Add("MyVehSecG", EntryPoint.GetVehicle.SecondaryColor.G.ToString());
                        XMLVehicle.Add("MyVehSecB", EntryPoint.GetVehicle.SecondaryColor.B.ToString());
                        XMLVehicle.Add("MyVehRimR", EntryPoint.GetVehicle.RimColor.R.ToString());
                        XMLVehicle.Add("MyVehRimG", EntryPoint.GetVehicle.RimColor.G.ToString());
                        XMLVehicle.Add("MyVehRimB", EntryPoint.GetVehicle.RimColor.B.ToString());
                        XMLVehicle.Add("MyVehDirt", EntryPoint.GetVehicle.DirtLevel.ToString());
                        XMLVehicle.Add("MyVehLivery", NativeFunction.Natives.GetVehicleLivery<int>(EntryPoint.GetVehicle).ToString());
                        XMLVehicle.Add("MyVehTint", NativeFunction.Natives.GetVehicleWindowTint<int>(EntryPoint.GetVehicle).ToString());
                        XMLVehicle.Add("MyVehPlate", EntryPoint.GetVehicle.LicensePlate);
                        XMLVehicle.Add("MyVehPlateStyle", NativeFunction.Natives.GetVehicleNumberPlateTextIndex<short>(EntryPoint.GetVehicle).ToString());
                        XMLVehicle.Add("MyVehHealth", EntryPoint.GetVehicle.Health.ToString());
                        XMLVehicle.Add("MyVehBodyHealth", NativeFunction.Natives.GetVehicleBodyHealth<float>(EntryPoint.GetVehicle).ToString());
                        XMLVehicle.Add("MyVehEngineHealth", EntryPoint.GetVehicle.EngineHealth.ToString());
                        XMLVehicle.Add("MyVehFuelTankHealth", EntryPoint.GetVehicle.FuelTankHealth.ToString());
                        XMLVehicle.Add("MyVehRadio", NativeFunction.Natives.GetPlayerRadioStationName<string>());
                    }
                } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting vehicle elements"); }

                try
                {  // Get character Elements
                    XMLPed.Add("MyModel", Game.LocalPlayer.Character.Model.Name);
                    XMLPed.Add("MyLocX", Game.LocalPlayer.Character.Position.X.ToString());
                    XMLPed.Add("MyLocY", Game.LocalPlayer.Character.Position.Y.ToString());
                    XMLPed.Add("MyLocZ", Game.LocalPlayer.Character.Position.Z.ToString());
                    XMLPed.Add("MyLocH", Game.LocalPlayer.Character.Heading.ToString());
                    XMLPed.Add("MyWanted", Game.LocalPlayer.WantedLevel.ToString());
                    XMLPed.Add("MyHealth", Game.LocalPlayer.Character.Health.ToString());
                    XMLPed.Add("MyArmor", Game.LocalPlayer.Character.Armor.ToString());
                    XMLPed.Add("MyInvincible", NativeFunction.Natives.GetPlayerInvincible<bool>(Game.LocalPlayer).ToString());
                } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting character elements"); }

                try
                { // Get Weapon Elements
                    foreach (KeyValuePair<long, string> weapon in Lookups.LookupWeapons)
                    {
                        bool hasPedGotWeapon = NativeFunction.Natives.HasPedGotWeapon<bool>(Game.LocalPlayer.Character, weapon.Key, false);
                        if (hasPedGotWeapon == true)
                        {
                            XMLWeapon.Add(weapon.Value.ToString(), NativeFunction.Natives.GetAmmoInPedWeapon<int>(Game.LocalPlayer.Character, weapon.Key).ToString());

                            foreach (KeyValuePair<string, string> weaponComponent in Lookups.LookupWeaponComponents)
                            {
                                Int32 componentHash = NativeFunction.Natives.GetHashKey<int>(weaponComponent.Key);
                                Boolean hasWeaponGotComponent = NativeFunction.Natives.HasPedGotWeaponComponent<bool>(Game.LocalPlayer.Character, weapon.Key, componentHash);
                                if (hasWeaponGotComponent)
                                {
                                    XMLWeaponComps.Add(new KeyValuePair<string, string>(weapon.Value.ToString(), weaponComponent.Key.ToString()));
                                }
                            }
                        }
                    }
                } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting weapon elements"); }

                try
                { // Get World Elements
                    XMLWorld.Add("MyTime", World.TimeOfDay.ToString());
                    XMLWorld.Add("MyWeather", NativeFunction.Natives.GetPrevWeatherTypeHashName<int>().ToString());
                    XMLWorld.Add("MyPuddles", World.WaterPuddlesIntensity.ToString());
                    XMLWorld.Add("MyWindSpeed", NativeFunction.Natives.GetWindSpeed<float>().ToString());
                    XMLWorld.Add("MyWindDirection", NativeFunction.Natives.GetWindDirection<float>().ToString());
                } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting world elements"); }

                try
                { // Now Write backup to XML file for later use
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                    {
                        Indent = true,
                        IndentChars = "    "
                    };
                    // XmlWriter xmlWriter = XmlWriter.Create(XMLpath);
                    using (XmlWriter xmlWriter = XmlWriter.Create(EntryPoint.XMLpath, xmlWriterSettings))
                {
                    // Start the XML
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Backup");
                    xmlWriter.WriteAttributeString("LastRun", DateTime.Now.ToString("F"));

                    // Vehicle elements
                    xmlWriter.WriteStartElement("MyVehicleElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLVehicle)
                    {
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // Ped elements
                    xmlWriter.WriteStartElement("MyPedElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLPed)
                    {
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // Prop elements
                    xmlWriter.WriteStartElement("MyPropElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLProp)
                    {
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // Weapon elements
                    xmlWriter.WriteStartElement("MyWeaponElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLWeapon)
                    {
                        //     xmlWriter.WriteStartElement("WEAPON");
                        //     xmlWriter.WriteAttributeString("NAME", XMLValue.Key);
                        //     xmlWriter.WriteAttributeString("AMMO", XMLValue.Value);
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // Weapon Components elements
                    xmlWriter.WriteStartElement("MyWeaponComponents");
                    foreach (KeyValuePair<string, string> XMLValue in XMLWeaponComps)
                    {
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // World elements
                    xmlWriter.WriteStartElement("MyWorldElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLWorld)
                    {
                        xmlWriter.WriteStartElement(XMLValue.Key);
                        xmlWriter.WriteString(XMLValue.Value);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // End the XML
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
                }

            } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error writing to XML"); }

            EntryPoint.Command_Notification("RecovFR: Backup complete...");
            return;

            } catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Someting went wrong (catch all)"); }
            return;
        }

    }
}
