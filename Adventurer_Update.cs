using System; //General C# functions
using System.Collections.Generic; //So I can use List
using System.Drawing; //The colors, man
using System.IO; //Mainly used for reading and writing files
using System.Linq;
using System.Runtime.InteropServices; //For use in wrangling pointers in their place
using System.Threading;
using Tao.Sdl;
using KalaGame;

namespace Adventurer
{
	public partial class Adventurer
	{
        #region Variables		
        public static Sdl.SDL_Event keyEvent; //To tell whether a key was pressed or lifted
        public static Sdl.SDL_Event oldKeyEvent; //For telling if a key was just pressed
        public static bool[] keys = new bool[255]; //An array to see which keys are down.
        public static bool shift = false; //Whether the shift key is down
        public static bool[] pressList = new bool[10];
		#endregion
		
		static void Update()
		{
			switch (gameState)
			{
			case GameState.OpeningMenu:
				Update_OpeningMenu();
				break;
				
			case GameState.MainGame:
				Update_MainGame();
				break;
				
			case GameState.EscapeMenu:
				Update_EscapeMenu();
				break;
				
			case GameState.HealthMenu:
				Update_HealthMenu();
				break;
				
			case GameState.HelpMenu:
				Update_HelpMenu();
				break;
				
			case GameState.InventoryMenu:
				Update_InventoryMenu();
				break;
			}
		}
		
		static void Update_OpeningMenu()
		{
	        switch (Update_GetKey())
	        {
            	#region Directions
            case "2": //If down was pressed                
                pressList[2] = true;
                if (selectionCursor < 3) //If down was pressed...                   
                    selectionCursor++; //...move the cursor down one, if possible.
                else
                    selectionCursor = 1; //Loop over if needed
                break;
                

            case "8": //If up was pressed
                if (selectionCursor > 1)
                    selectionCursor--;
                else
                    selectionCursor = 3;
                break;
            	#endregion

            	#region Enter
            case "Enter": //If enter aka "271" or "13" was pressed
	            if (selectionCursor == 1) //If "New Game"
	            {
	                worldSeed = (int)DateTime.Now.Ticks; //Set the world seed
	
	                gameState = GameState.NameSelect; //Yup, name selection
	                Draw(); //Draw the name select menu
	
	                Update_GetSessionStr();
	
	                if (Directory.Exists("Saves/" + sessionName))
	                {
	                    try
	                    {
	                        FileL_ImportUniverse(sessionName); //Load it if we've got it
	                    }
	                    catch
	                    {
	                        Sdl.SDL_FillRect(screen, ref screenArea, 0); //Clear for next draw cycle
	                        screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface)); //Put the screen data in its place
	                        DrawText(veraSmall, "Loading of {" + sessionName + "} has failed.", 
	                            new Vector2(windowSizeX / 2 - 130, windowSizeY / 2));
	                        DrawText(veraSmall, "You can either clear it and start a new game, or save it and quit.",
	                            new Vector2(windowSizeX / 2 - 130, windowSizeY / 2 + 15));
	                        DrawText(veraSmall, "Delete corrupted data? [y/n]",
	                            new Vector2(windowSizeX / 2 - 130, windowSizeY / 2 + 30));
	                        Sdl.SDL_Flip(screen);
	
	                        if (Update_GetKey() == "y")
	                        {
	                            Directory.Delete("Saves/" + sessionName, true);
	
	                            string[] lines = File.ReadAllLines("Saves/SaveStockpile.txt"); //Read in the list of sessions
	                            List<string> newData = new List<string>();
	
	                            for (int n = 0; n < lines.Length; n++) //Loop through the lines
	                            {
	                                if (lines[n].StartsWith("[NAME] " + sessionName))
	                                {
	                                    n += 2;
										if (n >= lines.Length)
											break;
	                                }
	
	                                newData.Add(lines[n]);
	                            }
	                            File.WriteAllLines("Saves/SaveStockpile.txt", newData.ToArray());
	
	                            NewGame();
	                        }
	                        else
	                        {
	                            run = false;
	                            return;
	                        }                                                        
	                    }
	                }
	                else
	                {
	                    NewGame();
	                }
	
	                gameState = GameState.MainGame;
	            }
	            else if (selectionCursor == 2) //If "Help"
	                gameState = GameState.HelpMenu;
	            else if (selectionCursor == 4) //If "Load Game"
	            {
	                string[] saves = Directory.GetDirectories("Saves/");
	
	                if (saves.Length > 0) //If there's a save game
	                {
                        using (StreamReader read = new StreamReader(saves[0] + "/WorldData.txt")) //Open up the world data file
                        {
                            string line = read.ReadLine(); //Read the seed in
                            line = line.Remove(0, 7); //Remove "[SEED] "
                            worldSeed = int.Parse(line); //Seed the world

                            line = read.ReadLine(); //Read in the player position
                        }

	                    gameState = GameState.MainGame;
	                }
	            }
	            else if (selectionCursor == 3) //If "Quit"
	                run = false;
	            break;                
            	#endregion               
	        }
		}
		static void Update_MainGame()
		{
            for (int n = 0; n < currentLevel.creatures.Count; n++) //For every creature
            {
                #region All Creatures
                if (currentLevel.creatures[n].status_paralyzed > 0)
                {
                    break;
                }

                if (currentLevel.creatures[n].turn_energy >= TURN_THRESHOLD) //If a creature's turn
                {
                    if (n == 0)
                    {
						if (!Update_Player(currentLevel.creatures[0])) //If skipped turn
						{
							return;
						}
                    }
                    else
                    {        
						Update_Creature(currentLevel.creatures[n]);
                    }
					
					Update_PostTurn(currentLevel.creatures[n]);
                }

                currentLevel.creatures[n].turn_energy += currentLevel.creatures[n].speed; //Every cycle, add to energy
                #endregion
            }

			Update_World();            
		}
		static void Update_HealthMenu()
		{
            switch (Update_GetKey())
            {
                case "Escape":
                case "Space":
                    gameState = GameState.MainGame;
                    break;
            }
		}
		static void Update_HelpMenu()
		{		
			switch(Update_GetKey())
			{
            case "Space":
            case "Escape":
                gameState = GameState.OpeningMenu;				
                return;
			}
		}
		static void Update_EscapeMenu()
		{
            switch (Update_GetKey())
            {
            case "Space":
            case "Escape":
                gameState = GameState.MainGame;
                break;

            case "2": //If down was pressed
                if (selectionCursor < 2) //If down was pressed...                   
                    selectionCursor++; //...move the cursor down one, if possible.
                else
                    selectionCursor = 1; //Loop over if needed
                break;

            case "8": //If up was pressed
                if (selectionCursor > 1)
                    selectionCursor--;
                else
                    selectionCursor = 2;
                break;

            case "Enter": //If enter aka "271" was pressed
                if (selectionCursor == 1) //If "Return to game"
                    gameState = GameState.MainGame;
                else if (selectionCursor == 2) //If "Quit and Save"
                {
                    FileS_World();
                    run = false;
                }
                break;
            }
		}
		static void Update_InventoryMenu()
		{
            if (inventorySelect <= 0) //If at the outer menu
            {
				Inv_Outer();
            }
            else //If within an item
            {
                if (inventorySelect > currentLevel.creatures[0].inventory.Count) //If the item is nonexistent
                    inventorySelect = 0; //Revert

                if (inventoryMode == 0) //If we're just looking at an item
                {
                    Inv_Main();
                }
                else if (inventoryMode == 1) //If in crafting mode
                {
                    Inv_CombCraft();
                }

                inventorySelect = 0; //Revert
                inventoryMode = 0;
            }		
		}
		
		static void Inv_Outer()
		{
			bool done = false;
			
			while(!done)
			{
				string input = Update_GetKey();
			    switch (input)
	            {
	                case "Space":
	                case "Enter":
	                    gameState = GameState.MainGame;
	                    return;
	            }
	
	            inventorySelect = LetterIndexToNumber(input); //Set to letter input, if any
	
	            if (inventorySelect > currentLevel.creatures[0].inventory.Count || inventorySelect <= 0)
	                inventorySelect = 0;
				else // If a valid selection
				{
					Inv_Main();
					gameState = GameState.MainGame;
					done = true;
				}
			}
		}
		static void Inv_Main()
		{
			Item thisItem = currentLevel.creatures[0].inventory[inventorySelect - 1];
			Creature player = currentLevel.creatures[0];
			
			Draw();
			while (true)
			{
	            switch (Update_GetKey())
	            {
				case "Space":
				case "Enter":
				case "Backspace":
                case "Escape":
					inventorySelect = 0; //Nothing selected anymore
					Draw(); //Update the screen for backing out of menu
					return;
					
	            case "b": //Break down item
	                currentLevel.creatures[0].BreakDownItem(currentLevel, thisItem); //Break down said item
	                inventorySelect = 0;
	                gameState = GameState.MainGame;
	                return;
	
	            case "c":
	                #region Enter Combine Craft Mode
	                if (currentLevel.creatures[0].isDextrous)
	                {
	                    craftableItems = currentLevel.creatures[0].FindValidRecipe( //Find list of items
	                        thisItem, currentLevel.content.items.ToList()); //that can be made	
	                    inventoryMode = 1; //Crafting mode with item
						Inv_CombCraft();
						return;
	                }
	                else
	                {
	                    currentLevel.creatures[0].message.Add("Your limbs are too clumsy to make tools.");
	                    inventorySelect = 0; //Back out of menu
	                    gameState = GameState.MainGame;
	                }
	                #endregion
	                return;
	
	            case "d":
	                currentLevel.creatures[0].Drop(currentLevel, thisItem); //Drop said item	
	                inventorySelect = 0; //Back out of menu 
	                gameState = GameState.MainGame;
	                return;
	
	            case "e":
	                #region Eat Item
	                if (thisItem.name.StartsWith(currentLevel.creatures[0].name)) //Aka, goblin eating goblin corpse
	                {
	                    if (currentLevel.creatures[0].food > 2500)
	                    {
	                        currentLevel.creatures[0].message.Add("You nauseate yourself - you cannot bring yourself to eat one of your own kind");
	                        break;
	                    }
	                    else
	                    {
	                        currentLevel.creatures[0].message.Add("It really is a matter of starvation or cannibalism.");
	                    }
	                }
	
	                currentLevel.creatures[0].Eat(currentLevel, thisItem); //Eat said item	
	                inventorySelect = 0; //Back out of menu
	                gameState = GameState.MainGame;
	                #endregion
	                return;
	
	            case "f":
	                #region Fire Item
	                Vector2 targetPos = Update_GetPosition();
	
	                //currentLevel.tileArray[targetPos.X, targetPos.Y].itemList.Add(firedItem); //It ends up on selected tile
	                //currentLevel.creatureList[0].inventory.RemoveAt(inventorySelect - 1); //Remove item from inventory                                                
	                currentLevel.creatures[0].message.Add("You send the " + thisItem.name + " flying.");
	                
	                for (int i = 0; i < currentLevel.creatures.Count; i++)
	                {
	                    Creature c = currentLevel.creatures[i];                                   
	                    if (c.pos == targetPos)
	                    {                                                        
	                        currentLevel.creatures[0].RangeAttack(currentLevel, c, thisItem);
	                    }
	                }                                                
	
	                inventorySelect = 0; //Back out of menu 
	                gameState = GameState.MainGame;
	                #endregion
	                return;
	
	            case "u":
	                Inv_Main_UseItem();
	                return;
					
				case "w":
	                #region Wield Item
	                Item w;
	
	                w = currentLevel.creatures[0].inventory[inventorySelect - 1];
	                if (currentLevel.creatures[0].CanWield(w))
	                {
	                    currentLevel.creatures[0].Wield(w);
	                    gameState = GameState.MainGame;
	                }
	                else
	                {
	                    currentLevel.creatures[0].message.Add("The " + w.name + " slips from your grip");
	                }
	                
	                inventorySelect = 0; //Back out of menu
	                gameState = GameState.MainGame;
	                #endregion
	                return;
	
				case "W":
	                #region Wear Item
	                if (thisItem is Armor)
	                {
						Armor thisArmor = (Armor)thisItem;
	                    if (currentLevel.creatures[0].CanWear(thisArmor))
	                    {
	                        currentLevel.creatures[0].Wear(thisArmor);//If it's armor, wear it.
	                        gameState = GameState.MainGame;
	                    }
	                }
					else if (thisItem is Amulet)
					{
						Amulet thisAmulet = (Amulet)thisItem;
						if (player.amulet == null)
						{
							player.Wear(new Amulet(thisAmulet), currentLevel);
						}
						else
						{
							player.inventory.Add(new Amulet(player.amulet)); //Copy amulet
							player.amulet = new Amulet(thisAmulet); //Overwrite amulet
						}
					}
	                else
	                    currentLevel.creatures[0].message.Add("That's not armor.");
	
	                inventorySelect = 0; //Back out of menu
	                gameState = GameState.MainGame;
	                #endregion
					return;
	            }
			}
		}
		static void Inv_Main_UseItem()
		{
            if (!currentLevel.creatures[0].isDextrous) //Check to see if these can be used
            {
                currentLevel.creatures[0].message.Add("Your limbs are too clumsy to use this");
            }
            else if (currentLevel.creatures[0].inventory[inventorySelect - 1].use.Count > 1) //If it has multple uses
            {
                //inventoryMode = 2; //Switch to Use mode with item WIP
            }
            else if (currentLevel.creatures[0].inventory[inventorySelect - 1].use.Count == 1)
            {
                //Do the use for the item
                if (currentLevel.creatures[0].inventory[inventorySelect - 1].use[0] == "dig")
                {
                    #region Dig
                    Vector2 pos = currentLevel.creatures[0].pos;

                    if (currentLevel.tileArray[pos.X, pos.Y].hasBeenDug) //If it's been dug
                    {
                        Creature player = currentLevel.creatures[0];
                        player.message.Add("You break through the floor, and fall all the way to the next level down.");
                        player.TakeDamage(5);
                        player.message.Add("You land with a painful thud.");
                        mapPos.Z++;
                        GenLevel("dungeon", true);
                        while (!currentLevel.tileArray[player.pos.X, player.pos.Y].isPassable) //Keep going until we can move
                        {
                            player.pos.X = (short)rng.Next(1, Level.GRIDW);
                            player.pos.Y = (short)rng.Next(1, Level.GRIDH);
                        }
                        currentLevel.creatures[0] = player;
                        Item dirtChunk = new Item(100f, 100f, "dirt chunk", Color.FromArgb(157, 144, 118), new List<Item>(), new List<string>()); //Chunk of dirt
                        currentLevel.tileArray[player.pos.X, player.pos.Y].itemList.Add(new Item(dirtChunk)); //Put the dirt chunk there
                    }
                    else
                    {                                                            
                        Item dirtChunk = new Item(100f, 100f, "dirt chunk", Color.FromArgb(157, 144, 118), new List<Item>(), new List<string>()); //Chunk of dirt
                        currentLevel.tileArray[pos.X, pos.Y].itemList.Add(new Item(dirtChunk)); //Put the dirt chunk there
                        currentLevel.tileArray[pos.X, pos.Y].hasBeenDug = true; //We've dug a pit here.
                        currentLevel.creatures[0].message.Add("You dig a hole, unearthing some dirt.");
                    }
                    #endregion
                }
                else if (currentLevel.creatures[0].inventory[inventorySelect - 1].use[0] == "mine")
                {
                    #region Mine
                    currentLevel.creatures[0].message.Add("Choose a direction to dig.");
                    gameState = GameState.MainGame;
                    string input = Update_GetKey();

                    Vector2 playerPos = currentLevel.creatures[0].pos;
                    Vector2 radius = new Vector2(playerPos.X, playerPos.Y);

                    if (input == "1")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y + 1);
                    else if (input == "2")
                        radius = new Vector2(playerPos.X - 0, playerPos.Y + 1);
                    else if (input == "3")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y + 1);
                    else if (input == "4")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y - 0);
                    else if (input == "6")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y - 0);
                    else if (input == "7")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y - 1);
                    else if (input == "8")
                        radius = new Vector2(playerPos.X - 0, playerPos.Y - 1);
                    else if (input == "9")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y - 1);
                    else
                    {
                        currentLevel.creatures[0].message.Add("Dig cancelled");
                        return;
                    }
                    
                    if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.Count > 0)
                    {
                        if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Door)
                        {
                            if (rngDie.Roll(2) == 1) // 1/2 chance
                            {
                                currentLevel.creatures[0].message.Add("Your swing bounces wildly off the door");
                            }
                            else
                            {
                                currentLevel.creatures[0].message.Add("You break right through the door");
                                currentLevel.tileArray[radius.X, radius.Y].MakeOpen(); //Clear out adjacent tile
                            }
                        }
                        else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Tree)
                        {
                            if (rngDie.Roll(100) == 1) // 1% chance
                            {
                                currentLevel.creatures[0].message.Add("You somehow hit a weak point and topple the tree!");
                                Tree thisTree = (Tree)currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0];
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(500f, 500f, CapitalizeFirst(thisTree.species) + " log", Color.Brown, new List<Item>(), new List<string>()));
                                currentLevel.creatures[0].message.Add("You cut down the " + thisTree.species + " tree.");
                                currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.RemoveAt(0);
                            }
                            else // 99% chance
                            {
                                currentLevel.creatures[0].message.Add("You chip off only sawdust");
                                currentLevel.tileArray[radius.X, radius.Y].MakeOpen(); //Clear out adjacent tile
                            }
                        }
                        else
                        {
                            currentLevel.tileArray[radius.X, radius.Y].MakeOpen(); //Clear out adjacent tile
                        }
                    }
                    else
                    {
                        currentLevel.tileArray[radius.X, radius.Y].MakeOpen(); //Clear out the adjacent tile
                    }
                    #endregion
                }
                else if (currentLevel.creatures[0].inventory[inventorySelect - 1].use[0] == "tripwire")
                {
                    #region Tripwire
                    Vector2 playerPos = currentLevel.creatures[0].pos;
                    if (currentLevel.tileArray[playerPos.X, playerPos.Y].fixtureLibrary.Count <= 0)
                    {
                        Trap thisTrap = new Trap(new Effect(rngDie.Roll(5), "tripwire"));
                        thisTrap.visible = true; //The player made it, so they can see it
                        currentLevel.tileArray[playerPos.X, playerPos.Y].fixtureLibrary.Add(thisTrap);

                        currentLevel.creatures[0].inventory.RemoveAt(inventorySelect - 1); //Rope is now in trap
                        currentLevel.creatures[0].message.Add("You make a tripwire from the rope.");
                    }
                    else
                    {
                        currentLevel.creatures[0].message.Add("There is already a " +
                            currentLevel.tileArray[playerPos.X, playerPos.Y].fixtureLibrary[0].type + " here.");
                    }
                    #endregion
                }
                else if (currentLevel.creatures[0].inventory[inventorySelect - 1].use[0] == "chop")
                {
                    #region Chop
                    currentLevel.creatures[0].message.Add("Choose a direction to chop.");
                    gameState = GameState.MainGame;
                    string input = Update_GetKey();

                    Vector2 playerPos = currentLevel.creatures[0].pos;
                    Vector2 radius = new Vector2(playerPos.X, playerPos.Y);

                    if (input == "1")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y + 1);
                    else if (input == "2")
                        radius = new Vector2(playerPos.X - 0, playerPos.Y + 1);
                    else if (input == "3")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y + 1);
                    else if (input == "4")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y - 0);
                    else if (input == "6")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y - 0);
                    else if (input == "7")
                        radius = new Vector2(playerPos.X - 1, playerPos.Y - 1);
                    else if (input == "8")
                        radius = new Vector2(playerPos.X - 0, playerPos.Y - 1);
                    else if (input == "9")
                        radius = new Vector2(playerPos.X + 1, playerPos.Y - 1);
                    else
                    {
                        currentLevel.creatures[0].message.Add("Chop cancelled");
                        return;
                    }

                    bool creatureThere = false;
                    foreach (Creature c in currentLevel.creatures)
                    {
                        if (c.pos == radius) //If a creature is there.
                        {
                            creatureThere = true;
                        }
                    }                                                        

                    if (creatureThere) //Stupid foreach limitations
                        currentLevel.creatures[0].MeleeAttack(currentLevel, Direction.Parse(input));
                    else
                    {
                        if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.Count > 0)
                        {
                            if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Tree)
                            {
                                Tree thisTree = (Tree)currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0];
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(500f, 500f, CapitalizeFirst(thisTree.species) + " log", Color.Brown, new List<Item>(), new List<string>()));
                                currentLevel.creatures[0].message.Add("You cut down the " + thisTree.species + " tree.");
                                currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.RemoveAt(0);
                            }
                            else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Door)
                            {                                
								Item stick = new Item(100f, 100f, "stick", Color.Brown, new List<Item>(), new List<string>());

                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));

                                currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.RemoveAt(0);
                                currentLevel.tileArray[radius.X, radius.Y].isPassable = true;
                                currentLevel.tileArray[radius.X, radius.Y].isTransparent = true;
                                currentLevel.tileArray[radius.X, radius.Y].isDoor = false;

                                currentLevel.creatures[0].message.Add("You chop the door to pieces");
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                currentLevel.creatures[0].message.Add("You can't think of an obvious use for it at the moment.");
            }
            gameState = GameState.MainGame;
            inventorySelect = 0;
            inventoryMode = 0;
		}
		static void Inv_CombCraft()
		{
			Draw();
            while(true)
			{
				string input = Update_GetKey();
				switch(input)
				{
				case "Escape":
				case "Space":
				case "Backspace":
					gameState = GameState.MainGame;
					inventoryMode = 0;
					inventorySelect = 0;
					return;
				}
				int selection = LetterIndexToNumber(input);
				
	            Queue<Item> items = new Queue<Item>();
	            foreach (Item item in craftableItems) //Get a queue of all the craftable items
	            {
	                items.Enqueue(item);
	
	                if (items.Count >= 24)
	                {
	                    items.Dequeue(); //Don't let there be more than 24 in the queue
	                }
	            }
	
	            int count = items.Count;
	            for (int c = 1; c <= count; c++) //Pretty much a foreach, but allows deletion
	            {
	                if (c == selection) //If we've selected this item
	                {
	                    currentLevel.creatures[0].CombineItem(currentLevel.creatures[0].inventory[inventorySelect - 1],
	                        items.Dequeue());
	
	                    inventorySelect = 0; //Back out of menu
	                    inventoryMode = 0;
	                    gameState = GameState.MainGame;
						return;
	                }
	                else
	                {
	                    items.Dequeue();
	                }
	            }
	            items.Clear();
			}
		}
		static void Inv_Use()
		{
			
		}
		
		static bool Update_Player(Creature c)
		{
            if (currentLevel.playerDiedHere)
            {				
				if (c.revive > 0) //If extra life
				{
					c.revive = 0; //Not anymore, sukka
					if (c.amulet.effect.type == "revive")
					{
						c.message.Add("Your " + c.amulet.name + " disintegrates.");
						c.amulet = null;
					}
					c.hp = c.hpMax; //Full heal
					c.food = 15000; //Full food
					c.message.Add("You have a near death experience");
					c.constitution--;	
					currentLevel.playerDiedHere = false;
				}
				else
				{
	                c.message.Add("You have died from " + currentLevel.causeOfDeath);
	                c.message.Add("This happened because " + currentLevel.mannerOfDeath);
	                c.message.Add("You have slain " + currentLevel.creatures[0].killCount + " foes");
	                c.message.Add("You lived to see " + exploredLevels + " areas in your world");
	
	                Draw();
	                while (Update_GetKey() != "Enter") { }; //Wait for Enter
	
	                FileD_World();
	
	                run = false;
	                gameState = GameState.OpeningMenu;
	                return false;
				}
            }

            bool wait = true;
            Vector2 radius;
            while (wait)
            {
                Draw();
                string input = Update_GetKey();

                #region Process Input
                switch (input)
                {
                case "1":
                case "2":
                case "3":
                case "4":
                case "6":
                case "7":
                case "8":
                case "9":
					return Update_Move(currentLevel.creatures[0], 
                                       Direction.Parse(input));
					
				case "5":
				case ".":
				case "s":
					return true; //Wait a turn

                case ">":
                    #region [S] Adventurer: Descend
                    if (currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary.Count > 0)
                    { //If there's a fixture here
                        if (currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary[0] is Stairs)
                        { //If that fixture is stairs
                            Stairs stairs = (Stairs)currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary[0]; //Stairs
                            if (stairs.isDown)
                            { //If those stairs are down stairs
                                mapPos.Z++; //Go down a level
								c = new Creature(c); //Break reference link
                                GenLevel("dungeon", true);
                                c.pos = currentLevel.creatures[0].pos; //Place the player at the new up stairs position
                                currentLevel.creatures[0] = c;
                                wait = true;
                            }
                            else
                            {
                                c.message.Add("How does one go down up stairs?");
                            }
                        }
                    }
                    break;
                    #endregion

                case "<":
                    #region [S] Adventurer: Ascend
                    if (currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary.Count > 0)
                    {
                        if (currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary[0] is Stairs)
                        {
                            Stairs stairs = (Stairs)currentLevel.tileArray[c.pos.X, c.pos.Y].fixtureLibrary[0]; //Stairs
                            if (!stairs.isDown)
                            {
                                mapPos.Z--; //Go up a level
								c = new Creature(c); //Break reference link
                                if (mapPos.Z < 1)
                                {
                                    Random thisLevelRNG = new Random(levelSeed[mapPos.X, mapPos.Y, mapPos.Z]); //This level's generator
                                    if (thisLevelRNG.Next(0, 100) < 20)
                                    {
                                        GenLevel("village", true);
                                    }
                                    else
                                    {
                                        GenLevel("forest", true);
                                    }
                                }
                                else
                                    GenLevel("dungeon", true);
                                for (int y = 0; y < Level.GRIDH; y++)
                                    for (int x = 0; x < Level.GRIDW; x++)
                                        if (currentLevel.tileArray[x, y].fixtureLibrary.Count > 0)
                                            if (currentLevel.tileArray[x, y].fixtureLibrary[0].type == "stairs")
                                            {
                                                stairs = (Stairs)currentLevel.tileArray[x, y].fixtureLibrary[0];
                                                if (stairs.isDown)
                                                    c.pos = new Vector2(x, y); //Place on down stairs
                                            }

                                currentLevel.creatures[0] = c;
                            }
                            else
                            {
                                c.message.Add("How does one go up down stairs?");
                            }
                        }
                    }
                    break;
                    #endregion

                case "Escape":
                    gameState = GameState.EscapeMenu;
                    return false;

                case ",":
                    #region Pick Up
                    c.PickUp(currentLevel);

                    while (c.inventory.Count > 25)
                    {
                        c.message.Add("You drop your " + c.inventory[25] + " to make room.");
                        c.Drop(currentLevel, c.inventory[25]); //Drop item
                    }

                    wait = false;
                    break;
                    #endregion

                case "c":
                    #region Close
                    Vector2[] position = new Vector2[10];
                    position[1] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y + 1);
                    position[2] = new Vector2((int)c.pos.X    , (int)c.pos.Y + 1);
                    position[3] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y + 1);
                    position[4] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y    );
                    position[6] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y    );
                    position[7] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y - 1);
                    position[8] = new Vector2((int)c.pos.X    , (int)c.pos.Y - 1);
                    position[9] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y - 1);

                    for (int dir = 1; dir <= 9; dir++)
                    {
                        if (dir == 5)
                            dir++; //Skip 5
                        foreach (Fixture f in currentLevel.tileArray[(int)position[dir].X, (int)position[dir].Y].fixtureLibrary)
                        {
                            if (f.type == "door")
                            {
                                Door door = (Door)f;
                                if (door.isOpen)
                                {
                                    c.CloseDoor(currentLevel.tileArray[
                                        (int)position[dir].X, (int)position[dir].Y], currentLevel);
                                }
                            }
                        }
                    }
                    break;
                    #endregion

                case "e":
                    currentLevel.tileArray[c.pos.X, c.pos.Y].engraving = Update_GetString(); //Engrave
                    wait = false;
                    break;

                case "h":
                    #region Hax dig
                    if (debugMode)
                    {
                        c.message.Add("Choose a direction to dig.");
                        string s = Update_GetKey();

                        radius = new Vector2(c.pos.X, c.pos.Y);

                        if (s == "1")
                            radius = new Vector2(c.pos.X - 5, c.pos.Y + 5);
                        else if (s == "2")
                            radius = new Vector2(c.pos.X - 0, c.pos.Y + 5);
                        else if (s == "3")
                            radius = new Vector2(c.pos.X + 5, c.pos.Y + 5);
                        else if (s == "4")
                            radius = new Vector2(c.pos.X - 5, c.pos.Y - 0);
                        else if (s == "6")
                            radius = new Vector2(c.pos.X + 5, c.pos.Y - 0);
                        else if (s == "7")
                            radius = new Vector2(c.pos.X - 5, c.pos.Y - 5);
                        else if (s == "8")
                            radius = new Vector2(c.pos.X - 0, c.pos.Y - 5);
                        else if (s == "9")
                            radius = new Vector2(c.pos.X + 5, c.pos.Y - 5);
                        else
                        {
                            c.message.Add("Dig cancelled");
                            break;
                        }

                        c.message.Add("You release a blast of unnatural energy, tearing through the walls of the dungeon.");
                        currentLevel.DigLine(c.pos, radius);
                        wait = false;
                    }
                    wait = true;
                    break;
                    #endregion

                case "i":
                    gameState = GameState.InventoryMenu; //Gamestate is now the Inventory Menu
                    return false;

                case "k":
                    #region Kick/Break
                    c.message.Add("Choose a direction.");
                    input = Update_GetKey();

                    radius = new Vector2(c.pos.X, c.pos.Y);

                    if (input == "1")
                        radius = new Vector2(c.pos.X - 1, c.pos.Y + 1);
                    else if (input == "2")
                        radius = new Vector2(c.pos.X - 0, c.pos.Y + 1);
                    else if (input == "3")
                        radius = new Vector2(c.pos.X + 1, c.pos.Y + 1);
                    else if (input == "4")
                        radius = new Vector2(c.pos.X - 1, c.pos.Y - 0);
                    else if (input == "6")
                        radius = new Vector2(c.pos.X + 1, c.pos.Y - 0);
                    else if (input == "7")
                        radius = new Vector2(c.pos.X - 1, c.pos.Y - 1);
                    else if (input == "8")
                        radius = new Vector2(c.pos.X - 0, c.pos.Y - 1);
                    else if (input == "9")
                        radius = new Vector2(c.pos.X + 1, c.pos.Y - 1);
                    else
                    {
                        c.message.Add("Cancelled");
                        break;
                    }

                    bool creatureThere = false;

                    foreach (Creature d in currentLevel.creatures)
                    {
                        if (d.pos == radius) //If a creature is there.
                        {
                            creatureThere = true;
                        }
                    }

                    if (creatureThere) //Stupid foreach limitations
                    {
                        c.MeleeAttack(currentLevel, Direction.Parse(input));
                    }
                    else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.Count > 0)
                    {
                        if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Door)
                        {
                            if (rngDie.Roll(2) == 1) // 1/2 chance
                            {
                                Item stick = new Item(100f, 100f, "stick", Color.Brown, new List<Item>(), new List<string>());

                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));
                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(stick));

                                currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.RemoveAt(0);
                                currentLevel.tileArray[radius.X, radius.Y].isPassable = true;
                                currentLevel.tileArray[radius.X, radius.Y].isTransparent = true;
                                currentLevel.tileArray[radius.X, radius.Y].isDoor = false;

                                c.message.Add("The door splinters apart.");
                            }
                            else
                            {
                                c.message.Add("The door thuds.");
                            }
                        }
                        else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Trap)
                        {
                            Trap thisTrap = (Trap)currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0];

                            if (thisTrap.effect.type == "tripwire")
                            {
                                Item rope = new Item(100f, 100f, "rope", Color.Wheat, new List<Item>(), new List<string>());

                                foreach (Item t in content.items)
                                {
                                    if (t.name == "rope")
                                    {
                                        rope = new Item(t); //Copy an actual rope if possible
                                    }
                                }

                                currentLevel.tileArray[radius.X, radius.Y].itemList.Add(new Item(rope));

                                c.message.Add("You take apart the tripwire.");

                                currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary.RemoveAt(0);
                            }
                        }
                        else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Stairs)
                        {
                            c.TakeDamage(1); //Ow.
                            c.message.Add("You kick the hard stairs and hurt your leg.");
                        }
                        else if (currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0] is Tree)
                        {
                            Tree thisTree = (Tree)currentLevel.tileArray[radius.X, radius.Y].fixtureLibrary[0];

                            if (rng.Next(1, 101) > 50) //50% chance
                            {
                                c.TakeDamage(1); //Ow.
                                c.message.Add("You hurt your leg kicking the tree.");
                            }
                            else if (thisTree.fruit != String.Empty && rng.Next(1, 101) > 50) //If it has fruit, 50% chance
                            {
                                currentLevel.tileArray[c.pos.X, c.pos.Y].itemList.Add(new Item(100f, 100f, thisTree.fruit, Color.Lime, new List<Item>(), new List<string>())); //Add a fruit
                                c.message.Add("A " + thisTree.fruit + " drops at your feet.");
                                thisTree.fruit = String.Empty; //No more fruit
                            }
                        }
                    }
                    else
                    {
                        c.message.Add("You kick at nothing.");
                    }
                    break;
                    #endregion

                case "l":
                    if (debugMode)
                    {
                        if (iCanSeeForever)
                            iCanSeeForever = false;
                        else
                            iCanSeeForever = true;
                    }
                    return false;

                case "o":
                    #region Open
                    position = new Vector2[10];
                    position[1] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y + 1);
                    position[2] = new Vector2((int)c.pos.X    , (int)c.pos.Y + 1);
                    position[3] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y + 1);
                    position[4] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y    );
                    position[6] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y    );
                    position[7] = new Vector2((int)c.pos.X - 1, (int)c.pos.Y - 1);
                    position[8] = new Vector2((int)c.pos.X    , (int)c.pos.Y - 1);
                    position[9] = new Vector2((int)c.pos.X + 1, (int)c.pos.Y - 1);

                    for (int dir = 1; dir <= 9; dir++)
                    {
                        if (dir == 5)
                            dir++; //Skip 5
                        foreach (Fixture f in currentLevel.tileArray[(int)position[dir].X, (int)position[dir].Y].fixtureLibrary)
                        {
                            if (f.type == "door")
                            {
                                Door door = (Door)f;
                                if (!door.isOpen)
                                {
                                    c.OpenDoor(currentLevel.tileArray[(int)position[dir].X, (int)position[dir].Y], currentLevel);
                                }
                            }
                        }
                    }
                    break;
                    #endregion

                case "v":
                    #region Hax Dive
                    if (debugMode)
                    {
						c = new Creature(c); //Break reference link
                        c.message.Add("You warp through the floor");
                        mapPos.Z++;//Go down a level
                        GenLevel("dungeon", true);
                        c.pos = currentLevel.creatures[0].pos; //Place the player at the new up stairs position
                        currentLevel.creatures[0] = c;
                        c.message.Add("Now entering area (" + mapPos.X + ", " + mapPos.Y + ", " + mapPos.Z + ")");
                    }
                    break;
                    #endregion

                case "w":
                    if (c.weapon == null)
                        c.message.Add("You are wielding nothing.");
                    else
                    {
                        c.Unwield();
                        wait = false;
                    }
                    break;

                case "W":
                    if (c.wornArmor.Count > 0)
                    {
                        c.RemoveAll();
                        wait = true;
                    }
                    else
                        c.message.Add("You are wearing no armor.");
                    break;

                case "x":
                    Update_GetPosition(); //Retrieve a position
                    return false;

                case "X":
                    if (debugMode) //Toggle debug mode
                    {
                        debugMode = false;
                        c.message.Add("You feel the unnatural energy fade.");
                    }
                    else
                    {
                        debugMode = true;
                        c.message.Add("You call upon the power of Kalasen.");
                    }

                    iCanSeeForever = false; //Disable infinisight
                    return false;

                case "z":
                    gameState = GameState.HealthMenu;
                    return false;
					
				default:
					return false; //If no other appropriate key is pressed, don't use a turn
                }
                #endregion
            }
			
			return true;
		} //Returns whether the turn was spent
		static void Update_Creature(Creature c)
		{
            c.message.Clear(); //Monsters shouldn't need to keep messages
            string action = c.mind.DecideAction(currentLevel, c); //Decide action

            #region Perform Action

            #region Move Actions
            List<String> actionList = new List<String>();
            actionList.Add("Move 1");
            actionList.Add("Move 2");
            actionList.Add("Move 3");
            actionList.Add("Move 4");
            actionList.Add("Move 5");
            actionList.Add("Move 6");
            actionList.Add("Move 7");
            actionList.Add("Move 8");
            actionList.Add("Move 9");

            for (int k = 0; k <= 8; k++)
            {
                if (action == actionList[k])
                {
                    c.Move(currentLevel, k + 1);
                }
            }

            actionList.Clear();
            #endregion

            #region Item Management Actions
            if (action == "Pick Up")
            {
                currentLevel = c.PickUp(currentLevel);
            }

            if (action == "Unwield")
                c.Unwield();            

            if (action.StartsWith("Wield"))
            {
                while (action.StartsWith("Wield"))
                    action = action.Remove(0, 6); //Clip everything but the index
                
                Weapon targetItem = (Weapon)c.inventory[int.Parse(action)];
                c.Wield(targetItem);
            }

            if (action == "Remove")            
                c.RemoveAll();            

            if (action.StartsWith("Wear"))
            {
                while (action.StartsWith("Wear"))
                    action = action.Remove(0, 5); //Clip everything but the index
                
                Armor targetItem = (Armor)c.inventory[int.Parse(action)];
                c.Wear(targetItem);
            }

            if (action.StartsWith("Eat")) //If it starts with "Eat"
            {
                action = action.Remove(0, 4); //Clip everything but the index
                Item targetItem = c.inventory[int.Parse(action)];
                c.Eat(currentLevel, targetItem); //Eat the given item
            }
            #endregion

            #region Melee Attack
            if (action.StartsWith("Attack"))
            {
                action = action.Remove(0, 7);
                currentLevel = c.MeleeAttack(currentLevel, Direction.Parse(action));
            }
            #endregion
            #endregion	
		}
		static void Update_World()
		{
			ticks++;
            if (ticks % (TURN_THRESHOLD / 12) == 0) //If we're in time with the average turn
            {
				ticks = 0;
                totalTurnCount++;
				
				foreach (Creature c in currentLevel.creatures)
					c.CycleWithWorld(currentLevel);
                if ((int)totalTurnCount % 42 == 0) //Every 42 turns
                {
                    if (rng.Next(1, currentLevel.creatures.Count) == 1 && currentLevel.levelType != "village") //With a percentage inversely proportional to already existing creatures
                    {
                        currentLevel.SpawnCreature(false, "monster"); //Spawn new creature
                    }
                }
			}
		}
		static void Update_PostTurn(Creature c)
		{
			c.Wait(currentLevel);
			
            if (c.message.Count > 50)
                c.message.RemoveRange(0, c.message.Count - 50); //Toss excess messages

            #region Check if a creature should be dead
            int killedIndex = 0;

            if (c.ShouldBeDead(currentLevel))
            {
				currentLevel.SlayCreature(c);
            }
            
            if (killedIndex > 0)
                currentLevel.creatures.RemoveAt(killedIndex);
            #endregion
		}
		static void Update_Hunger(Creature c)
		{
            c.food -= 2; //Hunger

            if (c.food < 0) //If starving
            {
                c.hp--;
                currentLevel.causeOfDeath = "organ failure.";
                currentLevel.mannerOfDeath = "you were starving.";
            }
		}
		static void Update_Smell(Creature c)
		{
            int x = (int)c.pos.X;
            int y = (int)c.pos.Y;
            string smelledWhat = String.Empty;
            for (int i = 0; i < currentLevel.tileArray[x, y].scentMagnitude.Count; i++)
                if (currentLevel.tileArray[x, y].scentMagnitude[i] > c.senseOfSmell)
                {
                    smelledWhat = currentLevel.tileArray[x, y].scentIdentifier[i];
                }
            if (smelledWhat != String.Empty)
            {
                bool seeSmelled = false;
                bool ownSmell = false;
                for (int i = 1; i < currentLevel.creatures.Count; i++)
                {
                    if (currentLevel.LineOfSight(c.pos,
                        currentLevel.creatures[i].pos) && currentLevel.creatures[i].name == smelledWhat)
                    {
                        seeSmelled = true; //It's the same as something seen
                    }

                    if (smelledWhat == c.name)
                        ownSmell = true; //It's the creature's own smell                    
                }

                if (!seeSmelled && !ownSmell) //If it's an un-obvious smell
                {
                    int count = c.message.Count;
                    if (count > 0)
                    {
                        //if (currentLevel.creatureList[n].message[count - 1] != "You smell a " + smelledWhat + ".") //Don't spam this
                        //    currentLevel.creatureList[n].message.Add("You smell a " + smelledWhat + ".");
                    }
                }
            }
		}
		static bool Update_Move(Creature c, Directions dir)
		{
            bool peacefulAdjacent = false; //Whether the adjacent creature, if any, is peaceful

            foreach (Creature d in currentLevel.creatures)
            {
                if (d is QuestGiver && d.pos == c.pos.AdjacentVector(dir))
                {
                    QuestGiver qG = (QuestGiver)d;
                    bool haveItem = false;
                    peacefulAdjacent = true;
                    c.message.Add("You chat with the " + d.name + ".");

                    int inventoryCount = c.inventory.Count; //Bluh foreach
                    for (int itemIndex = 0; itemIndex < inventoryCount; itemIndex++)
                    {
                        Item item = c.inventory[itemIndex]; //There, foreach simulated
                        if (item.name == qG.wantObject)
                        {
                            haveItem = true;
                            c.message.Add(CapitalizeFirst(d.name) + ": I see you have a " + qG.wantObject + ". Trade for a " + qG.giveObject + "? (y/n)");
                            Draw(); //Draw this to the screen
                            if (Update_GetKey() == "y")
                            {
                                d.inventory.Add(item); //Give away the wanted item
                                c.inventory.Remove(item);

                                int cInventoryCount = c.inventory.Count; //Bluh foreach
                                for (itemIndex = 0; itemIndex < cInventoryCount; itemIndex++)
                                {
                                    Item cItem = d.inventory[itemIndex];
                                    c.inventory.Add(cItem); //Recieve giveObject
                                    d.inventory.Remove(cItem);
                                }
                                foreach (Item cItem in d.inventory)
                                {
                                    if (cItem.name == qG.giveObject)
                                    {
                                        c.inventory.Add(cItem);
                                    }
                                }

                                qG.CycleWantGiveItem(content.items); //Cycle what s/he wants and what he will give

                                c.message.Add(CapitalizeFirst(d.name) + ": You trade your items.");
                            }
                            else
                            {
                                c.message.Add(CapitalizeFirst(d.name) + ": Too bad. Come back if you change your mind.");
                            }
                            break;
                        }
                    }

                    if (!haveItem) //If we don't have the item
                    {
                        c.message.Add(CapitalizeFirst(d.name) + ": Hello adventurer. If you bring me a " + qG.wantObject + ", I'll give you a " + qG.giveObject + ".");
                    }
					
					return true; //Talking is not a free action
                }
            }

            if (!peacefulAdjacent)
            {            
	            if (c.CanAttackMelee(currentLevel, dir) && !peacefulAdjacent)
				{
	                currentLevel = c.MeleeAttack(currentLevel, dir);
					return true;
				}
	            else if (!currentLevel.MoveWillBeBlocked(0, dir))
	            {
	                bool moved = c.Move(currentLevel, (int)dir);
					Update_MapEdge(c); //Check for hitting map edge
					return moved;
	            }
			}
			
			return false;
		} //Returns whether a turn was spent
		static void Update_MapEdge(Creature c)
		{
            if (c.pos.X <= 1)
            {
                Creature player = new Creature(c); //Grab copy of player
                mapPos.X--; //We've gone to the left
                if (mapPos.X == -1)
                {
                    mapPos.X = 99; //Loop around
                }

                Random thisLevelRNG = new Random(levelSeed[mapPos.X, mapPos.Y, mapPos.Z]); //This level's generator
                if (thisLevelRNG.Next(0, 100) < 30)
                {
                    GenLevel("village", true);
                }
                else
                {
                    GenLevel("forest", true);
                }

                player.pos.X = Level.GRIDW - 2; //Now on other end of map
                currentLevel.creatures[0] = player; //Creature 0 is the player                                                                        
            }
            else if (c.pos.X >= Level.GRIDW - 1)
            {
                Creature player = new Creature(c); //Grab copy of player
                mapPos.X++; //We've gone to the left
                if (mapPos.X == 100)
                {
                    mapPos.X = 0; //Loop around
                }

                Random thisLevelRNG = new Random(levelSeed[mapPos.X, mapPos.Y, mapPos.Z]); //This level's generator
                if (thisLevelRNG.Next(0, 100) < 30)
                {
                    GenLevel("village", true);
                }
                else
                {
                    GenLevel("forest", true);
                }
                player.pos.X = 2; //Now on other end of map
                currentLevel.creatures[0] = player; //Creature 0 is the player                                    
            }
            else if (c.pos.Y <= 1)
            {
                Creature player = new Creature(c); //Grab copy of player
                mapPos.Y--; //We've gone to the left
                if (mapPos.Y == -1)
                {
                    mapPos.Y = 99; //Loop around
                }

                Random thisLevelRNG = new Random(levelSeed[mapPos.X, mapPos.Y, mapPos.Z]); //This level's generator
                if (thisLevelRNG.Next(0, 100) < 30)
                {
                    GenLevel("village", true);
                }
                else
                {
                    GenLevel("forest", true);
                }

                player.pos.Y = Level.GRIDH - 2; //Now on other end of map
                currentLevel.creatures[0] = player; //Creature 0 is the player                                    
            }
            else if (c.pos.Y >= Level.GRIDH - 1)
            {
                Creature player = new Creature(c); //Grab copy of player
                mapPos.Y++; //We've gone to the left
                if (mapPos.X == 100)
                {
                    mapPos.X = 0; //Loop around
                }

                Random thisLevelRNG = new Random(levelSeed[mapPos.X, mapPos.Y, mapPos.Z]); //This level's generator
                if (thisLevelRNG.Next(0, 100) < 30)
                {
                    GenLevel("village", true);
                }
                else
                {
                    GenLevel("forest", true);
                }

                player.pos.Y = 2; //Now on other end of map
                currentLevel.creatures[0] = player; //Creature 0 is the player                                    
            }
		}

        static string Update_GetKey()
        {
            Sdl.SDL_Event keyEvent, oldKeyEvent;
            Sdl.SDL_PollEvent(out keyEvent);

            while (keyEvent.type == Sdl.SDL_KEYDOWN)
            {
                Sdl.SDL_PollEvent(out keyEvent);
            }
            oldKeyEvent = keyEvent;
            //Draw();

            while (true)
            {
                Sdl.SDL_PollEvent(out keyEvent);
                Thread.Sleep(1);

                switch (keyEvent.type) //Test keyEvent
                {
                    //case 17: //Unknown, happens during keyboard freeze
                    case Sdl.SDL_KEYDOWN: //If a key is down
                        switch (keyEvent.key.keysym.sym)
                        {
                            #region Special
                            case Sdl.SDLK_ESCAPE:
                                return "Escape";

                            case Sdl.SDLK_PERIOD:
                                if (shift)
                                    return ">";
                                return ".";

                            case Sdl.SDLK_COMMA:
                                if (shift)
                                    return "<";
                                return ",";

                            case Sdl.SDLK_SPACE:
                                return "Space";

                            case Sdl.SDLK_BACKSPACE:
                                return "Backspace";

                            case Sdl.SDLK_RSHIFT:
                            case Sdl.SDLK_LSHIFT:
                                {
                                    shift = true; //Shift is down
                                    break;
                                }

                            case 271: //If enter aka "271" or "13" was pressed
                            case 13:
                                return "Enter";
                            #endregion

                            #region Numbers
                            case Sdl.SDLK_KP1:
                            case Sdl.SDLK_DELETE:
                                return "1";

                            case Sdl.SDLK_KP2:
                            case Sdl.SDLK_DOWN:
                                return "2";

                            case Sdl.SDLK_KP3:
                            case Sdl.SDLK_PAGEDOWN:
                                return "3";

                            case Sdl.SDLK_KP4:
                            case Sdl.SDLK_LEFT:
                                return "4";

                            case Sdl.SDLK_KP5:
                                return "5";

                            case Sdl.SDLK_KP6:
                            case Sdl.SDLK_RIGHT:
                                return "6";

                            case Sdl.SDLK_KP7:
                            case Sdl.SDLK_INSERT:
                                return "7";

                            case Sdl.SDLK_KP8:
                            case Sdl.SDLK_UP:
                                return "8";

                            case Sdl.SDLK_KP9:
                            case Sdl.SDLK_PAGEUP:
                                return "9";
                            #endregion

                            #region Letters
                            case Sdl.SDLK_a:
                                if (shift)
                                    return "A";
                                return "a";

                            case Sdl.SDLK_b:
                                if (shift)
                                    return "B";
                                return "b";

                            case Sdl.SDLK_c:
                                if (shift)
                                    return "C";
                                return "c";

                            case Sdl.SDLK_d:
                                if (shift)
                                    return "D";
                                return "d";

                            case Sdl.SDLK_e:
                                if (shift)
                                    return "E";
                                return "e";

                            case Sdl.SDLK_f:
                                if (shift)
                                    return "F";
                                return "f";

                            case Sdl.SDLK_g:
                                if (shift)
                                    return "G";
                                return "g";

                            case Sdl.SDLK_h:
                                if (shift)
                                    return "H";
                                return "h";

                            case Sdl.SDLK_i:
                                if (shift)
                                    return "I";
                                return "i";

                            case Sdl.SDLK_j:
                                if (shift)
                                    return "J";
                                return "j";

                            case Sdl.SDLK_k:
                                if (shift)
                                    return "K";
                                return "k";

                            case Sdl.SDLK_l:
                                if (shift)
                                    return "L";
                                return "l";

                            case Sdl.SDLK_m:
                                if (shift)
                                    return "M";
                                return "m";

                            case Sdl.SDLK_n:
                                if (shift)
                                    return "N";
                                return "n";

                            case Sdl.SDLK_o:
                                if (shift)
                                    return "O";
                                return "o";

                            case Sdl.SDLK_p:
                                if (shift)
                                    return "P";
                                return "p";

                            case Sdl.SDLK_q:
                                if (shift)
                                    return "Q";
                                return "q";

                            case Sdl.SDLK_r:
                                if (shift)
                                    return "R";
                                return "r";

                            case Sdl.SDLK_s:
                                if (shift)
                                    return "S";
                                return "s";

                            case Sdl.SDLK_t:
                                if (shift)
                                    return "T";
                                return "t";

                            case Sdl.SDLK_u:
                                if (shift)
                                    return "U";
                                return "u";

                            case Sdl.SDLK_v:
                                if (shift)
                                    return "V";
                                return "v";

                            case Sdl.SDLK_w:
                                if (shift)
                                    return "W";
                                return "w";

                            case Sdl.SDLK_x:
                                if (shift)
                                    return "X";
                                return "x";

                            case Sdl.SDLK_y:
                                if (shift)
                                    return "Y";
                                return "y";

                            case Sdl.SDLK_z:
                                if (shift)
                                    return "Z";
                                return "z";
                            #endregion

                            default:
                                break;
                        }

                        break;

                    case Sdl.SDL_KEYUP:
                        switch (keyEvent.key.keysym.sym)
                        {
                            case Sdl.SDLK_LSHIFT:
                            case Sdl.SDLK_RSHIFT:
                                shift = false;
                                break;
                        }
                        break;

                    default:
                        break;
                }

                oldKeyEvent.type = keyEvent.type; //Update this
            }
        } //Hold everything and just wait for the user to press a key
        static string Update_GetString()
        {
            string input = String.Empty;
            string returnString = String.Empty;

            currentLevel.creatures[0].message.Add(returnString);

            while (input != "Enter" && input != "Escape" && input != "Backspace")
            {
                if (input == "Space")
                {
                    returnString += " ";
                }
                //else if (input == "Backspace")
                //{
                //    if (returnString.Length > 1) //If there's a letter to remove
                //        returnString = returnString.Remove(input.Length - 1); //Remove last letter
                //}
                else
                {
                    returnString += CapitalizeFirst(input); //If not a special case, add the letter
                }

                currentLevel.creatures[0].message[currentLevel.creatures[0].message.Count-1] = "~" + returnString; //Update the single message
                Draw();
                input = Update_GetKey();
            }

            return returnString;
        }
        static void Update_GetSessionStr()
        {
            string input = String.Empty;

            while (input != "Enter" && input != "Escape" && input != "Backspace")
            {               
                if (input == "Space")
                {
                    sessionName += " ";
                }
                //else if (input == "Backspace")
                //{
                //    if (returnString.Length > 1) //If there's a letter to remove
                //        returnString = returnString.Remove(input.Length - 1); //Remove last letter
                //}
                else
                {
                    sessionName += input; //If not a special case, add the letter
                }

                Draw();
                input = Update_GetKey();
            }            
        }
        static Vector2 Update_GetPosition()
        {
            gameState = GameState.WaitForPosition;
            cursorPos = currentLevel.creatures[0].pos; //Start at player's position
            Vector2 currentPos = currentLevel.creatures[0].pos; //The position the cursor is on
            bool done = false; //Variable for loop break

            Sdl.SDL_Event keyEvent, oldKeyEvent;
            Sdl.SDL_PollEvent(out keyEvent);            

            while (!done)
            {
                oldKeyEvent = keyEvent;
                Sdl.SDL_PollEvent(out keyEvent);
                Draw(); //Update screen

                switch (keyEvent.type) //Test keyEvent
                {
                    case Sdl.SDL_KEYDOWN: //If a key is down
                        if (oldKeyEvent.type != Sdl.SDL_KEYDOWN ||
                            oldKeyEvent.key.keysym.sym == Sdl.SDLK_RSHIFT ||
                            oldKeyEvent.key.keysym.sym == Sdl.SDLK_LSHIFT)
                        {
                            switch (keyEvent.key.keysym.sym)
                            {
                                case Sdl.SDLK_ESCAPE:
                                    gameState = GameState.MainGame;
                                    done = true;                                    
                                    break;

                                case 271: //If enter aka "271" or "13" was pressed
                                case 13:
                                    gameState = GameState.MainGame;
                                    return cursorPos;

                                case Sdl.SDLK_KP1:
                                case Sdl.SDLK_DELETE:
                                    cursorPos.X--;
                                    cursorPos.Y++;
                                    break;

                                case Sdl.SDLK_KP2:
                                case Sdl.SDLK_DOWN:
                                    cursorPos.Y++;
                                    break;

                                case Sdl.SDLK_KP3:
                                case Sdl.SDLK_PAGEDOWN:
                                    cursorPos.X++;
                                    cursorPos.Y++;
                                    break;

                                case Sdl.SDLK_KP4:
                                case Sdl.SDLK_LEFT:
                                    cursorPos.X--;
                                    break;

                                case Sdl.SDLK_KP6:
                                case Sdl.SDLK_RIGHT:
                                    cursorPos.X++;
                                    break;

                                case Sdl.SDLK_KP7:
                                case Sdl.SDLK_INSERT:
                                    cursorPos.X--;
                                    cursorPos.Y--;
                                    break;

                                case Sdl.SDLK_KP8:
                                case Sdl.SDLK_UP:
                                    cursorPos.Y--;
                                    break;

                                case Sdl.SDLK_KP9:
                                case Sdl.SDLK_PAGEUP:
                                    cursorPos.X++;
                                    cursorPos.Y--;
                                    break;

                                default:
                                    done = false;
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return currentPos;
        }
	}
}