using UnityEngine;

namespace GeometricStrategy
{
    public sealed class ProfessionWorker : MonoBehaviour
    {
        [SerializeField] private ProfessionType profession = ProfessionType.Farmer;
        [SerializeField] private ResourceWallet wallet;
        [SerializeField] private CraftingSystem crafting;
        [SerializeField] private ResourceNode assignedResource;
        [SerializeField, Min(0.25f)] private float workInterval = 2f;
        [SerializeField, Min(1)] private int harvestAmount = 3;

        private float nextWorkTime;

        public ProfessionType Profession => profession;

        private void Start()
        {
            EnsureEmblem();
        }

        private void Update()
        {
            if (Time.time < nextWorkTime) return;
            nextWorkTime = Time.time + workInterval;
            WorkOnce();
        }

        public void Configure(ProfessionType role, ResourceWallet targetWallet, CraftingSystem craftingSystem = null, ResourceNode resource = null)
        {
            profession = role;
            wallet = targetWallet;
            crafting = craftingSystem;
            assignedResource = resource;
            EnsureEmblem();
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
                    if (!wallet.Spend(ResourceType.Food, 2)) return false;
                    wallet.Add(ResourceType.Horse, 1);
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

        private void EnsureEmblem()
        {
            Transform emblem = transform.Find("ProfessionEmblem");
            if (emblem == null)
            {
                var go = new GameObject("ProfessionEmblem");
                emblem = go.transform;
                emblem.SetParent(transform, false);
                emblem.localPosition = new Vector3(0f, 0f, -0.08f);
            }

            GeometricEmblemRenderer renderer = emblem.GetComponent<GeometricEmblemRenderer>();
            if (renderer == null) renderer = emblem.gameObject.AddComponent<GeometricEmblemRenderer>();
            renderer.ConfigureProfession(profession, Color.white);
        }
    }
}
