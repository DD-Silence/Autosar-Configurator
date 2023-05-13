/*  
 *  This file is a part of Autosar Configurator for ECU GUI based 
 *  configuration, checking and code generation.
 *  
 *  Copyright (C) 2021-2023 DJS Studio E-mail:ddsilence@sina.cn
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

using Arxml.Model;
using System.Numerics;
using GenTool_CsDataServerDomAsr4.Iface;

namespace Ecuc.EcucBase.EBswmd
{
    /// <summary>
    /// ECU Configuration BSW Module Description (EcucBswmd) interface.
    /// Base interface of all kind of BSW Module Description interface.
    /// </summary>
    public interface IEcucBswmdBase
    {
        /// <summary>
        /// Model of EcucBswmd.
        /// </summary>
        IEcucDefinitionElement Model { get; }
        /// <summary>
        /// Manager of this class..
        /// </summary>
        EcucBswmdManager Manager { get; }
        /// <summary>
        /// ShortName of EcucBswmd module.
        /// </summary>
        string ShortName { get; }
        /// <summary>
        /// Description of EcucBswmd module.
        /// </summary>
        string Desc { get; }
        /// <summary>
        /// Trace of EcucBswmd module.
        /// </summary>
        string Trace { get; }
        /// <summary>
        /// Smallest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        uint Lower { get; }
        /// <summary>
        /// Biggest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        uint Upper { get; }
        /// <summary>
        /// Autosar path of Bswmd module.
        /// </summary>
        string AsrPath { get; }
        /// <summary>
        /// Short form of AsrPath.
        /// </summary>
        string AsrPathShort { get; }
        /// <summary>
        /// Whether this is a multiply EcucBswmd.
        /// Multiply means biggest quantity can be more than 1
        /// </summary>
        bool IsMultiply { get; }
        /// <summary>
        /// Whether this is a single EcucBswmd.
        /// Single means biggest quantity can only be 1
        /// </summary>
        bool IsSingle { get; }
        /// <summary>
        /// Whether this is a single EcucBswmd.
        /// Single means biggest quantity can only be 1
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Get EcucBswmd from BswmdPath.
        /// BswmdPath is Autosar path of EcucBswmd.
        /// It usually comes from EcucInstance DEFINITION-REF tag.
        /// </summary>
        /// <param name="bswmdPath">BswmdPath of EcucBswmd to be got.
        /// If bswmdPath is "", this will be returned.</param>
        /// <returns>EcucBswmd has Autosar path of bswmdPath</returns>
        /// <exception cref="Exception">Can not find EcucBswmd has Autosar path of bswmdPath</exception>
        IEcucBswmdBase GetBswmdFromBswmdPath(string bswmdPath);
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucBswmdBase : IEcucBswmdBase
    {
        /// <summary>
        /// Model of EcucBswmd
        /// </summary>
        public IEcucDefinitionElement Model { get; protected set; }
        /// <summary>
        /// Root data model of EcucBswmd.
        /// </summary>
        public EcucBswmdManager Manager { get; protected set; }

        /// <summary>
        /// ShortName of EcucBswmd module.
        /// </summary>
        public string ShortName => Model.ShortName;

        /// <summary>
        /// Description of EcucBswmd module.
        /// </summary>
        public string Desc
        {
            get
            {
                var result = "";

                if (Model.DescSpecified)
                {
                    foreach (var d in Model.Desc.L2List)
                    {
                        result += d.Value;
                    }
                }
                if (Model.IntroductionSpecified)
                {
                    foreach (var p in Model.Introduction.PList)
                    {
                        foreach (var l1 in p.L1List)
                        {
                            result += $"{Environment.NewLine}{l1.Value}";
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Trace of EcucBswmd module.
        /// </summary>
        public string Trace
        {
            get
            {
                var result = "";

                if (Model.IntroductionSpecified)
                {
                    foreach (var t in Model.Introduction.TraceList)
                    {
                        foreach (var l4 in t.LongName.L4List)
                        {
                            result += $"{l4.Value} ";
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Smallest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        public uint Lower => uint.Parse(Model.LowerMultiplicity.Value);

        /// <summary>
        /// Biggest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        public uint Upper
        {
            get
            {
                if (Model.UpperMultiplicityInfiniteSpecified)
                {
                    return uint.MaxValue;
                }
                else
                {
                    try
                    {
                        return uint.Parse(Model.UpperMultiplicity.Value);
                    }
                    catch
                    {
                        return uint.MaxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Autosar path of Bswmd module.
        /// </summary>
        public string AsrPath
        {
            get
            {
                return Model.AsrPath;
            }
        }

        /// <summary>
        /// Short form of AsrPath.
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
        /// Whether this is a multiply EcucBswmd.
        /// Multiply means biggest quantity can be more than 1
        /// </summary>
        public bool IsMultiply => Upper > 1;

        /// <summary>
        /// Whether this is a single EcucBswmd.
        /// Single means biggest quantity can only be 1
        /// </summary>
        public bool IsSingle => Upper == 1;

        /// <summary>
        /// Whether this is a single EcucBswmd.
        /// Single means biggest quantity can only be 1
        /// </summary>
        public bool IsRequired => Lower != 0;

        /// <summary>
        /// Get EcucBswmd from BswmdPath.
        /// BswmdPath is Autosar path of EcucBswmd.
        /// It usually comes from EcucInstance DEFINITION-REF tag.
        /// </summary>
        /// <param name="bswmdPath">BswmdPath of EcucBswmd to be got.
        /// If bswmdPath is "", this will be returned.</param>
        /// <returns>EcucBswmd has Autosar path of bswmdPath</returns>
        /// <exception cref="Exception">Can not find EcucBswmd has Autosar path of bswmdPath</exception>
        public IEcucBswmdBase GetBswmdFromBswmdPath(string bswmdPath)
        {
            if (bswmdPath == "")
            {
                // bswmdPath is "", return this
                return this;
            }
            else
            {
                try
                {
                    // get EcucBswmd from Root
                    return Manager.GetBswmdFromBswmdPath(bswmdPath);
                }
                catch
                {
                    throw new Exception($"Can not find bswmd with path {bswmdPath}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        public EcucBswmdBase(IEcucDefinitionElement model, EcucBswmdManager manager)
        {
            Model = model;
            Manager = manager;
        }
    }

    /// <summary>
    /// Bswmd module interface
    /// Base interface of all kind of Bswmd class with Containers member.
    /// </summary>
    public interface IEcucBswmdHasContainer : IEcucBswmdBase
    {
        /// <summary>
        /// Containers in EcucBswmd.
        /// </summary>
        List<IEcucBswmdContainer> Containers { get; }
    }

    /// <summary>
    /// Bswmd module interface
    /// Base interface of all kind of Bswmd class with parameter member.
    /// </summary>
    public interface IEcucBswmdHasParameter : IEcucBswmdBase
    {
        /// <summary>
        /// Containers in EcucBswmd.
        /// </summary>
        List<IEcucBswmdParam> Paras { get; }
    }

    /// <summary>
    /// Bswmd module interface
    /// Base interface of all kind of Bswmd class with reference member.
    /// </summary>
    public interface IEcucBswmdHasReference : IEcucBswmdBase
    {
        /// <summary>
        /// Containers in EcucBswmd.
        /// </summary>
        List<IEcucBswmdReference> Refs { get; }
    }

    /// <summary>
    /// Bswmd container interface
    /// Base interface of Bswmd container.
    /// </summary>
    public interface IEcucBswmdContainer : IEcucBswmdHasContainer, IEcucBswmdHasParameter, IEcucBswmdHasReference
    {

    }

    /// <summary>
    /// Bswmd parameter interface
    /// Base interface of all kind of Bswmd parameter in Bswmd container.
    /// </summary>
    public interface IEcucBswmdParam : IEcucBswmdBase
    {

    }

    /// <summary>
    /// Bswmd reference interface
    /// Base interface of all kind of Bswmd reference in Bswmd container.
    /// </summary>
    public interface IEcucBswmdReference : IEcucBswmdBase
    {

    }

    /// <summary>
    /// EcucBswmd top level manager of several arxml files.
    /// EcucBswmd elements in several arxml files are combined.
    /// </summary>
    public class EcucBswmdManager
    {
        /// <summary>
        /// Several Asr from several arxml files.
        /// </summary>
        private readonly ArFile arFile = new();
        /// <summary>
        /// EcucBswmd moudles converted from several arxml files.
        /// </summary>
        public List<EcucBswmdModule> Modules { get; } = new();

        /// <summary>
        /// Initialize EcucBswmd from several arxml files.
        /// </summary>
        /// <param name="fileNames">File names of arxml files.</param>
        public EcucBswmdManager(string[] fileNames)
        {
            InitMultiple(fileNames);
        }

        /// <summary>
        /// Internal procees of Initialization of EcucBswmd from serveral arxml files.
        /// </summary>
        /// <param name="fileNames"></param>
        private void InitMultiple(string[] fileNames)
        {
            arFile.AddFile(fileNames);

            if ((arFile != null) && (arFile.root != null))
            {
                List<IARPackage> arPackages = new();

                foreach (var package in arFile.root.ArPackageList)
                {
                    // collect all ArPackages.
                    arPackages.Add(package);
                }
                var query = from package in arFile.Packages
                            from element in package.ElementList
                            where element.IdType == ReferrableType.tEcucModuleDef
                            select element as IEcucModuleDef;
                query.ToList().ForEach(x => Modules.Add(new EcucBswmdModule(x, this)));
            }
        }

        /// <summary>
        /// Index EcucBswmd module by short name.
        /// </summary>
        /// <param name="shortName">Short name of targeted EcucBswmd module</param>
        /// <returns>EcucBswmd module with shortName</returns>
        /// <exception cref="Exception">Can not find EcucBswmd module with shortName</exception>
        public EcucBswmdModule this[string shortName]
        {
            get
            {
                var query = from module in Modules
                            where module.ShortName == shortName
                            select module;

                var result = query.ToList();
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                {
                    throw new Exception($"Can not find bswmd module {shortName}");
                }
            }
        }

        /// <summary>
        /// Get EcucBswmd from BswmdPath.
        /// BswmdPath is Autosar path of EcucBswmd.
        /// It usually comes from EcucInstance DEFINITION-REF tag.
        /// </summary>
        /// <param name="bswmdPath">BswmdPath of EcucBswmd to be got.
        /// If bswmdPath is "", this will be returned.</param>
        /// <returns>EcucBswmd has Autosar path of bswmdPath</returns>
        /// <exception cref="Exception">Can not find EcucBswmd has Autosar path of bswmdPath</exception>
        public IEcucBswmdBase GetBswmdFromBswmdPath(string bswmdPath)
        {
            if (arFile.root != null)
            {
                var metaObjs = arFile.root.Find(bswmdPath);
                if (metaObjs.Count == 0)
                {
                    foreach (var module in Modules)
                    {
                        if ((module.RefinedPath != "") && (bswmdPath.StartsWith(module.RefinedPath)))
                        {
                            bswmdPath = bswmdPath.Replace(module.RefinedPath, module.AsrPath);
                            metaObjs = arFile.root.Find(bswmdPath);
                            break;
                        }
                    }
                }
                if (metaObjs.Count > 0)
                {
                    foreach (var metaObj in metaObjs)
                    {
                        return metaObj switch
                        {
                            IEcucModuleDef moduleDef => new EcucBswmdModule(moduleDef, this),
                            IEcucParamConfContainerDef containerDef => new EcucBswmdContainer(containerDef, this),
                            IEcucChoiceContainerDef choiceContainerDef => new EcucBswmdChoiceContainer(choiceContainerDef, this),
                            IEcucEnumerationParamDef enumerationParamDef => new EcucBswmdEnumerationPara(enumerationParamDef, this),
                            IEcucIntegerParamDef integerParamDef => new EcucBswmdIntegerPara(integerParamDef, this),
                            IEcucBooleanParamDef booleanParamDef => new EcucBswmdBooleanPara(booleanParamDef, this),
                            IEcucFloatParamDef floatParamDef => new EcucBswmdFloatPara(floatParamDef, this),
                            IEcucStringParamDef stringParamDef => new EcucBswmdStringPara(stringParamDef, this),
                            IEcucFunctionNameDef functionNameDef => new EcucBswmdFunctionNamePara(functionNameDef, this),
                            IEcucReferenceDef referenceDef => new EcucBswmdRef(referenceDef, this),
                            IEcucForeignReferenceDef foreignReferenceDef => new EcucBswmdForeignRef(foreignReferenceDef, this),
                            IEcucSymbolicNameReferenceDef symbolicNameReferenceDef => new EcucBswmdSymbolicNameRef(symbolicNameReferenceDef, this),
                            IEcucChoiceReferenceDef choiceReferenceDef => new EcucBswmdChoiceRef(choiceReferenceDef, this),
                            _ => throw new Exception($"Invalid type when find bswmd path {bswmdPath}")
                        };
                    }
                }
            }
            throw new Exception($"Can not find bswmd path {bswmdPath}");
        }
    }

    /// <summary>
    /// EcucBswmd module class.
    /// EcucBswmd module class represent module of ECUCMODULEDEF tag.
    /// </summary>
    public class EcucBswmdModule : EcucBswmdBase, IEcucBswmdHasContainer
    {
        /// <summary>
        /// Containers of EcucBswmd module.
        /// </summary>
        public List<IEcucBswmdContainer> Containers
        {
            get
            {
                List<IEcucBswmdContainer> result = new();

                foreach (var container in ModelLocal.ContainerList)
                {
                    result.Add(container.IdType switch
                    {
                        ReferrableType.tEcucParamConfContainerDef => container is IEcucParamConfContainerDef paramConfContainerDef ? new EcucBswmdContainer(paramConfContainerDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucChoiceContainerDef => container is IEcucChoiceContainerDef choiceContainerDef ? new EcucBswmdChoiceContainer(choiceContainerDef, Manager) : throw new Exception("Invalid model type"),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }

        /// <summary>
        /// Local unpacked model
        /// </summary>
        public IEcucModuleDef ModelLocal
        {
            get
            {
                if (Model is IEcucModuleDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        public string RefinedPath
        {
            get
            {
                if (ModelLocal.RefinedModuleDefSpecified)
                {
                    return ModelLocal.RefinedModuleDef.Value;
                }
                return "";
            }
        }

        /// <summary>
        /// Initialize EcucBswmd module class.
        /// </summary>
        /// <param name="model">Data model converted from arxml file.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Autosar data model which contain this EcucBswmd module.</param>
        public EcucBswmdModule(IEcucModuleDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd container class.
    /// EcucBswmd container class represent tag ECUCPARAMCONFCONTAINERDEF.
    /// </summary>
    public class EcucBswmdContainer : EcucBswmdBase, IEcucBswmdContainer
    {
        /// <summary>
        /// Containers of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdContainer> Containers
        {
            get
            {
                List<IEcucBswmdContainer> result = new();

                foreach (var container in ModelLocal.SubContainerList)
                {
                    result.Add(container.IdType switch
                    {
                        ReferrableType.tEcucParamConfContainerDef => container is IEcucParamConfContainerDef paramConfContainerDef ? new EcucBswmdContainer(paramConfContainerDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucChoiceContainerDef => container is IEcucChoiceContainerDef choiceContainerDef ? new EcucBswmdChoiceContainer(choiceContainerDef, Manager) : throw new Exception("Invalid model type"),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }
        /// <summary>
        /// Parameters of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdParam> Paras
        {
            get
            {
                List<IEcucBswmdParam> result = new();

                foreach (var para in ModelLocal.ParameterList)
                {
                    result.Add(para.IdType switch
                    {
                        ReferrableType.tEcucEnumerationParamDef => para is IEcucEnumerationParamDef enumerationParamDef ? new EcucBswmdEnumerationPara(enumerationParamDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucIntegerParamDef => para is IEcucIntegerParamDef integerParamDef ? new EcucBswmdIntegerPara(integerParamDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucBooleanParamDef => para is IEcucBooleanParamDef booleanParamDef ? new EcucBswmdBooleanPara(booleanParamDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucFloatParamDef => para is IEcucFloatParamDef floatParamDef ? new EcucBswmdFloatPara(floatParamDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucStringParamDef => para is IEcucStringParamDef stringParamDef ? new EcucBswmdStringPara(stringParamDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucFunctionNameDef => para is IEcucFunctionNameDef functionNameDef ? new EcucBswmdFunctionNamePara(functionNameDef, Manager) : throw new Exception("Invalid model type"),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }
        /// <summary>
        /// References of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdReference> Refs
        {
            get
            {
                List<IEcucBswmdReference> result = new();
                foreach (var reference in ModelLocal.ReferenceList)
                {
                    result.Add(reference.IdType switch
                    {
                        ReferrableType.tEcucReferenceDef => reference is IEcucReferenceDef referenceDef ? new EcucBswmdRef(referenceDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucForeignReferenceDef => reference is IEcucForeignReferenceDef foreignReferenceDef ? new EcucBswmdForeignRef(foreignReferenceDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucSymbolicNameReferenceDef => reference is IEcucSymbolicNameReferenceDef symbolocNameReferenceDef ? new EcucBswmdSymbolicNameRef(symbolocNameReferenceDef, Manager) : throw new Exception("Invalid model type"),
                        ReferrableType.tEcucChoiceReferenceDef => reference is IEcucChoiceReferenceDef choiceReferenceDef ? new EcucBswmdChoiceRef(choiceReferenceDef, Manager) : throw new Exception("Invalid model type"),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }

        /// <summary>
        /// Local unpacked model
        /// </summary>
        public IEcucParamConfContainerDef ModelLocal
        {
            get
            {
                if (Model is IEcucParamConfContainerDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd container.
        /// </summary>
        /// <param name="model">Data model convert from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdContainer(IEcucParamConfContainerDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd choice container.
    /// Choice container contains several option containers.
    /// Only one of choice containers can exist in parent container.
    /// </summary>
    public class EcucBswmdChoiceContainer : EcucBswmdBase, IEcucBswmdContainer
    {
        /// <summary>
        /// Containers of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdContainer> Containers { get; } = new();
        /// <summary>
        /// Parameters of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdParam> Paras { get; } = new();
        /// <summary>
        /// References of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdReference> Refs { get; } = new();

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucChoiceContainerDef ModelLocal
        {
            get
            {
                if (Model is IEcucChoiceContainerDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd choice container.
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdChoiceContainer(IEcucChoiceContainerDef model, EcucBswmdManager manager)
            : base(model, manager)
        {
            // Parse children of EcucBswmd container
            foreach (var choice in ModelLocal.ChoiceList)
            {
                Containers.Add(new EcucBswmdContainer(choice, Manager));
            }
        }
    }

    /// <summary>
    /// EcucBswmd parameter of enumeration.
    /// Enumeration parameter with several candidate value.
    /// Only oen candidate is chosen as value.
    /// </summary>
    public class EcucBswmdEnumerationPara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Candidate strings.
        /// </summary>
        public List<string> Candidate { get; set; } = new();
        /// <summary>
        /// Default string.
        /// Default string shall one of candidate.
        /// </summary>
        public string Default { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucEnumerationParamDef ModelLocal
        {
            get
            {
                if (Model is IEcucEnumerationParamDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initi
        /// </summary>
        /// <param name="model">Data model converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdEnumerationPara(IEcucEnumerationParamDef model, EcucBswmdManager manager)
            : base(model, manager)
        {
            // Parse all candidate values
            foreach (var literal in ModelLocal.LiteralList)
            {
                Candidate.Add(literal.ShortName);
            }

            // Parse default value
            if (ModelLocal.DefaultValueSpecified)
            {
                Default = ModelLocal.DefaultValue;
            }
            else
            {
                if (Candidate.Count > 0)
                {
                    Default = Candidate[0];
                }
                else
                {
                    Default = "";
                }
            }
        }
    }

    /// <summary>
    /// EcucBswmd parameter of integer class.
    /// </summary>
    public class EcucBswmdIntegerPara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Minimun value of integer.
        /// </summary>
        public BigInteger Min { get; }
        /// <summary>
        /// Maxinum value of integer.
        /// </summary>
        public BigInteger? Max { get; }
        /// <summary>
        /// Default value of integer.
        /// </summary>
        public BigInteger Default { get; }
        /// <summary>
        /// Format of data displayed.
        /// HEX for 16 digital representation, DEC for 10 digital representation.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucIntegerParamDef ModelLocal
        {
            get
            {
                if (Model is IEcucIntegerParamDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd parameter of integer.
        /// </summary>
        /// <param name="model">Data model converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdIntegerPara(IEcucIntegerParamDef model, EcucBswmdManager manager)
            : base(model, manager)
        {
            // Parse mininum value
            if (!ModelLocal.MinSpecified)
            {
                Min = 0;
            }
            else
            {
                Min = BigInteger.Parse(ModelLocal.Min.Value);
            }

            // Parse maximun value if exist
            if (!ModelLocal.MaxSpecified)
            {
                Max = null;
            }
            else if (ModelLocal.Max.Value == "*")
            {
                Max = null;
            }
            else
            {
                Max = BigInteger.Parse(ModelLocal.Max.Value);
            }

            // Parse default value
            if (ModelLocal.DefaultValueSpecified)
            {
                if (ModelLocal.DefaultValue.Value == "")
                {
                    Default = Min;
                }
                else
                {
                    Default = BigInteger.Parse(ModelLocal.DefaultValue.Value);
                }
            }
            else
            {
                Default = Min;
            }

            // Parse format
            if (ModelLocal.AdminDataSpecified)
            {
                foreach (var f in ModelLocal.AdminData.SdgList)
                {
                    if (f.GidSpecified)
                    {
                        if (f.Gid == "DV:Display")
                        {
                            //var formats2 = f.SD;
                            //foreach (var f2 in formats2)
                            //{
                            //    if (f2.GID == "DV:DefaultFormat")
                            //    {
                            //        Format = f2.Untyped.Value;
                            //    }
                            //}
                        }
                    }
                }
            }

            // Check format at last
            Format ??= "";
        }
    }

    /// <summary>
    /// EcucBswmd parameter of boolean.
    /// </summary>
    public class EcucBswmdBooleanPara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Default value.
        /// false or true
        /// </summary>
        public bool Default { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucBooleanParamDef ModelLocal
        {
            get
            {
                if (Model is IEcucBooleanParamDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd boolean parameter.
        /// </summary>
        /// <param name="model">Model data converted form arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdBooleanPara(IEcucBooleanParamDef model, EcucBswmdManager manager)
            : base(model, manager)
        {
            // Parse default value
            if (ModelLocal.DefaultValueSpecified)
            {
                if (ModelLocal.DefaultValue.Value == "0")
                {
                    Default = false;
                }
                else if (ModelLocal.DefaultValue.Value == "1")
                {
                    Default = true;
                }
                else
                {
                    Default = bool.Parse(ModelLocal.DefaultValue.Value);
                }
            }
            else
            {
                Default = false;
            }
        }
    }

    /// <summary>
    /// EcucBswmd parameter of float.
    /// </summary>
    public class EcucBswmdFloatPara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public double Default { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucFloatParamDef ModelLocal
        {
            get
            {
                if (Model is IEcucFloatParamDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd float parameter.
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdFloatPara(IEcucFloatParamDef model, EcucBswmdManager manager)
            : base(model, manager)
        {
            // Parse default value
            if (ModelLocal.DefaultValueSpecified)
            {
                Default = double.Parse(ModelLocal.DefaultValue.Value);
            }
            else
            {
                Default = 0.0f;
            }
        }
    }

    /// <summary>
    /// EcucBswmd parameter of string.
    /// </summary>
    public class EcucBswmdStringPara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public List<string> Default
        {
            get
            {
                List<string> result = new();

                foreach (var variant in ModelLocal.EcucStringParamDefVariantList)
                {
                    if (variant.DefaultValueSpecified)
                    {
                        result.Add(variant.DefaultValue);
                    }
                }
                if (result.Count == 0)
                {
                    result.Add("");
                }
                return result;
            }
        }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucStringParamDef ModelLocal
        {
            get
            {
                if (Model is IEcucStringParamDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdStringPara(IEcucStringParamDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd paramter of function name.
    /// </summary>
    public class EcucBswmdFunctionNamePara : EcucBswmdBase, IEcucBswmdParam
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public List<string> Default
        {
            get
            {
                List<string> result = new();

                foreach (var variant in ModelLocal.EcucFunctionNameDefVariantList)
                {
                    if (variant.DefaultValueSpecified)
                    {
                        result.Add(variant.DefaultValue);
                    }
                }
                if (result.Count == 0)
                {
                    result.Add("");
                }
                return result;
            }
        }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucFunctionNameDef ModelLocal
        {
            get
            {
                if (Model is IEcucFunctionNameDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Initialize EcucBswmd function name parameter.
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdFunctionNamePara(IEcucFunctionNameDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBsemd reference.
    /// </summary>
    public class EcucBswmdRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucReferenceDef ModelLocal
        {
            get
            {
                if (Model is IEcucReferenceDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Reference desitination Bswmd path.
        /// </summary>
        public string DestPath
        {
            get
            {
                if (ModelLocal.DestinationSpecified)
                {
                    return ModelLocal.Destination.Value;
                }
                return "";
            }
        }

        /// <summary>
        /// Reference desitination tag.
        /// </summary>
        public ReferrableType Dest
        {
            get
            {
                if (ModelLocal.DestinationSpecified)
                {
                    return ModelLocal.Destination.DestType;
                }
                return ReferrableType.tXfile;
            }
        }

        /// <summary>
        /// Initialize EcucBswmd reference.
        /// </summary>
        /// <param name="model">Data model converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdRef(IEcucReferenceDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd reference from tag other than Ecuc tags.
    /// </summary>
    public class EcucBswmdForeignRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucForeignReferenceDef ModelLocal
        {
            get
            {
                if (Model is IEcucForeignReferenceDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Reference desitination tag.
        /// </summary>
        public string DestType
        {
            get
            {
                if (ModelLocal.DestinationTypeSpecified)
                {
                    return ModelLocal.DestinationType;
                }
                return "";
            }
        }

        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Parent EcucBswmd container.</param>
        /// <param name="refinedPath"></param>
        public EcucBswmdForeignRef(IEcucForeignReferenceDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd reference of symbolic name.
    /// </summary>
    public class EcucBswmdSymbolicNameRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucSymbolicNameReferenceDef ModelLocal
        {
            get
            {
                if (Model is IEcucSymbolicNameReferenceDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Reference desitination tag.
        /// </summary>
        public ReferrableType Dest
        {
            get
            {
                if (ModelLocal.DestinationSpecified)
                {
                    return ModelLocal.Destination.DestType;
                }
                return ReferrableType.tXfile;
            }
        }

        /// <summary>
        /// Reference desitination desitination tag.
        /// </summary>
        public string DestPath
        {
            get
            {
                if (ModelLocal.DestinationSpecified)
                {
                    return ModelLocal.Destination.Value;
                }
                return "";
            }
        }

        /// <summary>
        /// Initialize EcucBswmd symbolic name.
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        public EcucBswmdSymbolicNameRef(IEcucSymbolicNameReferenceDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// EcucBswmd reference of choice reference.
    /// </summary>
    public class EcucBswmdChoiceRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public IEcucChoiceReferenceDef ModelLocal
        {
            get
            {
                if (Model is IEcucChoiceReferenceDef modelLocal)
                {
                    return modelLocal;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        /// <summary>
        /// Parse choices reference.
        /// </summary>
        public Dictionary<string, ReferrableType> Choices
        {
            get
            {
                var result = new Dictionary<string, ReferrableType>();
                foreach (var destRef in ModelLocal.DestinationList)
                {
                    result[destRef.Value] = destRef.DestType;
                }
                return result;
            }
        }

        /// <summary>
        /// Initialize EcucBswmd reference of choices.
        /// </summary>
        /// <param name="model">Model data from converted arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Parent EcucBswmd container.</param>
        /// /// <param name="refinedPath"></param>
        public EcucBswmdChoiceRef(IEcucChoiceReferenceDef model, EcucBswmdManager manager)
            : base(model, manager)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class EcucBswmdTypeConvert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmdType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string BswmdTypeToInsatnce(Type bswmdType)
        {
            return bswmdType switch
            {
                IEcucParamConfContainerDef => "ECUC-CONTAINER-VALUE",
                IEcucEnumerationParamDef => "ECUC-TEXTUAL-PARAM-VALUE",
                IEcucIntegerParamDef => "ECUC-NUMERICAL-PARAM-VALUE",
                IEcucBooleanParamDef => "ECUC-NUMERICAL-PARAM-VALUE",
                IEcucFloatParamDef => "ECUC-NUMERICAL-PARAM-VALUE",
                IEcucStringParamDef => "ECUC-TEXTUAL-PARAM-VALUE",
                IEcucFunctionNameDef => "ECUC-TEXTUAL-PARAM-VALUE",
                IEcucReferenceDef => "ECUC-REFERENCE-VALUE",
                IEcucForeignReferenceDef => "ECUC-REFERENCE-VALUE",
                IEcucSymbolicNameReferenceDef => "ECUC-REFERENCE-VALUE",
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmd"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string BswmdToInsatnce(string bswmd)
        {
            return bswmd switch
            {
                "ECUC-PARAM-CONF-CONTAINER-DEF" => "ECUC-CONTAINER-VALUE",
                "ECUC-ENUMERATION-PARAM-DEF" => "ECUC-TEXTUAL-PARAM-VALUE",
                "ECUC-INTEGER-PARAM-DEF" => "ECUC-NUMERICAL-PARAM-VALUE",
                "ECUC-BOOLEAN-PARAM-DEF" => "ECUC-NUMERICAL-PARAM-VALUE",
                "ECUC-FLOAT-PARAM-DEF" => "ECUC-NUMERICAL-PARAM-VALUE",
                "ECUC-STRING-PARAM-DEF" => "ECUC-TEXTUAL-PARAM-VALUE",
                "ECUC-FUNCTION-NAME-DEF" => "ECUC-TEXTUAL-PARAM-VALUE",
                "ECUC-REFERENCE-DEF" => "ECUC-REFERENCE-VALUE",
                "ECUC-FOREIGN-REFERENCE-DEF" => "ECUC-REFERENCE-VALUE",
                "ECUC-SYMBOLIC-NAME-REFERENCE-DEF" => "ECUC-REFERENCE-VALUE",
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmd"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ReferrableType BswmdToBswmdType(string bswmd)
        {
            return bswmd switch
            {
                "ECUC-PARAM-CONF-CONTAINER-DEF" => ReferrableType.tEcucParamConfContainerDef,
                "ECUC-ENUMERATION-PARAM-DEF" => ReferrableType.tEcucEnumerationParamDef,
                "ECUC-INTEGER-PARAM-DEF" => ReferrableType.tEcucIntegerParamDef,
                "ECUC-BOOLEAN-PARAM-DEF" => ReferrableType.tEcucBooleanParamDef,
                "ECUC-FLOAT-PARAM-DEF" => ReferrableType.tEcucFloatParamDef,
                "ECUC-STRING-PARAM-DEF" => ReferrableType.tEcucStringParamDef,
                "ECUC-FUNCTION-NAME-DEF" => ReferrableType.tEcucFunctionNameDef,
                "ECUC-REFERENCE-DEF" => ReferrableType.tEcucReferenceDef,
                "ECUC-FOREIGN-REFERENCE-DEF" => ReferrableType.tEcucForeignReferenceDef,
                "ECUC-SYMBOLIC-NAME-REFERENCE-DEF" => ReferrableType.tEcucSymbolicNameReferenceDef,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
