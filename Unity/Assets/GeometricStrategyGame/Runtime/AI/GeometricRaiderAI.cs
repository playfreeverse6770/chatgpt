using UnityEngine;

namespace GeometricStrategy
{
    [RequireComponent(typeof(GeometricUnit))]
    public sealed class GeometricRaiderAI : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float thinkInterval = 0.2f;
        [SerializeField, Min(1f)] private float searchRadius = 50f;
        [SerializeField] private bool preferKings = true;

        private GeometricUnit self;
        private GeometricUnit target;
        private float nextThinkTime;

        private void Awake()
        {
            self = GetComponent<GeometricUnit>();
        }

        private void Start()
        {
            PlayFactionCue();
        }

        private void Update()
        {
            if (self == null || !self.IsAlive) return;

            if (Time.time >= nextThinkTime)
            {
                nextThinkTime = Time.time + thinkInterval;
                if (target == null || !target.IsAlive || !GeometricGameRules.AreEnemies(self.Faction, target.Faction))
                    target = FindTarget();
            }

            if (target == null) return;

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= self.Stats.attackRange)
            {
                self.StopMoving();
                self.TryAttack(target);
            }
            else
            {
                self.SetMoveTarget(target.transform.position);
            }
        }

        private GeometricUnit FindTarget()
        {
            GeometricUnit[] units = FindObjectsOfType<GeometricUnit>();
            GeometricUnit best = null;
            float bestScore = float.MaxValue;

            for (int i = 0; i < units.Length; i++)
            {
                GeometricUnit candidate = units[i];
                if (candidate == null || candidate == self || !candidate.IsAlive) continue;
                if (!GeometricGameRules.AreEnemies(self.Faction, candidate.Faction)) continue;

                float distance = Vector2.Distance(transform.position, candidate.transform.position);
                if (distance > searchRadius) continue;

                float score = distance;
                if (preferKings && candidate.IsKing) score *= 0.45f;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            return best;
        }

        private void PlayFactionCue()
        {
            if (GeometricAudioService.Instance == null || self == null) return;

            switch (self.Faction)
            {
                case FactionId.WolfClan:
                    GeometricAudioService.Instance.Play(AudioCue.Wolf, transform.position);
                    break;
                case FactionId.BearClan:
                    GeometricAudioService.Instance.Play(AudioCue.Bear, transform.position);
                    break;
                case FactionId.EagleClan:
                    GeometricAudioService.Instance.Play(AudioCue.Eagle, transform.position);
                    break;
            }
        }
    }
}
