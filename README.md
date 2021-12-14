# mapdas
Tools for Gamecube and Wii symbol maps

# Special thanks
- Arookas for the Demangler
- JoshuaMK for vital work on the tool that allowed for the decompilation

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
- Cleaning up the exported filesystem quickly suing the Common FS Cleaner tool, that searches for easily done functions and decompiles them, furthermore it also has the capability to encase blocks of code in namespaces, given a list of namespaces

# How To Use
Load the symbol map by clicking "Open Map", note that you can open multiple MAP files.

Upon right clicking on the opened symbol map file, you can also load a DOL file.

When traversing the symbol map tree, you can access the various menus by right clicking on the entries.

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
