# Script

## Pick one ECUC module

Picking one ECUC module through instance a new EcucData with the ECUC module name. Picking one ECUC module is start of future ECUC element picking.

### Example

```c#
/* Pick Can module */
var can = new EcucData(instanceManager, bswmdManager, "Can");
```

## Pick other element

After picking a module, the further element element can be picked by [] operation.

### Parameter

1. Further element name.
2. Filter function to filter result.

[] operation can be chained to continous picking level after level.

Member, the result of [] is a list.

### Example

```c#
/* Pick controller from CanConfigSet of Can module.
Controller name shall not end with "_001" and "_002" */
var controllers = can["CanConfigSet"]["CanController", x => !(x.AsrPathShort.EndsWith("_001") || x.AsrPathShort.EndsWith("_002"))];
```

## Pick element value

The value is stored in element and can be got by property "Value".
Since [] operation return a list of EcucData, the value of first EcucData can be got through property "FirstValue".
List of EcucData also has "Value" property return a list of value for each element.

### Example

```c#
/* Pick CanIf module */
var canIf = new EcucData(instanceManager, bswmdManager, "CanIf");
/* Pick CanIfRxPduCfgs which CanIfRxPduUserRxIndicationUL element is not "CAN_NM" */
var canIdTypeContainers = canIf["CanIfInitCfg"]["CanIfRxPduCfg", x => x["CanIfRxPduUserRxIndicationUL"].FirstValue != "CAN_NM"]["CanIfRxPduCanIdType"];
/* Pick all Id Types */
var canIdTypes = canIdTypeContainers.Value;
```

Since value is stored as string, property "ValueAsInt" is used to pick integer result;

### Example

```c#
/* Pick all filters in CanFilterMask */
var filters = controllers["CanFilterMask"];
/* Pick CanFilterCodeValue as integer */
var codeValues = filters["CanFilterCodeValue"].ValueAsInt;
/* Pick CanFilterMaskValue as integer */
var maskValues = filters["CanFilterMaskValue"].ValueAsInt;
```

## Add container

AddContainer and AddContainerWithRequiredField of EcucData are used to add sub container with specified name.

AddContainer will add a empty subcontainer with no sub container and value.
AddContainerWithRequiredFiel will add a subcontainer with required sub container and value.

### Parameter

1. Container name
2. Container instance name

### Example

```c#
/* Pick CanHardwareObject from CanController from CanConfigSet */
var controller = data["CanConfigSet"]["CanController", x => !(x.Value.EndsWith("_001") || x.Value.EndsWith("_002"))].First;
var hardwareObject = data["CanConfigSet"]["CanHardwareObject", x => x.Value.EndsWith("Rx")].First;
/* Add container CanFilterMask to CanController */
var filter = controller.AddContainerWithRequiredField("CanFilterMask", $"CanFilterMaskRx_0x{id:X3}");
```

## Set validation status of element

Method UpdateValidStatus of EcucData is used to set EcucData validatation result. The validation  result will implict transfer to upper element.

### Parameter

1. Status of validation to set. True for valid and false for invalid.
2. Hit message of validation.

### Example

```c#
/* Set Can module to invalid whit hint "Filter missing" */
can.UpdateValidStatus(false, "Filter missing");
/* Set Can module back to valid */
can.UpdateValidStatus(true);
```

## Set validation solution of element

Method UpdateValidSolve of EcucData is used to set EcucData validatation soultion.

### Parameter

1. Hit message of solution.
2. Function of solution.
3. Parameter transfer to function.

### Example

```c#
/* Add soultion to Can module */
can.UpdateValidSolve($"Add missing filter 0x{frame.ID:X3}", FixCanFilter, frame.ID);

/* Fix Can filter solution */
private void FixCanFilter(EcucData data, object? idSuggest)
{
    if (idSuggest is uint id)
    {
        /* Pick CanHardwareObject from CanController from CanConfigSet */
        var controller = data["CanConfigSet"]["CanController", x => !(x.Value.EndsWith("_001") || x.Value.EndsWith("_002"))].First;
        var hardwareObject = data["CanConfigSet"]["CanHardwareObject", x => x.Value.EndsWith("Rx")].First;
        /* Add container CanFilterMask to CanController */
        var filter = controller.AddContainerWithRequiredField("CanFilterMask", $"CanFilterMaskRx_0x{id:X3}");
        /* Update CanFilterMask value */
        filter["CanFilterMaskValue"].FirstValueAsInt = 0x7FF;
        filter["CanFilterCodeValue"].FirstValueAsInt = id;
        filter["IsLocked"].FirstValue = "false";
        /* Add reference CanFilterMaskRef to CanFilterMask */
        hardwareObject.AddRef("CanFilterMaskRef", filter.AsrPath);
    }
}
```
