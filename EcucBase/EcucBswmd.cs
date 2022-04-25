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
using Autosar;
using System.Numerics;

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
        IArEcucBswmdBase Model { get; }
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
        /// Autosar path of refined module from standard.
        /// </summary>
        string RefinedPath { get; }
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

    public class EcucBswmdBase : IEcucBswmdBase
    {
        /// <summary>
        /// Model of EcucBswmd
        /// </summary>
        public IArEcucBswmdBase Model { get; protected set; }
        /// <summary>
        /// Root data model of EcucBswmd.
        /// </summary>
        public EcucBswmdManager Manager { get; protected set; }

        /// <summary>
        /// ShortName of EcucBswmd module.
        /// </summary>
        public string ShortName => Model.SHORTNAME.TypedValue;

        /// <summary>
        /// Description of EcucBswmd module.
        /// </summary>
        public string Desc
        {
            get
            {
                var result = "";

                try
                {
                    foreach (var d in Model.DESC.L2)
                    {
                        result += d.Untyped.Value;
                    }
                    foreach (var p in Model.INTRODUCTION.P)
                    {
                        result += $"{Environment.NewLine}{p.Untyped.Value}";
                    }
                }
                catch
                {
                    result = "";
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

                try
                {
                    foreach (var t in Model.INTRODUCTION.TRACE)
                    {
                        foreach (var l4 in t.LONGNAME.L4)
                        {
                            result += $"{l4.Untyped.Value} ";
                        }
                    }
                }
                catch
                {
                    result = "";
                }

                return result;
            }
        }

        /// <summary>
        /// Smallest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        public uint Lower => uint.Parse(Model.LOWERMULTIPLICITY.Untyped.Value);

        /// <summary>
        /// Biggest quantity of EcucBswmd can exist in Bswmd module.
        /// </summary>
        public uint Upper
        {
            get
            {
                if (Model.UPPERMULTIPLICITYINFINITE != null)
                {
                    return uint.MaxValue;
                }
                else
                {
                    try
                    {
                        return uint.Parse(Model.UPPERMULTIPLICITY.Untyped.Value);
                    }
                    catch
                    {
                        return uint.MaxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Autosar path of refined module from standard.
        /// </summary>
        public string RefinedPath { get; protected set; }

        /// <summary>
        /// Autosar path of Bswmd module.
        /// </summary>
        public string AsrPath
        {
            get
            {
                try
                {
                    return Manager.AsrRawAsrPathDict[Model];
                }
                catch
                {
                    throw new Exception($"Can not find Autosar model {Model}");
                }
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
                    return Manager.AsrPathBswmdDict[bswmdPath];
                }
                catch
                {
                    throw new Exception($"Can not find bswmd with path {bswmdPath}");
                }
            }
        }

        public EcucBswmdBase(IArEcucBswmdBase model, EcucBswmdManager manager)
        {
            Model = model;
            Manager = manager;
            RefinedPath = "";
        }
    }

    /// <summary>
    /// Bswmd module interface
    /// Base interface of all kind of Bswmd class with Containers member.
    /// </summary>
    public interface IEcucBswmdModule : IEcucBswmdBase
    {
        /// <summary>
        /// Containers in EcucBswmd.
        /// </summary>
        List<IEcucBswmdModule> Containers { get; }
    }

    /// <summary>
    /// Bswmd container interface
    /// Base interface of all kind of Bswmd class with Containers, Parameters and References member.
    /// </summary>
    public interface IEcucBswmdContainer : IEcucBswmdModule
    {
        /// <summary>
        /// Parent EcucBswmd.
        /// Parent shall be EcucBswmd with Containers member.
        /// </summary>
        IEcucBswmdModule Parent { get; }
        /// <summary>
        /// Parameters in EcucBswmd.
        /// </summary>
        List<IEcucBswmdParam> Paras { get; }
        /// <summary>
        /// References in EcucBswmd.
        /// </summary>
        List<IEcucBswmdReference> Refs { get; }
    }

    /// <summary>
    /// Bswmd parameter interface
    /// Base interface of all kind of Bswmd parameter in Bswmd container.
    /// </summary>
    public interface IEcucBswmdParam : IEcucBswmdBase
    {
        /// <summary>
        /// Parent EcucBswmd.
        /// Parent shall be EcucBswmd with Containers member.
        /// </summary>
        public IEcucBswmdModule Parent { get; }
    }

    /// <summary>
    /// Bswmd reference interface
    /// Base interface of all kind of Bswmd reference in Bswmd container.
    /// </summary>
    public interface IEcucBswmdReference : IEcucBswmdBase
    {
        /// <summary>
        /// Parent EcucBswmd.
        /// Parent shall be EcucBswmd with Containers member.
        /// </summary>
        public IEcucBswmdModule Parent { get; }
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
        private readonly List<Asr> Autosars = new();
        /// <summary>
        /// EcucBswmd moudles converted from several arxml files.
        /// </summary>
        public List<EcucBswmdModule> EcucModules { get; } = new();
        /// <summary>
        /// Data model to Autosar path dictionary.
        /// </summary>
        public Dictionary<object, string> AsrRawAsrPathDict = new();
        /// <summary>
        /// Autosar path to EcucBswmd dictionary.
        /// </summary>
        public Dictionary<string, IEcucBswmdBase> AsrPathBswmdDict = new();

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
            // create one task for each arxml file.
            var tasks1 = new Task[fileNames.Length];
            // step 1 to convert arxml file to Asr
            for (int i = 0; i < fileNames.Length; i++)
            {
                tasks1[i] = InitMultipleSingleStep1(fileNames[i]);
            }
            Task.WaitAll(tasks1);

            // create one task for each Asr data model. 
            var tasks2 = new Task[Autosars.Count];
            // step 2 to collect all Autosar path.
            for (int i = 0; i < Autosars.Count; i++)
            {
                tasks2[i] = InitMultipleSingleStep2(Autosars[i]);
            }
            Task.WaitAll(tasks2);

            // create one task for each Asr data model.
            var tasks3 = new Task[Autosars.Count];
            // step 3 to extract EcucBswmd module.
            for (int i = 0; i < Autosars.Count; i++)
            {
                tasks3[i] = InitMultipleSingleStep3(Autosars[i]);
            }
            Task.WaitAll(tasks3);
        }

        /// <summary>
        /// Step 1 to convert arxml file to Asr
        /// </summary>
        /// <param name="fileName">Arxml file name</param>
        /// <returns></returns>
        private Task InitMultipleSingleStep1(string fileName)
        {
            return Task.Run(() =>
            {
                var autosar = new Asr(fileName);

                Monitor.Enter(Autosars);
                try
                {
                    // collect all Asr data model.
                    Autosars.Add(autosar);
                }
                finally
                {
                    Monitor.Exit(Autosars);
                }

                List<ARPACKAGE> arPackages = new();
                foreach (var package in autosar.ArPackages)
                {
                    // collect all ArPackages.
                    arPackages.Add(package);
                }
            });
        }

        /// <summary>
        /// Step 2 to collect all Autosar path.
        /// </summary>
        /// <param name="ar">Asr data model</param>
        /// <returns></returns>
        private Task InitMultipleSingleStep2(Asr ar)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(AsrRawAsrPathDict);
                try
                {
                    // exchange value and key 
                    foreach (var value in ar.AsrPathModelDict)
                    {
                        AsrRawAsrPathDict[value.Value] = value.Key;
                    }
                }
                finally
                {
                    Monitor.Exit(AsrRawAsrPathDict);
                }
            });
        }

        /// <summary>
        /// Step 3 to extract EcucBswmd module.
        /// </summary>
        /// <param name="ar">Asr data model</param>
        /// <returns></returns>
        private Task InitMultipleSingleStep3(Asr ar)
        {
            return Task.Run(() =>
            {
                // filter arPackages which has ECUCMODULEDEF member.
                var query = from package in ar.ArPackages
                            where package.ELEMENTS?.ECUCMODULEDEF != null
                            from ecucModule in package.ELEMENTS.ECUCMODULEDEF
                            select ecucModule;

                Monitor.Enter(EcucModules);
                try
                {
                    // convert EcucBswmd module from tag ECUCMODULEDEF
                    query.ToList().ForEach(x => EcucModules.Add(new EcucBswmdModule(x, this, ar)));
                }
                finally
                {
                    Monitor.Exit(EcucModules);
                }
            });
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
                var query = from module in EcucModules
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
            try
            {
                return AsrPathBswmdDict[bswmdPath];
            }
            catch
            {
                throw new Exception($"Can not find bswmd path {bswmdPath}");
            }
        }
    }

    /// <summary>
    /// EcucBswmd module class.
    /// EcucBswmd module class represent module of ECUCMODULEDEF tag.
    /// </summary>
    public class EcucBswmdModule : EcucBswmdBase, IEcucBswmdModule
    {
        /// <summary>
        /// Containers of EcucBswmd module.
        /// </summary>
        public List<IEcucBswmdModule> Containers { get; } = new();
        /// <summary>
        /// Parent Autosar data model.
        /// </summary>
        public Asr Parent { get; }

        /// <summary>
        /// Local unpacked model
        /// </summary>
        public ECUCMODULEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCMODULEDEF modelLocal)
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
        /// Initialize EcucBswmd module class.
        /// </summary>
        /// <param name="model">Data model converted from arxml file.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Autosar data model which contain this EcucBswmd module.</param>
        public EcucBswmdModule(ECUCMODULEDEF model, EcucBswmdManager manager, Asr parent)
            : base(model, manager)
        {
            Parent = parent;
            if (ModelLocal.REFINEDMODULEDEFREF != null)
            {
                RefinedPath = ModelLocal.REFINEDMODULEDEFREF.Untyped.Value;
            }
            else
            {
                RefinedPath = "";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (RefinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Collect all containers
            if (ModelLocal.CONTAINERS != null)
            {
                if (ModelLocal.CONTAINERS.ECUCPARAMCONFCONTAINERDEF != null)
                {
                    // parameter container case 
                    foreach (var container in ModelLocal.CONTAINERS.ECUCPARAMCONFCONTAINERDEF)
                    {
                        Containers.Add(new EcucBswmdContainer(container, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.CONTAINERS.ECUCCHOICECONTAINERDEF != null)
                {
                    // choice container case 
                    foreach (var container in ModelLocal.CONTAINERS.ECUCCHOICECONTAINERDEF)
                    {
                        Containers.Add(new EcucBswmdChoiceContainer(container, Manager, this, RefinedPath));
                    }
                }
            }
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
        public List<IEcucBswmdModule> Containers { get; } = new();
        /// <summary>
        /// Parameters of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdParam> Paras { get; } = new();
        /// <summary>
        /// References of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdReference> Refs { get; } = new();
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Local unpacked model
        /// </summary>
        public ECUCPARAMCONFCONTAINERDEF ModelLocal
        {
            get
            {
                if (Model is ECUCPARAMCONFCONTAINERDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd module or container</param>
        public EcucBswmdContainer(ECUCPARAMCONFCONTAINERDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (RefinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse EcucBswmd container
            if (ModelLocal.SUBCONTAINERS != null)
            {
                if (ModelLocal.SUBCONTAINERS != null)
                {
                    if (ModelLocal.SUBCONTAINERS.ECUCPARAMCONFCONTAINERDEF != null)
                    {
                        foreach (var container in ModelLocal.SUBCONTAINERS.ECUCPARAMCONFCONTAINERDEF)
                        {
                            Containers.Add(new EcucBswmdContainer(container, Manager, this, RefinedPath));
                        }
                    }

                    if (ModelLocal.SUBCONTAINERS.ECUCCHOICECONTAINERDEF != null)
                    {
                        foreach (var container in ModelLocal.SUBCONTAINERS.ECUCCHOICECONTAINERDEF)
                        {
                            Containers.Add(new EcucBswmdChoiceContainer(container, Manager, this, RefinedPath));
                        }
                    }
                }
            }

            // Parse EcucBswmd parameter
            if (ModelLocal.PARAMETERS != null)
            {
                if (ModelLocal.PARAMETERS.ECUCENUMERATIONPARAMDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCENUMERATIONPARAMDEF)
                    {
                        Paras.Add(new EcucBswmdEnumerationPara(para, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.PARAMETERS.ECUCINTEGERPARAMDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCINTEGERPARAMDEF)
                    {
                        Paras.Add(new EcucBswmdIntegerPara(para, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.PARAMETERS.ECUCBOOLEANPARAMDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCBOOLEANPARAMDEF)
                    {
                        Paras.Add(new EcucBswmdBooleanPara(para, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.PARAMETERS.ECUCFLOATPARAMDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCFLOATPARAMDEF)
                    {
                        Paras.Add(new EcucBswmdFloatPara(para, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.PARAMETERS.ECUCSTRINGPARAMDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCSTRINGPARAMDEF)
                    {
                        Paras.Add(new EcucBswmdStringPara(para, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.PARAMETERS.ECUCFUNCTIONNAMEDEF != null)
                {
                    foreach (var para in ModelLocal.PARAMETERS.ECUCFUNCTIONNAMEDEF)
                    {
                        Paras.Add(new EcucBswmdFunctionNamePara(para, Manager, this, RefinedPath));
                    }
                }
            }

            // Parse EcucBswmd reference
            if (ModelLocal.REFERENCES != null)
            {
                if (ModelLocal.REFERENCES.ECUCREFERENCEDEF != null)
                {
                    foreach (var reference in ModelLocal.REFERENCES.ECUCREFERENCEDEF)
                    {
                        Refs.Add(new EcucBswmdRef(reference, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.REFERENCES.ECUCFOREIGNREFERENCEDEF != null)
                {
                    foreach (var reference in ModelLocal.REFERENCES.ECUCFOREIGNREFERENCEDEF)
                    {
                        Refs.Add(new EcucBswmdForeignRef(reference, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.REFERENCES.ECUCSYMBOLICNAMEREFERENCEDEF != null)
                {
                    foreach (var reference in ModelLocal.REFERENCES.ECUCSYMBOLICNAMEREFERENCEDEF)
                    {
                        Refs.Add(new EcucBswmdSymbolicNameRef(reference, Manager, this, RefinedPath));
                    }
                }

                if (ModelLocal.REFERENCES.ECUCCHOICEREFERENCEDEF != null)
                {
                    foreach (var reference in ModelLocal.REFERENCES.ECUCCHOICEREFERENCEDEF)
                    {
                        Refs.Add(new EcucBswmdChoiceRef(reference, Manager, this, RefinedPath));
                    }
                }
            }
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
        public List<IEcucBswmdModule> Containers { get; } = new();
        /// <summary>
        /// Parameters of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdParam> Paras { get; } = new();
        /// <summary>
        /// References of EcucBswmd container.
        /// </summary>
        public List<IEcucBswmdReference> Refs { get; } = new();
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCCHOICECONTAINERDEF ModelLocal
        {
            get
            {
                if (Model is ECUCCHOICECONTAINERDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd module or container.</param>
        public EcucBswmdChoiceContainer(ECUCCHOICECONTAINERDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse children of EcucBswmd container
            foreach (var choice in ModelLocal.CHOICES.ECUCPARAMCONFCONTAINERDEF)
            {
                if (refinedPath == "")
                {
                    Containers.Add(new EcucBswmdContainer(choice, Manager, this, refinedPath));
                }
                else
                {
                    Containers.Add(new EcucBswmdContainer(choice, Manager, this, $"{refinedPath}/{AsrPathShort}"));
                }
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
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCENUMERATIONPARAMDEF ModelLocal
        {
            get
            {
                if (Model is ECUCENUMERATIONPARAMDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdEnumerationPara(ECUCENUMERATIONPARAMDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse all candidate values
            foreach (var literal in ModelLocal.LITERALS.ECUCENUMERATIONLITERALDEF)
            {
                Candidate.Add(literal.SHORTNAME.TypedValue);
            }

            // Parse default value
            var value = ModelLocal.DEFAULTVALUE;
            if (value != null)
            {
                Default = value.Untyped.Value;
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
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCINTEGERPARAMDEF ModelLocal
        {
            get
            {
                if (Model is ECUCINTEGERPARAMDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdIntegerPara(ECUCINTEGERPARAMDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse mininum value
            try
            {
                Min = BigInteger.Parse(ModelLocal.MIN.Untyped.Value);
            }
            catch
            {
                Min = 0;
            }

            // Parse maximun value if exist
            if (ModelLocal.MAX == null)
            {
                Max = null;
            }
            else if (ModelLocal.MAX.Untyped.Value == "*")
            {
                Max = null;
            }
            else
            {
                Max = BigInteger.Parse(ModelLocal.MAX.Untyped.Value);
            }

            // Parse default value
            var value = ModelLocal.DEFAULTVALUE;
            if (value != null)
            {
                if (value.Untyped.Value == "")
                {
                    Default = Min;
                }
                else
                {
                    Default = BigInteger.Parse(value.Untyped.Value);
                }
            }
            else
            {
                Default = Min;
            }

            // Parse format
            var format = ModelLocal.ADMINDATA;
            if (format != null)
            {
                var formats = format.SDGS.SDG;
                foreach (var f in formats)
                {
                    if (f.GID == "DV:Display")
                    {
                        var formats2 = f.SD;
                        foreach (var f2 in formats2)
                        {
                            if (f2.GID == "DV:DefaultFormat")
                            {
                                Format = f2.Untyped.Value;
                            }
                        }
                    }
                }
            }

            // Check format at last
            if (Format == null)
            {
                Format = "";
            }
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
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCBOOLEANPARAMDEF ModelLocal
        {
            get
            {
                if (Model is ECUCBOOLEANPARAMDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdBooleanPara(ECUCBOOLEANPARAMDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse default value
            var value = ModelLocal.DEFAULTVALUE;
            try
            {
                if (value.Untyped.Value == "1")
                {
                    Default = true;
                }
                else if (value.Untyped.Value == "0")
                {
                    Default = false;
                }
                else
                {
                    Default = bool.Parse(value.Untyped.Value);
                }
            }
            catch
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
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCFLOATPARAMDEF ModelLocal
        {
            get
            {
                if (Model is ECUCFLOATPARAMDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdFloatPara(ECUCFLOATPARAMDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse default value
            var value = ModelLocal.DEFAULTVALUE;
            try
            {
                Default = double.Parse(value.Untyped.Value);
            }
            catch
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
        public List<string> Default { get; } = new();
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCSTRINGPARAMDEF ModelLocal
        {
            get
            {
                if (Model is ECUCSTRINGPARAMDEF modelLocal)
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
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdStringPara(ECUCSTRINGPARAMDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parse default strings
            if (ModelLocal.ECUCSTRINGPARAMDEFVARIANTS != null)
            {
                foreach (var variant in ModelLocal.ECUCSTRINGPARAMDEFVARIANTS.ECUCSTRINGPARAMDEFCONDITIONAL)
                {
                    if (variant.DEFAULTVALUE != null)
                    {
                        Default.Add(variant.DEFAULTVALUE.Untyped.Value);
                    }
                }
            }
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
        public List<string> Default { get; } = new();
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCFUNCTIONNAMEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCFUNCTIONNAMEDEF modelLocal)
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
        /// <param name="parent">Parent of EcucBswmd container.</param>
        public EcucBswmdFunctionNamePara(ECUCFUNCTIONNAMEDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }

            // Parese default values.
            if (ModelLocal.ECUCFUNCTIONNAMEDEFVARIANTS != null)
            {
                foreach (var variant in ModelLocal.ECUCFUNCTIONNAMEDEFVARIANTS.ECUCFUNCTIONNAMEDEFCONDITIONAL)
                {
                    if (variant.DEFAULTVALUE != null)
                    {
                        Default.Add(variant.DEFAULTVALUE.Untyped.Value);
                    }
                }
            }

            if (Default.Count == 0)
            {
                Default.Add("");
            }
        }
    }

    /// <summary>
    /// EcucBsemd reference.
    /// </summary>
    public class EcucBswmdRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCREFERENCEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCREFERENCEDEF modelLocal)
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
                return ModelLocal.DESTINATIONREF.TypedValue;
            }
        }

        /// <summary>
        /// Reference desitination tag.
        /// </summary>
        public string Dest
        {
            get
            {
                return ModelLocal.DESTINATIONREF.DEST;
            }
        }

        /// <summary>
        /// Initialize EcucBswmd reference.
        /// </summary>
        /// <param name="model">Data model converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdRef(ECUCREFERENCEDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }
        }
    }

    /// <summary>
    /// EcucBswmd reference from tag other than Ecuc tags.
    /// </summary>
    public class EcucBswmdForeignRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCFOREIGNREFERENCEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCFOREIGNREFERENCEDEF modelLocal)
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
                return ModelLocal.DESTINATIONTYPE.TypedValue;
            }
        }

        /// <summary>
        /// Initialize 
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdForeignRef(ECUCFOREIGNREFERENCEDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }
        }
    }

    /// <summary>
    /// EcucBswmd reference of symbolic name.
    /// </summary>
    public class EcucBswmdSymbolicNameRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCSYMBOLICNAMEREFERENCEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCSYMBOLICNAMEREFERENCEDEF modelLocal)
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
        public string Dest
        {
            get
            {
                return ModelLocal.DESTINATIONREF.DEST;
            }
        }

        /// <summary>
        /// Reference desitination desitination tag.
        /// </summary>
        public string DestPath
        {
            get
            {
                return ModelLocal.DESTINATIONREF.TypedValue;
            }
        }

        /// <summary>
        /// Initialize EcucBswmd symbolic name.
        /// </summary>
        /// <param name="model">Model data converted from arxml.</param>
        /// <param name="manager">Manager of this class.</param>
        /// <param name="parent">Parent EcucBswmd container.</param>
        public EcucBswmdSymbolicNameRef(ECUCSYMBOLICNAMEREFERENCEDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }
        }
    }

    /// <summary>
    /// EcucBswmd reference of choice reference.
    /// </summary>
    public class EcucBswmdChoiceRef : EcucBswmdBase, IEcucBswmdReference
    {
        /// <summary>
        /// Parent EcucBswmd module.
        /// </summary>
        public IEcucBswmdModule Parent { get; }

        /// <summary>
        /// Unpacked EcucBswmd choice container.
        /// </summary>
        public ECUCCHOICEREFERENCEDEF ModelLocal
        {
            get
            {
                if (Model is ECUCCHOICEREFERENCEDEF modelLocal)
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
        public Dictionary<string, string> Choices
        {
            get
            {
                var result = new Dictionary<string, string>();
                foreach (var destRef in ModelLocal.DESTINATIONREFS.DESTINATIONREF)
                {
                    result[destRef.TypedValue] = destRef.DEST;
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
        public EcucBswmdChoiceRef(ECUCCHOICEREFERENCEDEF model, EcucBswmdManager manager, IEcucBswmdModule parent, string refinedPath)
            : base(model, manager)
        {
            Parent = parent;
            if (refinedPath == "")
            {
                RefinedPath = "";
            }
            else
            {
                RefinedPath = $"{refinedPath}/{AsrPathShort}";
            }

            // Register this to manager.
            Monitor.Enter(Manager.AsrPathBswmdDict);
            try
            {
                Manager.AsrPathBswmdDict[AsrPath] = this;
                if (refinedPath != "")
                {
                    Manager.AsrPathBswmdDict[RefinedPath] = this;
                }
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathBswmdDict);
            }
        }
    }
}
