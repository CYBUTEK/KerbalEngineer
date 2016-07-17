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
        private Toggle m_DisplayToggle = null;

        [SerializeField]
        private Text m_DisplayText = null;

        [SerializeField]
        private Toggle m_EditToggle = null;

        private ISectionModule m_Section;

        /// <summary>
        ///     Gets or sets the section's editor visibility.
        /// </summary>
        public bool IsEditorVisible
        {
            get
            {
                if (m_EditToggle != null)
                {
                    return m_EditToggle.isOn;
                }

                return true;
            }
            set
            {
                if (m_EditToggle != null)
                {
                    m_EditToggle.isOn = value;
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

            m_Section = section;
        }

        /// <summary>
        ///     Sets the section's display visibility.
        /// </summary>
        public void SetDisplayVisible(bool visible)
        {
            if (m_Section != null)
            {
                m_Section.IsVisible = visible;
            }
        }

        /// <summary>
        ///     Sets the section's editor visibility.
        /// </summary>
        public void SetEditorVisible(bool visible)
        {
            if (m_Section != null)
            {
                m_Section.IsEditorVisible = visible;
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
            if (m_Section == null || m_Section.IsDeleted)
            {
                Destroy(gameObject);
                return;
            }

            // display visible
            if (m_DisplayToggle != null)
            {
                m_DisplayToggle.isOn = m_Section.IsVisible;
            }

            // display name
            if (m_DisplayText != null)
            {
                m_DisplayText.text = m_Section.Name.ToUpperInvariant();
            }

            // editor visible
            if (m_EditToggle != null)
            {
                m_EditToggle.isOn = m_Section.IsEditorVisible;
            }
        }
    }
}