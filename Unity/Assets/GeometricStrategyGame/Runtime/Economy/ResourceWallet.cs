using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    public sealed class ResourceWallet : MonoBehaviour
    {
        [Serializable]
        public struct StartingResource
        {
            public ResourceType type;
            [Min(0)] public int amount;
        }

        [SerializeField] private FactionId owner = FactionId.PlayerOne;
        [SerializeField] private List<StartingResource> startingResources = new List<StartingResource>();

        private readonly Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

        public event Action<ResourceType, int> Changed;
        public FactionId Owner => owner;

        private void Awake()
        {
            amounts.Clear();
            for (int i = 0; i < startingResources.Count; i++)
                Add(startingResources[i].type, startingResources[i].amount);
        }

        public void SetOwner(FactionId faction)
        {
            owner = faction;
        }

        public int Get(ResourceType type)
        {
            return amounts.TryGetValue(type, out int value) ? value : 0;
        }

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0) return;
            int next = Get(type) + amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
        }

        public bool CanAfford(ResourceType type, int amount)
        {
            return amount >= 0 && Get(type) >= amount;
        }

        public bool Spend(ResourceType type, int amount)
        {
            if (amount < 0 || !CanAfford(type, amount)) return false;
            int next = Get(type) - amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
            return true;
        }
    }
}
