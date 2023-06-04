﻿/*  
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
using System.ComponentModel;

namespace Ecuc.EcucUi
{
    /// <summary>
    /// TableLayout control for Ecuc Data display.
    /// </summary>
    public class EcucTableLayout : TableLayoutPanel
    {
        /// <summary>
        /// Single data container for display.
        /// </summary>
        public EcucData? Data { get; set; }
        /// <summary>
        /// Multiply data container data bswmd.
        /// </summary>
        public IEcucBswmdBase? Bswmd { get; set; }
        /// <summary>
        /// Multiply data container datas.
        /// </summary>
        public EcucDataList? Datas { get; set; }
        /// <summary>
        /// RichTextBox for bswmd description display.
        /// </summary>
        private readonly RichTextBox rtDesc;
        /// <summary>
        /// TreeNode for dereference lookup.
        /// </summary>
        private TreeNodeCollection? treeNodes;
        /// <summary>
        /// Reference form for reference candidate.
        /// </summary>
        private readonly Form wdReference;
        /// <summary>
        /// Context menu with create, delete and deref menu item.
        /// </summary>
        private readonly ContextMenuStrip cm;
        /// <summary>
        /// Create menu item for add  parameter or reference.
        /// </summary>
        private readonly ToolStripMenuItem cmCreate;
        /// <summary>
        /// Delete menu item for delete parameter or reference.
        /// </summary>
        private readonly ToolStripMenuItem cmDelete;
        /// <summary>
        /// DeRef menu item for reference lookup.
        /// </summary>
        private readonly ToolStripMenuItem cmDeRef;

        /// <summary>
        /// Initialize EcucTableLayout.
        /// </summary>
        /// <param name="desc">RichTextBox for display bswmd description.</param>
        /// <param name="wd">Window for reference candidate display.</param>
        public EcucTableLayout(RichTextBox desc, Form wd)
        {
            // Handle input parameter
            rtDesc = desc;
            wdReference = wd;

            // Initialize basic parameter.
            AutoScroll = true;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.HighlightText;
            ColumnCount = 2;
            ColumnStyles.Add(new ColumnStyle());
            ColumnStyles.Add(new ColumnStyle());
            Dock = DockStyle.Fill;
            Location = new Point(0, 0);
            Margin = new Padding(4, 4, 4, 4);
            Name = "tpValue";
            RowCount = 1;
            RowStyles.Add(new RowStyle());
            Size = new Size(1000, 800);
            TabIndex = 0;
            MouseDown += MouseDownEventHandler;

            // Initialize context menu and its items
            cm = new ContextMenuStrip();
            cmCreate = new ToolStripMenuItem();
            cmDelete = new ToolStripMenuItem();
            cmDeRef = new ToolStripMenuItem();
            cm.VisibleChanged += VisibleEventHandler;
            cm.ImageScalingSize = new Size(20, 20);
            cm.Items.AddRange(new ToolStripItem[] { cmCreate, cmDelete, cmDeRef });
            cm.Name = "cmTableLayout";
            cm.Size = new Size(127, 58);
            cmCreate.Name = "cmTableLayoutCreate";
            cmCreate.Size = new Size(126, 24);
            cmCreate.Text = "Create Parameter";
            cmCreate.Visible = false;
            cmCreate.MouseDown += CmCreateHandler;
            cmDelete.Name = "cmTableLayoutDelete";
            cmDelete.Size = new Size(126, 24);
            cmDelete.Text = "Delete Parameter";
            cmDelete.Visible = false;
            cmDelete.MouseDown += CmDeleteHandler;
            cmDeRef.Name = "cmTableLayoutDeRef";
            cmDeRef.Size = new Size(126, 24);
            cmDeRef.Text = "Find Reference";
            cmDeRef.Visible = false;
            ContextMenuStrip = cm;
        }

        /// <summary>
        /// Update treeNodes parameter.
        /// </summary>
        /// <param name="nodes">TreeNodes add for deref lookup.</param>
        public void AddTreeNodes(TreeNodeCollection nodes)
        {
            treeNodes = nodes;
        }

        /// <summary>
        /// Refresh whole TableLayout from sracth.
        /// </summary>
        public void RefreshUi()
        {
            if (Data != null)
            {
                // Refresh single container layout
                RefreshUi(Data);
            }
            else if (Bswmd != null && Datas != null)
            {
                // Refresh multi container layout
                RefreshUi(Bswmd, Datas);
            }
        }

        /// <summary>
        /// Refresh whole TableLayout from sracth with multiply container datas refreash.
        /// </summary>
        /// <param name="bswmd">Bswmd of multiply container.</param>
        /// <param name="datas">Datas of multiply container.</param>
        public void RefreshUi(IEcucBswmdBase bswmd, EcucDataList datas)
        {
            // Refresh data
            Data = null;
            Bswmd = bswmd;
            Datas = datas;
            // Preparations
            Visible = false;
            Controls.Clear();
            RowCount = 0;

            // Filter TexbBox
            var filterTextBox = new TextBox()
            {
                Width = Width - 50,
                Name = "DATAGRIDFILTER"
            };
            filterTextBox.KeyDown += FilterKeyDownEventHandler;
            Controls.Add(filterTextBox);
            Controls.Add(new Label());
            RowCount++;

            // Container DataGrid
            var list = new EcucMultiplyDataGrid(bswmd, datas)
            {
                Size = new(Size.Width - 50, Size.Height - 50),
                Name = "DATAGRID"
            };
            Controls.Add(list);
            RowCount++;
            Visible = true;
        }

        /// <summary>
        /// Refresh whole TableLayout from sracth with single container data refreash.
        /// </summary>
        /// <param name="data">Data of single container.</param>
        public void RefreshUi(EcucData data)
        {
            // Refresh data
            Bswmd = null;
            Datas = null;
            Data = data;
            Data.PropertyChanged += DataChangeEventHandler;

            // Preparations
            Visible = false;
            Controls.Clear();
            RowCount = 0;

            // No need to display module
            if (Data.InstanceType == typeof(EcucInstanceModule))
            {
                return;
            }

            // Display shortname
            RefreshShortNameUi(Data);

            // Display parameters
            foreach (var para in Data.SortedParas)
            {
                // Get multiply status and skip invalid
                var status = para.Value.GetMultiplyStatus(para.Key);
                if (status.Invalid)
                {
                    continue;
                }

                if (status.Ok && !status.Empty)
                {
                    // Have parameter
                    RefreshParaUi(new TableLayoutPanelElementPara(para.Key, para.Value, true));
                }
                else
                {
                    // Haven't parameter
                    RefreshParaUi(new TableLayoutPanelElementPara(para.Key, para.Value, false));
                }
            }

            // Display references
            foreach (var reference in Data.SortedRefs)
            {
                // Get multiply status and skip invalid
                var status = reference.Value.GetMultiplyStatus(reference.Key);
                if (status.Invalid)
                {
                    continue;
                }

                if (status.Ok && !status.Empty)
                {
                    // Have reference
                    RefreshRefUi(new TableLayoutPanelElementRef(reference.Key, reference.Value, true));
                }
                else
                {
                    // Haven't reference
                    RefreshRefUi(new TableLayoutPanelElementRef(reference.Key, reference.Value, false));
                }
            }
            Visible = true;
        }

        /// <summary>
        /// Refresh to redraw short name control.
        /// </summary>
        /// <param name="data">Data of container</param>
        private void RefreshShortNameUi(EcucData data)
        {
            // Label for short name
            var lb = new Label
            {
                AutoSize = true,
                Text = "ShortName",
                Tag = data,
                Enabled = true,
                Name = $"lb_ShortName"
            };
            Controls.Add(lb);

            // TextBox for short name
            var tb = new TextBox
            {
                Text = data.Value,
                Tag = data,
                Name = "ShortName",
                Enabled = true,
                Width = data.Value.Length * 8 + 20 > Width / 2 ? data.Value.Length * 8 + 20 : Width / 2
            };
            tb.DataBindings.Add(new Binding("Text", data, "Value"));
            Controls.Add(tb);
        }

        /// <summary>
        /// Refresh to redraw parameter control.
        /// </summary>
        /// <param name="ui">TableLayout elements of parameter.</param>
        private void RefreshParaUi(TableLayoutPanelElementPara ui)
        {
            // Label for name of parameter
            var lb = new Label
            {
                AutoSize = true,
                Text = ui.Title,
                Tag = ui,
                Enabled = ui.Exist,
                Name = $"lb_{ui.Title}"
            };
            lb.MouseDown += ChildMouseDownEventHandler;
            Controls.Add(lb);

            // Differnet kind of control for content of parameter
            switch (ui.Type)
            {
                case EcucInstanceParaType.ENUM:
                    {
                        // Parameter of enumeration type
                        if (ui.Multiply == false)
                        {
                            // Signle parameter within container, ComboBox is used
                            var cb = new ComboBox();
                            var maxWords = 0;
                            foreach (var c in ui.Candidate)
                            {
                                cb.Items.Add(c);
                                if (c.Length > maxWords)
                                {
                                    maxWords = c.Length;
                                }
                            }
                            cb.Width = maxWords * 8 + 30 > Width / 2 ? maxWords * 8 + 30 : Width / 2;
                            cb.Text = ui.Text[0];
                            cb.Tag = ui;
                            cb.Name = ui.Title;
                            cb.Enabled = ui.Exist;
                            cb.MouseDown += ChildMouseDownEventHandler;
                            if (ui.Exist == true)
                            {
                                cb.DataBindings.Add(new Binding("Text", ui.Datas[0], "Value"));
                            }
                            Controls.Add(cb);
                        }
                        RowCount++;
                    }
                    break;

                case EcucInstanceParaType.BOOL:
                    {
                        // Parameter of boolean type
                        if (ui.Multiply == false)
                        {
                            // Signle parameter within container, CheckBox is used
                            var cb = new CheckBox();
                            if (ui.Text[0] == "true")
                            {
                                cb.Checked = true;
                            }
                            else
                            {
                                cb.Checked = false;
                            }
                            cb.Tag = ui;
                            cb.Name = ui.Title;
                            cb.Enabled = ui.Exist;
                            cb.MouseDown += ChildMouseDownEventHandler;
                            if (ui.Exist == true)
                            {
                                cb.DataBindings.Add(new Binding("Checked", ui.Datas[0], "Value"));
                            }
                            Controls.Add(cb);
                        }
                        RowCount++;
                    }
                    break;

                case EcucInstanceParaType.TEXT:
                    {
                        // Parameter of text type
                        if (ui.Multiply == false)
                        {
                            // Signle parameter within container, TextBox is used
                            var tb = new TextBox
                            {
                                Tag = ui,
                                Name = ui.Title,
                                Enabled = ui.Exist
                            };
                            tb.MouseDown += ChildMouseDownEventHandler;
                            if (ui.Exist == true)
                            {
                                if (ui.Datas.Count > 0)
                                {
                                    tb.DataBindings.Add(new Binding("Text", ui.Datas[0], "Value"));
                                    tb.Width = ui.Text[0].Length * 8 + 20 > Width / 2 ? ui.Text[0].Length * 8 + 20 : Width / 2;
                                }
                                else
                                {
                                    tb.Width = Width / 2;
                                }
                            }
                            else
                            {
                                tb.Width = Width / 2;
                            }
                            Controls.Add(tb);
                        }
                        else
                        {
                            // Multiply parameters within container, EcucSingleListView is used
                            var lv = new EcucSingleListView(ui, wdReference, this)
                            {
                                View = View.Details,
                                AutoArrange = true,
                                Enabled = ui.Exist,
                                Name = ui.Title
                            };

                            lv.MouseDown += ChildMouseDownEventHandler;
                            Controls.Add(lv);
                        }
                        RowCount++;
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Refresh to redraw reference control.
        /// </summary>
        /// <param name="ui">TableLayout elements of reference.</param>
        private void RefreshRefUi(TableLayoutPanelElementRef ui)
        {
            // Label for name of reference
            Label lb = new()
            {
                AutoSize = true,
                Text = ui.Title,
                Tag = ui,
                Enabled = ui.Exist,
                Name = $"lb_{ui.Title}"
            };
            lb.MouseDown += ChildMouseDownEventHandler;
            Controls.Add(lb);

            // Different control for different type of reference
            if (ui.Multiply == false)
            {
                // Signle parameter within container, TextBox is used
                var tb = new TextBox();
                if (ui.Text.Count > 0)
                {
                    tb.Text = ui.Text[0];
                    tb.Width = ui.Text[0].Length * 8 + 20 > Width / 2 ? ui.Text[0].Length * 8 + 20 : Width / 2;
                }
                else
                {
                    tb.Width = Width / 2;
                }
                tb.Tag = ui;
                tb.Name = ui.Title;
                tb.Enabled = ui.Exist;
                tb.MouseDown += ChildMouseDownEventHandler;
                tb.MouseDoubleClick += RefMouseDoubleClickEventHandler;
                tb.ReadOnly = true;
                Controls.Add(tb);
            }
            else
            {
                // Multiply parameters within container, EcucSingleListView is used
                var lv = new EcucSingleListView(ui, wdReference, this)
                {
                    View = View.Details,
                    AutoArrange = true,
                    Enabled = ui.Exist
                };
                lv.MouseDown += ChildMouseDownEventHandler;
                Controls.Add(lv);
            }
            RowCount++;
        }

        /// <summary>
        /// Update TableLayout within existing layout.
        /// </summary>
        public void UpdateUi()
        {
            if (Data == null)
            {
                return;
            }

            if (Data.InstanceType == typeof(EcucInstanceModule))
            {
                return;
            }

            // Iterate all parameters in container
            foreach (var para in Data.SortedParas)
            {
                // Get multiplicity and skip invalid
                var status = para.Value.GetMultiplyStatus(para.Key);
                if (status.Invalid)
                {
                    continue;
                }

                // Update others
                if (status.Ok && !status.Empty)
                {
                    UpdateParaUi(new TableLayoutPanelElementPara(para.Key, para.Value, true));
                }
                else
                {
                    UpdateParaUi(new TableLayoutPanelElementPara(para.Key, para.Value, false));
                }
            }

            // Iterate all references in container
            foreach (var reference in Data.SortedRefs)
            {
                // Get multiplicity and skip invalid
                var status = reference.Value.GetMultiplyStatus(reference.Key);
                if (status.Invalid)
                {
                    continue;
                }

                // Update reference
                if (status.Ok && !status.Empty)
                {
                    UpdateRefUi(new TableLayoutPanelElementRef(reference.Key, reference.Value, true));
                }
                else
                {
                    UpdateRefUi(new TableLayoutPanelElementRef(reference.Key, reference.Value, false));
                }
            }
        }

        /// <summary>
        /// Update parameter ui.
        /// </summary>
        /// <param name="ui">Paremter ui data.</param>
        /// <exception cref="Exception">Can not find related Label.</exception>
        private void UpdateParaUi(TableLayoutPanelElementPara ui)
        {
            // Find label
            var ctrl = Controls[$"lb_{ui.Title}"];
            if (ctrl != null)
            {
                // Update label
                ctrl.Tag = ui;
                ctrl.Enabled = ui.Exist;
            }
            else
            {
                throw new Exception($"Can not find label {ui.Title}");
            }

            // Find content control
            ctrl = Controls[ui.Title];
            if (ctrl != null)
            {
                // Update content control according to different type
                ctrl.Tag = ui;
                switch (ui.Type)
                {
                    case EcucInstanceParaType.ENUM:
                        {
                            if (ui.Multiply == false)
                            {
                                ctrl.Enabled = ui.Exist;
                                ctrl.DataBindings.Clear();
                                if (ui.Exist == true)
                                {
                                    if (ui.Datas.Count >= 1)
                                    {
                                        ctrl.DataBindings.Add(new Binding("Text", ui.Datas[0], "Value"));
                                    }
                                }
                                else
                                {
                                    ctrl.Text = "";
                                }
                            }
                        }
                        break;

                    case EcucInstanceParaType.BOOL:
                        {
                            if (ui.Multiply == false)
                            {
                                ctrl.Enabled = ui.Exist;
                                ctrl.DataBindings.Clear();
                                if (ui.Exist == true)
                                {
                                    ctrl.DataBindings.Add(new Binding("Checked", ui.Datas[0], "Value"));
                                }
                                else
                                {
                                    ctrl.Text = "";
                                }
                            }
                        }
                        break;

                    case EcucInstanceParaType.TEXT:
                        {
                            if (ui.Multiply == false)
                            {
                                ctrl.Enabled = ui.Exist;
                                ctrl.DataBindings.Clear();
                                if (ui.Exist == true)
                                {
                                    ctrl.DataBindings.Add(new Binding("Text", ui.Datas[0], "Value"));
                                }
                                else
                                {
                                    ctrl.Text = "";
                                }
                            }
                            else
                            {
                                ctrl.Enabled = ui.Exist;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else
            {
                throw new Exception($"Can not find label {ui.Title}");
            }
        }

        /// <summary>
        /// Data changed handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void DataChangeEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            // Update ui when data changed
            UpdateUi();
        }

        /// <summary>
        /// Updata reference ui
        /// </summary>
        /// <param name="ui">Reference ui data.</param>
        /// <exception cref="Exception">Can not find related Label.</exception>
        private void UpdateRefUi(TableLayoutPanelElementRef ui)
        {
            // Find label
            var ctrl = Controls[$"lb_{ui.Title}"];
            if (ctrl != null)
            {
                // Update label
                ctrl.Tag = ui;
                ctrl.Enabled = ui.Exist;
            }
            else
            {
                throw new Exception($"Can not find label {ui.Title}");
            }

            // Find content control
            ctrl = Controls[ui.Title];
            if (ctrl != null)
            {
                // Update content control
                ctrl.Tag = ui;
                if (ui.Multiply == false)
                {
                    if (ui.Text.Count > 0)
                    {
                        ctrl.Text = ui.Text[0];
                    }
                    else
                    {
                        ctrl.Text = "";
                    }

                    ctrl.Enabled = ui.Exist;
                    if (ui.Exist == false)
                    {
                        ctrl.Text = "";
                    }
                }
                else
                {
                    ctrl.Enabled = ui.Exist;
                    if (ctrl is EcucSingleListView ctrlList)
                    {
                        ctrlList.RefreshUi(ui);
                    }
                }
            }
        }

        /// <summary>
        /// Mouse click handler.
        /// </summary>
        /// <param name="sender">Clicked control.</param>
        /// <param name="e">Mouse event.</param>
        private void ChildMouseDownEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is Control ctrl)
            {
                if (ctrl.Tag is IEcucTableLayoutPanelElement element)
                {
                    var bswmd = element.Bswmd;

                    // Prepare meta infomation of bswmd
                    rtDesc.Clear();
                    if (bswmd == null)
                    {
                        return;
                    }
                    rtDesc.Text += $"Name: {bswmd.ShortName}{Environment.NewLine}";
                    rtDesc.Text += $"Description: {bswmd.Desc}{Environment.NewLine}";
                    rtDesc.Text += $"Trace: {bswmd.Trace}{Environment.NewLine}";
                    rtDesc.Text += $"Lower Multiplicity: {bswmd.Lower}{Environment.NewLine}";
                    if (bswmd.Upper == int.MaxValue)
                    {
                        rtDesc.Text += $"Upper Multiplicity: Inf{Environment.NewLine}";
                    }
                    else
                    {
                        rtDesc.Text += $"Upper Multiplicity: {bswmd.Upper}{Environment.NewLine}";
                    }
                    rtDesc.Text += $"BSWMD Path: {bswmd.AsrPath}{Environment.NewLine}";
                    
                    // Prepare create and delete popup menu item
                    if (element.Exist == true)
                    {
                        cmCreate.Visible = false;
                        if (element.Bswmd.IsRequired == true)
                        {
                            cmDelete.Visible = false;
                        }
                        else
                        {
                            cmDelete.Visible = true;
                        }
                        cmDelete.Tag = element;
                    }
                    else
                    {
                        cmCreate.Visible = true;
                        cmDelete.Visible = false;
                        cmCreate.Tag = element;
                    }

                    // Prepare de-reference popup menu item
                    if (element is TableLayoutPanelElementRef elementRef)
                    {
                        cmDeRef.Visible = true;
                        cmDeRef.DropDownItems.Clear();
                        foreach (var data in elementRef.Datas)
                        {
                            var item = cmDeRef.DropDownItems.Add(data.Value);
                            item.Tag = data;
                            item.MouseDown += CmDeRefItemClickEventHandler;
                        }
                    }
                    else
                    {
                        cmDeRef.Visible = false;
                        cmDeRef.DropDownItems.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// De-reference menu item click handler.
        /// </summary>
        /// <param name="sender">Clicked de-reference menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmDeRefItemClickEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                if (toolStripItem.Tag is EcucData data)
                {
                    try
                    {
                        // De-reference and select it
                        var deref = data.DeRef();
                        if (treeNodes != null)
                        {
                            ExpandAllNodes(treeNodes, deref.AsrPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Expand all parent of node with name "name".
        /// </summary>
        /// <param name="nodes">Start nodes.</param>
        /// <param name="name">Name of node expanded.</param>
        /// <returns>
        ///     true: Expaned node is child of this node.
        ///     false: Expaned node is not child of this node.
        /// </returns>
        private bool ExpandAllNodes(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
            {
                if (name.StartsWith(node.Name) == true || node is EcucContainersTreeNode)
                {
                    if (name == node.Name)
                    {
                        node.TreeView.SelectedNode = node;
                        node.Parent?.Expand();
                        return true;
                    }
                    else
                    {
                        if (ExpandAllNodes(node.Nodes, name) == true)
                        {
                            node.Parent?.Expand();
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Mouse double click handler of reference textbox.
        /// </summary>
        /// <param name="sender">Textbox clicked.</param>
        /// <param name="e">Mouse event.</param>
        private void RefMouseDoubleClickEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.Tag is IEcucTableLayoutPanelElement element)
                {
                    if (element.Datas.Count > 0)
                    {
                        // Have data and prepare data for reference form
                        wdReference.Tag = new KeyValuePair<IEcucBswmdBase, EcucDataList>(element.Bswmd, element.Datas);
                        wdReference.ShowDialog();
                        // Get result from reference form and update ui
                        if (wdReference.Tag is Dictionary<string, List<string>> referenceDict)
                        {
                            if (Data != null)
                            {
                                Data[element.Title].First.Value = referenceDict.First().Value.First();
                                UpdateUi();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create menu item handler.
        /// </summary>
        /// <param name="sender">Create menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmCreateHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is IEcucTableLayoutPanelElement element)
                {
                    if (Data != null)
                    {
                        if (element is TableLayoutPanelElementPara)
                        {
                            // Add parameter
                            Data.AddPara(element.Bswmd.AsrPathShort);
                        }
                        else
                        {
                            // Add reference
                            wdReference.Tag = new KeyValuePair<IEcucBswmdBase, EcucDataList>(element.Bswmd, element.Datas);
                            wdReference.ShowDialog();
                            if (wdReference.Tag is Dictionary<string, List<string>> referenceDict)
                            {
                                Data.AddRef(element.Bswmd.AsrPathShort, referenceDict.First().Value.First(), referenceDict.First().Key);
                            }
                        }
                        // Update ui
                        UpdateUi();
                    }
                }
            }
        }

        /// <summary>
        /// Delete menu item handler.
        /// </summary>
        /// <param name="sender">Delete menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmDeleteHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is IEcucTableLayoutPanelElement element)
                {
                    if (Data != null)
                    {
                        if (element is TableLayoutPanelElementPara)
                        {
                            // Delete parameter
                            Data.DelPara(element.Bswmd.AsrPathShort);
                        }
                        else
                        {
                            // Delete reference
                            Data.DelRef(element.Bswmd.AsrPathShort);
                        }
                        // Update ui
                        UpdateUi();
                    }
                }
            }
        }

        /// <summary>
        /// EcucTableLayout mouse click event handler.
        /// </summary>
        /// <param name="sender">EcucTableLayout.</param>
        /// <param name="e">Mouse event.</param>
        private void MouseDownEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is EcucTableLayout)
            {
                // Get selected control
                var controlSeleted = GetChildAtPoint(e.Location);
                if (controlSeleted == null)
                {
                    cmCreate.Visible = false;
                    cmDelete.Visible = false;
                    cmDeRef.Visible = false;
                    return;
                }

                if (controlSeleted.Tag is IEcucTableLayoutPanelElement element)
                {
                    var bswmd = element.Bswmd;

                    // Prepare meta infomation of bswmd
                    rtDesc.Clear();
                    if (bswmd == null)
                    {
                        return;
                    }
                    rtDesc.Text += $"Name: {bswmd.ShortName}{Environment.NewLine}";
                    rtDesc.Text += $"Description: {bswmd.Desc}{Environment.NewLine}";
                    rtDesc.Text += $"Trace: {bswmd.Trace}{Environment.NewLine}";
                    rtDesc.Text += $"Lower Multiplicity: {bswmd.Lower}{Environment.NewLine}";
                    if (bswmd.Upper == int.MaxValue)
                    {
                        rtDesc.Text += $"Upper Multiplicity: Inf{Environment.NewLine}";
                    }
                    else
                    {
                        rtDesc.Text += $"Upper Multiplicity: {bswmd.Upper}{Environment.NewLine}";
                    }
                    rtDesc.Text += $"BSWMD Path: {bswmd.AsrPath}{Environment.NewLine}";

                    // Prapare create, delete and de-reference menu item
                    if (element.Exist == true)
                    {
                        cmCreate.Visible = false;
                        if (element.Bswmd.IsRequired == true)
                        {
                            cmDelete.Visible = false;
                        }
                        else
                        {
                            cmDelete.Visible = true;
                        }
                        cmDelete.Tag = element;
                    }
                    else
                    {
                        cmCreate.Visible = true;
                        cmDelete.Visible = false;
                        cmDeRef.Visible = false;
                        cmCreate.Tag = element;
                    }
                }
            }
        }
        
        /// <summary>
        /// Popup menu trigger event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibleEventHandler(object? sender, EventArgs e)
        {
            if (sender is Control ctrl)
            {
                if (Focused == false)
                {
                    ctrl.Visible = false;
                }
            }
        }

        /// <summary>
        /// Filter textbox key down event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Key event.</param>
        private void FilterKeyDownEventHandler(object? sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Enter is pressed, change filter
                    Control? ctrlGrid = Controls["DATAGRID"];
                    Control? ctrlText = Controls["DATAGRIDFILTER"];
                    if (ctrlGrid == null || ctrlText == null)
                    {
                        return;
                    }

                    if (ctrlGrid is EcucMultiplyDataGrid grid && ctrlText is TextBox tb)
                    {
                        grid.RefreshUi(tb.Text);
                    }
                }
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// Paramter value type.
    /// </summary>
    public enum EcucInstanceParaType
    {
        /// <summary>
        /// Text type with string value.
        /// </summary>
        TEXT = 0,
        /// <summary>
        /// Enumeration type with several candidates.
        /// </summary>
        ENUM,
        /// <summary>
        /// Boolean type with true or false.
        /// </summary>
        BOOL
    }

    /// <summary>
    /// Interface of ui element on EcucTableLayout
    /// </summary>
    public interface IEcucTableLayoutPanelElement
    {
        /// <summary>
        /// Ecuc datas of element.
        /// </summary>
        EcucDataList Datas { get; }
        /// <summary>
        /// Ecuc bsmwd of Datas.
        /// </summary>
        IEcucBswmdBase Bswmd { get; }
        /// <summary>
        /// Exist status of element.
        /// </summary>
        bool Exist { get; set; }
        /// <summary>
        /// Title of element.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Text of element.
        /// </summary>
        public List<string> Text { get; }
        /// <summary>
        /// Short form of Text.
        /// </summary>
        public List<string> TextShort { get; }
    }

    /// <summary>
    /// Ui element of parameter.
    /// </summary>
    public class TableLayoutPanelElementPara : IEcucTableLayoutPanelElement
    {
        /// <summary>
        /// Ecuc datas of element.
        /// </summary>
        public EcucDataList Datas { get; } = new();
        /// <summary>
        /// Exist status of element.
        /// </summary>
        public IEcucBswmdBase Bswmd { get; }
        /// <summary>
        /// Exist status of element.
        /// </summary>
        public bool Exist { get; set; } = false;

        /// <summary>
        /// Multiplicity of bswmd.
        /// </summary>
        public bool Multiply
        {
            get
            {
                return !Bswmd.IsSingle;
            }
        }

        /// <summary>
        /// Title of element.
        /// </summary>
        public string Title
        {
            get
            {
                return Bswmd.AsrPathShort;
            }
        }

        /// <summary>
        /// Candidate of element. Only valid for enumeration type.
        /// </summary>
        public List<string> Candidate
        {
            get
            {
                List<string> result = Bswmd switch
                {
                    EcucBswmdEnumerationPara bswmdPara => bswmdPara.Candidate,
                    _ => throw new NotImplementedException(),
                };
                return result;
            }
        }

        /// <summary>
        /// Convert bswmd parameter type to ui type.
        /// </summary>
        public EcucInstanceParaType Type
        {
            get
            {
                return Bswmd switch
                {
                    EcucBswmdEnumerationPara => EcucInstanceParaType.ENUM,
                    EcucBswmdIntegerPara => EcucInstanceParaType.TEXT,
                    EcucBswmdBooleanPara => EcucInstanceParaType.BOOL,
                    EcucBswmdFloatPara => EcucInstanceParaType.TEXT,
                    EcucBswmdStringPara => EcucInstanceParaType.TEXT,
                    EcucBswmdFunctionNamePara => EcucInstanceParaType.TEXT,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        /// <summary>
        /// Text of element accoding to bswmd type.
        /// </summary>
        public List<string> Text
        {
            get
            {
                switch (Bswmd)
                {
                    case EcucBswmdEnumerationPara bswmdEnum:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    result.Add(bswmdEnum.Default);
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            return result;
                        }

                    case EcucBswmdIntegerPara bswmdInt:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    if (bswmdInt.Format == "HEX")
                                    {
                                        result.Add(string.Format("0x{0:X}", bswmdInt.Default));
                                    }
                                    else
                                    {
                                        result.Add(string.Format("{0}", bswmdInt.Default));
                                    }
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            else
                            {
                                foreach (var data in Datas)
                                {
                                    if (bswmdInt.Format == "HEX")
                                    {
                                        result.Add(string.Format("0x{0:X}", Int64.Parse(data.Value)));
                                    }
                                    else
                                    {
                                        result.Add(string.Format("{0}", Int64.Parse(data.Value)));
                                    }
                                }
                            }
                            return result;
                        }

                    case EcucBswmdBooleanPara bswmdBool:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    result.Add(bswmdBool.Default.ToString());
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            else
                            {
                                foreach (var data in Datas)
                                {
                                    result.Add(data.Value);
                                }
                            }
                            return result;
                        }

                    case EcucBswmdFloatPara bswmdFloat:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    result.Add(string.Format("{0:F}", bswmdFloat.Default));
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            else
                            {
                                foreach (var data in Datas)
                                {
                                    result.Add(data.Value);
                                }
                            }
                            return result;
                        }

                    case EcucBswmdStringPara bswmdString:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    result = bswmdString.Default;
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            else
                            {
                                foreach (var data in Datas)
                                {
                                    result.Add(data.Value);
                                }
                            }
                            return result;
                        }

                    case EcucBswmdFunctionNamePara bswmdFunctionName:
                        {
                            var result = new List<string>();

                            if (Multiply == false)
                            {
                                if (Datas.Count == 0)
                                {
                                    result = bswmdFunctionName.Default;
                                }
                                else
                                {
                                    result.Add(Datas[0].Value);
                                }
                            }
                            else
                            {
                                foreach (var data in Datas)
                                {
                                    result.Add(data.Value);
                                }
                            }
                            return result;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Short form of Text.
        /// </summary>
        public List<string> TextShort
        {
            get
            {
                return Text;
            }
        }

        /// <summary>
        /// Initialize TableLayoutPanelElementPara.
        /// </summary>
        /// <param name="bswmd">Ecuc bswmd of element.</param>
        /// <param name="datas">Ecuc datas of element.</param>
        /// <param name="exist">Exist status of element.</param>
        public TableLayoutPanelElementPara(IEcucBswmdBase bswmd, EcucDataList datas, bool exist)
        {
            // Handle input
            Datas = datas;
            Bswmd = bswmd;
            Exist = exist;
        }
    }

    /// <summary>
    /// Ui element of reference.
    /// </summary>
    public class TableLayoutPanelElementRef : IEcucTableLayoutPanelElement
    {
        /// <summary>
        /// Ecuc datas of element.
        /// </summary>
        public EcucDataList Datas { get; } = new();
        /// <summary>
        /// Ecuc bswmd of element.
        /// </summary>
        public IEcucBswmdBase Bswmd { get; }
        /// <summary>
        /// Exist status of element.
        /// </summary>
        public bool Exist { get; set; } = false;

        /// <summary>
        /// Multiplicity of bswmd.
        /// </summary>
        public bool Multiply
        {
            get
            {
                return !Bswmd.IsSingle;
            }
        }

        /// <summary>
        /// Title of element.
        /// </summary>
        public string Title
        {
            get
            {
                return Bswmd.AsrPathShort;
            }
        }

        /// <summary>
        /// Text of element.
        /// </summary>
        public List<string> Text
        {
            get
            {
                var result = new List<string>();

                if (Multiply == false)
                {
                    if (Datas.Count > 0)
                    {
                        result.Add(Datas[0].Value);
                    }
                }
                else
                {
                    foreach (var data in Datas)
                    {
                        result.Add(data.Value);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Short form of Text.
        /// </summary>
        public List<string> TextShort
        {
            get
            {
                var result = new List<string>();

                foreach (var text in Text)
                {
                    var parts = text.Split('/');
                    if (parts.Length > 0)
                    {
                        result.Add(parts[^1]);
                    }
                    else
                    {
                        result.Add("");
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Initialize TableLayoutPanelElementRef.
        /// </summary>
        /// <param name="bswmd">Ecuc bswmd of element.</param>
        /// <param name="datas">Ecuc datas of element.</param>
        /// <param name="exist">Exist status of element.</param>
        public TableLayoutPanelElementRef(IEcucBswmdReference bswmd, EcucDataList datas, bool exist)
        {
            Datas = datas;
            Bswmd = bswmd;
            Exist = exist;
        }
    }
}
