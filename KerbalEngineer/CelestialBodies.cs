// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;

#endregion

namespace KerbalEngineer
{
    public class CelestialBodies
    {
        #region Instance

        private static CelestialBodies _instance;

        /// <summary>
        ///     Gets or creates a global instance to be used.
        /// </summary>
        public static CelestialBodies Instance
        {
            get { return _instance ?? (_instance = new CelestialBodies()); }
        }

        #endregion

        #region Fields

        private string selectedBodyName;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a list of BodyInfo objects.
        /// </summary>
        public Dictionary<string, BodyInfo> BodyList { get; private set; }

        /// <summary>
        ///     Gets and sets the selected body name.
        /// </summary>
        public string SelectedBodyName
        {
            get { return this.selectedBodyName; }
            set
            {
                this.selectedBodyName = value;
                if (this.BodyList.ContainsKey(this.selectedBodyName))
                {
                    this.SelectedBodyInfo = this.BodyList[this.selectedBodyName];
                }
            }
        }

        /// <summary>
        ///     Gets the selected BodyInfo object.
        /// </summary>
        public BodyInfo SelectedBodyInfo { get; private set; }

        #endregion

        #region Initialisation

        private CelestialBodies()
        {
            this.BodyList = new Dictionary<string, BodyInfo>();
            this.AddBody(new BodyInfo("Moho", 2.7d, 0, null, null));
            this.AddBody(new BodyInfo("Eve", 16.7d, 506.625d, null, new[] {"Gilly"}));
            this.AddBody(new BodyInfo("Gilly", 0.049d, 0, "Eve", null));
            this.AddBody(new BodyInfo("Kerbin", 9.81d, 101.325d, null, new[] {"Mun", "Minmus"}));
            this.AddBody(new BodyInfo("Mun", 1.63d, 0, "Kerbin", null));
            this.AddBody(new BodyInfo("Minmus", 0.491d, 0, "Kerbin", null));
            this.AddBody(new BodyInfo("Duna", 2.94d, 20.2650d, null, new[] {"Ike"}));
            this.AddBody(new BodyInfo("Ike", 1.1d, 0, "Duna", null));
            this.AddBody(new BodyInfo("Dres", 1.13d, 0, null, null));
            this.AddBody(new BodyInfo("Jool", 7.85d, 1519.88d, null, new[] {"Laythe", "Vall", "Tylo", "Bop", "Pol"}));
            this.AddBody(new BodyInfo("Laythe", 7.85d, 81.06d, "Jool", null));
            this.AddBody(new BodyInfo("Vall", 2.31d, 0, "Jool", null));
            this.AddBody(new BodyInfo("Tylo", 7.85d, 0, "Jool", null));
            this.AddBody(new BodyInfo("Bop", 0.589d, 0, "Jool", null));
            this.AddBody(new BodyInfo("Pol", 0.373d, 0, "Jool", null));
            this.AddBody(new BodyInfo("Eeloo", 1.69d, 0, null, null));

            this.SelectedBodyName = "Kerbin";
        }

        #endregion

        #region Methods

        private void AddBody(BodyInfo bodyInfo)
        {
            this.BodyList.Add(bodyInfo.Name, bodyInfo);
        }

        #endregion

        #region Embedded Classes

        public class BodyInfo
        {
            public BodyInfo(string name, double gravity, double atmosphere, string parent, string[] children)
            {
                this.Name = name;
                this.Gravity = gravity;
                this.Atmosphere = atmosphere;
                this.Parent = parent;
                this.Children = children;
            }

            public string Name { get; protected set; }
            public double Gravity { get; protected set; }
            public double Atmosphere { get; protected set; }
            public string Parent { get; protected set; }
            public string[] Children { get; protected set; }
        }

        #endregion
    }
}