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

using System;
using System.Collections.Generic;

using KerbalEngineer.Control.Panels;

using UnityEngine;

#endregion

namespace KerbalEngineer.Control
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ControlCentre : MonoBehaviour
    {
        #region Fields

        private static readonly List<IControlPanel> panels = new List<IControlPanel>();

        private static GUIStyle button;
        private static ControlCentre instance;
        private static GUIStyle label;
        private static GUIStyle title;

        private Vector2 contentsScrollPosition;
        private GUIStyle panelSelectorStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 900.0f, 500.0f);
        private IControlPanel selectedPanel;
        private bool shouldCentre = true;

        #endregion

        #region Properties

        public static GUIStyle Button
        {
            get
            {
                return button ?? (button = new GUIStyle(HighLogic.Skin.button)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fixedHeight = 30.0f
                });
            }
        }

        public static bool Enabled
        {
            get { return instance.enabled; }
            set { instance.enabled = value; }
        }

        public static GUIStyle Label
        {
            get
            {
                return label ?? (label = new GUIStyle(HighLogic.Skin.label)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 30.0f,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true,
                });
            }
        }

        public static List<IControlPanel> Panels
        {
            get { return panels; }
        }

        public static GUIStyle Title
        {
            get
            {
                return title ?? (title = new GUIStyle(HighLogic.Skin.label)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 26,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperCenter,
                    stretchWidth = true,
                });
            }
        }

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                if (instance == null)
                {
                    DontDestroyOnLoad(this);
                    instance = this;
                    this.enabled = false;
                    return;
                }
                Destroy(this);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void OnGUI()
        {
            try
            {
                GUI.skin = null;
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KERBAL ENGINEER REDUX " + EngineerGlobals.ASSEMBLY_VERSION + " - CONTROL CENTRE", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Start()
        {
            try
            {
                this.InitialiseStyles();
                LoadPanels();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private static void LoadPanels()
        {
            panels.Add(new BuildEngineerPanel());
            panels.Add(new BuildOverlayPanel());
        }

        private void CentreWindow()
        {
            if (this.shouldCentre && this.position.width > 0.0f && this.position.height > 0.0f)
            {
                this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                this.shouldCentre = false;
            }
        }

        private void DrawContents()
        {
            GUI.skin = HighLogic.Skin;
            this.contentsScrollPosition = GUILayout.BeginScrollView(this.contentsScrollPosition, false, true);
            GUI.skin = null;

            if (this.selectedPanel != null)
            {
                this.selectedPanel.Draw();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }

        private void DrawSelectors()
        {
            GUILayout.BeginVertical(HighLogic.Skin.box, GUILayout.Width(225.0f));
            foreach (var panel in panels)
            {
                if (GUILayout.Toggle(this.selectedPanel == panel, panel.Name, this.panelSelectorStyle))
                {
                    this.selectedPanel = panel;
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("CLOSE", Button))
            {
                this.enabled = false;
            }
            GUILayout.EndVertical();
        }

        private void InitialiseStyles()
        {
            this.panelSelectorStyle = new GUIStyle(Button)
            {
                fontSize = 16,
                fixedHeight = 40.0f
            };
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.BeginHorizontal();
                this.DrawSelectors();
                this.DrawContents();
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}