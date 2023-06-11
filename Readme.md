# <img src="EgsEcfEditorApp\Icon_AppBranding.ico" title="icon" width="32" height="32"/> Empyrion Configuration Editor <img src="EgsEcfEditorApp\Icon_AppBranding.ico" title="icon" width="32" height="32"/>
An application to simplify the handling and customizing of the `.ecf` configuration files of [Empyrion Galactic Survival](https://empyriongame.com/)

<img src="images/tool_teaser.png" title="Tool Teaser" width="1000" height="500"/>

## Content
- [Motivation](#motivation)
- [Installation](#installation)
- [Feature Overview](#feature-overview)
- [Tool Overview](#tool-overview)
- [Operations Overview](#operations-overview)
- [Shortcuts and Functions](#shortcuts-and-functions)
- [File Content Definition](#file-content-definition)
- [File Content Recognition](#file-content-recognition)
- [Planned Major Features](#planned-major-features)

## Motivation
Over a long time lightweight modding [Empyrion Galactic Survival](https://empyriongame.com/) was really simple by adjusting the `config_example.ecf`. Due to the mechanic that the changes were added to the default settings, just the additional adjustments must be maintained. The decision of Eleon to remove these `add adjustments` feature simply felt like:

<img src="https://media.giphy.com/media/h36vh423PiV9K/giphy.gif" width="300" height="300">

Now all the whole bunch of tons of settings must be maintained at once even if adjusting just one tiny value of one silly block. The awkward `.ecf` format makes this feel like:

<img src="https://media.giphy.com/media/xT5LMAvRY92qUXj7dC/giphy.gif" width="300" height="250">

You know what i'm talking about? Here comes the solution!

<img src="https://media.giphy.com/media/5Y2bU7FqLOuzK/giphy.gif" width="300" height="250">

## Installation
Just download the latest release and unzip the content wherever you might need to. Run the executeable file and have fun!

## Feature Overview
### Content Definition
For each `.ecf` file the tool needs a definition. These definitions are located in `.xml` files in the `EcfFileDefinitions` sub folder of the zip package. For creating or adjusting the definitions yourself refer to [File Content Definition](#file-content-definition). After loading a `.ecf` file the definition is attached to it. To reinterprete a `.ecf` file with a different or changed definition the `.ecf` file must be closed and reopened. The actual version of the tool is shipped with definitions for:
- `BlockGroupsConfig.ecf` (Vanilla and Reforged Eden)
- `BlocksConfig.ecf` (Vanilla and Reforged Eden)
- `BlockShapeWindow.ecf` (Vanilla)
- `Containers.ecf` (Vanilla and Reforged Eden)
- `DefReputation.ecf` (Vanilla and Reforged Eden)
- `EClassConfig.ecf` (Vanilla and Reforged Eden)
- `EGroupsConfig.ecf` (Vanilla and Reforged Eden)
- `Factions.ecf` (Vanilla and Reforged Eden)
- `FactionWarfare.ecf` (Vanilla and Reforged Eden)
- `GalaxyConfig.ecf` (Vanilla and Reforged Eden)
- `GlobalDefsConfig.ecf` (Vanilla and Reforged Eden)
- `ItemsConfig.ecf` (Vanilla and Reforged Eden)
- `LootGroups.ecf` (Vanilla and Reforged Eden)
- `MaterialConfig.ecf` (Vanilla and Reforged Eden)
- `StatusEffects.ecf` (Vanilla and Reforged Eden)
- `Templates.ecf` (Vanilla and Reforged Eden)
- `TokenConfig.ecf` (Vanilla and Reforged Eden)
- `TraderNPCConfig.ecf` (Vanilla and Reforged Eden)

### Content Recognition
At file loading the tool parses the file content according to the attached definition. The tool is balanced to the variety of the subtleties of the spellings in the `.ecf` files. Due to the design goal `the output should match input in at much details as possible` the tool in its release state will likely also report failures in the default `.ecf` files of the game. This is not a tool bug, but developer inaccuracies that may or may not be compensated for by a fallback. In order to achieve reliable behavior, I have chosen to report such bugs rather than legitimizing these inaccuracies by the definition. For details see [File Content Recognition](#file-content-recognition).

### Content Creation
At saving a `.ecf` file the whole content in the file is wiped and recreated. At default setting elements with unsolved errors will not be written to the file. Instead the tool tries to recreate the data originially read from the file. Due to the nature of the `.ecf` syntax of consecutive specifications its recommended to
```diff
- solve errors from top to bottom
```
to not follow rabbits.

### Language and Tool Support
The icons and controls have tooltips on mouse over. All labels and tootips are localised. At the moment de-DE and en-GB is supported. The language switches automaticly based on the local machine culture setting, defaulting to en-GB.

### Add, Edit, Remove Content
The basic functionality of this tool is the alteration of any `.ecf` content.

### Mass Changing
:wrench: Not implemented yet :wrench:

### Compare and Merge
The tool is capable of comparing two `.ecf` files, listing the differences and offer several options to update the opposite file partly or completely.

### Tech Tree Editor
The tool is capable of editing the "TechTree" which is based on several properties spreaded over different files and file elements.

### Item Handling Support
The context menu provides several functions like adding, removing or listing linked elements of corresponding `.ecf` files depending on which element is clicked and to which file type the item belongs to.

## Tool Overview

<img src="images/Area_Overview.png" title="Areas" width="1000" height="500"/>

### File Operation Area (1)
The standard file operations (new, open, reload, save, close) are located in this area. The cross-file functions (diff, merge, xml) are also arranged here.

### Filter and File Selection Area (2)
In this area each opened file will get a tab containing the file name. The label in the tool line indicates the attached game mode and content definition, for example `Vanilla` and `BlocksConfig`. The remaining icons provide different filter options applied to all content view areas.

### Content Operation Area (3)
The tools in this area provide content altering options like adding, editing or removing elements. The copy/paste function is located here, too.

### Settings Area (4)
The label shows the actual selected game mode and grants a temporary quick change option on click. The gear button opens the persistent settings menu. 

### Tree View Area (5)
The tree view area brings the structural overview. The root elements, child elements, parameters and comments are displayed in this view. If an element has an error the entry in this view will turn red.

### Parameter View Area (6)
The parameter view area shows the detail information of any parameter correlating to the selected tree element. Additionally the view analyzes and displays the inheritance dependancies to referenced elements to provide a overview over all parameters effecting the selected element. If an parameter has an error the entry in this view will turn red.

### Info View Area (7)
The info area displays additional detail information for the selected tree element and the selected parameter. Especially the element attributes (e.g `formatter`) can be found here.

### Error View Area (8)
In the error view all occured errors are listed. The view shows the error category and type together with additional information like line in file (if applicable) and the error producing data part.

## Operations Overview
### Changing settings
The gear button ( <img src="EgsEcfEditorApp\Resources\Icon_Settings.png" title="icon" width="16" height="16"/> ) opens a settings panel with several settings. The `abort` button closes the panel and discarding all changes. The `save` button saves the settings persistent and closes the panel. The `reset` button reloads the default settings.

<img src="images/app_settings_panel.png" title="App Settings Panel"/>

### Opening a file
After selecting the file to open the tool tries to guess which definition fits to the file by examining the file name. If this is successful the guessed definition is automaticly selected in the following dialog. In this case the Dialog can be skipped with `Ok`. When the guessing fails or a new file is created the definition must be selected manually from the provided drop down list.

<img src="images/preload_settings.png" title="File Property Selector"/>

For especially big files like `BlocksConfig.ecf` (or especially lame PCs :zany_face:) a loading dialog is shown while parsing the file content. 

<img src="images/loading_content.png" title="Loading Dialog"/>

### Adding And Editing Content
At adding or editing (see [Shortcuts and Functions](#shortcuts-and-functions)) the editor dialog is shown. The dialog is designed to not produce elements with errors. To achive this support pre opening checks, pre filled selection lists and pre closing checks will be performed. These logics depends primary on the attached file definition and the present content data.

At multi selection the panel will normally open for the first selected element. If only parameters are selected (even across parent element borders) the panel will enter a matrix editing mode. This mode provides a table organized structure for matrix arranged parameters or to edit equal parameters of different parents in one view.

<img src="images/editing_dialog.png" title="Editing Dialog"/>

### Comparing and Merging Files
In the compare and merge function ( <img src="EgsEcfEditorApp\Resources\Icon_CompareAndMerge.png" title="icon" width="16" height="16"/> ) the compare is started by selecting two of the opened files. Depending on file size a progress bar will be displayed until completion.

The two tree views will display all elements which differs between the two files. If no elements are displayed the two files are completely equal. The displayed elements belongs to the three categories:
- <img src="EgsEcfEditorApp\Resources\Icon_Add.png" title="icon" width="16" height="16"/> add / element will be created in opposite file
- <img src="EgsEcfEditorApp\Resources\Icon_Unequal.png" title="icon" width="16" height="16"/> unequal / element will be updated in opposite file
- <img src="EgsEcfEditorApp\Resources\Icon_Remove.png" title="icon" width="16" height="16"/> remove / element will be deleted in opposite file

After clicking one of the merge buttons ( <img src="EgsEcfEditorApp\Resources\Icon_MoveLeft.png" title="icon" width="16" height="16"/> / <img src="EgsEcfEditorApp\Resources\Icon_MoveRight.png" title="icon" width="16" height="16"/> ) all checked elements behind the arrow will be merged into the file the arrow points to. 

With the buttons above the tree view its possible to check / uncheck all elements of the corresponding category. In the tree view itself single elements can be checked / unchecked. The buttons above the tree view will display an indeterminated state to indicate the mixed state across pages of elements.

To maintain a suitable performance only a limited amount of diff elements will be displayed at once. If this limit is exceeded all additional elements will be moved to a second (or more) page. The two page select buttons ( <img src="EgsEcfEditorApp\Resources\Icon_MoveUp.png" title="icon" width="16" height="16"/> / <img src="EgsEcfEditorApp\Resources\Icon_MoveDown.png" title="icon" width="16" height="16"/> ) change the displayed page.

Clicking an element in the tree view will trigger the detail difference display at the lower window. The two text boxes again will only display differences. If this boxes are empty the selected element has no difference to the opposite element. If an element is marked as unequal but the details boxes shows no difference only a structural sub element contains a difference.

<img src="images/CompareAndMerge.png" title="Compare and Merge Dialog"/>

### Tech Tree Editing
At clicking the Tech Tree Editor function ( <img src="EgsEcfEditorApp\Resources\Icon_TechTreeEditor.png" title="icon" width="16" height="16"/> ) the tool parses the content of all opened files and shows the computed resulting Tech Trees as tabs. The parameters used for the computation can be altered within the new settings section `Tech Tree` found in the settings window. If new Tech Trees are added the localized name must be added manually to the localisation file of Empyrion. The Editor just uses the `.ecf` internal names. The tool can add, remove, rename and copy Tech Trees. Additionally it can add, remove, change and copy element details and element structures.

The element order question is a difficult one. Empyrion uses a ordering logic for the ingame display of the Tech Trees which is not predictable. This tool orders the elements as they appear in the `.ecf` files. The ingame order may vary but the tree element dependencies are not effected from this.

New elements can only be added if they are not already used in a Tech Tree. Elements which are already in a Tech Tree can only be copied to align with the mirrowing behaviour of Empyrion. A element must have the exact equal parent element structure in all containing Tech Trees. To preserve the integrity across the Tech Trees changes violating this rule will automaticly be spreaded to all Tech Trees the copied element belongs to.

All changes done in the Tech Tree Editor will directly effect the corresponding opened files. After closing the Tech Tree Editor window the files need just to be saved.

<img src="images/TechTreeEditorTeaser.png" title="Tech Tree Editor Dialog"/>

### Using Item Handling Support
Item Handling Support provides functions to manipulate (add, remove, list, show) elements across the border of `.ecf` files. The function will be shown context sensitive for elements and files for which a specific function is valid. All support functions check and report warnings for possible dependency breaks before a specific function is performed. If the warning is confirmed the function will be performed anyway. The possibly resulting error will `not` be listed in the error view since the tool cannot determime if an error is logically located in the source file or linked file. Additionally the dependency checks are integrated in the generic remove and change function.

The support functions only consider `.ecf` files which are opened in the tool. If a function reports that nothing could be found first check if the corresponding file is opened. For the first approach this contextual dependencies are implemented for:
- `BlockGroupsConfig.ecf`
- `GlobalDefsConfig.ecf`
- `Templates.ecf`

The settings menu has a new page for the item handling support functions options. Along with several options for parameter keys and presettings the dependency check in the generic item operations can be disabled. 

With the Item Handling Support the definition file handling is extended too. A parameter can be added to a definition from within the tool. This can be the own definition (missing definition error) or the definition of another 'ecf' file (needed for dependency) if the context requires it. The 'online' changeability of definitions comes together with the function to reinterprete (reload) already opened files with the changed definition. After adding a parameter to a definition the tool asks to reinterprete corresponding files. The reinterpretaretation can be performed manually too. For example after changing the game mode.

<img src="images/ItemHandlingSupport.png" title="Item Handling Support Context Menu Example"/>

## Shortcuts and Functions
### Shortcuts
- `double-click` opens the edit panel for the clicked item
- `right-click` opens the context panel for the clicked item
- `delete` key removes the selected items
- `strg + c` key copies the selected items to the clipboard
- `strg + v` key pastes the copied items into the selected item, or after it if insertion is not allowed for the item

### Context Panel
#### Tree View Context Panel
The context panel of the tree view provides access to the content operations suitable for tree items.

<img src="images/tree_context.png" title="Tree Context Panel"/>

#### Parameter View Context Panel
The context panel of the parameter view provides access to the content operations suitable for parameter items.

<img src="images/parameter_context.png" title="Parameter Context Panel"/>

#### Error View Context Panel
The context panel of the error view has support functions for error tracking.

<img src="images/error_context.png" title="Error Context Panel"/>

### Sorting
The three listing views [Tree View Area](#tree-view-area), [Parameter View Area](#parameter-view-area) and [Error View Area](#error-view-area) have a sorting panel each. The sort panel provides settings for concurrent items and various sorting options.

<img src="images/sorting_panel.png" title="Sorting Panel"/>

### Icons
#### Settings
- <img src="EgsEcfEditorApp\Resources\Icon_Settings.png" title="icon" width="16" height="16"/> opens the settings panel

#### File Operations
- <img src="EgsEcfEditorApp\Resources\Icon_NewFile.png" title="icon" width="16" height="16"/> create new file
- <img src="EgsEcfEditorApp\Resources\Icon_OpenFile.png" title="icon" width="16" height="16"/> open existing file
- <img src="EgsEcfEditorApp\Resources\Icon_ReloadFile.png" title="icon" width="16" height="16"/> reload content from selected file
- <img src="EgsEcfEditorApp\Resources\Icon_SaveFile.png" title="icon" width="16" height="16"/> save selected file
- <img src="EgsEcfEditorApp\Resources\Icon_SaveAsFile.png" title="icon" width="16" height="16"/> save selected file with new name
- <img src="EgsEcfEditorApp\Resources\Icon_SaveAllFiles.png" title="icon" width="16" height="16"/> save all opened files
- <img src="EgsEcfEditorApp\Resources\Icon_CloseFile.png" title="icon" width="16" height="16"/> close selected file
- <img src="EgsEcfEditorApp\Resources\Icon_CloseAllFiles.png" title="icon" width="16" height="16"/> close all open files
- <img src="EgsEcfEditorApp\Resources\Icon_CompareAndMerge.png" title="icon" width="16" height="16"/> compares two files, displays differences and offer merge options (see [Compare and Merge](#compare-and-merge))
- <img src="EgsEcfEditorApp\Resources\Icon_TechTreeEditor.png" title="icon" width="16" height="16"/> displays the tech tree editor (see [Tech Tree Editor](#tech-tree-editor))
- <img src="EgsEcfEditorApp\Resources\Icon_ReloadDefinitions.png" title="icon" width="16" height="16"/> reloads the `.xml` definitions (see [File Content Definition](#file-content-definition))
- <img src="EgsEcfEditorApp\Resources\Icon_ReplaceDefinition.png" title="icon" width="16" height="16"/> replaces the `.xml` definition in selected file (see [Item Handling Support](#item-handling-support))
- <img src="EgsEcfEditorApp\Resources\Icon_CheckDefinition.png" title="icon" width="16" height="16"/> checks the `.xml` definitions (see [File Content Definition](#file-content-definition))

#### Filter and Sorting Operations
- <img src="EgsEcfEditorApp\Resources\Icon_ApplyFilter.png" title="icon" width="16" height="16"/> apply filter
- <img src="EgsEcfEditorApp\Resources\Icon_ClearFilter.png" title="icon" width="16" height="16"/> clear filter
- <img src="EgsEcfEditorApp\Resources\Icon_ShowAllItems.png" title="icon" width="16" height="16"/> show item with / without error
- <img src="EgsEcfEditorApp\Resources\Icon_SortDirectionAscending.png" title="icon" width="16" height="16"/> switch the sort direction
- <img src="EgsEcfEditorApp\Resources\Icon_NumericSorting.png" title="icon" width="16" height="16"/> sets sorting to the origin (file) order
- <img src="EgsEcfEditorApp\Resources\Icon_AlphabeticSorting.png" title="icon" width="16" height="16"/> sets sorting to alphabetic order

#### Content Operations
- <img src="EgsEcfEditorApp\Resources\Icon_Add.png" title="icon" width="16" height="16"/> add item to / after selection
- <img src="EgsEcfEditorApp\Resources\Icon_ChangeSimple.png" title="icon" width="16" height="16"/> editing first selected item
- <img src="EgsEcfEditorApp\Resources\Icon_ChangeComplex.png" title="icon" width="16" height="16"/> editing items with logics (see [Mass Changing](#mass-changing))
- <img src="EgsEcfEditorApp\Resources\Icon_Remove.png" title="icon" width="16" height="16"/> removing selected items
- <img src="EgsEcfEditorApp\Resources\Icon_Copy.png" title="icon" width="16" height="16"/> copy selected items
- <img src="EgsEcfEditorApp\Resources\Icon_Paste.png" title="icon" width="16" height="16"/> paste copied items to / after selection
- <img src="EgsEcfEditorApp\Resources\Icon_Undo.png" title="icon" width="16" height="16"/> undo last change
- <img src="EgsEcfEditorApp\Resources\Icon_Redo.png" title="icon" width="16" height="16"/> redo last undo
- <img src="EgsEcfEditorApp\Resources\Icon_MoveUp.png" title="icon" width="16" height="16"/> move selected items up
- <img src="EgsEcfEditorApp\Resources\Icon_MoveDown.png" title="icon" width="16" height="16"/> move selected items down
- <img src="EgsEcfEditorApp\Resources\Icon_AddValue.png" title="icon" width="16" height="16"/> add value slot after selection
- <img src="EgsEcfEditorApp\Resources\Icon_AddValueGroup.png" title="icon" width="16" height="16"/> add value group after selection
- <img src="EgsEcfEditorApp\Resources\Icon_RemoveValue.png" title="icon" width="16" height="16"/> remove selected value slot
- <img src="EgsEcfEditorApp\Resources\Icon_RemoveValueGroup.png" title="icon" width="16" height="16"/> remove selected value group

#### Error Operations
- <img src="EgsEcfEditorApp\Resources\Icon_ShowInEditor.png" title="icon" width="16" height="16"/> show error in tool
- <img src="EgsEcfEditorApp\Resources\Icon_ShowInFile.png" title="icon" width="16" height="16"/> show error in file

#### Compare and Merge
- <img src="EgsEcfEditorApp\Resources\Icon_AllAddItemsSet.png" title="icon" width="16" height="16"/> change checkmark of all "add" items
- <img src="EgsEcfEditorApp\Resources\Icon_AllUnequalItemsSet.png" title="icon" width="16" height="16"/> change checkmark of all "unequal" items
- <img src="EgsEcfEditorApp\Resources\Icon_AllRemoveItemsSet.png" title="icon" width="16" height="16"/> change checkmark of all "remove" items
- <img src="EgsEcfEditorApp\Resources\Icon_MoveRight.png" title="icon" width="16" height="16"/> process all checked items to the opposite file
- <img src="EgsEcfEditorApp\Resources\Icon_MoveUp.png" title="icon" width="16" height="16"/> switch displayed page

#### Item Handling Support
- <img src="EgsEcfEditorApp\Resources\Icon_ListTemplates.png" title="icon" width="16" height="16"/> list linked items (for example templates)
- <img src="EgsEcfEditorApp\Resources\Icon_ShowTemplate.png" title="icon" width="16" height="16"/> show linked item (for example template)
- <img src="EgsEcfEditorApp\Resources\Icon_AddTemplate.png" title="icon" width="16" height="16"/> add item of linked type (for example template)
- <img src="EgsEcfEditorApp\Resources\Icon_DeleteTemplate.png" title="icon" width="16" height="16"/> remove item of linked type (for example template)
- <img src="EgsEcfEditorApp\Resources\Icon_AddToDefinition.png" title="icon" width="16" height="16"/> add item to linked definition (for example template)

## File Content Definition
The file content definition is the basic information for the tool which content is viable in the loaded `.ecf` file and which is not. To achive the design goal to be reliable able to load and interprete `.ecf` files from any source (default files, text editor tool files, manually edited files and so on) the definition provides several options. 

The definitions will be read from the `EcfFileDefinitions` sub folder from the tool root directory. The file name doesn't matter. Each file in this directory will be read, even in sub directories. The first entry in each file must contain the game mode and the definition name, both also shown in the tool. The definition name is the link to the corresponding `.ecf` file (for guessing) and must be unique. Every further `.xml` file with the same `mode` and `type` setting will be ignored.

<img src="images/xml_type.png" title="XML Type"/>

If the `EcfFileDefinitions` sub folder is missing or empty the tool will create a simple template `.xml` file for `BlocksConfig.ecf`. This file can be used as starting point for creation of own files and is structural complete, but will likely be incomplete compared to the expected `.ecf` content.

After starting the tool the present `.xml` files are loaded and prepared for use. Changing the `.xml` files after running the tool will not be recognized by the tool. If you want the tool to reload the definitions just hit the respective button ( <img src="EgsEcfEditorApp\Resources\Icon_ReloadDefinitions.png" title="icon" width="16" height="16"/> ). Due to the intended behaviour to not alter the definitions of opened `.ecf` files, reloading the '.xml' definitions or changing the game mode setting not effecting already opened `.ecf` files.

After bigger patches of [Empyrion Galactic Survival](https://empyriongame.com/) a definition maybe can partly went deprecated. In this case a backwards check of the definition is recommended. This can be done (if I'm too lame to release an update :sleeping:) by hitting the respective button ( <img src="EgsEcfEditorApp\Resources\Icon_CheckDefinition.png" title="icon" width="16" height="16"/> ). This function compares the actual selected `.ecf` file against the latest loaded `.xml` definition of the same type and lists the `.xml` content definition for which no content is found in the `.ecf` file.

### Xml File Content
The definition uses often the terminology `Block`. Don't missinterprete this with an `Empyrion ingame block`. In Xml-speaking `Block` stands for a `.ecf data block`.

The `formatting` chapter of the `.xml` files provides settings for the basic `.ecf` syntax. This chapter should mostly be similar for all `.ecf` files.

<img src="images/xml_formatting.png" title="XML ECF Formatting"/>

After the `formatting` follow the sub chapters for the different elements of the `.ecf` files. This chapter contains the expected `.ecf` content.

<img src="images/xml_ecf_content.png" title="XML ECF Content"/>

The available chapters are:

- `BlockTypePreMarks` the expected pre marks after the block opener (e.g. `+`)
- `BlockTypePostMarks` the expected seperators to define the end of the block type (e.g. ` `)
- `RootBlockTypes` the expected data types of blocks in the root level (e.g. `Block`)
- `RootBlockAttributes` the expected attributes for blocks in the root level (e.g. `Id`) 
- `ChildBlockTypes` the expected data types of blocks not in the root level (e.g. `Child`)
- `ChildBlockAttributes` the expected attributes for blocks not in the root level (e.g. `DropOnDestroy`)
- `BlockParameters` the expected parameters for the data blocks (e.g. `Material`)
- `ParameterAttributes` the expected attributes for the parameters (e.g. `formatter`)

Each `.xml Param` line item can hold different switches to control the behaviour of the error checking while parsing and editing the `.ecf` content. The chapters are allowed to be empty, but remind that the respective functions in the [Editor Panel](#adding-and-editing-content) will be switched off without the definition set.

<img src="images/xml_options.png" title="ECF Options"/>

- `name` the name of the `.ecf` element (e.g. `Material`)
- `optional` determines if this element is optional, non-optional elements will be treated as mandatory and can in some cases overwrite optional ones
- `hasValue` determines if this element has (reading) / should have (creation) a value
- `allowBlank` determines if for this element a `empty value` is allowed 
- `forceEscape` determines if at creation this element must be escaped, useful for text references like `TechTreeParent`
- `info` a free text to preserv designer infos for the element, shown in the tool, not written to the file

## File Content Recognition
The Parsing of the `.ecf` tries to interprete the whole variety of spellings present in the different files. To minimize result validation efforts and to preserv the readability of the files by [Empyrion Galactic Survival](https://empyriongame.com/), the tool reproduces nearly the same spelling as the original file.

The exceptions are the white spaces, the empty lines and the comments. All the function-irrelevant white spaces and empty lines will be remove. All the different varieties of comment spellings will be streamlined to appended inline comments.

The tool parses the `.ecf` file content line by line and seperates the line content item by item according to the definition and chronologic. A fault can depending on its severity lead to a whole bunch of follower errors. This is the reason of solving errors from top to bottom.

In the settings are several behaviour adjustments possible how the tool should handle errors at writing content to the file:
- `Write only valid items to file` Unchecking this option activates the behaviour that all elements will be written in its current state from within the tool. Notice that the file is the persistent storage. After loading a file written with this option unchecked several informations and the resulting errors will be gone.
- `Invalidate parent of invalid item` Checking this option activates the behaviour that error states will be inherited structure upwards. A error of a sub element invalidates its containing element upto the root element. 
- `Allow fallback to original data` Unchecking this option activates the behaviour that elements with errors has no permission to try to use the data from the original file. In this case a creation error will be reported. Notice that even with this option checked the error could occur for newly created elements which have no original data available.

The default setting will not write elements with errors to the resulting file and the tool will try to recreate the data read from the original file. In the error view all occured errors will be listed. The errors belong to the four categories `structural`, `interpretation`, `editing` or `creation`:

### Structural Error
This errors occur during the parsing of the content at the loading of the `.ecf` file. The corresponding line in the file violates the syntax in a manner which makes it impossible to attach this data to the managed structure within the tool. Such an error must be corrected in the original file if the data content is needed.

### Interpretation Error
This errors occur during the parsing of the content at the loading of the `.ecf` file. It means that the found data basically fits to the `.ecf` syntax but the content is unknown or contains invalid data compared to the definition. It can in mostly all cases be corrected in the tool.

### Editing error
This errors occur during content editing in the tool at operations which would consume too much performance to prevent it by a pre check (or similar). It has the same meaning as `Interpretation errors` and can in all cases be corrected in the tool.

### Creation error
This errors occur during content writing at hitting `save`. This error depends on the error handling settings. It only reports elements that should be written but according to the settings no writeable data is available/permitted. It states if an element is not written because it contains an error and no fallback data from parsing could be found.

## Planned Major Features
- "Custom changed" Property for ecf items (storage, handling, display and usage) 
- Item Handling Support roll out for Entities, Loot/Containers, Factions
- Element, Parameter, Attribute, Comment mass changing (base on filter/types)
- Element, Parameter, Comment moving
- Undo / Redo
