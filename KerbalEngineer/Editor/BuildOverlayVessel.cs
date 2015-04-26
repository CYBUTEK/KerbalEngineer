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

#endregion

namespace KerbalEngineer.Editor
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using Helpers;
    using UnityEngine;
    using VesselSimulator;

    #endregion

    public class BuildOverlayVessel : MonoBehaviour
    {
        #region Constants

        private const float Width = 175.0f;

        #endregion

        #region Fields

        private static bool visible = true;

        private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

        private Stage lastStage;
        private bool open = true;
        private float openPercent;
        private GUIContent tabContent;
        private Rect tabPosition;
        private Vector2 tabSize;
        private Rect windowPosition = new Rect(330.0f, 0.0f, Width, 0.0f);

        #endregion

        #region Properties

        public static bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public bool Open
        {
            get { return this.open; }
            set { this.open = value; }
        }

        public Rect WindowPosition
        {
            get { return this.windowPosition; }
        }

        public float WindowX
        {
            get { return this.windowPosition.x; }
            set { this.windowPosition.x = value; }
        }

        #endregion

        #region Methods

        protected void Awake()
        {
            try
            {
                SimManager.OnReady -= this.GetStageInfo;
                SimManager.OnReady += this.GetStageInfo;
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
                if (!Visible || EditorLogic.RootPart == null || EditorLogic.fetch.editorScreen != EditorScreen.Parts)
                {
                    return;
                }

                this.open = GUI.Toggle(this.tabPosition, this.open, this.tabContent, BuildOverlay.TabStyle);
                if (this.openPercent > 0.0)
                {
                    this.windowPosition = GUILayout.Window(this.GetInstanceID(), this.windowPosition, this.VesselWindow, String.Empty, BuildOverlay.WindowStyle);
                }
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
                this.tabContent = new GUIContent("VESSEL");
                this.tabSize = BuildOverlay.TabStyle.CalcSize(this.tabContent);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Update()
        {
            try
            {
                if (!Visible || EditorLogic.RootPart == null)
                {
                    return;
                }

                if (this.openPercent > 0.0)
                {
                    this.SetVesselInfo();
                }

                this.SetSlidePosition();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void GetStageInfo()
        {
            this.lastStage = SimManager.LastStage;
        }

        private void SetSlidePosition()
        {
            if (this.open && this.openPercent < 1.0f)
            {
                this.openPercent = Mathf.Clamp(this.openPercent + Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }
            else if (!this.open && this.openPercent > 0.0f)
            {
                this.openPercent = Mathf.Clamp(this.openPercent - Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }

            this.windowPosition.y = Mathf.Lerp(Screen.height, Screen.height - this.windowPosition.height, this.openPercent);
            if (this.windowPosition.width < Width)
            {
                this.windowPosition.width = Width;
            }
            this.tabPosition.width = this.tabSize.x;
            this.tabPosition.height = this.tabSize.y;
            this.tabPosition.x = this.windowPosition.x;
            this.tabPosition.y = this.windowPosition.y - this.tabPosition.height;
        }

        private void SetVesselInfo()
        {
            SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;

            if (BuildAdvanced.Instance.ShowAtmosphericDetails)
            {
                SimManager.Atmosphere = CelestialBodies.SelectedBody.GetAtmospheres(BuildAdvanced.Altitude);
            }
            else
            {
                SimManager.Atmosphere = 0.0;
            }

            SimManager.RequestSimulation();
            SimManager.TryStartSimulation();

            if (this.lastStage != null)
            {
                this.infoItems.Clear();
                this.infoItems.Add(new PartInfoItem("Delta-V", this.lastStage.deltaV.ToString("N0") + " / " + this.lastStage.totalDeltaV.ToString("N0") + "m/s"));
                this.infoItems.Add(new PartInfoItem("Mass", Units.ToMass(this.lastStage.mass, this.lastStage.totalMass)));
                this.infoItems.Add(new PartInfoItem("TWR", this.lastStage.thrustToWeight.ToString("F2") + " (" + this.lastStage.maxThrustToWeight.ToString("F2") + ")"));
                this.infoItems.Add(new PartInfoItem("Parts", this.lastStage.partCount + " / " + this.lastStage.totalPartCount));
            }
        }

        private void VesselWindow(int windowId)
        {
            try
            {
                var firstItem = true;
                foreach (var item in this.infoItems)
                {
                    if (!firstItem)
                    {
                        GUILayout.Space(2.0f);
                    }
                    firstItem = false;

                    GUILayout.BeginHorizontal();
                    if (item.Value != null)
                    {
                        GUILayout.Label(item.Name + ":", BuildOverlay.NameStyle);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(item.Value, BuildOverlay.ValueStyle);
                    }
                    else
                    {
                        GUILayout.Label(item.Name, BuildOverlay.NameStyle);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}