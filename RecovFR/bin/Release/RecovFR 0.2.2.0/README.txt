# RecovFR
Backing up and restoring game play on the fly... [Work in Progress]

Installation: 
1. Copy the plugins folder into your "Grand Theft Auto V" install folder 
2. Edit the keybindings in the .ini to meet your requirements
** Do not need to replace .ini file if updating from previous version **

Current features:
- Backup stored in xml file, so even after full game crash or exit, backup can be restored
- Backup the following states at the press of a button or automatically (if enabled): 
  - Current character location
  - Current vehicle (if in vehicle)
  - Last vehicle and location (if on foot)
  - Vehicle colors, livery and dirt level
  - License Plate and style
  - Vehicle health
  - Wanted level
  - Health and armor
  - Current time in game
  - Current weather conditions
- Restore last backup at press of button

Planned features (If I am clever enough to work it out): 
- Backup / restore the following information: 
  - Current character model
  - If character is wet or dry
  - Current uniform and accessories
  - Current weapons, components and ammo
  - Vehicle neon light configurations
- Restructure XML for better reading
- Ability to turn on/off features in .ini file

Version history: 
0.2.2.0 Pre-release ("Whatever the weather update")
- Backup code optimisation
- Fixed DLC vehicle restore failure
- Fixed Vehicle plate style not restored
- Added vehicle health levels
- Added vehicle radio station
- Added vehicle livery, rim color and window tint
- Added Weather, wind speed and wind direction


0.2.1.0 Pre-release
- No longer dependent on LSPDFR (can be used separately)
- Fix game crash on restore if vehicle invalid
- Added vehicle color and dirt levels
- Added license plate and style
- Added wanted level
- Added health and armor

0.1.2.0 Pre-release
- Initial release for testing
