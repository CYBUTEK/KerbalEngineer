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

namespace KerbalEngineer.UIControls
{
    using System;
    using UnityEngine;

    public class SelectKeyBindPopup : MonoBehaviour
    {
        private readonly Array availableBindings = Enum.GetValues(typeof(KeyCode));
        private bool isCentred;
        private Rect position = new Rect(0.0f, 0.0f, 250.0f, 0.0f);

        /// <summary>
        ///     Gets whether a key bind popup is already open.
        /// </summary>
        public static bool IsOpen { get; private set; }

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
        ///     Shows a key bind popup allowing the user to select a key for binding.
        /// </summary>
        public static void Show(string name, KeyCode currentBinding, Action<KeyCode> acceptClicked)
        {
            SelectKeyBindPopup selectKeyBindPopup = new GameObject("SelectKeyBind").AddComponent<SelectKeyBindPopup>();
            selectKeyBindPopup.Name = name;
            selectKeyBindPopup.Binding = currentBinding;
            selectKeyBindPopup.AcceptClicked = acceptClicked;
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
            IsOpen = true;
        }

        /// <summary>
        ///     Called by unity when the component is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsOpen = false;
        }

        /// <summary>
        ///     Called by unity each frame to render the GUI.
        /// </summary>
        protected virtual void OnGUI()
        {
            position = GUILayout.Window(GetInstanceID(), position, RenderWindow, "Select Key Bind", HighLogic.Skin.window);

            // Centre the window.
            if (isCentred == false)
            {
                isCentred = true;
                CentreWindow();
            }
        }

        /// <summary>
        ///     Called by unity every frame.
        /// </summary>
        protected virtual void Update()
        {
            CentreWindow();
            UpdateBinding();
        }

        /// <summary>
        ///     Centres the window on the screen.
        /// </summary>
        private void CentreWindow()
        {
            if (position.width > 0.0f && position.height > 0.0f)
            {
                position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            }
        }

        /// <summary>
        ///     Renders the window content.
        /// </summary>
        private void RenderWindow(int id)
        {
            // Binding labels.
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            GUILayout.Label("Key Bind: " + Name);
            GUILayout.Label("Selected: " + Binding);
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
                    Binding = keyCode;
                }
            }
        }
    }
}