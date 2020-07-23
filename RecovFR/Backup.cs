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
            EntryPoint.MyPed = Game.LocalPlayer.Character;

            Dictionary<string, string> XMLVehicle = new Dictionary<string, string>();
            Dictionary<string, string> XMLPed = new Dictionary<string, string>();           
            List<Tuple<int, int, int, int>> XMLPedComponents = new List<Tuple<int, int, int, int>>();
            List<Tuple<int, int, int, int>> XMLPedProps = new List<Tuple<int, int, int, int>>();
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
                {  // Get component Elements
                    for (int i = 0; i <= 11; i++)
                    {
                        int GetComponentId = i;
                        int GetDrawableId = NativeFunction.Natives.GET_PED_DRAWABLE_VARIATION<int>(EntryPoint.MyPed, i);
                        int GetTextureId = NativeFunction.Natives.GET_PED_TEXTURE_VARIATION<int>(EntryPoint.MyPed, i);
                        int GetPaleteId = NativeFunction.Natives.GET_PED_PALETTE_VARIATION<int>(EntryPoint.MyPed, i);

                        XMLPedComponents.Add(new Tuple<int, int, int, int>(GetComponentId, GetDrawableId, GetTextureId, GetPaleteId));
                    }

                }
                catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting character component elements"); }

                try
                {  // Get prop Elements
                    for (int i = 0; i <= 3; i++)
                    {
                        int GetComponentId = i;
                        int GetDrawableId = NativeFunction.Natives.GET_PED_PROP_INDEX<int>(EntryPoint.MyPed, i);
                        int GetTextureId = NativeFunction.Natives.GET_PED_PROP_TEXTURE_INDEX<int>(EntryPoint.MyPed, i);
                        int GetPaleteId = NativeFunction.Natives.GET_PED_PALETTE_VARIATION<int>(EntryPoint.MyPed, i);

                        XMLPedProps.Add(new Tuple<int, int, int, int>(GetComponentId, GetDrawableId, GetTextureId, GetPaleteId));
                    }
                }
                catch (Exception e) { EntryPoint.ErrorLogger(e, "Backup", "Error collecting character prop elements"); }



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

                    // Ped component elements
                    xmlWriter.WriteStartElement("MyComponentElements");
                    foreach (Tuple<int, int, int, int> XMLValue in XMLPedComponents)
                    {
                        xmlWriter.WriteStartElement("Component");
                        xmlWriter.WriteAttributeString("ComponentId", XMLValue.Item1.ToString());
                        xmlWriter.WriteAttributeString("DrawableId", XMLValue.Item2.ToString());
                        xmlWriter.WriteAttributeString("TextureId", XMLValue.Item3.ToString());
                        xmlWriter.WriteAttributeString("PaleteId", XMLValue.Item4.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    // Ped prop elements
                    xmlWriter.WriteStartElement("MyPropElements");
                    foreach (Tuple<int, int, int, int> XMLValue in XMLPedProps)
                    {
                        xmlWriter.WriteStartElement("Prop");
                        xmlWriter.WriteAttributeString("ComponentId", XMLValue.Item1.ToString());
                        xmlWriter.WriteAttributeString("DrawableId", XMLValue.Item2.ToString());
                        xmlWriter.WriteAttributeString("TextureId", XMLValue.Item3.ToString());
                        xmlWriter.WriteAttributeString("PaleteId", XMLValue.Item4.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                        // Weapon elements
                        xmlWriter.WriteStartElement("MyWeaponElements");
                    foreach (KeyValuePair<string, string> XMLValue in XMLWeapon)
                    {
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
