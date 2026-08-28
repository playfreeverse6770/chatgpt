using UnityEngine;

namespace GeometricStrategy
{
    public sealed class PlayerCommandController : MonoBehaviour
    {
        [SerializeField] private FactionId controlledFaction = FactionId.PlayerOne;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private bool allowTabHotseatSwitch = true;

        private GeometricUnit selectedUnit;

        public FactionId ControlledFaction => controlledFaction;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void Update()
        {
            if (worldCamera == null) return;

            if (allowTabHotseatSwitch && Input.GetKeyDown(KeyCode.Tab))
                SwitchPlayer();

            if (Input.GetMouseButtonDown(0))
                SelectAtMouse();

            if (Input.GetMouseButtonDown(1) && selectedUnit != null)
                selectedUnit.SetMoveTarget(MouseWorldPoint());
        }

        public void SetControlledFaction(FactionId faction)
        {
            if (!GeometricGameRules.IsPlayerFaction(faction)) return;
            SetSelected(null);
            controlledFaction = faction;
        }

        public void SwitchPlayer()
        {
            SetControlledFaction(controlledFaction == FactionId.PlayerOne ? FactionId.PlayerTwo : FactionId.PlayerOne);
            Debug.Log("[GeometricStrategy] Active local player: " + controlledFaction);
        }

        private void SelectAtMouse()
        {
            Collider2D hit = Physics2D.OverlapPoint(MouseWorldPoint());
            GeometricUnit candidate = hit != null ? hit.GetComponent<GeometricUnit>() : null;

            if (candidate == null || candidate.Faction != controlledFaction)
            {
                SetSelected(null);
                return;
            }

            SetSelected(candidate);
        }

        private void SetSelected(GeometricUnit unit)
        {
            if (selectedUnit != null) selectedUnit.SetSelected(false);
            selectedUnit = unit;
            if (selectedUnit != null) selectedUnit.SetSelected(true);
        }

        private Vector3 MouseWorldPoint()
        {
            Vector3 point = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0f;
            return point;
        }
    }
}
