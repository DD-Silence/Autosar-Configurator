# Autosar-Configurator
    First open source Autosar compliant configurator with the following features,
    * Fast and low memory usage.
    * Compliant with Vector, EB, Mentor SIP package. Review arxml from SIP as well as Autosar.
    * Similar operation logic with Vector Davincci tool chain. 
    * Add, delete, modify and save containers and parameters by GUI and script.
    * Customize Autosar model and operate like origin model.
    
Autosar Configurator is not based on Artop as others do. It is fully constructed on existing C# infrastructure of XSD, XML and Linq handing. The speed is much faster than eclipse based soultion. Fifty arxml can be opened in less than 5 seconds and memory is less than 200 M bytes after opening fifty arxmls. 

Autosar Configurator is compliant with Autosar 4.4. It is designed according to [AUTOSAR_TPS_ECUConfiguration](/manual/standard/AUTOSAR_TPS_ECUConfiguration.pdf) and [AUTOSAR_RS_ECUConfiguration](/manual/standard/AUTOSAR_RS_ECUConfiguration.pdf).It is adapted with Autosar ECU Configuration arxml from standard V4.4 (AUTOSAR_MOD_ECUConfigurationParameters.arxml) and arxml in business SIP. 

Script is used to visit every container and parameter with easy way.  
Code generation is also possible through script but later code template will be used for code generation.  

Feel free to try the function and feedback by issues.

---
## Installion
Download from link release https://github.com/DD-Silence/Autosar-Configurator/releases/ and unzip it to the folder you want.
Double Click Autosar-Configurator.exe to run.

## Operation
Operation can refer [Basic Operation](/manual/operation/basic_operation.md)

## Folder
Folder structure can refer [Folder Structure](/manual/operation/folder.md)

## Script
Manual not ready.

