#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometricStrategy.Editor
{
    public static class GeometricStrategyMvpBuilder
    {
        private sealed class FactionEconomy
        {
            public GameObject root;
            public ResourceWallet wallet;
            public CraftedInventory inventory;
            public LivestockInventory livestock;
            public CraftingSystem crafting;
        }

        [MenuItem("Tools/Geometric Strategy/Build MVP Scene")]
        public static void BuildMvpScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject root = new GameObject("GeometricStrategy_MVP");
            GameObject systems = Child(root, "Systems");
            GameObject units = Child(root, "Units");
            GameObject resources = Child(root, "Resources");
            GameObject workers = Child(root, "Workers");

            CreateCamera(systems.transform);

            GameObject gameSystems = Child(systems, "GameSystems");
            gameSystems.AddComponent<GeometricStrategyGameManager>();
            gameSystems.AddComponent<GeometricAudioService>();
            gameSystems.AddComponent<PlayerCommandController>();

            FactionEconomy playerOne = CreateFactionEconomy(systems.transform, FactionId.PlayerOne);
            FactionEconomy playerTwo = CreateFactionEconomy(systems.transform, FactionId.PlayerTwo);

            ResourceNode wood = CreateResource(resources.transform, ResourceType.Wood, new Vector2(-2.8f, 4.4f), 180);
            ResourceNode stone = CreateResource(resources.transform, ResourceType.Stone, new Vector2(-0.9f, 4.4f), 180);
            ResourceNode metal = CreateResource(resources.transform, ResourceType.Metal, new Vector2(1.0f, 4.4f), 140);
            ResourceNode gold = CreateResource(resources.transform, ResourceType.Gold, new Vector2(2.9f, 4.4f), 100);

            CreatePlayerArmy(units.transform, FactionId.PlayerOne, -1f);
            CreatePlayerArmy(units.transform, FactionId.PlayerTwo, 1f);

            CreateRaider(units.transform, FactionId.WolfClan, UnitArchetype.Soldier, UnitLevel.Level2, new Vector2(0f, 6.2f));
            CreateRaider(units.transform, FactionId.BearClan, UnitArchetype.Cavalry, UnitLevel.Level3, new Vector2(0f, -6.2f));
            CreateRaider(units.transform, FactionId.EagleClan, UnitArchetype.Archer, UnitLevel.Level2, new Vector2(5.2f, 5.4f));

            CreateProfession(workers.transform, ProfessionType.Carpenter, playerOne, wood, new Vector2(-8.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Miner, playerOne, stone, new Vector2(-7.1f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Farmer, playerOne, null, new Vector2(-5.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.AnimalBreeder, playerOne, null, new Vector2(-4.1f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Blacksmith, playerOne, null, new Vector2(-2.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Weaponsmith, playerOne, null, new Vector2(-1.1f, -4.9f));

            CreateProfession(workers.transform, ProfessionType.Carpenter, playerTwo, wood, new Vector2(8.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Miner, playerTwo, metal, new Vector2(7.1f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Farmer, playerTwo, null, new Vector2(5.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.AnimalBreeder, playerTwo, null, new Vector2(4.1f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Blacksmith, playerTwo, null, new Vector2(2.6f, -4.9f));
            CreateProfession(workers.transform, ProfessionType.Weaponsmith, playerTwo, gold, new Vector2(1.1f, -4.9f));

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[GeometricStrategy] MVP scene generated. Save the scene, then enter Play Mode. Left click selects units; right click moves them; Tab switches Player One / Player Two control.");
        }

        private static void CreateCamera(Transform parent)
        {
            GameObject go = new GameObject("Main Camera");
            go.transform.SetParent(parent, false);
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 0f, -10f);

            Camera camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 8.2f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = GeometricPalette.BoardBackground;
            go.AddComponent<AudioListener>();
        }

        private static FactionEconomy CreateFactionEconomy(Transform parent, FactionId faction)
        {
            GameObject go = new GameObject(faction + "_Economy");
            go.transform.SetParent(parent, false);

            ResourceWallet wallet = go.AddComponent<ResourceWallet>();
            wallet.SetOwner(faction);
            CraftedInventory inventory = go.AddComponent<CraftedInventory>();
            LivestockInventory livestock = go.AddComponent<LivestockInventory>();
            CraftingSystem crafting = go.AddComponent<CraftingSystem>();
            crafting.SetWallet(wallet);
            crafting.SetInventory(inventory);

            return new FactionEconomy
            {
                root = go,
                wallet = wallet,
                inventory = inventory,
                livestock = livestock,
                crafting = crafting
            };
        }

        private static ResourceNode CreateResource(Transform parent, ResourceType type, Vector2 position, int amount)
        {
            GameObject go = new GameObject("Resource_" + type);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            ResourceNode node = go.AddComponent<ResourceNode>();
            node.Configure(type, amount);
            return node;
        }

        private static void CreatePlayerArmy(Transform parent, FactionId faction, float side)
        {
            float x = 7.6f * side;
            CreateUnit(parent, faction, UnitArchetype.King, UnitLevel.Level1, new Vector2(x, 0f), false);
            CreateUnit(parent, faction, UnitArchetype.Soldier, UnitLevel.Level1, new Vector2(6.0f * side, 0f), false);
            CreateUnit(parent, faction, UnitArchetype.Soldier, UnitLevel.Level1, new Vector2(6.0f * side, 1.45f), false);
            CreateUnit(parent, faction, UnitArchetype.Archer, UnitLevel.Level1, new Vector2(6.0f * side, 2.9f), false);
            CreateUnit(parent, faction, UnitArchetype.Cavalry, UnitLevel.Level1, new Vector2(6.0f * side, -1.8f), false);
        }

        private static GeometricUnit CreateRaider(Transform parent, FactionId faction, UnitArchetype archetype, UnitLevel level, Vector2 position)
        {
            return CreateUnit(parent, faction, archetype, level, position, true);
        }

        private static GeometricUnit CreateUnit(Transform parent, FactionId faction, UnitArchetype archetype, UnitLevel level, Vector2 position, bool addRaiderAI)
        {
            GameObject go = new GameObject(faction + "_" + archetype);
            go.transform.SetParent(parent, false);
            go.transform.position = position;

            GeometricUnit unit = go.AddComponent<GeometricUnit>();
            unit.Configure(archetype, level, faction);
            go.AddComponent<GeometricAutoCombat>();

            if (addRaiderAI)
                go.AddComponent<GeometricRaiderAI>();

            return unit;
        }

        private static void CreateProfession(Transform parent, ProfessionType profession, FactionEconomy economy, ResourceNode resource, Vector2 position)
        {
            GameObject go = new GameObject(economy.wallet.Owner + "_" + profession);
            go.transform.SetParent(parent, false);
            go.transform.position = position;

            GeometricShapeRenderer shape = go.AddComponent<GeometricShapeRenderer>();
            shape.Configure(ProfessionSymbol(profession), ProfessionColor(profession), 0.48f);

            ProfessionWorker worker = go.AddComponent<ProfessionWorker>();
            worker.Configure(profession, economy.wallet, economy.crafting, resource, economy.livestock, AnimalType.Horse);
        }

        private static GeometricSymbol ProfessionSymbol(ProfessionType profession)
        {
            switch (profession)
            {
                case ProfessionType.Farmer: return GeometricSymbol.Diamond;
                case ProfessionType.Miner: return GeometricSymbol.Octagon;
                case ProfessionType.Weaponsmith: return GeometricSymbol.Diamond;
                default: return GeometricSymbol.Hexagon;
            }
        }

        private static Color ProfessionColor(ProfessionType profession)
        {
            switch (profession)
            {
                case ProfessionType.Blacksmith: return GeometricPalette.Blacksmith;
                case ProfessionType.Farmer: return GeometricPalette.Farmer;
                case ProfessionType.Carpenter: return GeometricPalette.Carpenter;
                case ProfessionType.Miner: return GeometricPalette.Miner;
                case ProfessionType.Weaponsmith: return GeometricPalette.Weaponsmith;
                case ProfessionType.AnimalBreeder: return GeometricPalette.AnimalBreeder;
                default: return Color.white;
            }
        }

        private static GameObject Child(GameObject parent, string name)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            return child;
        }
    }
}
#endif
