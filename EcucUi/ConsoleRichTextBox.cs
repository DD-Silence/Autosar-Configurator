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

using System.Text;

namespace Ecuc.EcucUi
{
    /// <summary>
    /// RichTextBox to display console information
    /// </summary>
    public class ConsoleRichTextBox : TextWriter
    {
        /// <summary>
        /// Embedded rich textbox.
        /// </summary>
        RichTextBox TextBox { get; }
        /// <summary>
        /// Write delegate type.
        /// </summary>
        /// <param name="value">Value to display.</param>
        delegate void WriteFunc(string value);
        /// <summary>
        /// Write function.
        /// </summary>
        private readonly WriteFunc writeFunc;
        /// <summary>
        /// Write line function.
        /// </summary>
        private readonly WriteFunc writeLineFunc;
        /// <summary>
        /// Popup menu in rich textbox.
        /// </summary>
        private readonly ContextMenuStrip cm;
        /// <summary>
        /// Clear menu item.
        /// </summary>
        private readonly ToolStripMenuItem cmClear;

        /// <summary>
        /// Initialize console rich textbox.
        /// </summary>
        /// <param name="textBox">embedded rich textbox.</param>
        public ConsoleRichTextBox(RichTextBox textBox)
        {
            // Handle input
            TextBox = textBox;
            writeFunc = Write;
            writeLineFunc = WriteLine;

            // Prepare controls.
            cm = new ContextMenuStrip();
            cmClear = new ToolStripMenuItem();

            cm.ImageScalingSize = new Size(20, 20);
            cm.Items.AddRange(new ToolStripItem[] {cmClear});
            cm.Name = "cmTextBox";
            cm.Size = new Size(127, 58);

            cmClear.Name = "cmTextBoxClear";
            cmClear.Size = new Size(126, 24);
            cmClear.Text = "Clear";
            cmClear.MouseDown += CmClearEventHandler;
            TextBox.ContextMenuStrip = cm;
        }

        /// <summary>
        /// Encoding in UTF-8.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Write function.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public override void Write(string? value)
        {
            if (value == null)
            {
                return;
            }

            if (TextBox.InvokeRequired == true)
            {
                TextBox.BeginInvoke(writeFunc, value);
            }
            else
            {
                TextBox.AppendText($"[{DateTime.Now}]{value}");
            }
        }

        /// <summary>
        /// Write line function.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public override void WriteLine(string? value)
        {
            if (value == null)
            {
                return;
            }

            if (TextBox.InvokeRequired == true)
            {
                TextBox.BeginInvoke(writeLineFunc, value);
            }
            else
            {
                TextBox.AppendText($"[{DateTime.Now}]{value}{NewLine}");
            }
        }

        /// <summary>
        /// Clear operation to clear all texts in console.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void CmClearEventHandler(object? sender, MouseEventArgs e)
        {
            TextBox.Clear();
        }
    }
}