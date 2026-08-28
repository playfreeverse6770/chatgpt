using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    public sealed class LivestockInventory : MonoBehaviour
    {
        private readonly Dictionary<AnimalType, int> amounts = new Dictionary<AnimalType, int>();

        public event Action<AnimalType, int> Changed;

        public int Get(AnimalType type)
        {
            return amounts.TryGetValue(type, out int value) ? value : 0;
        }

        public void Add(AnimalType type, int amount = 1)
        {
            if (amount <= 0) return;
            int next = Get(type) + amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
        }

        public bool Consume(AnimalType type, int amount = 1)
        {
            if (amount <= 0 || Get(type) < amount) return false;
            int next = Get(type) - amount;
            amounts[type] = next;
            Changed?.Invoke(type, next);
            return true;
        }
    }
}
