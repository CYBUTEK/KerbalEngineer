namespace KerbalEngineer.Unity
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class Setting : MonoBehaviour
    {
        [SerializeField]
        private Text label = null;

        [SerializeField]
        private Transform buttonsTransform = null;

        [SerializeField]
        private GameObject settingButtonPrefab = null;

        [SerializeField]
        private GameObject settingTogglePrefab = null;

        private Action onUpdate;

        public Button AddButton(string text, float width, UnityAction onClick)
        {
            Button button = null;

            if (settingButtonPrefab != null)
            {
                GameObject buttonObject = Instantiate(settingButtonPrefab);
                if (buttonObject != null)
                {
                    button = buttonObject.GetComponent<Button>();

                    SetParentTransform(buttonObject, buttonsTransform);
                    SetWidth(buttonObject, width);
                    SetText(buttonObject, text);
                    SetButton(buttonObject, onClick);
                }
            }

            return button;
        }

        public Toggle AddToggle(string text, float width, UnityAction<bool> onValueChanged)
        {
            Toggle toggle = null;

            if (settingTogglePrefab != null)
            {
                GameObject toggleObject = Instantiate(settingTogglePrefab);
                if (toggleObject != null)
                {
                    toggle = toggleObject.GetComponent<Toggle>();

                    SetParentTransform(toggleObject, buttonsTransform);
                    SetWidth(toggleObject, width);
                    SetText(toggleObject, text);
                    SetToggle(toggleObject, onValueChanged);
                }
            }

            return toggle;
        }

        public void AddUpdateHandler(Action onUpdate)
        {
            this.onUpdate = onUpdate;
        }

        public void SetLabel(string text)
        {
            if (label != null)
            {
                label.text = text;
            }
        }

        protected virtual void Update()
        {
            if (onUpdate != null)
            {
                onUpdate.Invoke();
            }
        }

        private static void SetButton(GameObject buttonObject, UnityAction onClick)
        {
            if (buttonObject != null)
            {
                Button button = buttonObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(onClick);
                }
            }
        }

        private static void SetParentTransform(GameObject childObject, Transform parentTransform)
        {
            if (childObject != null && parentTransform != null)
            {
                childObject.transform.SetParent(parentTransform, false);
            }
        }

        private static void SetText(GameObject parentObject, string text)
        {
            if (parentObject != null)
            {
                Text textComponent = parentObject.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = text;
                }
            }
        }

        private static void SetToggle(GameObject toggleObject, UnityAction<bool> onValueChanged)
        {
            if (toggleObject != null)
            {
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener(onValueChanged);
                }
            }
        }

        private static void SetWidth(GameObject parentObject, float width)
        {
            if (parentObject != null)
            {
                LayoutElement layout = parentObject.GetComponent<LayoutElement>();
                if (layout != null)
                {
                    if (width > 0.0f)
                    {
                        layout.preferredWidth = width;
                    }
                    else
                    {
                        layout.flexibleWidth = 1.0f;
                    }
                }
            }
        }
    }
}