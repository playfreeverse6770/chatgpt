using System;
using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class GeometricUnit : MonoBehaviour
    {
        [SerializeField] private UnitArchetype archetype = UnitArchetype.Soldier;
        [SerializeField] private UnitLevel level = UnitLevel.Level1;
        [SerializeField] private FactionId faction = FactionId.PlayerOne;
        [SerializeField] private float selectionScale = 1.08f;

        private GeometricShapeRenderer shapeRenderer;
        private GeometricEmblemRenderer emblemRenderer;
        private GeometricSelectionRing selectionRing;
        private CircleCollider2D clickCollider;
        private UnitStats stats;
        private float currentHealth;
        private float nextAttackTime;
        private bool hasMoveTarget;
        private Vector3 moveTarget;
        private bool selected;

        public event Action<GeometricUnit> Died;
        public event Action<GeometricUnit> Changed;

        public UnitArchetype Archetype => archetype;
        public UnitLevel Level => level;
        public FactionId Faction => faction;
        public UnitStats Stats => stats;
        public float CurrentHealth => currentHealth;
        public bool IsAlive => currentHealth > 0f;
        public bool IsKing => archetype == UnitArchetype.King;
        public bool IsSelected => selected;
        public bool HasMoveTarget => hasMoveTarget;
        public Vector3 MoveTarget => moveTarget;

        private void Awake()
        {
            EnsurePresentation();
            ApplyConfiguration(true);
        }

        private void Update()
        {
            if (!IsAlive || !hasMoveTarget) return;

            Vector3 delta = moveTarget - transform.position;
            delta.z = 0f;
            if (delta.sqrMagnitude <= 0.02f)
            {
                hasMoveTarget = false;
                Changed?.Invoke(this);
                return;
            }

            transform.position += delta.normalized * stats.moveSpeed * Time.deltaTime;
        }

        public void Configure(UnitArchetype newArchetype, UnitLevel newLevel, FactionId newFaction)
        {
            archetype = newArchetype;
            level = newLevel;
            faction = newFaction;
            EnsurePresentation();
            ApplyConfiguration(true);
        }

        public void SetMoveTarget(Vector3 worldPosition)
        {
            moveTarget = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            hasMoveTarget = true;
            Changed?.Invoke(this);
        }

        public void StopMoving()
        {
            hasMoveTarget = false;
            Changed?.Invoke(this);
        }

        public bool TryAttack(GeometricUnit target)
        {
            if (!IsAlive || target == null || !target.IsAlive) return false;
            if (!GeometricGameRules.AreEnemies(faction, target.faction)) return false;

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > stats.attackRange || Time.time < nextAttackTime) return false;

            nextAttackTime = Time.time + 1f / Mathf.Max(0.05f, stats.attacksPerSecond);
            target.TakeDamage(stats.damage, this);

            if (GeometricAudioService.Instance != null)
                GeometricAudioService.Instance.Play(archetype == UnitArchetype.Archer ? AudioCue.ArrowShot : AudioCue.Hit, transform.position);

            return true;
        }

        public void TakeDamage(float amount, GeometricUnit attacker = null)
        {
            if (!IsAlive || amount <= 0f) return;
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            Changed?.Invoke(this);

            if (currentHealth <= 0f)
            {
                Died?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public bool LevelUp()
        {
            if (level >= UnitLevel.Level7) return false;

            float healthRatio = stats.maxHealth <= 0f ? 1f : currentHealth / stats.maxHealth;
            level = (UnitLevel)((int)level + 1);
            ApplyConfiguration(false);
            currentHealth = Mathf.Max(1f, stats.maxHealth * healthRatio);

            if (GeometricAudioService.Instance != null)
                GeometricAudioService.Instance.Play(AudioCue.Upgrade, transform.position);

            Changed?.Invoke(this);
            return true;
        }

        public void SetSelected(bool value)
        {
            if (selected == value) return;
            selected = value;
            transform.localScale = Vector3.one * (selected ? selectionScale : 1f);
            if (selectionRing != null) selectionRing.SetVisible(selected);
            Changed?.Invoke(this);
        }

        private void EnsurePresentation()
        {
            shapeRenderer = GetComponent<GeometricShapeRenderer>();
            if (shapeRenderer == null) shapeRenderer = gameObject.AddComponent<GeometricShapeRenderer>();

            Transform emblemTransform = transform.Find("FactionEmblem");
            if (emblemTransform == null)
            {
                var emblemObject = new GameObject("FactionEmblem");
                emblemTransform = emblemObject.transform;
                emblemTransform.SetParent(transform, false);
                emblemTransform.localPosition = new Vector3(0f, 0f, -0.05f);
            }

            emblemRenderer = emblemTransform.GetComponent<GeometricEmblemRenderer>();
            if (emblemRenderer == null) emblemRenderer = emblemTransform.gameObject.AddComponent<GeometricEmblemRenderer>();

            Transform ringTransform = transform.Find("SelectionRing");
            if (ringTransform == null)
            {
                var ringObject = new GameObject("SelectionRing");
                ringTransform = ringObject.transform;
                ringTransform.SetParent(transform, false);
            }
            selectionRing = ringTransform.GetComponent<GeometricSelectionRing>();
            if (selectionRing == null) selectionRing = ringTransform.gameObject.AddComponent<GeometricSelectionRing>();
            selectionRing.SetVisible(selected);

            clickCollider = GetComponent<CircleCollider2D>();
            if (clickCollider == null) clickCollider = gameObject.AddComponent<CircleCollider2D>();
            clickCollider.radius = 0.65f;
        }

        private void ApplyConfiguration(bool refillHealth)
        {
            stats = GeometricGameRules.Stats(archetype, level);
            if (refillHealth || currentHealth <= 0f) currentHealth = stats.maxHealth;

            float radius = archetype == UnitArchetype.King ? 0.85f : 0.62f;
            shapeRenderer.Configure(GeometricGameRules.UnitSymbol(archetype), GeometricGameRules.LevelColor(level), radius);
            emblemRenderer.ConfigureFaction(faction);
            if (selectionRing != null) selectionRing.SetRadius(radius + 0.2f);
            name = faction + "_" + archetype + "_L" + (int)level;
            Changed?.Invoke(this);
        }
    }
}
