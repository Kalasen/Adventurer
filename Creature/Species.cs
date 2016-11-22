using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Species
    {        
        public byte speed, strength, dexterity, constitution, intelligence, wisdom, charisma, creatureImage, baseLevel;
        public short minDLevel;
        public int senseOfSmell, damageMin, damageMax, smelliness, bloodMax, mass, ac, xpWorth;
        public string name, armorType, habitat;
        public Color color;
        public List<BodyPart> anatomy;
        public Weapon weapon;
        public byte weaponChance;
        public List<Armor> armor; //A list of all armors
        public List<byte> armorChance; //A list of the chance to generate with it
        public List<Item> inventory;
        public List<byte> inventoryChance;
        public Random rng = new Random();
        public Dice rngDice = new Dice();
        public DNotation damage = new DNotation();

        public Species(byte speed, DNotation damage, int mass, byte imageIndex, Color color,
            string name, string habitat, List<BodyPart> anatomy, int seed)
        {
            xpWorth = 1;
            baseLevel = 0;
            rng = new Random(seed);
            rngDice = new Dice(rng.Next());
            this.damage = damage;
            this.habitat = habitat;
            this.speed = speed;
            this.mass = mass;
            this.creatureImage = imageIndex;
            this.name = name;
            this.color = color;
            this.anatomy = anatomy;
        }

        public Species(Species cG)
        {
			this.damage = new DNotation(cG.damage); // Deep copy
            this.xpWorth = cG.xpWorth;
            this.baseLevel = cG.baseLevel;
            this.habitat = cG.habitat;
            this.rng = cG.rng;
            this.strength = cG.strength;
            this.dexterity = cG.dexterity;
            this.constitution = cG.constitution;
            this.intelligence = cG.intelligence;
            this.wisdom = cG.wisdom;
            this.charisma = cG.charisma;
            this.creatureImage = cG.creatureImage;
            this.speed = cG.speed;
            this.senseOfSmell = cG.senseOfSmell;
            this.damage = cG.damage;
            this.smelliness = cG.smelliness;
            this.bloodMax = cG.bloodMax;
            this.mass = cG.mass;
            this.color = cG.color;
            this.anatomy = cG.anatomy;
            this.weapon = cG.weapon;
            this.weaponChance = cG.weaponChance;
            this.armor = cG.armor;
            this.armorChance = cG.armorChance;
            this.minDLevel = cG.minDLevel;
            this.name = cG.name;
            this.armorType = cG.armorType;
            
            this.inventory = cG.inventory;
            this.anatomy = cG.anatomy;
        }

        public Creature GenerateCreature(string socialClass, List<Item> ItemLibrary, int rngSeed)
        {
            rng = new Random(rngSeed); //Able to generate persistent creatures            
            List<BodyPart> thisCreatureAnatomy = new List<BodyPart>();
            foreach (BodyPart b in anatomy)
                thisCreatureAnatomy.Add(new BodyPart(b));
            Creature genCreature;
            if (socialClass == "monster")
                genCreature = new Creature(speed, damage, creatureImage, name, color, anatomy, mass, rng.Next());
            else
            {
                genCreature = new QuestGiver(speed, damage, creatureImage, name, color, anatomy, mass, rng.Next(), ItemLibrary);
            }
			genCreature.anatomy = new List<BodyPart>(); //Clear out whatever
            foreach (BodyPart b in thisCreatureAnatomy)
                genCreature.anatomy.Add(new BodyPart(b));

			if (rngDice.Roll(2) == 1) //1 in 2 chance
				genCreature.gold = rngDice.Roll(10,10); //10d10
            genCreature.armorType = armorType;            
            genCreature.wornArmor = armor;
            genCreature.weapon = weapon;
            genCreature.strength = (byte)rng.Next(7, 14); //7-13
            genCreature.dexterity = (byte)rng.Next(7, 14); //7-13
            genCreature.constitution = (byte)rng.Next(7, 14); //7-13
            genCreature.hpMax = rngDice.Roll(baseLevel, 8);
            if (baseLevel <= 0)
                genCreature.hpMax = rngDice.Roll(4);
            genCreature.hp = genCreature.hpMax;
            genCreature.intelligence = (byte)rng.Next(7, 14); //7-13
            genCreature.wisdom = (byte)rng.Next(7, 14); //7-13
            genCreature.charisma = (byte)rng.Next(7, 14); //7-13
            genCreature.xpWorth = xpWorth;
            foreach (Item i in this.inventory)            
                genCreature.inventory.Add(i);

            thisCreatureAnatomy.Clear();

            return genCreature;            
        }       
    }
}
