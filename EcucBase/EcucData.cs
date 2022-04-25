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

using Ecuc.EcucBase.EBase;
using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EInstance;
using System.ComponentModel;
using System.Numerics;

namespace Ecuc.EcucBase.EData
{
    /// <summary>
    /// EcucData multiply status.
    /// Multiply status is calculated from quantity in instance, lower and upper
    /// in bswmd. It is represent as int, every status occupy one bit. Several
    /// status can exist at same time.
    /// </summary>
    public class EcucDataMultiplyStatus
    {
        /// <summary>
        /// Status enumeration
        /// </summary>
        public enum StatusType
        {
            /// <summary>
            /// Fatal error happen during calculation or EcucBswmd upper is 0.
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// The quantity is 0.
            /// </summary>
            Empty = 1,
            /// <summary>
            /// The quantity is less than lower.
            /// </summary>
            Lack = 2,
            /// <summary>
            /// The quantity is same as lower.
            /// </summary>
            OkLower = 4,
            /// <summary>
            /// The quantity is bigger than lower and smaller than upper.
            /// </summary>
            OkMiddle = 8,
            /// <summary>
            /// The quantity is same as upper.
            /// </summary>
            OkUpper = 16,
            /// <summary>
            /// The quantity is bigger than upper.
            /// </summary>
            Overflow = 32,
        }

        /// <summary>
        /// Status.
        /// </summary>
        public StatusType Status { get; }

        /// <summary>
        /// Initialize class by status enumeration.
        /// </summary>
        /// <param name="status"></param>
        public EcucDataMultiplyStatus(StatusType status)
        {
            Status = status;
        }

        /// <summary>
        /// Helper property to get whether invalid.
        /// </summary>
        public bool Invalid => Status == StatusType.Invalid;
        /// <summary>
        /// Helper property to get whether empty.
        /// </summary>
        public bool Empty => (Status & StatusType.Empty) == StatusType.Empty;
        /// <summary>
        /// Helper property to get whether lack.
        /// </summary>
        public bool Lack => (Status & StatusType.Lack) == StatusType.Lack;
        /// <summary>
        /// Helper property to get whether okLower.
        /// </summary>
        public bool OkLower => (Status & StatusType.OkLower) == StatusType.OkLower;
        /// <summary>
        /// Helper property to get whether okMiddle.
        /// </summary>
        public bool OkMiddle => (Status & StatusType.OkMiddle) == StatusType.OkMiddle;
        /// <summary>
        /// Helper property to get whether okUpper.
        /// </summary>
        public bool OkUpper => (Status & StatusType.OkUpper) == StatusType.OkUpper;
        /// <summary>
        /// Helper property to get whether overflow.
        /// </summary>
        public bool Overflow => (Status & StatusType.Overflow) == StatusType.Overflow;

        /// <summary>
        /// Helper property to get whether can add more.
        /// </summary>
        public bool CanAdd => !OkUpper && !Overflow && !Invalid;
        /// <summary>
        /// Helper property to get whether can remove one.
        /// </summary>
        public bool CanRemove => !Lack && !OkLower && !Invalid;
        /// <summary>
        /// Helper property to get whether ok.
        /// </summary>
        public bool Ok => OkLower || OkMiddle || OkUpper;
        /// <summary>
        /// Helper property to get whether not ok.
        /// </summary>
        public bool NotOk => !Ok;
    }

    /// <summary>
    /// Ecuc data class.
    /// Ecuc data integrate EcucInstance and EcucBswmd. From EcucData, quantity of instance
    /// can be evalutaed. Add and Remove operation can also be evaluated by MultiplyStatus.
    /// </summary>
    public class EcucData : NotifyPropertyChangedBase
    {
        /// <summary>
        /// EcucInstance.
        /// </summary>
        private IEcucInstanceBase Instance { get; }
        /// <summary>
        /// EcucBswmd.
        /// </summary>
        private IEcucBswmdBase Bswmd { get; }

        /// <summary>
        /// Whether data is invalid.
        /// </summary>
        public bool ValidStatus
        {
            get
            {
                return Instance.Valid.ValidRecursive.Count == 0;
            }
        }

        /// <summary>
        /// Whether data is invalid.
        /// </summary>
        public string ValidInfo
        {
            get
            {
                string result = "";
                foreach (var v in Instance.Valid.ValidRecursive)
                {
                    result += $"{v.Info}{Environment.NewLine}";
                }
                return result;
            }
        }

        /// <summary>
        /// Whether data is invalid.
        /// </summary>
        public List<EcucValid> ValidRecursive
        {
            get
            {
                return Instance.Valid.ValidRecursive;
            }
        }

        /// <summary>
        /// Short form of AsrPath
        /// </summary>
        public string AsrPathShort
        {
            get
            {
                var parts = AsrPath.Split('/');
                if (parts.Length > 0)
                {
                    return parts[^1];
                }
                return "";
            }
        }

        /// <summary>
        /// Autosar path of EcucInstance as moudle or container.
        /// </summary>
        public string AsrPath
        {
            get
            {
                return InstanceAsModule.AsrPath;
            }
        }

        /// <summary>
        /// EcucInstance class type.
        /// </summary>
        public Type InstanceType
        {
            get
            {
                return Instance.GetType();
            }
        }

        /// <summary>
        /// EcucBswmd class type.
        /// </summary>
        public Type BswmdType
        {
            get
            {
                return Bswmd.GetType();
            }
        }

        /// <summary>
        /// EcucBswmd description.
        /// </summary>
        public string Desc
        {
            get
            {
                return Bswmd.Desc;
            }
        }

        /// <summary>
        /// EcucBswmd trace.
        /// </summary>
        public string Trace
        {
            get
            {
                return Bswmd.Trace;
            }
        }

        /// <summary>
        /// EcucBswmd smallest quantity.
        /// </summary>
        public uint Lower
        {
            get
            {
                return Bswmd.Lower;
            }
        }

        /// <summary>
        /// EcucBswmd biggest quantity.
        /// </summary>
        public uint Upper
        {
            get
            {
                return Bswmd.Upper;
            }
        }

        /// <summary>
        /// EcucInstance as IEcucInstanceModule.
        /// </summary>
        private IEcucInstanceModule InstanceAsModule
        {
            get
            {
                if (Instance is IEcucInstanceModule container)
                {
                    return container;
                }
                else
                {
                    throw new Exception("Instance of ecuc data is not module kind");
                }
            }
        }

        /// <summary>
        /// EcucBswmd as IEcucBswmdModule.
        /// </summary>
        private IEcucBswmdModule BswmdAsModule
        {
            get
            {
                if (Bswmd is IEcucBswmdModule container)
                {
                    return container;
                }
                else
                {
                    throw new Exception("Bswmd of ecuc data is not module kind");
                }
            }
        }

        /// <summary>
        /// EcucInstance as IEcucInstanceContainer.
        /// </summary>
        private IEcucInstanceContainer InstanceAsContainer
        {
            get
            {
                if (Instance is IEcucInstanceContainer container)
                {
                    return container;
                }
                else
                {
                    throw new Exception("Instance of ecuc data is not container kind");
                }
            }
        }

        /// <summary>
        /// EcucBswmd as IEcucBswmdContainer.
        /// </summary>
        private IEcucBswmdContainer BswmdAsContainer
        {
            get
            {
                if (Bswmd is IEcucBswmdContainer container)
                {
                    return container;
                }
                else
                {
                    throw new Exception("Bswmd of ecuc data is not container kind");
                }
            }
        }

        /// <summary>
        /// EcucBswmd path as well as EcucInstance BswmdPath.
        /// </summary>
        public string BswmdPath
        {
            get
            {
                return Bswmd.AsrPath;
            }
        }

        /// <summary>
        /// Short form of BswmdPath.
        /// </summary>
        public string BswmdPathShort
        {
            get
            {
                return Bswmd.AsrPathShort;
            }
        }

        /// <summary>
        /// Continers in EcucInstance
        /// </summary>
        public List<IEcucInstanceModule> Containers
        {
            get
            {
                return InstanceAsModule.Containers;
            }
        }

        /// <summary>
        /// Parameters in EcucInstance
        /// </summary>
        public List<IEcucInstanceParam> Paras
        {
            get
            {
                return InstanceAsContainer.Paras;
            }
        }

        /// <summary>
        /// References in EcucInstance
        /// </summary>
        public List<IEcucInstanceReference> Refs
        {
            get
            {
                return InstanceAsContainer.Refs;
            }
        }

        /// <summary>
        /// Continers in EcucBswmd
        /// </summary>
        public List<IEcucBswmdModule> BswmdContainers
        {
            get
            {
                return BswmdAsModule.Containers;
            }
        }

        /// <summary>
        /// Parameters in EcucBswmd
        /// </summary>
        public List<IEcucBswmdParam> BswmdParas
        {
            get
            {
                return BswmdAsContainer.Paras;
            }
        }

        /// <summary>
        /// References in EcucBswmd
        /// </summary>
        public List<IEcucBswmdReference> BswmdRefs
        {
            get
            {
                return BswmdAsContainer.Refs;
            }
        }

        /// <summary>
        /// The reference point to Autosar path
        /// </summary>
        public List<IEcucInstanceReference> Usage
        {
            get
            {
                try
                {
                    return Instance.GetReferenceByAsrPath(AsrPath);
                }
                catch
                {
                    return new List<IEcucInstanceReference>();
                }
            }
        }

        /// <summary>
        /// Get EcucBswmd from BswmdPath.
        /// </summary>
        /// <param name="bswmdPath"></param>
        /// <returns></returns>
        public IEcucBswmdBase GetBswmdFromBswmdPath(string bswmdPath)
        {
            return Bswmd.GetBswmdFromBswmdPath(bswmdPath);
        }

        /// <summary>
        /// Sort containers in EcucInstance according to IEcucBswmdModule
        /// </summary>
        public Dictionary<IEcucBswmdModule, EcucDataList> SortedContainers
        {
            get
            {
                var result = new Dictionary<IEcucBswmdModule, EcucDataList>();
                // Iterate bswmd containers in bswmd container
                foreach (var bswmdSubContainer in BswmdAsModule.Containers)
                {
                    result[bswmdSubContainer] = new EcucDataList();
                    // Find containers according to bswmd
                    var instanceContainters = InstanceAsModule.FindContainerByBswmd(bswmdSubContainer.AsrPath);
                    // Pack each container to EcucData and then to EcucDataList
                    foreach (var instanceContainter in instanceContainters)
                    {
                        result[bswmdSubContainer].Add(new EcucData(instanceContainter, bswmdSubContainer));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Sort parameters in EcucInstance according to IEcucBswmdParam
        /// </summary>
        public Dictionary<IEcucBswmdParam, EcucDataList> SortedParas
        {
            get
            {
                var result = new Dictionary<IEcucBswmdParam, EcucDataList>();
                // Iterate bswmd parameters in bswmd container
                foreach (var bswmdPara in BswmdAsContainer.Paras)
                {
                    result[bswmdPara] = new EcucDataList();
                    // Find parameters according to bswmd
                    var instanceParas = InstanceAsContainer.FindParaByBswmd(bswmdPara.AsrPath);
                    // Pack each parameter to EcucData and then to EcucDataList
                    foreach (var instancePara in instanceParas)
                    {
                        result[bswmdPara].Add(new EcucData(instancePara, bswmdPara));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Sort references in EcucInstance according to IEcucBswmdReference
        /// </summary>
        public Dictionary<IEcucBswmdReference, EcucDataList> SortedRefs
        {
            get
            {
                var result = new Dictionary<IEcucBswmdReference, EcucDataList>();
                // Iterate bswmd references in bswmd container
                foreach (var bswmdRef in BswmdAsContainer.Refs)
                {
                    result[bswmdRef] = new EcucDataList();
                    // Find references according to bswmd
                    var instanceRefs = InstanceAsContainer.FindRefByBswmd(bswmdRef.AsrPath);
                    // Pack each reference to EcucData and then to EcucDataList
                    foreach (var instanceRef in instanceRefs)
                    {
                        result[bswmdRef].Add(new EcucData(instanceRef, bswmdRef));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Value of EcucData.
        /// For IEcucInstanceModule, ShortName will be returned.
        /// For IEcucInstanceParam, Parameter value will be returned.
        /// For IEcucInstanceReference, Reference valueRef will be returned.
        /// </summary>
        public string Value
        {
            get
            {
                switch (Instance)
                {
                    // For IEcucInstanceModule, ShortName will be returned
                    case IEcucInstanceModule instanceContainer:
                        return instanceContainer.AsrPathShort;

                    // For IEcucInstanceParam, further handle is needed
                    case IEcucInstanceParam instancePara:
                        switch (Instance)
                        {
                            // For EcucInstanceNumericalParamValue
                            case EcucInstanceNumericalParamValue:
                                switch (Bswmd)
                                {
                                    case EcucBswmdIntegerPara bswmdInteger:
                                        // For EcucBswmdIntegerPara
                                        switch (bswmdInteger.Format)
                                        {
                                            // Hex format, 10 base string to integer and then to 16 base string
                                            case "HEX":
                                                try
                                                {
                                                    return $"0x{Convert.ToInt64(instancePara.Value, 10):X}";
                                                }
                                                catch
                                                {
                                                    throw new Exception($"Invalid format of value {instancePara.Value}");
                                                }

                                            // Dec format
                                            default:
                                                return instancePara.Value;
                                        }

                                    case EcucBswmdBooleanPara:
                                        // For EcucBswmdBooleanPara
                                        if (instancePara.Value == "0")
                                        {
                                            return "false";
                                        }
                                        else
                                        {
                                            return "true";
                                        }

                                    // For others, no need to change
                                    default:
                                        return instancePara.Value;
                                }

                            // For others, no need to change
                            default:
                                return instancePara.Value;
                        }

                    // For IEcucInstanceReference, ValueRef will be returned
                    case IEcucInstanceReference instanceRef:
                        return instanceRef.ValueRef;

                    default:
                        throw new Exception("invalid instance type");
                }
            }
            set
            {
                switch (Instance)
                {
                    case IEcucInstanceModule instanceContainer:
                        instanceContainer.ShortName = value;
                        break;

                    case IEcucInstanceParam instancePara:
                        switch (Instance)
                        {
                            case EcucInstanceNumericalParamValue:
                                switch (Bswmd)
                                {
                                    case EcucBswmdIntegerPara bswmdInteger:
                                        switch (bswmdInteger.Format)
                                        {
                                            case "HEX":
                                                try
                                                {
                                                    instancePara.Value = Convert.ToInt64(value, 16).ToString();
                                                }
                                                catch
                                                {
                                                    throw new Exception($"invalid format of value {value}");
                                                }
                                                break;

                                            default:
                                                instancePara.Value = value;
                                                break;
                                        }
                                        break;

                                    default:
                                        instancePara.Value = value;
                                        break;
                                }
                                break;

                            default:
                                instancePara.Value = value;
                                break;
                        }
                        break;

                    case IEcucInstanceReference instanceRef:
                        instanceRef.ValueRef = value;
                        break;

                    default:
                        break;
                }
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Value of EcucData convert to Int64.
        /// For IEcucInstanceParam, Parameter value with will be returned.
        /// </summary>
        public Int64 ValueAsInt
        {
            get
            {
                if (Instance is EcucInstanceNumericalParamValue instancePara && Bswmd is EcucBswmdIntegerPara)
                {
                    try
                    {
                        return Convert.ToInt64(instancePara.Value, 10);
                    }
                    catch
                    {
                        throw new Exception($"Invalid format of value {instancePara.Value}");
                    }
                }
                throw new Exception($"Instance can not convert to integer.");
            }
            set
            {
                if (Instance is EcucInstanceNumericalParamValue instancePara && Bswmd is EcucBswmdIntegerPara bswmdInteger)
                {
                    switch (bswmdInteger.Format)
                    {
                        case "HEX":
                            try
                            {
                                instancePara.Value = Convert.ToString(value, 16);
                            }
                            catch
                            {
                                throw new Exception($"invalid format of value {value}");
                            }
                            break;

                        default:
                            instancePara.Value = Convert.ToString(value, 10);
                            break;
                    }
                }
                RaisePropertyChanged();
            }
        }

        public string ValueShort
        {
            get
            {
                var parts = Value.Split('/');
                if (parts.Length > 0)
                {
                    return parts[^1];
                }
                return "";
            }
            set
            {
                var parts = Value.Split('/');
                parts[^1] = value;
                Value = string.Join("/", parts);
                RaisePropertyChanged();
            }
        }

        public EcucData(IEcucInstanceBase instance, IEcucBswmdBase bswmd)
        {
            Instance = instance;
            Bswmd = bswmd;

            Instance.PropertyChanged += PropertyChangedEventHandler;
        }

        public EcucData(EcucInstanceManager isntanceManager, EcucBswmdManager bswmdManager, string name)
        {
            var instanceModule = isntanceManager[name];
            if (instanceModule == null)
            {
                throw new Exception($"Can not get instance with name {name}");
            }
            var bswmdModule = bswmdManager.GetBswmdFromBswmdPath(instanceModule.BswmdPath);
            if (bswmdModule == null)
            {
                throw new Exception($"Can not get bswmd with path {instanceModule.BswmdPath}");
            }
            Instance = instanceModule;
            Bswmd = bswmdModule;
        }

        public EcucDataList this[string key]
        {
            get
            {
                var result = new EcucDataList();
                if (Bswmd != null)
                {
                    var bswmdGet = Bswmd.GetBswmdFromBswmdPath($"{Instance.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                if (Instance is IEcucInstanceModule instanceContainer && Bswmd is IEcucBswmdModule bswmdContainer)
                                {
                                    var query = from container in instanceContainer.Containers
                                                where container.BswmdPathShort == key
                                                select container;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from para in instanceOtherContainer.Paras
                                                where para.BswmdPathShort == key
                                                select para;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from reference in instanceOtherContainer.Refs
                                                where reference.BswmdPathShort == key
                                                select reference;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                return result;
            }
        }

        public EcucDataList this[string key, string name]
        {
            get
            {
                var result = new EcucDataList();
                if (Bswmd != null)
                {
                    var bswmdGet = Bswmd.GetBswmdFromBswmdPath($"{Instance.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                if (Instance is IEcucInstanceModule instanceContainer && Bswmd is IEcucBswmdModule bswmdContainer)
                                {
                                    var query = from container in instanceContainer.Containers
                                                where container.BswmdPathShort == key
                                                where container.ShortName == name
                                                select container;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from para in instanceOtherContainer.Paras
                                                where para.BswmdPathShort == key
                                                select para;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from reference in instanceOtherContainer.Refs
                                                where reference.BswmdPathShort == key
                                                where reference.ValueRefShort == name
                                                select reference;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                return result;
            }
        }

        public delegate bool FilterFunc(EcucData data);
        public EcucDataList this[string key, FilterFunc func]
        {
            get
            {
                var result = new EcucDataList();
                if (Bswmd != null)
                {
                    var bswmdGet = Bswmd.GetBswmdFromBswmdPath($"{Instance.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                if (Instance is IEcucInstanceModule instanceContainer && Bswmd is IEcucBswmdModule bswmdContainer)
                                {
                                    var query = from container in instanceContainer.Containers
                                                where container.BswmdPathShort == key
                                                where func(new EcucData(container, bswmdGet))
                                                select container;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from para in instanceOtherContainer.Paras
                                                where para.BswmdPathShort == key
                                                where func(new EcucData(para, bswmdGet))
                                                select para;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                if (Instance is IEcucInstanceContainer instanceOtherContainer && Bswmd is IEcucBswmdContainer bswmdOtherContainer)
                                {
                                    var query = from reference in instanceOtherContainer.Refs
                                                where reference.BswmdPathShort == key
                                                where func(new EcucData(reference, bswmdGet))
                                                select reference;

                                    query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                return result;
            }
        }

        private string RecommendedName(string bswmdPathShort)
        {
            if (Bswmd is IEcucBswmdModule bswmd && Instance is IEcucInstanceModule instance)
            {
                foreach (var bswmdSubContainer in bswmd.Containers)
                {
                    if (bswmdSubContainer.AsrPathShort == bswmdPathShort)
                    {
                        var instances = instance.FindContainerByBswmd(bswmdSubContainer.AsrPath);
                        bool okFlag = true;
                        var recommendedName = bswmdSubContainer.AsrPathShort;

                        foreach (var container in instances)
                        {
                            if (container.ShortName == recommendedName)
                            {
                                okFlag = false;
                                break;
                            }
                        }

                        if (okFlag == true)
                        {
                            return recommendedName;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            okFlag = true;
                            recommendedName = $"{bswmdSubContainer.AsrPathShort}_{i:D3}";
                            foreach (var container in instances)
                            {
                                if (container.ShortName == recommendedName)
                                {
                                    okFlag = false;
                                    break;
                                }
                            }

                            if (okFlag == true)
                            {
                                return recommendedName;
                            }
                        }
                    }
                }
                throw new Exception("Fail to find recommended name due to every candidate is used");
            }
            else
            {
                throw new Exception("Fail to find recommended name due to wrong Bswmd or Instance type");
            }
        }

        public EcucData AddContainer(string bswmdPathShort)
        {
            return AddContainer(bswmdPathShort, RecommendedName(bswmdPathShort));
        }

        public EcucData AddContainer(string bswmdPathShort, string shortName)
        {
            if (Bswmd is IEcucBswmdModule bswmdContainerContainer && Instance is IEcucInstanceModule instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Containers)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanAdd == true)
                        {
                            switch (bswmd)
                            {
                                case EcucBswmdContainer:
                                    {
                                        var instance = instanceContainerContainer.AddContainer(bswmdPathShort, shortName);
                                        RaisePropertyChanged();
                                        return new EcucData(instance, bswmd);
                                    }

                                case EcucBswmdChoiceContainer:
                                    {
                                        var instance = instanceContainerContainer.AddContainer(bswmdPathShort, shortName);
                                        RaisePropertyChanged();
                                        return new EcucData(instance, bswmd);
                                    }

                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not add container due to upper limit");
                        }
                    }
                }
                throw new Exception($"Unexpected bswmd path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public int DelContainer(string bswmdPathShort)
        {
            if (Bswmd is IEcucBswmdModule bswmdContainerContainer && Instance is IEcucInstanceModule instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Containers)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (bswmd.IsRequired == false)
                        {
                            RaisePropertyChanged();
                            return instanceContainerContainer.DelContainer(bswmd.AsrPath);
                        }
                        else
                        {
                            throw new Exception($"Can not delete container due to this is a required container");
                        }
                    }
                }
                throw new Exception($"Can not delete container due to invalid path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public int DelContainer(string bswmdPathShort, string shortName)
        {
            if (Bswmd is IEcucBswmdModule bswmdContainerContainer && Instance is IEcucInstanceModule instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Containers)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanRemove == true)
                        {
                            RaisePropertyChanged();
                            return instanceContainerContainer.DelContainer(bswmd.AsrPath, shortName);
                        }
                        else
                        {
                            throw new Exception($"Can not delete container due to lower limit");
                        }
                    }
                }
                throw new Exception($"Can not delete container due to invalid path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public EcucData AddContainerWithRequiredField(string bswmdPathShort)
        {
            return AddContainerWithRequiredField(bswmdPathShort, RecommendedName(bswmdPathShort));
        }

        public EcucData AddContainerWithRequiredField(string bswmdPathShort, string shortName)
        {
            if (Bswmd is IEcucBswmdModule bswmdContainerContainer && Instance is IEcucInstanceModule instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Containers)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanAdd == true)
                        {

                            switch (bswmd)
                            {
                                case EcucBswmdContainer:
                                case EcucBswmdChoiceContainer:
                                    var instance = instanceContainerContainer.AddContainer(bswmdPathShort, shortName);
                                    var dataNew = new EcucData(instance, bswmd);
                                    if (bswmd is EcucBswmdContainer bswmdContainer)
                                    {
                                        foreach (var bswmd2 in bswmdContainer.Containers)
                                        {
                                            if (bswmd2.IsRequired == true)
                                            {
                                                dataNew.AddContainerWithRequiredField(bswmd2.AsrPathShort, bswmd2.AsrPathShort);
                                            }
                                        }
                                        foreach (var bswmd3 in bswmdContainer.Paras)
                                        {
                                            if (bswmd3.IsRequired == true)
                                            {
                                                switch (bswmd3)
                                                {
                                                    case EcucBswmdEnumerationPara bswmdEnum:
                                                        dataNew.AddPara(bswmdEnum.AsrPathShort, bswmdEnum.Default);
                                                        break;

                                                    case EcucBswmdIntegerPara bswmdIntegert:
                                                        dataNew.AddPara(bswmdIntegert.AsrPathShort, bswmdIntegert.Default.ToString());
                                                        break;

                                                    case EcucBswmdBooleanPara bswmdBoolean:
                                                        dataNew.AddPara(bswmdBoolean.AsrPathShort, bswmdBoolean.Default.ToString());
                                                        break;

                                                    case EcucBswmdFloatPara bswmdFloat:
                                                        dataNew.AddPara(bswmdFloat.AsrPathShort, bswmdFloat.Default.ToString());
                                                        break;

                                                    case EcucBswmdStringPara bswmdString:
                                                        if (bswmdString.Default.Count > 0)
                                                        {
                                                            dataNew.AddPara(bswmdString.AsrPathShort, bswmdString.Default[0]);
                                                        }
                                                        else
                                                        {
                                                            dataNew.AddPara(bswmdString.AsrPathShort, "");
                                                        }
                                                        break;

                                                    case EcucBswmdFunctionNamePara bswmdFunction:
                                                        dataNew.AddPara(bswmdFunction.AsrPathShort, bswmdFunction.Default[0]);
                                                        break;

                                                    default:
                                                        throw new NotImplementedException();
                                                }
                                            }
                                        }
                                    }
                                    RaisePropertyChanged();
                                    return dataNew;

                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not add container due to upper limit");
                        }
                    }
                }
                throw new Exception($"Unexpected bswmd path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public List<EcucData> ContainerChain(Dictionary<string, string> chain)
        {
            var result = new List<EcucData>();

            if (Instance is IEcucInstanceModule instanceContainerContainer)
            {
                EcucData data = this;
                foreach (var kvp in chain)
                {
                    try
                    {
                        var instanceBase = instanceContainerContainer.GetInstanceFromAsrPath($"{instanceContainerContainer.AsrPath}/{kvp.Value}");
                        if (instanceBase.BswmdPath == $"{Bswmd.AsrPath}/{kvp.Key}")
                        {
                            var bswmd = Bswmd.GetBswmdFromBswmdPath(instanceBase.BswmdPath);
                            if (bswmd != null)
                            {
                                data = new EcucData(instanceBase, bswmd);
                                result.Add(data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        data = data.AddContainer(kvp.Key, kvp.Value);
                        result.Add(data);
                    }
                }
            }
            return result;
        }

        public EcucData AddPara(string bswmdPathShort, string value)
        {
            if (Bswmd is IEcucBswmdContainer bswmdContainerContainer && Instance is IEcucInstanceContainer instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Paras)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanAdd == true)
                        {
                            if (instanceContainerContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdEnumerationPara bswmdEnum:
                                        if (bswmdEnum.Candidate.Count > 0)
                                        {
                                            if (bswmdEnum.Candidate.Contains(value) == true)
                                            {
                                                var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, value, "ECUC-ENUMERATION-PARAM-DEF");
                                                RaisePropertyChanged();
                                                return new EcucData(textualPara, bswmd);
                                            }
                                            else
                                            {
                                                throw new Exception($"Can not add Enumerate Parameter {bswmdPathShort} due to value {value} not in candidate");
                                            }
                                        }
                                        else
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, value, "ECUC-ENUMERATION-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    case EcucBswmdIntegerPara bswmdIntegert:
                                        if (BigInteger.TryParse(value, out BigInteger integer) == true)
                                        {
                                            if (bswmdIntegert.Max != null)
                                            {
                                                if (integer <= bswmdIntegert.Max && integer >= bswmdIntegert.Min)
                                                {
                                                    var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, value, "ECUC-INTEGER-PARAM-DEF");
                                                    RaisePropertyChanged();
                                                    return new EcucData(numericalPara, bswmd);
                                                }
                                                else
                                                {
                                                    throw new Exception($"Can not add integer parameter {bswmdPathShort} due to beyond range, Max is {bswmdIntegert.Max}, Min is {bswmdIntegert.Min}, Value is {value}");
                                                }
                                            }
                                            else
                                            {
                                                if (integer >= bswmdIntegert.Min)
                                                {
                                                    var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, value, "ECUC-INTEGER-PARAM-DEF");
                                                    RaisePropertyChanged();
                                                    return new EcucData(numericalPara, bswmd);
                                                }
                                                else
                                                {
                                                    throw new Exception($"Can not add integer parameter {bswmdPathShort} due to beyond range, Min is {bswmdIntegert.Min}, Value is {value}");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception($"Can not add Integer Parameter {bswmdPathShort} due to can not convert value {value}");
                                        }

                                    case EcucBswmdBooleanPara:
                                        if (bool.TryParse(value, out _) == true)
                                        {
                                            var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, value, "ECUC-BOOLEAN-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(numericalPara, bswmd);
                                        }
                                        else
                                        {
                                            throw new Exception($"Can not add boolean parameter {bswmdPathShort} dut to can not convert {value} to boolean");
                                        }

                                    case EcucBswmdFloatPara:
                                        if (Double.TryParse(value, out _) == true)
                                        {
                                            var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, value, "ECUC-FLOAT-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(numericalPara, bswmd);
                                        }
                                        else
                                        {
                                            throw new Exception($"Can not add float parameter {bswmdPathShort} dut to can not convert {value} to float");
                                        }

                                    case EcucBswmdStringPara:
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, value, "ECUC-STRING-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    case EcucBswmdFunctionNamePara:
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, value, "ECUC-FUNCTION-NAME-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not add container due to upper limit");
                        }
                    }
                }
                throw new Exception($"Unexpected bswmd path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public EcucData AddPara(string bswmdPathShort)
        {
            if (Bswmd is IEcucBswmdContainer bswmdContainerContainer && Instance is IEcucInstanceContainer instanceContainerContainer)
            {
                foreach (var bswmd in bswmdContainerContainer.Paras)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanAdd == true)
                        {
                            if (instanceContainerContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdEnumerationPara bswmdEnum:
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, bswmdEnum.Default, "ECUC-ENUMERATION-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    case EcucBswmdIntegerPara bswmdIntegert:
                                        {
                                            var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, bswmdIntegert.Default.ToString(), "ECUC-INTEGER-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(numericalPara, bswmd);
                                        }

                                    case EcucBswmdBooleanPara BswmdBoolean:
                                        {
                                            var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, BswmdBoolean.Default.ToString(), "ECUC-BOOLEAN-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(numericalPara, bswmd);
                                        }

                                    case EcucBswmdFloatPara BswmdFloat:
                                        {
                                            var numericalPara = instanceContainer.AddNumericalPara(bswmdPathShort, BswmdFloat.Default.ToString(), "ECUC-FLOAT-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(numericalPara, bswmd);
                                        }

                                    case EcucBswmdStringPara BswmdString:
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, BswmdString.Default[0], "ECUC-STRING-PARAM-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    case EcucBswmdFunctionNamePara BswmdFunction:
                                        {
                                            var textualPara = instanceContainer.AddTextualPara(bswmdPathShort, BswmdFunction.Default[0], "ECUC-FUNCTION-NAME-DEF");
                                            RaisePropertyChanged();
                                            return new EcucData(textualPara, bswmd);
                                        }

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not add container due to upper limit");
                        }
                    }
                }
                throw new Exception($"Unexpected bswmd path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public List<EcucData> ParaChain(Dictionary<string, string> chain)
        {
            var result = new List<EcucData>();

            foreach (var kvp in chain)
            {
                result.Add(AddPara(kvp.Key, kvp.Value));
            }
            return result;
        }

        public void DelPara(string bswmdPathShort, string value)
        {
            if (Bswmd is IEcucBswmdContainer bswmdOtherContainer && Instance is IEcucInstanceContainer instanceOtherContainer)
            {
                foreach (var bswmd in bswmdOtherContainer.Paras)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanRemove == true)
                        {
                            if (instanceOtherContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdEnumerationPara:
                                        instanceContainer.DelTextualPara(bswmdPathShort, value);
                                        break;

                                    case EcucBswmdIntegerPara:
                                        instanceContainer.DelNumericalPara(bswmdPathShort, value);
                                        break;

                                    case EcucBswmdBooleanPara:
                                        instanceContainer.DelNumericalPara(bswmdPathShort, value);
                                        break;

                                    case EcucBswmdFloatPara:
                                        instanceContainer.DelNumericalPara(bswmdPathShort, value);
                                        break;

                                    case EcucBswmdStringPara:
                                        instanceContainer.DelTextualPara(bswmdPathShort, value);
                                        break;

                                    case EcucBswmdFunctionNamePara:
                                        instanceContainer.DelTextualPara(bswmdPathShort, value);
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        public void DelPara(string bswmdPath)
        {
            if (Bswmd is IEcucBswmdContainer bswmdOtherContainer && Instance is IEcucInstanceContainer instanceOtherContainer)
            {
                foreach (var bswmd in bswmdOtherContainer.Paras)
                {
                    if (bswmd.AsrPathShort == bswmdPath)
                    {
                        if (bswmd.IsRequired == false)
                        {
                            if (instanceOtherContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdEnumerationPara:
                                        instanceContainer.DelTextualPara(bswmdPath);
                                        break;

                                    case EcucBswmdIntegerPara:
                                        instanceContainer.DelNumericalPara(bswmdPath);
                                        break;

                                    case EcucBswmdBooleanPara:
                                        instanceContainer.DelNumericalPara(bswmdPath);
                                        break;

                                    case EcucBswmdFloatPara:
                                        instanceContainer.DelNumericalPara(bswmdPath);
                                        break;

                                    case EcucBswmdStringPara:
                                        instanceContainer.DelTextualPara(bswmdPath);
                                        break;

                                    case EcucBswmdFunctionNamePara:
                                        instanceContainer.DelTextualPara(bswmdPath);
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        public EcucData AddRef(string bswmdPathShort, string value, string typ = "")
        {
            if (Bswmd is IEcucBswmdContainer bswmdOtherContainer && Instance is IEcucInstanceContainer instanceOtherContainer)
            {
                foreach (var bswmd in bswmdOtherContainer.Refs)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanAdd == true)
                        {
                            if (instanceOtherContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdRef bswmdRef:
                                        {
                                            var reference = instanceContainer.AddReference(bswmdPathShort, "ECUC-REFERENCE-DEF", value, bswmdRef.Dest);
                                            RaisePropertyChanged();
                                            return new EcucData(reference, bswmd);
                                        }

                                    case EcucBswmdForeignRef bswmdForeign:
                                        {
                                            var reference = instanceContainer.AddReference(bswmdPathShort, "ECUC-FOREIGN-REFERENCE-DEF", value, bswmdForeign.DestType);
                                            RaisePropertyChanged();
                                            return new EcucData(reference, bswmd);
                                        }

                                    case EcucBswmdSymbolicNameRef bswmdSymbolic:
                                        {
                                            var reference = instanceContainer.AddReference(bswmdPathShort, "ECUC-SYMBOLIC-NAME-REFERENCE-DEF", value, bswmdSymbolic.Dest);
                                            RaisePropertyChanged();
                                            return new EcucData(reference, bswmd);
                                        }

                                    case EcucBswmdChoiceRef bswmdChoice:
                                        {
                                            foreach (var choice in bswmdChoice.Choices)
                                            {
                                                if (typ == choice.Key)
                                                {
                                                    var reference = instanceContainer.AddReference(bswmdPathShort, choice.Value, value, choice.Key);
                                                    RaisePropertyChanged();
                                                    return new EcucData(reference, bswmd);
                                                }
                                            }
                                            throw new Exception($"Unexpected input type {typ}");
                                        }

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not add container due to upper limit");
                        }
                    }
                }
                throw new Exception($"Unexpected bswmd path {bswmdPathShort}");
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public List<EcucData> RefChain(Dictionary<string, string> chain)
        {
            var result = new List<EcucData>();

            foreach (var kvp in chain)
            {
                result.Add(AddRef(kvp.Key, kvp.Value));
            }
            return result;
        }

        public EcucData DeRef()
        {
            if (Instance is EcucInstanceReferenceValue instanceRef)
            {
                var referenced = Instance.GetInstanceFromAsrPath(instanceRef.ValueRef);

                if (referenced != null)
                {
                    var bswmd = Bswmd.GetBswmdFromBswmdPath(referenced.BswmdPath);
                    if (bswmd != null)
                    {
                        return new EcucData(referenced, bswmd);
                    }
                }
            }
            throw new Exception($"Can not dereference");
        }

        public void DelRef(string bswmdPathShort, string valueRef)
        {
            if (Bswmd is IEcucBswmdContainer bswmdOtherContainer && Instance is IEcucInstanceContainer instanceOtherContainer)
            {
                foreach (var bswmd in bswmdOtherContainer.Refs)
                {
                    if (bswmd.AsrPathShort == bswmdPathShort)
                    {
                        if (GetMultiplyStatus(bswmd.AsrPath).CanRemove == true)
                        {
                            if (instanceOtherContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdRef:
                                        instanceContainer.DelReference(bswmdPathShort, valueRef);
                                        break;


                                    case EcucBswmdForeignRef:
                                        instanceContainer.DelReference(bswmdPathShort, valueRef);
                                        break;


                                    case EcucBswmdSymbolicNameRef:
                                        instanceContainer.DelReference(bswmdPathShort, valueRef);
                                        break;

                                    case EcucBswmdChoiceRef:
                                        instanceContainer.DelReference(bswmdPathShort, valueRef);
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                };
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not delete container due to lower limit");
                        }
                    }
                }
                RaisePropertyChanged();
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public void DelRef(string bswmdPath)
        {
            if (Bswmd is IEcucBswmdContainer bswmdOtherContainer && Instance is IEcucInstanceContainer instanceOtherContainer)
            {
                foreach (var bswmd in bswmdOtherContainer.Refs)
                {
                    if (bswmd.AsrPathShort == bswmdPath)
                    {
                        if (bswmd.IsRequired == false)
                        {
                            if (instanceOtherContainer is EcucInstanceContainer instanceContainer)
                            {
                                switch (bswmd)
                                {
                                    case EcucBswmdRef:
                                        instanceContainer.DelReference(bswmdPath);
                                        break;


                                    case EcucBswmdForeignRef:
                                        instanceContainer.DelReference(bswmdPath);
                                        break;


                                    case EcucBswmdSymbolicNameRef:
                                        instanceContainer.DelReference(bswmdPath);
                                        break;

                                    case EcucBswmdChoiceRef:
                                        instanceContainer.DelReference(bswmdPath);
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                };
                            }
                            else
                            {
                                throw new Exception($"Unexpected instance type {Instance.GetType()}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Can not delete container due to lower limit");
                        }
                    }
                }
                RaisePropertyChanged();
            }
            else
            {
                throw new Exception($"Unexpected bswmd type {Bswmd.GetType()} or instance type {Instance.GetType()}");
            }
        }

        public EcucDataMultiplyStatus GetMultiplyStatus(string bswmdPath)
        {
            EcucDataMultiplyStatus.StatusType result = EcucDataMultiplyStatus.StatusType.Invalid;
            try
            {
                var bswmd = Bswmd.GetBswmdFromBswmdPath(bswmdPath);
                int count = 0;

                switch (bswmd)
                {
                    case IEcucBswmdModule:
                        {
                            var instances = InstanceAsModule.FindContainerByBswmd(bswmdPath);
                            count = instances.Count;
                        }
                        break;

                    case IEcucBswmdParam:
                        {
                            var instances = InstanceAsContainer.FindParaByBswmd(bswmdPath);
                            count = instances.Count;
                        }
                        break;

                    case IEcucBswmdReference:
                        {
                            var instances = InstanceAsContainer.FindRefByBswmd(bswmdPath);
                            count = instances.Count;
                        }
                        break;

                    default:
                        break;
                }

                if (count == 0)
                {
                    result |= EcucDataMultiplyStatus.StatusType.Empty;
                }

                if (count == bswmd.Upper)
                {
                    result |= EcucDataMultiplyStatus.StatusType.OkUpper;
                }

                if (count < bswmd.Lower)
                {
                    result |= EcucDataMultiplyStatus.StatusType.Lack;
                }
                else if (count == bswmd.Lower)
                {
                    result |= EcucDataMultiplyStatus.StatusType.OkLower;
                }
                else if (count < bswmd.Upper)
                {
                    result |= EcucDataMultiplyStatus.StatusType.OkMiddle;
                }
                else
                {
                    result |= EcucDataMultiplyStatus.StatusType.Overflow;
                }
            }
            catch
            {
                result = EcucDataMultiplyStatus.StatusType.Invalid;
            }

            return new EcucDataMultiplyStatus(result);
        }

        void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Valid")
            {
                RaisePropertyChanged(nameof(ValidStatus));
            }
        }

        public void UpdateValidStatus(bool v, string vReason = "")
        {
            Instance.Valid.UpdateData(this);
            Instance.Valid.UpdateValidStatus(v, vReason);
        }

        public void UpdateValidSolve(string desc, EcucSolveHandler handler, object? param=null)
        {
            Instance.Valid.UpdateSolve(desc, handler, param);
        }

        public void UpdateValidSolve(List<EcucSolve> solves)
        {
            Instance.Valid.UpdateSolve(solves);
        }

        public void ClearValidSolve()
        {
            Instance.Valid.ClearSolve();
        }
    }

    public class EcucDataList : List<EcucData>
    {
        /// <summary>
        /// Wheter data is valid.
        /// </summary>
        public bool ValidStatus
        {
            get
            {
                // iterate all data in datas checking Invalid
                foreach (var data in this)
                {
                    if (data.ValidStatus == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public string ValidInfo
        {
            get
            {
                string result = "";
                foreach (var data in this)
                {
                    result += data.ValidInfo;
                }
                return result;
            }
        }

        public List<string> AsrPath
        {
            get
            {
                var result = new List<string>();
                foreach (var data in this)
                {
                    result.Add(data.AsrPath);
                }
                return result;
            }
        }

        public List<string> AsrPathShort
        {
            get
            {
                var result = new List<string>();
                foreach (var data in this)
                {
                    result.Add(data.AsrPathShort);
                }
                return result;
            }
        }

        public EcucDataList this[string key]
        {
            get
            {
                var result = new EcucDataList();
                foreach (var data in this)
                {
                    var bswmdGet = data.GetBswmdFromBswmdPath($"{data.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                var query = from container in data.Containers
                                            where container.BswmdPathShort == key
                                            select container;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                var query = from para in data.Paras
                                            where para.BswmdPathShort == key
                                            select para;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                var query = from reference in data.Refs
                                            where reference.BswmdPathShort == key
                                            select reference;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        default:
                            break;
                    }
                }

                return result;
            }
        }

        public EcucDataList this[string key, string name]
        {
            get
            {
                var result = new EcucDataList();
                foreach (var data in this)
                {
                    var bswmdGet = data.GetBswmdFromBswmdPath($"{data.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                var query = from container in data.Containers
                                            where container.BswmdPathShort == key
                                            where container.ShortName == name
                                            select container;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                var query = from para in data.Paras
                                            where para.BswmdPathShort == key
                                            where para.Value == name
                                            select para;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                var query = from reference in data.Refs
                                            where reference.BswmdPathShort == key
                                            where reference.ValueRefShort == name
                                            select reference;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        default:
                            break;
                    }
                }

                return result;
            }
        }

        public delegate bool SearchFunc(EcucData data);
        public EcucDataList this[string key, SearchFunc func]
        {
            get
            {
                var result = new EcucDataList();
                foreach (var data in this)
                {
                    var bswmdGet = data.GetBswmdFromBswmdPath($"{data.BswmdPath}/{key}");
                    switch (bswmdGet)
                    {
                        case IEcucBswmdModule:
                            {
                                var query = from container in data.Containers
                                            where container.BswmdPathShort == key
                                            where func(new EcucData(container, bswmdGet))
                                            select container;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdParam:
                            {
                                var query = from para in data.Paras
                                            where para.BswmdPathShort == key
                                            where func(new EcucData(para, bswmdGet))
                                            select para;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        case IEcucBswmdReference:
                            {
                                var query = from reference in data.Refs
                                            where reference.BswmdPathShort == key
                                            where func(new EcucData(reference, bswmdGet))
                                            select reference;

                                query.ToList().ForEach(x => result.Add(new EcucData(x, bswmdGet)));
                            }
                            break;

                        default:
                            break;
                    }
                }

                return result;
            }
        }

        public EcucDataList()
        {

        }

        public EcucDataList(List<EcucData> datas)
        {
            foreach (var data in datas)
            {
                Add(data);
            }
        }

        public List<string> Value
        {
            get
            {
                var result = new List<String>();
                foreach (var data in this)
                {
                    result.Add(data.Value);
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    if (value.Count == Count)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            this[i].Value = value[i];
                        }
                    }
                }
            }
        }

        public bool ValueSingleEqual(string value)
        {
            if (Value.Count == 1)
            {
                return Value[0] == value;
            }
            return false;
        }

        public EcucData First
        {
            get
            {
                if (Count > 0)
                {
                    return this[0];
                }
                else
                {
                    throw new Exception($"There is no data inside");
                }
            }
        }

        public string FirstValue
        {
            get
            {
                if (Count > 0)
                {
                    return this[0].Value;
                }
                else
                {
                    throw new Exception($"There is no text inside");
                }
            }
            set
            {
                if (value != null)
                {
                    if (Count > 0)
                    {
                        this[0].Value = value;
                    }
                }
            }
        }

        public Int64 FirstValueAsInt
        {
            get
            {
                if (Count > 0)
                {
                    return this[0].ValueAsInt;
                }
                else
                {
                    throw new Exception($"There is no integer inside");
                }
            }
            set
            {
                if (Count > 0)
                {
                    this[0].ValueAsInt = value;
                }
            }
        }

        public EcucDataList DeRef()
        {
            var result = new EcucDataList();

            foreach (var data in this)
            {
                var referenced = data.DeRef();

                if (referenced != null)
                {
                    result.Add(referenced);
                }
            }
            return result;
        }

        public EcucDataMultiplyStatus GetMultiplyStatus(IEcucBswmdBase bswmd)
        {
            EcucDataMultiplyStatus.StatusType result = EcucDataMultiplyStatus.StatusType.Invalid;

            try
            {
                if (bswmd.Upper == 0)
                {
                    result = EcucDataMultiplyStatus.StatusType.Invalid;
                }
                else
                {
                    if (Count == 0)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.Empty;
                    }

                    if (Count == bswmd.Upper)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.OkUpper;
                    }

                    if (Count < bswmd.Lower)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.Lack;
                    }
                    else if (Count == bswmd.Lower)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.OkLower;
                    }
                    else if (Count < bswmd.Upper)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.OkMiddle;
                    }
                    else if (Count > bswmd.Upper)
                    {
                        result |= EcucDataMultiplyStatus.StatusType.Overflow;
                    }
                }
            }
            catch
            {
                result = EcucDataMultiplyStatus.StatusType.Invalid;
            }

            return new EcucDataMultiplyStatus(result);
        }
    }
}
