# RecovFR
Backing up and restoring game play on the fly... [Work in Progress]

Installation: 
1. Copy the plugins folder into your "Grand Theft Auto V" install folder 
2. Edit the keybindings in the .ini to meet your requirements
** Note: previous backups will not work with this version **

Current features:
- Backup stored in xml file, so even after full game crash or exit, backup can be restored
- Backup the following states at the press of a button or automatically (if enabled): 
  - Current character location
  - Current clothing and accessories
  - Current weapons, components and ammo
  - Wanted level
  - Health and armor
  - Player invincibility* 
  - Current vehicle (if in vehicle)
  - Last vehicle and location (if on foot)
  - Vehicle colors, livery and dirt level
  - License Plate and style
  - Vehicle health
  - Vehicle invincibility*
  - Current time in game
  - Current weather conditions
  - Freeze time and weather on restore*
  - Set snow on terrain on restore*
- Restore last backup at press of button
- Ability to turn on/off features in .ini file or via in game menu

*Restore options set in game

Planned features (If I am clever enough to work it out): 
- Backup / restore the following information: 
  - Current character model

Version history: 
0.4.1.0 Pre-Release (The All Dressed-Up-Date)
- Added character cloting and accessories
- Added simple in-game menu
- Code tidy 
(Thank you opus49 for helping with my code and sanity)

0.3.1.1 Pre-Release (The I Messed-Up-Date)
- Removed character model from restore (critical bug)
- Fixed flashlight missing from restore

0.3.1.0 Pre-Release (The Tooled-Up-Date)
- Added weapons, ammo and components
- Added character model
- Enable player and vehicle invincibility on restore (from ini file)
- Set time and weather frozen on restore (from ini file)
- Enable snow on ground on restore (from ini file)
- INI and XML files restructured
- Better error handling
- Fixed plugin crash if restoring backup before entering a vehicle
- Fixed weather not changing from neutral to snow

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