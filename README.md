# Autosar-Configurator
    Autosar Configurator for GUI based configuration, checking and code generation.  
Autosar Configurator is not based on Artop as others do. It is fully constructed on existing C# infrastructure of XSD, XML and Linq handing. The speed is much faster than eclipse based soultion. Fifty arxml can be opened in less than 5 seconds and memory is less than 200 M bytes after opening fifty arxmls.

It is adapted with Autosar ECU Configuration arxml from standard V4.4 (AUTOSAR_MOD_ECUConfigurationParameters.arxml) and arxml in business SIP.  
Script is used to visit every container and parameter with easy way.  
Code generation is also possible through script but later code template will be used for code generation.  

Feel free to try the function and feedback by issues.

---
## Installion
Download from link release https://github.com/DD-Silence/Autosar-Configurator/releases/ and unzip it to the folder you want.

---
## Folder structure
### Data folder is where we should focus. There are 5 subfolders, bswmd, instance, template, script and generate.
## bswmd folder
This is where Basic SoftWare Module Description arxml file shall put. It is not forced, the path can be modified in GUI.  V4.4.0 AUTOSAR_MOD_ECUConfigurationParameters.arxml is put inside as an example.
## instance folder
This is where ECU parameter arxml file shall put. It is not forced, the path can be modified in GUI. Adc, Can, CanIf, CanNm, CanSM, CanTp, CanTrcv, Com, Dcm, Dem, EcuC and Pdur arxml initial arxml is inside. These files is just a header with no actual value inside.
## template folder
Backup folder of instance folder.
## script folder
It is possible to use C# script to visit container or parameter by exposed API inside Autosar Configurator. Add, delete, update and search is possible. User can use script to realize his special configuration check. These script are fored to put inside script folder. In order to well organized, folder is allowed to contain all related script files inside. Script file shall has extension of cs. There is no project to develop script now, user need to type script without the help of Visual Studio intellicode. It will be available soon. 
## generate folder
Generate folder is not finished, it is useless now.

---
## Operations
### Add container

### Delete option container

### Add option parameter

### Delete option parameter

### Add reference

### Delete reference

### Find referenced parameter or container

### Find Usage of container

### Save changes

### Reload UI

### Run Script and get result

---
## Customize bswmd arxml to create custom Autosar module

---

