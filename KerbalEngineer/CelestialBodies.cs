// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;

namespace KerbalEngineer
{
    public class CelestialBodies
    {
        #region Instance

        private static CelestialBodies _instance;
        /// <summary>
        /// Gets or creates a global instance to be used.
        /// </summary>
        public static CelestialBodies Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CelestialBodies();

                return _instance;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of BodyInfo objects.
        /// </summary>
        public Dictionary<string, BodyInfo> BodyList { get; private set; }

        private string _selectedBodyName = null;
        /// <summary>
        /// Gets and sets the selected body name.
        /// </summary>
        public string SelectedBodyName
        {
            get { return _selectedBodyName; }
            set
            {
                _selectedBodyName = value;
                if (BodyList.ContainsKey(_selectedBodyName))
                    SelectedBodyInfo = BodyList[_selectedBodyName];
            }
        }

        /// <summary>
        /// Gets the selected BodyInfo object.
        /// </summary>
        public BodyInfo SelectedBodyInfo { get; private set; }

        #endregion

        #region Initialisation

        private CelestialBodies()
        {
            BodyList = new Dictionary<string, BodyInfo>();
            AddBody(new BodyInfo("Moho", 2.7d, 0d, null, null));
            AddBody(new BodyInfo("Eve", 16.7d, 506.625d, null, new string[] { "Gilly" }));
            AddBody(new BodyInfo("Gilly", 0.049d, 0d, "Eve", null));
            AddBody(new BodyInfo("Kerbin", 9.81d, 101.325d, null, new string[] { "Mun", "Minmus" }));
            AddBody(new BodyInfo("Mun", 1.63d, 0d, "Kerbin", null));
            AddBody(new BodyInfo("Minmus", 0.491d, 0d, "Kerbin", null));
            AddBody(new BodyInfo("Duna", 2.94d, 20.2650d, null, new string[] { "Ike" }));
            AddBody(new BodyInfo("Ike", 1.1d, 0d, "Duna", null));
            AddBody(new BodyInfo("Dres", 1.13d, 0d, null, null));
            AddBody(new BodyInfo("Jool", 7.85d, 1519.88d, null, new string[] { "Laythe", "Vall", "Tylo", "Bop", "Pol" }));
            AddBody(new BodyInfo("Laythe", 7.85d, 81.06d, "Jool", null));
            AddBody(new BodyInfo("Vall", 2.31d, 0d, "Jool", null));
            AddBody(new BodyInfo("Tylo", 7.85d, 0d, "Jool", null));
            AddBody(new BodyInfo("Bop", 0.589d, 0d, "Jool", null));
            AddBody(new BodyInfo("Pol", 0.373d, 0d, "Jool", null));
            AddBody(new BodyInfo("Eeloo", 1.69d, 0d, null, null));

            SelectedBodyName = "Kerbin";
        }

        private void AddBody(BodyInfo bodyInfo)
        {
            BodyList.Add(bodyInfo.Name, bodyInfo);
        }

        #endregion

        #region Embedded Classes

        public class BodyInfo
        {
            public string Name { get; protected set; }
            public double Gravity { get; protected set; }
            public double Atmosphere { get; protected set; }
            public string Parent { get; protected set; }
            public string[] Children { get; protected set; }

            public BodyInfo(string name, double gravity, double atmosphere, string parent, string[] children)
            {
                Name = name;
                Gravity = gravity;
                Atmosphere = atmosphere;
                Parent = parent;
                Children = children;
            }
        }

        #endregion
    }
}
