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

namespace KerbalEngineer
{
    using UnityEngine;
    using Random = System.Random;

    public class TapeDriveAnimator : PartModule
    {
        [KSPField]
        public string Lights1 = string.Empty;

        [KSPField]
        public float Lights1Speed = 0;

        [KSPField]
        public string Lights2 = string.Empty;

        [KSPField]
        public float Lights2Speed = 0;

        [KSPField]
        public string Lights3 = string.Empty;

        [KSPField]
        public float Lights3Speed = 0;

        [KSPField]
        public string Lights4 = string.Empty;

        [KSPField]
        public float Lights4Speed = 0;

        [KSPField]
        public string Lights5 = string.Empty;

        [KSPField]
        public float Lights5Speed = 0;

        [KSPField]
        public string Lights6 = string.Empty;

        [KSPField]
        public float Lights6Speed = 0;

        [KSPField]
        public int MaxReelSpeed = 0;

        [KSPField]
        public int MaxRepeatTime = 0;

        [KSPField]
        public int MinReelSpeed = 0;

        [KSPField]
        public int MinRepeatTime = 0;

        [KSPField]
        public string Reel1 = string.Empty;

        [KSPField]
        public float Reel1SpeedRatio = 1;

        [KSPField]
        public string Reel2 = string.Empty;

        [KSPField]
        public float Reel2SpeedRatio = 1;

        [KSPField]
        public float RepeatTimeDenominator = 1;

        [KSPField]
        public float SpeedChangeAmount = 0;

        [KSPField]
        public float SpeedDeadZone = 0;

        [KSPField]
        public float SpeedStopZone = 0;

        [KSPField]
        public bool UseBakedAnimation = false;

        private Shader m_ButtonLightOffShader;
        private Shader m_ButtonLightOnShader;
        private Material m_ButtonSet1Material;
        private Material m_ButtonSet2Material;
        private Material m_ButtonSet3Material;
        private Material m_ButtonSet4Material;
        private Material m_ButtonSet5Material;
        private Material m_ButtonSet6Material;
        private float m_CurrentTime;
        private float m_DeltaTime;
        private bool m_IsRunning;
        private Random m_Random;
        private Transform m_Reel1Transform;
        private Transform m_Reel2Transform;
        private float m_RepeatTime;
        private bool m_SceneIsEditor;
        private float m_Speed;
        private float m_TargetSpeed;

        public bool IsRunning
        {
            get
            {
                return m_IsRunning;
            }
            set
            {
                m_IsRunning = value;

                if (m_IsRunning)
                {
                    if (UseBakedAnimation)
                    {
                        StartBakedAnimation();
                    }
                }
                else
                {
                    if (UseBakedAnimation)
                    {
                        StopBakedAnimation();
                    }
                }
            }
        }

        public override void OnStart(StartState state)
        {
            m_Random = new Random();

            StopBakedAnimation();
            IsRunning = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                part.OnEditorAttach += OnEditorAttach;
                part.OnEditorDetach += OnEditorDetach;

                m_SceneIsEditor = true;

                if (part.parent != null)
                {
                    IsRunning = true;
                }
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                IsRunning = true;
            }

            if (UseBakedAnimation == false)
            {
                InitialiseReels();
                InitialiseLights();
            }
        }

        public override void OnUpdate()
        {
            if (UseBakedAnimation)
            {
                return;
            }

            m_DeltaTime = m_SceneIsEditor ? Time.deltaTime : TimeWarp.deltaTime;

            if (TimeWarp.CurrentRate != 1.0f && TimeWarp.WarpMode != TimeWarp.Modes.LOW)
            {
                return;
            }

            if (IsRunning)
            {
                UpdateTimerCycle();
                UpdateSpeed();
                UpdateReels();
                UpdateLights();
            }
            else
            {
                m_TargetSpeed = 0;

                if (m_Speed != 0)
                {
                    UpdateSpeed();
                    UpdateReels();
                    UpdateLights();
                }
            }
        }

        private static void SetShaderOnMaterial(Material material, Shader shader)
        {
            if (material != null && shader != null)
            {
                material.shader = shader;
            }
        }

        private Material GetMaterialOnModelTransform(string transformName)
        {
            Transform modelTransform = GetModelTransform(transformName);
            if (modelTransform != null)
            {
                Renderer renderer = modelTransform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    return renderer.material;
                }
            }

            return null;
        }

        private Transform GetModelTransform(string transformName)
        {
            if (string.IsNullOrEmpty(transformName) == false)
            {
                return part.FindModelTransform(transformName);
            }

            return null;
        }

        private void InitialiseLights()
        {
            m_ButtonSet1Material = GetMaterialOnModelTransform(Lights1);
            m_ButtonSet2Material = GetMaterialOnModelTransform(Lights2);
            m_ButtonSet3Material = GetMaterialOnModelTransform(Lights3);
            m_ButtonSet4Material = GetMaterialOnModelTransform(Lights4);
            m_ButtonSet5Material = GetMaterialOnModelTransform(Lights5);
            m_ButtonSet6Material = GetMaterialOnModelTransform(Lights6);

            m_ButtonLightOffShader = Shader.Find("KSP/Specular");
            m_ButtonLightOnShader = Shader.Find("KSP/Unlit");
        }

        private void InitialiseReels()
        {
            if (string.IsNullOrEmpty(Reel1) == false)
            {
                m_Reel1Transform = part.FindModelTransform(Reel1);
            }

            if (string.IsNullOrEmpty(Reel2) == false)
            {
                m_Reel2Transform = part.FindModelTransform(Reel2);
            }
        }

        private void OnEditorAttach()
        {
            IsRunning = true;
        }

        private void OnEditorDetach()
        {
            IsRunning = false;
        }

        private void StartBakedAnimation()
        {
            foreach (Animation animator in part.FindModelAnimators())
            {
                animator.Play();
            }
        }

        private void StopBakedAnimation()
        {
            foreach (Animation animator in part.FindModelAnimators())
            {
                animator.Stop();
            }
        }

        private void Update()
        {
            if (m_SceneIsEditor)
            {
                OnUpdate();
            }
        }

        private void UpdateButtonMaterial(Material material, float targetSpeed)
        {
            if (material == null)
            {
                return;
            }

            bool lightsOn;

            if (targetSpeed > 0)
            {
                lightsOn = (m_Speed > targetSpeed);
            }
            else if (targetSpeed < 0)
            {
                lightsOn = (m_Speed < targetSpeed);
            }
            else
            {
                lightsOn = (m_Speed == 0);
            }

            SetShaderOnMaterial(material, lightsOn ? m_ButtonLightOnShader : m_ButtonLightOffShader);
        }

        private void UpdateLights()
        {
            UpdateButtonMaterial(m_ButtonSet1Material, Lights1Speed);
            UpdateButtonMaterial(m_ButtonSet2Material, Lights2Speed);
            UpdateButtonMaterial(m_ButtonSet3Material, Lights3Speed);
            UpdateButtonMaterial(m_ButtonSet4Material, Lights4Speed);
            UpdateButtonMaterial(m_ButtonSet5Material, Lights5Speed);
            UpdateButtonMaterial(m_ButtonSet6Material, Lights6Speed);
        }

        private void UpdateReels()
        {
            if (m_Reel1Transform != null && m_Speed != 0)
            {
                m_Reel1Transform.transform.Rotate(Vector3.right, m_Speed * Reel1SpeedRatio);
            }

            if (m_Reel2Transform != null && m_Speed != 0)
            {
                m_Reel2Transform.transform.Rotate(Vector3.right, m_Speed * Reel2SpeedRatio);
            }
        }

        private void UpdateSpeed()
        {
            if (m_Speed < m_TargetSpeed)
            {
                if (m_Speed < m_TargetSpeed - SpeedDeadZone)
                {
                    m_Speed += SpeedChangeAmount * m_DeltaTime;
                }
                else
                {
                    m_Speed = m_TargetSpeed;
                }
            }
            else if (m_Speed > m_TargetSpeed)
            {
                if (m_Speed > m_TargetSpeed + SpeedDeadZone)
                {
                    m_Speed -= SpeedChangeAmount * m_DeltaTime;
                }
                else
                {
                    m_Speed = m_TargetSpeed;
                }
            }
        }

        private void UpdateTimerCycle()
        {
            m_CurrentTime += m_DeltaTime;

            if (m_CurrentTime >= m_RepeatTime)
            {
                m_TargetSpeed = m_Random.Next(MinReelSpeed, MaxReelSpeed);

                if (m_TargetSpeed > -SpeedStopZone && m_TargetSpeed < SpeedStopZone)
                {
                    m_TargetSpeed = 0;
                }

                m_RepeatTime = m_Random.Next(MinRepeatTime, MaxRepeatTime);

                if (RepeatTimeDenominator != 0)
                {
                    m_RepeatTime /= RepeatTimeDenominator;
                }

                m_CurrentTime -= m_RepeatTime;
            }
        }
    }
}