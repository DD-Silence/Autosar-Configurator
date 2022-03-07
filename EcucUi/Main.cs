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

using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSScriptLib;
using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EInstance;
using Ecuc.EcucUi;

namespace AutosarConfigurator
{
    public partial class WdMain : Form
    {
        private EcucInstanceManager instanceManager;
        private EcucBswmdManager bswmdManager;
        private EcucTableLayout tpValue;
        private EcucTreeView tvModel;
        private Reference wdReference;

        public WdMain()
        {
            InitializeComponent();
            Console.SetOut(new ConsoleRichTextBox(rtStatus));
            HandleConfiguration();
        }

        private void CreateTabelLayoutPanel()
        {
            splitContainer2.Panel2.Controls.Clear();
            tpValue = new EcucTableLayout(rtDesc, wdReference);
            splitContainer2.Panel2.Controls.Add(tpValue);
        }

        private void CreateTreeView()
        {
            splitContainer2.Panel1.Controls.Clear();
            tvModel = new EcucTreeView(instanceManager, bswmdManager, rtDesc, tpValue);
            splitContainer2.Panel1.Controls.Add(tvModel);
        }

        private void CreateScriptMenu(string path, ToolStripItemCollection items)
        {
            var directories = Directory.GetDirectories(path);
            foreach (var d in directories)
            {
                ToolStripItem item = items.Add(d.Split("\\")[^1]);
                CreateScriptMenu(d, (item as ToolStripMenuItem).DropDownItems);
            }

            var files = Directory.GetFiles(path, "*.cs");
            foreach (var file in files)
            {
                var item = items.Add(file.Split("\\")[^1]);
                item.Tag = file;
                item.MouseDown += MnScriptClickEventHandler;
            }
        }

        private void RefreshAll()
        {
            GC.Collect();
            bswmdManager = new EcucBswmdManager(GetAllBswmdFiles());
            instanceManager = new EcucInstanceManager(GetAllInstanceFiles());

            wdReference = new Reference(instanceManager);

            CreateTabelLayoutPanel();
            CreateTreeView();
            tpValue.AddTreeNodes(tvModel.Nodes[0].Nodes);
            mnScript.DropDownItems.Clear();
#if DEBUG
            CreateScriptMenu("../../../../data/script", mnScript.DropDownItems);
#else
            CreateScriptMenu("data/script", mnScript.DropDownItems);
#endif
        }

        private void WdMain_Load(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private string[] GetAllBswmdFiles()
        {
            return Directory.GetFiles(tbBswmd.Text, "*.arxml");
        }

        private string[] GetAllInstanceFiles()
        {
            return Directory.GetFiles(tbInstance.Text, "*.arxml");
        }

        private void SaveConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                config.AppSettings.Settings["BSWMD_PATH"].Value = tbBswmd.Text;
            }
            catch
            {
                config.AppSettings.Settings.Add("BSWMD_PATH", tbBswmd.Text);
            }

            try
            {
                config.AppSettings.Settings["INSTANCE_PATH"].Value = tbInstance.Text;
            }
            catch
            {
                config.AppSettings.Settings.Add("INSTANCE_PATH", tbInstance.Text);
            }

            try
            {
                config.AppSettings.Settings["FORM_WIDTH"].Value = Width.ToString();
            }
            catch
            {
                config.AppSettings.Settings.Add("FORM_WIDTH", Width.ToString());
            }

            try
            {
                config.AppSettings.Settings["FORM_HEIGHT"].Value = Height.ToString();
            }
            catch
            {
                config.AppSettings.Settings.Add("FORM_HEIGHT", Height.ToString());
            }

            try
            {
                config.AppSettings.Settings["DISTANCE2"].Value = splitContainer2.SplitterDistance.ToString();
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE2", splitContainer2.SplitterDistance.ToString());
            }

            try
            {
                config.AppSettings.Settings["DISTANCE3"].Value = splitContainer3.SplitterDistance.ToString();
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE3", splitContainer3.SplitterDistance.ToString());
            }
            try
            {
                config.AppSettings.Settings["DISTANCE4"].Value = splitContainer4.SplitterDistance.ToString();
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE4", splitContainer4.SplitterDistance.ToString());
            }

            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void HandleConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                tbBswmd.Text = config.AppSettings.Settings["BSWMD_PATH"].Value;
            }
            catch
            {
#if DEBUG
                tbBswmd.Text = "../../../../data/bswmd";
#else
                tbBswmd.Text = "data/bswmd";
#endif
                config.AppSettings.Settings.Add("BSWMD_PATH", tbBswmd.Text);
            }

            try
            {
                tbInstance.Text = config.AppSettings.Settings["INSTANCE_PATH"].Value;
            }
            catch
            {
#if DEBUG
                tbInstance.Text = "../../../../data/instance";
#else
                tbBswmd.Text = "data/instance";
#endif
                config.AppSettings.Settings.Add("INSTANCE_PATH", tbInstance.Text);
            }

            try
            {
                Width = Convert.ToInt32(config.AppSettings.Settings["FORM_WIDTH"].Value);
            }
            catch
            {
                config.AppSettings.Settings.Add("FORM_WIDTH", Width.ToString());
            }

            try
            {
                Height = Convert.ToInt32(config.AppSettings.Settings["FORM_HEIGHT"].Value);
            }
            catch
            {
                config.AppSettings.Settings.Add("FORM_HEIGHT", Height.ToString());
            }

            try
            {
                splitContainer2.SplitterDistance = Convert.ToInt32(config.AppSettings.Settings["DISTANCE2"].Value);
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE2", splitContainer2.SplitterDistance.ToString());
            }

            try
            {
                splitContainer3.SplitterDistance = Convert.ToInt32(config.AppSettings.Settings["DISTANCE3"].Value);
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE3", splitContainer3.SplitterDistance.ToString());
            }

            try
            {
                splitContainer4.SplitterDistance = Convert.ToInt32(config.AppSettings.Settings["DISTANCE4"].Value);
            }
            catch
            {
                config.AppSettings.Settings.Add("DISTANCE4", splitContainer4.SplitterDistance.ToString());
            }

            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void BtBswmd_Click(object sender, EventArgs e)
        {
            fdMain.RootFolder = Environment.SpecialFolder.LocalApplicationData;
            if (fdMain.ShowDialog() == DialogResult.OK)
            {
                tbBswmd.Text = fdMain.SelectedPath;
                SaveConfiguration();
            }
        }

        private void BtInstance_Click(object sender, EventArgs e)
        {
            fdMain.RootFolder = Environment.SpecialFolder.LocalApplicationData;
            if (fdMain.ShowDialog() == DialogResult.OK)
            {
                tbInstance.Text = fdMain.SelectedPath;
                SaveConfiguration();
            }
        }

        private void MnSave_Click(object sender, EventArgs e)
        {
            instanceManager.Save();
        }

        private void MnReload_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MnScriptClickEventHandler(object sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is string file)
                {
                    CSScript.CacheEnabled = false;
                    dynamic script = CSScript.Evaluator.LoadFile(file);
                    Console.WriteLine($"Start to execute script {file}");
                    Task.Run(() =>
                    {
                        try
                        {
                            GC.Collect();
                            script.ScriptRun(bswmdManager, instanceManager);
                            Console.WriteLine($"Script {file} execute sucessfully");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Script {file} execute fail with exception:");
                            Console.WriteLine($"{ex.Message}");
                        }
                    });
                }
            }
        }

        private void WdMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfiguration();
        }
    }
}
