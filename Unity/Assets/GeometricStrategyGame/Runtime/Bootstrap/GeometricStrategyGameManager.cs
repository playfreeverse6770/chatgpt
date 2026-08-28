using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    public sealed class GeometricStrategyGameManager : MonoBehaviour
    {
        private readonly List<GeometricUnit> units = new List<GeometricUnit>();
        private bool matchEnded;

        public bool MatchEnded => matchEnded;

        private void Start()
        {
            GeometricUnit[] sceneUnits = FindObjectsOfType<GeometricUnit>();
            for (int i = 0; i < sceneUnits.Length; i++)
                Register(sceneUnits[i]);
        }

        public void Register(GeometricUnit unit)
        {
            if (unit == null || units.Contains(unit)) return;
            units.Add(unit);
            unit.Died += OnUnitDied;
        }

        public void Unregister(GeometricUnit unit)
        {
            if (unit == null) return;
            unit.Died -= OnUnitDied;
            units.Remove(unit);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != null)
                    units[i].Died -= OnUnitDied;
            }
        }

        private void OnUnitDied(GeometricUnit unit)
        {
            if (unit == null) return;
            units.Remove(unit);

            if (!matchEnded && unit.IsKing && GeometricGameRules.IsPlayerFaction(unit.Faction))
            {
                matchEnded = true;
                AudioCue cue = unit.Faction == FactionId.PlayerOne ? AudioCue.Defeat : AudioCue.Victory;
                if (GeometricAudioService.Instance != null)
                    GeometricAudioService.Instance.Play(cue, unit.transform.position);

                Debug.Log("[GeometricStrategy] King defeated: " + unit.Faction + ". Match ended.");
            }
        }
    }
}
