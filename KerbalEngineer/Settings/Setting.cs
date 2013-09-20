// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;

namespace KerbalEngineer.Settings
{
    [Serializable]
    public class Setting
    {
        #region Properties

        /// <summary>
        /// Gets and sets the name of the setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the setting's value object.
        /// </summary>
        public object Value { get; set; }

        #endregion

        #region Initialisation

        public Setting(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion
    }
}
