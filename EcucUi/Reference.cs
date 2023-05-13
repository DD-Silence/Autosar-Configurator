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

using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EInstance;

namespace AutosarConfigurator
{
    /// <summary>
    /// Reference form
    /// </summary>
    partial class Reference : Form
    {
        /// <summary>
        /// Data souce of instance manager.
        /// </summary>
        EcucInstanceManager Manager { get; }

        /// <summary>
        /// Initialize Reference.
        /// </summary>
        /// <param name="manager">Data souce of instance manager.</param>
        public Reference(EcucInstanceManager manager)
        {
            // Handle input
            Manager = manager;
            // Prepare control
            InitializeComponent();
            tbFilter.TextChanged += FilterTextChanged;
        }

        /// <summary>
        /// Form visibility changed event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void LvCandidate_VisibleChanged(object sender, EventArgs e)
        {
            // Clear all existing ui
            tbFilter.Text = "";
            lvCandidate.Items.Clear();
            lvCandidate.Columns.Clear();

            if (Visible == false)
            {
                return;
            }
            // Construct new ui when visible is true
            PrepareCandidate();
        }

        /// <summary>
        /// Double click event handler
        /// </summary>
        /// <param name="sender">Listview of candidates.</param>
        /// <param name="e">Not used.</param>
        private void ListViewCandidateDoubleClickEventHandler(object? sender, EventArgs e)
        {
            var output = new Dictionary<string, List<string>>();

            if (sender is ListView lv)
            {
                // Record selected items to tag and close form
                foreach (ListViewItem selectItem in lv.SelectedItems)
                {
                    if (selectItem.Tag is IEcucInstanceHasContainer instance)
                    {
                        if (output.ContainsKey(instance.BswmdPathShort) == false)
                        {
                            output[instance.BswmdPathShort] = new List<string>();
                        }
                        output[instance.BswmdPathShort].Add(instance.AsrPath);
                    }
                }
                Tag = output;
                Visible = false;
            }
        }

        /// <summary>
        /// Prepare candidate according to filter.
        /// </summary>
        /// <param name="filter">Filter expression string.</param>
        private void PrepareCandidate(string filter="")
        {
            // Clear all existing ui
            lvCandidate.Items.Clear();
            lvCandidate.Columns.Clear();

            try
            {
                // Prepare ui
                if (Tag is KeyValuePair<IEcucBswmdBase, EcucDataList> pair)
                {
                    List<IEcucInstanceBase> candidates = new();

                    switch (pair.Key)
                    {
                        case EcucBswmdRef:
                            if (pair.Key is EcucBswmdRef bswmdRef)
                            {
                                var result = Manager.GetInstancesByBswmdPath(bswmdRef.DestPath);
                                if (result.Count > 0)
                                {
                                    candidates.AddRange(result);
                                }
                                else
                                {
                                    var bswmd = bswmdRef.GetBswmdFromBswmdPath(bswmdRef.DestPath);
                                    candidates.AddRange(Manager.GetInstancesByBswmdPath(bswmd.AsrPath));
                                }
                            }
                            break;

                        case EcucBswmdForeignRef:
                            if (pair.Key is EcucBswmdForeignRef bswmdForeignRef)
                            {
                                if (bswmdForeignRef.DestType == "ECUC-MODULE-CONFIGURATION-VALUES")
                                {
                                    candidates.AddRange(Manager.EcucModules);
                                }
                            }
                            break;

                        case EcucBswmdSymbolicNameRef:
                            if (pair.Key is EcucBswmdSymbolicNameRef bswmdSymbolicRef)
                            {
                                var result = Manager.GetInstancesByBswmdPath(bswmdSymbolicRef.DestPath);
                                if (result.Count > 0)
                                {
                                    candidates.AddRange(result);
                                }
                                else
                                {
                                    var bswmd = bswmdSymbolicRef.GetBswmdFromBswmdPath(bswmdSymbolicRef.DestPath);
                                    candidates.AddRange(Manager.GetInstancesByBswmdPath(bswmd.AsrPath));
                                }
                            }
                            break;

                        case EcucBswmdChoiceRef:
                            if (pair.Key is EcucBswmdChoiceRef bswmdChoiceRef)
                            {
                                foreach (var choice in bswmdChoiceRef.Choices)
                                {
                                    var result = Manager.GetInstancesByBswmdPath(choice.Key);
                                    if (result.Count > 0)
                                    {
                                        candidates.AddRange(result);
                                    }
                                    else
                                    {
                                        var bswmd = bswmdChoiceRef.GetBswmdFromBswmdPath(choice.Key);
                                        candidates.AddRange(Manager.GetInstancesByBswmdPath(bswmd.AsrPath));
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    var c = lvCandidate.Columns.Add(pair.Key.ShortName);
                    foreach (var candidate in candidates)
                    {
                        if (candidate is IEcucInstanceHasContainer candidateModule)
                        {
                            bool findExist = false;
                            foreach (var pathExist in pair.Value)
                            {
                                if (pathExist.Value == candidateModule.AsrPath)
                                {
                                    findExist = true;
                                    break;
                                }
                            }

                            if (findExist == false)
                            {
                                if (filter != "")
                                {
                                    if (candidateModule.AsrPathShort.Contains(filter) == false)
                                    {
                                        continue;
                                    }
                                }
                                var item = lvCandidate.Items.Add(candidateModule.AsrPath);
                                item.Tag = candidate;
                            }

                        }
                    }

                    c.Width = Width / 2;
                    lvCandidate.DoubleClick += ListViewCandidateDoubleClickEventHandler;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Filter text changed event handler.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void FilterTextChanged(object? sender, EventArgs e)
        {
            PrepareCandidate(tbFilter.Text);
        }
    }
}
