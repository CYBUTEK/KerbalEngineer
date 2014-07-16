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
        public const string AssemblyVersion = "1.0.0.0";

        /// <summary>
        ///     Current version of Kerbal Engineer Redux.
        /// </summary>
        public const string PrettyVersion = "1.0 alpha";

        #endregion

        #region Fields

        private static int _windowId = int.MaxValue;

        #endregion

        #region Properties

        private static string _assemblyFile;
        private static string _assemblyName;
        private static string _assemblyPath;

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's path including the file name.
        /// </summary>
        public static string AssemblyFile
        {
            get { return _assemblyFile ?? (_assemblyFile = Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's file name.
        /// </summary>
        public static string AssemblyName
        {
            get { return _assemblyName ?? (_assemblyName = new FileInfo(AssemblyFile).Name); }
        }

        /// <summary>
        ///     Gets the Kerbal Engineer assembly's path excluding the file name.
        /// </summary>
        public static string AssemblyPath
        {
            get { return _assemblyPath ?? (_assemblyPath = AssemblyFile.Replace(new FileInfo(AssemblyFile).Name, "")); }
        }

        #endregion

        #region Methods

        public static int GetNextWindowId()
        {
            _windowId--;
            return _windowId;
        }

        #endregion
    }
}