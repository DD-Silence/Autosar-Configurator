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

using Meta.Iface;
using System.Reflection;

namespace Arxml.Model
{
    /// <summary>
    /// 
    /// </summary>
    public static class IMetaRIExtend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool Single(this IMetaRI role)
        {
            return ((uint)role.MaxOccurs == 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool Multiply(this IMetaRI role)
        {
            return ((uint)role.MaxOccurs > 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool Option(this IMetaRI role)
        {
            return ((uint)role.MinOccurs == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool Required(this IMetaRI role)
        {
            return ((uint)role.MinOccurs > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool SingleOption(this IMetaRI role)
        {
            return Single(role) & Option(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool SingleRequired(this IMetaRI role)
        {
            return Single(role) & Required(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool MultiplyOption(this IMetaRI role)
        {
            return Multiply(role) & Option(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool MultiplyRequired(this IMetaRI role)
        {
            return Multiply(role) & Required(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string Min(this IMetaRI role)
        {
            return role.MinOccurs.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string Max(this IMetaRI role)
        {
            if (role.MaxOccurs == -1)
            {
                return "Inf";
            }
            return role.MaxOccurs.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsMeta(this IMetaRI role)
        {
            foreach (var i in role.InterfaceType.GetTypeInfo().ImplementedInterfaces)
            {
                if (i.Name == "IMetaObjectInstance")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsEnum(this IMetaRI role)
        {
            if (role.InterfaceType.BaseType == null)
            {
                return false;
            }
            if (role.InterfaceType.BaseType.Name == "Enum")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsBool(this IMetaRI role)
        {
            if (role.InterfaceType.Name == "Boolean")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsInteger(this IMetaRI role)
        {
            if (role.InterfaceType.Name == "AsrInt")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsRefernce(this IMetaRI role)
        {
            if (role.InterfaceType.Name.EndsWith("_Ref"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool IsOther(this IMetaRI role)
        {
            return (!role.IsMeta()) && (!role.IsEnum()) && (!role.IsBool()) && (!role.IsInteger());
        }
    }
}
