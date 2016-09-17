// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Unity.Flight
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FlightMenuSection : MonoBehaviour
    {
        [SerializeField]
        private Toggle displayToggle = null;

        [SerializeField]
        private Text displayText = null;

        [SerializeField]
        private Toggle editToggle = null;

        private ISectionModule section;

        /// <summary>
        ///     Gets or sets the section's editor visibility.
        /// </summary>
        public bool IsEditorVisible
        {
            get
            {
                if (editToggle != null)
                {
                    return editToggle.isOn;
                }

                return true;
            }
            set
            {
                if (editToggle != null)
                {
                    editToggle.isOn = value;
                }
            }
        }

        /// <summary>
        ///     Sets the assigned section to be handled by the menu object.
        /// </summary>
        public void SetAssignedSection(ISectionModule section)
        {
            if (section == null)
            {
                return;
            }

            this.section = section;
        }

        /// <summary>
        ///     Sets the section's display visibility.
        /// </summary>
        public void SetDisplayVisible(bool visible)
        {
            if (section != null)
            {
                section.IsVisible = visible;
            }
        }

        /// <summary>
        ///     Sets the section's editor visibility.
        /// </summary>
        public void SetEditorVisible(bool visible)
        {
            if (section != null)
            {
                section.IsEditorVisible = visible;
            }
        }

        protected virtual void Update()
        {
            UpdateControls();
        }

        /// <summary>
        ///     Updates the menu section's controls.
        /// </summary>
        private void UpdateControls()
        {
            if (section == null || section.IsDeleted)
            {
                Destroy(gameObject);
                return;
            }

            // display visible
            if (displayToggle != null)
            {
                displayToggle.isOn = section.IsVisible;
            }

            // display name
            if (displayText != null)
            {
                displayText.text = section.Name.ToUpperInvariant();
            }

            // editor visible
            if (editToggle != null)
            {
                editToggle.isOn = section.IsEditorVisible;
            }
        }
    }
}