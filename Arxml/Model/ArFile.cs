/*  
 *  This file is a part of Arxml Editor.
 *  
 *  Copyright (C) 2021-2023 DJS Studio E-mail: ddsilence@sina.cn
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

using GenTool_CsDataServerAsrBase;
using GenTool_CsDataServerDomAsr4;
using GenTool_CsDataServerDomAsr4.Iface;

namespace Arxml.Model
{
    internal class ArFile
    {
        private readonly List<string> paths = new();
        public IAUTOSAR? root = null;

        public bool IsEmpty
        {
            get
            {
                return paths.Count == 0;
            }
        }

        public List<IARPackage> Packages
        {
            get
            {
                List<IARPackage> result = new();

                if (root != null)
                {
                    foreach (var obj in root.AllObjects)
                    {
                        if (obj is IARPackage arPackage)
                        {
                            result.Add(arPackage);
                        }
                    }
                }
                return result;
            }
        }


        public ArFile()
        {
        }

        public ArFile(string filePath)
        {
            AddFile(filePath);
        }

        public ArFile(string[] filePaths)
        {
            AddFile(filePaths);
        }

        private void Load()
        {
            var domain = DomainFactory.Instance.Create();
            if (domain != null)
            {
                domain.Load(paths.ToArray(), false);
                root = domain.Model;
            }
            else
            {
                throw new Exception("Fail to create domain.");
            }
        }

        public void AddFile(string filePath)
        {
            paths.Add(filePath);
            Load();
        }

        public void AddFile(string[] filePaths)
        {
            paths.AddRange(filePaths);
            Load();
        }

        public void Save()
        {
            root?.Domain.Save();
        }

        public void Clear()
        {
            paths.Clear();
            root = null;
        }

        public void Reload()
        {
            root?.Domain.Reload();
        }

        public void NewFile(string filePath, AsrVersion version)
        {
            Clear();
            paths.Add(filePath);
            GenTool_CsDataServerDomAsr4.Iface.IDomain domain = DomainFactory.Instance.Create();
            var file = domain.New(filePath);
            file.AsrVersion = version;
            root = domain.Model;
            domain.Save();
        }
    }
}
