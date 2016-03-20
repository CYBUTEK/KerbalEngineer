namespace KerbalEngineer.Settings
{
    using System;
    using Editor;
    using Flight;
    using KeyBinding;
    using Unity;
    using Unity.UI;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class SettingsWindow : MonoBehaviour
    {
        private static Window m_Window;

        public static void Close()
        {
            if (m_Window != null)
            {
                m_Window.Close();
            }
        }

        public static void Open()
        {
            if (m_Window == null)
            {
                m_Window = StyleManager.CreateWindow("SETTINGS", 600.0f);

                AddKeyBindingsButton();
                AddFlightActivationModes();
                AddBuildOverlayOptions();

                StyleManager.Process(m_Window);
            }
        }

        private static void AddBuildOverlayOptions()
        {
            if (m_Window != null)
            {
                Setting buildOverlay = StyleManager.CreateSetting("Build Engineer Overlay", m_Window);
                Toggle buildOverlayVisible = AddToggle(buildOverlay, "VISIBLE", 100.0f, value => BuildOverlay.Visible = value);
                Toggle buildOverlayNamesOnly = AddToggle(buildOverlay, "NAMES ONLY", 100.0f, value => BuildOverlayPartInfo.NamesOnly = value);
                Toggle buildOverlayClickToOpen = AddToggle(buildOverlay, "CLICK TO OPEN", 100.0f, value => BuildOverlayPartInfo.ClickToOpen = value);
                AddUpdateHandler(buildOverlay, () =>
                {
                    buildOverlayVisible.isOn = BuildOverlay.Visible;
                    buildOverlayNamesOnly.isOn = BuildOverlayPartInfo.NamesOnly;
                    buildOverlayClickToOpen.isOn = BuildOverlayPartInfo.ClickToOpen;
                });
            }
        }

        private static Button AddButton(Setting setting, string text, float width, UnityAction onClick)
        {
            Button button = null;

            if (setting != null)
            {
                button = setting.AddButton(text, width, onClick);
            }

            return button;
        }

        private static void AddFlightActivationModes()
        {
            if (m_Window != null)
            {
                Setting flightActivationMode = StyleManager.CreateSetting("Flight Engineer Activation Mode", m_Window);
                Toggle flightActivationModeCareer = AddToggle(flightActivationMode, "CAREER", 100.0f, value => FlightEngineerCore.IsCareerMode = value);
                Toggle flightActivationModePartless = AddToggle(flightActivationMode, "PARTLESS", 100.0f, value => FlightEngineerCore.IsCareerMode = !value);
                AddUpdateHandler(flightActivationMode, () =>
                {
                    flightActivationModeCareer.isOn = FlightEngineerCore.IsCareerMode;
                    flightActivationModePartless.isOn = !FlightEngineerCore.IsCareerMode;
                });
            }
        }

        private static void AddKeyBindingsButton()
        {
            if (m_Window != null)
            {
                Setting keyBindings = StyleManager.CreateSetting("Key Bindings", m_Window);
                AddButton(keyBindings, "EDIT KEY BINDINGS", 304.0f, KeyBinder.Show);
            }
        }

        private static Toggle AddToggle(Setting setting, string text, float width, UnityAction<bool> onValueChanged)
        {
            Toggle toggle = null;

            if (setting != null)
            {
                toggle = setting.AddToggle(text, width, onValueChanged);
            }

            return toggle;
        }

        private static void AddUpdateHandler(Setting setting, Action onUpdate)
        {
            if (setting != null && onUpdate != null)
            {
                setting.AddUpdateHandler(onUpdate);
            }
        }
    }
}