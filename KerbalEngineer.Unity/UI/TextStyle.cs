namespace KerbalEngineer.Unity.UI
{
    using UnityEngine;

    public class TextStyle
    {
        private Color colour;
        private Font font;
        private int size;
        private FontStyle style;

        public Color Colour
        {
            get
            {
                return colour;
            }
            set
            {
                colour = value;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public FontStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }
    }
}