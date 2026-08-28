using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class ResourceNode : MonoBehaviour
    {
        [SerializeField] private ResourceType resourceType = ResourceType.Wood;
        [SerializeField, Min(1)] private int amount = 100;
        [SerializeField, Min(0.1f)] private float visualRadius = 0.7f;

        private GeometricShapeRenderer shapeRenderer;

        public ResourceType ResourceType => resourceType;
        public int Amount => amount;
        public bool IsDepleted => amount <= 0;

        private void Awake()
        {
            EnsurePresentation();
            RefreshPresentation();
        }

        public void Configure(ResourceType type, int startingAmount)
        {
            resourceType = type;
            amount = Mathf.Max(1, startingAmount);
            EnsurePresentation();
            RefreshPresentation();
        }

        public int Harvest(int requestedAmount, ResourceWallet destination)
        {
            if (destination == null || requestedAmount <= 0 || IsDepleted) return 0;

            int harvested = Mathf.Min(requestedAmount, amount);
            amount -= harvested;
            destination.Add(resourceType, harvested);

            if (GeometricAudioService.Instance != null)
                GeometricAudioService.Instance.Play(AudioCue.Harvest, transform.position);

            if (IsDepleted)
                gameObject.SetActive(false);

            return harvested;
        }

        private void EnsurePresentation()
        {
            shapeRenderer = GetComponent<GeometricShapeRenderer>();
            if (shapeRenderer == null) shapeRenderer = gameObject.AddComponent<GeometricShapeRenderer>();

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null) collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = visualRadius;
        }

        private void RefreshPresentation()
        {
            shapeRenderer.Configure(
                GeometricGameRules.ResourceSymbol(resourceType),
                GeometricGameRules.ResourceColor(resourceType),
                visualRadius);
            name = "Resource_" + resourceType;
        }
    }
}
