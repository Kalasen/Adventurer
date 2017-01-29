using System;
using System.Collections.Generic;
using System.Text;
using KalaGame;

namespace Adventurer
{
    public class Sentience
    {
        int intelligence, aggression, hostility;
        bool inventoryCheck = true; //Whether we need to look at the inventory
        Random rng = new Random();
        Stack<byte> path = new Stack<byte>();
        Vector2 targetPos = new Vector2(-1,-1);

        public Sentience():this(50,50,50){} //Default Constructor
        public Sentience(int intelligence, int aggression, int hostility)
        {
            this.intelligence = intelligence;
            this.aggression = aggression;
            this.hostility = hostility;
        }
        public Sentience(Sentience s)
        {
			this.rng = s.rng;
			this.path = s.path;
			this.inventoryCheck = s.inventoryCheck;
            this.intelligence = s.intelligence;
            this.aggression = s.aggression;
            this.hostility = s.hostility;
        } //Copy method

        public string DecideAction(Level currentLevel, Creature thisCreature)
        {
            int dir = rng.Next(1, 8);
            if (dir >= 5)
                dir++;

            if (thisCreature is QuestGiver)            
                return "Wait";            

            #region Array of positions
            Vector2[] newPos = new Vector2[10];
            newPos[1] = new Vector2(thisCreature.pos.X - 1, thisCreature.pos.Y + 1); //1
            newPos[2] = new Vector2(thisCreature.pos.X    , thisCreature.pos.Y + 1); //2     
            newPos[3] = new Vector2(thisCreature.pos.X + 1, thisCreature.pos.Y + 1); //3
            newPos[4] = new Vector2(thisCreature.pos.X - 1, thisCreature.pos.Y);     //4 
            newPos[6] = new Vector2(thisCreature.pos.X + 1, thisCreature.pos.Y);     //6
            newPos[7] = new Vector2(thisCreature.pos.X - 1, thisCreature.pos.Y - 1); //7 
            newPos[8] = new Vector2(thisCreature.pos.X    , thisCreature.pos.Y - 1); //8     
            newPos[9] = new Vector2(thisCreature.pos.X + 1, thisCreature.pos.Y - 1); //9 
            #endregion

            #region Gather States
            while (newPos[dir].X <= 0 || newPos[dir].X >= 80 || newPos[dir].Y <= 0 || newPos[dir].Y >= 40) //While out of bounds
            {
                dir = rng.Next(1, 8);
                if (dir >= 5) dir++;
            }

            int canAttackMeleeDir = 0;
            Vector2 playerPos = currentLevel.creatures[0].pos;

            for (int y = 0; y < Level.GRIDH; y++)
                for (int x = 0; x < Level.GRIDW; x++)
                {
                    if (currentLevel.tileArray[x, y].itemList.Count > 0 &&
                        currentLevel.LineOfSight(thisCreature.pos, new Vector2(x, y)) &&
                        path.Count == 0)
                    {
                        thisCreature.targetPos = new Vector2(x, y); //Item is next target if seen
                        path = currentLevel.AStarPathfind(thisCreature, thisCreature.pos, new Vector2(x,y));
                    }
                }
            #endregion

            #region Decide on Action
            if (currentLevel.tileArray[(int)thisCreature.pos.X,
                (int)thisCreature.pos.Y].itemList.Count > 0) //If standing over an item
            {
                foreach (BodyPart b in thisCreature.anatomy)
                {
                    if (b.flags.HasFlag(BodyPartFlags.CanPickUpItem)) //If any part can pick up items
                    {
                        inventoryCheck = true; //We're picking up an item, so we need to see what we can do with it
                        return "Pick Up"; //Pick it up
                    }
                }
            }

            if (currentLevel.LineOfSight(thisCreature.pos, playerPos)) //If player is newly seen or smelled
            {
                targetPos = playerPos; //Keep the last known position in creature's memory
                canAttackMeleeDir = thisCreature.AdjacentToCreatureDir(currentLevel);

                if (currentLevel.ConvertAbsolutePosToRelative(thisCreature.pos, targetPos) > 0) //If adjacent
                {
                    return "Attack " + currentLevel.ConvertAbsolutePosToRelative(thisCreature.pos, targetPos);
                }

                path = currentLevel.AStarPathfind(thisCreature, thisCreature.pos, playerPos); //Path to player
                return "Move " + path.Pop();
            }

            if (path.Count > 0) //If there's a target
            {
                if (targetPos == thisCreature.pos) //If we're standing on it
                {
                    targetPos = new Vector2(-1,-1); //Forget this target
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
                        targetPos = new Vector2(-1,-1);
                        path.Clear();
                        return "Move " + dir; //Wander
                    }
                }
            }

            foreach (BodyPart b in thisCreature.anatomy)
            {
                foreach (Item i in thisCreature.inventory)
                {
                    if (i is Potion && (b.injury.HasFlag(InjuryLevel.Mangled) || b.injury.HasFlag(InjuryLevel.Broken))) //If the creature has a potion and is hurt badly
                    {
                        return "Eat " + thisCreature.inventory.IndexOf(i);
                    }
                }
            }

            for (int y = 0; y < Level.GRIDH; y++)
                for (int x = 0; x < Level.GRIDW; x++)
                {
                    if (currentLevel.tileArray[x, y].itemList.Count > 0)
                    {
                        if (currentLevel.LineOfSight(thisCreature.pos, new Vector2(x, y)))
                        {
                            foreach (BodyPart b in thisCreature.anatomy)
                            {
                                if (b.flags.HasFlag(BodyPartFlags.CanPickUpItem))
                                {
                                    targetPos = new Vector2(x, y);
                                    path = currentLevel.AStarPathfind(thisCreature, thisCreature.pos, playerPos); //Path to item
                                    break;
                                }
                            }
                        }
                    }
                }
            #endregion

            if (inventoryCheck)
            {
                foreach (Item i in thisCreature.inventory) //Look at all the items
                {
                    if (i is Weapon)
                    {
                        if (thisCreature.weapon == null)
                        {
                            return "Wield " + thisCreature.inventory.IndexOf(i);
                        }
                        else
                        {
                            if (i.damage.average > thisCreature.weapon.damage.average)
                            {
                                return "Unwield";
                            }
                        }
                    }

                    if (i is Armor)
                    {
                        if (thisCreature.CanWear((Armor)i))
                        {
                            return "Wear " + thisCreature.inventory.IndexOf(i);
                        }
                    }
                }

                inventoryCheck = false; //If we've checked everything and can't find a use, don't bother for a while
            }

            while (!thisCreature.CanMoveBorder(dir))
            {
                dir = rng.Next(1, 9);
                if (dir >= 5)
                    dir++;
            }
            return "Move " + dir; //Default action            
        }
        public bool ShouldBeHostileTo(int creatureNumber)
        {
            if (hostility >= 100) //If completely bloodthirsty
            {
                return true; //Always be hostile
            }

            if (creatureNumber == 0) //If the player
            {
                return true; //Always player-hostile for now.
            }

            return false; //If nothing else caught it, assume false
        }
    } //A mind, in which is held Artificial Intelligence. Or a long series of 'if' statements
}
