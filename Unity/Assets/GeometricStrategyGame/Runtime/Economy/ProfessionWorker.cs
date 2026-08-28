using UnityEngine;

namespace GeometricStrategy
{
    public sealed class ProfessionWorker : MonoBehaviour
    {
        [SerializeField] private ProfessionType profession = ProfessionType.Farmer;
        [SerializeField] private ResourceWallet wallet;
        [SerializeField] private CraftingSystem crafting;
        [SerializeField] private ResourceNode assignedResource;
        [SerializeField] private LivestockInventory livestock;
        [SerializeField] private AnimalType breedingAnimal = AnimalType.Horse;
        [SerializeField, Min(0.25f)] private float workInterval = 2f;
        [SerializeField, Min(1)] private int harvestAmount = 3;

        private float nextWorkTime;

        public ProfessionType Profession => profession;

        private void Start()
        {
            EnsureEmblems();
        }

        private void Update()
        {
            if (Time.time < nextWorkTime) return;
            nextWorkTime = Time.time + workInterval;
            WorkOnce();
        }

        public void Configure(
            ProfessionType role,
            ResourceWallet targetWallet,
            CraftingSystem craftingSystem = null,
            ResourceNode resource = null,
            LivestockInventory livestockInventory = null,
            AnimalType animal = AnimalType.Horse)
        {
            profession = role;
            wallet = targetWallet;
            crafting = craftingSystem;
            assignedResource = resource;
            livestock = livestockInventory;
            breedingAnimal = animal;
            EnsureEmblems();
        }

        public bool WorkOnce()
        {
            if (wallet == null) return false;

            switch (profession)
            {
                case ProfessionType.Miner:
                    if (assignedResource == null || assignedResource.IsDepleted) return false;
                    if (assignedResource.ResourceType != ResourceType.Stone &&
                        assignedResource.ResourceType != ResourceType.Metal &&
                        assignedResource.ResourceType != ResourceType.Gold)
                        return false;
                    return assignedResource.Harvest(harvestAmount, wallet) > 0;

                case ProfessionType.Carpenter:
                    if (assignedResource == null || assignedResource.IsDepleted) return false;
                    if (assignedResource.ResourceType != ResourceType.Wood) return false;
                    return assignedResource.Harvest(harvestAmount, wallet) > 0;

                case ProfessionType.Farmer:
                    wallet.Add(ResourceType.Food, 2);
                    return true;

                case ProfessionType.AnimalBreeder:
                    if (livestock == null || !wallet.Spend(ResourceType.Food, 2)) return false;
                    livestock.Add(breedingAnimal, breedingAnimal == AnimalType.Chicken ? 2 : 1);
                    return true;

                case ProfessionType.Blacksmith:
                    return crafting != null && crafting.Craft(CraftableType.BasicWeapon);

                case ProfessionType.Weaponsmith:
                    if (crafting == null) return false;
                    return crafting.Craft(CraftableType.AdvancedWeapon) || crafting.Craft(CraftableType.Bow);

                default:
                    return false;
            }
        }

        private void EnsureEmblems()
        {
            Transform professionEmblem = transform.Find("ProfessionEmblem");
            if (professionEmblem == null)
            {
                var go = new GameObject("ProfessionEmblem");
                professionEmblem = go.transform;
                professionEmblem.SetParent(transform, false);
                professionEmblem.localPosition = new Vector3(0f, 0f, -0.08f);
            }

            GeometricEmblemRenderer professionRenderer = professionEmblem.GetComponent<GeometricEmblemRenderer>();
            if (professionRenderer == null) professionRenderer = professionEmblem.gameObject.AddComponent<GeometricEmblemRenderer>();
            professionRenderer.ConfigureProfession(profession, Color.white);

            if (wallet == null) return;

            Transform ownerEmblem = transform.Find("OwnerEmblem");
            if (ownerEmblem == null)
            {
                var go = new GameObject("OwnerEmblem");
                ownerEmblem = go.transform;
                ownerEmblem.SetParent(transform, false);
            }

            ownerEmblem.localPosition = new Vector3(0.28f, -0.28f, -0.1f);
            ownerEmblem.localScale = Vector3.one * 0.42f;

            GeometricEmblemRenderer ownerRenderer = ownerEmblem.GetComponent<GeometricEmblemRenderer>();
            if (ownerRenderer == null) ownerRenderer = ownerEmblem.gameObject.AddComponent<GeometricEmblemRenderer>();
            ownerRenderer.ConfigureFaction(wallet.Owner);
        }
    }
}
