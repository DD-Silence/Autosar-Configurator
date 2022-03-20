## Basic Operation
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
![image](https://user-images.githubusercontent.com/101047683/157067885-ea4b4cd7-c15f-4463-8360-bfe79430b82d.png)  
The parameter is enabled with default value.
![image](https://user-images.githubusercontent.com/101047683/157067230-93c0f28e-6d8e-4bab-99e4-a0a5bd22751c.png)
Change it is also possible with candidate value.  
![image](https://user-images.githubusercontent.com/101047683/157067403-b9c6fc5c-08cc-4f32-95da-4e2d8934d005.png)  

### Delete option parameter
Enabled parameter can be deleted. Right click parameter and click Delete Parameter item.  
![image](https://user-images.githubusercontent.com/101047683/157068110-00ebd78f-f9b2-4500-bdd8-e087504b08e1.png)  
Parameter back to disabled status.  
![image](https://user-images.githubusercontent.com/101047683/157068217-568f1dd4-75c9-44a0-9aa7-30ca0f7917ec.png)
It is not possible to deleta a required parameter.  
![image](https://user-images.githubusercontent.com/101047683/157068411-41da383c-bbe9-4f69-9944-314a6ad6ae2d.png)  

### Add reference
Here is a CAN controller and a CanIf CanIfCtrlCanCtrlRef container with CanIfCtrlCanCtrlRef reference to connect  
these two container.
![image](https://user-images.githubusercontent.com/101047683/157070591-3803a6d3-d9c9-4ea1-8729-75ba4282cfa7.png)
Right click and choose Create Paraemter item.
![image](https://user-images.githubusercontent.com/101047683/157070986-a237a518-ba47-44cc-bd0c-8a0e2da73597.png)
A form with candidate items is popup and choose CAN controller want to link and double click.
![image](https://user-images.githubusercontent.com/101047683/157071246-240331a3-ff82-4d44-8959-5a3b88440220.png)
The reference link is created.
![image](https://user-images.githubusercontent.com/101047683/157071379-34005656-c669-41d3-94ee-b465d9ea4d63.png)

### Delete reference
If reference can be deleted, right click on reference and click Delete Paremeter item.  
![image](https://user-images.githubusercontent.com/101047683/157459532-6ddc6847-79ad-4282-9639-8be9d14f7dc5.png)  
The reference can be deleted.  
![image](https://user-images.githubusercontent.com/101047683/157459797-12769372-1d14-4fcc-b7c4-6abd36290859.png)  

### Find referenced parameter or container
On a reference, right click and choose Find Reference item.  
![image](https://user-images.githubusercontent.com/101047683/159148935-4fbb60b3-2f7f-4888-8515-f4ac44ffae9e.png)  
Then focus will navigate to the referenced parameter or container.  
![image](https://user-images.githubusercontent.com/101047683/159148954-378500f1-9111-415f-a420-5ad2e00be755.png)  

### Find Usage of container
On node of container, right click and choose Usage item.  
![image](https://user-images.githubusercontent.com/101047683/159148992-3fe09710-acd3-43dc-a64a-9ddbfe639599.png)  
Then focus will navigate to the container reference the first container.  
![image](https://user-images.githubusercontent.com/101047683/159149021-d26718a4-10a4-4970-97c6-1e3b6fa069dc.png)  

### Save changes
Click File menu and choose Save item to save configurations in arxml.  
![image](https://user-images.githubusercontent.com/101047683/159149059-fa1a2724-fd37-438f-b66b-cf4017412e31.png)  

### Reload UI
Click File menu and choose Reload item to reload ui.  
![image](https://user-images.githubusercontent.com/101047683/159149083-490277d0-05cd-4e91-8abf-b4374ff45d2b.png)  

### Run Script and get result
Click Script menu and choose the script you want to execute.  
![image](https://user-images.githubusercontent.com/101047683/159149629-a1add25e-16fa-4972-89e8-149af07e2157.png)
After execution of the script. Result will be shown in Console.  
![image](https://user-images.githubusercontent.com/101047683/159149128-c6211f11-8e0e-458f-acd2-87ea986e2211.png)  
If the script contains validation result, the wrong container and parameter will shown as red color.  
![image](https://user-images.githubusercontent.com/101047683/159149177-39a4b3d3-6fb1-4e9d-99b2-39f934b1ee7c.png)  
Hover on the red node or parameter, validtion result will be shown.  
Parameter validation result  
![image](https://user-images.githubusercontent.com/101047683/159149207-fdda5d2c-88e2-4b56-8918-ec0eb897d0e8.png)  
Container validation result  
![image](https://user-images.githubusercontent.com/101047683/159149223-5e9636c6-e13c-4591-9fc2-55df081c5779.png)  
The validation result are summarized in Validation windown.  
The summary is based on module.  
![image](https://user-images.githubusercontent.com/101047683/159149251-4ae26cd7-ea36-4e10-80a2-1b2bb34f6bee.png)  
Right click on each item can get Navigate and Solve item.  
![image](https://user-images.githubusercontent.com/101047683/159149277-8914a186-2721-47f3-a298-e26abda9e3c5.png)  
Click Navigate item will focus to the validated container.  
![image](https://user-images.githubusercontent.com/101047683/159149303-55c8cf8d-8c5d-4010-8541-46a27ecc3fab.png)  
Click Solve item will call script to solve the problem.  
![image](https://user-images.githubusercontent.com/101047683/159149333-cfdb1f4a-7099-4a49-a626-ced281e97c0b.png)  
The container will also turn to black to show the problem is fixed.  
![image](https://user-images.githubusercontent.com/101047683/159149373-3ec5ff03-5927-4d79-8d08-53181f5c1925.png)  

---
## Customize bswmd arxml to create custom Autosar module

---
