// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System.IO;
using System.Reflection;

#endregion

namespace KerbalEngineer
{
    public class EngineerGlobals
    {
        #region Constants

        /// <summary>
        ///     Current version of the Kerbal Engineer assembly.
        /// </summary>
        public const string AssemblyVersion = "1.0.16.6";

        #endregion

        #region Fields

        private static string assemblyFile;
        private static string assemblyName;
        private static string assemblyPath;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's path including the file name.
        /// </summary>
        public static string AssemblyFile
        {
            get { return assemblyFile ?? (assemblyFile = Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's file name.
        /// </summary>
        public static string AssemblyName
        {
            get { return assemblyName ?? (assemblyName = new FileInfo(AssemblyFile).Name); }
        }

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's path excluding the file name.
        /// </summary>
        public static string AssemblyPath
        {
            get { return assemblyPath ?? (assemblyPath = AssemblyFile.Replace(new FileInfo(AssemblyFile).Name, "")); }
        }

        #endregion
    }
}