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

using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EData;

namespace Ecuc.EcucUi
{
    /// <summary>
    /// Listview to contain single column data.
    /// </summary>
    public class EcucSingleListView : ListView
    {
        /// <summary>
        /// UiData to display EcucData.
        /// </summary>
        private IEcucTableLayoutPanelElement UiData { get; set; }
        /// <summary>
        /// Window of refernece.
        /// </summary>
        private readonly Form wdReference;
        /// <summary>
        /// EcucTableLayout control to display parameters.
        /// </summary>
        private readonly EcucTableLayout tableLayout;
        /// <summary>
        /// Popup menu in ListView.
        /// </summary>
        private readonly ContextMenuStrip cm;
        /// <summary>
        /// Create menu item.
        /// </summary>
        private readonly ToolStripMenuItem cmCreate;
        /// <summary>
        /// Detele menu item.
        /// </summary>
        private readonly ToolStripMenuItem cmDelete;

        /// <summary>
        /// Initialize EcucSingleListView.
        /// </summary>
        /// <param name="uiData">uiData to display.</param>
        /// <param name="wd">Form for reference choose.</param>
        /// <param name="table">EcucTableLayout to display parameter.</param>
        public EcucSingleListView(IEcucTableLayoutPanelElement uiData, Form wd, EcucTableLayout table)
        {
            // Handle input
            UiData = uiData;
            wdReference = wd;
            tableLayout = table;

            // Prepare controls.
            cm = new ContextMenuStrip();
            cmCreate = new ToolStripMenuItem();
            cmDelete = new ToolStripMenuItem();

            cm.ImageScalingSize = new Size(20, 20);
            cm.Items.AddRange(new ToolStripItem[] {
            cmCreate,
            cmDelete});
            cm.Name = "cmTableLayout";
            cm.Size = new Size(127, 58);

            cmCreate.Name = "cmTableLayoutCreate";
            cmCreate.Size = new Size(126, 24);
            cmCreate.Text = "Add";

            cmCreate.MouseDown += CmAddHandler;

            cmDelete.Name = "cmTableLayoutDelete";
            cmDelete.Size = new Size(126, 24);
            cmDelete.Text = "Delete";

            cmDelete.MouseDown += CmDeleteHandler;

            ContextMenuStrip = cm;
            
            // Refresh ui.
            RefreshUi();
        }

        /// <summary>
        /// Refresh listview.
        /// </summary>
        public void RefreshUi()
        {
            RefreshUi(UiData);
        }

        /// <summary>
        /// Change ui data and refresh listview.
        /// </summary>
        /// <param name="uiData"></param>
        public void RefreshUi(IEcucTableLayoutPanelElement uiData)
        {
            // Handle input
            UiData = uiData;

            // Clear existing ui
            Columns.Clear();
            Items.Clear();

            // Start to construct ui
            BeginUpdate();
            // Add column
            var c = Columns.Add(UiData.Bswmd.AsrPathShort);
            var maxWords = 0;
            if (UiData.Text.Count == 0)
            {
                // No data, use default width
                Width = tableLayout.Width / 2;
                Height = 100;
            }
            else
            {
                foreach (var text in UiData.Text)
                {
                    // Add text and calculate max words
                    var item = Items.Add(text);
                    if (text.Length > maxWords)
                    {
                        maxWords = text.Length;
                    }
                }
                // Calculate width
                Width = maxWords * 8 + 20 > tableLayout.Width / 2 ? maxWords * 8 + 20 : tableLayout.Width / 2;
                // Calculate height
                if (UiData.Text.Count * 20 + 60 < 100)
                {
                    Height = 100;
                }
                else if (UiData.Text.Count * 20 + 60 > tableLayout.Height / 2)
                {
                    Height = tableLayout.Height / 2;
                }
                else
                {
                    Height = UiData.Text.Count * 20 + 60;
                }
                Name = UiData.Title;
                Enabled = UiData.Exist;
            }
            c.Width = Width;
            EndUpdate();
        }

        /// <summary>
        /// Add menu item handler.
        /// </summary>
        /// <param name="sender">Menu item trigger this handler.</param>
        /// <param name="e">Mouse event trigger this handler.</param>
        private void CmAddHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                switch (UiData)
                {
                    case TableLayoutPanelElementPara elementPara:
                        // Add paramter
                        if (tableLayout.Data != null)
                        {
                            tableLayout.Data.AddPara(elementPara.Bswmd.AsrPathShort);
                            tableLayout.UpdateUi();
                        }
                        break;

                    case TableLayoutPanelElementRef elementRef:
                        // Prepare candidate reference information and call reference form
                        wdReference.Tag = new KeyValuePair<IEcucBswmdBase, EcucDataList>(UiData.Bswmd, UiData.Datas);
                        wdReference.ShowDialog();
                        // Get data from reference form
                        if (tableLayout.Data != null)
                        {
                            if (wdReference.Tag is Dictionary<string, List<string>> referenceDict)
                            {
                                // Iterate all references in tag and add reference
                                foreach (var references in referenceDict)
                                {
                                    foreach (var reference in references.Value)
                                    {
                                        tableLayout.Data.AddRef(elementRef.Bswmd.AsrPathShort, reference);
                                    }

                                }
                            }
                            // Update EcucTableLayout ui
                            tableLayout.UpdateUi();
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Delete menu item handler.
        /// </summary>
        /// <param name="sender">Menu item trigger this handler.</param>
        /// <param name="e">Mouse event trigger this handler.</param>
        private void CmDeleteHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                switch (UiData)
                {
                    case TableLayoutPanelElementPara elementPara:
                        // Delete paramter
                        if (tableLayout.Data != null)
                        {
                            tableLayout.Data.DelPara(elementPara.Bswmd.AsrPath);
                            tableLayout.UpdateUi();
                        }
                        break;

                    case TableLayoutPanelElementRef elementRef:
                        if (tableLayout.Data != null)
                        {
                            // Delete all reference in selected item
                            foreach (ListViewItem selectedItem in SelectedItems)
                            {
                                var str = selectedItem.Text;
                                if (str != null)
                                {
                                    tableLayout.Data.DelRef(elementRef.Bswmd.AsrPathShort, str);
                                }
                            }
                            // Update EcucTableLayout ui
                            tableLayout.UpdateUi();
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// DataGrid for display multiply container paramters.
    /// </summary>
    public class EcucMultiplyDataGrid : DataGridView
    {
        /// <summary>
        /// Ecuc bsmwd of containers.
        /// </summary>
        private IEcucBswmdBase Bswmd { get; set; }
        /// <summary>
        /// Ecuc data of containers.
        /// </summary>
        private EcucDataList Datas { get; set; }
        /// <summary>
        /// Ecuc data after filter.
        /// </summary>
        private EcucDataList FilteredDatas { get; set; } = new EcucDataList();

        /// <summary>
        /// Intialize EcucMultiplyDataGrid.
        /// </summary>
        /// <param name="bswmd">Ecuc bswmd of containers.</param>
        /// <param name="datas">Ecuc data of containers.</param>
        public EcucMultiplyDataGrid(IEcucBswmdBase bswmd, EcucDataList datas)
        {
            // Handle input
            Bswmd = bswmd;
            Datas = datas;
            FilteredDatas = datas;

            // Prepare controls
            VirtualMode = true;
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            CellValueNeeded += CellValueNeededEventHandler;

            // Refresh ui
            RefreshUi();
        }

        /// <summary>
        /// Refresh ui.
        /// </summary>
        /// <param name="filter">Filter of Ecuc data, empty as default.</param>
        public void RefreshUi(string filter = "")
        {
            RefreshUi(Bswmd, Datas, filter);
        }

        /// <summary>
        /// Change data and update ui.
        /// </summary>
        /// <param name="bswmd">Ecuc bswmd to change.</param>
        /// <param name="datas">Ecuc datas to change.</param>
        /// <param name="filter">Filter to change.</param>
        public void RefreshUi(IEcucBswmdBase bswmd, EcucDataList datas, string filter = "")
        {
            // Handle input
            Bswmd = bswmd;
            Datas = datas;

            // Clear existing ui
            Columns.Clear();
            Rows.Clear();

            // Add required ShortName column
            Columns.Add("ShortName", "ShortName");

            if (bswmd is EcucBswmdContainer bswmdContainer)
            {
                // Add all single parameter
                foreach (var bswmdPara in bswmdContainer.Paras)
                {
                    if (bswmdPara.IsSingle == true)
                    {
                        Columns.Add(bswmdPara.AsrPathShort, bswmdPara.AsrPathShort);
                    }
                }
                // Add all single reference
                foreach (var bswmdRef in bswmdContainer.Refs)
                {
                    if (bswmdRef.IsSingle == true)
                    {
                        Columns.Add(bswmdRef.AsrPathShort, bswmdRef.AsrPathShort);
                    }
                }
            }

            // Parse filter expression
            var filterDict = new Dictionary<string, string>();
            var filters = filter.Split('|');
            foreach (var f in filters)
            {
                var fs = f.Split('=');
                if (fs.Length == 2)
                {
                    filterDict[fs[0].Trim()] = fs[1].Trim();
                }
            }

            // Filter data
            FilteredDatas = Datas;
            if (filterDict.Count > 0)
            {
                foreach (var f in filterDict)
                {
                    if (f.Key == "ShortName")
                    {
                        var query = from data in FilteredDatas
                                    where data.Value.Contains(f.Value)
                                    select data;
                        FilteredDatas = new EcucDataList(query.ToList());
                    }
                    else
                    {
                        var query = from data in FilteredDatas
                                    where data[f.Key].FirstValue.Contains(f.Value)
                                    select data;
                        FilteredDatas = new EcucDataList(query.ToList());
                    }
                }
            }
            // Update row number
            RowCount = FilteredDatas.Count;
        }

        /// <summary>
        /// Cell value update handler.
        /// </summary>
        /// <param name="sender">Not needed.</param>
        /// <param name="e">Updated cell information.</param>
        private void CellValueNeededEventHandler(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                // First column is ShortName
                e.Value = FilteredDatas[e.RowIndex].Value;
            }
            else
            {
                try
                {
                    e.Value = FilteredDatas[e.RowIndex][Columns[e.ColumnIndex].Name].FirstValue;
                }
                catch
                {
                    e.Value = "";
                }
            }
        }
    }
}
