using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    public sealed class CraftedInventory : MonoBehaviour
    {
        private readonly Dictionary<CraftableType, int> amounts = new Dictionary<CraftableType, int>();

        public event Action<CraftableType, int> Changed;

        public int Get(CraftableType type)
        {
            return amounts.TryGetValue(type, out int value) ? value : 0;
        }

        public void Add(CraftableType type, int amount = 1)
        {
            if (amount <= 0) return;
            int next = Get(type) + amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
        }

        public bool Consume(CraftableType type, int amount = 1)
        {
            if (amount <= 0 || Get(type) < amount) return false;
            int next = Get(type) - amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
            return true;
        }
    }
}
