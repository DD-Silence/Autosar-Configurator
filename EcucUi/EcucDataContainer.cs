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

namespace Ecuc.EcucUi
{
    public class EcucSingleListView : ListView
    {
        private IEcucTableLayoutPanelElement UiData { get; set; }
        private readonly Form wdReference;
        private readonly EcucTableLayout tableLayout;
        private readonly ContextMenuStrip cm;
        private readonly ToolStripMenuItem cmCreate;
        private readonly ToolStripMenuItem cmDelete;

        public EcucSingleListView(IEcucTableLayoutPanelElement uiData, Form wd, EcucTableLayout table)
        {
            UiData = uiData;
            wdReference = wd;
            tableLayout = table;

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

            RefreshUi();
        }

        public void RefreshUi()
        {
            RefreshUi(UiData);
        }

        public void RefreshUi(IEcucTableLayoutPanelElement uiData)
        {
            UiData = uiData;
            Columns.Clear();
            Items.Clear();

            BeginUpdate();
            var c = Columns.Add(UiData.Bswmd.AsrPathShort);
            var maxWords = 0;
            if (UiData.Text.Count == 0)
            {
                Width = tableLayout.Height / 2;
                Height = 100;
            }
            else
            {
                foreach (var text in UiData.Text)
                {
                    var item = Items.Add(text);
                    if (text.Length > maxWords)
                    {
                        maxWords = text.Length;
                    }
                }
                Width = maxWords * 8 + 20 > tableLayout.Width / 2 ? maxWords * 8 + 20 : tableLayout.Width / 2;
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

        private void CmAddHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                switch (UiData)
                {
                    case TableLayoutPanelElementPara:
                        if (UiData is TableLayoutPanelElementPara elementPara)
                        {
                            if (tableLayout.Data != null)
                            {
                                tableLayout.Data.AddPara(elementPara.Bswmd.AsrPathShort);
                                tableLayout.UpdateUi();
                            }
                        }
                        break;

                    case TableLayoutPanelElementRef:
                        if (UiData is TableLayoutPanelElementRef elementRef)
                        {
                            wdReference.Tag = new KeyValuePair<IEcucBswmdBase, EcucDataList>(UiData.Bswmd, UiData.Datas);
                            wdReference.ShowDialog();
                            if (tableLayout.Data != null)
                            {
                                if (wdReference.Tag is Dictionary<string, List<string>> referenceDict)
                                {
                                    foreach (var references in referenceDict)
                                    {
                                        foreach (var reference in references.Value)
                                        {
                                            tableLayout.Data.AddRef(elementRef.Bswmd.AsrPathShort, reference);
                                        }

                                    }
                                }
                                tableLayout.UpdateUi();
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void CmDeleteHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                switch (UiData)
                {
                    case TableLayoutPanelElementPara:
                        if (UiData is TableLayoutPanelElementPara elementPara)
                        {
                            if (tableLayout.Data != null)
                            {
                                tableLayout.Data.DelPara(elementPara.Bswmd.AsrPath);
                                tableLayout.UpdateUi();
                            }
                        }
                        break;

                    case TableLayoutPanelElementRef:
                        if (UiData is TableLayoutPanelElementRef elementRef)
                        {
                            if (tableLayout.Data != null)
                            {
                                foreach (ListViewItem selectedItem in SelectedItems)
                                {
                                    var str = selectedItem.Text;
                                    if (str != null)
                                    {
                                        tableLayout.Data.DelRef(elementRef.Bswmd.AsrPathShort, str);
                                    }
                                }
                                tableLayout.UpdateUi();
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public class EcucMultiplyDataGrid : DataGridView
    {
        private IEcucBswmdBase Bswmd { get; set; }
        private EcucDataList Datas { get; set; }
        private EcucDataList FilteredDatas { get; set; } = new EcucDataList();

        public EcucMultiplyDataGrid(IEcucBswmdBase bswmd, EcucDataList datas)
        {
            Bswmd = bswmd;
            Datas = datas;
            FilteredDatas = datas;

            VirtualMode = true;
            ReadOnly = true;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            CellValueNeeded += CellValueNeededEventHandler;

            RefreshUi();
        }

        public void RefreshUi(string filter = "")
        {
            RefreshUi(Bswmd, Datas, filter);
        }

        public void RefreshUi(IEcucBswmdBase bswmd, EcucDataList datas, string filter = "")
        {
            Bswmd = bswmd;
            Datas = datas;
            Columns.Clear();
            Rows.Clear();

            Columns.Add("ShortName", "ShortName");

            if (bswmd is EcucBswmdContainer bswmdContainer)
            {
                foreach (var bswmdPara in bswmdContainer.Paras)
                {
                    if (bswmdPara.IsSingle == true)
                    {
                        Columns.Add(bswmdPara.AsrPathShort, bswmdPara.AsrPathShort);
                    }
                }

                foreach (var bswmdRef in bswmdContainer.Refs)
                {
                    if (bswmdRef.IsSingle == true)
                    {
                        Columns.Add(bswmdRef.AsrPathShort, bswmdRef.AsrPathShort);
                    }
                }
            }

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
                                    where data[f.Key].FirstText.Contains(f.Value)
                                    select data;
                        FilteredDatas = new EcucDataList(query.ToList());
                    }
                }
            }

            RowCount = FilteredDatas.Count;
        }

        private void CellValueNeededEventHandler(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Value = FilteredDatas[e.RowIndex].Value;
            }
            else
            {
                try
                {
                    e.Value = FilteredDatas[e.RowIndex][Columns[e.ColumnIndex].Name].FirstText;
                }
                catch
                {
                    e.Value = "";
                }
            }
        }
    }
}
