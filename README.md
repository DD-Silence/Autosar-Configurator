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
Right click and move to Add item. Wait to choose container want to add.  
![image](https://user-images.githubusercontent.com/101047683/157063201-78ee49da-be98-407b-9018-34953f55fc66.png)  
Whole path of container with required subcontainers are add.  
![image](https://user-images.githubusercontent.com/101047683/157063599-33fa39f2-a04a-4a90-98f3-2ff89ed1ce79.png)  
Required paramters in required container are also created.  
![image](https://user-images.githubusercontent.com/101047683/157065217-9cf4e6ca-cb72-47ae-bf62-4744536ae50f.png)  

### Delete option container
Right click the container and click Delete item.  
![image](https://user-images.githubusercontent.com/101047683/157065792-090ab999-6b6b-41b9-aafb-4ba18b448aba.png)  
The container want to delete is deleted.  
![image](https://user-images.githubusercontent.com/101047683/157066041-8a9414cf-b3db-475c-899a-8ed1ae8db932.png)  

### Add option parameter
These disable TextBox is option parameter in container. Right clck it and click Create Parameter item.  
![image](https://user-images.githubusercontent.com/101047683/157066652-97e2267c-a068-42b2-9bc6-5de789bdf994.png)  
The parameter is enabled with default value.
![image](https://user-images.githubusercontent.com/101047683/157067230-93c0f28e-6d8e-4bab-99e4-a0a5bd22751c.png)
Change it is also possible with candidate value.  
![image](https://user-images.githubusercontent.com/101047683/157067403-b9c6fc5c-08cc-4f32-95da-4e2d8934d005.png)  

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

