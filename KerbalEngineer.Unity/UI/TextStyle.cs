namespace KerbalEngineer.Unity.UI
{
    using UnityEngine;

    public class TextStyle
    {
        private Color m_Colour;
        private Font m_Font;
        private int m_Size;
        private FontStyle m_Style;

        public Color Colour
        {
            get
            {
                return m_Colour;
            }
            set
            {
                m_Colour = value;
            }
        }

        public Font Font
        {
            get
            {
                return m_Font;
            }
            set
            {
                m_Font = value;
            }
        }

        public int Size
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size = value;
            }
        }

        public FontStyle Style
        {
            get
            {
                return m_Style;
            }
            set
            {
                m_Style = value;
            }
        }
    }
}