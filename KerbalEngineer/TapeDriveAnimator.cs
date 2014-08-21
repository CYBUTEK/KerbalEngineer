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

using UnityEngine;

using Random = System.Random;

#endregion

namespace KerbalEngineer
{
    public class TapeDriveAnimator : PartModule
    {
        #region Public Fields

        [KSPField] public string Lights1 = "";
        [KSPField] public float Lights1Speed = 0;
        [KSPField] public string Lights2 = "";
        [KSPField] public float Lights2Speed = 0;
        [KSPField] public string Lights3 = "";
        [KSPField] public float Lights3Speed = 0;
        [KSPField] public string Lights4 = "";
        [KSPField] public float Lights4Speed = 0;
        [KSPField] public string Lights5 = "";
        [KSPField] public float Lights5Speed = 0;
        [KSPField] public string Lights6 = "";
        [KSPField] public float Lights6Speed = 0;
        [KSPField] public int MaxReelSpeed = 0;
        [KSPField] public int MaxRepeatTime = 0;
        [KSPField] public int MinReelSpeed = 0;
        [KSPField] public int MinRepeatTime = 0;
        [KSPField] public string Reel1 = "";
        [KSPField] public float Reel1SpeedRatio = 1;
        [KSPField] public string Reel2 = "";
        [KSPField] public float Reel2SpeedRatio = 1;
        [KSPField] public float RepeatTimeDenominator = 1;
        [KSPField] public float SpeedChangeAmount = 0;
        [KSPField] public float SpeedDeadZone = 0;
        [KSPField] public float SpeedStopZone = 0;
        [KSPField] public bool UseBakedAnimation = false;

        #endregion

        #region Private Fields

        private float currentTime;
        private float deltaTime;
        private Shader lights1ShaderOff;
        private Transform lights1Transform;
        private Shader lights2ShaderOff;
        private Transform lights2Transform;
        private Shader lights3ShaderOff;
        private Transform lights3Transform;
        private Shader lights4ShaderOff;
        private Transform lights4Transform;
        private Shader lights5ShaderOff;
        private Transform lights5Transform;
        private Shader lights6ShaderOff;
        private Transform lights6Transform;
        private Shader lightsShaderOn;
        private Random random;
        private Transform reel1Transform;
        private Transform reel2Transform;
        private float repeatTime;
        private bool sceneIsEditor;
        private float speed;
        private float targetSpeed;

        #endregion

        #region Properties

        private bool isRunning;

        public bool IsRunning
        {
            get { return this.isRunning; }
            set
            {
                this.isRunning = value;

                if (this.isRunning)
                {
                    if (this.UseBakedAnimation)
                    {
                        this.StartBakedAnimation();
                    }
                }
                else
                {
                    if (this.UseBakedAnimation)
                    {
                        this.StopBakedAnimation();
                    }
                }
            }
        }

        #endregion

        #region Initialisation

        public override void OnStart(StartState state)
        {
            this.random = new Random();

            this.StopBakedAnimation();
            this.IsRunning = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                this.part.OnEditorAttach += this.OnEditorAttach;
                this.part.OnEditorDetach += this.OnEditorDetach;

                this.sceneIsEditor = true;

                if (this.part.parent != null)
                {
                    this.IsRunning = true;
                }
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                this.IsRunning = true;
            }

            if (!this.UseBakedAnimation)
            {
                this.InitialiseReels();
                this.InitialiseLights();
            }
        }

        private void InitialiseReels()
        {
            if (this.Reel1 != "")
            {
                this.reel1Transform = this.part.FindModelTransform(this.Reel1);
            }

            if (this.Reel2 != "")
            {
                this.reel2Transform = this.part.FindModelTransform(this.Reel2);
            }
        }

        private void InitialiseLights()
        {
            if (this.Lights1 != "")
            {
                this.lights1Transform = this.part.FindModelTransform(this.Lights1);
                this.lights1ShaderOff = this.lights1Transform.renderer.material.shader;
            }

            if (this.Lights2 != "")
            {
                this.lights2Transform = this.part.FindModelTransform(this.Lights2);
                this.lights2ShaderOff = this.lights2Transform.renderer.material.shader;
            }

            if (this.Lights3 != "")
            {
                this.lights3Transform = this.part.FindModelTransform(this.Lights3);
                this.lights3ShaderOff = this.lights3Transform.renderer.material.shader;
            }

            if (this.Lights4 != "")
            {
                this.lights4Transform = this.part.FindModelTransform(this.Lights4);
                this.lights4ShaderOff = this.lights4Transform.renderer.material.shader;
            }

            if (this.Lights5 != "")
            {
                this.lights5Transform = this.part.FindModelTransform(this.Lights5);
                this.lights5ShaderOff = this.lights5Transform.renderer.material.shader;
            }

            if (this.Lights6 != "")
            {
                this.lights6Transform = this.part.FindModelTransform(this.Lights6);
                this.lights6ShaderOff = this.lights6Transform.renderer.material.shader;
            }

            this.lightsShaderOn = Shader.Find("Unlit/Texture");
        }

        #endregion

        #region Updating

        public override void OnUpdate()
        {
            if (!this.UseBakedAnimation)
            {
                this.deltaTime = this.sceneIsEditor ? Time.deltaTime : TimeWarp.deltaTime;

                if (TimeWarp.CurrentRate != 1.0f && TimeWarp.WarpMode != TimeWarp.Modes.LOW)
                {
                    return;
                }

                if (this.IsRunning)
                {
                    this.UpdateTimerCycle();
                    this.UpdateSpeed();
                    this.UpdateReels();
                    this.UpdateLights();
                }
                else
                {
                    this.targetSpeed = 0;

                    if (this.speed != 0)
                    {
                        this.UpdateSpeed();
                        this.UpdateReels();
                        this.UpdateLights();
                    }
                }
            }
        }

        private void Update()
        {
            if (this.sceneIsEditor)
            {
                this.OnUpdate();
            }
        }

        private void OnEditorAttach()
        {
            this.IsRunning = true;
        }

        private void OnEditorDetach()
        {
            this.IsRunning = false;
        }

        private void StopBakedAnimation()
        {
            foreach (var animator in this.part.FindModelAnimators())
            {
                animator.Stop();
            }
        }

        private void StartBakedAnimation()
        {
            foreach (var animator in this.part.FindModelAnimators())
            {
                animator.Play();
            }
        }

        private void UpdateTimerCycle()
        {
            this.currentTime += this.deltaTime;

            if (this.currentTime >= this.repeatTime)
            {
                this.targetSpeed = this.random.Next(this.MinReelSpeed, this.MaxReelSpeed);

                if (this.targetSpeed > -this.SpeedStopZone && this.targetSpeed < this.SpeedStopZone)
                {
                    this.targetSpeed = 0;
                }

                this.repeatTime = this.random.Next(this.MinRepeatTime, this.MaxRepeatTime);

                if (this.RepeatTimeDenominator != 0)
                {
                    this.repeatTime /= this.RepeatTimeDenominator;
                }

                this.currentTime -= this.repeatTime;
            }
        }

        private void UpdateSpeed()
        {
            if (this.speed < this.targetSpeed)
            {
                if (this.speed < this.targetSpeed - this.SpeedDeadZone)
                {
                    this.speed += this.SpeedChangeAmount * this.deltaTime;
                }
                else
                {
                    this.speed = this.targetSpeed;
                }
            }
            else if (this.speed > this.targetSpeed)
            {
                if (this.speed > this.targetSpeed + this.SpeedDeadZone)
                {
                    this.speed -= this.SpeedChangeAmount * this.deltaTime;
                }
                else
                {
                    this.speed = this.targetSpeed;
                }
            }
        }

        private void UpdateReels()
        {
            if (this.reel1Transform != null && this.speed != 0)
            {
                this.reel1Transform.transform.Rotate(Vector3.right, this.speed * this.Reel1SpeedRatio);
            }

            if (this.reel2Transform != null && this.speed != 0)
            {
                this.reel2Transform.transform.Rotate(Vector3.right, this.speed * this.Reel2SpeedRatio);
            }
        }

        private void UpdateLights()
        {
            if (this.lights1Transform != null)
            {
                this.UpdateLightTransform(this.lights1Transform, this.lightsShaderOn, this.lights1ShaderOff, this.Lights1Speed);
            }
            if (this.lights2Transform != null)
            {
                this.UpdateLightTransform(this.lights2Transform, this.lightsShaderOn, this.lights2ShaderOff, this.Lights2Speed);
            }
            if (this.lights3Transform != null)
            {
                this.UpdateLightTransform(this.lights3Transform, this.lightsShaderOn, this.lights3ShaderOff, this.Lights3Speed);
            }
            if (this.lights4Transform != null)
            {
                this.UpdateLightTransform(this.lights4Transform, this.lightsShaderOn, this.lights4ShaderOff, this.Lights4Speed);
            }
            if (this.lights5Transform != null)
            {
                this.UpdateLightTransform(this.lights5Transform, this.lightsShaderOn, this.lights5ShaderOff, this.Lights5Speed);
            }
            if (this.lights6Transform != null)
            {
                this.UpdateLightTransform(this.lights6Transform, this.lightsShaderOn, this.lights6ShaderOff, this.Lights6Speed);
            }
        }

        private void UpdateLightTransform(Component lights, Shader on, Shader off, float targetSpeed)
        {
            bool lightsOn;

            if (targetSpeed > 0)
            {
                lightsOn = (this.speed > targetSpeed);
            }
            else if (targetSpeed < 0)
            {
                lightsOn = (this.speed < targetSpeed);
            }
            else
            {
                lightsOn = (this.speed == 0);
            }

            lights.renderer.material.shader = lightsOn ? @on : off;
        }

        #endregion
    }
}