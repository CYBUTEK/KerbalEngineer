// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported
//
// This code is used to run the tape reel and light animations for the ER7500 part
// that was created for me by Keptin.
//
// It is by all means possible to reuse this module with your own part that requires
// the same kind of animation.  So feel free to use as you see fit :)

using UnityEngine;

namespace KerbalEngineer
{
    public class TapeDriveAnimator : PartModule
    {
        #region Fields

        private System.Random _random;
        private float _speed = 0f;
        private float _targetSpeed = 0f;
        private float _repeatTime = 0f;
        private float _currentTime = 0f;
        private float _deltaTime = 0f;
        private bool _sceneIsEditor = false;
        private bool _enabled = false;

        private Shader _lightsShaderOn;
        private Shader _lights1ShaderOff;
        private Shader _lights2ShaderOff;
        private Shader _lights3ShaderOff;
        private Shader _lights4ShaderOff;
        private Shader _lights5ShaderOff;
        private Shader _lights6ShaderOff;

        private Transform _reel1Transform;
        private Transform _reel2Transform;
        private Transform _lights1Transform;
        private Transform _lights2Transform;
        private Transform _lights3Transform;
        private Transform _lights4Transform;
        private Transform _lights5Transform;
        private Transform _lights6Transform;

        #endregion

        #region KSP Fields

        [KSPField]
        public bool useBakedAnimation = false;

        [KSPField]
        public int minReelSpeed = 0;

        [KSPField]
        public int maxReelSpeed = 0;

        [KSPField]
        public float speedStopZone = 0;

        [KSPField]
        public float speedDeadZone = 0;

        [KSPField]
        public float speedChangeAmount = 0;

        [KSPField]
        public int minRepeatTime = 0;

        [KSPField]
        public int maxRepeatTime = 0;

        [KSPField]
        public float repeatTimeDenominator = 1;

        [KSPField]
        public string reel1 = "";

        [KSPField]
        public string reel2 = "";

        [KSPField]
        public float reel1SpeedRatio = 1;

        [KSPField]
        public float reel2SpeedRatio = 1;

        [KSPField]
        public string lights1 = "";

        [KSPField]
        public float lights1Speed = 0;

        [KSPField]
        public string lights2 = "";

        [KSPField]
        public float lights2Speed = 0;

        [KSPField]
        public string lights3 = "";

        [KSPField]
        public float lights3Speed = 0;

        [KSPField]
        public string lights4 = "";

        [KSPField]
        public float lights4Speed = 0;

        [KSPField]
        public string lights5 = "";

        [KSPField]
        public float lights5Speed = 0;

        [KSPField]
        public string lights6 = "";

        [KSPField]
        public float lights6Speed = 0;

        #endregion

        #region Properties

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                if (_enabled)
                {
                    if (useBakedAnimation)
                        StartBakedAnimation();
                }
                else
                {
                    if (useBakedAnimation)
                        StopBakedAnimation();
                }
            }
        }

        #endregion

        #region Initialisation

        public override void OnStart(StartState state)
        {
            _random = new System.Random();

            StopBakedAnimation();
            Enabled = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                part.OnEditorAttach += OnEditorAttach;
                part.OnEditorDetach += OnEditorDetach;

                _sceneIsEditor = true;

                if (part.parent != null)
                    Enabled = true;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                Enabled = true;
            }

            if (!useBakedAnimation)
            {
                InitialiseReels();
                InitialiseLights();
            }
        }

        private void InitialiseReels()
        {
            if (reel1 != "")
                _reel1Transform = part.FindModelTransform(reel1);

            if (reel2 != "")
                _reel2Transform = part.FindModelTransform(reel2);
        }

        private void InitialiseLights()
        {
            if (lights1 != "")
            {
                _lights1Transform = part.FindModelTransform(lights1);
                _lights1ShaderOff = _lights1Transform.renderer.material.shader;
            }

            if (lights2 != "")
            {
                _lights2Transform = part.FindModelTransform(lights2);
                _lights2ShaderOff = _lights2Transform.renderer.material.shader;
            }

            if (lights3 != "")
            {
                _lights3Transform = part.FindModelTransform(lights3);
                _lights3ShaderOff = _lights3Transform.renderer.material.shader;
            }

            if (lights4 != "")
            {
                _lights4Transform = part.FindModelTransform(lights4);
                _lights4ShaderOff = _lights4Transform.renderer.material.shader;
            }

            if (lights5 != "")
            {
                _lights5Transform = part.FindModelTransform(lights5);
                _lights5ShaderOff = _lights5Transform.renderer.material.shader;
            }

            if (lights6 != "")
            {
                _lights6Transform = part.FindModelTransform(lights6);
                _lights6ShaderOff = _lights6Transform.renderer.material.shader;
            }

            _lightsShaderOn = Shader.Find("Self-Illumin/Specular");
        }

        #endregion

        #region Updating

        public override void OnUpdate()
        {
            if (!useBakedAnimation)
            {
                if (_sceneIsEditor)
                    _deltaTime = Time.deltaTime;
                else
                    _deltaTime = TimeWarp.deltaTime;

                if (TimeWarp.CurrentRate == 1f || TimeWarp.WarpMode == TimeWarp.Modes.LOW)
                {
                    if (Enabled)
                    {
                        UpdateTimerCycle();
                        UpdateSpeed();
                        UpdateReels();
                        UpdateLights();
                    }
                    else
                    {
                        _targetSpeed = 0f;
                        if (_speed != 0)
                        {
                            UpdateSpeed();
                            UpdateReels();
                            UpdateLights();
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (_sceneIsEditor)
                OnUpdate();
        }

        private void OnEditorAttach()
        {
            Enabled = true;
        }

        private void OnEditorDetach()
        {
            Enabled = false;
        }

        private void StopBakedAnimation()
        {
            foreach (Animation animation in part.FindModelAnimators())
                animation.Stop();
        }

        private void StartBakedAnimation()
        {
            foreach (Animation animation in part.FindModelAnimators())
                animation.Play();
        }

        private void UpdateTimerCycle()
        {
            _currentTime += _deltaTime;

            if (_currentTime >= _repeatTime)
            {
                _targetSpeed = _random.Next(minReelSpeed, maxReelSpeed);

                if (_targetSpeed > -speedStopZone && _targetSpeed < speedStopZone)
                    _targetSpeed = 0f;

                _repeatTime = _random.Next(minRepeatTime, maxRepeatTime);

                if (repeatTimeDenominator != 0)
                    _repeatTime /= repeatTimeDenominator;

                _currentTime -= _repeatTime;
            }
        }

        private void UpdateSpeed()
        {
            if (_speed < _targetSpeed)
            {
                if (_speed < _targetSpeed - speedDeadZone)
                    _speed += speedChangeAmount * _deltaTime;
                else
                    _speed = _targetSpeed;
            }
            else if (_speed > _targetSpeed)
            {
                if (_speed > _targetSpeed + speedDeadZone)
                    _speed -= speedChangeAmount * _deltaTime;
                else
                    _speed = _targetSpeed;
            }
        }

        private void UpdateReels()
        {
            if (_reel1Transform != null && _speed != 0)
                _reel1Transform.transform.Rotate(Vector3.right, _speed * reel1SpeedRatio);

            if (_reel2Transform != null && _speed != 0)
                _reel2Transform.transform.Rotate(Vector3.right, _speed * reel2SpeedRatio);
        }

        private void UpdateLights()
        {
            if (_lights1Transform != null)
                UpdateLightTransform(_lights1Transform, _lightsShaderOn, _lights1ShaderOff, lights1Speed);

            if (_lights2Transform != null)
                UpdateLightTransform(_lights2Transform, _lightsShaderOn, _lights2ShaderOff, lights2Speed);

            if (_lights3Transform != null)
                UpdateLightTransform(_lights3Transform, _lightsShaderOn, _lights3ShaderOff, lights3Speed);

            if (_lights4Transform != null)
                UpdateLightTransform(_lights4Transform, _lightsShaderOn, _lights4ShaderOff, lights4Speed);

            if (_lights5Transform != null)
                UpdateLightTransform(_lights5Transform, _lightsShaderOn, _lights5ShaderOff, lights5Speed);

            if (_lights6Transform != null)
                UpdateLightTransform(_lights6Transform, _lightsShaderOn, _lights6ShaderOff, lights6Speed);
        }

        private void UpdateLightTransform(Transform lights, Shader on, Shader off, float targetSpeed)
        {
            bool lightsOn = false;

            if (targetSpeed > 0)
                lightsOn = (_speed > targetSpeed);
            else if (targetSpeed < 0)
                lightsOn = (_speed < targetSpeed);
            else
                lightsOn = (_speed == 0);

            if (lightsOn)
                lights.renderer.material.shader = on;
            else
                lights.renderer.material.shader = off;
        }

        #endregion
    }
}