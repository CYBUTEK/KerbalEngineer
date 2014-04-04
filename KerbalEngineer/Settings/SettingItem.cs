// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

#endregion

namespace KerbalEngineer.Settings
{
    /// <summary>
    ///     A serialisable object for storing an item's name and value.
    /// </summary>
    public class SettingItem
    {
        #region Constructors

        /// <summary>
        ///     Creates and empty item object.
        /// </summary>
        public SettingItem() { }

        /// <summary>
        ///     Creates an item object containing a name and value.
        /// </summary>
        public SettingItem(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets and sets the object value of the item.
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}