# EgsEcfEditorApp
An application to simplify the handling and customizing of the .ecf configuration files of Empyrion Galactic Survival 


# Under Heavy Construction



## Content
- [Script Functions Overview](#script-functions-overview)
- [Pictures and Demonstration](#pictures-and-demonstration)
- [Configuration and Installation](#configuration-and-installation)
- [User Manual](#user-manual)
  - [Ingame Script Function Activation](#ingame-script-function-activation)
  - [Language Setting](#language-setting)
  - [Cargo Management Settings](#cargo-management-settings)
  - [Resetting Persistent Data](#resetting-persistent-data)
  - [Item Recognition](#item-recognition)
  - [Device Recognition](#device-recognition)






## Script Functions Overview
- `CpuInfDev` draws device damage states
- `CpuInfCpu` draws processor-lcd-device health information
- `CpuInfHll` draws a structure damage overview scheme for topview and sideview
- `CpuInfBox` draws container fill levels
- `CpuInfBay` draws docked vessel data
- `CpuInfThr` draws thruster-device health information (green=active, yellow=offline, red=missing)
- `CpuInfWpn` draws turret-device health information (green=active, yellow=offline, red=missing)
- `CpuCvrSrt` uses defined fill-levels and items-needs to try to preserve the item count or fill level for specified containers / tanks, additional sorts items from input containers to categorized containers
- `CpuCvrPrg` gather all items from near defined vessels (much logic/security conditions)
- `CpuCvrFll` reads item requests from near vessels and try to refill it (much logic/security conditions)
- `CpuInfS` draws compact information overview for smaller displays
- `CpuInfL` draws information overview for bigger displays

## Pictures and Demonstration
In the Steam workshop you can find a [Demo CV](https://steamcommunity.com/sharedfiles/filedetails/?id=2207655758) with this functions amongst other:

<table>
  <tr>
    <td>
      <figure>
        <figcaption>Compact Overview CpuInfS:</figcaption>
        <img src="images/CpuInfS.png" title="Compact Overview CpuInfS" width="400" height="400"/>
      </figure>
    </td>
    <td>
      <figure>
        <figcaption>Weapon Overview CpuInfWpn:</figcaption>
        <img src="images/CpuInfWpn.png" title="Weapon Overview CpuInfWpn" width="400" height="400"/>
      </figure>
    </td>
  </tr>
  <tr>
    <td>
      <figure>
        <figcaption>Structure Damage View CpuInfHll:</figcaption>
        <img src="images/CpuInfHll.png" title="Structure Damage View CpuInfHll" width="400" height="400"/>
      </figure>
    </td>
    <td>
      <figure>
        <figcaption>Container Overview CpuInfBox:</figcaption>
        <img src="images/CpuInfBox.png" title="Container Overview CpuInfBox" width="400" height="400"/>
      </figure>
    </td>
  </tr>
</table>

## Configuration and Installation
#### File Preparation
- Installing [EmpyrionScripting Mod](https://github.com/GitHub-TC/EmpyrionScripting)
- Copy "EgsEsExtension.dll" to ~\Empyrion\Games\\(savegamename)\Mods\EmpyrionScripting\CustomDLLs\
- Copy "EgsEsExtensionRun.cs" to ~\Empyrion\Games\\(savegamename)\Mods\EmpyrionScripting\Scripts\
- Copy "ItemStructureTree.ecf" to ~\Empyrion\Games\\(savegamename)\Mods\EmpyrionScripting\

#### Needed Configuration 
`@FilePath: ~\Empyrion\Games\(savegamename)\Mods\EmpyrionScripting\CsCompilerConfiguration.json`
- Add to `CustomAssemblies`: "CustomDLLs\\\\EgsEsExtension.dll"
- Add to `Usings`: "EgsEsExtension", "EgsEsExtension.Scripts", "EgsEsExtension.Locales"
- Add to `SymbolPermissions` under `SaveGame`: "EgsEsExtension", "EgsEsExtension.*"
<img src="images/Config.png" title="Example" width="400" height="400"/>

#### Recommended Configuration
`@FilePath: ~\Empyrion\Games\(savegamename)\Mods\EmpyrionScripting\Configuration.json`
- Set `SaveGameScriptsIntervallMS` to "5000"
- Set `EntityAccessMaxDistance` to "100"

## User Manual
### Ingame Script Function Activation
The scripts simulate a "processing device" behaviour. To go live ingame each script function needs at least one lcd device per structure/vessel the script should work on. The name of the lcd device has to begin with the exact [script-function name](#script-functions-overview). There can be more then one "cpu-lcd" "processing" each script function. Therefore the script function will NOT be executed multiple times. A second cpu-lcd is just a fallback device to keep the script active even if the first cpu-Lcd got for example destroyed. As long as one Lcd with the script function name is present on a structure the script stay active for this structure.
    
But a second "Cpu" device can be used to defined seperate/different drawing settings for different information displays. Add the optional seperator `:` after the script function name of the processing lcd device to have the possibility to define a name for information output displays. Each lcd device on the structure beginning with the so added name will get a continious information data stream and display it.

Add another optional `:` seperator to be able to define the font size for the before named displays. And add another optional `:` seperator to be able to define the output line count for the before named displays. If more lines still pending the script will wrap to a second column right next to the first column. So if the view is scattered just lower fontsize or raise line count to find the best fit for the display size/view. If no line count is defined all drawing items will be outputted in just one column as long as the display can draw it.

Certainly the script functions will even run without a information display. But the script functions will output at least basic processing information on the "processing cpu lcd" itself. If not needed or wanted just switch the processor lcd off in the control panel. The script functions will still be active, but no infos will be visible ingame.

At least lcd expanding is possible be adding a number from 0 to 9 at the end of the name on information lcds. Based on the output line count parameter the script functions will first try to go ahead on a slave/follower display before wrap around to a second column on the first display. The lcd location is expected as top to bottom by increasing number.

#### Example 1:
Runs script function `CpuCvrFll`, draws infos on all lcd devices name begin with "BoxInfo1" with standard font size and no column wrap

     - Lcd1 with CustomName: "CpuCvrFll:BoxInfo1"       -> Runs script function "CpuCvrFll"
     - Lcd2 with CustomName: "BoxInfo1"                 -> gets data from "CpuCvrFll:BoxInfo1"
     - Lcd3 with CustomName: "BoxInfo1"                 -> gets data from "CpuCvrFll:BoxInfo1"

#### Example 2 (on the same structure):
Runs no additional script function, but draws infos on all lcd devices name begin with "BoxInfo2" with font size 6 and will wrap to a new column right next to the least after every 20 lines

    - Lcd4 with CustomName: CpuCvrFll2:BoxInfo2:6:20    -> Runs script not twice, defines another display view
    - Lcd5 with CustomName: BoxInfo2                    -> gets data from "CpuBoxFill02:BoxInfo2:6:20"
    - Lcd6 with CustomName: BoxInfo2                    -> gets data from "CpuBoxFill02:BoxInfo2:6:20"

#### Example 3 (on the same structure):
Runs script `CpuCvrSrt`, draws infos on all lcd devices name begin with "SortInfo" with font size 4 and line/column wrap every 6 output lines

    - Lcd7 with CustomName: CpuCvrSrt:SortInfo:4:6      -> Runs script function "CpuCvrSrt"
    - Lcd8 with CustomName: SortInfo                    -> gets data from "CpuCvrSrt:SortInfo:4:6", 
                                                           no number at name-end means "0", will get the first 6 lines
                                                           
    - Lcd9 with CustomName: SortInfo                    -> gets data from "CpuCvrSrt:SortInfo:4:6", 
                                                           no number at name-end means "0", will get the first 6 lines
                                                           
    - Lcd10 with CustomName: SortInfo2                  -> gets data from "CpuCvrSrt:SortInfo:4:6", 
                                                           "2" at name-end means 2. display expansion, will get the third 6 lines
                                                           
    - Lcd11 with CustomName: SortInfo1                  -> gets data from "CpuCvrSrt:SortInfo:4:6", 
                                                           "1" at name-end means 1. display expansion, will get the second 6 lines
                                                           
    - Lcd12 with CustomName: SortInfo2                  -> gets data from "CpuCvrSrt:SortInfo:4:6", 
                                                           "2" at name-end means 2. display expansion, will get the third 6 lines

If more then 3 times 6 lines to draw the column wrap from example 2 takes place and the sequence starts over at lcd "0" in column "2"

#### Example 4 (on the same structure):
Display with no connection to these script functions

    - Lcd13 with CustomName: SortInf                    -> gets no data from no script

#### Example 5:
The script function `CpuInfHll` has a additional fifth parameter (after a additional `:`) which shifts the offset from center position of the "cut view through the structure" for the deck view display. The font and linecount parameter can additionally be added or not. If not the standard values will be used. If the fifth parameter is missing the structure center will be displayed.

    - Lcd14 with CustomName: CpuInfHll:HullInfo:::-3    -> Runs script function "CpuInfHll" and shifts view cut location by -3 block positions
    - Lcd15 with CustomName: HullInfo                   -> gets data from "CpuInfHll:HullInfo:::-3",
                                                           draws the structure view
    
### Language Setting
The script functions are designed to work with different languages. Actually deDE and enGB is implemented. Select the desired language in the script file `EgsEsExtensionRun.cs`. All item interactions will be done with local namings and headlines/messages will change, too.

### Cargo Management Settings
The both structure comprehensive script functions `CpuCvrPrg` and `CpuCvrFll` use a logical state model to simulate realistic dock supervision. For example: 
- If a vessel is offline a base will not recognize it (it not responses to radio request....)
- The faction id must be equal (no goods for enemies.... :) )
- and so on ...

All script functions with item-movement simulating a realistic transfer behaviour. The amount of transfered items per tick `SaveGameScriptsIntervallMS` is limited.

All script functions with item and cargo dependency rely on a "settings table" per structure. So for every structure/vessel the script function behaviour can be adjusted seperately. Therefore the structure needs a additional lcd device with a predefined customname:
- @deDE: Frachtverwaltung
- @enGB: Cargocontrol

The following table shows all settings the settings table can hold. Not all script functions needs all settings. The script function will tell if a expected setting (or the whole settings table lcd device) is missing. The format is one setting per line with `:` between setting name and setting value. Settings are grouped by a headline.

Just copy the [template table](#settings-Table-Copy-Template) to the editor panel of the lcd and change the values depending to the structure/vessel. Wildcard `*` on container names is allowed to specify a collection of containers.

###### Settings Table Description (not for copy)

    Output:on                           // should the structure provide/output items? "on" = on, any other text = off
    Input:on                            // should the structure except/collect items? "on" = on, any other text = off
    Priority:10                         // processing priority if queue needed, higher value = more priviledged

    SafetyStockEquip:mySpecialBox       // headline for item list of needed/requested items, value after ':' is the supervised box name, typically a "player accesable" one
    Pentaxid (veredelt):50              // example item to request, uses localized item name, value after ':' is the requested amount, 
                                        // all structure wide suitable items will be used to try to achive this amount

    SafetyStockAmmo:myAmmoBox           // headline for list of structure-needed/requested ammo, value after ':' is the supervised box name, typically the ammo-controller to feed guns and turrets
    Plasmaladung:200                    // example item to request, uses localized item name, value after ':' is the requested amount, 
                                        // all structure wide suitable items will be used to try to achive this amount

    SafetyStock:myBox1                  // headline for list of minimum inventory amount of listed items, value after ':' is the supervised box name, headline can be used multiple times for multiple boxes
    Sauerstoffflasche:100               // example item to hold level, uses localized item name, value after ':' is the amount level to hold,
                                        // all structure wide suitable items will be used to try to achive this amount

    FluidLevels:                        // headline for list of fuellevels
    FuelLevel:90                        // refill level for fuel, all structur wide suitable items will be used to try to achive this percentage level
    OxygenLevel:80                      // refill level for oxygen, all structur wide suitable items will be used to try to achive this percentage level
    PentaxidLevel:100                   // refill level for pentaxid, all structur wide suitable items will be used to try to achive this percentage level

    Container:                          // headline for list of containers
    OutputBox:myBox*                    // CustomName of the box(es) which should be purged by near bases (CpuCvrPrg) or from which items shall be used to refill near vessels(CpuCvrFll)
    InputBox:myInputBox                 // CustomName of the box(es) which should receive items purged from near vessels (CpuCvrPrg) or which items shall be transfered to from near base (CpuCvrFll), additional the source for sorting to categorized boxes (CpuCvrSrt)
    StockSourceBox:myLittleBox          // CustomName of the box(es) from which the (CpuCvrSrt) collects the items to refill all SafetyStock containers
    OreBox:myBox1                       // CustomName of the box(es) the items of group "ore" will be sorted to
    IngotBox:myBox1                     // CustomName of the box(es) the items of group "ingot" will be sorted to
    ComponentBox:myBox1                 // CustomName of the box(es) the items of group "component" will be sorted to
    BlockLargeBox:myBox1                // CustomName of the box(es) the items of group "block_L" will be sorted to
    BlockSmallBox:myBox1                // CustomName of the box(es) the items of group "block_S" will be sorted to
    MedicBox:myBox1                     // CustomName of the box(es) the items of group "medical" will be sorted to
    FoodBox:myBox1                      // CustomName of the box(es) the items of group "food" will be sorted to, hopefully a fridge :)
    IngredientBox:myBox1                // CustomName of the box(es) the items of group "ingredient" will be sorted to, hopefully a fridge :)
    SproutBox:myBox1                    // CustomName of the box(es) the items of group "sprout" will be sorted to
    EquipBox:myBox2*                     // CustomName of the box(es) the items of group "equip" will be sorted to
    ModBox:myBox2*                       // CustomName of the box(es) the items of group "mod" will be sorted to
    DeviceLargeBox:myBox2*               // CustomName of the box(es) the items of group "device_L" will be sorted to
    DeviceSmallBox:myBox2*               // CustomName of the box(es) the items of group "device_S" will be sorted to
    WeaponBox:myBox2*                    // CustomName of the box(es) the items of group "player weapon" will be sorted to
    AmmoBox:myBox1                      // CustomName of the box(es) the items of group "ammo" will be sorted to (only if not needed for SafetyStockAmmo)
    RefillsBox:myBox1                   // CustomName of the box(es) the items of group "refills" will be sorted to (only if not needed for tank refilling)
    TreasureBox:myVault                 // CustomName of the box(es) the items of group "treasure items" will be sorted to

###### Settings Table Copy-Template

    Output:on
    Input:on
    Priority:10

    SafetyStockEquip:mySpecialBox
    Pentaxid (veredelt):50

    SafetyStockAmmo:myAmmoBox
    Plasmaladung:200

    SafetyStock:myBox1
    Sauerstoffflasche:100
    
    FluidLevels:
    FuelLevel:90
    OxygenLevel:80
    PentaxidLevel:100

    Container:
    OutputBox:myBox*
    InputBox:myInputBox
    StockSourceBox:myLittleBox
    OreBox:myBox1
    IngotBox:myBox1
    ComponentBox:myBox1
    BlockLargeBox:myBox1
    BlockSmallBox:myBox1
    MedicBox:myBox1
    FoodBox:myBox1
    IngredientBox:myBox1
    SproutBox:myBox1
    EquipBox:myBox2*
    ModBox:myBox2*
    DeviceLargeBox:myBox2*
    DeviceSmallBox:myBox2*
    WeaponBox:myBox2*
    AmmoBox:myBox1
    RefillsBox:myBox1
    TreasureBox:myVault

### Resetting Persistent Data
Some script functions use persistent data storage to accomplish a report about missing(destroyed) devices. These persistent data will remain until the game/server restarts. Therefore if the displayed information on a lcd is outdated it could be necessary to reset these data manually (after a conscious redesign of a structure for example).

Just add a lcd with the custom name `ResetData` and the structure the lcd is located on will be cleared from storage. Remind to rename/remove the lcd after the success message appears on these lcd to restore the normal functions of the data-using script functions.

### Item Recognition
If a display outputs the message "unknown item" the desired item-id is propably missing in the `ItemStructureTree.ecf` file. In this case you could add the missing id from the display to one of existing groups in the file. (`,` seperated)
    
### Device Recognition
Due to a special(silly :) ) eleon api interface behaviour the script functions with generic device supervision/recognition only works if the desired device(s) have a custom name set. So rename all devices from which you expect to be recognized. It is suitable to remove at least one letter from the default name in the controlpanel and hit enter to change "default name" to "custom name" and make the device "visible" to the Api.
