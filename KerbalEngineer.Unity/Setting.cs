namespace KerbalEngineer.Unity
{
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

        public GameObject AddButton(string text, UnityAction onClick)
        {
            GameObject buttonObject = null;

            if (m_SettingButtonPrefab != null)
            {
                buttonObject = Instantiate(m_SettingButtonPrefab);

                SetParentTransform(buttonObject, m_ButtonsTransform);
                SetText(buttonObject, text);
                SetButton(buttonObject, onClick);
            }

            return buttonObject;
        }

        public GameObject AddToggle(string text, UnityAction<bool> onValueChanged)
        {
            GameObject toggleObject = null;

            if (m_SettingTogglePrefab != null)
            {
                toggleObject = Instantiate(m_SettingTogglePrefab);

                SetParentTransform(toggleObject, m_ButtonsTransform);
                SetText(toggleObject, text);
                SetToggle(toggleObject, onValueChanged);
            }

            return toggleObject;
        }

        public void SetLabel(string text)
        {
            if (m_Label != null)
            {
                m_Label.text = text;
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
    }
}