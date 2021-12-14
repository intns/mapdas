# mapdas
Tools for Gamecube and Wii symbol maps

# Special thanks
- Arookas for the Demangler
- JoshuaMK for vital work on the tool that allowed for the decompilation

# Requirements / Dependencies:
- PyiiASMH -> https://github.com/JoshuaMKW/pyiiasmh/releases

# Features
- Loads multiple symbol map files
- Displays map files in a sorted tree
- Parses and demangles symbols (thanks to arookas for this feature)
- Can export symbol maps to .idc files (IDA's scripted language)
- Can export every referenced file inside the symbol map to get a 1:1 recreation of the original developer's directory and file layout
- Can extract every type found in function arguments into an easily-compiled C++ header (see Extract Types when right clicking on the symbol map when it's loaded)
- Extracting symbols and converting it to a linker map easily for use in a decompilation project / modding project
- Loading DOL files to extract the PowerPC assembly for functions to get a better base for decompiling
- Viewing the hex and the PowerPC assembly of a function side by side 

# How To Use
You must first load the symbol map by going to File -> Open -> MAP.
At this point you can traverse the directories and files of the project; even the functions inside of the files.
You can optionally load a DOL file to decompile individual functions and add even more features to the tool when exporting to a filesystem by right-clicking the symbol map file, and opening the DOL.
All of the features after this will be obvious, by right clicking on the various entries in the symbol map you are presented with different options.

# Menu Entries that can be seen with a right click
## On a Symbol Map
  - Open DOL
  - Export Filesystem
  - Export To Linker Map
  - Convert To IDC
  - Export Types
  - Close

## On a file within a Symbol Map
  - Export File
  - Export Types

## On a function within a file
  - Export Types
  - Decompile
