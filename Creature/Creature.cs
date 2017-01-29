using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;
using KalaGame;

namespace Adventurer
{
    public class Creature
    {
        #region Variables
        public byte speed, strength, dexterity, constitution, intelligence, wisdom, charisma, level;
        public byte turn_energy = 0;
        public int senseOfSmell, creatureImage, turnsToWait, smelliness, mass, gold,
            minDLevel, blind, confusion, coma, detectItem, detectMonster, seeInvisible, invisibility, speedy, 
			levitate, revive, sleep, food, seed, killCount, hp, mp, hpMax, mpMax, gp, xp, xpWorth, xpBorder; //TODO: Change to flags enum what we can
        public bool isAlive, isDextrous, isPlayer;
        public byte xpLevel;
        public string name, armorType;
		public Amulet amulet;
        public Point2D targetPos, pos;
        public Color color;
        public Random rng = new Random();
        public Dice rngDie = new Dice();
        public DNotation attack = new DNotation();
        public Item weapon;
        public List<Item> inventory = new List<Item>();
        public List<BodyPart> anatomy = new List<BodyPart>();
        public List<BodyPart> lostParts = new List<BodyPart>();
        public List<Armor> wornArmor = new List<Armor>();
        public List<string> message = new List<string>(10);
        public Sentience mind;
        public byte status_paralyzed = 0;		
        #endregion

        public Creature(byte speed, DNotation attack, int creatureImage,
            string name, Color color, List<BodyPart> anatomy, int mass, int rngSeed)
        {
            xpBorder = 10;
            level = 1;
            xpWorth = 1;
            hp = 10;
            hpMax = 10;
            mp = 10;
            mpMax = 10;
            gp = 0;
            xp = 0;
            xpLevel = 1;
            rng = new Random(rngSeed);
			this.gold = 0;
            this.attack = new DNotation(attack);
            this.killCount = 0;
            this.food = 15000; //Start with 15000 food units
            this.isPlayer = false;
            this.anatomy = new List<BodyPart>(anatomy);
            this.isAlive = true;
            this.creatureImage = creatureImage;
            this.speed = speed;
            this.turnsToWait = speed;
            this.mind = new Sentience();
            this.name = name;
            this.smelliness = 32;
            this.senseOfSmell = 10;
            this.color = color;
            this.mass = mass;
            this.minDLevel = 1;
            this.rng = new Random(rngSeed);

            foreach (BodyPart b in this.anatomy)
            {
                if (b.flags.HasFlag(BodyPartFlags.CanPickUpItem))
                {
                    this.isDextrous = true;
                    b.flags |= BodyPartFlags.CanUseWeapon;
                }
            }
        }
        public Creature(Creature c)
        {
			this.gold = c.gold;
			this.attack = new DNotation(c.attack); //Deep copy
            this.turn_energy = c.turn_energy;
            this.level = c.level;
            this.xpWorth = c.xpWorth;
            this.attack = c.attack;
            this.xp = c.xp;
            this.xpLevel = c.xpLevel;
            this.xpBorder = c.xpBorder;
            this.hp = c.hp;
            this.hpMax = c.hpMax;
            this.mp = c.mp;
            this.mpMax = c.mpMax;
            this.gp = c.gp;
            this.killCount = c.killCount;
            this.seed = c.seed;
            this.food = c.food;
            this.isPlayer = c.isPlayer;
            this.rng = c.rng;
            this.isAlive = true;
            this.creatureImage = c.creatureImage;
            this.speed = c.speed;
            this.turnsToWait = c.speed;
            this.mind = c.mind;
            this.name = c.name;
            this.smelliness = c.smelliness;
            this.senseOfSmell = c.senseOfSmell;
            this.mass = c.mass;
            this.color = c.color;
            this.pos = c.pos;
            this.minDLevel = c.minDLevel;
            this.armorType = c.armorType;
            this.weapon = c.weapon;
            this.strength = c.strength;
            this.dexterity = c.dexterity;
            this.constitution = c.constitution;
            this.intelligence = c.intelligence;
            this.wisdom = c.wisdom;
            this.charisma = c.charisma;

			this.anatomy = new List<BodyPart>();
            foreach (BodyPart b in c.anatomy)
                this.anatomy.Add(new BodyPart(b));
			
			this.inventory = new List<Item>();
            foreach (Item i in c.inventory)
                this.inventory.Add(i);
			
			this.wornArmor = new List<Armor>();
            foreach (Armor a in c.wornArmor)
                this.wornArmor.Add(a);

            this.isDextrous = c.isDextrous;
        }

		public void Affect(Effect e, Level currentLevel)
		{
			Affect(e, currentLevel, false); //No mute
		}
		public void Affect(Effect e, Level currentLevel, bool mute)
		{
			switch(e.type)
			{
			case "blind":
				blind = e.magnitude;				
				if (!mute) message.Add("Your eyes! YOUR EYES OR OTHER SENSORY ORGAN(S)!");
				break;
				
			case "choke":
				hp -= e.magnitude;
				if (!mute) message.Add("Hrk! You can't breathe.");
				break;
				
			case "confuse":
				confusion = e.magnitude;				
				if (!mute) message.Add("Has anyone really been far as decided to use even to go want to do more like?");
				break;
				
			case "detect monster":
				detectMonster = e.magnitude;
				if (!mute) message.Add("You feel a sense of presence.");
				break;
				
			case "detect item":
				detectItem = e.magnitude;
				if (!mute) message.Add("You feel an intuition about treasure.");
				break;
				
			case "gain ability":
				strength += (byte) rngDie.Roll(e.magnitude);
				dexterity += (byte)rngDie.Roll(e.magnitude);
				constitution += (byte)rngDie.Roll(e.magnitude);
				intelligence += (byte)rngDie.Roll(e.magnitude);
				wisdom += (byte)rngDie.Roll(e.magnitude);
				charisma += (byte)rngDie.Roll(e.magnitude);
				
				if (strength > 18)
					strength = 18;
				if (dexterity > 18)
					dexterity = 18;
				if (constitution > 18)
					constitution = 18;
				if (intelligence > 18)
					intelligence = 18;
				if (wisdom > 18)
					wisdom = 18;
				if (charisma > 18)
					charisma = 18;
				
				if (!mute) message.Add("Oh, wow... you feel awesome.");
				break;
				
			case "heal":
				foreach (BodyPart b in anatomy)
                {
                    b.currentHealth += (int)(b.maxHealth * 0.25f);

                    if (b.currentHealth > b.maxHealth)
                        b.currentHealth = b.maxHealth;
                }

                hp += e.magnitude;
                if (hp > hpMax)
                    hp = hpMax;

                if (!mute) message.Add("You feel better.");
				break;
				
			case "invisibility":
				invisibility = e.magnitude;
				if (!mute)
				{
					message.Add("Your body becomes transparent.");
					message.Add("Monsters seem to still sense you, though. Somehow.");
				}
				break;
				
			case "level up":
				LevelUpForSlayingImp();
				if (!mute) message.Add("You feel more experienced.");
				break;
				
			case "levitation":
				levitate = e.magnitude;
				if (!mute) message.Add("You float up off the ground.");
				break;
				
			case "paralyze":
				status_paralyzed = (byte)e.magnitude;
				if (!mute) message.Add("You're rooted to the spot!");
				break;
				
			case "poison":
				hp -= e.magnitude;
				if (!mute) message.Add("You don't feel so well");
				break;
				
			case "polymorph":
				if (!mute) 
				{
					message.Add("You don't feel like yourself.");
					message.Add("Actually, you feel like the universe is too lazy to make anything happen yet.");
				}
				break;
				
			case "regenerate body part":
				if (lostParts.Count > 0)
                {
                    lostParts[0].currentHealth = lostParts[0].maxHealth; //Heal part
                    anatomy.Add(lostParts[0]); //Restore part                    
                    if (!mute) message.Add("A shiver runs through your body, and your missing " + lostParts[0].name + " grows back where it once was, good as new.");
                    lostParts.RemoveAt(0);                    
                }
                else
                {
                    if (!mute) message.Add("A tingling runs through your body briefly.");
                }
				break;
				
			case "revive":
				revive = e.magnitude;
				if (!mute) message.Add("You are quite certain that nothing bad will happen. Nothing at all.");
				break;
				
			case "see invisible":
				seeInvisible = e.magnitude;
				if (!mute) message.Add("Your senses seem clearer.");
				break;
				
			case "sleep":
				status_paralyzed = (byte)e.magnitude;
				if (!mute) message.Add("shhh only dreams now");
				break;
				
			case "speed":
				speedy = e.magnitude;
				speed *= (byte)(1 + (e.magnitude / 100)); //Percentage boost
				if (!mute) message.Add("You feel much faster.");
				break;
			}
		}
        public int AdjacentToCreatureDir(Level currentLevel)
        {
            #region Array of positions
            Point2D[] newPos = new Point2D[10];
            newPos[1] = new Point2D(this.pos.X - 1, this.pos.Y + 1); //1
            newPos[2] = new Point2D(this.pos.X, this.pos.Y + 1); //2     
            newPos[3] = new Point2D(this.pos.X + 1, this.pos.Y + 1); //3
            newPos[4] = new Point2D(this.pos.X - 1, this.pos.Y);     //4 
            newPos[6] = new Point2D(this.pos.X + 1, this.pos.Y);     //6
            newPos[7] = new Point2D(this.pos.X - 1, this.pos.Y - 1); //7 
            newPos[8] = new Point2D(this.pos.X, this.pos.Y - 1); //8     
            newPos[9] = new Point2D(this.pos.X + 1, this.pos.Y - 1); //9 
            #endregion

            for (int i = 1; i <= 9; i++)
            {
                if (i == 5)
                    i++; //Skip position 5

                if (currentLevel.IsCreatureAt(newPos[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        public void BreakDownItem(Level currentLevel, Item i)
        {
            if (i.componentList.Count == 0)
            {
                message.Add("You mangle the " + i.name + " into useless debris.");
            }
            else
            {
                foreach (Item t in i.componentList)
                {
                    //byte phase = 0; //0 - solid, 1 - liquid, 2 - vapor
					Material m = t.material;
//                    if (m.meltPoint < 15.555) //Room temperature
//                        phase = 1;
//
//                    if (m.boilPoint < 15.555) //60 fahrenheit, or 15.5 celsius
//                        phase = 2;

                    if (t.name == "raw material")
                    {
                        message.Add("Some liquid spills everywhere");
                        break;
                    }
//                    else if (phase == 2)
//                    {
//                        message.Add("Some vapor escapes");
//                        break;
//                    }
//
//                    if (phase == 0)
					else
                    {                        
                        inventory.Add(t);
                        message.Add("You salvage the " + t.name + " from the " + i.name + ".");
                        if (inventory.Count > 26)
                        {
                            message.Add("Not having room in your inventory, you drop it on the ground.");
                            Drop(currentLevel, t); //Drop item
                        }
                    }
                }
            }

            inventory.Remove(i);
        }
        public bool CanMoveBorder(int dir)
        {
            #region Array of positions
            Point2D[] newPos = new Point2D[10];
            newPos[1] = new Point2D(this.pos.X - 1, this.pos.Y + 1);
            newPos[2] = new Point2D(this.pos.X, this.pos.Y + 1);
            newPos[3] = new Point2D(this.pos.X + 1, this.pos.Y + 1);
            newPos[4] = new Point2D(this.pos.X - 1, this.pos.Y);
            newPos[6] = new Point2D(this.pos.X + 1, this.pos.Y);
            newPos[7] = new Point2D(this.pos.X - 1, this.pos.Y - 1);
            newPos[8] = new Point2D(this.pos.X, this.pos.Y - 1);
            newPos[9] = new Point2D(this.pos.X + 1, this.pos.Y - 1);
            #endregion

            if (newPos[dir].X < 1 || newPos[dir].Y < 1 || newPos[dir].X > Level.GRIDW - 1 || newPos[dir].Y > Level.GRIDH - 1)
                return false;

            return true;
        }
        public bool CanAttackMelee(Level currentLevel, Directions dir)
        {
            return currentLevel.IsCreatureAt(pos.AdjacentVector(dir));
        }
        public bool CanWear(Armor a)
        {
            if (!(a is Armor)) //If it's not armor
            {
                message.Add("That isn't armor.");
                return false;
            }

            if (a.shape != this.armorType) //If it's not the right shape for this creature
            {
                message.Add("That doesn't fit on your body.");
                return false;
            }

            foreach (BodyPart b in anatomy)
                foreach (string s in a.covers)
                {
                    foreach (Armor other in wornArmor)
                        foreach (string otherS in other.covers)
                        {
                            if (s == otherS) //If some piece of armor is already covering a part
                            {
                                message.Add("Your " + other.name + " is already covering your " + s + ".");
                                return false;
                            }
                        }
                    if (s == b.name) //If it fits on any body part
                        return true;
                }

            message.Add("That doesn't fit on any part of your body.");
            return false;
        }
        public bool CanWield(Item w)
        {
            foreach (BodyPart b in anatomy)
                if (b.flags.HasFlag(BodyPartFlags.CanUseWeapon))
                    return true; //If can wield weapon

            message.Add("You have no body parts that can use a weapon.");
            return false;
        }
        public void CloseDoor(Tile tile, Level currentLevel)
        {
            Door door = (Door)tile.fixtureLibrary[0];
            door.Close(tile, currentLevel);
        }
		public void CycleWithWorld(Level currentLevel)
		{
			if (status_paralyzed > 0)
				status_paralyzed--;
			if (blind > 0)
				blind--;
			if (confusion > 0)
				confusion--;
			if (coma > 0)
				coma--;
			if (detectItem > 0)
				detectItem--;
			if (detectMonster > 0)
				detectMonster--;
			if (seeInvisible > 0)
				seeInvisible--;
			if (invisibility > 0)
				invisibility--;
			if (levitate > 0)
				levitate--;
			if (revive > 0)
				revive--;
			
			if (speedy > 0)
			{
				speedy--;
				if (speedy <= 0)
					speed = (byte)(speed*0.66); //Bring back to normal speed
			}
			
			if (amulet != null)
				Affect(amulet.effect, currentLevel, true); //Affect ourselves with the amulet if we have it
		}
        public void CombineItem(Item component, Item target)
        {
            foreach (Item i in target.componentList)
                inventory.Remove(i);

            inventory.Add(target);
            message.Add("You make the " + target.name + " from the " + component.name + ".");
        }
        public List<Item> FindValidRecipe(Item i, List<Item> itemLibrary)
        {
            List<Item> candidate = new List<Item>(); //A list to hold all recipes
            List<Item> reject = new List<Item>(); //Candidates that will be rejected from the list
            List<string> inventorySnapshot = new List<string>();
            foreach (Item t in inventory)
                inventorySnapshot.Add(t.name);
            int count;

            foreach (Item t in itemLibrary) //Find all items in the world
                foreach (Item e in t.componentList) //To see if this item is part of their component recipe                
                    if (e.name == i.name) //If any component matches this, we've found a recipe
                        candidate.Add(t); //Add this as a candidate

            foreach (Item t in candidate) //For every candidate item to make
            {
                inventorySnapshot = new List<string>();
                foreach (Item z in inventory)
                    inventorySnapshot.Add(z.name);
                foreach (Item e in t.componentList) //For every component in the candidate item
                {
                    bool found = false; //Whether we've found the right component
                    count = inventorySnapshot.Count;
                    for (int m = 0; m < count; m++)
                    {
                        if (inventorySnapshot[m] == e.name) //If we've found it
                        {
                            found = true; //Then we've found it
                            inventorySnapshot.RemoveAt(m); //It will be used, don't keep it around
                            count--;
                        }
                    }
					for (int n = 0; n < inventory.Count; n++)                    
						if (!found) //If we haven't got a matching component in inventory
                            reject.Add(t); //Reject it from the list
                }
            }

            foreach (Item t in reject)
                candidate.Remove(t); //Remove the rejects from the candidate list

            restart:
            count = candidate.Count;
            for (int k = 0; k < count; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (candidate[k] == candidate[j] && k != j)
                    {
                        candidate.RemoveAt(k);
                        goto restart;
                    }
                }
            }
            
            return candidate; //Return all the items we can make from this one                
        } //Find all items that can be made from this item
        public Level Drop(Level currentLevel, Item dropItem)
        {
            int x = pos.X;
            int y = pos.Y;

            currentLevel.tileArray[x, y].itemList.Add(dropItem); //Add item to tile 
            inventory.Remove(dropItem); //From creature
            this.message.Add("You drop the " + dropItem.name + ".");
            return currentLevel;
        }
        public Level Eat(Level currentLevel, Item eatItem)
        {
            int itemIndex = inventory.IndexOf(eatItem);
            this.message.Add("You ingest the " + eatItem.name + ".");
            if (currentLevel.LineOfSight(currentLevel.creatures[0].pos, this.pos) &&
                currentLevel.creatures[0].pos != this.pos) //If the player sees it and it's not the player
            {
                currentLevel.creatures[0].message.Add("The " + this.name + " ingests a " + eatItem.name + ".");
            }

            currentLevel = inventory[itemIndex].Eat(currentLevel, this);
            return currentLevel;
        }
        public bool InMeleeRange(Level currentLevel)
        {
            #region Array of positions
            Point2D[] newPos = new Point2D[9];
            newPos[1] = new Point2D(this.pos.X - 1, this.pos.Y + 1); //1
            newPos[2] = new Point2D(this.pos.X, this.pos.Y + 1); //2     
            newPos[3] = new Point2D(this.pos.X + 1, this.pos.Y + 1); //3
            newPos[4] = new Point2D(this.pos.X - 1, this.pos.Y);     //4 
            newPos[6] = new Point2D(this.pos.X + 1, this.pos.Y);     //6
            newPos[7] = new Point2D(this.pos.X - 1, this.pos.Y - 1); //7 
            newPos[8] = new Point2D(this.pos.X, this.pos.Y - 1); //8     
            newPos[9] = new Point2D(this.pos.X + 1, this.pos.Y - 1); //9 
            #endregion

            for (int i = 1; i <= 9; i++)
            {
                if (i == 5)
                    i++; //Skip over position 5

                if (currentLevel.IsCreatureAt(newPos[i])) //If there's a creature in any direction
                    return true;
            }

            return false;
        }
        public void LevelUpForSlayingImp()
        {
            while (xp > xpBorder * 2) //Multiple levels in one go
            {
                xpBorder *= 2; //AKA 20,40,80,160,320, etc.
                int hpPrev = hpMax;
                hpMax += (rngDie.Roll(constitution) + rngDie.Roll(hpMax)) / 2;
                hp += hpMax - hpPrev;
                level++;
            }
        }
        public void LosePart(Level currentLevel, BodyPart b)
        {
            lostParts.Add(b); //Keep track of lost, removed parts

            Item part = new Item(100f, 100f, $"{name} {b.name}", Color.Red, new List<Item>(), new List<string>());            
            part.edible = true;
            part.itemImage = 253;           
            part.nutrition = 500;
            currentLevel.tileArray[pos.X, pos.Y].itemList.Add(new Item(part)); //Gibs

            anatomy.Remove(b); //But do remove them.
        }
        public Level MeleeAttack(Level currentLevel, Directions dir)
        {
            if (dir == Directions.NONE)
                throw new ArgumentOutOfRangeException("dir", "Attempted to melee attack own location");
            int opponentIndex = -1;

            #region Attack creature in given direction
            if (currentLevel.CreatureNAt(pos.AdjacentVector(dir)) >= 0)
            {
                opponentIndex = currentLevel.CreatureNAt(pos.AdjacentVector(dir));
                var opponent = currentLevel.creatures[opponentIndex];

                if (opponent is QuestGiver)
                {
                    //TODO: Make aggressive
                }

                byte chanceToMiss = 10;
                chanceToMiss += (byte)(15 - dexterity);
                if (rng.Next(0, 101) < chanceToMiss)
                {
                    message.Add("You miss the " + currentLevel.creatures[opponentIndex].name + ".");
                    currentLevel.creatures[opponentIndex].message.Add("The " + name + " misses you.");
                }
                else
                {
                    int damage = 1;
                    if (weapon == null)
                        damage = rngDie.Roll(attack);
                    else
                        damage = rngDie.Roll(weapon.damage);                    

                    int partIndex = rng.Next(0, currentLevel.creatures[opponentIndex].anatomy.Count);
                    BodyPart part = currentLevel.creatures[opponentIndex].anatomy[partIndex];
                    foreach (Armor a in wornArmor)
                        foreach (string s in a.covers)
                            if (s == part.name)
                            {
                                //damage -= a.aC;
                            }                    

                    if (damage <= 0)
                    {
                        damage = 0;
                        message.Add("You deal no damage.");
                    }

                    var damageBefore = currentLevel.creatures[opponentIndex].anatomy[partIndex].injury;
                    currentLevel.creatures[opponentIndex].TakeDamage(damage);
                    var damageAfter = currentLevel.creatures[opponentIndex].anatomy[partIndex].injury;

                    message.Add("You hit the " + currentLevel.creatures[opponentIndex].name + " in the " +
                        part.name + " for " + damage + " damage.");
                    currentLevel.creatures[opponentIndex].message.Add("The " + this.name + " hits you in the " +
                        part.name + " for " + damage + " damage.");

                    if (damageBefore != damageAfter)
                    {
                        switch(damageAfter)
                        {
                            case InjuryLevel.Minor:
                                message.Add($"You wound the {currentLevel.creatures[opponentIndex].name}'s {part.name}.");
                                break;
                            case InjuryLevel.Broken:
                                message.Add($"You break the {currentLevel.creatures[opponentIndex].name}'s {part.name}.");
                                break;
                            case InjuryLevel.Mangled:
                                message.Add($"You mangle the {currentLevel.creatures[opponentIndex].name}'s {part.name}.");
                                break;
                            case InjuryLevel.Destroyed:
                                message.Add($"You obliterate the {currentLevel.creatures[opponentIndex].name}'s {part.name}.");
                                break;
                        }
                    }

                    if (weapon is Potion)
                    {
                        Potion p = (Potion)weapon;
                        currentLevel.creatures[opponentIndex].inventory.Add(p);
                        p.Eat(currentLevel, currentLevel.creatures[opponentIndex]); //Smash, effect affects the creature
                        message.Add($"The ${p.name} smashes against the ${currentLevel.creatures[opponentIndex].name}");
                        weapon = null; //Smashed, gone
                    }

                    if (opponentIndex == 0) //If player
                    {
                        currentLevel.causeOfDeath = $"lethal damage to your {part.name}.";

                        if(weapon == null)
                            currentLevel.mannerOfDeath = $"you were hit in the {part.name} by a {name}.";
                        else
                            currentLevel.mannerOfDeath = $"you were struck in the {part.name} by a {weapon.name} wielded by a {name}.";                        
                    }
                }
            }
            #endregion

            #region If Killed Opponent
            if (currentLevel.creatures[opponentIndex].ShouldBeDead(currentLevel))
            {
                if (opponentIndex == 0) //If it was the player
                {
                    currentLevel.playerDiedHere = true;
                }
                else
                {
					Creature c = currentLevel.creatures[opponentIndex];
                    killCount++;
                    xp += currentLevel.creatures[opponentIndex].xpWorth;
                    if (xp > xpBorder*2) //AKA 20, 40, 80, 160
                        LevelUpForSlayingImp();
                    int count = currentLevel.creatures[opponentIndex].inventory.Count;
                    for (int i = 0; i < count; i++)
                        currentLevel = currentLevel.creatures[opponentIndex].Drop(currentLevel,
                            currentLevel.creatures[opponentIndex].inventory[0]); //Drop on death

                    this.message.Add("You kill the " + currentLevel.creatures[opponentIndex].name + ".");

                    if (currentLevel.creatures[opponentIndex].name == "dragon")
                        this.message.Add("You monster.");

                    Item corpse = new Item(500f, 
                        500f, 
                        $"{currentLevel.creatures[opponentIndex].name} corpse",
					    currentLevel.creatures[opponentIndex].color,
                        new List<Item>(),
                        new List<string>()
                    );
                    corpse.itemImage = 253; //"�"
                    corpse.edible = true;
                    corpse.nutrition = 3000; //For now, default to this
                    currentLevel.tileArray[currentLevel.creatures[opponentIndex].pos.X, currentLevel.creatures[opponentIndex].pos.Y].itemList.Add(new Item(corpse)); //Gibs

                    for (int y = 1; y < Level.GRIDH; y++)
                        for (int x = 1; x < Level.GRIDW; x++)
                        {
                            currentLevel.tileArray[x, y].scentIdentifier.RemoveAt(opponentIndex); //Remove it from scent tracking
                            currentLevel.tileArray[x, y].scentMagnitude.RemoveAt(opponentIndex);
                        }
                    currentLevel.creatures.RemoveAt(opponentIndex); //Creature is gone ***Improve with death drop***
                }
            }
            #endregion

            return currentLevel; //Give back the level now that we're done with it
        }

        //TODO: Update to use Directions enum
        public bool Move(Level currentLevel, int dir)
        {
            #region Array of positions
            Point2D[] newPos = new Point2D[10];
            newPos[1] = new Point2D(this.pos.X - 1, this.pos.Y + 1); //1
            newPos[2] = new Point2D(this.pos.X, this.pos.Y + 1); //2     
            newPos[3] = new Point2D(this.pos.X + 1, this.pos.Y + 1); //3
            newPos[4] = new Point2D(this.pos.X - 1, this.pos.Y);     //4 
            newPos[6] = new Point2D(this.pos.X + 1, this.pos.Y);     //6
            newPos[7] = new Point2D(this.pos.X - 1, this.pos.Y - 1); //7 
            newPos[8] = new Point2D(this.pos.X, this.pos.Y - 1); //8     
            newPos[9] = new Point2D(this.pos.X + 1, this.pos.Y - 1); //9 
            #endregion

            if (!currentLevel.IsCreatureAt(newPos[dir]) && //If no creature's there and..
                currentLevel.tileArray[(int)newPos[dir].X, (int)newPos[dir].Y].isPassable) //...the tile is passable...
            {
                this.pos = newPos[dir];
				
				if (confusion > 0 && rngDie.Roll(2) == 1) //If confused, then half the time...
				{
					dir = rng.Next(1,9); //1-8 //Randomize movement
					if (dir >= 5)
						dir++; //Skip 5
				}
                if (!String.IsNullOrEmpty(currentLevel.tileArray[newPos[dir].X, newPos[dir].Y].engraving))
                {
                    message.Add("Something is written here:");
                    message.Add(currentLevel.tileArray[newPos[dir].X, newPos[dir].Y].engraving); //Any message here
                }

                if (currentLevel.tileArray[pos.X, pos.Y].fixtureLibrary.Count > 0) //Check what's there, if anything
                {
                    foreach (Fixture f in currentLevel.tileArray[pos.X, pos.Y].fixtureLibrary)
                    {
                        if (f is Trap) //General Ackbar: "IT'S A TRAP!"
                        {
                            Trap t = (Trap)f; //"No, f, you are the traps. And then f was a trap
                            if (t.effect.type == "tripwire" && !(isPlayer && t.visible)) //If it's a tripwire... KEEP WORKING HERE
                            {
                                if (isPlayer || currentLevel.LineOfSight(currentLevel.creatures[0].pos, pos))
                                {
                                    t.visible = true; //We know it's here now
                                }
                                //t.armed = false; //Should this be disarmed when tripped over?
                                message.Add("You trip over a tripwire."); //WHOOPS
                                status_paralyzed += (byte)t.effect.magnitude;
                            }
                        }
                    }
                }

                //If there's an item on the tile
                if (currentLevel.tileArray[pos.X, pos.Y].itemList.Count > 0)
                {
                    message.Add("There is a " + currentLevel.tileArray[pos.X, pos.Y].itemList[0].name + " here.");
                }
				
				return true; //Moving took a turn
            }
            else if (currentLevel.tileArray[(int)newPos[dir].X, (int)newPos[dir].Y].isDoor && isDextrous)
            {
                OpenDoor(currentLevel.tileArray[(int)newPos[dir].X, (int)newPos[dir].Y], currentLevel);
                message.Add("You open a door");
				return true; //Door opening took a turn
            }
			
			return false; //Didn't spend a turn
        } //Returns whether a turn was taken
        public void OpenDoor(Tile tile, Level currentLevel)
        {
            if (isDextrous == true)
            {                
                Door door = (Door)tile.fixtureLibrary[0];
                door.Open(tile, currentLevel);
            }
            else
            {
                message.Add("Opposable thumbs would be helpful here.");
            }
        }
        public Level PickUp(Level currentLevel)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            int stackSize = currentLevel.tileArray[x, y].itemList.Count;

            if (stackSize > 0) //If there's an item at the player's position
            {
                if (currentLevel.tileArray[x, y].itemList[stackSize - 1] == null)
                {
                    currentLevel.tileArray[x, y].itemList.RemoveAt(stackSize - 1);
                }
                else
                {
                    Item pickedItem = currentLevel.tileArray[x, y].itemList[stackSize - 1];
					if (pickedItem is Currency)
					{
						Currency pickedCash = (Currency) pickedItem;
						gold += pickedCash.worth; //Add value						
					}
					else
					{
	                    inventory.Add(pickedItem); //Transfer to creature
	                    this.message.Add("You pick up the " + pickedItem.name + ".");
					}
					currentLevel.tileArray[x, y].itemList.Remove(pickedItem); //From tile
					//Wait(currentLevel, speed / 2); //Picking up an item takes half a turn
                }
            }

            return currentLevel;
        }
        public void RangeAttack(Level currentLevel, Creature target, Item firedItem)
        {
            #region Attack creature in given direction
            if (target is QuestGiver)
            {
                Creature c = (Creature)target;
                target = new Creature(c); //And then target was a monster
                message.Add("The " + c.name + " gets angry!"); //And s/he's mad.
            }

            byte chanceToMiss = 10; //The chance to miss target
            chanceToMiss += (byte)(15 - dexterity); //Dex bonus
            if (rng.Next(0, 101) < chanceToMiss) //If miss
            {
                message.Add("You miss the " + target.name + ".");
                target.message.Add("The " + name + " misses you.");
                currentLevel.tileArray[targetPos.X, targetPos.Y].itemList.Add(firedItem); //It ends up on selected tile                   
            }
            else
            {
                int damage = rngDie.Roll(attack); //Inclusive

                //damage = (int)((float)damage * ((float)strength / 10f)); //Strength bonus

                int partIndex = rng.Next(0, target.anatomy.Count);
                BodyPart part = target.anatomy[partIndex];
                foreach (Armor a in wornArmor)
                    foreach (string s in a.covers)
                        if (s == part.name)
                            damage -= a.aC;
                //float conBonus = target.constitution / 10f;
                //damage = (int)((float)damage / conBonus);

                target.TakeDamage(damage);

                this.message.Add("You hit the " + target.name + " in the " + part.name + " for " + damage + " damage.");
                target.message.Add("The " + this.name + " hits you in the " + part.name + " for " + damage + " damage.");

                if (firedItem is Potion)
                {
                    Potion p = (Potion)firedItem;
                    target.inventory.Add(p);
                    p.Eat(currentLevel, target); //Smash, effect affects the creature
                    this.message.Add("The " + p.name + " smashes against the " + target.name);
                }
                else
                {
                    currentLevel.tileArray[targetPos.X, targetPos.Y].itemList.Add(firedItem); //It ends up on selected tile
                }

                if (target == currentLevel.creatures[0]) //If player
                {
                    currentLevel.causeOfDeath = "lethal damage to your " + part.name + ".";
                    currentLevel.mannerOfDeath = "you were struck in the " + part.name + " by a " + firedItem.name + " thrown by a " + name + ".";
                }

                inventory.Remove(firedItem); //Remove item from inventory
            }
            #endregion

            #region If Killed Opponent
            if (target.ShouldBeDead(currentLevel))
            {
                killCount++;
                if (currentLevel.creatures.IndexOf(target) == 0) //If it was the player
                {
                    currentLevel.playerDiedHere = true;
                }
                else
                {
                    int count = target.inventory.Count;
                    for (int i = 0; i < count; i++)
                        currentLevel = target.Drop(currentLevel, target.inventory[0]); //Drop on death

                    this.message.Add("You kill the " + target.name + ".");

                    if (target.name == "dragon")
                        this.message.Add("You monster.");

                    Item corpse = new Item(target.mass, target.mass, $"{target.name} corpse", target.color, new List<Item>(), new List<string>());
                    corpse.itemImage = 253;
                    corpse.edible = true;
                    corpse.nutrition = 3000; //For now, default to this
                    currentLevel.tileArray[target.pos.X, target.pos.Y].itemList.Add(new Item(corpse)); //Gibs                    

                    for (int y = 1; y < 40; y++)
                        for (int x = 1; x < 80; x++)
                        {
                            currentLevel.tileArray[x, y].scentIdentifier.RemoveAt(currentLevel.creatures.IndexOf(target)); //Remove it from scent tracking
                            currentLevel.tileArray[x, y].scentMagnitude.RemoveAt(currentLevel.creatures.IndexOf(target));
                        }
                    currentLevel.creatures.RemoveAt(currentLevel.creatures.IndexOf(target)); //Creature is gone ***Improve with death drop***
                }
            }
            #endregion
        }
        public void Regenerate(Level currentLevel)
        {
            if (rngDie.Roll(413) < constitution * 4 && hp < hpMax)
            {
                hp++;
            }
        } //A regeneration and health check cycle
        public void Remove(Armor a)
        {
            inventory.Add(a);
            wornArmor.Remove(a);
        }
        public void RemoveAll()
        {
            int count = wornArmor.Count;
            for (int i = 0; i < count; i++)
            {
                inventory.Add(wornArmor[0]);
                message.Add("You remove the " + wornArmor[0].name + ".");
                wornArmor.Remove(wornArmor[0]);
            }
        }
        public Level Scent(Level currentLevel)
        {
            int index = currentLevel.creatures.IndexOf(this);
            if (pos.X > 0 && pos.X < 80 && pos.Y > 0 && pos.Y < 40)
            {
                currentLevel.tileArray[pos.X, pos.Y].scentMagnitude[index] += smelliness;
            }
            return currentLevel;
        }
        public bool ShouldBeDead(Level currentLevel)
        {
            if (hp <= 0)
            {
                currentLevel.causeOfDeath = "overall body damage.";
                return true;
            }

            foreach (BodyPart b in this.anatomy)
            {
                if (b.flags.HasFlag(BodyPartFlags.LifeCritical) && b.injury == InjuryLevel.Destroyed) //If your head is chunky salsa, you're dead.          
                    return true;
            }

            return false; //If none of those trip, be glad to be alive.
        }
        public void TakeDamage(int damage)
        {
            hp -= damage;
        }
        public void Unwield()
        {
            message.Add("You put away the " + weapon.name + ".");
            inventory.Add(weapon);
            weapon = null;
        }		
		public void Wait(Level currentLevel)
		{
			turn_energy -= Adventurer.TURN_THRESHOLD; //If we've gotten here, the turn's been taken
            Scent(currentLevel);
            Regenerate(currentLevel);
		}
        public void Wear(Armor a)
        {
            wornArmor.Add(a);
            message.Add("You wear the " + a.name + ".");
            inventory.Remove(a);
        }
		public void Wear(Amulet a, Level currentLevel)
		{
			if (amulet != null)
			{
				inventory.Add(new Amulet(amulet));
				message.Add("You take off the " + amulet.name + " and wear the " + a.name + ".");
			}
			else
				message.Add("You wear the " + a.name + ".");
			
			amulet = a;            
            inventory.Remove(a);
			Affect(a.effect, currentLevel);
		}
        public void Wield(Item w)
        {
            if (weapon == null)
            {
                weapon = w;
                message.Add("You wield the " + w.name + ".");
                inventory.Remove(w);
            }
            else
            {
                inventory.Add(weapon);
                message.Add("You put away the wielded " + weapon.name + " and instead wield the " + w.name + ".");
                weapon = w;
                inventory.Remove(w);
                
            }
        }

        public override string ToString()
        {
            return name;
        }
    } //Whether monster or player, they're all creatures
}
