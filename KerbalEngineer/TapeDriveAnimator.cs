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

        private Shader buttonLightOffShader;
        private Shader buttonLightOnShader;
        private Material buttonSet1Material;
        private Material buttonSet2Material;
        private Material buttonSet3Material;
        private Material buttonSet4Material;
        private Material buttonSet5Material;
        private Material buttonSet6Material;
        private float currentTime;
        private float deltaTime;
        private bool isRunning;
        private Random random;
        private Transform reel1Transform;
        private Transform reel2Transform;
        private float repeatTime;
        private bool sceneIsEditor;
        private float speed;
        private float targetSpeed;

        public bool IsRunning
        {
            get
            {
                return isRunning;
                return isRunning;
            }
            set
            {
                isRunning = value;

                if (isRunning)
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
            random = new Random();

            StopBakedAnimation();
            IsRunning = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                part.OnEditorAttach += OnEditorAttach;
                part.OnEditorDetach += OnEditorDetach;

                sceneIsEditor = true;

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

            deltaTime = sceneIsEditor ? Time.deltaTime : TimeWarp.deltaTime;

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
                targetSpeed = 0;

                if (speed != 0)
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
            buttonSet1Material = GetMaterialOnModelTransform(Lights1);
            buttonSet2Material = GetMaterialOnModelTransform(Lights2);
            buttonSet3Material = GetMaterialOnModelTransform(Lights3);
            buttonSet4Material = GetMaterialOnModelTransform(Lights4);
            buttonSet5Material = GetMaterialOnModelTransform(Lights5);
            buttonSet6Material = GetMaterialOnModelTransform(Lights6);

            buttonLightOffShader = Shader.Find("KSP/Specular");
            buttonLightOnShader = Shader.Find("KSP/Unlit");
        }

        private void InitialiseReels()
        {
            if (string.IsNullOrEmpty(Reel1) == false)
            {
                reel1Transform = part.FindModelTransform(Reel1);
            }

            if (string.IsNullOrEmpty(Reel2) == false)
            {
                reel2Transform = part.FindModelTransform(Reel2);
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
            if (sceneIsEditor)
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
                lightsOn = (speed > targetSpeed);
            }
            else if (targetSpeed < 0)
            {
                lightsOn = (speed < targetSpeed);
            }
            else
            {
                lightsOn = (speed == 0);
            }

            SetShaderOnMaterial(material, lightsOn ? buttonLightOnShader : buttonLightOffShader);
        }

        private void UpdateLights()
        {
            UpdateButtonMaterial(buttonSet1Material, Lights1Speed);
            UpdateButtonMaterial(buttonSet2Material, Lights2Speed);
            UpdateButtonMaterial(buttonSet3Material, Lights3Speed);
            UpdateButtonMaterial(buttonSet4Material, Lights4Speed);
            UpdateButtonMaterial(buttonSet5Material, Lights5Speed);
            UpdateButtonMaterial(buttonSet6Material, Lights6Speed);
        }

        private void UpdateReels()
        {
            if (reel1Transform != null && speed != 0)
            {
                reel1Transform.transform.Rotate(Vector3.right, speed * Reel1SpeedRatio);
            }

            if (reel2Transform != null && speed != 0)
            {
                reel2Transform.transform.Rotate(Vector3.right, speed * Reel2SpeedRatio);
            }
        }

        private void UpdateSpeed()
        {
            if (speed < targetSpeed)
            {
                if (speed < targetSpeed - SpeedDeadZone)
                {
                    speed += SpeedChangeAmount * deltaTime;
                }
                else
                {
                    speed = targetSpeed;
                }
            }
            else if (speed > targetSpeed)
            {
                if (speed > targetSpeed + SpeedDeadZone)
                {
                    speed -= SpeedChangeAmount * deltaTime;
                }
                else
                {
                    speed = targetSpeed;
                }
            }
        }

        private void UpdateTimerCycle()
        {
            currentTime += deltaTime;

            if (currentTime >= repeatTime)
            {
                targetSpeed = random.Next(MinReelSpeed, MaxReelSpeed);

                if (targetSpeed > -SpeedStopZone && targetSpeed < SpeedStopZone)
                {
                    targetSpeed = 0;
                }

                repeatTime = random.Next(MinRepeatTime, MaxRepeatTime);

                if (RepeatTimeDenominator != 0)
                {
                    repeatTime /= RepeatTimeDenominator;
                }

                currentTime -= repeatTime;
            }
        }
    }
}