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
using Ecuc.EcucBase.EBase;
using Ecuc.EcucBase.EBswmd;
using GenTool_CsDataServerDomAsr4.Iface;
using Meta.Iface;
using System.ComponentModel;

namespace Ecuc.EcucBase.EInstance
{
    public class CommentGroup
    {
        public ISdg Model { get; }

        public CommentGroupList Groups
        {
            get
            {
                return new CommentGroupList(Model.SdgContentsType.SdgList);
            }
        }

        public Dictionary<string, string> Data
        {
            get
            {
                var result = new Dictionary<string, string>();

                if (Model.SdgContentsTypeSpecified)
                {
                    foreach (var sd in Model.SdgContentsType.SdList)
                    {
                        result[sd.Gid] = sd.Value;
                    }
                }
                return result;
            }
            set
            {
                Model.SdgContentsTypeSpecified = false;
                Model.SdgContentsTypeSpecified = true;
                foreach (var item in Data)
                {
                    var sd = Model.SdgContentsType.AddNewSd();
                    sd.Gid = item.Key;
                    sd.Value = item.Value;
                }
            }
        }

        public Dictionary<string, ReferrableType> Ref
        {
            get
            {
                var result = new Dictionary<string, ReferrableType>();

                if (Model.SdgContentsTypeSpecified)
                {
                    foreach (var sd in Model.SdgContentsType.SdxList)
                    {
                        result[sd.Value] = sd.DestType;
                    }
                }
                return result;
            }
            set
            {
                Model.SdgContentsTypeSpecified = false;
                foreach (var item in value)
                {
                    var sdx = Model.SdgContentsType.AddNewSdx();

                    sdx.DestType = item.Value;
                    sdx.Value = item.Key;
                }
            }
        }

        public CommentGroup(ISdg model)
        {
            Model = model;
        }

        public CommentGroup this[string name]
        {
            get
            {
                try
                {
                    return Groups[name];
                }
                catch
                {
                    throw new Exception($"Fail to get comment {name}");
                }
            }
            set
            {
                Groups[name] = value;
            }
        }

        public string GetCommentByName(string name)
        {
            try
            {
                return Data[name];
            }
            catch
            {
                throw new Exception($"Can not get comment {name}");
            }
        }

        public void SetCommentByName(string name, string value)
        {
            var sd = Model.SdgContentsType.AddNewSd();
            sd.Gid = name;
            sd.Value = value;
        }
    }

    public class CommentGroupList : List<CommentGroup>
    {
        public ISpecializedMetaCollectionInstance<ISdg> Model { get; }

        public CommentGroupList(ISpecializedMetaCollectionInstance<ISdg> model)
        {
            Model = model;
            foreach (var m in Model)
            {
                Add(new CommentGroup(m));
            }
        }

        public CommentGroup this[string name]
        {
            get
            {
                foreach (var sdg in Model)
                {
                    if (sdg.Gid == name)
                    {
                        return new CommentGroup(sdg);
                    }
                }
                throw new Exception($"Fail to get comment group {name}");
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                foreach (var sdg in Model)
                {
                    if (sdg.Gid == name)
                    {
                        return;
                    }
                }

                Add(value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEcucInstanceBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        string BswmdPath { get; }
        /// <summary>
        /// 
        /// </summary>
        string BswmdPathShort { get; }
        /// <summary>
        /// 
        /// </summary>
        public string DefDest { get; }

        /// <summary>
        /// 
        /// </summary>
        EcucInstanceManager Manager { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        IEcucInstanceBase GetInstanceFromAsrPath(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        List<IEcucInstanceReferenceBase> GetReferenceByAsrPath(string path);

        /// <summary>
        /// 
        /// </summary>
        bool IsDirty { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceBase : NotifyPropertyChangedBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IHasDefinitionRef Model;

        /// <summary>
        /// 
        /// </summary>
        public string BswmdPath
        {
            get
            {
                return Model.DefinitionRefSpecified ? Model.DefinitionRef.Value : "";
            }
            set
            {
                if ((value != null) && (value != BswmdPath))
                {
                    Model.DefinitionRef.Value = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BswmdPathShort
        {
            get
            {
                var parts = BswmdPath.Split('/');
                if (parts.Length > 0)
                {
                    return parts[^1];
                }
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefDest
        {
            get
            {
                if (Model.DefinitionRefSpecified)
                {
                    Model.DefinitionRef.DestType.ToString();
                }
                return "";
            }
            set
            {
                if ((value != null) && (DefDest != value))
                {
                    Model.DefinitionRef.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(DefDest);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EcucInstanceManager Manager { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEcucInstanceBase GetInstanceFromAsrPath(string path)
        {
            return Manager.GetInstanceByAsrPath(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<IEcucInstanceReferenceBase> GetReferenceByAsrPath(string path)
        {
            return Manager.GetReferenceByAsrPath(path);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        public EcucInstanceBase(IHasDefinitionRef model, EcucInstanceManager manager)
        {
            Model = model;
            Manager = manager;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEcucInstanceHasContainer : IEcucInstanceBase
    {
        /// <summary>
        /// 
        /// </summary>
        string ShortName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string AsrPath { get; }
        /// <summary>
        /// 
        /// </summary>
        string AsrPathShort { get; }
        /// <summary>
        /// 
        /// </summary>
        List<IEcucInstanceHasContainer> Containers { get; }
        CommentGroupList Comment { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        EcucInstanceContainer AddContainer(string path, string shortName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        int DelContainer(string path, string shortName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        int DelContainer(string path);
        /// <summary>
        /// 
        /// </summary>
        int DelContainer();
        /// <summary>
        /// 
        /// </summary>
        void DeleteAndRemoveFromOwner();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmdPath"></param>
        /// <returns></returns>
        List<IEcucInstanceHasContainer> FindContainerByBswmd(string bswmdPath)
        {
            if (bswmdPath == "")
            {
                return Containers;
            }
            else
            {
                var query = from container in Containers
                            where container.BswmdPath == bswmdPath
                            select container;

                return query.ToList();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEcucInstanceContainer : IEcucInstanceHasContainer
    {
        /// <summary>
        /// 
        /// </summary>
        List<IEcucInstanceParameterBase> Paras { get; }
        /// <summary>
        /// 
        /// </summary>
        List<IEcucInstanceReferenceBase> Refs { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmdPath"></param>
        /// <returns></returns>
        List<IEcucInstanceParameterBase> FindParaByBswmd(string bswmdPath)
        {
            var query = from para in Paras
                        where para.BswmdPath == bswmdPath
                        select para;

            return query.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmdPath"></param>
        /// <returns></returns>
        List<IEcucInstanceReferenceBase> FindRefByBswmd(string bswmdPath)
        {
            var query = from reference in Refs
                        where reference.BswmdPath == bswmdPath
                        select reference;

            return query.ToList();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEcucInstanceParameterBase : IEcucInstanceBase
    {
        /// <summary>
        /// 
        /// </summary>
        IEcucInstanceContainer Parent { get; }
        /// <summary>
        /// 
        /// </summary>
        string Value { get; set; }
        public List<string> Comment { get; }
        /// <summary>
        /// 
        /// </summary>
        void DeleteAndRemoveFromOwner();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IEcucInstanceReferenceBase : IEcucInstanceBase
    {
        /// <summary>
        /// 
        /// </summary>
        IEcucInstanceContainer Parent { get; }
        /// <summary>
        /// 
        /// </summary>
        string ValueRef { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string ValueRefShort
        {
            get
            {
                var parts = ValueRef.Split('/');
                if (parts.Length > 0)
                {
                    return parts[^1];
                }
                return "";
            }
            set
            {
                var parts = ValueRef.Split('/');
                if (parts.Length > 0)
                {
                    if (parts[^1] != value)
                    {
                        parts[^1] = value;
                        ValueRef = string.Join("/", parts);
                    } 
                }
            }
        }

        public List<string> Comment { get; }
        /// <summary>
        /// 
        /// </summary>
        void DeleteAndRemoveFromOwner();
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceManager
    {
        private readonly ArFile Autosars = new();
        /// <summary>
        /// 
        /// </summary>
        public List<EcucInstanceModule> EcucModules { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        public EcucInstanceManager(string[] fileNames)
        {
            InitMultiple(fileNames);
            var query = from module in EcucModules
                        orderby module.ShortName ascending
                        select module;
            EcucModules = query.ToList();
        }


        private void InitMultiple(string[] fileNames)
        {
            var tasks = new Task[fileNames.Length];

            Autosars.AddFile(fileNames);

            if (Autosars.root != null)
            {
                var query = from package in Autosars.root.ArPackageList
                            from element in package.ElementList
                            where element.IdType == ReferrableType.tEcucModuleConfigurationValues
                            select element as IEcucModuleConfigurationValues;

                query.ToList().ForEach(x =>
                {
                    var module = new EcucInstanceModule(x, this);
                    EcucModules.Add(module);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bswmdPath"></param>
        /// <returns></returns>
        public List<IEcucInstanceBase> GetInstancesByBswmdPath(string bswmdPath)
        {
            var result = new List<IEcucInstanceBase>();

            if (Autosars.root != null)
            {
                foreach (var metaObj in Autosars.root.AllObjects)
                {
                    if (metaObj is IHasDefinitionRef hasDefinition)
                    {
                        if ((hasDefinition.DefinitionRefSpecified) && (hasDefinition.DefinitionRef.Value == bswmdPath))
                        {
                            result.Add(metaObj switch
                            {
                                IEcucModuleConfigurationValues moduleConfiguration => new EcucInstanceModule(moduleConfiguration, this),
                                IEcucContainerValue container => new EcucInstanceContainer(container, this),
                                IEcucTextualParamValue textualParam => new EcucInstanceTextualParamValue(textualParam, this),
                                IEcucNumericalParamValue numericalParam => new EcucInstanceNumericalParamValue(numericalParam, this),
                                IEcucReferenceValue reference => new EcucInstanceReferenceValue(reference, this),
                                _ => throw new Exception($"Invalid type when instance with bswmd path {bswmdPath}")
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEcucInstanceBase GetInstanceByAsrPath(string path)
        {
            if (Autosars.root != null)
            {
                var metaObjs = Autosars.root.Find(path);

                if (metaObjs.Count > 0)
                {
                    foreach (var metaObj in metaObjs)
                    {
                        return metaObj switch
                        {
                            IEcucModuleConfigurationValues moduleConfiguration => new EcucInstanceModule(moduleConfiguration, this),
                            IEcucContainerValue containerValue => new EcucInstanceContainer(containerValue, this),
                            _ => throw new Exception($"Invalid type when find instance path {path}")
                        };
                    }
                }
            }
            throw new Exception($"Can not find instance path {path}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<IEcucInstanceReferenceBase> GetReferenceByAsrPath(string path)
        {
            List<IEcucInstanceReferenceBase> result = new();
            if (Autosars.root != null)
            {
                var metaObjs = Autosars.root.Find(path);
                if (metaObjs.Count > 0)
                {
                    foreach (var metaObj in metaObjs)
                    {
                        var referencedObjs = metaObj.ReferencedFromList;
                        foreach (var referencedObj in referencedObjs)
                        {
                            var owner = referencedObj.Owner;
                            result.Add(owner switch
                            {
                                IEcucReferenceValue reference => new EcucInstanceReferenceValue(reference, this),
                            _ => throw new Exception($"Invalid type when find instance path {path}")
                            });
                        }
                    }
                    return result;
                }
            }
            throw new Exception($"Can not get reference from {path}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EcucInstanceModule? this[string name]
        {
            get
            {
                var query = from module in EcucModules
                            where module.ShortName == name
                            select module;

                var result = query.ToList();
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            Autosars.Save();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceModule : EcucInstanceBase, IEcucInstanceHasContainer
    {
        /// <summary>
        /// 
        /// </summary>
        private IEcucModuleConfigurationValues ModelLocal
        {
            get
            {
                if (Model is IEcucModuleConfigurationValues ecucModuleConfiguration)
                {
                    return ecucModuleConfiguration;
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
        public CommentGroupList Comment
        {
            get
            {
                return new CommentGroupList(ModelLocal.AdminData.SdgList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ShortName
        {
            get
            {
                return ModelLocal.ShortName;
            }
            set
            {
                if ((value != null) && (ShortName != value))
                {
                    ModelLocal.ShortName = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AsrPath
        {
            get
            {
                return ModelLocal.AsrPath;
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public List<IEcucInstanceHasContainer> Containers
        {
            get
            {
                List<IEcucInstanceHasContainer> result = new();

                foreach (var container in ModelLocal.ContainerList)
                {
                    result.Add(new EcucInstanceContainer(container, Manager));
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <param name="parent"></param>
        public EcucInstanceModule(IEcucModuleConfigurationValues model, EcucInstanceManager manager)
            :base(model, manager)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public EcucInstanceContainer AddContainer(string path, string shortName)
        {
            var container = ModelLocal.AddNewContainer();
            container.Definition.DestType = ReferrableType.tEcucParamConfContainerDef;
            container.Definition.Value = $"{BswmdPath}/{path}";
            container.ShortName = shortName;
            container.Uuid = Guid.NewGuid().ToString();
            //else
            //{
            //    throw new Exception($"Find duplicate container {shortName} when add continer");
            //}
            return new EcucInstanceContainer(container, Manager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public int DelContainer(string path, string shortName)
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if ((container.BswmdPath == path) && (container.ShortName == shortName))
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int DelContainer(string path)
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if (container.BswmdPathShort == path)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DelContainer()
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if (container is EcucInstanceContainer instanceContainer)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAndRemoveFromOwner()
        {
            ModelLocal.DeleteAndRemoveFromOwner();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceContainer : EcucInstanceBase, IEcucInstanceContainer
    {
        /// <summary>
        /// 
        /// </summary>
        public IEcucContainerValue ModelLocal
        {
            get
            {
                if (Model is IEcucContainerValue ecucContainer)
                {
                    return ecucContainer;
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
            }
        }

        public CommentGroupList Comment
        {
            get
            {
                return new CommentGroupList(ModelLocal.AdminData.SdgList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ShortName
        {
            get
            {
                return ModelLocal.ShortName;
            }
            set
            {
                if ((value != null) && (ShortName != value))
                {
                    ModelLocal.ShortName = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AsrPath
        {
            get
            {
                return ModelLocal.AsrPath;
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public List<IEcucInstanceHasContainer> Containers
        {
            get
            {
                List<IEcucInstanceHasContainer> result = new();

                foreach (var container in ModelLocal.SubContainerList)
                {
                    result.Add(new EcucInstanceContainer(container, Manager));
                }
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<IEcucInstanceParameterBase> Paras
        {
            get
            {
                List<IEcucInstanceParameterBase> result = new();

                foreach (var para in ModelLocal.ParameterValueList)
                {
                    result.Add(para switch
                    {
                        IEcucTextualParamValue textualParamValue => new EcucInstanceTextualParamValue(textualParamValue, Manager),
                        IEcucNumericalParamValue numericalParamValue => new EcucInstanceNumericalParamValue(numericalParamValue, Manager),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<IEcucInstanceReferenceBase> Refs
        {
            get
            {
                List<IEcucInstanceReferenceBase> result = new();

                foreach (var reference in ModelLocal.ReferenceValueList)
                {
                    result.Add(reference switch
                    {
                        IEcucReferenceValue referenceValue => new EcucInstanceReferenceValue(referenceValue, Manager),
                        _ => throw new Exception("Invalid model type")
                    });
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <param name="parent"></param>
        public EcucInstanceContainer(IEcucContainerValue model, EcucInstanceManager manager)
            :base(model, manager)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public EcucInstanceContainer AddContainer(string path, string shortName)
        {
            var container = ModelLocal.AddNewSubContainer();
            container.Definition.DestType = ReferrableType.tEcucParamConfContainerDef;
            container.Definition.Value = $"{BswmdPath}/{path}";
            container.ShortName = shortName;
            container.Uuid = Guid.NewGuid().ToString();
            return new EcucInstanceContainer(container, Manager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public int DelContainer(string path, string shortName)
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if (container.BswmdPath == path && container.ShortName == shortName)
                {
                    container.DeleteAndRemoveFromOwner();
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int DelContainer(string path)
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if (container.BswmdPathShort == path)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        container.DeleteAndRemoveFromOwner();
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DelContainer()
        {
            int count = 0;
            foreach (var container in Containers)
            {
                if (container is EcucInstanceContainer instanceContainer)
                {
                    container.DeleteAndRemoveFromOwner();
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public EcucInstanceTextualParamValue AddTextualPara(string path, string value, string dest)
        {
            var textualParamValue = ModelLocal.AddNewParameterValueAsEcucTextualParamValue();
            textualParamValue.DefinitionRef.Value = $"{BswmdPath}/{path}";
            textualParamValue.DefinitionRef.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(dest);
            textualParamValue.Value = value ;
            var paraNew = new EcucInstanceTextualParamValue(textualParamValue, Manager);
            return paraNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void DelTextualPara(string path, string value)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path && para.Value == value
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceTextualParamValue textualPara)
                {
                    para.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void DelTextualPara(string path)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceTextualParamValue textualPara)
                {
                    para.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public EcucInstanceNumericalParamValue AddNumericalPara(string path, string value, string dest)
        {
            var numericalParamValue = ModelLocal.AddNewParameterValueAsEcucNumericalParamValue();
            numericalParamValue.DefinitionRef.Value = $"{BswmdPath}/{path}";
            numericalParamValue.DefinitionRef.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(dest);
            numericalParamValue.Value.Value = value;
            var paraNew = new EcucInstanceNumericalParamValue(numericalParamValue, Manager);
            return paraNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void DelNumericalPara(string path, string value)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path && para.Value == value
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceNumericalParamValue numericalPara)
                {
                    para.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void DelNumericalPara(string path)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceNumericalParamValue numericalPara)
                {
                    para.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defDest"></param>
        /// <param name="valueRef"></param>
        /// <param name="valueDest"></param>
        /// <returns></returns>
        public EcucInstanceReferenceValue AddReference(string path, ReferrableType defDest, string valueRef, ReferrableType valueDest)
        {
            var referenceValue = ModelLocal.AddNewReferenceValueAsEcucReferenceValue();
            referenceValue.DefinitionRef.Value = $"{BswmdPath}/{path}";
            referenceValue.DefinitionRef.DestType = defDest;
            referenceValue.Value.DestType = valueDest;
            referenceValue.Value.Value = valueRef;
            var referenceNew = new EcucInstanceReferenceValue(referenceValue, Manager);
            return referenceNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="valueRef"></param>
        public void DelReference(string path, string valueRef)
        {
            var query = from reference in Refs
                        where reference.BswmdPathShort == path && reference.ValueRef == valueRef
                        select reference;

            foreach (var reference in query.ToList())
            {
                if (reference is EcucInstanceReferenceValue instanceReference)
                {
                    reference.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void DelReference(string path)
        {
            var query = from reference in Refs
                        where reference.BswmdPathShort == path
                        select reference;

            foreach (var reference in query.ToList())
            {
                if (reference is EcucInstanceReferenceValue instanceReference)
                {
                    reference.DeleteAndRemoveFromOwner();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAndRemoveFromOwner()
        {
            ModelLocal.DeleteAndRemoveFromOwner();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceTextualParamValue : EcucInstanceBase, IEcucInstanceParameterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IEcucInstanceContainer Parent
        {
            get
            {
                if (ModelLocal.Parent is IEcucContainerValue container)
                {
                    return new EcucInstanceContainer(container, Manager);
                }
                throw new Exception($"Parent is not EcucContainerValue");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEcucTextualParamValue ModelLocal
        {
            get
            {
                if (Model is IEcucTextualParamValue ecucTextualParam)
                {
                    return ecucTextualParam;
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
        public string Value
        {
            get
            {
                if (ModelLocal.ValueSpecified)
                {
                    return ModelLocal.Value;
                }
                return "";
            }
            set
            {
                if ((value != null) && (Value != value))
                {
                    ModelLocal.Value = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Dest
        {
            get
            {
                if (ModelLocal.DefinitionRefSpecified)
                {
                    return ModelLocal.DefinitionRef.DestType.ToString();
                }
                return "";
            }
            set
            {
                if ((value != null) && (Dest != value))
                {
                    ModelLocal.DefinitionRef.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in ModelLocal.AnnotationList)
                {
                    if (anno.AnnotationOriginSpecified)
                    {
                        result.Add(anno.AnnotationOrigin);
                    }
                }
                return result;
            }
            set
            {
                var annos = ModelLocal.AnnotationList.ToArray();

                foreach (var anno in annos)
                {
                    ModelLocal.RemoveAnnotation(anno);
                }

                foreach (var a in Comment)
                {
                    var newAnno = ModelLocal.AddNewAnnotation();
                    newAnno.AnnotationOriginSpecified = true;
                    newAnno.AnnotationOrigin = a;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <param name="parent"></param>
        public EcucInstanceTextualParamValue(IEcucTextualParamValue model, EcucInstanceManager manager)
            :base(model, manager)
        {
            Model = model;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAndRemoveFromOwner()
        {
            ModelLocal.DeleteAndRemoveFromOwner();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceNumericalParamValue : EcucInstanceBase, IEcucInstanceParameterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IEcucInstanceContainer Parent
        {
            get
            {
                if (ModelLocal.Parent is IEcucContainerValue container)
                {
                    return new EcucInstanceContainer(container, Manager);
                }
                throw new Exception($"Parent is not EcucContainerValue");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEcucNumericalParamValue ModelLocal
        {
            get
            {
                if (Model is IEcucNumericalParamValue ecucNumericalParam)
                {
                    return ecucNumericalParam;
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
        public string Value
        {
            get
            {
                if (ModelLocal.ValueSpecified)
                {
                    return ModelLocal.Value.Value;
                }
                return "";
            }
            set
            {
                if ((value != null) && (Value == value))
                {
                    ModelLocal.Value.Value = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Dest
        {
            get
            {
                if (Model.DefinitionRefSpecified)
                {
                    return Model.DefinitionRef.DestType.ToString();
                }
                return "";
            }
            set
            {
                if ((value != null) && (Dest != value))
                {
                    Model.DefinitionRef.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in ModelLocal.AnnotationList)
                {
                    if (anno.AnnotationOriginSpecified)
                    {
                        result.Add(anno.AnnotationOrigin);
                    }
                }
                return result;
            }
            set
            {
                var annos = ModelLocal.AnnotationList.ToArray();

                foreach (var anno in annos)
                {
                    ModelLocal.RemoveAnnotation(anno);
                }

                foreach (var a in Comment)
                {
                    var newAnno = ModelLocal.AddNewAnnotation();
                    newAnno.AnnotationOriginSpecified = true;
                    newAnno.AnnotationOrigin = a;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <param name="parent"></param>
        public EcucInstanceNumericalParamValue(IEcucNumericalParamValue model, EcucInstanceManager manager)
            :base(model, manager)
        {
            Model = model;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAndRemoveFromOwner()
        {
            ModelLocal.DeleteAndRemoveFromOwner();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EcucInstanceReferenceValue : EcucInstanceBase, IEcucInstanceReferenceBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IEcucInstanceContainer Parent
        {
            get
            {
                if (ModelLocal.Parent is IEcucContainerValue container)
                {
                    return new EcucInstanceContainer(container, Manager);
                }
                throw new Exception($"Parent is not EcucContainerValue");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEcucReferenceValue ModelLocal
        {
            get
            {
                if (Model is IEcucReferenceValue ecucReference)
                {
                    return ecucReference;
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
        public string ValueRef
        {
            get
            {
                if (ModelLocal.ValueSpecified)
                {
                    return ModelLocal.Value.Value;
                }
                return "";
            }
            set
            {
                if ((value != null) && (ValueRef != value))
                {
                    ModelLocal.Value.Value = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValueDest
        {
            get
            {
                if (ModelLocal.ValueSpecified)
                {
                    ModelLocal.Value.DestType.ToString();
                }
                return "";
            }
            set
            {
                if ((value != null) && (DefDest != value))
                {
                    ModelLocal.Value.DestType = EcucBswmdTypeConvert.BswmdToBswmdType(value); ;
                }
            }
        }

        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in ModelLocal.AnnotationList)
                {
                    if (anno.AnnotationOriginSpecified)
                    {
                        result.Add(anno.AnnotationOrigin);
                    }
                }
                return result;
            }
            set
            {
                var annos = ModelLocal.AnnotationList.ToArray();

                foreach (var anno in annos)
                {
                    ModelLocal.RemoveAnnotation(anno);
                }

                foreach (var a in Comment)
                {
                    var newAnno = ModelLocal.AddNewAnnotation();
                    newAnno.AnnotationOriginSpecified = true;
                    newAnno.AnnotationOrigin = a;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <param name="parent"></param>
        public EcucInstanceReferenceValue(IEcucReferenceValue model, EcucInstanceManager manager)
            :base(model, manager)
        {
            Model = model;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteAndRemoveFromOwner()
        {
            ModelLocal.DeleteAndRemoveFromOwner();
        }
    }
}
