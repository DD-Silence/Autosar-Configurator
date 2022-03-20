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

using System.Configuration;
using CSScriptLib;
using Ecuc.EcucBase.EBswmd;
using Ecuc.EcucBase.EInstance;
using Ecuc.EcucUi;

namespace AutosarConfigurator
{
    /// <summary>
    /// Main form.
    /// </summary>
    public partial class WdMain : Form
    {
        /// <summary>
        /// Data source of instance manager.
        /// </summary>
        private EcucInstanceManager? instanceManager;
        /// <summary>
        /// Data source of bswmd manager.
        /// </summary>
        private EcucBswmdManager? bswmdManager;
        /// <summary>
        /// EcucTableLayout to display parameter.
        /// </summary>
        private EcucTableLayout? tpValue;
        /// <summary>
        /// TreeView of model.
        /// </summary>
        private EcucModelTreeView? tvModel;
        /// <summary>
        /// TreeView of validation.
        /// </summary>
        private EcucValidationTreeView? tvValidation;
        /// <summary>
        /// Reference form.
        /// </summary>
        private Reference? wdReference;

        /// <summary>
        /// Main form.
        /// </summary>
        public WdMain()
        {
            InitializeComponent();
            // Redirect console output to ConsoleRichTextBox
            Console.SetOut(new ConsoleRichTextBox(rtScript));
            // Read configuration
            HandleConfiguration();
        }

        /// <summary>
        /// Create EcucTableLayout.
        /// </summary>
        private void CreateTabelLayoutPanel()
        {
            splitContainer2.Panel2.Controls.Clear();
            if (wdReference != null)
            {
                tpValue = new EcucTableLayout(rtDesc, wdReference);
                splitContainer2.Panel2.Controls.Add(tpValue);
            }
        }

        /// <summary>
        /// Create tree view of model.
        /// </summary>
        private void CreateModelTreeView()
        {
            splitContainer2.Panel1.Controls.Clear();
            if (instanceManager != null && bswmdManager != null && tpValue != null)
            {
                tvModel = new EcucModelTreeView(instanceManager, bswmdManager, rtDesc, tpValue);
                splitContainer2.Panel1.Controls.Add(tvModel);
            }
        }

        /// <summary>
        /// Create tree view of validation.
        /// </summary>
        private void CreateValidationTreeView()
        {
            if (instanceManager != null && bswmdManager != null && tvModel != null)
            {
                tpValid.Controls.Clear();
                tvValidation = new EcucValidationTreeView(instanceManager, bswmdManager, tvModel);
                tpValid.Controls.Add(tvValidation);
            }
        }

        /// <summary>
        /// Create script menu according to script folder arrangement.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="items"></param>
        private void CreateScriptMenu(string path, ToolStripItemCollection items)
        {
            var directories = Directory.GetDirectories(path);
            foreach (var d in directories)
            {
                ToolStripItem item = items.Add(d.Split("\\")[^1]);
                if (item is ToolStripMenuItem menuItem)
                {
                    CreateScriptMenu(d, menuItem.DropDownItems);
                }
            }

            var files = Directory.GetFiles(path, "*.cs");
            foreach (var file in files)
            {
                var item = items.Add(file.Split("\\")[^1]);
                item.Tag = file;
                item.MouseDown += MnScriptClickEventHandler;
            }
        }

        /// <summary>
        /// Reload operation.
        /// </summary>
        private void RefreshAll()
        {
            GC.Collect();
            bswmdManager = new EcucBswmdManager(GetAllBswmdFiles());
            instanceManager = new EcucInstanceManager(GetAllInstanceFiles());

            wdReference = new Reference(instanceManager);

            CreateTabelLayoutPanel();
            CreateModelTreeView();
            if (tpValue != null && tvModel != null)
            {
                tpValue.AddTreeNodes(tvModel.Nodes[0].Nodes);
            }
            mnScript.DropDownItems.Clear();
            CreateValidationTreeView();
#if DEBUG
            CreateScriptMenu("../../../../data/script", mnScript.DropDownItems);
#else
            CreateScriptMenu("data/script", mnScript.DropDownItems);
#endif
        }

        /// <summary>
        /// Form load event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void WdMain_Load(object sender, EventArgs e)
        {
            RefreshAll();
        }

        /// <summary>
        /// Get all bswmd files from specified foler.
        /// </summary>
        /// <returns>Paths of all bswmd files.</returns>
        private string[] GetAllBswmdFiles()
        {
            return Directory.GetFiles(tbBswmd.Text, "*.arxml");
        }

        /// <summary>
        /// Get all instance files from specified foler.
        /// </summary>
        /// <returns>Paths of all instance files.</returns>
        private string[] GetAllInstanceFiles()
        {
            return Directory.GetFiles(tbInstance.Text, "*.arxml");
        }

        /// <summary>
        /// Save config of form.
        /// </summary>
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

        /// <summary>
        /// Handle config of form.
        /// </summary>
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

        /// <summary>
        /// Button of bswmd folder change.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used</param>
        private void BtBswmd_Click(object sender, EventArgs e)
        {
            fdMain.RootFolder = Environment.SpecialFolder.LocalApplicationData;
            if (fdMain.ShowDialog() == DialogResult.OK)
            {
                tbBswmd.Text = fdMain.SelectedPath;
                SaveConfiguration();
            }
        }

        /// <summary>
        /// Button of instance folder change.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used</param>
        private void BtInstance_Click(object sender, EventArgs e)
        {
            fdMain.RootFolder = Environment.SpecialFolder.LocalApplicationData;
            if (fdMain.ShowDialog() == DialogResult.OK)
            {
                tbInstance.Text = fdMain.SelectedPath;
                SaveConfiguration();
            }
        }

        /// <summary>
        /// Save menu item click handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used</param>
        private void MnSave_Click(object sender, EventArgs e)
        {
            if (instanceManager != null)
            {
                instanceManager.Save();
            }
        }

        /// <summary>
        /// Reload menu item click handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used</param>
        private void MnReload_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        /// <summary>
        /// Quit menu item click handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used</param>
        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Script menu item click handler.
        /// </summary>
        /// <param name="sender">Menu item clicked</param>
        /// <param name="e">Mouse event.</param>
        private void MnScriptClickEventHandler(object? sender, MouseEventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is string file)
                {
                    CSScript.CacheEnabled = false;
                    dynamic script = CSScript.Evaluator.LoadFile(file);
                    Console.WriteLine($"Start to execute script {file}");
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
                }
            }
        }

        /// <summary>
        /// Form closing event handler.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void WdMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfiguration();
        }
    }
}
