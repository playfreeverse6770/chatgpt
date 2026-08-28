using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class GeometricStrategyHud : MonoBehaviour
    {
        [SerializeField] private PlayerCommandController commands;
        [SerializeField] private GeometricStrategyGameManager gameManager;
        [SerializeField] private ResourceWallet playerOneWallet;
        [SerializeField] private ResourceWallet playerTwoWallet;
        [SerializeField] private bool showTutorial = true;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle smallStyle;
        private GUIStyle centerStyle;
        private GUIStyle resourceStyle;
        private Texture2D panelTexture;
        private Texture2D accentTexture;

        public void Configure(PlayerCommandController controller, GeometricStrategyGameManager manager, ResourceWallet p1, ResourceWallet p2)
        {
            commands = controller;
            gameManager = manager;
            playerOneWallet = p1;
            playerTwoWallet = p2;
        }

        private void Awake()
        {
            BuildStyles();
        }

        private void OnDestroy()
        {
            if (panelTexture != null) Destroy(panelTexture);
            if (accentTexture != null) Destroy(accentTexture);
        }

        private void OnGUI()
        {
            if (commands == null) commands = FindObjectOfType<PlayerCommandController>();
            if (gameManager == null) gameManager = FindObjectOfType<GeometricStrategyGameManager>();
            if (panelTexture == null) BuildStyles();

            FactionId active = commands != null ? commands.ControlledFaction : FactionId.PlayerOne;
            ResourceWallet wallet = active == FactionId.PlayerTwo ? playerTwoWallet : playerOneWallet;

            DrawTopBar(active, wallet);
            DrawSelectedUnit();
            DrawWorldLabels();
            DrawTutorial();
            DrawMatchEnd();
        }

        private void DrawTopBar(FactionId active, ResourceWallet wallet)
        {
            float width = Mathf.Min(Screen.width - 24f, 900f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, 12f, width, 68f);
            GUI.DrawTexture(panel, panelTexture, ScaleMode.StretchToFill);

            string playerName = active == FactionId.PlayerOne ? "PLAYER 1" : "PLAYER 2";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 8f, 130f, 28f), playerName, titleStyle);
            GUI.Label(new Rect(panel.x + 18f, panel.y + 35f, 150f, 20f), "TAB = SWITCH PLAYER", smallStyle);

            if (wallet == null) return;
            string resources =
                "WOOD  " + wallet.Get(ResourceType.Wood) +
                "    STONE  " + wallet.Get(ResourceType.Stone) +
                "    METAL  " + wallet.Get(ResourceType.Metal) +
                "    GOLD  " + wallet.Get(ResourceType.Gold) +
                "    COIN  " + wallet.Get(ResourceType.Coin) +
                "    FOOD  " + wallet.Get(ResourceType.Food);
            GUI.Label(new Rect(panel.x + 165f, panel.y + 23f, panel.width - 180f, 26f), resources, resourceStyle);
        }

        private void DrawSelectedUnit()
        {
            if (commands == null || commands.SelectedUnit == null) return;
            GeometricUnit unit = commands.SelectedUnit;
            float hp = unit.Stats.maxHealth <= 0f ? 0f : unit.CurrentHealth / unit.Stats.maxHealth;
            Rect box = new Rect(16f, Screen.height - 105f, 280f, 88f);
            GUI.DrawTexture(box, panelTexture, ScaleMode.StretchToFill);
            GUI.Label(new Rect(box.x + 14f, box.y + 8f, box.width - 28f, 24f), unit.Archetype + "   LEVEL " + (int)unit.Level, titleStyle);
            GUI.Label(new Rect(box.x + 14f, box.y + 35f, box.width - 28f, 20f), "HP  " + Mathf.CeilToInt(unit.CurrentHealth) + " / " + Mathf.CeilToInt(unit.Stats.maxHealth), bodyStyle);
            Rect hpBg = new Rect(box.x + 14f, box.y + 61f, box.width - 28f, 10f);
            GUI.Box(hpBg, GUIContent.none);
            GUI.DrawTexture(new Rect(hpBg.x, hpBg.y, hpBg.width * Mathf.Clamp01(hp), hpBg.height), accentTexture, ScaleMode.StretchToFill);
        }

        private void DrawWorldLabels()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            GeometricUnit[] units = FindObjectsOfType<GeometricUnit>();
            for (int i = 0; i < units.Length; i++)
            {
                GeometricUnit unit = units[i];
                if (unit == null || !unit.IsAlive) continue;
                Vector3 screen = cam.WorldToScreenPoint(unit.transform.position + Vector3.up * 0.9f);
                if (screen.z < 0f) continue;
                float hp = unit.Stats.maxHealth <= 0f ? 0f : Mathf.Clamp01(unit.CurrentHealth / unit.Stats.maxHealth);
                float x = screen.x - 25f;
                float y = Screen.height - screen.y;
                GUI.Box(new Rect(x, y, 50f, 5f), GUIContent.none);
                GUI.DrawTexture(new Rect(x, y, 50f * hp, 5f), accentTexture, ScaleMode.StretchToFill);
                GUI.Label(new Rect(x - 10f, y + 5f, 70f, 18f), "L" + (int)unit.Level, centerStyle);
            }

            ResourceNode[] resources = FindObjectsOfType<ResourceNode>();
            for (int i = 0; i < resources.Length; i++)
            {
                ResourceNode node = resources[i];
                if (node == null || node.IsDepleted) continue;
                Vector3 screen = cam.WorldToScreenPoint(node.transform.position + Vector3.up * 0.72f);
                if (screen.z < 0f) continue;
                GUI.Label(new Rect(screen.x - 55f, Screen.height - screen.y, 110f, 20f), node.ResourceType.ToString().ToUpperInvariant(), centerStyle);
            }
        }

        private void DrawTutorial()
        {
            if (!showTutorial) return;
            Rect panel = new Rect(Screen.width - 300f, Screen.height - 128f, 284f, 112f);
            GUI.DrawTexture(panel, panelTexture, ScaleMode.StretchToFill);
            GUI.Label(new Rect(panel.x + 14f, panel.y + 8f, 250f, 22f), "HOW TO PLAY", titleStyle);
            GUI.Label(new Rect(panel.x + 14f, panel.y + 34f, 250f, 70f),
                "LEFT CLICK  Select unit\nRIGHT CLICK  Move selected unit\nTAB  Switch Player 1 / Player 2\nDestroy the enemy SUN KING to win",
                bodyStyle);
        }

        private void DrawMatchEnd()
        {
            if (gameManager == null || !gameManager.MatchEnded) return;
            Rect panel = new Rect(Screen.width * 0.5f - 210f, Screen.height * 0.5f - 55f, 420f, 110f);
            GUI.DrawTexture(panel, panelTexture, ScaleMode.StretchToFill);
            GUI.Label(new Rect(panel.x, panel.y + 18f, panel.width, 34f), "KING DEFEATED", centerStyle);
            GUI.Label(new Rect(panel.x, panel.y + 58f, panel.width, 26f), gameManager.ResultMessage, centerStyle);
        }

        private void BuildStyles()
        {
            panelTexture = MakeTexture(new Color32(18, 24, 34, 235));
            accentTexture = MakeTexture(new Color32(73, 225, 183, 255));

            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = new Color32(220, 230, 242, 255) } };
            smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 10, normal = { textColor = new Color32(145, 162, 184, 255) } };
            resourceStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = new Color32(244, 248, 255, 255) } };
            centerStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
        }

        private static Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
