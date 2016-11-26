using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class BodyPart
    {
        public string name { get; set; }
        public int maxHealth { get; set; }

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set
            {
                if (_currentHealth > maxHealth)
                    _currentHealth = maxHealth;

                if (_currentHealth < 0)
                    _currentHealth = 0;

                _currentHealth = value;
            }
        }                
        
        public BodyPartFlags flags;
        public InjuryLevel injury
        {
            get
            {
                if (currentHealth <= 0)
                    return InjuryLevel.Destroyed;
                if (currentHealth <= maxHealth * 0.25)
                    return InjuryLevel.Mangled;
                if (currentHealth <= maxHealth * 0.50)
                    return InjuryLevel.Broken;
                if (currentHealth <= maxHealth * 0.75)
                    return InjuryLevel.Minor;

                return InjuryLevel.Healthy;
            }
        }

        BodyPart parent;

        public BodyPart(string name, int health, BodyPartFlags flags, BodyPart parent = null)
        {
            this.name = name;
            this.currentHealth = this.maxHealth = health;
            this.flags = flags;
            this.parent = parent;
        }

        public BodyPart(BodyPart b)
        {
            this.name = b.name;            
            this.currentHealth = b.currentHealth;
            this.maxHealth = b.maxHealth;
            this.flags = b.flags;
            this.parent = b.parent != null ? new BodyPart(b.parent) : null;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public enum InjuryLevel
    {
        Healthy,
        Minor,
        Broken,
        Mangled,
        Destroyed
    }

    [Flags]
    public enum BodyPartFlags
    {
        None = 0,
        UsedForBreathing = 1 << 0,
        CanUseWeapon = 1 << 1,
        CanHoldItem = 1 << 2,
        CanWearJewelry = 1 << 3,
        CanPickUpItem = 1 << 4,
        CanHear = 1 << 5,
        CanSee = 1 << 6,
        CanSmell = 1 << 7,
        LifeCritical = 1 << 8
    }
}

