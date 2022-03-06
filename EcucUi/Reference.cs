/*  
 *  This file is a part of Autosar Configurator for ECU GUI based 
 *  configuration, checking and code generation.
 *  
 *  Copyright (C) 2021-2022 Dai Jin Shi
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
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace AutosarConfigurator
{
    partial class Reference : Form
    {
        EcucInstanceManager Manager { get; }

        public Reference(EcucInstanceManager manager)
        {
            Manager = manager;
            InitializeComponent();
            tbFilter.TextChanged += FilterTextChanged;
        }

        private void LvCandidate_VisibleChanged(object sender, EventArgs e)
        {
            tbFilter.Text = "";
            lvCandidate.Items.Clear();
            lvCandidate.Columns.Clear();

            if (Visible == false)
            {
                return;
            }

            PrepareCandidate();
        }

        private void ListViewCandidateDoubleClickEventHandler(object sender, EventArgs e)
        {
            var output = new Dictionary<string, List<string>>();

            if (sender is ListView lv)
            {
                foreach (ListViewItem selectItem in lv.SelectedItems)
                {
                    if (selectItem.Tag is IEcucInstanceModule instance)
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

        private void PrepareCandidate(string filter="")
        {
            lvCandidate.Items.Clear();
            lvCandidate.Columns.Clear();

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
                    if (candidate is IEcucInstanceModule candidateModule)
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

        private void FilterTextChanged(object sender, EventArgs e)
        {
            PrepareCandidate(tbFilter.Text);
        }
    }
}
