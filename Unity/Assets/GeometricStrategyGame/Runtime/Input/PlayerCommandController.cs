using UnityEngine;

namespace GeometricStrategy
{
    public sealed class PlayerCommandController : MonoBehaviour
    {
        [SerializeField] private FactionId controlledFaction = FactionId.PlayerOne;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private bool allowTabHotseatSwitch = true;

        private GeometricUnit selectedUnit;
        private GameObject destinationMarker;

        public FactionId ControlledFaction => controlledFaction;
        public GeometricUnit SelectedUnit => selectedUnit;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            CreateDestinationMarker();
        }

        private void Update()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return;

            if (allowTabHotseatSwitch && Input.GetKeyDown(KeyCode.Tab))
                SwitchPlayer();

            if (Input.GetMouseButtonDown(0))
                SelectAtMouse();

            if (Input.GetMouseButtonDown(1) && selectedUnit != null)
            {
                Vector3 target = MouseWorldPoint();
                selectedUnit.SetMoveTarget(target);
                ShowDestinationMarker(target);
            }
        }

        public void SetControlledFaction(FactionId faction)
        {
            if (!GeometricGameRules.IsPlayerFaction(faction)) return;
            SetSelected(null);
            controlledFaction = faction;
            HideDestinationMarker();
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
            else HideDestinationMarker();
        }

        private Vector3 MouseWorldPoint()
        {
            Vector3 point = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0f;
            return point;
        }

        private void CreateDestinationMarker()
        {
            if (destinationMarker != null) return;
            destinationMarker = new GameObject("DestinationMarker");
            destinationMarker.transform.SetParent(transform, false);
            GeometricShapeRenderer renderer = destinationMarker.AddComponent<GeometricShapeRenderer>();
            renderer.Configure(GeometricSymbol.Diamond, new Color32(255, 255, 255, 180), 0.18f);
            destinationMarker.SetActive(false);
        }

        private void ShowDestinationMarker(Vector3 position)
        {
            if (destinationMarker == null) CreateDestinationMarker();
            destinationMarker.transform.position = new Vector3(position.x, position.y, -0.1f);
            destinationMarker.SetActive(true);
            CancelInvoke(nameof(HideDestinationMarker));
            Invoke(nameof(HideDestinationMarker), 1.1f);
        }

        private void HideDestinationMarker()
        {
            if (destinationMarker != null) destinationMarker.SetActive(false);
        }
    }
}
