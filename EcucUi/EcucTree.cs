/*  
 *  This file is a part of Autosar Configurator for ECU GUI based 
 *  configuration, checking and code generation.
 *  
 *  Copyright (C) 2021-2022 Dai Jin Shi E-mail:DD-Silence@sina.cn
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
    public class EcucTreeView : TreeView
    {
        public EcucInstanceManager InstanceManager { get; }
        public EcucBswmdManager BswmdManager { get; }

        private readonly RichTextBox richBox;
        private readonly EcucTableLayout tableLayout;
        private readonly ContextMenuStrip cmTreeView;
        private readonly ToolStripMenuItem cmTreeViewAdd;
        private readonly ToolStripMenuItem cmTreeViewDelete;
        private readonly ToolStripMenuItem cmTreeViewUsage;

        public EcucTreeView(EcucInstanceManager instanceManager, EcucBswmdManager bswmdManager, RichTextBox rb, EcucTableLayout panel)
        {
            richBox = rb;
            tableLayout = panel;
            InstanceManager = instanceManager;
            BswmdManager = bswmdManager;

            Dock = DockStyle.Fill;
            Location = new Point(0, 0);
            Margin = new Padding(4);
            Name = "tvModel";
            Size = new Size(450, 850);
            TabIndex = 0;
            AfterSelect += TvModel_AfterSelect;
            MouseClick += TvModel_MouseClick;

            cmTreeView = new ContextMenuStrip();
            cmTreeViewAdd = new ToolStripMenuItem();
            cmTreeViewDelete = new ToolStripMenuItem();
            cmTreeViewUsage = new ToolStripMenuItem();

            cmTreeView.ImageScalingSize = new Size(20, 20);
            cmTreeView.Items.AddRange(new ToolStripItem[] {
            cmTreeViewAdd,
            cmTreeViewDelete,
            cmTreeViewUsage});
            cmTreeView.Name = "cmTreeView";
            cmTreeView.Size = new Size(127, 58);

            cmTreeViewAdd.Name = "cmTreeViewAdd";
            cmTreeViewAdd.Size = new Size(126, 24);
            cmTreeViewAdd.Text = "Add";
            cmTreeViewAdd.Visible = false;

            cmTreeViewDelete.Name = "cmTreeViewDelete";
            cmTreeViewDelete.Size = new Size(126, 24);
            cmTreeViewDelete.Text = "Delete";

            cmTreeViewUsage.Name = "cmTreeViewUsage";
            cmTreeViewUsage.Size = new Size(126, 24);
            cmTreeViewUsage.Text = "Usage";

            cmTreeViewDelete.MouseDown += CmTreeViewDeleteHandler;

            ContextMenuStrip = cmTreeView;

            RefreshUi();
        }

        public void RefreshUi()
        {
            BeginUpdate();
            Nodes.Clear();
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

        private void TvModel_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            var selectedNode = SelectedNode;
            if (selectedNode == null)
            {
                return;
            }

            switch (selectedNode)
            {
                case EcucContainerTreeNode:
                    if (selectedNode is EcucContainerTreeNode containerNode)
                    {
                        tableLayout.RefreshUi(containerNode.Data);
                        richBox.Clear();
                        richBox.Text += string.Format("Name: {0}\r\n", containerNode.Data.BswmdPathShort);
                        richBox.Text += string.Format("Description: {0}\r\n", containerNode.Data.Desc);
                        richBox.Text += string.Format("Lower Multiplicity: {0}\r\n", containerNode.Data.Lower);
                        if (containerNode.Data.Upper == uint.MaxValue)
                        {
                            richBox.Text += $"Upper Multiplicity: n";
                        }
                        else
                        {
                            richBox.Text += string.Format("Upper Multiplicity: {0}\r\n", containerNode.Data.Upper);
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected ui container type");
                    }
                    break;

                case EcucContainersTreeNode:
                    if (selectedNode is EcucContainersTreeNode containersNode)
                    {
                        tableLayout.RefreshUi(containersNode.Bswmd, containersNode.Datas);
                        richBox.Clear();
                        richBox.Text += string.Format("Name: {0}\r\n", containersNode.Bswmd.ShortName);
                        richBox.Text += string.Format("Description: {0}\r\n", containersNode.Bswmd.Desc);
                        richBox.Text += string.Format("Lower Multiplicity: {0}\r\n", containersNode.Bswmd.Lower);
                        if (containersNode.Bswmd.Upper == uint.MaxValue)
                        {
                            richBox.Text += $"Upper Multiplicity: n";
                        }
                        else
                        {
                            richBox.Text += string.Format("Upper Multiplicity: {0}\r\n", containersNode.Bswmd.Upper);
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected ui container type");
                    }
                    break;

                default:
                    break;
            }
        }

        private void TvModel_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender is TreeView tv)
                {
                    var node = tv.GetNodeAt(e.Location);
                    if (node != null)
                    {
                        tv.SelectedNode = node;
                    }

                    cmTreeViewAdd.DropDownItems.Clear();
                    cmTreeViewUsage.DropDownItems.Clear();
                    switch (tv.SelectedNode)
                    {
                        case EcucContainerTreeNode:
                            if (tv.SelectedNode is EcucContainerTreeNode containerNode)
                            {
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
                                        var addedItem = cmTreeViewAdd.DropDownItems.Add(bswmdContainer.AsrPathShort);
                                        addedItem.Tag = containerNode;
                                        addedItem.Name = bswmdContainer.AsrPathShort;
                                        addedItem.MouseDown += CmTreeViewAddHandler;
                                    }
                                }
                                cmTreeViewDelete.Tag = containerNode;

                                foreach (var u in containerNode.Data.Usage)
                                {
                                    var item = cmTreeViewUsage.DropDownItems.Add(u.Parent.ShortName);
                                    item.Tag = u.Parent;
                                    item.MouseDown += CmTreeViewUsageHandler;
                                }
                            }
                            else
                            {
                                throw new Exception("Unexpected tag type");
                            }
                            break;

                        case EcucContainersTreeNode:
                            break;

                        default:
                            throw new Exception("Unexpected tag type");
                    }

                    if (cmTreeViewAdd.DropDownItems.Count > 0)
                    {
                        cmTreeViewAdd.Visible = true;
                    }
                    else
                    {
                        cmTreeViewAdd.Visible = false;
                    }

                    if (cmTreeViewUsage.DropDownItems.Count > 0)
                    {
                        cmTreeViewUsage.Visible = true;
                    }
                    else
                    {
                        cmTreeViewUsage.Visible = false;
                    }

                    cmTreeView.Visible = true;
                }
            }
        }

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
                    case EcucContainerTreeNode:
                        if (item.Tag is EcucContainerTreeNode containerNode)
                        {
                            BeginUpdate();
                            containerNode.AddContainer(item.Text);
                            EndUpdate();
                        }
                        break;

                    default:
                        break;
                }
                SelectedNode.Expand();
            }
        }

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
                    case EcucContainerTreeNode:
                        if (item.Tag is EcucContainerTreeNode container)
                        {
                            if (container.Parent is EcucContainersTreeNode)
                            {
                                if (container.Parent.Parent is EcucContainerTreeNode parentParentContainer)
                                {
                                    BeginUpdate();
                                    parentParentContainer.DelContainer(container.Data.BswmdPathShort, container.Data.Value);
                                    EndUpdate();
                                }
                            }
                            else if (container.Parent is EcucContainerTreeNode parentContainer)
                            {
                                parentContainer.DelContainer(container.Data.BswmdPathShort, container.Title);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void CmTreeViewUsageHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripItem toolStripItem)
            {
                if (toolStripItem.Tag is EcucInstanceContainer container)
                {
                    try
                    {
                        ExpandAllNodes(Nodes[0].Nodes, container.AsrPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

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

    public class EcucContainersTreeNode : TreeNode
    {
        public IEcucBswmdModule Bswmd;
        public EcucDataList Datas { get; private set; }
        private bool valid = false;

        public bool Valid
        {
            get
            {
                return valid;
            }
            set
            {
                if (valid == value)
                {
                    return;
                }

                valid = value;
                if (Parent != null)
                {
                    if (Parent is EcucContainersTreeNode containers)
                    {
                        containers.Valid = value;
                    }
                    else if(Parent is EcucContainerTreeNode container)
                    {
                        container.Valid = value;
                    }
                }
                if (valid == false)
                {
                    ForeColor = Color.Red;
                }
                else
                {
                    ForeColor = Color.Black;
                }
            }
        }

        public EcucContainersTreeNode(IEcucBswmdModule bswmd, EcucDataList datas)
        {
            Bswmd = bswmd;
            Datas = datas;
            Valid = datas.Valid;

            UpdateUi(datas);
        }

        public void UpdateUi()
        {
            UpdateUi(Datas);
        }

        public void UpdateUi(EcucDataList datas)
        {
            if (Datas != datas)
            {
                Datas = datas;
            }

            if (Text != Bswmd.AsrPathShort)
            {
                Text = Bswmd.AsrPathShort;
                Name = Bswmd.AsrPathShort;
            }

            if (datas.Count == 0)
            {
                Parent.Nodes.Remove(this);
                return;
            }

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

    public class EcucContainerTreeNode : TreeNode
    {
        public EcucData Data { get; private set; }

        public string Title
        {
            get
            {
                return Data.Value;
            }
        }

        private bool valid = false;

        public bool Valid
        {
            get
            {
                return valid;
            }
            set
            {
                if (valid == value)
                {
                    return;
                }

                valid = value;
                if (Parent != null)
                {
                    if (Parent is EcucContainersTreeNode containers)
                    {
                        containers.Valid = value;
                    }
                    else if (Parent is EcucContainerTreeNode container)
                    {
                        container.Valid = value;
                    }
                }
                if (valid == false)
                {
                    ForeColor = Color.Red;
                }
                else
                {
                    ForeColor = Color.Black;
                }
            }
        }

        public EcucContainerTreeNode(EcucData data)
        {
            Data = data;
            Data.PropertyChanged += PropertyChangedEventHandler;
            Name = Data.AsrPath;
            Valid = data.Valid;

            UpdateUi();
        }

        public void UpdateUi()
        {
            UpdateUi(Data);
        }

        public void UpdateUi(EcucData data)
        {
            if (data != Data)
            {
                Data = data;
            }

            if (Text != Data.Value)
            {
                Text = Data.Value;
                Name = Data.AsrPath;
            }

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

        public void AddContainer(string bswmdPathShort)
        {
            Data.AddContainerWithRequiredField(bswmdPathShort);
            UpdateUi();
        }

        public int DelContainer(string bswmdPathShort, string shortName)
        {
            var count = Data.DelContainer(bswmdPathShort, shortName);
            UpdateUi();
            return count;
        }

        void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Text = Data.Value;
                Name = Data.AsrPath;
            }

            if (e.PropertyName == "Valid" || e.PropertyName == "PropertyChangedEventHandler")
            {
                Valid = Data.Valid;
            }
        }
    }
}