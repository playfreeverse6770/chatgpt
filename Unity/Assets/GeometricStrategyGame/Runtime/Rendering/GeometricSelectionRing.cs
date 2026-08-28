using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class GeometricSelectionRing : MonoBehaviour
    {
        [SerializeField] private float radius = 0.82f;
        [SerializeField] private float width = 0.055f;
        [SerializeField] private int segments = 48;

        private LineRenderer line;

        private void Awake()
        {
            EnsureLine();
            SetVisible(false);
        }

        public void SetRadius(float value)
        {
            radius = Mathf.Max(0.1f, value);
            Rebuild();
        }

        public void SetVisible(bool visible)
        {
            EnsureLine();
            line.enabled = visible;
        }

        private void EnsureLine()
        {
            if (line != null) return;
            line = GetComponent<LineRenderer>();
            if (line == null) line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = Mathf.Max(12, segments);
            line.startWidth = width;
            line.endWidth = width;
            line.sortingOrder = 20;
            line.startColor = new Color32(255, 255, 255, 240);
            line.endColor = new Color32(255, 255, 255, 240);

            Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            line.material = new Material(shader) { name = "GeometricSelectionRingMaterial" };
            Rebuild();
        }

        private void Rebuild()
        {
            if (line == null) return;
            int count = Mathf.Max(12, segments);
            line.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.PI * 2f * i / count;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.08f));
            }
        }
    }
}
