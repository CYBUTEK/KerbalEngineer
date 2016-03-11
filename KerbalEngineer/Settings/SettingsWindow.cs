namespace KerbalEngineer.Settings
{
    using KeyBinding;
    using Unity;
    using Unity.UI;

    public static class SettingsWindow
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
                m_Window = StyleManager.CreateWindow("SETTINGS", 400.0f);
                Setting keyBindings = StyleManager.CreateSetting("Key Bindings", m_Window);
                if (keyBindings != null)
                {
                    keyBindings.AddButton("EDIT KEY BINDINGS", KeyBinder.Show);
                }
                StyleManager.Process(m_Window);
            }
        }
    }
}