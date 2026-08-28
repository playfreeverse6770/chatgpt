using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    [Serializable]
    public struct ResourceCost
    {
        public ResourceType type;
        [Min(0)] public int amount;

        public ResourceCost(ResourceType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }

    public sealed class CraftingRecipe
    {
        public CraftableType craftable;
        public ResourceCost[] costs;
        public ResourceType outputResource;
        public int outputAmount;

        public CraftingRecipe(CraftableType craftable, ResourceCost[] costs, ResourceType outputResource = ResourceType.Coin, int outputAmount = 0)
        {
            this.craftable = craftable;
            this.costs = costs;
            this.outputResource = outputResource;
            this.outputAmount = outputAmount;
        }
    }

    public static class CraftingCatalog
    {
        private static readonly Dictionary<CraftableType, CraftingRecipe> Recipes = new Dictionary<CraftableType, CraftingRecipe>
        {
            { CraftableType.Wall, new CraftingRecipe(CraftableType.Wall, new[] { new ResourceCost(ResourceType.Wood, 12) }) },
            { CraftableType.Bow, new CraftingRecipe(CraftableType.Bow, new[] { new ResourceCost(ResourceType.Wood, 8), new ResourceCost(ResourceType.Metal, 2) }) },
            { CraftableType.BasicWeapon, new CraftingRecipe(CraftableType.BasicWeapon, new[] { new ResourceCost(ResourceType.Wood, 4), new ResourceCost(ResourceType.Metal, 5) }) },
            { CraftableType.Catapult, new CraftingRecipe(CraftableType.Catapult, new[] { new ResourceCost(ResourceType.Wood, 28), new ResourceCost(ResourceType.Stone, 16), new ResourceCost(ResourceType.Metal, 8) }) },
            { CraftableType.AdvancedWeapon, new CraftingRecipe(CraftableType.AdvancedWeapon, new[] { new ResourceCost(ResourceType.Metal, 18), new ResourceCost(ResourceType.Gold, 4) }) },
            { CraftableType.Coin, new CraftingRecipe(CraftableType.Coin, new[] { new ResourceCost(ResourceType.Gold, 1) }, ResourceType.Coin, 10) }
        };

        public static CraftingRecipe Get(CraftableType type) => Recipes[type];
    }

    public sealed class CraftingSystem : MonoBehaviour
    {
        [SerializeField] private ResourceWallet wallet;
        [SerializeField] private CraftedInventory inventory;

        public void SetWallet(ResourceWallet value)
        {
            wallet = value;
        }

        public void SetInventory(CraftedInventory value)
        {
            inventory = value;
        }

        public bool CanCraft(CraftableType type)
        {
            if (wallet == null) return false;
            CraftingRecipe recipe = CraftingCatalog.Get(type);
            for (int i = 0; i < recipe.costs.Length; i++)
            {
                if (!wallet.CanAfford(recipe.costs[i].type, recipe.costs[i].amount))
                    return false;
            }
            return true;
        }

        public bool Craft(CraftableType type)
        {
            if (!CanCraft(type)) return false;
            CraftingRecipe recipe = CraftingCatalog.Get(type);

            for (int i = 0; i < recipe.costs.Length; i++)
                wallet.Spend(recipe.costs[i].type, recipe.costs[i].amount);

            if (recipe.outputAmount > 0)
                wallet.Add(recipe.outputResource, recipe.outputAmount);
            else if (inventory != null)
                inventory.Add(type, 1);

            if (GeometricAudioService.Instance != null)
                GeometricAudioService.Instance.Play(type == CraftableType.Coin ? AudioCue.Coin : AudioCue.Build, transform.position);

            return true;
        }
    }
}
