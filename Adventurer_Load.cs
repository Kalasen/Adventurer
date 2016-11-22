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
		static List<Species> FileL_Creatures(List<Item> itemLibrary)
        {
            #region Storage Variables
            string name;
            string habitat = String.Empty;
            string creatureArmorType = "error";
            byte speed = 0;
            byte imageIndex = 0;
            byte baseLevel = 1;
            DNotation damage = new DNotation();
            int mass = 1;
            int xp = 1;
            short minLevel = -999; //The minimum level a creature can be spawned at
            Weapon weapon = null;
            List<Armor> armor = new List<Armor>();
            List<Item> inventory = new List<Item>();
            Color creatureColor = Color.White;
            List<BodyPart> anatomy = new List<BodyPart>();
            List<Species> creatureList = new List<Species>();
            #endregion

            StreamReader read = new StreamReader("Content/Encyclopaedia Adventura/Creatures.txt");
            string line = String.Empty;

            #region CREATURE.TXT
            while (line != "[END]") // While there's still code stuff in the file
            {
                line = read.ReadLine(); //Gather the next line
                line = line.Trim(); //Ignore white space on ends

                #region CREATURE
                if (line.StartsWith("[CREATURE]")) //If the line is a CREATURE tag
                {
                    line = line.Remove(0, 11); //Clip off the CREATURE tag, leaving only what's after
                    name = line; //Which should be the creature's name

                    while (line != "[/CREATURE]") //While within a creature tag area
                    {
                        line = read.ReadLine();
                        line = line.Trim();

                        #region HABITAT
                        if (line.StartsWith("[HABITAT]")) //If the line is a habitat tag
                        {
                            line = line.Remove(0, 10); //Clip off the habitat tag, leaving only what's after
                            habitat = line; //Which should be the creature's habitat
                        }
                        #endregion

                        #region MINLEVEL
                        if (line.StartsWith("[MINLEVEL]")) //If the line is a mimimum level tag
                        {
                            line = line.Remove(0, 11); //Clip off the min level tag, leaving only what's after
                            minLevel = short.Parse(line); //Which should be the creature's minimum level
                        }
                        #endregion

                        #region BASELVL
                        if (line.StartsWith("[BASELVL]")) //If the line is a mimimum level tag
                        {
                            line = line.Remove(0, 10); //Clip off the min level tag, leaving only what's after
                            baseLevel = byte.Parse(line); //Which should be the creature's minimum level
                        }
                        #endregion

                        #region SPEED
                        if (line.StartsWith("[SPEED]")) //If the line is a speed tag
                        {
                            line = line.Remove(0, 8); //Clip off the speed tag, leaving only what's after
                            speed = byte.Parse(line); //Which should be the creature's speed
                        }
                        #endregion

                        #region DAMAGE
                        if (line.StartsWith("[DAMAGE]")) //If the line is a damage tag
                        {
                            line = line.Remove(0, 9); //Clip off the min damage tag, leaving only what's after
                            string[] pieces = line.Split(new char[2]{'d', '+'});
                            damage.nDie = int.Parse(pieces[0]);
                            damage.sides = int.Parse(pieces[1]);
                                                        
                            if (pieces.Length > 2)                            
                            {
                                damage.bonus = int.Parse(pieces[2]); //Which should be the creature's minimum damage
                            }                            
                        }
                        #endregion

                        #region IMAGE
                        if (line.StartsWith("[IMAGE]")) //If the line is an image tag
                        {
                            line = line.Remove(0, 8); //Clip off the image tag, leaving only what's after
                            imageIndex = byte.Parse(line); //Which should be the creature's image index
                        }
                        #endregion

                        #region ARMORTYPE
                        if (line.StartsWith("[ARMORTYPE]")) //If the line is an ARMORTYPE tag
                        {
                            line = line.Remove(0, 12); //Clip off the ARMORTYPE tag, leaving only what's after
                            creatureArmorType = line; //Which should be the creature's armor type
                        }
                        #endregion

                        #region COLOR
                        if (line.StartsWith("[COLOR]")) //If the line is a color tag
                        {
                            byte r, g, b;
                            string[] data = line.Split(' ');
                            r = byte.Parse(data[1]); //The first three numbers
                            g = byte.Parse(data[2]); //The next three numbers
                            b = byte.Parse(data[3]); //The next three numbers

                            creatureColor = Color.FromArgb(r, g, b);
                        }
                        #endregion

                        #region MASS
                        if (line.StartsWith("[MASS]")) //If the line is an image tag
                        {
                            line = line.Remove(0, 7); //Clip off the image tag, leaving only what's after
                            mass = int.Parse(line); //Which should be the creature's image index
                        }
                        #endregion

                        #region BODYPART
                        if (line.StartsWith("[BODYPART]")) //If the line is a body part tag
                        {
                            string partName = String.Empty;
                            string parentName = String.Empty;
                            string armorType = String.Empty;
                            bool canPickUpItem = false;
                            bool lifeCritical = false;
                            int noInjury = 0;
                            int minorInjury = 0;
                            int breakInjury = 0;
                            int mangleInjury = 0;

                            line = line.Remove(0, 11);
                            partName = line;

                            while (line != "[/BODYPART]") //While within the tag area
                            {
                                line = read.ReadLine();
                                line = line.Trim();

                                if (line.StartsWith("[PARENT]"))
                                {
                                    line = line.Remove(0, 9);
                                    parentName = line;
                                }

                                if (line.StartsWith("[PICKUP]"))
                                    canPickUpItem = true;

                                if (line.StartsWith("[LIFECRITICAL]"))
                                    lifeCritical = true;

                                if (line.StartsWith("[ARMORTYPE]"))
                                {
                                    line = line.Remove(0, 12);
                                    armorType = line;
                                }

                                if (line.StartsWith("[HEALTH]"))
                                {
                                    line = line.Remove(0, 9);
                                    noInjury = int.Parse(line);
                                    minorInjury = (int)((double)noInjury * 0.75);
                                    breakInjury = (int)((double)noInjury * 0.5);
                                    mangleInjury = (int)((double)noInjury * 0.25);
                                }
                            }

                            BodyPart thisBodyPart = new BodyPart(partName, parentName, canPickUpItem, armorType,
                                noInjury, minorInjury, breakInjury, mangleInjury);
                            thisBodyPart.lifeCritical = lifeCritical;
                            anatomy.Add(thisBodyPart);
                        }
                        #endregion

                        #region ITEM
                        if (line.StartsWith("[ITEM]")) //If the line is an item tag
                        {
                            line = line.Remove(0, 7); //Clip off the item tag, leaving only what's after
                            string itemName = line; //Which should be the item to spawn the creature with
                            foreach (Item i in itemLibrary) //Search for the right item
                                if (itemName == i.name)
                                    inventory.Add(i);
                        }
                        #endregion

                        #region ARMOR
                        if (line.StartsWith("[ARMOR]")) //If the line is an item tag
                        {
                            line = line.Remove(0, 8); //Clip off the item tag, leaving only what's after
                            string armorName = line; //Which should be the item to spawn the creature with
                            foreach (Item i in itemLibrary) //Search for the right item
                                if (armorName == i.name && i is Armor)
                                {
                                    armor.Add((Armor)i);
                                }
                        }
                        #endregion

                        #region WEAPON
                        if (line.StartsWith("[WEAPON]")) //If the line is an item tag
                        {
                            line = line.Remove(0, 9); //Clip off the item tag, leaving only what's after
                            string weaponName = line; //Which should be the item to spawn the creature with
                            foreach (Item i in itemLibrary) //Search for the right item
                                if (weaponName == i.name && i is Weapon)
                                {
                                    weapon = (Weapon)i;
                                }
                        }
                        #endregion

                        #region XP
                        if (line.StartsWith("[XP]")) //If the line is a speed tag
                        {
                            line = line.Remove(0, 5); //Clip off the speed tag, leaving only what's after
                            xp = int.Parse(line); //Which should be the creature's speed
                        }
                        #endregion
                    }
                    Species thisCreature = new Species(speed, new DNotation(damage), mass, imageIndex,
                        creatureColor, name, habitat, new List<BodyPart>(anatomy), rng.Next());
                    thisCreature.armorType = creatureArmorType;
                    thisCreature.armor = new List<Armor>(armor);
                    thisCreature.baseLevel = baseLevel;
                    thisCreature.inventory = new List<Item>(inventory);
                    thisCreature.minDLevel = minLevel;
                    thisCreature.armorType = creatureArmorType;
                    thisCreature.weapon = weapon;
                    thisCreature.xpWorth = xp;
                    creatureList.Add(new Species(thisCreature)); //Add the new creature to the bestiary
                    armor.Clear();
                    inventory.Clear();
                    anatomy.Clear();
                    weapon = null;
                    creatureArmorType = null;                    
                }
                #endregion
            }
            #endregion

            read.Close();
            return new List<Species>(creatureList);
        }

        //TODO: Finish converting to XML
		public static List<Item> FileL_Item(List<Material> universalMaterialList, StreamReader read)
		{			
			string[] splitTag = new string[1023];
			List<Item> itemList = new List<Item>();
			
			while (splitTag[0] != "[END]")
			{
				string name = String.Empty; //Name of the item
				
				string line = String.Empty;
				while(line == String.Empty)
					line = read.ReadLine().Trim();
				
				splitTag = line.Split(']');
				splitTag[0] += "]"; //Add the trimmed ']' back
				if (splitTag.Length > 1)
					splitTag[1] = splitTag[1].Trim(); //Lose the space	
				
				switch(splitTag[0])
				{
				case "[AMULET]":
				case "[ARMOR]":
				case "[COMPONENT]":
				case "[POTION]":
				case "[TOOL]":
				case "[WEAPON]":
					name = splitTag[1];
					DNotation damage = new DNotation();
					int aC = 1;
					float mass = 1f;
					float volume = 1f;
					string armorType = "missingatomy";
					Material material = new Material();
					List<string> useList = new List<string>();
					List<string> covers = new List<string>();
					List<Item> componentList = new List<Item>();
					Effect effect = new Effect();
					Color color = Color.White;
					
					while (splitTag[0] != "[/AMULET]" &&
					       splitTag[0] != "[/ARMOR]" &&
					       splitTag[0] != "[/COMPONENT]" &&
					       splitTag[0] != "[/POTION]" &&
					       splitTag[0] != "[/TOOL]" &&
					       splitTag[0] != "[/WEAPON]") //While within the item block
					{
						splitTag = read.ReadLine().Trim().Split(']'); //New line, trimmed whitespace, split by space						
						splitTag[0] += "]"; //Add the trimmed ']' back
						if (splitTag.Length <= 1)
							break;
						
						splitTag[1] = splitTag[1].Trim(); //Lose the space						
							
						string data = splitTag[1];				
						
						#region Gather item info from line
						switch (splitTag[0])
						{			
						case "[AC]":
							aC = int.Parse(data); //Which should be the armor class
							break;
							
						case "[ARMORTYPE]":
							armorType = data; //Which should be the armor type
							break;
							
						case "[COLOR]":							
                            string[] piece = data.Split(' '); //Clip off the color tag, leaving only what's after
							byte[] c = new byte[3];
                            c[0] = byte.Parse(piece[0]); //Red
                            c[1] = byte.Parse(piece[1]); //Green                            
                            c[2] = byte.Parse(piece[2]); //Blue

                            color = Color.FromArgb(c[0], c[1], c[2]);
							break;
							
						case "[COMPONENT]":
							string[] dataSplit = data.Split(':');
							foreach (Item i in itemLibrary) //Search for the item referenced
							{
                                if (i.name == dataSplit[0])
								{
									Item component = Item.CopyDeep(i); //True new item
									if (dataSplit.Length > 1) //If there's a second part
										foreach(Material m in universalMaterialList) //Check all materials									
											if (m.name == dataSplit[1]) //For a match
												component.material = new Material(m); //Add the overridden material if we can
									
                                    componentList.Add(Item.CopyDeep(component)); 
								}
							}							
							break;
							
						case "[COVERS]":
							covers.Add(data); //What armor covers
							break;
							
						case "[DAMAGE]":
							string[] pieces = data.Split(new char[2] { 'd', '+' });
                            damage.nDie = int.Parse(pieces[0]);
                            damage.sides = int.Parse(pieces[1]);
                            if (pieces.Length > 2)
                                damage.bonus = int.Parse(pieces[2]); //Which should be the creature's minimum damage
							break;
							
						case "[EFFECT]":
							string[] parts = data.Split(':'); //Split the line
							if (parts.Length >= 2)
								effect = new Effect(int.Parse(parts[1]), parts[0]);
							else
								effect = new Effect(1, parts[0]);
							break;
							
						case "[MASS]":
	                        mass = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture); //Respect my ,. authoritah
							break;
							
						case "[MATERIAL]":
                            foreach (Material i in universalMaterialList) //Search for the material referenced
                                if (i.name == data)
									material = i; //Success               
							break;
							
						case "[USE]":
							useList.Add(data);
							break;
							
						case "[VOLUME]":
	                        volume = float.Parse(data, System.Globalization.CultureInfo.InvariantCulture); //Respect my ,. authoritah
							break;
						}
						#endregion
					}
					
					Item thisItem = new Item();
					
					#region Build item from info, add to list
					switch(splitTag[0])
					{
					case "[/AMULET]":
						thisItem = new Amulet(mass, volume, name, color, effect);
						break;
					case "[/ARMOR]":
						thisItem = new Armor(mass, volume, aC, armorType, componentList, name, covers, color);
						break;
					case "[/COMPONENT]":
						thisItem = new Item(mass, volume, name, color);
						break;
					case "[/POTION]":
						thisItem = new Potion(name, color, componentList, effect);
						break;
					case "[/TOOL]":
						thisItem = new Item(mass, volume, name, color);
						break;
					case "[/WEAPON]":
						thisItem = new Weapon(mass, volume, name, color, damage);
						break;
					}
					
					thisItem.componentList = new List<Item>(componentList); //Copy, not pointer-link
					thisItem.damage = new DNotation(damage); //Copy, not pointer-link
					thisItem.use = new List<string>(useList); //Copy, not pointer link
					thisItem.material = material;
					
					itemList.Add(Item.CopyDeep(thisItem)); //Copy, not reference the soon deleted temp item
					
					material = null;
					componentList.Clear();
					covers.Clear();
					useList.Clear();
					#endregion
					break;
				}
			}
			
			read.Close();
			return itemList;
		}
		
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

                        if (piece[1] == "pinecone") //Bluh hackish improve later
                        {
                            currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Add(new Item("pinecone"));
                        }
                        else
                        {
                            foreach (Species c in bestiary) //For gore
                            {
                                if (piece[1].StartsWith(c.name)) //If it's a creature part
                                {
                                    string[] split = piece[1].Split(' '); //Get creature name and part name
                                    
									Item gore;
                                    if (split[1] == "corpse")
                                        gore = new Item(piece[1], c.color);
                                    else
                                        gore = new Item(piece[1], Color.Crimson);
                                    gore.itemImage = 253; //"²"
                                    gore.edible = true;
                                    currentLevel.tileArray[tilePos.X, tilePos.Y].itemList.Add(gore);
                                    break;
                                }
                            }

                            foreach (Item i in itemLibrary)
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

                        foreach (Species c in bestiary) //Look for the creature to add
                        {
                            if (piece[0] == c.name) //If the name matches the type of creature generator
                            {
                                if (isPlayer)
                                {
                                    Creature p = c.GenerateCreature("monster", itemLibrary, int.Parse(piece[1]));
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
                                        Creature p = c.GenerateCreature("quest giver", itemLibrary, int.Parse(piece[1]));
                                        p.inventory.Clear();
                                        p.wornArmor.Clear();
                                        p.weapon = null;
                                        currentLevel.SpawnCreature(p, tilePos);
                                    }
                                    else
                                    {
                                        Creature p = c.GenerateCreature("monster", itemLibrary, int.Parse(piece[1]));
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
                                currentLevel.creatures[creatureIndex].inventory.Add(new Item("pinecone"));
                            }

                            foreach (Species c in bestiary) //For gore
                            {
                                if (piece[a].StartsWith(c.name)) //If it's a creature part
                                {
                                    string[] split = piece[a].Split(' '); //Get creature name and part name
									Item gore;
                                    if (split[1] == "corpse")
                                        gore = new Item(piece[1], c.color);
                                    else
                                        gore = new Item(piece[1], Color.Crimson);
                                    gore.itemImage = 253;
                                    gore.edible = true;
                                    currentLevel.creatures[creatureIndex].inventory.Add(gore);
                                    break;
                                }
                            }

                            foreach (Item i in itemLibrary)
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