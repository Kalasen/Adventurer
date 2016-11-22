using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class BodyPart
    {
        public bool usedForBreathing, canUseWeapon, canHoldItem, canWearJewelry, canPickUpItem, isInjured,
            isBroken, isMangled, canHear, canSee, canSmell, lifeCritical; //TODO: Change to flags enum?
        public string name, parent, armorType;
        public int noInjury, minorInjury, breakInjury, mangleInjury, removeInjury, currentHealth; //TODO: Change to single int?
        public Armor armor;

        public BodyPart(string name, string parent, bool canPickUpItem, string armorType, int noInjury,
            int minorInjury, int breakInjury, int mangleInjury)
        {
            this.name = name;
            this.parent = parent;
            this.currentHealth = noInjury;
            this.noInjury = noInjury;
            this.minorInjury = minorInjury;
            this.breakInjury = breakInjury;
            this.mangleInjury = mangleInjury;
            this.canPickUpItem = canPickUpItem;
            this.canUseWeapon = canPickUpItem;
            this.armorType = armorType;
        }

        public BodyPart(BodyPart b)
        {
            this.name = b.name;
            this.parent = b.parent;
            this.currentHealth = b.currentHealth;
            this.noInjury = b.noInjury;
            this.minorInjury = b.minorInjury;
            this.breakInjury = b.breakInjury;
            this.mangleInjury = b.mangleInjury;
            this.canPickUpItem = b.canPickUpItem;
            this.armorType = b.armorType;
            this.lifeCritical = b.lifeCritical;
            this.canUseWeapon = b.canUseWeapon;
        }
    } //Part of what makes up a creature is the parts of its body
}
