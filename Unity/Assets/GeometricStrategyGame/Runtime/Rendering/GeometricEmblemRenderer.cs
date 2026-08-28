using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class GeometricEmblemRenderer : MonoBehaviour
    {
        [SerializeField] private Color lineColor = Color.white;
        [SerializeField, Range(0.01f, 0.15f)] private float lineWidth = 0.045f;
        [SerializeField] private float scale = 0.42f;

        private LineRenderer line;

        public void ConfigureFaction(FactionId faction, Color? overrideColor = null)
        {
            EnsureLine();
            lineColor = overrideColor ?? DefaultEmblemColor(faction);
            Draw(FactionPoints(faction));
        }

        public void ConfigureProfession(ProfessionType profession, Color? overrideColor = null)
        {
            EnsureLine();
            lineColor = overrideColor ?? Color.white;
            Draw(ProfessionPoints(profession));
        }

        private void EnsureLine()
        {
            if (line != null) return;

            line = GetComponent<LineRenderer>();
            if (line == null) line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = false;
            line.numCapVertices = 3;
            line.numCornerVertices = 3;
            line.sortingOrder = 10;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            line.material = new Material(shader) { name = "GeometricEmblemMaterial" };
        }

        private void Draw(IReadOnlyList<Vector2> points)
        {
            EnsureLine();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.startColor = lineColor;
            line.endColor = lineColor;
            line.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
                line.SetPosition(i, new Vector3(points[i].x * scale, points[i].y * scale, -0.02f));
        }

        private static Color DefaultEmblemColor(FactionId faction)
        {
            switch (faction)
            {
                case FactionId.PlayerOne: return Color.white;
                case FactionId.PlayerTwo: return Color.black;
                case FactionId.WolfClan: return new Color32(225, 225, 225, 255);
                case FactionId.BearClan: return new Color32(45, 25, 15, 255);
                case FactionId.EagleClan: return new Color32(255, 240, 120, 255);
                default: return Color.gray;
            }
        }

        private static Vector2[] FactionPoints(FactionId faction)
        {
            switch (faction)
            {
                case FactionId.PlayerOne:
                    return new[] { V(0, 1), V(0.75f, 0), V(0, -1), V(-0.75f, 0), V(0, 1) };
                case FactionId.PlayerTwo:
                    return new[] { V(-0.8f, 0.8f), V(0.8f, -0.8f), V(0, 0), V(0.8f, 0.8f), V(-0.8f, -0.8f) };
                case FactionId.WolfClan:
                    return new[] { V(-0.9f, 0.8f), V(-0.45f, 0.25f), V(-0.2f, 0.55f), V(0, 0.15f), V(0.2f, 0.55f), V(0.45f, 0.25f), V(0.9f, 0.8f), V(0.55f, -0.2f), V(0, -0.85f), V(-0.55f, -0.2f), V(-0.9f, 0.8f) };
                case FactionId.BearClan:
                    return new[] { V(-0.85f, 0.45f), V(-0.6f, 0.85f), V(-0.25f, 0.55f), V(0.25f, 0.55f), V(0.6f, 0.85f), V(0.85f, 0.45f), V(0.6f, -0.45f), V(0, -0.8f), V(-0.6f, -0.45f), V(-0.85f, 0.45f) };
                case FactionId.EagleClan:
                    return new[] { V(-1, 0.35f), V(-0.35f, 0.65f), V(0, 0.15f), V(0.35f, 0.65f), V(1, 0.35f), V(0.35f, -0.1f), V(0, -0.8f), V(-0.35f, -0.1f), V(-1, 0.35f) };
                default:
                    return new[] { V(-0.6f, 0), V(0.6f, 0) };
            }
        }

        private static Vector2[] ProfessionPoints(ProfessionType profession)
        {
            switch (profession)
            {
                case ProfessionType.Blacksmith:
                    return new[] { V(-0.85f, 0.55f), V(0.1f, 0.55f), V(0.1f, 0.2f), V(0.75f, 0.2f), V(0.75f, -0.05f), V(0.1f, -0.05f), V(-0.45f, -0.8f) };
                case ProfessionType.Farmer:
                    return new[] { V(0, -0.9f), V(0, 0.9f), V(-0.7f, 0.45f), V(0, 0.1f), V(0.7f, 0.45f), V(0, 0.1f) };
                case ProfessionType.Carpenter:
                    return new[] { V(-0.8f, -0.6f), V(0.8f, 0.6f), V(0.45f, 0.9f), V(-0.8f, -0.6f), V(-0.45f, -0.9f) };
                case ProfessionType.Miner:
                    return new[] { V(-0.9f, 0.55f), V(0, 0.85f), V(0.9f, 0.55f), V(0, 0.25f), V(0, -0.9f) };
                case ProfessionType.Weaponsmith:
                    return new[] { V(-0.8f, -0.8f), V(0.8f, 0.8f), V(0.35f, 0.55f), V(0.8f, 0.8f), V(0.55f, 0.35f), V(0, 0), V(-0.8f, 0.8f), V(0.8f, -0.8f) };
                case ProfessionType.AnimalBreeder:
                    return new[] { V(-0.75f, -0.15f), V(-0.55f, 0.55f), V(-0.15f, 0.85f), V(0.15f, 0.85f), V(0.55f, 0.55f), V(0.75f, -0.15f), V(0, -0.85f), V(-0.75f, -0.15f) };
                default:
                    return new[] { V(-0.5f, 0), V(0.5f, 0) };
            }
        }

        private static Vector2 V(float x, float y) => new Vector2(x, y);
    }
}
