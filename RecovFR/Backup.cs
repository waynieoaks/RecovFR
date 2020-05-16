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
            Dictionary <string, string> XMLBuilder = new Dictionary <string, string>();
            // Am I in a vehicle?
            
            if (Game.LocalPlayer.Character.IsInAnyVehicle(true)) {
                try {
                    EntryPoint.GetVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    EntryPoint.InVehicle = true;
                } catch {
                    EntryPoint.GetVehicle = null;
                    EntryPoint.InVehicle = false;
                }
            } else {
                 try {
                    EntryPoint.GetVehicle = Game.LocalPlayer.Character.LastVehicle;
                    EntryPoint.InVehicle = false;
                } catch {
                    EntryPoint.GetVehicle = null;
                    EntryPoint.InVehicle = false;
                }
            }

            // Get vehicle Elements
            if (EntryPoint.InVehicle == false)
            {
                XMLBuilder.Add("InVehicle", "false");
            }  else
            {
                XMLBuilder.Add("InVehicle", "true");
            }

            if (EntryPoint.GetVehicle == null)
            {
                XMLBuilder.Add("MyVehicle", "None");
                XMLBuilder.Add("MyVehLocX", "0");
                XMLBuilder.Add("MyVehLocY", "0");
                XMLBuilder.Add("MyVehLocZ", "0");
                XMLBuilder.Add("MyVehLocH", "0");
                XMLBuilder.Add("MyVehPriR", "0");
                XMLBuilder.Add("MyVehPriG", "0");
                XMLBuilder.Add("MyVehPriB", "0");
                XMLBuilder.Add("MyVehSecR", "0");
                XMLBuilder.Add("MyVehSecG", "0");
                XMLBuilder.Add("MyVehSecB", "0");
                XMLBuilder.Add("MyVehRimR", "0");
                XMLBuilder.Add("MyVehRimG", "0");
                XMLBuilder.Add("MyVehRimB", "0");
                XMLBuilder.Add("MyVehDirt", "0");
                XMLBuilder.Add("MyVehLivery", "-1");
                XMLBuilder.Add("MyVehTint", "-1");
                XMLBuilder.Add("MyVehPlate", "");
                XMLBuilder.Add("MyVehPlateStyle", "0");
                XMLBuilder.Add("MyVehHealth", "0");
                XMLBuilder.Add("MyVehEngineHealth", "0");
                XMLBuilder.Add("MyVehFuelTankHealth", "0");
                XMLBuilder.Add("MyVehRadio", "");
            }
            else
            {
                XMLBuilder.Add("MyVehicle", EntryPoint.GetVehicle.Model.Name.ToString());
                XMLBuilder.Add("MyVehLocX", EntryPoint.GetVehicle.Position.X.ToString());
                XMLBuilder.Add("MyVehLocY", EntryPoint.GetVehicle.Position.Y.ToString());
                XMLBuilder.Add("MyVehLocZ", EntryPoint.GetVehicle.Position.Z.ToString());
                XMLBuilder.Add("MyVehLocH", EntryPoint.GetVehicle.Heading.ToString());
                XMLBuilder.Add("MyVehPriR", EntryPoint.GetVehicle.PrimaryColor.R.ToString());
                XMLBuilder.Add("MyVehPriG", EntryPoint.GetVehicle.PrimaryColor.G.ToString());
                XMLBuilder.Add("MyVehPriB", EntryPoint.GetVehicle.PrimaryColor.B.ToString());
                XMLBuilder.Add("MyVehSecR", EntryPoint.GetVehicle.SecondaryColor.R.ToString());
                XMLBuilder.Add("MyVehSecG", EntryPoint.GetVehicle.SecondaryColor.G.ToString());
                XMLBuilder.Add("MyVehSecB", EntryPoint.GetVehicle.SecondaryColor.B.ToString());
                XMLBuilder.Add("MyVehRimR", EntryPoint.GetVehicle.RimColor.R.ToString());
                XMLBuilder.Add("MyVehRimG", EntryPoint.GetVehicle.RimColor.G.ToString());
                XMLBuilder.Add("MyVehRimB", EntryPoint.GetVehicle.RimColor.B.ToString());

                //XMLBuilder.Add("MyVehNeonR", NativeFunction.Natives.GetVehicleNeonLightsColor<int>(EntryPoint.GetVehicle).ToString());
                //XMLBuilder.Add("MyVehNeonG", EntryPoint.GetVehicle.RimColor.G.ToString());
                //XMLBuilder.Add("MyVehNeonB", EntryPoint.GetVehicle.RimColor.B.ToString());

                XMLBuilder.Add("MyVehDirt", EntryPoint.GetVehicle.DirtLevel.ToString());
                XMLBuilder.Add("MyVehLivery", NativeFunction.Natives.GetVehicleLivery<int>(EntryPoint.GetVehicle).ToString());
                XMLBuilder.Add("MyVehTint", NativeFunction.Natives.GetVehicleWindowTint<int>(EntryPoint.GetVehicle).ToString());
                XMLBuilder.Add("MyVehPlate", EntryPoint.GetVehicle.LicensePlate);
                XMLBuilder.Add("MyVehPlateStyle", NativeFunction.Natives.GetVehicleNumberPlateTextIndex<short>(EntryPoint.GetVehicle).ToString());
                XMLBuilder.Add("MyVehHealth", EntryPoint.GetVehicle.Health.ToString());
                XMLBuilder.Add("MyVehBodyHealth", NativeFunction.Natives.GetVehicleBodyHealth<float>(EntryPoint.GetVehicle).ToString());
                XMLBuilder.Add("MyVehEngineHealth", EntryPoint.GetVehicle.EngineHealth.ToString());
                XMLBuilder.Add("MyVehFuelTankHealth", EntryPoint.GetVehicle.FuelTankHealth.ToString());
                XMLBuilder.Add("MyVehRadio", NativeFunction.Natives.GetPlayerRadioStationName<string>());
            }

            // Get character Elements
            XMLBuilder.Add("MyLocX", Game.LocalPlayer.Character.Position.X.ToString());
            XMLBuilder.Add("MyLocY", Game.LocalPlayer.Character.Position.Y.ToString());
            XMLBuilder.Add("MyLocZ", Game.LocalPlayer.Character.Position.Z.ToString());
            XMLBuilder.Add("MyLocH", Game.LocalPlayer.Character.Heading.ToString());
            XMLBuilder.Add("MyWanted", Game.LocalPlayer.WantedLevel.ToString());
            XMLBuilder.Add("MyHealth", Game.LocalPlayer.Character.Health.ToString());
            XMLBuilder.Add("MyArmor", Game.LocalPlayer.Character.Armor.ToString());
            
            // Get World Elements
            XMLBuilder.Add("MyTime", World.TimeOfDay.ToString());
            XMLBuilder.Add("MyWeather", NativeFunction.Natives.GetPrevWeatherTypeHashName<int>().ToString());
            XMLBuilder.Add("MyPuddles", World.WaterPuddlesIntensity.ToString());
            XMLBuilder.Add("MyWindSpeed", NativeFunction.Natives.GetWindSpeed<float>().ToString());
            XMLBuilder.Add("MyWindDirection", NativeFunction.Natives.GetWindDirection<float>().ToString());

            // Now Write backup to XML file for later use
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
                foreach (KeyValuePair<string, string> XMLValue in XMLBuilder)
                {
                    xmlWriter.WriteStartElement(XMLValue.Key);
                    xmlWriter.WriteString(XMLValue.Value);
                    xmlWriter.WriteEndElement();
                }

                // End the XML
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }

            EntryPoint.Command_Notification("RecovFR: Backup complete...");
            return;
        }

    }
}
