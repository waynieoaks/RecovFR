# RecovFR
Backing up and restoring game play on the fly... [Work in Progress]

Current features:
- Backup stored in xml file, so even after full game crash or exit, backup can be restored
- Backup the following states at the press of a button or automatically (if enabled): 
  - Current character location
  - Current vehicle (if in vehicle)
  - Last vehicle and location (if on foot)
  - Vehicle colors and dirt level
  - License Plate and style
  - Wanted level
  - Health and armor
  - Current time in game
- Restore last backup at press of button

Planned features: 
- Backup of the following information: 
  - Current character model (if possible)
  - Current character uniform (From EUP, if possible)
  - Current character weapons loadout and ammo
  - Current weather conditions 
  
Known issues: 
- Some DLC vehicles cannot be recovered, investigating cause

Installation: 
1. Copy the plugins folder into your "Grand Theft Auto V" install folder 
2. Edit the keybindings in the .ini to meet your requirements

Version history: 
0.2.1.0 Pre-release
- No longer dependent on LSPDFR (can be used separately)
- Fix game crash on restore if vehicle invalid
- Added vehicle color and dirt levels
- Added license plate and style
- Added wanted level
- Added health and armor

0.1.2.0 Pre-release
- Initial release for testing
