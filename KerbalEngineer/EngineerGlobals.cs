// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

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