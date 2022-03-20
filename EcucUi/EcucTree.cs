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
using Ecuc.EcucBase.EData;
using Ecuc.EcucBase.EInstance;
using System.ComponentModel;

namespace Ecuc.EcucUi
{
    /// <summary>
    /// Ecuc TreeView for model.
    /// </summary>
    public class EcucModelTreeView : TreeView
    {
        /// <summary>
        /// Data source of instance manager.
        /// </summary>
        public EcucInstanceManager InstanceManager { get; }
        /// <summary>
        /// Data source of bswmd manager.
        /// </summary>
        public EcucBswmdManager BswmdManager { get; }

        /// <summary>
        /// RichTextBox to display meta information.
        /// </summary>
        private readonly RichTextBox richBox;
        /// <summary>
        /// EcucTableLayout to display parameter information.
        /// </summary>
        private readonly EcucTableLayout tableLayout;
        /// <summary>
        /// Popup menu.
        /// </summary>
        private readonly ContextMenuStrip cm;
        /// <summary>
        /// Add menu item.
        /// </summary>
        private readonly ToolStripMenuItem tmAdd;
        /// <summary>
        /// Delete menu item.
        /// </summary>
        private readonly ToolStripMenuItem tmDelete;
        /// <summary>
        /// Usage menu item.
        /// </summary>
        private readonly ToolStripMenuItem tmUsage;

        /// <summary>
        /// Initialize EcucModelTreeView.
        /// </summary>
        /// <param name="instanceManager">Data source of instance manager.</param>
        /// <param name="bswmdManager">Data source of bswmd manager.</param>
        /// <param name="rb">RichTextBox to display meta information.</param>
        /// <param name="panel">EcucTableLayout to display parameter information.</param>
        public EcucModelTreeView(EcucInstanceManager instanceManager, EcucBswmdManager bswmdManager, RichTextBox rb, EcucTableLayout panel)
        {
            // Handle input
            richBox = rb;
            tableLayout = panel;
            InstanceManager = instanceManager;
            BswmdManager = bswmdManager;

            // Prepare control
            Dock = DockStyle.Fill;
            Location = new Point(0, 0);
            Margin = new Padding(4);
            Name = "tvModel";
            Size = new Size(450, 850);
            TabIndex = 0;
            AfterSelect += AfterSelectEventHandler;
            MouseClick += MouseClickEventHandler;

            cm = new ContextMenuStrip();
            tmAdd = new ToolStripMenuItem();
            tmDelete = new ToolStripMenuItem();
            tmUsage = new ToolStripMenuItem();

            cm.ImageScalingSize = new Size(20, 20);
            cm.Items.AddRange(new ToolStripItem[] { tmAdd, tmDelete, tmUsage });
            cm.Name = "cm";
            cm.Size = new Size(127, 58);

            tmAdd.Name = "tmAdd";
            tmAdd.Size = new Size(126, 24);
            tmAdd.Text = "Add";
            tmAdd.Visible = false;

            tmDelete.Name = "tmDelete";
            tmDelete.Size = new Size(126, 24);
            tmDelete.Text = "Delete";

            tmUsage.Name = "tmUsage";
            tmUsage.Size = new Size(126, 24);
            tmUsage.Text = "Usage";

            tmDelete.MouseDown += CmTreeViewDeleteHandler;

            ContextMenuStrip = cm;

            ShowNodeToolTips = true;

            // Refresh ui
            RefreshUi();
        }

        /// <summary>
        /// Refresh ui.
        /// </summary>
        public void RefreshUi()
        {
            BeginUpdate();
            // Clear existing ui
            Nodes.Clear();
            // Construct new ui
            var nodeRoot = Nodes.Add("Ecu Configuration");
            foreach (var module in InstanceManager.EcucModules)
            {
                var ecucData = new EcucData(module, BswmdManager.GetBswmdFromBswmdPath(module.BswmdPath));
                EcucContainerTreeNode node = new(ecucData);
                nodeRoot.Nodes.Add(node);
            }
            nodeRoot.Expand();
            SelectedNode = nodeRoot;
            EndUpdate();
        }

        /// <summary>
        /// Node select event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void AfterSelectEventHandler(object? sender, TreeViewEventArgs e)
        {
            var selectedNode = SelectedNode;
            if (selectedNode == null)
            {
                return;
            }

            switch (selectedNode)
            {
                case EcucContainerTreeNode containerNode:
                    // Update parameter layout with single container
                    tableLayout.RefreshUi(containerNode.Data);
                    // Update meta richbox
                    richBox.Clear();
                    richBox.Text += $"Name: {containerNode.Data.BswmdPathShort}{Environment.NewLine}";
                    richBox.Text += $"Description: {containerNode.Data.Desc}{Environment.NewLine}";
                    richBox.Text += $"Trace: {containerNode.Data.Trace}{Environment.NewLine}";
                    richBox.Text += $"Lower Multiplicity: {containerNode.Data.Lower}{Environment.NewLine}";
                    if (containerNode.Data.Upper == uint.MaxValue)
                    {
                        richBox.Text += $"Upper Multiplicity: n{Environment.NewLine}";
                    }
                    else
                    {
                        richBox.Text += $"Upper Multiplicity: {containerNode.Data.Upper}{Environment.NewLine}";
                    }
                    richBox.Text += $"BSWMD Path: {containerNode.Data.BswmdPath}{Environment.NewLine}";
                    break;

                case EcucContainersTreeNode containersNode:
                    // Update parameter layout with multiply containers
                    tableLayout.RefreshUi(containersNode.Bswmd, containersNode.Datas);
                    richBox.Clear();
                    // Update meta richbox
                    richBox.Text += $"Name: {containersNode.Bswmd.ShortName}{Environment.NewLine}";
                    richBox.Text += $"Description: {containersNode.Bswmd.Desc}{Environment.NewLine}";
                    richBox.Text += $"Trace: {containersNode.Bswmd.Trace}{Environment.NewLine}";
                    richBox.Text += $"Lower Multiplicity: {containersNode.Bswmd.Lower}{Environment.NewLine}";
                    if (containersNode.Bswmd.Upper == uint.MaxValue)
                    {
                        richBox.Text += $"Upper Multiplicity: n{Environment.NewLine}";
                    }
                    else
                    {
                        richBox.Text += $"Upper Multiplicity: {containersNode.Bswmd.Upper}{Environment.NewLine}";
                    }
                    richBox.Text += $"BSWMD Path: {containersNode.Bswmd.AsrPath}{Environment.NewLine}";
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Mouse click event handler.
        /// </summary>
        /// <param name="sender">EcucModelTreeView</param>
        /// <param name="e">Mouse event.</param>
        private void MouseClickEventHandler(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender is TreeView tv)
                {
                    // Right clich shall popup menu
                    var node = tv.GetNodeAt(e.Location);
                    if (node != null)
                    {
                        tv.SelectedNode = node;
                    }

                    // Prepare menu item.
                    tmDelete.Visible = false;
                    tmAdd.DropDownItems.Clear();
                    tmUsage.DropDownItems.Clear();
                    switch (tv.SelectedNode)
                    {
                        case EcucContainerTreeNode containerNode:
                            if (containerNode.Data.BswmdType == typeof(EcucBswmdChoiceContainer))
                            {
                                if (containerNode.Data.GetMultiplyStatus("").CanAdd == false)
                                {
                                    return;
                                }
                            }

                            foreach (var bswmdContainer in containerNode.Data.BswmdContainers)
                            {
                                if (containerNode.Data.GetMultiplyStatus(bswmdContainer.AsrPath).CanAdd == true)
                                {
                                    var addedItem = tmAdd.DropDownItems.Add(bswmdContainer.AsrPathShort);
                                    addedItem.Tag = containerNode;
                                    addedItem.Name = bswmdContainer.AsrPathShort;
                                    addedItem.MouseDown += CmTreeViewAddHandler;
                                }
                            }
                            tmDelete.Visible = true;
                            tmDelete.Tag = containerNode;

                            foreach (var u in containerNode.Data.Usage)
                            {
                                var item = tmUsage.DropDownItems.Add(u.Parent.ShortName);
                                item.Tag = u.Parent;
                                item.MouseDown += CmTreeViewUsageHandler;
                            }
                            break;

                        case EcucContainersTreeNode:
                            break;

                        default:
                            break;
                    }

                    if (tmAdd.DropDownItems.Count > 0)
                    {
                        tmAdd.Visible = true;
                    }
                    else
                    {
                        tmAdd.Visible = false;
                    }

                    if (tmUsage.DropDownItems.Count > 0)
                    {
                        tmUsage.Visible = true;
                    }
                    else
                    {
                        tmUsage.Visible = false;
                    }

                    // Display popup
                    cm.Visible = true;
                }
            }
        }

        /// <summary>
        /// Add menu item click event handler.
        /// </summary>
        /// <param name="sender">Add menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmTreeViewAddHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag == null)
                {
                    return;
                }

                switch (item.Tag)
                {
                    case EcucContainerTreeNode containerNode:
                        // Add container
                        BeginUpdate();
                        containerNode.AddContainer(item.Text);
                        EndUpdate();
                        break;

                    default:
                        break;
                }
                // Expand select node to display added container
                SelectedNode.Expand();
            }
        }

        /// <summary>
        /// Delete menu item click event handler.
        /// </summary>
        /// <param name="sender">Delete menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmTreeViewDeleteHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag == null)
                {
                    return;
                }

                switch (item.Tag)
                {
                    case EcucContainerTreeNode container:
                        if (container.Parent is EcucContainersTreeNode)
                        {
                            if (container.Parent.Parent is EcucContainerTreeNode parentParentContainer)
                            {
                                // Delete container
                                BeginUpdate();
                                parentParentContainer.DelContainer(container.Data.BswmdPathShort, container.Data.Value);
                                EndUpdate();
                            }
                        }
                        else if (container.Parent is EcucContainerTreeNode parentContainer)
                        {
                            parentContainer.DelContainer(container.Data.BswmdPathShort, container.Title);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Usage menu item click event handler.
        /// </summary>
        /// <param name="sender">Usage menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void CmTreeViewUsageHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                if (toolStripItem.Tag is EcucInstanceContainer container)
                {
                    try
                    {
                        // Find usage
                        ExpandAllNodes(container.AsrPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Expand all parent of node with name "name" from top.
        /// </summary>
        /// <param name="name">Name of node expanded.</param>
        public void ExpandAllNodes(string name)
        {
            ExpandAllNodes(Nodes[0].Nodes, name);
        }

        /// <summary>
        /// Internal Expand all parent of node with name "name".
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
                        if (node.Parent != null)
                        {
                            node.Parent.Expand();
                        }
                        return true;
                    }
                    else
                    {
                        if (ExpandAllNodes(node.Nodes, name) == true)
                        {
                            if (node.Parent != null)
                            {
                                node.Parent.Expand();
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// TreeNode with multiple containers.
    /// </summary>
    public class EcucContainersTreeNode : TreeNode
    {
        /// <summary>
        /// Ecuc bswmd of containers.
        /// </summary>
        public IEcucBswmdModule Bswmd;
        /// <summary>
        /// Ecuc datas of containers.
        /// </summary>
        public EcucDataList Datas { get; private set; }

        /// <summary>
        /// Initialize EcucContainersTreeNode.
        /// </summary>
        /// <param name="bswmd">Ecuc bswmd of containers.</param>
        /// <param name="datas">Ecuc datas of containers.</param>
        public EcucContainersTreeNode(IEcucBswmdModule bswmd, EcucDataList datas)
        {
            // Handle input
            Bswmd = bswmd;
            Datas = datas;

            // Updateui
            UpdateUi(datas);
        }

        /// <summary>
        /// Update ui.
        /// </summary>
        public void UpdateUi()
        {
            UpdateUi(Datas);
        }

        /// <summary>
        /// Change datas and update ui.
        /// </summary>
        /// <param name="datas"></param>
        /// <exception cref="Exception"></exception>
        public void UpdateUi(EcucDataList datas)
        {
            // Handle input
            if (Datas != datas)
            {
                Datas = datas;
            }

            // Handle text and name
            if (Text != Bswmd.AsrPathShort)
            {
                Text = Bswmd.AsrPathShort;
                Name = Bswmd.AsrPathShort;
            }

            // Remove parent if no data exist
            if (datas.Count == 0)
            {
                Parent.Nodes.Remove(this);
                return;
            }

            // Update tooltip if needed
            if (Datas.ValidStatus == false)
            {
                ToolTipText = Datas.ValidInfo;
                ForeColor = Color.Red;
            }
            else
            {
                ToolTipText = "";
                ForeColor = Color.Black;
            }

            // Check exist ui with new to decrease time
            int i;
            if (Nodes.Count <= datas.Count)
            {
                for (i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is EcucContainerTreeNode containerNode)
                    {
                        containerNode.UpdateUi(datas[i]);
                    }
                    else
                    {
                        throw new Exception("Fail to update ui due to invalid child node type");
                    }
                }
                for (; i < datas.Count; i++)
                {
                    Nodes.Add(new EcucContainerTreeNode(datas[i]));
                }
            }
            else
            {
                for (i = 0; i < datas.Count; i++)
                {
                    if (Nodes[i] is EcucContainerTreeNode containerNode)
                    {
                        containerNode.UpdateUi(datas[i]);
                    }
                    else
                    {
                        throw new Exception("Fail to update ui due to invalid child node type");
                    }
                }
                for (; i < Nodes.Count; i++)
                {
                    Nodes.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// TreeNode with single container.
    /// </summary>
    public class EcucContainerTreeNode : TreeNode
    {
        /// <summary>
        /// Ecuc data of container.
        /// </summary>
        public EcucData Data { get; private set; }

        /// <summary>
        /// Title of node.
        /// </summary>
        public string Title
        {
            get
            {
                return Data.Value;
            }
        }

        /// <summary>
        /// Initialize EcucContainerTreeNode.
        /// </summary>
        /// <param name="data">Ecuc data of container.</param>
        public EcucContainerTreeNode(EcucData data)
        {
            // Handle input
            Data = data;

            // Handle control
            Data.PropertyChanged += PropertyChangedEventHandler;
            Name = Data.AsrPath;

            // Update ui
            UpdateUi();
        }

        /// <summary>
        /// Update ui.
        /// </summary>
        public void UpdateUi()
        {
            UpdateUi(Data);
        }

        /// <summary>
        /// Change data and update ui.
        /// </summary>
        /// <param name="data"></param>
        public void UpdateUi(EcucData data)
        {
            // Handle input
            if (data != Data)
            {
                Data = data;
            }

            // Handle text and name
            if (Text != Data.Value)
            {
                Text = Data.Value;
                Name = Data.AsrPath;
            }

            if (Data.ValidStatus == false)
            {
                ToolTipText = Data.ValidInfo;
                ForeColor = Color.Red;
            }
            else
            {
                ToolTipText = "";
                ForeColor = Color.Black;
            }

            // Check existing ui with new to decease operation
            foreach (var subContainer in Data.SortedContainers)
            {
                if (subContainer.Value.Count == 0)
                {
                    if (subContainer.Key.IsSingle == true)
                    {
                        foreach (TreeNode node in Nodes)
                        {
                            if (node is EcucContainerTreeNode containerNode)
                            {
                                if (containerNode.Data.BswmdPathShort == subContainer.Key.AsrPathShort)
                                {
                                    Nodes.Remove(node);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Nodes.ContainsKey(subContainer.Key.AsrPathShort) == true)
                        {
                            Nodes.RemoveByKey(subContainer.Key.AsrPathShort);
                        }
                    }
                }
                else
                {
                    if (subContainer.Key.IsSingle == true)
                    {
                        if (subContainer.Value.Count == 1)
                        {
                            if (Nodes.ContainsKey(subContainer.Value.First.AsrPath) == true)
                            {
                                var node = Nodes[subContainer.Value.First.AsrPath];
                                if (node is EcucContainerTreeNode containerNode)
                                {
                                    containerNode.UpdateUi(subContainer.Value[0]);
                                }
                            }
                            else
                            {
                                Nodes.Add(new EcucContainerTreeNode(subContainer.Value[0]));
                            }
                        }
                    }
                    else
                    {
                        if (Nodes.ContainsKey(subContainer.Key.AsrPathShort) == true)
                        {
                            var node = Nodes[subContainer.Key.AsrPathShort];
                            if (node is EcucContainersTreeNode containersNode)
                            {
                                containersNode.UpdateUi(subContainer.Value);
                            }
                        }
                        else
                        {
                            if (subContainer.Value.Count > 0)
                            {
                                var ui = new EcucContainersTreeNode(subContainer.Key, subContainer.Value);
                                Nodes.Add(ui);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add sub-container with short form of bswmd path.
        /// </summary>
        /// <param name="bswmdPathShort">Short form of bswmd path of container to add.</param>
        public void AddContainer(string bswmdPathShort)
        {
            // Add all neccessary sub-elment of container
            Data.AddContainerWithRequiredField(bswmdPathShort);
            // Update ui
            UpdateUi();
        }

        /// <summary>
        /// Delete sub-container with short form of bswmd path and short name.
        /// </summary>
        /// <param name="bswmdPathShort">Short form of bswmd path of container to delete.</param>
        /// <param name="shortName">Short name of of container to delete.</param>
        /// <returns>Count of container with same bswmdPathShort left in container.</returns>
        public int DelContainer(string bswmdPathShort, string shortName)
        {
            try
            {
                // Delete container
                var count = Data.DelContainer(bswmdPathShort, shortName);
                // Update ui
                UpdateUi();
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Data.Containers.FindAll(x => x.BswmdPathShort == bswmdPathShort).Count;
            }
            
        }

        /// <summary>
        /// Data Propert changed event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Changed event.</param>
        void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Text = Data.Value;
                Name = Data.AsrPath;
            }

            if (e.PropertyName == "ValidStatus")
            {
                UpdateUi();
            }
        }
    }

    /// <summary>
    /// Ecuc validation treeview.
    /// </summary>
    public class EcucValidationTreeView : TreeView
    {
        /// <summary>
        /// Data source of instance manager.
        /// </summary>
        public EcucInstanceManager InstanceManager { get; }
        /// <summary>
        /// Data source of bswmd manager.
        /// </summary>
        public EcucBswmdManager BswmdManager { get; }

        /// <summary>
        /// Model treeview to navigate.
        /// </summary>
        private readonly EcucModelTreeView modelTreeView;
        /// <summary>
        /// Popup menu.
        /// </summary>
        private readonly ContextMenuStrip cm;
        /// <summary>
        /// Navigate menu item.
        /// </summary>
        private readonly ToolStripMenuItem tmNavigate;
        /// <summary>
        /// Solve menu item.
        /// </summary>
        private readonly ToolStripMenuItem tmSolve;

        /// <summary>
        /// Initiailize EcucValidationTreeView.
        /// </summary>
        /// <param name="instanceManager">Data source of instance manager.</param>
        /// <param name="bswmdManager">Data source of bswmd manager.</param>
        /// <param name="treeViewModel">Model treeview to navigate.</param>
        public EcucValidationTreeView(EcucInstanceManager instanceManager, EcucBswmdManager bswmdManager, EcucModelTreeView treeViewModel)
        {
            // Handle input
            InstanceManager = instanceManager;
            BswmdManager = bswmdManager;
            modelTreeView = treeViewModel;

            // Prepare control
            Dock = DockStyle.Fill;
            Location = new Point(0, 0);
            Margin = new Padding(4);
            Name = "tvValidation";
            Size = new Size(450, 850);
            TabIndex = 0;
            MouseClick += TreeViewMouseClickEventHandler;

            cm = new ContextMenuStrip();
            tmNavigate = new ToolStripMenuItem();
            tmSolve = new ToolStripMenuItem();

            cm.ImageScalingSize = new Size(20, 20);
            cm.Items.AddRange(new ToolStripItem[] { tmNavigate, tmSolve });
            cm.Name = "cm";
            cm.Size = new Size(127, 58);

            tmNavigate.Name = "tmNavigate";
            tmNavigate.Size = new Size(126, 24);
            tmNavigate.Text = "Navigate";
            tmNavigate.Visible = true;
            tmNavigate.MouseDown += TmNavigateMouseDownEventHandler;

            tmSolve.Name = "tmSolve";
            tmSolve.Size = new Size(126, 24);
            tmSolve.Text = "Solve";

            ContextMenuStrip = cm;

            // Refresh ui
            RefreshUi();
        }

        /// <summary>
        /// Refresh ui.
        /// </summary>
        public void RefreshUi()
        {
            // Iterate all module adding node
            foreach (var module in InstanceManager.EcucModules)
            {
                var node = Nodes.Add($"{module.AsrPathShort} (0)");
                node.Name = module.AsrPathShort;
                node.Tag = new EcucData(module, BswmdManager.GetBswmdFromBswmdPath(module.BswmdPath));
                module.PropertyChanged += ModuleChangedEventHandler;
            }
        }

        /// <summary>
        /// Module data changed event handler.
        /// </summary>
        /// <param name="sender">Module which data changed.</param>
        /// <param name="e">Propery event.</param>
        private void ModuleChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is EcucInstanceModule module)
            {
                if (Nodes[module.AsrPathShort] != null)
                {
                    if (Nodes[module.AsrPathShort].Tag is EcucData data)
                    {
                        // Find node and change its text
                        var valids = data.ValidRecursive;
                        var root = Nodes[module.AsrPathShort];
                        root.Text = $"{root.Name} ({valids.Count})";

                        // Clear invalid information
                        root.Nodes.Clear();
                        // Add invalid information
                        foreach (var valid in valids)
                        {
                            var node = root.Nodes.Add(valid.Info);
                            node.Tag = valid;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mouse click event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Mouse event.</param>
        private void TreeViewMouseClickEventHandler(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Right click and find selected node
                var node = GetNodeAt(e.Location);
                if (node != null)
                {
                    SelectedNode = node;
                    if (node.Tag is EcucValid valid)
                    {
                        if (valid.Solves.Count == 0)
                        {
                            tmSolve.Visible = false;
                        }
                        else
                        {
                            // Prepare solve candidates
                            tmSolve.DropDownItems.Clear();
                            foreach (var solve in valid.Solves)
                            {
                                var item = tmSolve.DropDownItems.Add(solve.Description);
                                item.MouseDown += TmSolveMouseDownEventHandler;
                                item.Tag = solve;
                            }
                            tmSolve.Visible = true;
                        }
                    }
                    // Display popup
                    ContextMenuStrip.Show();
                }
            }
        }

        /// <summary>
        /// Navigate menu item mouse click handler.
        /// </summary>
        /// <param name="sender">Navigate menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void TmNavigateMouseDownEventHandler(object? sender, MouseEventArgs e)
        {
            if (SelectedNode != null)
            {
                // Parse node text and find position to navigate in mode treeview
                var parts = SelectedNode.Text.Split("@");
                if (parts.Length > 1)
                {
                    var part = parts[1];
                    parts = part.Split(":");
                    if (parts.Length > 0)
                    {
                        modelTreeView.ExpandAllNodes(parts[0]);
                        modelTreeView.Focus();
                    }
                }
                else if (parts.Length == 1)
                {
                    var part = parts[0];
                    parts = part.Split(":");
                    if (parts.Length > 0)
                    {
                        modelTreeView.ExpandAllNodes(parts[0]);
                        modelTreeView.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Solve menu item mouse click handler.
        /// </summary>
        /// <param name="sender">Solve menu item.</param>
        /// <param name="e">Mouse event.</param>
        private void TmSolveMouseDownEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem item)
            {
                if (item.Tag is EcucSolve solve)
                {
                    // Run solver
                    solve.Solve();
                }
            }
        }
    }
}