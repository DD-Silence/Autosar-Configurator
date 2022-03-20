using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EInstance;
using System;
using System.Collections.Generic;
using Ecuc.EcucBase.EBase;

class Script
{
    public void ScriptRun(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        Check(bswmdManager, instanceManager);
    }

    private void Check(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        Console.WriteLine("Start check process");
        CheckControllerIds(bswmdManager, instanceManager);
        CheckTransceiverIds(bswmdManager, instanceManager);
        CheckDriverUsage(bswmdManager, instanceManager);
        CheckDriver2HohReference(bswmdManager, instanceManager);
    }

    private void CheckControllerIds(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        var canIf = new EcucData(instanceManager, bswmdManager, "CanIf");
        var controllerIds = canIf["CanIfCtrlDrvCfg"]["CanIfCtrlCfg"]["CanIfCtrlId"];

        var idCheck = new EcucIdRangeCheck();
        foreach (var id in controllerIds)
        {
            idCheck.Add(id.ValueAsInt, id);
        }
        idCheck.Check();
    }

    private void CheckTransceiverIds(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        var canIf = new EcucData(instanceManager, bswmdManager, "CanIf");
        var transceiverIds = canIf["CanIfTrcvDrvCfg"]["CanIfTrcvCfg"]["CanIfTrcvId"];

        var idCheck = new EcucIdRangeCheck();
        foreach (var id in transceiverIds)
        {
            idCheck.Add(id.ValueAsInt, id);
        }
        idCheck.Check();
    }

    private void CheckDriverUsage(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        var canIf = new EcucData(instanceManager, bswmdManager, "CanIf");
        var driverNames = canIf["CanIfCtrlDrvCfg"]["CanIfCtrlDrvNameRef"];
        var driverDict = new Dictionary<string, EcucData>();

        foreach (var driverName in driverNames)
        {
            if (!driverDict.ContainsKey(driverName.ValueShort))
            {
                driverDict.Add(driverName.ValueShort, driverName);
                driverName.UpdateValidStatus(true);
            }
            else
            {
                Console.WriteLine($"Same driver name {driverName.Value}");
                driverName.UpdateValidStatus(false, "Same driver name");
                driverName.ClearValidSolve();
                driverDict[driverName.Value].UpdateValidStatus(false, "Same driver name");
                driverDict[driverName.Value].ClearValidSolve();
            }
        }

    }

    private void CheckDriver2HohReference(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
    {
        var canIf = new EcucData(instanceManager, bswmdManager, "CanIf");
        var hohRefs = canIf["CanIfCtrlDrvCfg"]["CanIfCtrlDrvInitHohConfigRef"];
        var hohDict = new Dictionary<string, EcucData>();

        foreach (var hohRef in hohRefs)
        {
            if (!hohDict.ContainsKey(hohRef.ValueShort))
            {
                hohDict.Add(hohRef.ValueShort, hohRef);
                hohRef.UpdateValidStatus(true);
            }
            else
            {
                Console.WriteLine($"Same Hoh reference {hohRef.Value}");
                hohRef.UpdateValidStatus(false, "Same Hoh reference");
                hohRef.ClearValidSolve();
                hohDict[hohRef.Value].UpdateValidStatus(false, "Same Hoh reference");
                hohDict[hohRef.Value].ClearValidSolve();
            }
        }

        var hohs = canIf["CanIfInitCfg"]["CanIfInitHohCfg"];

        foreach (var hoh in hohs)
        {
            if (hohDict.ContainsKey(hoh.ValueShort))
            {
                hoh.UpdateValidStatus(true);
            }
            else
            {
                Console.WriteLine($"Hoh {hoh.Value} is not referenced by any driver");
                hoh.UpdateValidStatus(false, "Hoh is not referenced by any driver");
                hoh.ClearValidSolve();
            }
        }
    }
}
