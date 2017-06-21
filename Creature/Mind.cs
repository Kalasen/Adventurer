using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KalaGame;

namespace Adventurer
{
    /// <summary>
    /// A mind, in which is held Artificial Intelligence. Or a long series of 'if' statements
    /// </summary>
    public class Mind
    {
        int intelligence, aggression, hostility;
        bool inventoryCheck = true; // Whether we need to look at the inventory
        Random rng = new Random();
        Stack<byte> path = new Stack<byte>();
        Point2D targetPos = new Point2D(-1,-1);
        Creature creature;

        public Mind(Creature creature, int intelligence = 50, int aggression = 50, int hostility = 50)
        {
            this.creature = creature;
            this.intelligence = intelligence;
            this.aggression = aggression;
            this.hostility = hostility;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="s">The Sentience to copy</param>
        public Mind(Mind s)
        {
            this.creature = s.creature;
			this.rng = s.rng;
			this.path = s.path;
			this.inventoryCheck = s.inventoryCheck;
            this.intelligence = s.intelligence;
            this.aggression = s.aggression;
            this.hostility = s.hostility;
        }

        public string DecideAction(Level currentLevel)
        {
            int dir = rng.Next(1, 8);
            if (dir >= 5)
                dir++;

            if (creature is QuestGiver)            
                return "Wait";            

            #region Array of positions
            Point2D[] newPos = new Point2D[10];
            newPos[1] = new Point2D(creature.pos.X - 1, creature.pos.Y + 1); //1
            newPos[2] = new Point2D(creature.pos.X    , creature.pos.Y + 1); //2     
            newPos[3] = new Point2D(creature.pos.X + 1, creature.pos.Y + 1); //3
            newPos[4] = new Point2D(creature.pos.X - 1, creature.pos.Y);     //4 
            newPos[6] = new Point2D(creature.pos.X + 1, creature.pos.Y);     //6
            newPos[7] = new Point2D(creature.pos.X - 1, creature.pos.Y - 1); //7 
            newPos[8] = new Point2D(creature.pos.X    , creature.pos.Y - 1); //8     
            newPos[9] = new Point2D(creature.pos.X + 1, creature.pos.Y - 1); //9 
            #endregion

            #region Gather States
            while (newPos[dir].X <= 0 || newPos[dir].X >= 80 || newPos[dir].Y <= 0 || newPos[dir].Y >= 40) //While out of bounds
            {
                dir = rng.Next(1, 8);
                if (dir >= 5) dir++;
            }

            int canAttackMeleeDir = 0;
            Point2D playerPos = currentLevel.creatures[0].pos;

            for (int y = 0; y < Level.GRIDH; y++)
                for (int x = 0; x < Level.GRIDW; x++)
                {
                    if (currentLevel.tileArray[x, y].itemList.Count > 0 &&
                        currentLevel.LineOfSight(creature.pos, new Point2D(x, y)) &&
                        path.Count == 0)
                    {
                        creature.targetPos = new Point2D(x, y); //Item is next target if seen
                        path = currentLevel.AStarPathfind(creature, creature.pos, new Point2D(x,y));
                    }
                }
            #endregion

            #region Decide on Action
            if (currentLevel.tileArray[creature.pos.X,
                creature.pos.Y].itemList.Count > 0) //If standing over an item
            {
                foreach (BodyPart b in creature.anatomy)
                {
                    if (b.Flags.HasFlag(BodyPartFlags.CanPickUpItem)) //If any part can pick up items
                    {
                        inventoryCheck = true; //We're picking up an item, so we need to see what we can do with it
                        return "Pick Up"; //Pick it up
                    }
                }
            }

            if (ShouldBeHostileTo(currentLevel.creatures.FirstOrDefault(c => c.isPlayer)) && currentLevel.LineOfSight(creature.pos, playerPos)) //If player is newly seen or smelled
            {
                targetPos = playerPos; //Keep the last known position in creature's memory
                canAttackMeleeDir = creature.AdjacentToCreatureDir(currentLevel);

                if (currentLevel.ConvertAbsolutePosToRelative(creature.pos, targetPos) > 0) //If adjacent
                {
                    return "Attack " + currentLevel.ConvertAbsolutePosToRelative(creature.pos, targetPos);
                }

                path = currentLevel.AStarPathfind(creature, creature.pos, playerPos); //Path to player
                return "Move " + path.Pop();
            }

            if (path.Count > 0) //If there's a target
            {
                if (targetPos == creature.pos) //If we're standing on it
                {
                    targetPos = new Point2D(-1,-1); //Forget this target
                    path.Clear();
                }
                else
                {
                    if (canAttackMeleeDir == path.Peek()) //If there's a creature in our way
                        return "Attack " + canAttackMeleeDir; //Attack it

                    if (path.Peek() > 0)
                        return "Move " + path.Pop(); //Go towards target
                    else
                    {
                        targetPos = new Point2D(-1,-1);
                        path.Clear();
                        return "Move " + dir; //Wander
                    }
                }
            }

            foreach (BodyPart b in creature.anatomy)
            {
                foreach (Item i in creature.inventory)
                {
                    if (i is Potion && (b.Injury.HasFlag(InjuryLevel.Mangled) || b.Injury.HasFlag(InjuryLevel.Broken))) //If the creature has a potion and is hurt badly
                    {
                        return "Eat " + creature.inventory.IndexOf(i);
                    }
                }
            }

            for (int y = 0; y < Level.GRIDH; y++)
                for (int x = 0; x < Level.GRIDW; x++)
                {
                    if (currentLevel.tileArray[x, y].itemList.Count > 0)
                    {
                        if (currentLevel.LineOfSight(creature.pos, new Point2D(x, y)))
                        {
                            foreach (BodyPart b in creature.anatomy)
                            {
                                if (b.Flags.HasFlag(BodyPartFlags.CanPickUpItem))
                                {
                                    targetPos = new Point2D(x, y);
                                    path = currentLevel.AStarPathfind(creature, creature.pos, playerPos); //Path to item
                                    break;
                                }
                            }
                        }
                    }
                }
            #endregion

            if (inventoryCheck)
            {
                foreach (Item i in creature.inventory) //Look at all the items
                {
                    if (i is Weapon)
                    {
                        if (creature.weapon == null)
                        {
                            return "Wield " + creature.inventory.IndexOf(i);
                        }
                        else
                        {
                            if (i.damage.average > creature.weapon.damage.average)
                            {
                                return "Unwield";
                            }
                        }
                    }

                    if (i is Armor)
                    {
                        if (creature.CanWear((Armor)i))
                        {
                            return "Wear " + creature.inventory.IndexOf(i);
                        }
                    }
                }

                inventoryCheck = false; //If we've checked everything and can't find a use, don't bother for a while
            }

            while (!creature.CanMoveBorder(dir))
            {
                dir = rng.Next(1, 9);
                if (dir >= 5)
                    dir++;
            }
            return "Move " + dir; //Default action            
        }

        public bool ShouldBeHostileTo(Creature targetCreature)
        {
            // If completely bloodthirsty, always yes
            if (hostility >= 100)
                return true;

            // Injured creatures are in fight or flight mode. In this case, just fight.
            if (creature.hp < creature.hpMax)
                return true;

            return false; // Default peaceful
        }
    }
}