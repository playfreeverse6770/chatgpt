using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class GeometricShapeRenderer : MonoBehaviour
    {
        [SerializeField] private GeometricSymbol symbol = GeometricSymbol.Circle;
        [SerializeField] private Color fillColor = Color.white;
        [SerializeField, Min(0.05f)] private float radius = 0.6f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Material runtimeMaterial;

        public GeometricSymbol Symbol => symbol;
        public Color FillColor => fillColor;

        private void Awake()
        {
            EnsureComponents();
            Rebuild();
        }

        private void OnDestroy()
        {
            if (runtimeMaterial != null)
                Destroy(runtimeMaterial);
            if (meshFilter != null && meshFilter.sharedMesh != null)
                Destroy(meshFilter.sharedMesh);
        }

        public void Configure(GeometricSymbol newSymbol, Color newColor, float newRadius = 0.6f)
        {
            symbol = newSymbol;
            fillColor = newColor;
            radius = Mathf.Max(0.05f, newRadius);
            EnsureComponents();
            Rebuild();
        }

        private void EnsureComponents()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Rebuild()
        {
            if (meshFilter.sharedMesh != null)
                Destroy(meshFilter.sharedMesh);
            meshFilter.sharedMesh = GeometricMeshFactory.Create(symbol, radius);

            if (runtimeMaterial == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Color");

                if (shader == null)
                {
                    Debug.LogError("[GeometricStrategy] No compatible unlit shader was found. Use a standard Unity 2D/Built-In or URP project.");
                    return;
                }

                runtimeMaterial = new Material(shader) { name = "GeometricRuntimeMaterial" };
                meshRenderer.sharedMaterial = runtimeMaterial;
            }

            if (runtimeMaterial.HasProperty("_BaseColor"))
                runtimeMaterial.SetColor("_BaseColor", fillColor);
            if (runtimeMaterial.HasProperty("_Color"))
                runtimeMaterial.SetColor("_Color", fillColor);
        }
    }
}
