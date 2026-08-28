using UnityEngine;

namespace GeometricStrategy
{
    public sealed class PlayerCommandController : MonoBehaviour
    {
        [SerializeField] private FactionId controlledFaction = FactionId.PlayerOne;
        [SerializeField] private Camera worldCamera;

        private GeometricUnit selectedUnit;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void Update()
        {
            if (worldCamera == null) return;

            if (Input.GetMouseButtonDown(0))
                SelectAtMouse();

            if (Input.GetMouseButtonDown(1) && selectedUnit != null)
                selectedUnit.SetMoveTarget(MouseWorldPoint());
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
