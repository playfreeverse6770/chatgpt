using System;
using UnityEngine;

namespace GeometricStrategy
{
    public enum UnitArchetype
    {
        Soldier,
        Archer,
        Cavalry,
        King
    }

    public enum UnitLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7
    }

    public enum FactionId
    {
        Neutral,
        PlayerOne,
        PlayerTwo,
        WolfClan,
        BearClan,
        EagleClan
    }

    public enum ResourceType
    {
        Wood,
        Stone,
        Metal,
        Gold,
        Coin,
        Food,
        Horse
    }

    public enum ProfessionType
    {
        Blacksmith,
        Farmer,
        Carpenter,
        Miner,
        Weaponsmith,
        AnimalBreeder
    }

    public enum CraftableType
    {
        Wall,
        Bow,
        BasicWeapon,
        Catapult,
        AdvancedWeapon,
        Coin
    }

    public enum GeometricSymbol
    {
        Circle,
        Triangle,
        Square,
        Sun,
        SixPointStar,
        Trapezoid,
        Pentagon,
        Hexagon,
        Diamond,
        Octagon
    }

    public enum AudioCue
    {
        Hit,
        ArrowShot,
        Wolf,
        Bear,
        Eagle,
        Victory,
        Defeat,
        Upgrade,
        Build,
        Harvest,
        Coin
    }

    [Serializable]
    public struct UnitStats
    {
        public float maxHealth;
        public float damage;
        public float attackRange;
        public float attacksPerSecond;
        public float moveSpeed;

        public UnitStats(float maxHealth, float damage, float attackRange, float attacksPerSecond, float moveSpeed)
        {
            this.maxHealth = maxHealth;
            this.damage = damage;
            this.attackRange = attackRange;
            this.attacksPerSecond = attacksPerSecond;
            this.moveSpeed = moveSpeed;
        }
    }

    public static class GeometricGameRules
    {
        public static Color LevelColor(UnitLevel level)
        {
            switch (level)
            {
                case UnitLevel.Level1: return new Color32(255, 221, 0, 255);
                case UnitLevel.Level2: return new Color32(255, 136, 0, 255);
                case UnitLevel.Level3: return new Color32(220, 45, 45, 255);
                case UnitLevel.Level4: return new Color32(140, 65, 190, 255);
                case UnitLevel.Level5: return new Color32(60, 175, 80, 255);
                case UnitLevel.Level6: return new Color32(90, 190, 245, 255);
                case UnitLevel.Level7: return new Color32(15, 70, 180, 255);
                default: return Color.white;
            }
        }

        public static Color ResourceColor(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood: return new Color32(46, 160, 67, 255);
                case ResourceType.Stone: return new Color32(125, 125, 125, 255);
                case ResourceType.Metal: return new Color32(120, 72, 42, 255);
                case ResourceType.Gold: return new Color32(235, 190, 30, 255);
                case ResourceType.Coin: return new Color32(255, 210, 55, 255);
                case ResourceType.Food: return new Color32(180, 210, 75, 255);
                case ResourceType.Horse: return new Color32(150, 95, 55, 255);
                default: return Color.white;
            }
        }

        public static GeometricSymbol UnitSymbol(UnitArchetype type)
        {
            switch (type)
            {
                case UnitArchetype.Soldier: return GeometricSymbol.Circle;
                case UnitArchetype.Archer: return GeometricSymbol.Triangle;
                case UnitArchetype.Cavalry: return GeometricSymbol.Square;
                case UnitArchetype.King: return GeometricSymbol.Sun;
                default: return GeometricSymbol.Circle;
            }
        }

        public static GeometricSymbol ResourceSymbol(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood: return GeometricSymbol.SixPointStar;
                case ResourceType.Stone: return GeometricSymbol.Trapezoid;
                case ResourceType.Metal:
                case ResourceType.Gold: return GeometricSymbol.Pentagon;
                default: return GeometricSymbol.Hexagon;
            }
        }

        public static UnitStats Stats(UnitArchetype archetype, UnitLevel level)
        {
            float multiplier = 1f + (((int)level - 1) * 0.18f);
            UnitStats baseline;

            switch (archetype)
            {
                case UnitArchetype.Archer:
                    baseline = new UnitStats(70f, 16f, 5.4f, 0.8f, 2.7f);
                    break;
                case UnitArchetype.Cavalry:
                    baseline = new UnitStats(130f, 22f, 1.3f, 0.75f, 4.1f);
                    break;
                case UnitArchetype.King:
                    baseline = new UnitStats(350f, 24f, 2.1f, 0.6f, 1.9f);
                    break;
                default:
                    baseline = new UnitStats(100f, 18f, 1.35f, 0.9f, 2.5f);
                    break;
            }

            baseline.maxHealth *= multiplier;
            baseline.damage *= multiplier;
            baseline.moveSpeed *= 1f + (((int)level - 1) * 0.025f);
            return baseline;
        }

        public static bool IsPlayerFaction(FactionId faction)
        {
            return faction == FactionId.PlayerOne || faction == FactionId.PlayerTwo;
        }

        public static bool AreEnemies(FactionId a, FactionId b)
        {
            if (a == FactionId.Neutral || b == FactionId.Neutral || a == b)
                return false;

            return true;
        }
    }
}
