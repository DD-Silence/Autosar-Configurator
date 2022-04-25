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
using System.ComponentModel;

namespace Ecuc.EcucBase.EInstance
{
    public class CommentGroup
    {
        public SDG Model { get; }

        public string Name
        {
            get
            {
                return Model.Untyped.Value;
            }
        }

        public CommentGroupList Groups
        {
            get
            {
                return new CommentGroupList(Model.SDG1);
            }
        }

        public Dictionary<string, string> Data
        {
            get
            {
                var result = new Dictionary<string, string>();

                foreach (var sd in Model.SD)
                {
                    result[sd.GID] = sd.Untyped.Value;
                }
                return result;
            }
        }

        public Dictionary<string, string> Ref
        {
            get
            {
                var result = new Dictionary<string, string>();

                foreach (var sd in Model.SDXREF)
                {
                    result[sd.Untyped.Value] = sd.DEST;
                }
                return result;
            }
        }

        public CommentGroup(SDG model)
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
            var sdNew = new SD
            {
                GID = name
            };
            sdNew.Untyped.Value = value;
            Model.SD.Add(sdNew);
        }
    }

    public class CommentGroupList : List<CommentGroup>
    {
        public IList<SDG> Model { get; }

        public CommentGroupList()
        {
            Model = new List<SDG>();
        }

        public CommentGroupList(IList<SDG> model)
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
                    if (sdg.GID == name)
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
                    if (sdg.GID == name)
                    {
                        return;
                    }
                }

                Add(value);
            }
        }
    }

    public interface IEcucInstanceBase : INotifyPropertyChanged
    {
        string BswmdPath { get; }
        string BswmdPathShort
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

        EcucInstanceManager Manager { get; }

        IEcucInstanceBase GetInstanceFromAsrPath(string path)
        {
            try
            {
                return Manager.AsrPathInstanceDict[path];
            }
            catch
            {
                throw new Exception($"Can not get isntance from {path}");
            }
        }

        List<IEcucInstanceReference> GetReferenceByAsrPath(string path)
        {
            try
            {
                return Manager.InstanceReferenceDict[path];
            }
            catch
            {
                throw new Exception($"Can not get reference from {path}");
            }
        }

        bool IsDirty { get; set; }
        EcucValid Valid { get; }
    }

    public interface IEcucInstanceModule : IEcucInstanceBase
    {
        string ShortName { get; set; }
        string AsrPath { get; }
        string AsrPathShort { get; }
        List<IEcucInstanceModule> Containers { get; }
        CommentGroupList Comment { get; set; }

        EcucInstanceContainer AddContainer(string path, string shortName);
        int DelContainer(string path, string shortName);
        int DelContainer(string path);
        void DelContainer();
        void UpdateAsrPathShort(string newName);
        void UpdateAsrPathPrefix(string newName);

        List<IEcucInstanceModule> FindContainerByBswmd(string bswmdPath)
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

    public interface IEcucInstanceContainer : IEcucInstanceModule
    {
        List<IEcucInstanceParam> Paras { get; }
        List<IEcucInstanceReference> Refs { get; }
        IEcucInstanceModule Parent { get; }

        List<IEcucInstanceParam> FindParaByBswmd(string bswmdPath)
        {
            var query = from para in Paras
                        where para.BswmdPath == bswmdPath
                        select para;

            return query.ToList();
        }

        List<IEcucInstanceReference> FindRefByBswmd(string bswmdPath)
        {
            var query = from reference in Refs
                        where reference.BswmdPath == bswmdPath
                        select reference;

            return query.ToList();
        }
    }

    public interface IEcucInstanceParam : IEcucInstanceBase
    {
        IEcucInstanceModule Parent { get; }
        string Value { get; set; }
        public List<string> Comment { get; set; }
    }

    public interface IEcucInstanceReference : IEcucInstanceBase
    {
        IEcucInstanceModule Parent { get; }
        string ValueRef { get; set; }
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
                var ValueRefOld = ValueRef;
                var parts = ValueRef.Split('/');
                if (parts.Length > 0)
                {
                    if (parts[^1] != value)
                    {
                        parts[^1] = value;
                        ValueRef = string.Join("/", parts);
                        Manager.InstanceReferenceDict[ValueRef] = Manager.InstanceReferenceDict[ValueRefOld];
                        Manager.InstanceReferenceDict.Remove(ValueRefOld);
                    } 
                }
            }
        }
        public List<string> Comment { get; set; }
    }

    public class EcucInstanceManager
    {
        private readonly List<Asr> Autosars = new();
        public List<EcucInstanceModule> EcucModules { get; } = new();
        public Dictionary<object, string> AsrRawAsrPathDict = new();
        public Dictionary<string, object> AsrPathAsrRawDict = new();
        public Dictionary<string, List<IEcucInstanceBase>> BswmdPathInstanceDict = new();
        public Dictionary<string, IEcucInstanceBase> AsrPathInstanceDict = new();
        public Dictionary<string, List<IEcucInstanceReference>> InstanceReferenceDict = new();
        public Dictionary<string, AUTOSAR> FileAsrRawDict = new();
        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;
                }
            }
        }

        public EcucInstanceManager(string content)
        {
            InitSingle(content);
        }

        public EcucInstanceManager(string[] fileNames)
        {
            InitMultiple(fileNames);
            var query = from module in EcucModules
                        orderby module.ShortName ascending
                        select module;
            EcucModules = query.ToList();
        }

        private void InitSingle(string content)
        {
            var autosar = new Asr(content);
            Autosars.Add(autosar);
            List<ARPACKAGE> arPackages = new();
            foreach (var package in autosar.ArPackages)
            {
                arPackages.Add(package);
            }

            foreach (var value in autosar.AsrPathModelDict)
            {
                AsrPathAsrRawDict[value.Key] = value.Value;
                AsrRawAsrPathDict[value.Value] = value.Key;
            }

            Monitor.Enter(EcucModules);
            try
            {
                var query = from package in arPackages
                            where package.ELEMENTS?.ECUCMODULECONFIGURATIONVALUES != null
                            from ecucModuleValue in package.ELEMENTS.ECUCMODULECONFIGURATIONVALUES
                            select ecucModuleValue;

                query.ToList().ForEach(x => EcucModules.Add(new EcucInstanceModule(x, this, autosar)));
            }
            finally
            {
                Monitor.Exit(EcucModules);
            }
        }

        private void InitMultiple(string[] fileNames)
        {
            var tasks = new Task[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
            {
                tasks[i] = InitMultipleSingleStep1(fileNames[i]);
            }
            Task.WaitAll(tasks);

            tasks = new Task[Autosars.Count];
            for (int i = 0; i < Autosars.Count; i++)
            {
                tasks[i] = InitMultipleSingleStep2(Autosars[i]);
            }
            Task.WaitAll(tasks);

            for (int i = 0; i < Autosars.Count; i++)
            {
                tasks[i] = InitMultipleSingleStep3(Autosars[i]);
            }
            Task.WaitAll(tasks);
        }

        private Task InitMultipleSingleStep1(string fileName)
        {
            return Task.Run(() =>
            {
                var autosar = new Asr(fileName);
                Monitor.Enter(Autosars);
                try
                {
                    Autosars.Add(autosar);
                }
                finally
                {
                    Monitor.Exit(Autosars);
                }
            });
        }

        private Task InitMultipleSingleStep2(Asr ar)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(AsrRawAsrPathDict);
                try
                {
                    foreach (var value in ar.AsrPathModelDict)
                    {
                        if (AsrPathAsrRawDict.ContainsKey(value.Key) == false)
                        {
                            AsrPathAsrRawDict[value.Key] = value.Value;
                            AsrRawAsrPathDict[value.Value] = value.Key;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(AsrRawAsrPathDict);
                }
            });
        }

        private Task InitMultipleSingleStep3(Asr ar)
        {
            return Task.Run(() =>
            {
                Monitor.Enter(EcucModules);
                try
                {
                    var query = from package in ar.ArPackages
                                where package.ELEMENTS.ECUCMODULECONFIGURATIONVALUES != null
                                from ecucModuleValue in package.ELEMENTS.ECUCMODULECONFIGURATIONVALUES
                                select ecucModuleValue;

                    query.ToList().ForEach(x =>
                    {
                        var module = new EcucInstanceModule(x, this, ar);
                        EcucModules.Add(module);
                        ar.Modules.Add(module);
                    });
                }
                finally
                {
                    Monitor.Exit(EcucModules);
                }
            });
        }

        public List<IEcucInstanceBase> GetInstancesByBswmdPath(string bswmdPath)
        {
            var result = new List<IEcucInstanceBase>();

            if (BswmdPathInstanceDict.ContainsKey(bswmdPath))
            {
                result = BswmdPathInstanceDict[bswmdPath];
            }
            return result;
        }

        public IEcucInstanceBase GetInstanceByAsrPath(string path)
        {
            try
            {
                return AsrPathInstanceDict[path];
            }
            catch
            {
                throw new Exception($"Can not get isntance from {path}");
            }
        }

        public List<IEcucInstanceReference> GetReferenceByAsrPath(string path)
        {
            try
            {
                return InstanceReferenceDict[path];
            }
            catch
            {
                throw new Exception($"Can not get reference from {path}");
            }
        }

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

        public void Save()
        {
            var query = from asr in Autosars
                        where asr.IsDirty == true
                        select asr;

            var result = query.ToList();

            var tasks = new Task[result.Count];

            for (int i = 0; i < result.Count; i++)
            {
                tasks[i] = SaveSingle(result[i]);
            }
            Task.WaitAll(tasks);
        }

        private static Task SaveSingle(Asr asr)
        {
            return Task.Run(() =>
            {
                asr.Save();
            });
        }

        /// <summary>
        /// Add object with name to dictionary.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public void AddAsrPath(string name, object model)
        {
            AsrPathAsrRawDict[name] = model;
            AsrRawAsrPathDict[model] = name;
            if (model is IEcucInstanceBase instance)
            {
                AsrPathInstanceDict[name] = instance;
            }
        }

        public void RemoveAsrPath(string name, object model)
        {
            AsrPathAsrRawDict.Remove(name);
            AsrRawAsrPathDict.Remove(model);
        }
    }

    public class EcucInstanceModule : NotifyPropertyChangedBase, IEcucInstanceModule
    {
        private ECUCMODULECONFIGURATIONVALUES Model { get; }
        public EcucInstanceManager Manager { get; }
        public Asr Parent { get; }
        public EcucValid Valid { get; set; }

        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;

                    if (isDirty == true)
                    {
                        Parent.IsDirty = true;
                    }
                    else
                    {
                        foreach (var container in Containers)
                        {
                            container.IsDirty = false;
                        }
                    }
                }
            }
        }

        public CommentGroupList Comment
        {
            get
            {
                if (Model.ADMINDATA == null)
                {
                    return new CommentGroupList();
                }

                if (Model.ADMINDATA.SDGS == null)
                {
                    return new CommentGroupList();
                }

                return new CommentGroupList(Model.ADMINDATA.SDGS.SDG);
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (value.Count == 0)
                {
                    return;
                }

                if (Model.ADMINDATA == null)
                {
                    Model.ADMINDATA = new ADMINDATA();
                }

                if (Model.ADMINDATA.SDGS == null)
                {
                    Model.ADMINDATA.SDGS = new ADMINDATA.SDGSLocalType();
                }

                foreach (var item in value)
                {
                    Model.ADMINDATA.SDGS.SDG.Add(item.Model);
                }
            }
        }

        public string BswmdPath
        {
            get
            {
                return Model.DEFINITIONREF.TypedValue;
            }
        }

        public string ShortName
        {
            get
            {
                return Model.SHORTNAME.TypedValue;
            }
            set
            {
                if (value != null)
                {
                    if (ShortName != value)
                    {
                        // cache old autosar path
                        var asrPathOld = AsrPath;
                        var parts = asrPathOld.Split('/');
                        if (parts.Length > 0)
                        {
                            // add new path to dict
                            UpdateAsrPathShort(value);
                            // autosar model modify
                            Model.SHORTNAME.TypedValue = value;
                        }
                    }
                }
            }
        }

        public string AsrPath
        {
            get
            {
                return Manager.AsrRawAsrPathDict[Model];
            }
        }

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

        public List<IEcucInstanceModule> Containers { get; } = new List<IEcucInstanceModule>();

        public EcucInstanceModule(ECUCMODULECONFIGURATIONVALUES model, EcucInstanceManager manager, Asr parent)
        {
            Model = model;
            Manager = manager;
            Parent = parent;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Monitor.Enter(Manager.BswmdPathInstanceDict);
            try
            {
                if (Manager.BswmdPathInstanceDict.ContainsKey(BswmdPath))
                {
                    Manager.BswmdPathInstanceDict[BswmdPath].Add(this);
                }
                else
                {
                    Manager.BswmdPathInstanceDict[BswmdPath] = new List<IEcucInstanceBase>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.BswmdPathInstanceDict);
            }

            Monitor.Enter(Manager.AsrPathInstanceDict);
            try
            {
                Manager.AsrPathInstanceDict[AsrPath] = this;
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathInstanceDict);
            }

            if (Model.CONTAINERS != null)
            {
                if (Model.CONTAINERS.ECUCCONTAINERVALUE != null)
                {
                    foreach (ECUCCONTAINERVALUE container in Model.CONTAINERS.ECUCCONTAINERVALUE)
                    {
                        Containers.Add(new EcucInstanceContainer(container, Manager, this));
                    }
                }
            }
        }

        public EcucInstanceContainer AddContainer(string path, string shortName)
        {
            var model = new ECUCCONTAINERVALUE
            {
                DEFINITIONREF = new ECUCCONTAINERVALUE.DEFINITIONREFLocalType
                {
                    DEST = "ECUC-PARAM-CONF-CONTAINER-DEF",
                    TypedValue = $"{BswmdPath}/{path}"
                },
                SHORTNAME = new IDENTIFIER
                {
                    TypedValue = shortName
                },
                UUID = Guid.NewGuid().ToString()
            };

            if (Manager.AsrPathAsrRawDict.ContainsKey($"{AsrPath}/{shortName}") == false)
            {
                if (Model.CONTAINERS == null)
                {
                    Model.CONTAINERS = new ECUCMODULECONFIGURATIONVALUES.CONTAINERSLocalType();
                }
                Model.CONTAINERS.ECUCCONTAINERVALUE.Add(model);
                Manager.AddAsrPath($"{AsrPath}/{shortName}", model);
                var newContainer = new EcucInstanceContainer(model, Manager, this);
                Containers.Add(newContainer);
                IsDirty = true;
                return newContainer;
            }
            else
            {
                throw new Exception($"Find duplicate container {shortName} when add continer");
            }
        }

        public int DelContainer(string path, string shortName)
        {
            foreach (var container in Containers)
            {
                if (container.BswmdPath == path && container.ShortName == shortName)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        Manager.RemoveAsrPath(instanceContainer.AsrPath, instanceContainer);
                        Model.CONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                        IsDirty = true;

                        if (container.Containers.Count > 0)
                        {
                            if (container is EcucInstanceContainer instanceContainer2)
                            {
                                instanceContainer2.DelContainer();
                            }
                        }
                    }
                }
            }

            Containers.RemoveAll(x => x.BswmdPath == path && x.ShortName == shortName);
            return Containers.FindAll(x => x.BswmdPath == path).Count;
        }

        public int DelContainer(string path)
        {
            foreach (var container in Containers)
            {
                if (container.BswmdPathShort == path)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        Manager.RemoveAsrPath(instanceContainer.AsrPath, instanceContainer);
                        Model.CONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                        IsDirty = true;

                        if (container.Containers.Count > 0)
                        {
                            if (container is EcucInstanceContainer instanceContainer2)
                            {
                                instanceContainer2.DelContainer();
                            }
                        }
                    }
                }
            }

            Containers.RemoveAll(x => x.BswmdPath == path);
            return Containers.FindAll(x => x.BswmdPath == path).Count;
        }

        public void DelContainer()
        {
            foreach (var container in Containers)
            {
                if (container is EcucInstanceContainer instanceContainer)
                {
                    Manager.RemoveAsrPath(instanceContainer.AsrPath, instanceContainer);
                    Model.CONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                    IsDirty = true;

                    if (container.Containers.Count > 0)
                    {
                        if (container is EcucInstanceContainer instanceContainer2)
                        {
                            instanceContainer2.DelContainer();
                        }
                    }
                }
            }
            Containers.RemoveAll(x => x.BswmdPath != "");
        }

        /// <summary>
        /// Update autosar path dictionary of container
        /// </summary>
        /// <param name="newName">New short name of container</param>
        public void UpdateAsrPathShort(string newName)
        {
            // Construct new autosar path
            var asrPathOld = AsrPath;
            var parts = AsrPath.Split("/");
            parts[^1] = newName;
            var asrPathNew = string.Join("/", parts);

            // Check name duplication
            if (Manager.AsrPathAsrRawDict.ContainsKey(asrPathNew))
            {
                throw new Exception($"Already have continer with name {newName}");
            }

            // Update AsrPathAsrRawDict
            Manager.AsrPathAsrRawDict[asrPathNew] = Manager.AsrPathAsrRawDict[asrPathOld];
            Manager.AsrPathAsrRawDict.Remove(asrPathOld);
            // Update AsrRawAsrPathDict
            Manager.AsrRawAsrPathDict[Model] = asrPathNew;
            // Update AsrPathInstanceDict
            Manager.AsrPathInstanceDict[asrPathNew] = Manager.AsrPathInstanceDict[asrPathOld];
            Manager.AsrPathInstanceDict.Remove(asrPathOld);

            // Updata InstanceReferenceDict, change all reference point to container
            try
            {
                var usage = (this as IEcucInstanceBase).GetReferenceByAsrPath(asrPathOld);
                foreach (var u in usage)
                {
                    u.ValueRef = asrPathNew;
                }
            }
            catch
            {

            }

            foreach (var continer in Containers)
            {
                continer.UpdateAsrPathPrefix(asrPathNew);
            }
        }

        /// <summary>
        /// Update autosar path prefix of container.
        /// The prefix may be changed by parent container shortname changed.
        /// </summary>
        /// <param name="newName">New short name of container</param>
        public void UpdateAsrPathPrefix(string newName)
        {
            // Construct new autosar path
            var asrPathOld = AsrPath;
            var parts = AsrPath.Split("/");
            var asrPathNew = $"{newName}/{parts[^1]}";

            // Check name duplication
            if (Manager.AsrPathAsrRawDict.ContainsKey(asrPathNew))
            {
                throw new Exception($"Already have continer with name {newName}");
            }

            // Update AsrPathAsrRawDict
            Manager.AsrPathAsrRawDict[asrPathNew] = Manager.AsrPathAsrRawDict[asrPathOld];
            Manager.AsrPathAsrRawDict.Remove(asrPathOld);
            // Update AsrRawAsrPathDict
            Manager.AsrRawAsrPathDict[Model] = asrPathNew;
            // Update AsrPathInstanceDict
            Manager.AsrPathInstanceDict[asrPathNew] = Manager.AsrPathInstanceDict[asrPathOld];
            Manager.AsrPathInstanceDict.Remove(asrPathOld);

            // Updata InstanceReferenceDict, change all reference point to container
            try
            {
                var usage = (this as IEcucInstanceBase).GetReferenceByAsrPath(asrPathOld);
                foreach (var u in usage)
                {
                    u.ValueRef = asrPathNew;
                }
            }
            catch
            {

            }

            foreach (var continer in Containers)
            {
                continer.UpdateAsrPathPrefix(asrPathNew);
            }
        }

        private void ValidChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Valid));
        }
    }

    public class EcucInstanceContainer : NotifyPropertyChangedBase, IEcucInstanceContainer
    {
        public ECUCCONTAINERVALUE Model { get; }
        public EcucInstanceManager Manager { get; }
        public IEcucInstanceModule Parent { get; }
        public EcucValid Valid { get; set; }

        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;
                    if (value == true)
                    {
                        Parent.IsDirty = true;
                    }
                    else
                    {
                        foreach (var container in Containers)
                        {
                            container.IsDirty = false;
                        }
                    }
                }
            }
        }

        public CommentGroupList Comment
        {
            get
            {
                if (Model.ADMINDATA == null)
                {
                    return new CommentGroupList();
                }

                if (Model.ADMINDATA.SDGS == null)
                {
                    return new CommentGroupList();
                }

                return new CommentGroupList(Model.ADMINDATA.SDGS.SDG);
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (value.Count == 0)
                {
                    return;
                }

                if (Model.ADMINDATA == null)
                {
                    Model.ADMINDATA = new ADMINDATA();
                }

                if (Model.ADMINDATA.SDGS == null)
                {
                    Model.ADMINDATA.SDGS = new ADMINDATA.SDGSLocalType();
                }

                foreach (var item in value)
                {
                    Model.ADMINDATA.SDGS.SDG.Add(item.Model);
                }
            }
        }

        public string BswmdPath
        {
            get
            {
                return Model.DEFINITIONREF.TypedValue;
            }
        }

        public string ShortName
        {
            get
            {
                return Model.SHORTNAME.TypedValue;
            }
            set
            {
                if (value != null)
                {
                    if (ShortName != value)
                    {
                        // cache old autosar path
                        var asrPathOld = AsrPath;
                        var parts = asrPathOld.Split('/');
                        if (parts.Length > 0)
                        {
                            // add new path to dict
                            UpdateAsrPathShort(value); 
                            // autosar model modify
                            Model.SHORTNAME.TypedValue = value;
                        }
                    }
                }
            }
        }

        public string AsrPath
        {
            get
            {
                return Manager.AsrRawAsrPathDict[Model];
            }
        }

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

        public List<IEcucInstanceModule> Containers { get; } = new();
        public List<IEcucInstanceParam> Paras { get; } = new();
        public List<IEcucInstanceReference> Refs { get; } = new();

        public EcucInstanceContainer(ECUCCONTAINERVALUE model, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = model;
            Manager = manager;
            Parent = parent;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Monitor.Enter(Manager.BswmdPathInstanceDict);
            try
            {
                if (Manager.BswmdPathInstanceDict.ContainsKey(BswmdPath))
                {
                    Manager.BswmdPathInstanceDict[BswmdPath].Add(this);
                }
                else
                {
                    Manager.BswmdPathInstanceDict[BswmdPath] = new List<IEcucInstanceBase>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.BswmdPathInstanceDict);
            }

            Monitor.Enter(Manager.AsrPathInstanceDict);
            try
            {
                Manager.AsrPathInstanceDict[AsrPath] = this;
            }
            finally
            {
                Monitor.Exit(Manager.AsrPathInstanceDict);
            }

            if (Model.SUBCONTAINERS != null)
            {
                if (Model.SUBCONTAINERS.ECUCCONTAINERVALUE != null)
                {
                    foreach (ECUCCONTAINERVALUE container in Model.SUBCONTAINERS.ECUCCONTAINERVALUE)
                    {
                        Containers.Add(new EcucInstanceContainer(container, Manager, this));
                    }
                }
            }

            if (Model.PARAMETERVALUES != null)
            {
                if (Model.PARAMETERVALUES.ECUCTEXTUALPARAMVALUE != null)
                {
                    foreach (ECUCTEXTUALPARAMVALUE para in Model.PARAMETERVALUES.ECUCTEXTUALPARAMVALUE)
                    {
                        Paras.Add(new EcucInstanceTextualParamValue(para, Manager, this));
                    }
                }

                if (Model.PARAMETERVALUES.ECUCNUMERICALPARAMVALUE != null)
                {
                    foreach (ECUCNUMERICALPARAMVALUE para in Model.PARAMETERVALUES.ECUCNUMERICALPARAMVALUE)
                    {
                        Paras.Add(new EcucInstanceNumericalParamValue(para, Manager, this));
                    }
                }
            }

            if (Model.REFERENCEVALUES != null)
            {
                if (Model.REFERENCEVALUES.ECUCREFERENCEVALUE != null)
                {
                    foreach (ECUCREFERENCEVALUE reference in Model.REFERENCEVALUES.ECUCREFERENCEVALUE)
                    {
                        Refs.Add(new EcucInstanceReferenceValue(reference, Manager, this));
                    }
                }
            }
        }

        public EcucInstanceContainer AddContainer(string path, string shortName)
        {
            var model = new ECUCCONTAINERVALUE
            {
                DEFINITIONREF = new ECUCCONTAINERVALUE.DEFINITIONREFLocalType
                {
                    DEST = "ECUC-PARAM-CONF-CONTAINER-DEF",
                    TypedValue = $"{BswmdPath}/{path}"
                },
                SHORTNAME = new IDENTIFIER
                {
                    TypedValue = shortName
                },
                UUID = Guid.NewGuid().ToString()
            };

            if (Manager.AsrPathAsrRawDict.ContainsKey($"{AsrPath}/{shortName}") == false)
            {
                if (Model.SUBCONTAINERS == null)
                {
                    Model.SUBCONTAINERS = new ECUCCONTAINERVALUE.SUBCONTAINERSLocalType();
                }
                Model.SUBCONTAINERS.ECUCCONTAINERVALUE.Add(model);
                Manager.AsrPathAsrRawDict[$"{AsrPath}/{shortName}"] = model;
                Manager.AsrRawAsrPathDict[model] = $"{AsrPath}/{shortName}";
                var newContainer = new EcucInstanceContainer(model, Manager, this);
                Containers.Add(newContainer);
                IsDirty = true;
                return newContainer;
            }
            else
            {
                throw new Exception($"Find duplicate container {shortName} when add continer");
            }
        }

        public int DelContainer(string path, string shortName)
        {
            foreach (var container in Containers)
            {
                if (container.BswmdPath == path && container.ShortName == shortName)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        Manager.AsrPathAsrRawDict.Remove(instanceContainer.AsrPath);
                        Manager.AsrRawAsrPathDict.Remove(instanceContainer);
                        Model.SUBCONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                        IsDirty = true;

                        if (container.Containers.Count > 0)
                        {
                            if (container is EcucInstanceContainer instanceContainer2)
                            {
                                instanceContainer2.DelContainer();
                            }
                        }
                    }
                }
            }

            Containers.RemoveAll(x => x.BswmdPath == path && x.ShortName == shortName);
            return Containers.FindAll(x => x.BswmdPath == path).Count;
        }

        public int DelContainer(string path)
        {
            foreach (var container in Containers)
            {
                if (container.BswmdPathShort == path)
                {
                    if (container is EcucInstanceContainer instanceContainer)
                    {
                        Manager.AsrPathAsrRawDict.Remove(instanceContainer.AsrPath);
                        Manager.AsrRawAsrPathDict.Remove(instanceContainer);
                        Model.SUBCONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                        IsDirty = true;

                        if (container.Containers.Count > 0)
                        {
                            if (container is EcucInstanceContainer instanceContainer2)
                            {
                                instanceContainer2.DelContainer();
                            }
                        }
                    }
                }
            }

            Containers.RemoveAll(x => x.BswmdPath == path);
            return Containers.FindAll(x => x.BswmdPath == path).Count;
        }

        public void DelContainer()
        {
            foreach (var container in Containers)
            {
                if (container is EcucInstanceContainer instanceContainer)
                {
                    Manager.AsrPathAsrRawDict.Remove(instanceContainer.AsrPath);
                    Manager.AsrRawAsrPathDict.Remove(instanceContainer);
                    Model.SUBCONTAINERS.ECUCCONTAINERVALUE.Remove(instanceContainer.Model);
                    IsDirty = true;

                    if (container.Containers.Count > 0)
                    {
                        if (container is EcucInstanceContainer instanceContainer2)
                        {
                            instanceContainer2.DelContainer();
                        }
                    }
                }
            }
            Containers.RemoveAll(x => x.BswmdPath != "");
        }

        public EcucInstanceTextualParamValue AddTextualPara(string path, string value, string dest)
        {
            var paraNew = new EcucInstanceTextualParamValue($"{BswmdPath}/{path}", dest, value, Manager, this);

            if (Model.PARAMETERVALUES == null)
            {
                Model.PARAMETERVALUES = new ECUCCONTAINERVALUE.PARAMETERVALUESLocalType();
            }
            Model.PARAMETERVALUES.ECUCTEXTUALPARAMVALUE.Add(paraNew.Model);
            Paras.Add(paraNew);
            paraNew.IsDirty = true;
            return paraNew;
        }

        public void DelTextualPara(string path, string value)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path && para.Value == value
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceTextualParamValue textualPara)
                {
                    Model.PARAMETERVALUES.ECUCTEXTUALPARAMVALUE.Remove(textualPara.Model);
                    Manager.BswmdPathInstanceDict[para.BswmdPath].Remove(para);
                    Paras.Remove(para);
                    IsDirty = true;
                }
            }
        }

        public void DelTextualPara(string path)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceTextualParamValue textualPara)
                {
                    Model.PARAMETERVALUES.ECUCTEXTUALPARAMVALUE.Remove(textualPara.Model);
                    Manager.BswmdPathInstanceDict[para.BswmdPath].Remove(para);
                    Paras.Remove(para);
                    IsDirty = true;
                }
            }
        }

        public EcucInstanceNumericalParamValue AddNumericalPara(string path, string value, string dest)
        {
            var paraNew = new EcucInstanceNumericalParamValue($"{BswmdPath}/{path}", dest, value, Manager, this);
            if (Model.PARAMETERVALUES == null)
            {
                Model.PARAMETERVALUES = new ECUCCONTAINERVALUE.PARAMETERVALUESLocalType();
            }
            Model.PARAMETERVALUES.ECUCNUMERICALPARAMVALUE.Add(paraNew.Model);
            Paras.Add(paraNew);
            paraNew.IsDirty = true;
            return paraNew;
        }

        public void DelNumericalPara(string path, string value)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path && para.Value == value
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceNumericalParamValue numericalPara)
                {
                    Model.PARAMETERVALUES.ECUCNUMERICALPARAMVALUE.Remove(numericalPara.Model);
                    Manager.BswmdPathInstanceDict[para.BswmdPath].Remove(para);
                    Paras.Remove(para);
                    IsDirty = true;
                }
            }
        }

        public void DelNumericalPara(string path)
        {
            var query = from para in Paras
                        where para.BswmdPathShort == path
                        select para;

            foreach (var para in query.ToList())
            {
                if (para is EcucInstanceNumericalParamValue numericalPara)
                {
                    Model.PARAMETERVALUES.ECUCNUMERICALPARAMVALUE.Remove(numericalPara.Model);
                    Manager.BswmdPathInstanceDict[para.BswmdPath].Remove(para);
                    Paras.Remove(para);
                    IsDirty = true;
                }
            }
        }

        public EcucInstanceReferenceValue AddReference(string path, string defDest, string valueRef, string valueDest)
        {
            var refNew = new EcucInstanceReferenceValue($"{BswmdPath}/{path}", defDest, valueRef, valueDest, Manager, this);
            if (Model.REFERENCEVALUES == null)
            {
                Model.REFERENCEVALUES = new ECUCCONTAINERVALUE.REFERENCEVALUESLocalType();
            }
            Model.REFERENCEVALUES.ECUCREFERENCEVALUE.Add(refNew.Model);
            refNew.IsDirty = true;
            Refs.Add(refNew);
            return refNew;
        }

        public void DelReference(string path, string valueRef)
        {
            var query = from reference in Refs
                        where reference.BswmdPathShort == path && reference.ValueRef == valueRef
                        select reference;

            foreach (var reference in query.ToList())
            {
                if (reference is EcucInstanceReferenceValue instanceReference)
                {
                    Model.REFERENCEVALUES.ECUCREFERENCEVALUE.Remove(instanceReference.Model);
                    Refs.Remove(instanceReference);
                    IsDirty = true;
                }
            }
        }

        public void DelReference(string path)
        {
            var query = from reference in Refs
                        where reference.BswmdPathShort == path
                        select reference;

            foreach (var reference in query.ToList())
            {
                if (reference is EcucInstanceReferenceValue instanceReference)
                {
                    Model.REFERENCEVALUES.ECUCREFERENCEVALUE.Remove(instanceReference.Model);
                    Refs.Remove(instanceReference);
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Update autosar path dictionary of container
        /// </summary>
        /// <param name="newName">New short name of container</param>
        public void UpdateAsrPathShort(string newName)
        {
            // Construct new autosar path
            var asrPathOld = AsrPath;
            var parts = AsrPath.Split("/");
            parts[^1] = newName;
            var asrPathNew = string.Join("/", parts);

            // Check name duplication
            if (Manager.AsrPathAsrRawDict.ContainsKey(asrPathNew))
            {
                throw new Exception($"Already have continer with name {newName}");
            }

            // Update AsrPathAsrRawDict
            Manager.AsrPathAsrRawDict[asrPathNew] = Manager.AsrPathAsrRawDict[asrPathOld];
            Manager.AsrPathAsrRawDict.Remove(asrPathOld);
            // Update AsrRawAsrPathDict
            Manager.AsrRawAsrPathDict[Model] = asrPathNew;
            // Update AsrPathInstanceDict
            Manager.AsrPathInstanceDict[asrPathNew] = Manager.AsrPathInstanceDict[asrPathOld];
            Manager.AsrPathInstanceDict.Remove(asrPathOld);

            // Updata InstanceReferenceDict, change all reference point to container
            try
            {
                var usage = (this as IEcucInstanceBase).GetReferenceByAsrPath(asrPathOld);
                foreach (var u in usage)
                {
                    u.ValueRef = asrPathNew;
                }
                Manager.InstanceReferenceDict[asrPathNew] = Manager.InstanceReferenceDict[asrPathOld];
                Manager.InstanceReferenceDict.Remove(asrPathOld);
            }
            catch
            {

            }

            foreach (var continer in Containers)
            {
                continer.UpdateAsrPathPrefix(asrPathNew);
            }
        }

        /// <summary>
        /// Update autosar path prefix of container.
        /// The prefix may be changed by parent container shortname changed.
        /// </summary>
        /// <param name="newName">New short name of container</param>
        public void UpdateAsrPathPrefix(string newName)
        {
            // Construct new autosar path
            var asrPathOld = AsrPath;
            var parts = AsrPath.Split("/");
            var asrPathNew = $"{newName}/{parts[^1]}";

            // Check name duplication
            if (Manager.AsrPathAsrRawDict.ContainsKey(asrPathNew))
            {
                throw new Exception($"Already have continer with name {newName}");
            }

            // Update AsrPathAsrRawDict
            Manager.AsrPathAsrRawDict[asrPathNew] = Manager.AsrPathAsrRawDict[asrPathOld];
            Manager.AsrPathAsrRawDict.Remove(asrPathOld);
            // Update AsrRawAsrPathDict
            Manager.AsrRawAsrPathDict[Model] = asrPathNew;
            // Update AsrPathInstanceDict
            Manager.AsrPathInstanceDict[asrPathNew] = Manager.AsrPathInstanceDict[asrPathOld];
            Manager.AsrPathInstanceDict.Remove(asrPathOld);

            // Updata InstanceReferenceDict, change all reference point to container
            try
            {
                var usage = (this as IEcucInstanceBase).GetReferenceByAsrPath(asrPathOld);
                foreach (var u in usage)
                {
                    u.ValueRef = asrPathNew;
                }
                Manager.InstanceReferenceDict[asrPathNew] = Manager.InstanceReferenceDict[asrPathOld];
                Manager.InstanceReferenceDict.Remove(asrPathOld);
            }
            catch
            {

            }

            foreach (var continer in Containers)
            {
                continer.UpdateAsrPathPrefix(asrPathNew);
            }
        }

        private void ValidChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            Parent.Valid.RaisePropertyChanged(nameof(Valid));
            RaisePropertyChanged(nameof(Valid));
        }
    }

    public class EcucInstanceTextualParamValue : NotifyPropertyChangedBase, IEcucInstanceParam
    {
        public ECUCTEXTUALPARAMVALUE Model { get; }
        public EcucInstanceManager Manager { get; }
        public IEcucInstanceModule Parent { get; }
        public EcucValid Valid { get; set; }

        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;
                    if (value == true)
                    {
                        Parent.IsDirty = true;
                    }
                }
            }
        }

        public string BswmdPath
        {
            get
            {
                return Model.DEFINITIONREF.TypedValue;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (BswmdPath == value)
                {
                    return;
                }
                Model.DEFINITIONREF.TypedValue = value;
            }
        }

        public string Value
        {
            get
            {
                try
                {
                    return Model.VALUE.TypedValue;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (Value == value)
                {
                    return;
                }
                Model.VALUE.TypedValue = value;
                IsDirty = true;
            }
        }

        public string Dest
        {
            get
            {
                return Model.DEFINITIONREF.DEST;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (Dest == value)
                {
                    return;
                }
                Model.DEFINITIONREF.DEST = value;
            }
        }

        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in Model.ANNOTATIONS.ANNOTATION)
                {
                    result.Add(anno.ANNOTATIONORIGIN.TypedValue);
                }
                return result;
            }
            set
            {
                Model.ANNOTATIONS.ANNOTATION = new List<ANNOTATION>();
                foreach (var comment in value)
                {
                    Model.ANNOTATIONS.ANNOTATION.Add
                    (
                        new ANNOTATION
                        {
                            ANNOTATIONORIGIN = new STRING
                            {
                                TypedValue = comment
                            }
                        }
                    );
                }
            }
        }

        public EcucInstanceTextualParamValue(string bswmdPath, string dest, string value, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = new ECUCTEXTUALPARAMVALUE
            {
                VALUE = new VERBATIMSTRING
                {
                    TypedValue = value
                },
                DEFINITIONREF = new ECUCTEXTUALPARAMVALUE.DEFINITIONREFLocalType
                {
                    TypedValue = bswmdPath,
                    DEST = dest
                },
            };
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        public EcucInstanceTextualParamValue(ECUCTEXTUALPARAMVALUE model, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = model;
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        private void Register()
        {
            Monitor.Enter(Manager.BswmdPathInstanceDict);
            try
            {
                if (Manager.BswmdPathInstanceDict.ContainsKey(BswmdPath))
                {
                    Manager.BswmdPathInstanceDict[BswmdPath].Add(this);
                }
                else
                {
                    Manager.BswmdPathInstanceDict[BswmdPath] = new List<IEcucInstanceBase>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.BswmdPathInstanceDict);
            }
        }

        private void ValidChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            Parent.Valid.RaisePropertyChanged(nameof(Valid));
            RaisePropertyChanged(nameof(Valid));
        }
    }

    public class EcucInstanceNumericalParamValue : NotifyPropertyChangedBase, IEcucInstanceParam
    {
        public ECUCNUMERICALPARAMVALUE Model { get; }
        public EcucInstanceManager Manager { get; }
        public IEcucInstanceModule Parent { get; }
        public EcucValid Valid { get; set; }

        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;
                    if (value == true)
                    {
                        Parent.IsDirty = true;
                    }
                }
            }
        }

        public string BswmdPath
        {
            get
            {
                return Model.DEFINITIONREF.TypedValue;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (BswmdPath == value)
                {
                    return;
                }
                Model.DEFINITIONREF.TypedValue = value;
            }
        }

        public string Value
        {
            get
            {
                try
                {
                    return Model.VALUE.Untyped.Value;
                }
                catch
                {
                    return "0";
                }
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (Value == value)
                {
                    return;
                }
                Model.VALUE.Untyped.Value = value;
                IsDirty = true;
            }
        }

        public string Dest
        {
            get
            {
                return Model.DEFINITIONREF.DEST;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (Dest == value)
                {
                    return;
                }
                Model.DEFINITIONREF.DEST = value;
            }
        }

        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in Model.ANNOTATIONS.ANNOTATION)
                {
                    result.Add(anno.ANNOTATIONORIGIN.TypedValue);
                }
                return result;
            }
            set
            {
                Model.ANNOTATIONS.ANNOTATION = new List<ANNOTATION>();
                foreach (var comment in value)
                {
                    Model.ANNOTATIONS.ANNOTATION.Add
                    (
                        new ANNOTATION
                        {
                            ANNOTATIONORIGIN = new STRING
                            {
                                TypedValue = comment
                            }
                        }
                    );
                }
            }
        }

        public EcucInstanceNumericalParamValue(string bswmdPath, string dest, string value, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = new ECUCNUMERICALPARAMVALUE
            {
                VALUE = new NUMERICALVALUEVARIATIONPOINT(),
                DEFINITIONREF = new ECUCNUMERICALPARAMVALUE.DEFINITIONREFLocalType
                {
                    TypedValue = bswmdPath,
                    DEST = dest
                }
            };
            Model.VALUE.Untyped.Value = value;
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        public EcucInstanceNumericalParamValue(ECUCNUMERICALPARAMVALUE model, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = model;
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        private void Register()
        {
            Monitor.Enter(Manager.BswmdPathInstanceDict);
            try
            {
                if (Manager.BswmdPathInstanceDict.ContainsKey(BswmdPath))
                {
                    Manager.BswmdPathInstanceDict[BswmdPath].Add(this);
                }
                else
                {
                    Manager.BswmdPathInstanceDict[BswmdPath] = new List<IEcucInstanceBase>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.BswmdPathInstanceDict);
            }
        }

        private void ValidChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            Parent.Valid.RaisePropertyChanged(nameof(Valid));
            RaisePropertyChanged(nameof(Valid));
        }
    }

    public class EcucInstanceReferenceValue : NotifyPropertyChangedBase, IEcucInstanceReference
    {
        public ECUCREFERENCEVALUE Model { get; }
        public EcucInstanceManager Manager { get; }
        public IEcucInstanceModule Parent { get; }
        public EcucValid Valid { get; set; }

        private bool isDirty = false;

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (value != IsDirty)
                {
                    isDirty = value;
                    if (value == true)
                    {
                        Parent.IsDirty = true;
                    }
                }
            }
        }

        public string BswmdPath
        {
            get
            {
                return Model.DEFINITIONREF.TypedValue;
            }
        }

        public string DefDest
        {
            get
            {
                return Model.VALUEREF.DEST;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (DefDest == value)
                {
                    return;
                }

                Model.VALUEREF.DEST = value;
            }
        }

        public string ValueRef
        {
            get
            {
                if (Model.VALUEREF != null)
                {
                    return Model.VALUEREF.TypedValue;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (ValueRef == value)
                {
                    return;
                }

                Model.VALUEREF.TypedValue = value;
                IsDirty = true;
            }
        }

        public string ValueDest
        {
            get
            {
                return Model.VALUEREF.DEST;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (ValueDest == value)
                {
                    return;
                }

                Model.VALUEREF.DEST = value;
            }
        }

        public List<string> Comment
        {
            get
            {
                var result = new List<string>();
                foreach (var anno in Model.ANNOTATIONS.ANNOTATION)
                {
                    result.Add(anno.ANNOTATIONORIGIN.TypedValue);
                }
                return result;
            }
            set
            {
                Model.ANNOTATIONS.ANNOTATION = new List<ANNOTATION>();
                foreach (var comment in value)
                {
                    Model.ANNOTATIONS.ANNOTATION.Add
                    (
                        new ANNOTATION
                        {
                            ANNOTATIONORIGIN = new STRING
                            {
                                TypedValue = comment
                            }
                        }
                    );
                }
            }
        }

        public EcucInstanceReferenceValue(string bswmdPath, string defDest, string valueRef, string valueDest, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            valueDest = valueDest switch
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
            Model = new ECUCREFERENCEVALUE
            {
                DEFINITIONREF = new ECUCREFERENCEVALUE.DEFINITIONREFLocalType
                {
                    TypedValue = bswmdPath,
                    DEST = defDest
                },
                VALUEREF = new ECUCREFERENCEVALUE.VALUEREFLocalType
                {
                    TypedValue = valueRef,
                    DEST = valueDest
                }
            };
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        public EcucInstanceReferenceValue(ECUCREFERENCEVALUE model, EcucInstanceManager manager, IEcucInstanceModule parent)
        {
            Model = model;
            Manager = manager;
            Parent = parent;
            IsDirty = parent.IsDirty;
            Valid = new EcucValid(this);
            Valid.PropertyChanged += ValidChangedEventHandler;

            Register();
        }

        private void Register()
        {
            Monitor.Enter(Manager.BswmdPathInstanceDict);
            try
            {
                if (Manager.BswmdPathInstanceDict.ContainsKey(BswmdPath))
                {
                    Manager.BswmdPathInstanceDict[BswmdPath].Add(this);
                }
                else
                {
                    Manager.BswmdPathInstanceDict[BswmdPath] = new List<IEcucInstanceBase>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.BswmdPathInstanceDict);
            }

            Monitor.Enter(Manager.InstanceReferenceDict);
            try
            {
                if (Manager.InstanceReferenceDict.ContainsKey(ValueRef))
                {
                    Manager.InstanceReferenceDict[ValueRef].Add(this);
                }
                else
                {
                    Manager.InstanceReferenceDict[ValueRef] = new List<IEcucInstanceReference>() { this };
                }
            }
            finally
            {
                Monitor.Exit(Manager.InstanceReferenceDict);
            }
        }

        private void ValidChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            Parent.Valid.RaisePropertyChanged(nameof(Valid));
            RaisePropertyChanged(nameof(Valid));
        }
    }
}
