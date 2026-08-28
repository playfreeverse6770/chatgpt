using UnityEngine;

namespace GeometricStrategy
{
    /// <summary>
    /// Central visual palette for the minimalist top-down presentation.
    /// Gameplay meaning comes from shape + level color + inner faction emblem,
    /// so the board itself remains intentionally quiet and high-contrast.
    /// </summary>
    public static class GeometricPalette
    {
        public static readonly Color BoardBackground = new Color32(18, 23, 34, 255);
        public static readonly Color NeutralLine = new Color32(225, 231, 239, 255);
        public static readonly Color Selection = new Color32(255, 244, 179, 255);

        // Level spectrum: warm yellow -> royal blue.
        public static readonly Color Level1Yellow = new Color32(255, 209, 102, 255);
        public static readonly Color Level2Orange = new Color32(255, 159, 67, 255);
        public static readonly Color Level3Red = new Color32(255, 93, 115, 255);
        public static readonly Color Level4Purple = new Color32(155, 93, 229, 255);
        public static readonly Color Level5Green = new Color32(46, 204, 113, 255);
        public static readonly Color Level6LightBlue = new Color32(76, 201, 240, 255);
        public static readonly Color Level7DeepBlue = new Color32(67, 97, 238, 255);

        // Resource palette. Shapes remain the primary resource identifier.
        public static readonly Color WoodGreen = new Color32(46, 171, 95, 255);
        public static readonly Color StoneGray = new Color32(138, 148, 166, 255);
        public static readonly Color MetalBrown = new Color32(145, 92, 62, 255);
        public static readonly Color Gold = new Color32(245, 194, 66, 255);
        public static readonly Color Coin = new Color32(255, 216, 90, 255);
        public static readonly Color Food = new Color32(168, 211, 88, 255);
        public static readonly Color Horse = new Color32(176, 117, 72, 255);

        // Professions are deliberately muted so combat-unit level colors dominate the board.
        public static readonly Color Blacksmith = new Color32(82, 91, 108, 255);
        public static readonly Color Farmer = new Color32(112, 184, 92, 255);
        public static readonly Color Carpenter = new Color32(176, 117, 72, 255);
        public static readonly Color Miner = new Color32(124, 134, 151, 255);
        public static readonly Color Weaponsmith = new Color32(191, 78, 91, 255);
        public static readonly Color AnimalBreeder = new Color32(55, 174, 168, 255);
    }
}
