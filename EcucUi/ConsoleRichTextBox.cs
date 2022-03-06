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
using System.ComponentModel;
using System.Text;

namespace Ecuc.EcucUi
{
    public class ConsoleRichTextBox : TextWriter
    {
        RichTextBox TextBox { get; }
        delegate void WriteFunc(string value);
        WriteFunc writeFunc;
        WriteFunc writeLineFunc;
        private readonly ContextMenuStrip cm;
        private readonly ToolStripMenuItem cmClear;

        public ConsoleRichTextBox(RichTextBox textBox)
        {
            TextBox = textBox;
            writeFunc = Write;
            writeLineFunc = WriteLine;

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

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

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
                TextBox.AppendText(value);
            }
        }

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
                TextBox.AppendText(value);
                TextBox.AppendText(NewLine);
            }
        }

        private void CmClearEventHandler(object? sender, MouseEventArgs e)
        {
            TextBox.Clear();
        }
    }
}