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

using Autosar;
using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EInstance;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ecuc.EcucBase.EBase
{
    /// <summary>
    /// INotifyPropertyChanged abstaract to avoid type property name
    /// </summary>
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Asr Class represent a single arxml file
    /// </summary>
    public class Asr
    {
        /// <summary>
        /// File name of arxml file.
        /// </summary>
        private string FileName { get; } = "";
        /// <summary>
        /// Whether arxml model is modified or not.
        /// </summary>
        private bool isDirty = false;
        /// <summary>
        /// Converted data model of arxml file.
        /// </summary>
        public AUTOSAR? Model { get; } = null;
        /// <summary>
        /// Packages of Autosar in arxml file.
        /// </summary>
        public List<ARPACKAGE> ArPackages { get; } = new List<ARPACKAGE>();
        /// <summary>
        /// ECUC modules in arxml file.
        /// </summary>
        public List<EcucInstanceModule> Modules { get; } = new List<EcucInstanceModule>();
        /// <summary>
        /// Dictionary between Autosar path and converted model.
        /// </summary>
        public Dictionary<string, object> AsrPathModelDict { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Whether arxml model is modified or not.
        /// </summary>
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
                    // dirty from true to false shall tranfer to all modules
                    if (value == false)
                    {
                        foreach (var module in Modules)
                        {
                            module.IsDirty = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initialze Asr from filename.
        /// File will be converted to data model. All Autosar packages will be extracted.
        /// Autosar path dictionary will be generated.
        /// </summary>
        /// <param name="filename">Filename of file want to be converted.</param>
        public Asr(string filename)
        {
            FileName = filename;
            Model = AUTOSAR.Load(filename);

            if (Model != null)
            {
                // get all autosar packages
                FindAllArPackage();
                // iterate all elements to generate Autosar path dictionary
                CreateAsrPath();
            }
        }

        /// <summary>
        /// Iterate all ARPACKAGE and ARPACKAGES tag to get all Autosar package from Model.
        /// ARPACKAGES tag can have ARPACKAGE member and ARPACKAGE tag can have ARPACKAGES member.
        /// All ARPACKAGES and ARPACKAGE tag shall be checked to find all Autosar packages.
        /// </summary>
        private void FindAllArPackage()
        {
            if (Model == null)
            {
                return;
            }

            var query = from package in Model.ARPACKAGES.ARPACKAGE
                        select package;
            query.ToList().ForEach(x =>
            {
                if (x.ARPACKAGES != null)
                {
                    // Tag ARPACKAGE has ARPACKAGES member, step deeper.
                    FindAllArPackage(x);
                }
                // Tag ARPACKAGE has no ARPACKAGES member, get it.
                ArPackages.Add(x);
            });
        }

        /// <summary>
        /// Iterate all ARPACKAGE and ARPACKAGES tag to get all Autosar package from ARPACKAGE.
        /// </summary>
        /// <param name="arPackage">Autosar package to be checked</param>
        private void FindAllArPackage(ARPACKAGE arPackage)
        {
            var query = from package in arPackage.ARPACKAGES.ARPACKAGE
                        select package;
            query.ToList().ForEach(x =>
            {
                if (x.ARPACKAGES != null)
                {
                    // Tag ARPACKAGE has ARPACKAGES member, step deeper.
                    FindAllArPackage(x);
                }
                // Tag ARPACKAGE has no ARPACKAGES member, get it.
                ArPackages.Add(x);
            });
        }

        /// <summary>
        /// Iterate all tag in Model to genereate Autosar path dictionary.
        /// Any tag has SHORTNAME member can be treated as a dictionary.
        /// SHORTNAME with seperator "/" from top can idenfify tags in Model.
        /// as directory path in file system.
        /// </summary>
        private void CreateAsrPath()
        {
            string asrPath = "";
            if (Model != null)
            {
                // start from top with ARPACKAGES tag
                CreateAsrPath(Model.ARPACKAGES, asrPath);
            }
        }

        /// <summary>
        /// Iterate all tag in model to genereate Autosar path dictionary.
        /// Any tag has SHORTNAME member can be treated as a dictionary.
        /// SHORTNAME with seperator "/" from top can idenfify tags in Model.
        /// </summary>
        /// <param name="model">model to be checked</param>
        /// <param name="asrPath">current Autosar path</param>
        private void CreateAsrPath(object model, string asrPath)
        {
            // Reflect all properties to get SHORTNAME in model
            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.Name == "SHORTNAME")
                {
                    // Property with member "SHORTNAME"
                    if (prop.GetValue(model) is IDENTIFIER identifier)
                    {
                        // It is an IDENTIFIER instance, generate new path and put it into dictionary
                        asrPath += $"/{identifier.TypedValue}";
                        AsrPathModelDict.Add(asrPath, model);
                    }
                }
            }

            // Reflect all properties to decide it is needed to check deeper
            foreach (var prop in model.GetType().GetProperties())
            {
                if ((prop.PropertyType.Namespace == "Autosar") || (prop.PropertyType.Namespace == "System.Collections.Generic"))
                {
                    // Property with namespace "Autosar" or it is a Generic Collection instance
                    var v = prop.GetValue(model);
                    if ((v != null) && (prop.Name != "SHORTNAME"))
                    {
                        // Property is not null and not "SHORTNAME"
                        if (v is IEnumerable<object> valueList)
                        {
                            // Property is a Generic Collection instance, check each member deeper
                            foreach (var v2 in valueList)
                            {
                                CreateAsrPath(v2, asrPath);
                            }
                        }
                        else
                        {
                            // Property is not Generic Collection instance, check it deeper
                            CreateAsrPath(v, asrPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save model to file.
        /// After save, all module shall make IsDirty to false.
        /// </summary>
        public void Save()
        {
            if (FileName != "" && Model != null)
            {
                Model.Save(FileName);
                IsDirty = false;
            }
        }
    }

    /// <summary>
    /// Ecuc solve action handler delegate to invalid data.
    /// </summary>
    /// <param name="data">Ecuc data to solve invalid.</param>
    public delegate void EcucSolveHandler(EcucData data, object? param);

    /// <summary>
    /// Ecuc solve class.
    /// </summary>
    public class EcucSolve
    {
        /// <summary>
        /// Description of solve.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Handler to solve invalid data.
        /// </summary>
        public EcucSolveHandler Handler { get; set; }
        /// <summary>
        /// Ecuc valid this solve belong to.
        /// </summary>
        public EcucValid Valid { get; }

        /// <summary>
        /// Paramter for solve opearation.
        /// </summary>
        private readonly object? parameter;

        /// <summary>
        /// Initialize Ecuc solve class.
        /// </summary>
        /// <param name="desc">Description of invalid solve.</param>
        /// <param name="handler">Handler of invalid solve.</param>
        /// <param name="valid">Ecuc valid this solve belong to.</param>
        /// <param name="param">Parameter for solve operation.</param>
        public EcucSolve(string desc, EcucSolveHandler handler, EcucValid valid, object? param = null)
        {
            Description = desc;
            Handler = handler;
            Valid = valid;
            parameter = param;
        }

        /// <summary>
        /// Solve the valid problem.
        /// </summary>
        public void Solve()
        {
            // Only solve Ecuc valid with data
            if (Valid.Data != null)
            {
                try
                {
                    // Use handler to solve problem
                    Handler(Valid.Data, parameter);
                    // Change valid to true
                    Valid.UpdateValidStatus(true);
                }
                catch
                {
                    Console.WriteLine($"Solve {Description} failed");
                }
            }
        }
    }

    /// <summary>
    /// Ecuc valid class.
    /// Describe valid status, invalid reason and solve actions.
    /// </summary>
    public class EcucValid : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Valid status.
        /// </summary>
        private bool valid;
        /// <summary>
        /// Invalid reason.
        /// </summary>
        private string invalidReason;

        /// <summary>
        /// Instance of this valid belong to.
        /// </summary>
        public IEcucInstanceBase Instance { get; }

        /// <summary>
        /// Data of this valid belong to.
        /// </summary>
        public EcucData? Data { get; private set; }

        /// <summary>
        /// Solves to solve problem.
        /// </summary>
        public List<EcucSolve> Solves { get; private set; } = new();

        /// <summary>
        /// Valid status.
        /// </summary>
        public bool ValidStatus
        {
            get
            {
                return valid;
            }
        }

        /// <summary>
        /// Invalid reason.
        /// </summary>
        public string InvalidReason
        {
            get
            {
                return invalidReason;
            }
        }

        /// <summary>
        /// Recursice valid from this level to bottom.
        /// </summary>
        public List<EcucValid> ValidRecursive
        {
            get
            {
                var result = new List<EcucValid>();
                // Module asks containers
                if (Instance is EcucInstanceModule instanceModule)
                {
                    foreach (var container in instanceModule.Containers)
                    {
                        if (container.Valid.ValidRecursive.Count > 0)
                        {
                            result.AddRange(container.Valid.ValidRecursive);
                        }
                    }
                }

                // Container asks containers, parameters and references
                if (Instance is EcucInstanceContainer instanceContainer)
                {
                    foreach (var container in instanceContainer.Containers)
                    {
                        if (container.Valid.ValidRecursive.Count > 0)
                        {
                            result.AddRange(container.Valid.ValidRecursive);
                        }
                    }
                    foreach (var para in instanceContainer.Paras)
                    {
                        if (para.Valid.ValidRecursive.Count > 0)
                        {
                            result.AddRange(para.Valid.ValidRecursive);
                        }
                    }
                    foreach (var reference in instanceContainer.Refs)
                    {
                        if (reference.Valid.ValidRecursive.Count > 0)
                        {
                            result.AddRange(reference.Valid.ValidRecursive);
                        }
                    }
                }

                if (valid == false)
                {
                    // If self valid is false, add self
                    result.Add(this);
                }
                return result;
            }
        }

        /// <summary>
        /// Valid infomation.
        /// </summary>
        public string Info
        {
            get
            {
                return Instance switch
                {
                    IEcucInstanceModule module => $"{module.AsrPath}: {InvalidReason}",
                    IEcucInstanceParam param => $"{Instance.BswmdPathShort}@{param.Parent.AsrPath}: {InvalidReason}",
                    IEcucInstanceReference reference => $"{Instance.BswmdPathShort}@{reference.Parent.AsrPath}: {InvalidReason}",
                    _ => throw new Exception($"Invalid instance type {Instance.GetType()}"),
                };
            }
        }

        /// <summary>
        /// Initialize Ecuc valid class.
        /// </summary>
        /// <param name="instance">Instance of this valid belong to.</param>
        /// <param name="v">Valid status.</param>
        /// <param name="vReason">Invalid reason.</param>
        public EcucValid(IEcucInstanceBase instance, bool v = true, string vReason = "")
        {
            Instance = instance;
            valid = v;
            if (v == false)
            {
                invalidReason = vReason;
            }
            else
            {
                // ignore vReason when valid status is true
                invalidReason = "";
            }
        }

        /// <summary>
        /// Change data of this valid belong to.
        /// </summary>
        /// <param name="data">Data of this valid belong to.</param>
        public void UpdateData(EcucData data)
        {
            Data = data;
        }

        /// <summary>
        /// Change valid status and invalid reason.
        /// </summary>
        /// <param name="v">Valid status.</param>
        /// <param name="vReason">Invalid status.</param>
        public void UpdateValidStatus(bool v, string vReason = "")
        {
            if (v == true)
            {
                if (valid == false)
                {
                    // Valid status from false to true
                    valid = true;
                    invalidReason = "";
                    RaisePropertyChanged("Valid");
                }
            }
            else
            {
                if (valid == true)
                {
                    // Valid status from true to false
                    valid = false;
                    invalidReason = vReason;
                    RaisePropertyChanged("Valid");
                }
                else
                {
                    // Valid status from false to false
                    if (invalidReason != vReason)
                    {
                        invalidReason = vReason;
                        RaisePropertyChanged("Valid");
                    }
                }
            }
        }

        /// <summary>
        /// Add solve to valid.
        /// </summary>
        /// <param name="desc">Descrption of solve.</param>
        /// <param name="handler">Handler of solve.</param>
        /// <param name="param">Parameter of solve operation.</param>
        public void UpdateSolve(string desc, EcucSolveHandler handler, object? param = null)
        {
            Solves.Add(new EcucSolve(desc, handler, this, param));
        }

        /// <summary>
        /// Change  solves of valid.
        /// </summary>
        /// <param name="solves">Solves to change.</param>
        public void UpdateSolve(List<EcucSolve> solves)
        {
            Solves = solves;
        }

        /// <summary>
        /// Clear all solves.
        /// </summary>
        public void ClearSolve()
        {
            Solves.Clear();
        }

        /// <summary>
        /// Execute solver.
        /// </summary>
        /// <param name="index"></param>
        public void Solve(int index)
        {
            if (Data != null)
            {
                if (index < Solves.Count && index >= 0)
                {
                    // Run handler and update status
                    Solves[index].Solve();
                    UpdateValidStatus(true);
                }
            }
        }
    }

    public class EcucIdRangeCheck
    {
        readonly SortedDictionary<Int64, List<EcucData>> IdDict = new();

        public void Add(Int64 id, EcucData data)
        {
            try
            {
                if (!IdDict.ContainsKey(id))
                {
                    IdDict.Add(id, new());
                }
                IdDict[id].Add(data);
            }
            catch
            {
                Console.WriteLine($"Id range add fail when add data with id {id}");
            }
        }

        public void Check()
        {
            Int64 expected = 0;
            Int64 suggested = 0;

            foreach (var pair in IdDict)
            {
                if (pair.Key < 0)
                {
                    Console.WriteLine($"Id is negtive.");
                    foreach (var data in pair.Value)
                    {
                        data.UpdateValidStatus(false, $"Id is negtive.");
                        data.ClearValidSolve();
                        data.UpdateValidSolve($"Rearrange Id to {suggested}", IdRerangeHandler, suggested);
                        suggested++;
                    }
                }
                else if (pair.Value.Count > 1)
                {
                    Console.WriteLine($"Id is not unique ({pair.Key}).");
                    foreach (var data in pair.Value)
                    {
                        data.UpdateValidStatus(false, $"Id is not unique.");
                        data.ClearValidSolve();
                        data.UpdateValidSolve($"Rearrange Id to {suggested}", IdRerangeHandler, suggested);
                        suggested++;
                    }
                }
                else if (pair.Key != expected)
                {
                    Console.WriteLine($"Id is not consecutive.");
                    foreach (var data in pair.Value)
                    {
                        data.UpdateValidStatus(false, $"Id is not consecutive.");
                        data.ClearValidSolve();
                        data.UpdateValidSolve($"Rearrange Id to {suggested}", IdRerangeHandler, suggested);
                        suggested++;
                    }
                }
                else
                {
                    foreach (var data in pair.Value)
                    {
                        if (expected != suggested)
                        {
                            Console.WriteLine($"Id shall be re-arranged.");
                            data.UpdateValidStatus(false, $"Id shall be re-arranged.");
                            data.ClearValidSolve();
                            data.UpdateValidSolve($"Rearrange Id to {suggested}", IdRerangeHandler, suggested);
                        }
                        else
                        {
                            data.UpdateValidStatus(true);
                            data.ClearValidSolve();
                        }
                        expected++;
                        suggested++;
                    }
                }
            }
        }

        private void IdRerangeHandler(EcucData data, object? idSuggest)
        {
            if (idSuggest is Int64 id)
            {
                data.Value = id.ToString();
            }
        }
    }
}
