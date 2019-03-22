using System;
using System.Collections.Generic;
using System.Linq;
using KalaGame;

namespace Adventurer
{
    public class Level
    {
        #region Variables
        public const byte GRIDW = 56;
        public const byte GRIDH = 33;
		public const byte ROOMTEMPERATURE = 25;

        public ContentEncyclopedia content;
        public Creature player;
        public List<Creature> creatures = new List<Creature>();
        public Tile[,] tileArray = new Tile[GRIDW, GRIDH]; //X,Y; used for storing base tile information
        public int roomCount, corridorCount, creatureCount, itemCount, levelNumber, doorCount, seed;
        public Point3D mapPos;
        public bool playerDiedHere;
        public bool genError = false;
        public string levelType, causeOfDeath, mannerOfDeath;
        public Random rng = new Random();
        public Dice rngDie = new Dice();
        public List<Room> rooms = new List<Room>();
        public List<Corridor> corridors = new List<Corridor>();
        #endregion

        public Level(int rCount, int cCount, String type, ContentEncyclopedia content, 
            int rngSeed, Point3D mapPos)
        {
            seed = rngSeed;
            causeOfDeath = "unknown causes."; //If this never gets tripped, then wtf
            mannerOfDeath = "mysterious circumstances - will they never find a cure?"; //Ditto
            this.mapPos = mapPos;
            levelType = type;
            rng = new Random(rngSeed);
            rngDie = new Dice(rngSeed);
            this.content = content;
            int itemCount = rng.Next(5, 11); //5 to 10 items per level
            int creatureCount = rng.Next(5, 11); //5 to 10 initial creatures
            int x, y;

			switch(type)
			{
            case "dungeon":
                FillWithFreshTiles(false);
                MakeEmptyRooms();
				MakeCorridors();

                #region Place other fixtures
                bool doneFixtures = false; //Set up a stop condition for the next loop
                while (!doneFixtures) //A loop to place the down stairs randomly in an open space
                {
                    int r = rng.Next(0, rooms.Count); //Random major room
                    while (rooms[r].isIsolated) //Make sure we don't put stuff in isolated rooms
                        r = rng.Next(0, rooms.Count);

                    x = rng.Next(rooms[r].x + 1, rooms[r].x + rooms[r].width - 1);
                    y = rng.Next(rooms[r].y + 1, rooms[r].y + rooms[r].height - 1); //Pick a random spot in room

                    if (tileArray[x, y].isPassable && tileArray[x, y].fixtureLibrary.Count <= 0)
                    {
                        tileArray[x, y].fixtureLibrary.Add(new Stairs(true));

                        doneFixtures = true;
                    }
                }

                doneFixtures = false; //Reset
                for (int k = 0; k < 5; k++)
                {
                    x = rng.Next(3, GRIDW - 2);
                    y = rng.Next(3, GRIDH - 2); //Pick a random spot

                    if (tileArray[x, y].isPassable && tileArray[x, y].fixtureLibrary.Count <= 0) //If it's walkable
                    {
                        Trap thisTrap = new Trap(new Effect(rngDie.Roll(5), "tripwire"));
                        //thisTrap.visible = true; //For now, let's see where these things go
                        tileArray[x, y].fixtureLibrary.Add(thisTrap); //Place a tripwire here
                    }
                    else
                    {
                        k--;
                    }
                }
                #endregion

                PlacePlayer();

                rng = new Random((int)DateTime.Now.Ticks); //Reseed the level so the other things will change

                for (int i = 1; i <= creatureCount; i++)
                {
                    SpawnCreature(true, "monster");
                }

                #region Place the items
                for (int i = 1; i <= itemCount; i++)
                {
                    bool doneItems = false; //Set up a stop condition for the next loop
                    while (doneItems == false) //A loop to place the creature randomly in an open space
                    {
                        int r = rng.Next(0, rooms.Count); //Random major room
                        while (rooms[r].isIsolated)
                            r = rng.Next(0, rooms.Count);

                        x = rng.Next(rooms[r].x + 1, rooms[r].x + rooms[r].width - 1);
                        y = rng.Next(rooms[r].y + 1, rooms[r].y + rooms[r].height - 1); //Pick a random spot in room

                        if (tileArray[x, y].isPassable == true) //If it's passable
                        {
                            List<Armor> armorList = new List<Armor>();
                            List<Potion> potionList = new List<Potion>();
                            List<Weapon> weaponList = new List<Weapon>();
                            List<Item> plainList = new List<Item>();

                            foreach (Item t in content.items) //Organze the items for random drops TODO: Is this necessary? Linq?
                            {
                                if (t is Armor)
                                    armorList.Add((Armor)t);
                                else if (t is Potion)
                                    potionList.Add((Potion)t);
                                else if (t is Weapon)
                                    weaponList.Add((Weapon)t);
                                else
                                {
                                    if (ROOMTEMPERATURE > t.material.meltPoint) //Ugh, liquid
                                    {
                                        plainList.Add(t);
                                    }
								}
                            }

                            Item thisItem;

                            if (rng.Next(0, 100) < 50) //50% chance
                            {
                                thisItem = plainList[(int)rng.Next(0, plainList.Count)]; //Random other item
                            }
                            else if (rng.Next(0, 100) < 30) //30% chance of 70%
                            {
                                thisItem = armorList[(int)rng.Next(0, armorList.Count)]; //Random armor
                            }
                            else if (rng.Next(0, 100) < 30) //30% chance of 70% of 70%
                            {
                                thisItem = weaponList[(int)rng.Next(0, weaponList.Count)]; //Random weapon
                            }
                            else
                            {
                                thisItem = potionList[(int)rng.Next(0, potionList.Count)]; //Random potion                            
                            }
							
                            tileArray[x, y].itemList.Add(thisItem); //Add a item to the Level's item list

                            armorList.Clear();
                            potionList.Clear();
                            weaponList.Clear();
                            doneItems = true;
                        }
                    }
                }
                #endregion
				break;
				
			case "forest":		            
                FillWithFreshTiles(true);
				FillWithTrees();
                PlacePlayer();

                rng = new Random((int)DateTime.Now.Ticks); //Reseed the level so the other things will change

                creatureCount = rng.Next(1, 5); //1 to 4 initial creatures

                for (int i = 1; i <= creatureCount; i++)
                {
                    SpawnCreature(false, "monster");
                }
				break;
				
			case "village":
                FillWithFreshTiles(true);
                FillWithTrees();
                MakeEmptyRooms();
				MakeShop();
				MakeCorridors();
                PlacePlayer();

                #region Place the villagers
                creatureCount = roomCount; //1 for every room

                for (int i = 1; i <= creatureCount; i++)
                {
                    SpawnCreature(true, "quest giver");
                }
                #endregion
				break;
            }
        }
		
		protected void FillWithFreshTiles(bool open)
		{
			int x,y;
		    for (y = 0; y < GRIDH; y++) //Repeat gridSizeY lines...
                for (x = 0; x < GRIDW; x++) //...by gridSizeX lines...
                {
                    tileArray[x, y] = null; //Set all tiles as nothing
                }

			
            for (y = 0; y < GRIDH; y++) //Repeat gridSizeY lines...
			{
                for (x = 0; x < GRIDW; x++) //...by gridSizeX lines...
                {
					if (open)
					{
	                    tileArray[x, y] = new Tile(true, Adventurer.air, 2, true, true); //Set all tiles as roomable solid rock	                    
	                    tileArray[x, y].isWall = false;			
					}
					else
					{
						tileArray[x, y] = new Tile(false, Adventurer.rock, 2, true, false); //Set all tiles as roomable solid rock	                    
	                    tileArray[x, y].isWall = true;	
					}
					tileArray[x, y].isRoomEdge = false;
                }
			}
		}
		protected void FillWithTrees()
		{
		    for (int k = 0; k < GRIDH * GRIDW / 6; k++)
            {
                int x = rng.Next(1, GRIDW);
                int y = rng.Next(1, GRIDH); //Pick a random spot

                if (tileArray[x, y].isPassable && tileArray[x,y].fixtureLibrary.Count <= 0) //If it's walkable
                {
                    Tree thisTree = new Tree(rng);
                    tileArray[x, y].isPassable = false;
                    tileArray[x, y].isTransparent = false;
                    
                    tileArray[x, y].fixtureLibrary.Add(thisTree); //Place a tree here
                }
                else
                {
                    k--;
                }
            }
		}
		protected void MakeEmptyRooms()
		{
		    int x = rng.Next(3, GRIDW-10); //Stay within the level bounds
            int y = rng.Next(3, GRIDH-10); //Stay within the level bounds
            int width = rng.Next(3, 9);
            int height = rng.Next(3, 9);
            int i = 1;
            int roomCount = rng.Next(7,10);
            Room room = new Room(i, x, y, width, height);
            rooms.Add(room);
            DigRoom(room, tileArray); //First room

            while (rooms.Count < roomCount) //While we need more rooms
            {
                x = rng.Next(3, GRIDW - 10); //Stay within the level bounds
                y = rng.Next(3, GRIDH - 10); //Stay within the level bounds
                width = rng.Next(5, 9);
                height = rng.Next(5, 9);
                room = new Room(i, x, y, width, height);

                if (CanDigRoom(room, tileArray))
                {
                    {
                        rooms.Add(room);
                        DigRoom(room, tileArray);
                    }
                }
            }
		}
		protected void MakeShop()
		{
			int x = rng.Next(3, GRIDW-10); //Stay within the level bounds
            int y = rng.Next(3, GRIDH-10); //Stay within the level bounds
            int width = rng.Next(3, 9);
            int height = rng.Next(3, 9);
		}
		protected void MakeCorridors()
		{
			int iterate = 1;

            foreach (Room a in rooms)
            {
                foreach (Room b in rooms)
                {
                    if (a != b)
                    {
                        Point2D sideA = new Point2D(rng.Next(a.x + 1, a.x + a.width - 1), a.y + a.height + 1);
                        Point2D sideB = new Point2D(rng.Next(b.x + 1, b.x + b.width - 1), b.y - 1);
                        Point2D midPointA, midPointB;
                        bool horizontal = false;

                        if (rng.Next(0, 2) == 1) //50% chance
                        {
                            sideA = new Point2D(a.x + a.width + 1, rng.Next(a.y + 1, a.y + a.height - 1));
                            sideB = new Point2D(b.x - 1, rng.Next(b.y + 1, b.y + b.height - 1));
                            horizontal = true;
                        }

                        if (sideA.X + sideB.X > sideA.Y + sideB.Y)
                        {
                            int xAvg = (sideA.X + sideB.X) / 2;
                            midPointA = new Point2D(xAvg, sideA.Y);
                            midPointB = new Point2D(xAvg, sideB.Y); //For right angles
                        }
                        else
                        {
                            int yAvg = (sideA.Y + sideB.Y) / 2;
                            midPointA = new Point2D(sideA.X, yAvg);
                            midPointB = new Point2D(sideB.X, yAvg); //For right angles
                        }

                        if (CanDigCorridor(sideA, midPointA, tileArray) &&
                            CanDigCorridor(midPointA, midPointB, tileArray) &&
                            CanDigCorridor(midPointB, sideB, tileArray)) //Right angles
                        {
                            corridors.Add(new Corridor(sideA, sideB));
                            DigCorridor(sideA, midPointA);
                            DigCorridor(midPointA, midPointB);
                            DigCorridor(midPointB, sideB);

                            Point2D doorPos = new Point2D(sideA.X, sideA.Y - 1);

                            if (horizontal)
                                doorPos = new Point2D(sideA.X - 1, sideA.Y);

                            if (tileArray[doorPos.X, doorPos.Y].fixtureLibrary.Count < 1)
                            {
                                DigDoor(doorPos.X, doorPos.Y);

                                tileArray[doorPos.X, doorPos.Y].fixtureLibrary.Add(new Door(
                                    tileArray[doorPos.X, doorPos.Y], false));

                                tileArray[doorPos.X, doorPos.Y].isPassable = false;
                                tileArray[doorPos.X, doorPos.Y].isTransparent = false;
                                tileArray[doorPos.X, doorPos.Y].isDoor = true;
                                a.doorCount++;
                            }

                            doorPos = new Point2D(sideB.X, sideB.Y + 1);

                            if (tileArray[doorPos.X, doorPos.Y].fixtureLibrary.Count < 1)
                            {
                                if (horizontal)
                                    doorPos = new Point2D(sideB.X + 1, sideB.Y);

                                DigDoor(doorPos.X, doorPos.Y);

                                tileArray[doorPos.X, doorPos.Y].fixtureLibrary.Add(new Door(
                                    tileArray[doorPos.X, doorPos.Y], false));
                                tileArray[doorPos.X, doorPos.Y].isPassable = false;
                                tileArray[doorPos.X, doorPos.Y].isTransparent = false;
                                tileArray[doorPos.X, doorPos.Y].isDoor = true;
                                b.doorCount++;
                            }
                        }
                    }
                }
            }

            foreach (Room c in rooms)
            {
                while (c.doorCount <= 0 && iterate <= 9000) //Keep trying for a while if not all rooms have doors
                {
                    foreach (Room b in rooms)
                    {
                        if (c != b)
                        {
                            Point2D sideA = new Point2D(rng.Next(c.x + 1, c.x + c.width - 1), c.y + c.height + 1);
                            Point2D sideB = new Point2D(rng.Next(b.x + 1, b.x + b.width - 1), b.y - 1);
                            Point2D midPointA, midPointB;

                            if (sideA.X + sideB.X > sideA.Y + sideB.Y)
                            {
                                int xAvg = (sideA.X + sideB.X) / 2;
                                midPointA = new Point2D(xAvg, sideA.Y);
                                midPointB = new Point2D(xAvg, sideB.Y); //For right angles
                            }
                            else
                            {
                                int yAvg = (sideA.Y + sideB.Y) / 2;
                                midPointA = new Point2D(sideA.X, yAvg);
                                midPointB = new Point2D(sideB.X, yAvg); //For right angles
                            }

                            if (CanDigCorridor(sideA, midPointA, tileArray) &&
                                CanDigCorridor(midPointA, midPointB, tileArray) &&
                                CanDigCorridor(midPointB, sideB, tileArray)) //Right angles
                            {
                                corridors.Add(new Corridor(sideA, sideB));
                                DigCorridor(sideA, midPointA);
                                DigCorridor(midPointA, midPointB);
                                DigCorridor(midPointB, sideB);

                                DigDoor((int)sideA.X, (int)sideA.Y - 1);
                                tileArray[(int)sideA.X, (int)sideA.Y - 1].fixtureLibrary.Add(new Door(
                                    tileArray[(int)sideA.X, (int)sideA.Y - 1], true));

                                tileArray[(int)sideA.X, (int)sideA.Y - 1].isPassable = false;
                                tileArray[(int)sideA.X, (int)sideA.Y - 1].isTransparent = false;
                                tileArray[(int)sideA.X, (int)sideA.Y - 1].isDoor = true;
                                c.doorCount++;
                                DigDoor((int)sideB.X, (int)sideB.Y + 1);
                                tileArray[(int)sideB.X, (int)sideB.Y + 1].fixtureLibrary.Add(new Door(
                                    tileArray[(int)sideB.X, (int)sideB.Y + 1], true));
                                tileArray[(int)sideB.X, (int)sideB.Y + 1].isPassable = false;
                                tileArray[(int)sideB.X, (int)sideB.Y + 1].isTransparent = false;
                                tileArray[(int)sideB.X, (int)sideB.Y + 1].isDoor = true;
                                b.doorCount++;
                            }
                        }
                    }                        

                    iterate++;
                }
                if (iterate > 9000)
                {
                    throw new ApplicationException("New level has isolated room");
                }
            }
		}
		protected void PlacePlayer()
		{
		    bool donePlayer = false; //Set up a stop condition for the next loop
            while (donePlayer == false) //A loop to place the player creature randomly in an open space
            {
                int x = rng.Next(3, GRIDW - 2);
                int y = rng.Next(3, GRIDH - 2); //Pick a random spot in level

                if (tileArray[x, y].isPassable && tileArray[x, y].fixtureLibrary.Count <= 0)
                {
                    Species c = content.bestiary.First(); //TODO: Select humanoid quest giver?
                    Creature thisCreature = c.GenerateCreature("quest giver", content.items.ToList(), rng.Next()); //If I don't do this, any time I change thisCreature it changes every creature in the list
                    thisCreature.pos = new Point2D(x, y);
                    thisCreature.isPlayer = true; //This is the player
                    if (this.levelType != "village" && this.levelType != "forest") tileArray[x, y].fixtureLibrary.Add(new Stairs(false)); //Add up stairs where the player starts
                    for (y = 0; y < GRIDH; y++)
                        for (x = 0; x < GRIDW; x++)
                        {
                            tileArray[x, y].scentIdentifier.Add(thisCreature.name); //Keep track of this creature's scent now
                            tileArray[x, y].scentMagnitude.Add(0); //Start it at zero scent in the room
                        }

                    if (thisCreature is QuestGiver)
                    {
                        this.creatures.Add(new QuestGiver((QuestGiver)thisCreature));
                    }
                    else
                    {
                        this.creatures.Add(new Creature(thisCreature));
                    }

                    donePlayer = true;
                }
            }
		}

        public Stack<byte> AStarPathfind(Creature creature, Point2D pointA, Point2D pointB)
        {
            Stack<byte> path = new Stack<byte>();

            if (ConvertAbsolutePosToRelative(pointA, pointB) > 0)
            {
                path.Push((byte)ConvertAbsolutePosToRelative(pointA, pointB));
                return path;
            }

            if (pointA.X == pointB.X && pointA.Y == pointB.Y)
            {
                path.Push(5);
                return path;
            }

            #region Variables
            //int g = 0;
            //int h = (int)(Math.Abs(pointB.X - pointA.X) + (int)Math.Abs(pointB.Y - pointA.Y));
            bool done = false;
            AStarTile currentTile = new AStarTile(pointA, new Point2D(-1, -1), 0,
                100 * ((int)(Math.Abs(pointB.X - pointA.X) + (int)Math.Abs(pointB.Y - pointA.Y))));
            List<AStarTile> openList = new List<AStarTile>();
            List<AStarTile> closedList = new List<AStarTile>();
            #endregion

            openList.Add(currentTile); //Add the current tile

            while (!done)
            {
                #region Find new current tile
                currentTile = openList[0]; //The lowest F cost tile is the current tile
                openList.RemoveAt(0); //Removed from the open list
                closedList.Add(currentTile); //And placed in the closed list
                #endregion

                if (currentTile.pos == pointB) //If target has been just added to closed list
                {
                    done = true;
                }

                #region Define adjacent vectors
                Point2D[] adjacent = new Point2D[8];
                adjacent[0] = new Point2D(currentTile.pos.X - 1, currentTile.pos.Y + 1); //Adjacent tile at position 1
                adjacent[1] = new Point2D(currentTile.pos.X, currentTile.pos.Y + 1);     //Adjacent tile at position 2
                adjacent[2] = new Point2D(currentTile.pos.X + 1, currentTile.pos.Y + 1); //Adjacent tile at position 3
                adjacent[3] = new Point2D(currentTile.pos.X - 1, currentTile.pos.Y);     //Adjacent tile at position 4
                adjacent[4] = new Point2D(currentTile.pos.X + 1, currentTile.pos.Y);     //Adjacent tile at position 6
                adjacent[5] = new Point2D(currentTile.pos.X - 1, currentTile.pos.Y - 1); //Adjacent tile at position 7
                adjacent[6] = new Point2D(currentTile.pos.X, currentTile.pos.Y - 1);     //Adjacent tile at position 8
                adjacent[7] = new Point2D(currentTile.pos.X + 1, currentTile.pos.Y - 1); //Adjacent tile at position 9
                #endregion

                #region Process adjacent tiles to Current Tile
                for (int i = 0; i <= 7; i++) // the adjacent tiles 0-7, all 8
                {
                    if (adjacent[i].X < 0 || adjacent[i].X > GRIDW-1 || adjacent[i].Y < 0 || adjacent[i].Y > GRIDH-1)
                    {
                        i++;
                        break;
                    }

                    bool shouldBeOpen = true;
                    bool isAlreadyOpen = false;
                    bool closedDoor = false; //Whether this tile has a door in it.
                    int indexOnOpen = -1;

                    //if (tileArray[(int)adjacent[i].X, (int)adjacent[i].Y].fixtureLibrary.Count > 0)
                    //{
                    //    foreach (Fixture fixture in tileArray[(int)adjacent[i].X, (int)adjacent[i].Y].fixtureLibrary)
                    //    {
                    //        if (fixture is Door) //If it's a door
                    //        {
                    //            Door door = (Door)fixture;
                    //            if (!door.isOpen) //If the door is closed
                    //                closedDoor = true;
                    //        }
                    //    }
                    //}

                    if (tileArray[(int)adjacent[i].X, (int)adjacent[i].Y] == null)
                    {
                        shouldBeOpen = false;                        
                    }
                    else if (this.tileArray[(int)adjacent[i].X, (int)adjacent[i].Y].isPassable == false &&
                        !(closedDoor && creature.isDextrous)) //If not passable
                    {
                        shouldBeOpen = false; //It's not a candidate if not blocked by door
                    }
                    else //If passable
                    {
                        foreach (AStarTile j in closedList) //For each closedList item
                        {
                            if (j.pos == adjacent[i]) //If a closedList item is the same as the potential tile
                            {
                                shouldBeOpen = false; //Not a candidate
                            }
                        }

                        foreach (Creature c in creatures)
                            if (c.pos == adjacent[i] && adjacent[i] != pointB) //If a creature is in the way
                                shouldBeOpen = false;

                        int openListCount = openList.Count;
                        for (int j = 0; j < openListCount; j++)
                        {
                            if (openList[j].pos == adjacent[i]) //If an openList item is the same as the potential tile
                            {
                                isAlreadyOpen = true; //Mark it as already on the open list
                                indexOnOpen = j;
                            }
                        }
                    }

                    if (shouldBeOpen) //If it should go on the open list
                    {
                        int g = 100; //Weight of tile ***IMPROVE THIS LATER***
                        int h = 100 * ((int)(Math.Abs(pointB.X - adjacent[i].X) + (int)Math.Abs(pointB.Y - adjacent[i].Y))); //Heuristic guess
                        //int f = g + h; //Definition of f cost
                        AStarTile newTile = new AStarTile(adjacent[i], currentTile.pos, currentTile.g + g, h, g); //Make an official tile for it

                        if (isAlreadyOpen) //If already on the open list
                        {
                            if (newTile.g < openList[indexOnOpen].g) //If this path to the tile is cheaper than its previous g 
                            {
                                openList[indexOnOpen] = newTile; //Overwrite its old info with this cheaper info
                            }
                        }

                        else
                        {
                            openList.Insert(AStarSequentialSearch(openList, newTile), newTile); //Insert in openList a sequential search according to f cost

                            //Calculate where this new tile should go with a binary search. ***UNFINISHED***
                            //AStarBinarySearch(openList, newTile);
                        }
                    }
                }
                if (openList.Count == 0) //If no more tiles to look at, there's no path
                {
                    path.Push(0);
                    return path;
                }
                #endregion
            }

            #region Trace back to starting tile
            int oldIndex = -1;
            int index = 0;

            while (currentTile.parentpos != new Point2D(-1, -1)) //Until parent is starting tile
            {
                int closedListCount = closedList.Count;
                for (int j = 0; j < closedListCount; j++)
                {
                    if (closedList[j].pos == currentTile.parentpos)
                    {
                        oldIndex = index;
                        index = j;
                        path.Push((byte)ConvertAbsolutePosToRelative(closedList[index].pos, currentTile.pos));
                        break;
                    }
                }
                
                currentTile = closedList[index];
            }
            #endregion

            currentTile = closedList[oldIndex];

            openList.Clear();
            closedList.Clear();
            return path;
        }
        public static int AStarSequentialSearch(List<AStarTile> openList, AStarTile tileToInsert)
        {
            int max = openList.Count - 1;

            if (openList.Count == 0) //If the list is empty
            {
                return 0; //Insert at zero, since nothing's there to mess up
            }

            if (openList.Count == 1)
            {
                if (tileToInsert.f < openList[0].f)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            for (int i = 0; i <= max; i++)
            {
                if (i == max) //If at the far right of the list
                {
                    if (tileToInsert.f >= openList[i].f) //newtile.f > farrightnumber.f
                    {
                        return i + 1; //Insert it just to the right of the max number, bumping nothing
                    }
                    else
                    {
                        return i;
                    }
                }
                else if (i == 0) //If at the far left of the list
                {
                    if (tileToInsert.f <= openList[i].f) //newtile.f < farleftnumber.f
                    {
                        return 0; //Insert it at index zero, bumping everything to the right one
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (openList[i - 1].f < tileToInsert.f && tileToInsert.f <= openList[i].f) //If between two
                    {
                        return i; //Bump everything it's less than to the right.
                    }
                }
            }
            return -1; //Error if it makes it here
        }
        public bool CanDigRoom(Room room, Tile[,] tileArray)
        {
            for (int y = room.y; y <= room.y + room.height; y++) //For every tile in room's height...
                for (int x = room.x; x <= room.x + room.width; x++) //...and width.
                {
                    if (tileArray[x, y].isCorridorEdge || tileArray[x, y].adjacentToRoomN > 0)
                        return false;
                }

            return true;
        }
        public bool CanDigCorridor(Point2D pointA, Point2D pointB, Tile[,] tileArray)
        {
            #region Variables
            int xa = (int)pointA.X;
            int xb = (int)pointB.X;
            int ya = (int)pointA.Y;
            int yb = (int)pointB.Y;

            //Error-catching
            if (xa < 1 || xa > GRIDW || xb < 1 || xb > GRIDW || ya < 1 || ya > GRIDH || yb < 1 || yb > GRIDH)
                return false;

            bool steep = (Math.Abs(yb - ya) > Math.Abs(xb - xa));
            #endregion

            #region If Steep
            if (steep)
            {
                //swap(xa, ya)
                int ca = xa;
                xa = ya;
                ya = ca;

                //swap(x1, y1)
                int cb = xb;
                xb = yb;
                yb = cb;
            }
            #endregion

            #region If xa > xb
            if (xa > xb)
            {
                //swap(x0, x1)
                int xc = xa;
                xa = xb;
                xb = xc;

                //swap(y0, y1)
                int yc = ya;
                ya = yb;
                yb = yc;
            }
            #endregion

            #region More variable tomfoolery
            int deltax = xb - xa;
            int deltay = Math.Abs(yb - ya);
            int error = deltax / 2;
            int ystep;
            int y = ya;
            #endregion

            #region If/else ya < yb
            if (ya < yb)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }
            #endregion

            #region For x from xa to xb
            for (int x = xa; x <= xb; x++) //for x from x0 to x1
            {
                if (steep)
                {
                    //plot (y,x)
                    //for (int adjY = y - 1; adjY <= y + 1; adjY++)
                    //    for (int adjX = x - 1; adjX <= x + 1; adjX++)
                    //        if (tileArray[adjY, adjX].isCorridorEdge)
                    //          return false;

                    if (!(tileArray[y, x - 1].isPassable && tileArray[y, x + 1].isPassable)) // If we've haven't hit a horizontal corridor
                    {
                        if ((tileArray[y - 1, x - 1].isPassable || tileArray[y + 1, x - 1].isPassable) &&
                            (tileArray[y - 1, x + 1].isPassable || tileArray[y + 1, x + 1].isPassable)) // If we're adjacent to a vertical corridor
                        {
                            return false;
                        }
                    }

                    if (tileArray[y, x].adjacentToRoomN > 0)
                        return false;
                }
                else
                {
                    //plot(x,y)
                    //for (int adjY = y - 1; adjY <= y + 1; adjY++)
                    //    for (int adjX = x - 1; adjX <= x + 1; adjX++)
                    //        if (tileArray[adjX, adjY].isCorridorEdge)
                    //            return false;

                    if (!(tileArray[x - 1, y].isPassable && tileArray[x + 1, y].isPassable)) // If we've haven't hit a horizontal corridor
                    {
                        if ((tileArray[x - 1, y - 1].isPassable || tileArray[x - 1, y + 1].isPassable) &&
                            (tileArray[x + 1, y - 1].isPassable || tileArray[x + 1, y + 1].isPassable)) // If we're adjacent to a vertical corridor
                        {
                            return false;
                        }
                    }

                    if (tileArray[x, y].adjacentToRoomN > 0)
                        return false;
                }

                error = error - deltay;

                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
            #endregion

            return true;
        }
        public byte ConvertAbsolutePosToRelative(Point2D pointA, Point2D pointB)
        {
            Point2D[] absPos = new Point2D[10];
            absPos[1] = new Point2D(pointA.X - 1, pointA.Y + 1);
            absPos[2] = new Point2D(pointA.X, pointA.Y + 1);
            absPos[3] = new Point2D(pointA.X + 1, pointA.Y + 1);
            absPos[4] = new Point2D(pointA.X - 1, pointA.Y);
            absPos[6] = new Point2D(pointA.X + 1, pointA.Y);
            absPos[7] = new Point2D(pointA.X - 1, pointA.Y - 1);
            absPos[8] = new Point2D(pointA.X, pointA.Y - 1);
            absPos[9] = new Point2D(pointA.X + 1, pointA.Y - 1);

            for (byte i = 1; i <= 9; i++)
            {
                if (i == 5)
                    i++; //Skip position 5

                if (pointB == absPos[i]) //If pointB matches one of the adjacent squares
                    return i; //Return the relative direction of said adjacent square
            }

            return 0; //Error if it reaches here

            #region Obsolete code
            //if (pointA.X - pointB.X == 1 && pointA.Y - pointB.Y == -1) //If position 1
            //{
            //    return 1;
            //}
            //else if (pointA.X - pointB.X == 0 && pointA.Y - pointB.Y == -1) //If position 2
            //{
            //    return 2;
            //}
            //else if (pointA.X - pointB.X == -1 && pointA.Y - pointB.Y == -1) //If position 3
            //{
            //    return 3;
            //}
            //else if (pointA.X - pointB.X == 1 && pointA.Y - pointB.Y == 0) //If position 4
            //{
            //    return 4;
            //}
            //else if (pointA.X - pointB.X == -1 && pointA.Y - pointB.Y == 0) //If position 6
            //{
            //    return 6;
            //}
            //else if (pointA.X - pointB.X == 1 && pointA.Y - pointB.Y == 1) //If position 7
            //{
            //    return 7;
            //}
            //else if (pointA.X - pointB.X == 0 && pointA.Y - pointB.Y == 1) //If position 8
            //{
            //    return 8;
            //}
            //else if (pointA.X - pointB.X == -1 && pointA.Y - pointB.Y == 1) //If position 9
            //{
            //    return 9;
            //}
            //else
            //{
            //    return -1; //Error if none of these
            //}
            #endregion
        }
        public void DigCorridor(Point2D pointA, Point2D pointB)
        {
            #region Variables
            int xa = (int)pointA.X;
            int xb = (int)pointB.X;
            int ya = (int)pointA.Y;
            int yb = (int)pointB.Y;

            //Error-catching
            if (xa < 1 || xa > GRIDW || xb < 1 || xb > GRIDW || ya < 1 || ya > GRIDH || yb < 1 || yb > GRIDH)
                return;

            bool steep = (Math.Abs(yb - ya) > Math.Abs(xb - xa));
            #endregion

            #region If Steep
            if (steep)
            {
                //swap(xa, ya)
                int ca = xa;
                xa = ya;
                ya = ca;

                //swap(x1, y1)
                int cb = xb;
                xb = yb;
                yb = cb;
            }
            #endregion

            #region If xa > xb
            if (xa > xb)
            {
                //swap(x0, x1)
                int xc = xa;
                xa = xb;
                xb = xc;

                //swap(y0, y1)
                int yc = ya;
                ya = yb;
                yb = yc;
            }
            #endregion

            #region More variable tomfoolery
            int deltax = xb - xa;
            int deltay = Math.Abs(yb - ya);
            int error = deltax / 2;
            int ystep;
            int y = ya;
            #endregion

            #region If/else ya < yb
            if (ya < yb)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }
            #endregion

            #region For x from xa to xb
            for (int x = xa; x <= xb; x++) //for x from x0 to x1
            {
                if (steep)
                {
                    //plot(y,x)
                    tileArray[y, x].fixtureLibrary.Clear();
                    tileArray[y, x].tileImage = 46;
                    tileArray[y, x].isRoomable = false;
                    tileArray[y, x].isPassable = true;
                    tileArray[y, x].material = Adventurer.air;
                    tileArray[y, x].isTransparent = true;
                    tileArray[y, x].isWall = false;
                    tileArray[y + 1, x].isCorridorEdge = true;
                    tileArray[y - 1, x].isCorridorEdge = true;
                    tileArray[y, x + 1].isCorridorEdge = true;
                    tileArray[y, x + 1].isCorridorEdge = true;
                    tileArray[y + 1, x + 1].isCorridorEdge = true;
                    tileArray[y - 1, x + 1].isCorridorEdge = true;
                    tileArray[y + 1, x - 1].isCorridorEdge = true;
                    tileArray[y - 1, x - 1].isCorridorEdge = true;
                }
                else
                {
                    //plot(x,y)
                    tileArray[x, y].fixtureLibrary.Clear();
                    tileArray[x, y].tileImage = 46;
                    tileArray[x, y].isRoomable = false;
                    tileArray[x, y].isPassable = true;
                    tileArray[x, y].material = Adventurer.air;
                    tileArray[x, y].isTransparent = true;
                    tileArray[x, y].isWall = false;
                    tileArray[x + 1, y].isCorridorEdge = true;
                    tileArray[x - 1, y].isCorridorEdge = true;
                    tileArray[x, y + 1].isCorridorEdge = true;
                    tileArray[x, y + 1].isCorridorEdge = true;
                    tileArray[x + 1, y + 1].isCorridorEdge = true;
                    tileArray[x - 1, y + 1].isCorridorEdge = true;
                    tileArray[x + 1, y - 1].isCorridorEdge = true;
                    tileArray[x - 1, y - 1].isCorridorEdge = true;
                }

                error = error - deltay;

                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
            #endregion
        }
        public bool DiggableLine(Point2D pointA, Point2D pointB)
        {
            #region Variables
            int xa = (int)pointA.X;
            int xb = (int)pointB.X;
            int ya = (int)pointA.Y;
            int yb = (int)pointB.Y;
            bool steep = (Math.Abs(yb - ya) > Math.Abs(xb - xa));
            bool shouldDig = false;
            #endregion

            #region If Steep
            if (steep)
            {
                //swap(xa, ya)
                int ca = xa;
                xa = ya;
                ya = ca;

                //swap(x1, y1)
                int cb = xb;
                xb = yb;
                yb = cb;
            }
            #endregion

            #region If xa > xb
            if (xa > xb)
            {
                //swap(x0, x1)
                int xc = xa;
                xa = xb;
                xb = xc;

                //swap(y0, y1)
                int yc = ya;
                ya = yb;
                yb = yc;
            }
            #endregion

            #region More variable tomfoolery
            int deltax = xb - xa;
            int deltay = Math.Abs(yb - ya);
            int error = deltax / 2;
            int ystep;
            int y = ya;
            #endregion

            #region If/else ya < yb
            if (ya < yb)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }
            #endregion

            #region For x from xa to xb
            for (int x = xa; x <= xb; x++) //for x from x0 to x1
            {
                if (steep)
                {
                    //plot(y,x)
                    if (tileArray[y, x].material.isDiggable)
                        shouldDig = true;
                }
                else
                {
                    //plot(x,y)
                    if (tileArray[x, y].material.isDiggable)
                        shouldDig = true;
                }

                error = error - deltay;

                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
            #endregion

            return shouldDig;
        }
        public void DigLine(Point2D pointA, Point2D pointB)
        {
            #region Variables
            int xa = (int)pointA.X;
            int xb = (int)pointB.X;
            int ya = (int)pointA.Y;
            int yb = (int)pointB.Y;

            //Error-catching
            if (xa < 1 || xa > GRIDW || xb < 1 || xb > GRIDW || ya < 1 || ya > GRIDH || yb < 1 || yb > GRIDH)
                return;

            bool steep = (Math.Abs(yb - ya) > Math.Abs(xb - xa));
            #endregion

            #region If Steep
            if (steep)
            {
                //swap(xa, ya)
                int ca = xa;
                xa = ya;
                ya = ca;

                //swap(x1, y1)
                int cb = xb;
                xb = yb;
                yb = cb;
            }
            #endregion

            #region If xa > xb
            if (xa > xb)
            {
                //swap(x0, x1)
                int xc = xa;
                xa = xb;
                xb = xc;

                //swap(y0, y1)
                int yc = ya;
                ya = yb;
                yb = yc;
            }
            #endregion

            #region More variable tomfoolery
            int deltax = xb - xa;
            int deltay = Math.Abs(yb - ya);
            int error = deltax / 2;
            int ystep;
            int y = ya;
            #endregion

            #region If/else ya < yb
            if (ya < yb)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }
            #endregion

            #region For x from xa to xb
            for (int x = xa; x <= xb; x++) //for x from x0 to x1
            {
                if (steep)
                {
                    //plot(y,x)
                    tileArray[y, x].tileImage = 46;
                    tileArray[y, x].isRoomable = false;
                    tileArray[y, x].isPassable = true;
                    tileArray[y, x].material = Adventurer.air;
                    tileArray[y, x].isTransparent = true;
                    tileArray[y, x].isWall = false;
                }
                else
                {
                    //plot(x,y)
                    tileArray[x, y].tileImage = 46;
                    tileArray[x, y].isRoomable = false;
                    tileArray[x, y].isPassable = true;
                    tileArray[x, y].material = Adventurer.air;
                    tileArray[x, y].isTransparent = true;
                    tileArray[x, y].isWall = false;
                }

                error = error - deltay;

                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
            #endregion
        }
        public void DigRoom(Room room, Tile[,] tileArray)
        {
            for (int y = room.y; y <= room.y + room.height; y++) //For every tile in room's height...
                for (int x = room.x; x <= room.x + room.width; x++) //...and width.
                {
                    if (y == room.y || y == room.y + room.height ||
                        x == room.x || x == room.x + room.width) //If edge of room
                    {
                        tileArray[x, y].adjacentToRoomN = room.roomNumber; //Remember what room this edge is next to
                        tileArray[x, y].isWall = true;
                        tileArray[x, y].isTransparent = false;
                        tileArray[x, y].isPassable = false;
                        tileArray[x, y].material = Adventurer.rock;
                        tileArray[x, y].fixtureLibrary.Clear();
                    }
                    else
                    {
                        tileArray[x, y].isPassable = true; //Is passable
                        tileArray[x, y].isWall = false; //Is not a wall
                        tileArray[x, y].material = Adventurer.air; //Air is not diggable
                        tileArray[x, y].tileImage = 46; //Is drawn as open space
                        tileArray[x, y].isTransparent = true; //Can see through it
                        tileArray[x, y].fixtureLibrary.Clear();
                    }
                }
        }
        public void DigDoor(int x, int y)
        {
            tileArray[x, y].tileImage = 46;
            tileArray[x, y].isRoomable = false;
            tileArray[x, y].isPassable = true;
            tileArray[x, y].material = Adventurer.air;
            tileArray[x, y].isTransparent = true;
            tileArray[x, y].isWall = false;
        }
        public int CountLineTiles(Point2D pntA, Point2D pntB)
        {
            Point2D pointA = pntA;
            Point2D pointB = pntB;

            #region If A is to the right of B, reverse it
            if (pointA.X > pointB.X)
            {
                Point2D pointC = pointA;
                pointA = pointB;
                pointB = pointC;
            }
            #endregion

            #region If A is to the left of B
            if (pointA.X < pointB.X) //The heart of this function.
            {
                int deltaX;
                int deltaY;

                #region Horizontal Line
                if (pointA.Y == pointB.Y) //If a horizontal line
                {
                    return (int)(pointB.X - pointA.X); //Return the X difference between the two
                }
                #endregion

                #region A is further down than B
                if (pointA.Y > pointB.Y) //If A is further down than B...
                {
                    deltaX = (int)(pointB.X - pointA.X); //B.X - A.X = deltaX
                    deltaY = (int)(pointA.Y - pointB.Y); //A.X - B.X = deltaY
                    if (deltaX > deltaY)
                    {
                        return (int)deltaX;
                    }
                    else
                    {
                        return (int)deltaY;
                    }
                }
                #endregion

                #region A is higher up than B
                if (pointA.Y < pointB.Y)
                {
                    deltaX = (int)(pointB.X - pointA.X); //B.X - A.X = deltaX
                    deltaY = (int)(pointB.Y - pointA.Y); //A.X - B.X = deltaY

                    if (deltaX > deltaY)
                    {
                        return (int)deltaX;
                    }
                    else
                    {
                        return (int)deltaY;
                    }
                }
                #endregion
            }
            #endregion

            return 0; //If math spontaneously breaks, in order to appease C#, return nothing
        }
        public bool LineOfSight(Point2D pointA, Point2D pointB)
        {
            if (pointA == pointB)            
                return true;            

            #region Variables
            int xa = (int)pointA.X;
            int xb = (int)pointB.X;
            int ya = (int)pointA.Y;
            int yb = (int)pointB.Y;

            //Error-catching
            if (xa < 1 || xa > GRIDW || xb < 1 || xb > GRIDW || ya < 1 || ya > GRIDH || yb < 1 || yb > GRIDH)
                return false;

            bool steep = (Math.Abs(yb - ya) > Math.Abs(xb - xa));
            #endregion

            #region If Steep
            if (steep)
            {
                //swap(xa, ya)
                int ca = xa;
                xa = ya;
                ya = ca;

                //swap(x1, y1)
                int cb = xb;
                xb = yb;
                yb = cb;
            }
            #endregion

            #region If xa > xb
            if (xa > xb)
            {
                //swap(x0, x1)
                int xc = xa;
                xa = xb;
                xb = xc;

                //swap(y0, y1)
                int yc = ya;
                ya = yb;
                yb = yc;
            }
            #endregion

            #region More variable tomfoolery
            int deltax = xb - xa;
            int deltay = Math.Abs(yb - ya);
            int error = deltax / 2;
            int ystep;
            int y = ya;
            #endregion

            #region If/else ya < yb
            if (ya < yb)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }
            #endregion

            #region For x from xa to xb
            for (int x = xa; x <= xb - 1; x++) //for x from x0 to x1
            {

                if (steep)
                {
                    //plot(y,x)
                    if (tileArray[y, x].isTransparent == false && (x != xa))
                    {
                        return false;
                    }
                }
                else
                {
                    //plot(y,x)
                    if (tileArray[x, y].isTransparent == false && (x != xa))
                    {
                        return false;
                    }
                }

                error = error - deltay;

                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
            #endregion

            return true;
        }
        public bool IsCreatureAt(Point2D point)
        {
            foreach (Creature i in creatures)
            {
                if (i.pos == point)
                {
                    return true;
                }
            }

            return false; //If no matches, return false
        }
        public Creature CreatureAdjacent(Creature thisCreature)
        {
            #region Array of positions
            Point2D[] newPos = new Point2D[10];
            newPos[1] = new Point2D(thisCreature.pos.X - 1, thisCreature.pos.Y + 1); //1
            newPos[2] = new Point2D(thisCreature.pos.X, thisCreature.pos.Y + 1); //2     
            newPos[3] = new Point2D(thisCreature.pos.X + 1, thisCreature.pos.Y + 1); //3
            newPos[4] = new Point2D(thisCreature.pos.X - 1, thisCreature.pos.Y);     //4 
            newPos[6] = new Point2D(thisCreature.pos.X + 1, thisCreature.pos.Y);     //6
            newPos[7] = new Point2D(thisCreature.pos.X - 1, thisCreature.pos.Y - 1); //7 
            newPos[8] = new Point2D(thisCreature.pos.X, thisCreature.pos.Y - 1); //8     
            newPos[9] = new Point2D(thisCreature.pos.X + 1, thisCreature.pos.Y - 1); //9 
            #endregion

            for (int i = 1; i <= 9; i++)
            {
                if (i == 5)
                    i++; //Skip position 5

                if (IsCreatureAt(newPos[i])) //If there's an adjacent creature
                {
                    foreach (Creature k in creatures) //Find out which creature it is
                    {
                        if (k.pos == newPos[i]) //When you get a match
                        {
                            return k; //Return it
                        }
                    }
                }
            }

            return null;
        }
        public int CreatureNAt(Point2D point)
        {
            foreach (Creature i in creatures)
            {
                if (i.pos == point)
                {
                    return creatures.IndexOf(i);
                }
            }

            return -1;
        }
        public bool MoveWillBeBlocked(int creatureNumber, Directions movedir)
        {
            return IsCreatureAt(creatures[creatureNumber].pos.AdjacentVector(movedir));
        }
		public void SlayCreature(Creature c)
		{
			if (c.revive > 0) //If extra life
			{
				c.revive = 0; //Not anymore, sukka
				if (c.amulet.effect.type == "revive")
					c.amulet = null;
				c.hp = c.hpMax; //Full heal
				c.food = 15000; //Full food
				c.message.Add("You have a near death experience");
				c.constitution--;				
			}
			else
			{
                if (creatures.IndexOf(c) == 0)
                    playerDiedHere = true;
                else
                {
                    int count = c.inventory.Count;
                    for (int i = 0; i < count; i++)
                        c.Drop(this, c.inventory[0]); //Drop all items

                    count = c.wornArmor.Count;
                    for (int i = 0; i < count; i++)
                    {
                        tileArray[c.pos.X, c.pos.Y].itemList.Add(c.wornArmor[0]); //Add item to tile 
                        c.wornArmor.Remove(c.wornArmor[0]); //From creature
                    } //Drop all armor

                    tileArray[c.pos.X, c.pos.Y].itemList.Add(c.weapon); //Add item to tile 
                    c.weapon = null; //Drop the weapon

                    Item corpse = new Item(c.mass, c.mass, c.name + " corpse", c.color, new List<Item>(), new List<string>());
                    corpse.itemImage = 253;
                    corpse.edible = true;
                    tileArray[c.pos.X, c.pos.Y].itemList.Add(new Item(corpse)); //Gibs

                    for (int y = 1; y < Level.GRIDH; y++)
                        for (int x = 1; x < Level.GRIDW; x++)
                        {
                            tileArray[x, y].scentIdentifier.RemoveAt(
                                creatures.IndexOf(c)); //Remove it from scent tracking
                            tileArray[x, y].scentMagnitude.RemoveAt(
                                creatures.IndexOf(c));
                        }

                    if (LineOfSight(creatures[0].pos, c.pos))
                        creatures[0].message.Add("The " + c.name + " dies.");

                    //killedIndex = creatureList.IndexOf(c);
                }
			}
		}
        public void SmellSpread()
        {
            Point2D[] adjacent = new Point2D[10];
            for (int y = 1; y < GRIDH - 1; y++)
                for (int x = 1; x < GRIDW - 1; x++) //For every tile (2,2) through (39,39), otherwise it goes off the edge
                {
                    adjacent[1] = new Point2D(x - 1, y + 1);
                    adjacent[2] = new Point2D(x, y + 1);
                    adjacent[3] = new Point2D(x + 1, y + 1);
                    adjacent[4] = new Point2D(x - 1, y);
                    adjacent[6] = new Point2D(x + 1, y);
                    adjacent[7] = new Point2D(x - 1, y - 1);
                    adjacent[8] = new Point2D(x, y - 1);
                    adjacent[9] = new Point2D(x + 1, y - 1);

                    for (int i = 0; i < tileArray[x, y].scentMagnitude.Count; i++) //For each specific smell
                    {
                        int totalScent = tileArray[x, y].scentMagnitude[i];
                        int divideTotal = 1;
                        for (int k = 1; k <= 9; k++)
                        {
                            if (k == 5)
                                k++; //Skip position 5

                            if (tileArray[(int)adjacent[k].X, (int)adjacent[k].Y].isPassable)
                            {
                                totalScent += tileArray[(int)adjacent[k].X, (int)adjacent[k].Y].scentMagnitude[i];
                                divideTotal++;
                            }
                        }

                        tileArray[x, y].scentMagnitude[i] = (totalScent / divideTotal) - 1; //This one's scent is the average of all adjacent tiles, minus scent decay

                        for (int k = 1; k < creatures.Count; k++)
                        {
                            tileArray[x, y].scentIdentifier[k] = creatures[k].name;
                        }
                    }
                }
        }
        public void SpawnCreature(bool roomed, string social)
        {
            if (content.bestiary.Count() == 0)
                throw new Exception("Attempted to spawn creature with no species in this level's bestiary.");

            bool doneCreature = false; //Set up a stop condition for the next loop
            while (doneCreature == false) //A loop to place the player creature randomly in an open space
            {
                int r = 1;
                int x = 1;
                int y = 1;

                if (roomed)
                {
                    r = rng.Next(0, rooms.Count); //Random major room  
                    while (rooms[r].isIsolated)
                        r = rng.Next(0, rooms.Count);
                    x = rng.Next(rooms[r].x + 1, rooms[r].x + rooms[r].width - 1);
                    y = rng.Next(rooms[r].y + 1, rooms[r].y + rooms[r].height - 1); //Pick a random spot in room
                }
                else
                {
                    x = rng.Next(1, GRIDW);
                    y = rng.Next(1, GRIDH);
                }
                bool place = true;

                foreach (Creature c in creatures)
                    if (c.pos == new Point2D(x, y))
                        place = false;

                if (!tileArray[x, y].isPassable)
                    place = false;

                if (place)
                {
                    Species c = content.bestiary.First(); //Default
                    if (social == "monster")
                    {
                        c = content.bestiary.ToList()[rng.Next(0, content.bestiary.Count())];
                    }
                    else
                    {
                        foreach (Species cG in content.bestiary)
                        {
                            if (cG.name == "human")
                                c = cG;
                        }
                    }

                    Creature thisCreature = c.GenerateCreature(social, content.items.ToList(), rng.Next());
                    thisCreature.pos = new Point2D(x, y);
                    for (y = 0; y < GRIDH; y++)
                        for (x = 0; x < GRIDW; x++)
                        {
                            tileArray[x, y].scentIdentifier.Add(thisCreature.name); //Keep track of this creature's scent now
                            tileArray[x, y].scentMagnitude.Add(0); //Start it at zero scent in the room
                        }

                    if (thisCreature is QuestGiver)
                    {
                        creatures.Add(new QuestGiver((QuestGiver)thisCreature));
                    }
                    else
                    {
                        creatures.Add(new Creature(thisCreature));
                    }
                    //creatureList[i].pos = new Vector2(x,y); //Place the creature's position there

                    doneCreature = true;
                }
            }
        }
        public void SpawnCreature(Creature c, Point2D pos)
        {
            c.pos = pos;

            for (int y = 0; y < GRIDH; y++)
                for (int x = 0; x < GRIDW; x++)
                {
                    tileArray[x, y].scentIdentifier.Add(c.name); //Keep track of this creature's scent now
                    tileArray[x, y].scentMagnitude.Add(0); //Start it at zero scent in the room
                }
            creatures.Add(c);
        }
        public void TempuratureSpread()
        {
            Point2D[] adjacent = new Point2D[10];
            for (int y = 1; y < GRIDH - 1; y++)
                for (int x = 1; x < GRIDW - 1; x++) //For every tile (2,2) through (39,39), otherwise it goes off the edge
                {
                    adjacent[1] = new Point2D(x - 1, y + 1);
                    adjacent[2] = new Point2D(x, y + 1);
                    adjacent[3] = new Point2D(x + 1, y + 1);
                    adjacent[4] = new Point2D(x - 1, y);
                    adjacent[6] = new Point2D(x + 1, y);
                    adjacent[7] = new Point2D(x - 1, y - 1);
                    adjacent[8] = new Point2D(x, y - 1);
                    adjacent[9] = new Point2D(x + 1, y - 1);

                    float totaltemp = tileArray[x, y].temperature;
                    int divideTotal = 1;

                    for (int k = 1; k <= 9; k++)
                    {
                        if (k == 5)
                            k++; //Skip position 5

                        if (tileArray[(int)adjacent[k].X, (int)adjacent[k].Y].isPassable)
                        {
                            totaltemp += tileArray[(int)adjacent[k].X, (int)adjacent[k].Y].temperature;
                            divideTotal++;
                        }
                    }

                    tileArray[x, y].temperature = (totaltemp / divideTotal) - 1; //This one's scent is the average of all adjacent tiles, minus scent decay

                    if (tileArray[x, y].temperature > 20)
                    {
                        tileArray[x, y].temperature -= 0.5f;
                        if (tileArray[x, y].temperature < 20)
                            tileArray[x, y].temperature = 20;
                    }
                    else if (tileArray[x, y].temperature < 20)
                    {
                        tileArray[x, y].temperature += 0.5f;
                        if (tileArray[x, y].temperature > 20)
                            tileArray[x, y].temperature = 20;
                    }
                }
        }
    } //An individual floor of the dungeon
}