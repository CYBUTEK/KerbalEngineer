// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.KeyBinding
{
    using System;
    using Extensions;
    using UnityEngine;

    public class KeyBindPopup : MonoBehaviour
    {
        private const string LOCK_ID = "KER_KeyBindPopup";
        private static Rect position = new Rect(Screen.width, Screen.height, 250.0f, 0.0f);
        private static bool hasCentred;
        private static KeyBindPopup instance;
        private readonly Array availableBindings = Enum.GetValues(typeof(KeyCode));

        /// <summary>
        ///     Gets the delegate to be invoked when accepted button is clicked.
        /// </summary>
        public Action<KeyCode> AcceptClicked { get; private set; }

        /// <summary>
        ///     Gets the name of the binding to change.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the selected binding.
        /// </summary>
        public KeyCode Binding { get; private set; }

        /// <summary>
        ///     Gets whether a key bind popup is already open.
        /// </summary>
        public static bool IsOpen
        {
            get
            {
                return (instance != null);
            }
        }

        /// <summary>
        ///     Gets and sets the input lock state.
        /// </summary>
        public bool InputLock
        {
            get
            {
                return InputLockManager.GetControlLock(LOCK_ID) != ControlTypes.None;
            }
            set
            {
                if (value)
                {
                    InputLockManager.SetControlLock(ControlTypes.All, LOCK_ID);
                }
                else
                {
                    InputLockManager.SetControlLock(ControlTypes.None, LOCK_ID);
                }
            }
        }

        /// <summary>
        ///     Shows a key bind popup allowing the user to select a key for binding.
        /// </summary>
        public static void Show(string name, KeyCode currentBinding, Action<KeyCode> acceptClicked)
        {
            if (instance == null)
            {
                instance = new GameObject("SelectKeyBind").AddComponent<KeyBindPopup>();
            }

            instance.Name = name;
            instance.Binding = currentBinding;
            instance.AcceptClicked = acceptClicked;
        }

        /// <summary>
        ///     Handles the accept button click event.
        /// </summary>
        public void OnAccept()
        {
            if (AcceptClicked != null)
            {
                AcceptClicked.Invoke(Binding);
            }
            Destroy(gameObject);
        }

        /// <summary>
        ///     Handles the cancel button click event.
        /// </summary>
        public void OnCancel()
        {
            Destroy(gameObject);
        }

        /// <summary>
        ///     Called by unity when the component is created.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                OnCancel();
            }
        }

        /// <summary>
        ///     Called by unity when the component is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            InputLock = false;
        }

        /// <summary>
        ///     Called by unity each frame to render the GUI.
        /// </summary>
        protected virtual void OnGUI()
        {
            position = GUILayout.Window(GetInstanceID(), position, RenderWindow, "Select Key Bind", HighLogic.Skin.window).ClampToScreen();
            CentreWindow();
        }

        /// <summary>
        ///     Called by unity every frame.
        /// </summary>
        protected virtual void Update()
        {
            CentreWindow();
            UpdateBinding();
            UpdateInputLock();
        }

        /// <summary>
        ///     Centres the window on the screen.
        /// </summary>
        private static void CentreWindow()
        {
            if (hasCentred == false && position.width > 0.0f && position.height > 0.0f)
            {
                hasCentred = true;
                position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            }
        }

        /// <summary>
        ///     Renders the window content.
        /// </summary>
        private void RenderWindow(int id)
        {
            GUILayout.Label("Press the desired key to change it.");

            // Binding labels.
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            GUILayout.Label("Key Bind: " + Name);
            GUILayout.Label("Selected: " + Binding);
            if (GUILayout.Button("Clear", HighLogic.Skin.button))
            {
                Binding = KeyCode.None;
            }
            GUILayout.EndVertical();

            // Window buttons.
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel", HighLogic.Skin.button))
            {
                OnCancel();
            }

            if (GUILayout.Button("Accept", HighLogic.Skin.button))
            {
                OnAccept();
            }
            GUILayout.EndHorizontal();

            // Make the window to be draggable.
            GUI.DragWindow();
        }

        /// <summary>
        ///     Updates the binding selected by the user.
        /// </summary>
        private void UpdateBinding()
        {
            for (int i = 0; i < availableBindings.Length; ++i)
            {
                KeyCode keyCode = (KeyCode)availableBindings.GetValue(i);

                if (keyCode == KeyCode.Mouse0)
                {
                    continue;
                }

                if (Input.GetKeyDown(keyCode))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        Binding = keyCode;
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the input lock.
        /// </summary>
        private void UpdateInputLock()
        {
            bool mouseOver = position.MouseIsOver();
            bool inputLock = InputLock;

            if (mouseOver && inputLock == false)
            {
                InputLock = true;
            }
            else if (mouseOver == false && inputLock)
            {
                InputLock = false;
            }
        }
    }
}