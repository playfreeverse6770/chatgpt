using UnityEngine;

namespace GeometricStrategy
{
    [RequireComponent(typeof(GeometricUnit))]
    public sealed class GeometricAutoCombat : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float scanInterval = 0.15f;

        private GeometricUnit self;
        private float nextScanTime;

        private void Awake()
        {
            self = GetComponent<GeometricUnit>();
        }

        private void Update()
        {
            if (self == null || !self.IsAlive || Time.time < nextScanTime) return;
            nextScanTime = Time.time + scanInterval;

            GeometricUnit[] units = FindObjectsOfType<GeometricUnit>();
            GeometricUnit best = null;
            float bestDistance = self.Stats.attackRange + 0.001f;

            for (int i = 0; i < units.Length; i++)
            {
                GeometricUnit candidate = units[i];
                if (candidate == null || candidate == self || !candidate.IsAlive) continue;
                if (!GeometricGameRules.AreEnemies(self.Faction, candidate.Faction)) continue;

                float distance = Vector2.Distance(transform.position, candidate.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = candidate;
                }
            }

            if (best != null)
                self.TryAttack(best);
        }
    }
}
