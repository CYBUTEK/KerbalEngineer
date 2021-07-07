using UnityEngine;
namespace KerbalEngineer.Flight.Readouts {
    public class ReadoutModuleConfigNode {
        public string Name { get; set; }
        public Color Color { get; set; } = HighLogic.Skin.label.normal.textColor;
    }
}