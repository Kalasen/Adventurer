using System; //General C# functions
using System.Collections.Generic; //So Lists can be used
using System.Drawing; //The colors, man
using System.IO; //Mainly used for reading and writing files
using System.Runtime.InteropServices; //For use in wrangling pointers in their place
using System.Xml;
using Tao.Sdl;

namespace Adventurer
{
	public partial class Adventurer
    { 
		static void FileS_Level(Level currentLevel)
        {
            string folderPath = "Saves/" + sessionName.ToString(); //Folder name based on seed
            string filePath = folderPath + "/(" + currentLevel.mapPos.X + ", " + currentLevel.mapPos.Y + ", " + 
                currentLevel.mapPos.Z + ").txt"; //File path

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); //Create the folder if it doesn't exist

            List<string> data = new List<string>(); //The level data as it will be written to the file.

            data.Add("[TYPE] " + currentLevel.levelType); //Track the level's type
            data.Add("[SEED] " + currentLevel.seed.ToString()); //Store the base seed

            #region Write tile data
            for (int y = 0; y < Level.GRIDH; y++)
                for (int x = 0; x < Level.GRIDW; x++)
                {                    
                    Tile currentTile = currentLevel.tileArray[x, y]; //Working with current tile

                    bool creatureHere = false;
                    foreach (Creature c in currentLevel.creatures)
                        if (c.pos == new Vector2(x, y))
                            creatureHere = true; //There's a creature at this tile

                    if (currentTile.itemList.Count > 0 || currentTile.fixtureLibrary.Count > 0 || creatureHere)
                    {
                        data.Add("(" + x + ", " + y + ")"); //Mark the tile if there's something notable here
                    }

                    foreach (Item i in currentTile.itemList)
                    {
                        #region Write item data
                        if (i is Armor)
                            data.Add("[ARMOR] " + i.name);
                        else if (i is Potion)
                            data.Add("[POTION] " + i.name);
                        else if (i is Scroll)
                            data.Add("[SCROLL] " + i.name);
                        else if (i is Weapon)
                            data.Add("[WEAPON] " + i.name);
                        else if (i != null)
                            data.Add("[ITEM] " + i.name);
                        #endregion
                    }

                    foreach (Fixture f in currentTile.fixtureLibrary)
                    {
                        #region Save Fixture Data
                        if (f is Door)
                        {
                            Door d = (Door)f;
                            if (d.isOpen)
							{
								if (d.isVertical)
								{
                                	data.Add("[DOOR] open:true");
								}
								else
								{
									data.Add("[DOOR] open:false");
								}
							}
                            else
							{
                                if (d.isVertical)
								{
                                	data.Add("[DOOR] closed:true");
								}
								else
								{
									data.Add("[DOOR] closed:false");
								}
							}
                        }
                        else if (f is Stairs)
                        {
                            Stairs s = (Stairs)f;
                            if (s.isDown)
                                data.Add("[STAIRS] down");
                            else
                                data.Add("[STAIRS] up");
                        }
                        else if (f is Tree)
                        {
                            Tree t = (Tree)f;
                            data.Add("[TREE] " + t.species + ":" + t.fruit);
                        }
                        else if (f is Trap)
                        {
                            Trap t = (Trap)f;
                            data.Add("[TRAP] " + t.effect.type);
                        }
                        else if (f is Trap)
                            data.Add("[FIXTURE] ");
                        #endregion
                    }

                    foreach (Creature c in currentLevel.creatures)
                    {
                        #region Save Creatures
                        if (c.pos.X == x && c.pos.Y == y)
                        {
                            if (c is QuestGiver)
                            {
                                QuestGiver q = (QuestGiver)c;
                                data.Add("[QUESTGIVER] " + q.name + ":" + q.seed + ":" + q.wantObject + ":" + q.giveObject);

                                foreach (Item i in q.inventory)                                
                                    data[data.Count - 1] += ":" + i.name;

                                if (q.wornArmor.Count > 0)
                                    data[data.Count - 1] += ":ARMOR";

                                foreach (Armor a in q.wornArmor)
                                    data[data.Count - 1] += ":" + a.name;

                                if (q.weapon != null)
                                    data[data.Count - 1] += ":WEAPON" + q.weapon.name;
                            }
                            else if (currentLevel.creatures[0].pos == c.pos) //If it's the player
                            {
                                data.Add("[PLAYER] " + c.name);
                                data[data.Count - 1] += ":" + c.seed;
                                data[data.Count - 1] += ":" + c.hp;
                                data[data.Count - 1] += ":" + c.hpMax;
                                data[data.Count - 1] += ":" + c.xp;
								data[data.Count - 1] += ":" + c.gold;

                                foreach (Item i in c.inventory)
                                    data[data.Count - 1] += ":" + i.name;

                                if (c.wornArmor.Count > 0)
                                    data[data.Count - 1] += ":ARMOR:";

                                foreach (Armor a in c.wornArmor)
                                    data[data.Count - 1] += ":" + a.name;

                                if (c.weapon != null)
                                    data[data.Count - 1] += ":WEAPON:" + c.weapon.name;
                            }
                            else
                            {
                                data.Add("[CREATURE] " + c.name);
                                data[data.Count - 1] += ":" + c.seed; //Store how to generate base creature

                                foreach (Item i in c.inventory)
                                    data[data.Count - 1] += ":" + i.name;

                                if (c.wornArmor.Count > 0)
                                    data[data.Count - 1] += ":ARMOR:";

                                foreach (Armor a in c.wornArmor)
                                    data[data.Count - 1] += ":" + a.name;

                                if (c.weapon != null)
                                    data[data.Count - 1] += ":WEAPON:" + c.weapon.name;
                            }
                        }
                        #endregion
                    }
                }
            #endregion

            data.Add("[END]");
            string[] dataArray = data.ToArray(); //Convert to a fixed array for next part.

            File.WriteAllLines(filePath, dataArray); //Write the data
        }
        static void FileL_Level(Vector3 pos)
        {
            #region Setup
            string levelPath = "Saves/" + sessionName.ToString() + "/(" + pos.X.ToString() + ", " + pos.Y.ToString() + ", " +
                pos.Z.ToString() + ").txt"; //The path to the level
            Vector2 tilePos = new Vector2(); //The tile position we're working with

            if (!File.Exists(levelPath))
            {
                GenLevel("Dungeon", false); //Fabricate one
                return;
            }
            #endregion

            #region Gen base level
            using (StreamReader read = new StreamReader(levelPath))
            {
                string line = read.ReadLine(); //First entry should be the level type
                line = line.Remove(0, 7); //Clip off "[TYPE] "
                string levelType = line; // example: "Dungeon"

                line = read.ReadLine(); //Next is the seed
                line = line.Remove(0, 7); //Clip off "[SEED] "

                levelSeed[pos.X, pos.Y, pos.Z] = int.Parse(line); //Fix the seed to its original

                GenLevel(levelType, false); //Recreate the originally genned level
                Creature tempCreature = new Creature(currentLevel.creatures[0]);
                currentLevel.creatures.Clear(); //We'll fill this in from the load file
                for (int y = 0; y < Level.GRIDH; y++)
                    for (int x = 0; x < Level.GRIDW; x++) //For all tiles
                    {
                        currentLevel.tileArray[x, y].scentIdentifier.Add(tempCreature.name); //Keep track of this creature's scent now
                        currentLevel.tileArray[x, y].scentMagnitude.Add(0); //Start it at zero scent in the room
                        currentLevel.tileArray[x, y].itemList.Clear(); //Clear out previous items
                        if (!currentLevel.tileArray[x, y].isPassable && currentLevel.tileArray[x, y].fixtureLibrary.Count > 0)
                        {
                            currentLevel.tileArray[x, y].isTransparent = true; //Remove influence of fixtures
                            currentLevel.tileArray[x, y].isPassable = true;
                        }
                        currentLevel.tileArray[x, y].fixtureLibrary.Clear();
                    }
                //currentLevel.creatureList.Add(tempCreature);
            #endregion

                line = read.ReadLine();
                while (line != "[END]")
                {
                    #region Work on individual tiles
                    if (line.StartsWith("(")) //If it's a tile position
                    {
                        #region Base Tile Data
                        string[] piece = line.Split(','); //Split apart based on commas

                        piece[0] = piece[0].Remove(0, 1); //Remove "("
                        piece[1] = piece[1].Substring(0, piece[1].Length - 1); //Remove ")"
                        tilePos.X = short.Parse(piece[0]); //Get the X digit
                        tilePos.Y = short.Parse(piece[1]); //Get the Y digit
                        currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Clear(); //Empty the random gen items.
                        currentLevel.tileArray[tilePos.X, tilePos.Y].fixtureLibrary.Clear(); //Empty the random gen items.
                        #endregion
                    }
                    else if (line.StartsWith("[ARMOR]") || line.StartsWith("[ITEM]") || line.StartsWith("[POTION]") || line.StartsWith("[SCROLL]")
                        || line.StartsWith("[WEAPON]")) //If an item of some sort
                    {
                        #region Item Data
                        string[] piece = line.Split(']'); //Split based on ]
                        piece[0] = piece[0].Remove(0, 1);
                        piece[1] = piece[1].Remove(0, 1);

                        if (piece[1] == "pinecone") //TODO: Bluh hackish improve later
                        {
                            currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Add(new Item(content.items.Find(item => item.name == "pinecone")));
                        }
                        else
                        {
                            foreach (Species c in content.bestiary) //For gore
                            {
                                if (piece[1].StartsWith(c.name)) //If it's a creature part
                                {
                                    string[] split = piece[1].Split(' '); //Get creature name and part name
                                    
									Item gore;
                                    if (split[1] == "corpse")
                                        gore = new Item(500f, 500f, $"{c.name} corpse", c.color, new List<Item>(), new List<string>());
                                    else
                                        gore = new Item(500f, 500f, $"{c.name} corpse", Color.Crimson, new List<Item>(), new List<string>());
                                    gore.itemImage = 253; //"²"
                                    gore.edible = true;
                                    currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Add(gore);
                                    break;
                                }
                            }

                            foreach (Item i in content.items)
                            {
                                if (i.name == piece[1])
                                {
                                    currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Add(i);
                                    break;
                                }
                            }
                        }
                        #endregion
                    }
                    else if (line.StartsWith("[CREATURE]") || line.StartsWith("[QUESTGIVER]") || line.StartsWith("[PLAYER]"))
                    {
                        #region Creature Data
                        bool isPlayer = false;
                        int creatureIndex = 0;

                        if (line.StartsWith("[CREATURE]"))
                            line = line.Remove(0, 11); // Clip off the "[CREATURE] " part
                        else if (line.StartsWith("[QUESTGIVER]"))
                            line = line.Remove(0, 13); // Clip off the "[QUESTGIVER] " part
                        else if (line.StartsWith("[PLAYER]"))
                        {
                            isPlayer = true;
                            line = line.Remove(0, 9); //Clip off the "[PLAYER] " part
                        }

                        string[] piece = line.Split(':'); //Split it by ':'                    

                        foreach (Species c in content.bestiary) //Look for the creature to add
                        {
                            if (piece[0] == c.name) //If the name matches the type of creature generator
                            {
                                if (isPlayer)
                                {
                                    Creature p = c.GenerateCreature("monster", content.items, int.Parse(piece[1]));
                                    p.hp = int.Parse(piece[2]);
                                    p.hpMax = int.Parse(piece[3]);
                                    p.xp = int.Parse(piece[4]);
									p.gold = int.Parse(piece[5]);
                                    while (p.xp > p.xpBorder*2)
                                        p.xpBorder *= 2;
                                    p.inventory.Clear();
                                    p.wornArmor.Clear();
                                    p.weapon = null;
                                    p.pos = tilePos;
                                    creatureIndex = 0;

                                    for (int y = 0; y < Level.GRIDH; y++)
                                        for (int x = 0; x < Level.GRIDW; x++)
                                        {
                                            currentLevel.tileArray[x, y].scentIdentifier.Add(p.name); //Keep track of this creature's scent now
                                            currentLevel.tileArray[x, y].scentMagnitude.Add(0); //Start it at zero scent in the room
                                        }

                                    currentLevel.creatures.Insert(0, p); //Insert player at spot 0
                                }
                                else
                                {
                                    creatureIndex = currentLevel.creatures.Count;

                                    if (currentLevel.levelType == "village")
                                    {
                                        Creature p = c.GenerateCreature("quest giver", content.items, int.Parse(piece[1]));
                                        p.inventory.Clear();
                                        p.wornArmor.Clear();
                                        p.weapon = null;
                                        currentLevel.SpawnCreature(p, tilePos);
                                    }
                                    else
                                    {
                                        Creature p = c.GenerateCreature("monster", content.items, int.Parse(piece[1]));
                                        p.inventory.Clear();
                                        p.wornArmor.Clear();
                                        p.weapon = null;
                                        currentLevel.SpawnCreature(p, tilePos);
                                    }
                                }
                            }
                        }

                        for (int a = 2; a < piece.Length; a++) //Look for items in the inventory to add
                        {
                            if (piece[a] == "pinecone") //Bluh hackish improve later
                            {
                                currentLevel.creatures[creatureIndex].inventory.Add(new Item(content.items.Find(item => item.name == "pinecone")));
                            }

                            foreach (Species c in content.bestiary) //For gore
                            {
                                if (piece[a].StartsWith(c.name)) //If it's a creature part
                                {
                                    string[] split = piece[a].Split(' '); //Get creature name and part name
									Item gore;
                                    if (split[1] == "corpse")
                                        gore = new Item(500f, 500f, $"{c.name} corpse", c.color, new List<Item>(), new List<string>());
                                    else
                                        gore = new Item(500f, 500f, $"{c.name} corpse", Color.Crimson, new List<Item>(), new List<string>());

                                    gore.itemImage = 253;
                                    gore.edible = true;
                                    currentLevel.creatures[creatureIndex].inventory.Add(gore);
                                    break;
                                }
                            }

                            foreach (Item i in content.items)
                            {
                                if (i.name == piece[a])
                                {
                                    currentLevel.creatures[creatureIndex].inventory.Add(i);
                                }
                            }
                        }
                        #endregion
                    }
                    else if (line.StartsWith("[TRAP]"))
                    {
                        #region Trap Data
                        line = line.Remove(0, 7); //Remove the "[TRAP] " part
                        currentLevel.tileArray[tilePos.X, tilePos.Y].fixtureLibrary.Add(new Trap(new Effect(rngDie.Roll(5), line))); //Add trap
                        #endregion
                    }
                    else if (line.StartsWith("[TREE]"))
                    {
                        #region Tree Data
                        line = line.Remove(0, 7); //Remove the "[TREE] " part
                        string[] piece = line.Split(':');
                        Tree thisTree = new Tree(rng);
                        thisTree.species = piece[0];
                        thisTree.fruit = piece[1];

                        currentLevel.tileArray[tilePos.X, tilePos.Y].isPassable = false; //Fix for tree
                        currentLevel.tileArray[tilePos.X, tilePos.Y].isTransparent = false; //Fix for tree
                        currentLevel.tileArray[tilePos.X, tilePos.Y].fixtureLibrary.Add(thisTree); //Add tree
                        #endregion
                    }
                    else if (line.StartsWith("[DOOR]"))
                    {
                        #region Door Data
                        line = line.Remove(0, 7);
						string[] dataSplit = line.Split(':');
                        Tile thisTile = currentLevel.tileArray[tilePos.X, tilePos.Y];
                        Door thisDoor = new Door(thisTile, bool.Parse(dataSplit[1]));
                        thisTile.fixtureLibrary.Add(thisDoor);

                        if (dataSplit[0] == "open")
                        {
                            thisDoor.Open(thisTile, currentLevel); //Open the door from default closed
                        }
                        else
                        {
                            thisDoor.Close(thisTile, currentLevel);
                        }
                        #endregion
                    }
                    else if (line.StartsWith("[STAIRS]"))
                    {
                        #region Stairs Data
                        line = line.Remove(0, 9);
                        if (line == "down")
                            currentLevel.tileArray[tilePos.X, tilePos.Y].fixtureLibrary.Add(new Stairs(true));
                        else
                            currentLevel.tileArray[tilePos.X, tilePos.Y].fixtureLibrary.Add(new Stairs(false));
                        #endregion
                    }
                    #endregion

                    line = read.ReadLine();
                }
            }
            //read.Dispose();
            //read.Close(); //Close this now that we're done with it
        } //PCLoadLevel
        static void FileL_ImportUniverse(string handle)
        {
            #region Mobius Double Reacharound Virus
            //MSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPA
            //MSPA   ~ATH do {We_Part()}  MSPA
            //MSPAMSPAMSPAMSPAMSPAMSPAMSPAMSPA
            //
            //bifurcate THIS[THIS, THIS];
            //import universe U1;
            //import universe U2;

            //~ATH(U1) (
            //
            //     ~ATH(!U2) {
            //
            //} EXECUTE (~ATH(THIS){}EXECUTE(NULL));
            //
            //     } EXECUTE(~ATH(THIS){}EXECUTE(NULL));
            //
            //[THIS, THIS].DIE();
            #endregion

            string[] lines = File.ReadAllLines("Saves/SaveStockpile.txt"); //Read in the list of sessions

            for (int n = 0; n < lines.Length; n++) //Loop through the lines
            {
                if (lines[n].StartsWith("[NAME] " + handle))
                {
                    sessionName = handle; //Load in session name
                    worldSeed = int.Parse(lines[n + 1].Remove(0, 7)); //Load in seed

                    string folderPath = "Saves/" + sessionName;
                    string[] worldData = File.ReadAllLines(folderPath + "/WorldData.txt"); //Load in world data
                    string playerPos = worldData[1].Remove(0, 13); //x, y, z
                    string[] positions = playerPos.Split(',');
                    positions[2] = positions[2].Substring(0, positions[2].Length - 1); //Take off last parenthesis

                    mapPos.X = int.Parse(positions[0]);
                    mapPos.Y = int.Parse(positions[1]);
                    mapPos.Z = int.Parse(positions[2]);

                    FileL_Level(mapPos); //Load first level

                    currentLevel.creatures[0].killCount = int.Parse(worldData[2].Remove(0, 12)); //Get killcount
                    exploredLevels = int.Parse(worldData[3].Remove(0, 11)); //Get explored levels
                    break;
                }
            }
        }
        static void FileS_World()
        {
            FileS_Level(currentLevel);

            string folderPath = "Saves/" + sessionName;
            string filePath = folderPath + "/WorldData.txt";

            List<string> data = new List<string>(); //The data to write

            data.Clear();

            data.Add("[SEED] " + worldSeed.ToString()); // Track the world seed
            data.Add("[PLAYERPOS] (" + mapPos.X.ToString() +
                "," + mapPos.Y.ToString() +
                "," + mapPos.Z.ToString() + ")"); //Track player's position
            data.Add("[KILLCOUNT] " + currentLevel.creatures[0].killCount);
            data.Add("[EXPLORED] " + exploredLevels);

            File.WriteAllLines(filePath, data.ToArray()); //Write the data
        }
        static void FileD_World()
        {
            string[] oldData = File.ReadAllLines("Saves/SaveStockpile.txt");
            List<string> newData = new List<string>();

            newData.Add("[RACE] " + currentLevel.creatures[0].name);
            newData.Add("[DEATHCAUSE] Died from " + currentLevel.causeOfDeath);
            newData.Add("[DEATHMANNER] This happened because " + currentLevel.mannerOfDeath);
            newData.Add("[KILLCOUNT] " + currentLevel.creatures[0].killCount);
            newData.Add("[EXPLORED] " + exploredLevels);
            File.WriteAllLines("Graveyard/" + sessionName + "-" + worldSeed + ".txt", newData.ToArray());

            newData.Clear();

            for (int n = 0; n < oldData.Length; n++)
            {
                if (sessionName == oldData[n].Remove(0,7))
                {
                    n++; //Skip this and the next one
                }
                else
                {
                    newData.Add(oldData[n]);
                }
            }

            File.WriteAllLines("Saves/SaveStockpile.txt", newData.ToArray());

            for (int z = 0; z < 100; z++)
                for (int y = 0; y < 100; y++)
                    for (int x = 0; x < 100; x++)
                    {
                        levelSeed[x, y, z] = rng.Next(); //Seed the world
                    }
            totalTurnCount = 0;
            mapPos = new Vector3(50, 50, 1);

            if (Directory.Exists("Saves/" + sessionName))
            {
                Directory.Delete("Saves/" + sessionName, true); //DELETE EFFING EVERYTHING
            }
        }
	}
}