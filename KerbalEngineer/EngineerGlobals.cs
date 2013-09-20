// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.IO;
using System.Reflection;

namespace KerbalEngineer
{
    public class EngineerGlobals
    {
        #region Properties

        /// <summary>
        /// Current version of the Kerbal Engineer assembly.
        /// </summary>
        public const string AssemblyVersion = "1.0.0.0";

        private static string _assemblyFile;
        /// <summary>
        /// Gets the Kerbal Engineer assembly's path including the file name.
        /// </summary>
        public static string AssemblyFile
        {
            get
            {
                if (_assemblyFile == null)
                    _assemblyFile = Assembly.GetExecutingAssembly().Location;

                return _assemblyFile;
            }
        }

        private static string _assemblyName;
        /// <summary>
        /// Gets the Kerbal Engineer assembly's file name.
        /// </summary>
        public static string AssemblyName
        {
            get
            {
                if (_assemblyName == null)
                    _assemblyName = new FileInfo(AssemblyFile).Name;

                return _assemblyName;
            }
        }

        private static string _assemblyPath;
        /// <summary>
        /// Gets the Kerbal Engineer assembly's path excluding the file name.
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                if (_assemblyPath == null)
                    _assemblyPath = AssemblyFile.Replace(new FileInfo(AssemblyFile).Name, "");

                return _assemblyPath;
            }
        }

        #endregion

        #region Methods

        private static int _windowID = int.MaxValue;
        public static int GetNextWindowID()
        {
            _windowID--;
            return _windowID;
        }

        #endregion
    }
}
