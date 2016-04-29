namespace KerbalEngineer.Unity
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class Setting : MonoBehaviour
    {
        [SerializeField]
        private Text m_Label = null;

        [SerializeField]
        private Transform m_ButtonsTransform = null;

        [SerializeField]
        private GameObject m_SettingButtonPrefab = null;

        [SerializeField]
        private GameObject m_SettingTogglePrefab = null;

        private Action m_OnUpdate;

        public Button AddButton(string text, float width, UnityAction onClick)
        {
            Button button = null;

            if (m_SettingButtonPrefab != null)
            {
                GameObject buttonObject = Instantiate(m_SettingButtonPrefab);
                if (buttonObject != null)
                {
                    button = buttonObject.GetComponent<Button>();

                    SetParentTransform(buttonObject, m_ButtonsTransform);
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

            if (m_SettingTogglePrefab != null)
            {
                GameObject toggleObject = Instantiate(m_SettingTogglePrefab);
                if (toggleObject != null)
                {
                    toggle = toggleObject.GetComponent<Toggle>();

                    SetParentTransform(toggleObject, m_ButtonsTransform);
                    SetWidth(toggleObject, width);
                    SetText(toggleObject, text);
                    SetToggle(toggleObject, onValueChanged);
                }
            }

            return toggle;
        }

        public void AddUpdateHandler(Action onUpdate)
        {
            m_OnUpdate = onUpdate;
        }

        public void SetLabel(string text)
        {
            if (m_Label != null)
            {
                m_Label.text = text;
            }
        }

        protected virtual void Update()
        {
            if (m_OnUpdate != null)
            {
                m_OnUpdate.Invoke();
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