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

using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EInstance;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ecuc.EcucBase.EBase
{
    /// <summary>
    /// INotifyPropertyChanged abstaract to avoid type property name
    /// </summary>
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Ecuc solve action handler delegate to invalid data.
    /// </summary>
    /// <param name="data">Ecuc data to solve invalid.</param>
    /// <param name="param"></param>
    /// 
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
        /// Valid infomation.
        /// </summary>
        public string Info
        {
            get
            {
                return Instance switch
                {
                    IEcucInstanceHasContainer module => $"{module.AsrPath}: {InvalidReason}",
                    IEcucInstanceParameterBase param => $"{param.Parent.BswmdPathShort}: {InvalidReason}",
                    IEcucInstanceReferenceBase reference => $"{reference.Parent.BswmdPathShort}: {InvalidReason}",
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

    /// <summary>
    /// 
    /// </summary>
    public class EcucIdRangeCheck
    {
        readonly SortedDictionary<Int64, List<EcucData>> IdDict = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
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
    }
}
