/*  
 *  This file is a part of Autosar Configurator for ECU GUI based 
 *  configuration, checking and code generation.
 *  
 *  Copyright (C) 2021-2022 DJS Studio E-mail:DD-Silence@sina.cn
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.

 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.

 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EInstance;

namespace Ecuc.EcucBase.EPdu
{
    public enum EcucPduType
    {
        CanIfRx = 0,
        CanIfTx,
        ComRx,
        ComTx,
        SecOCRx,
        SecOCTx,
        CanTpNSduRx,
        CanTpNSduTx,
        CanTpNPduRx,
        CanTpNPduTx,
        CanTpFcNPduRx,
        CanTpFcNPduTx,
        EcucPdu
    }

    public class EcucPdu
    {
        EcucBswmdManager BswmdManager { get; }
        EcucInstanceManager InstanceManager { get; }

        public EcucPdu(EcucBswmdManager bswmdManager, EcucInstanceManager instanceManager)
        {
            BswmdManager = bswmdManager;
            InstanceManager = instanceManager;
        }

        private static bool FilterFuncComRx(EcucData data)
        {
            return data["ComIPduDirection"].ValueSingleEqual("RECEIVE");
        }

        private static bool FilterFuncComTx(EcucData data)
        {
            return data["ComIPduDirection"].ValueSingleEqual("SEND");
        }

        public EcucDataList GetPdusByType(EcucPduType typ)
        {
            switch (typ)
            {
                case EcucPduType.CanIfRx:
                    {
                        var instanceModule = InstanceManager["CanIf"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanIfInitCfg"]["CanIfRxPduCfg"];
                    }

                case EcucPduType.CanIfTx:
                    {
                        var instanceModule = InstanceManager["CanIf"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanIfInitCfg"]["CanIfTxPduCfg"];
                    }

                case EcucPduType.ComRx:
                    {
                        var instanceModule = InstanceManager["Com"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["ComConfig"]["ComIPdu", x => FilterFuncComRx(x)];
                    }

                case EcucPduType.ComTx:
                    {
                        var instanceModule = InstanceManager["Com"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["ComConfig"]["ComIPdu", x => FilterFuncComTx(x)];
                    }

                case EcucPduType.SecOCRx:
                    {
                        var instanceModule = InstanceManager["SecOC"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["SecOCRxPduProcessing"]["SecOCRxAuthenticPduLayer"];
                    }

                case EcucPduType.SecOCTx:
                    {
                        var instanceModule = InstanceManager["SecOC"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["SecOCRxPduProcessing"]["SecOCTxAuthenticPduLayer"];
                    }

                case EcucPduType.CanTpNSduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu"];
                    }

                case EcucPduType.CanTpNSduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu"];
                    }

                case EcucPduType.CanTpNPduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu"]["CanTpRxNPdu"];
                    }

                case EcucPduType.CanTpNPduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu"]["CanTpTxNPdu"];
                    }

                case EcucPduType.CanTpFcNPduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu"]["CanTpTxFcNPdu"];
                    }

                case EcucPduType.CanTpFcNPduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu"]["CanTpRxFcNPdu"];
                    }

                case EcucPduType.EcucPdu:
                    {
                        var instanceModule = InstanceManager["SecOc"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["EcucPduCollection"]["Pdu"];
                    }

                default:
                    return new EcucDataList();
            }
        }

        static private bool FilterShortNameFunc(EcucData data, string name)
        {
            return data.Value.StartsWith(name);
        }

        private static bool FilterShortNameFuncComRx(EcucData data, string name)
        {
            return data.Value.StartsWith(name) && data["ComIPduDirection"].ValueSingleEqual("RECEIVE");
        }

        private static bool FilterShortNameFuncComTx(EcucData data, string name)
        {
            return data.Value.StartsWith(name) && data["ComIPduDirection"].ValueSingleEqual("SEND");
        }

        public EcucDataList GetPduByTypeAndName(EcucPduType typ, string name)
        {
            switch (typ)
            {
                case EcucPduType.CanIfRx:
                    {
                        var instanceModule = InstanceManager["CanIf"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList(); ;
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanIfInitCfg"]["CanIfRxPduCfg", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanIfTx:
                    {
                        var instanceModule = InstanceManager["CanIf"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanIfInitCfg"]["CanIfTxPduCfg", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.ComRx:
                    {
                        var instanceModule = InstanceManager["Com"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["ComConfig"]["ComIPdu", x => FilterShortNameFuncComRx(x, name)];
                    }

                case EcucPduType.ComTx:
                    {
                        var instanceModule = InstanceManager["Com"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["ComConfig"]["ComIPdu", x => FilterShortNameFuncComTx(x, name)];
                    }

                case EcucPduType.SecOCRx:
                    {
                        var instanceModule = InstanceManager["SecOC"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["SecOCRxPduProcessing"]["SecOCRxAuthenticPduLayer", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.SecOCTx:
                    {
                        var instanceModule = InstanceManager["SecOC"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["SecOCRxPduProcessing"]["SecOCTxAuthenticPduLayer", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpNSduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpNSduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpNPduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu"]["CanTpRxNPdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpNPduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu"]["CanTpTxNPdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpFcNPduRx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpRxNSdu"]["CanTpTxFcNPdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.CanTpFcNPduTx:
                    {
                        var instanceModule = InstanceManager["CanTp"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["CanTpConfig"]["CanTpChannel"]["CanTpTxNSdu"]["CanTpRxFcNPdu", x => FilterShortNameFunc(x, name)];
                    }

                case EcucPduType.EcucPdu:
                    {
                        var instanceModule = InstanceManager["SecOc"];
                        if (instanceModule == null)
                        {
                            return new EcucDataList();
                        }
                        var bswmdModule = BswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
                        if (bswmdModule == null)
                        {
                            return new EcucDataList();
                        }
                        var data = new EcucData(instanceModule, bswmdModule);
                        return data["EcucPduCollection"]["Pdu", x => FilterShortNameFunc(x, name)];
                    }


                default:
                    return new EcucDataList();
            }
        }
    }

    public class EcucPduResult
    {
        public EcucDataList Datas { get; }
        private EcucPduType Typ { get; }

        public EcucDataList Reference
        {
            get
            {
                return Typ switch
                {
                    EcucPduType.CanIfRx => Datas["CanIfRxPduRef"],
                    EcucPduType.CanIfTx => Datas["CanIfTxPduRef"],
                    EcucPduType.ComRx => Datas["ComPduIdRef"],
                    EcucPduType.ComTx => Datas["ComPduIdRef"],
                    EcucPduType.SecOCRx => Datas["SecOCRxAuthenticLayerPduRef"],
                    EcucPduType.SecOCTx => Datas["SecOCTxAuthenticLayerPduRef"],
                    EcucPduType.CanTpNSduRx => Datas["CanTpRxNSduRef"],
                    EcucPduType.CanTpNSduTx => Datas["CanTpTxNSduRef"],
                    EcucPduType.CanTpNPduRx => Datas["CanTpRxNPduRef"],
                    EcucPduType.CanTpNPduTx => Datas["CanTpTxNPduRef"],
                    EcucPduType.CanTpFcNPduRx => Datas["CanTpRxFcNPduRef"],
                    EcucPduType.CanTpFcNPduTx => Datas["CanTpTxFcNPduRef"],
                    _ => new EcucDataList(),
                };
            }
        }

        public EcucDataList DeReference
        {
            get
            {
                return Reference.DeRef();
            }
        }

        public EcucPduResult(EcucDataList datas, EcucPduType typ)
        {
            Datas = datas;
            Typ = typ;
        }
    }
}
