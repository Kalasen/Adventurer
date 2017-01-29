using System; //General C# functions
using System.Linq; //Enable Queue reversing
using System.Collections.Generic; //So I can use List
using System.Drawing; //The colors, man
using System.Runtime.InteropServices; //For use in wrangling pointers in their place
using Tao.Sdl;
using KalaGame;

namespace Adventurer
{
	public partial class Adventurer
	{
		#region Variables
		public const byte TILEWIDTH = 16; //The width of tiles in pixels
        public const byte TILEHEIGHT = 16; //The height of tiles in pixels
		
		public static Sdl.SDL_Surface screenData; //The screen bitmap        
        public static Sdl.SDL_Surface[] imageData = new Sdl.SDL_Surface[256]; //The image bitmaps
        static IntPtr screen; //Pointer to screen bitmap
        public static IntPtr[] image = new IntPtr[256]; //Pointer to image data
        public static Sdl.SDL_Rect screenArea; //The area of the screen
        public static Sdl.SDL_Rect source; //A source rectangle to pull an image from
        public static Sdl.SDL_Rect target; //A target rectangle for where to draw it to
        public static int windowSizeX = 900; //The horizontal size of the screen
        public static int windowSizeY = 750; //The vertical size of the screen
		
        public static SdlTtf.TTF_Font veraData; //The data of the font
        static IntPtr vera; //The pointer to the font
        public static SdlTtf.TTF_Font veraSmallData; //The data of the font
        static IntPtr veraSmall; //The pointer to the font
		#endregion

        static void Draw()
        {
            Sdl.SDL_FillRect(screen, ref screenArea, 0); //Clear for next draw cycle
            screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface)); //Put the screen data in its place
            
			switch (Adventurer.gameState)
			{
			case GameState.OpeningMenu:
				Draw_Opening();
				break;
				
			case GameState.NameSelect:
				Draw_Name();
				break;
				
			case GameState.CreatureSelect:
				Draw_CreatureSel();
				break;
				
			case GameState.HelpMenu:
				Draw_Help();
				break;
				
			case GameState.MainGame:
				Draw_Main();
				break;
				
			case GameState.InventoryMenu:
				Draw_Inventory();
				break;
				
			case GameState.HealthMenu:
				Draw_Health();
				break;
				
			case GameState.WaitForPosition:
				Draw_GetPos();
				break;
				
			case GameState.EscapeMenu:
				Draw_Escape();
				break;
			}
			
            Sdl.SDL_Flip(screen); //Update screen
        } //Draws things to the screen
		
		static void Draw_Opening()
		{
            DrawText(vera, "Adventurer", new Point2D(windowSizeX / 2 - 130, 20), Color.White);
            DrawText(vera, "by", new Point2D(windowSizeX / 2 - 50, 50), Color.White);
            DrawText(vera, "Kalasen Zyphurus", new Point2D(windowSizeX / 2 - 130, 80), Color.White);

            if (selectionCursor == 1) //If New Game is highlighted
                DrawText(vera, "<Start Game>", new Point2D(350, windowSizeY / 2), Color.White);
            else
                DrawText(vera, "Start Game", new Point2D(350, windowSizeY / 2), Color.Gray);

            if (selectionCursor == 2) //If Help is highlighted
                DrawText(vera, "<Help>", new Point2D(350, windowSizeY / 2 + 30), Color.White);
            else
                DrawText(vera, "Help", new Point2D(350, windowSizeY / 2 + 30), Color.Gray);

            if (selectionCursor == 3) //If Quit is highlighted
                DrawText(vera, "<Quit>", new Point2D(350, windowSizeY / 2 + 60), Color.White);
            else
                DrawText(vera, "Quit", new Point2D(350, windowSizeY / 2 + 60), Color.Gray);
		}
		static void Draw_Name()
		{
	        DrawText(vera, "Name Select", new Point2D(windowSizeX / 2 - 130, 20), Color.White);
            DrawText(veraSmall, "Enter name: " + sessionName, new Point2D(windowSizeX / 2 - 130, windowSizeY / 2), Color.White);
		}
		static void Draw_CreatureSel()
		{
            DrawText(vera, "Creature Select", new Point2D(350, 20)); //Draw the title

            int m = 60;
            Queue<string> items = new Queue<string>();
            foreach (Species item in content.bestiary)
            {
                if (items.Count >= 26)
                {
                    items.Dequeue(); //Don't let there be more than 26 in the queue
                }

                items.Enqueue(CapitalizeFirst(item.name));
            }

            int count = items.Count;
            for (int c = 1; c <= count; c++)
            {
                m += 15; //Skip down 15 pixels

                #region Get item letter
                string s = "a";
                if (c == 2)
                    s = "b";
                if (c == 3)
                    s = "c";
                if (c == 4)
                    s = "d";
                if (c == 5)
                    s = "e";
                if (c == 6)
                    s = "f";
                if (c == 7)
                    s = "g";
                if (c == 8)
                    s = "h";
                if (c == 9)
                    s = "i";
                if (c == 10)
                    s = "j";
                if (c == 11)
                    s = "k";
                if (c == 12)
                    s = "l";
                if (c == 13)
                    s = "m";
                if (c == 14)
                    s = "n";
                if (c == 15)
                    s = "o";
                if (c == 16)
                    s = "p";
                if (c == 17)
                    s = "q";
                if (c == 18)
                    s = "r";
                if (c == 19)
                    s = "s";
                if (c == 20)
                    s = "t";
                if (c == 21)
                    s = "u";
                if (c == 22)
                    s = "v";
                if (c == 23)
                    s = "w";
                if (c == 24)
                    s = "x";
                if (c == 25)
                    s = "y";
                if (c == 26)
                    s = "z";
                #endregion

                DrawText(veraSmall, s + ": " + items.Dequeue(),
                    new Point2D(350, m), Color.White);
            }

            items.Clear();
		}
		static void Draw_Help()
		{
            DrawText(vera, "Help", new Point2D(400, 20), Color.White);
            DrawText(vera, "Controls:", new Point2D(10, 50), Color.White);
            DrawText(veraSmall, "Movement: arrows, end, home, pgup, pgdn; numpad", new Point2D(10, 80), Color.White);

            DrawText(veraSmall, "c - Close", new Point2D(10, 95), Color.White);
            DrawText(veraSmall, "e - Engrave", new Point2D(10, 110), Color.White);
            DrawText(veraSmall, "h - Debug digging", new Point2D(10, 125), Color.White);
            DrawText(veraSmall, "i - Inventory menu", new Point2D(10, 140), Color.White);
            DrawText(veraSmall, "k - Kick/Dismantle", new Point2D(10, 155), Color.White);
            DrawText(veraSmall, "l (L) - Toggle debug omnivision", new Point2D(10, 170), Color.White);
            DrawText(veraSmall, "o - Open", new Point2D(10, 185), Color.White);
            DrawText(veraSmall, "w - Unwield an item", new Point2D(10, 200), Color.White);
            DrawText(veraSmall, "W - Remove an item", new Point2D(10, 215), Color.White);
            DrawText(veraSmall, "x - Examine at range", new Point2D(10, 230), Color.White);
            DrawText(veraSmall, "X - Debug mode", new Point2D(10, 245), Color.White);
            DrawText(veraSmall, "z - Status", new Point2D(10, 260), Color.White);
            DrawText(veraSmall, ", - Pick up item", new Point2D(10, 275), Color.White);

            DrawText(veraSmall, "Esc (Normal Game) - Main menu", new Point2D(10, 305), Color.White);
            DrawText(veraSmall, "Space, Escape - Back a menu", new Point2D(10, 320), Color.White);
		}
		static void Draw_Main()
		{
            Draw_HUD();			
			Draw_Tiles();
		}
		static void Draw_Inventory()
		{
			Draw_Main(); //Need the background
            SdlGfx.boxColor(screen, (short)(windowSizeX * 0.66), 0, (short)windowSizeX, (short)windowSizeY,
                Color.FromArgb(1, 1, 1, 255).ToArgb()); //Black backdrop
            SdlGfx.rectangleColor(screen, (short)(windowSizeX * 0.66), 0, (short)windowSizeX, (short)windowSizeY,
                Color.White.ToArgb()); //White border
            DrawText(vera, "Inventory Menu", new Point2D(690, 20), Color.White);
            DrawText(veraSmall, "Cancel: Space", new Point2D(605, windowSizeY - 20), Color.White);

            #region List items
            if (inventorySelect == 0) //If none selected
            {
                int m = 60;
                Queue<string> items = new Queue<string>();
                foreach (Item item in currentLevel.creatures[0].inventory)
                {
                    if (items.Count >= 26)
                    {
                        items.Dequeue(); //Don't let there be more than 26 in the queue
                    }

                    items.Enqueue(CapitalizeFirst(item.name));
                }

                int count = items.Count;
                for (int c = 1; c <= count; c++)
                {
                    m += 15; //Skip down 15 pixels

                    #region Get item letter
                    string s = "a";
                    if (c == 2)
                        s = "b";
                    if (c == 3)
                        s = "c";
                    if (c == 4)
                        s = "d";
                    if (c == 5)
                        s = "e";
                    if (c == 6)
                        s = "f";
                    if (c == 7)
                        s = "g";
                    if (c == 8)
                        s = "h";
                    if (c == 9)
                        s = "i";
                    if (c == 10)
                        s = "j";
                    if (c == 11)
                        s = "k";
                    if (c == 12)
                        s = "l";
                    if (c == 13)
                        s = "m";
                    if (c == 14)
                        s = "n";
                    if (c == 15)
                        s = "o";
                    if (c == 16)
                        s = "p";
                    if (c == 17)
                        s = "q";
                    if (c == 18)
                        s = "r";
                    if (c == 19)
                        s = "s";
                    if (c == 20)
                        s = "t";
                    if (c == 21)
                        s = "u";
                    if (c == 22)
                        s = "v";
                    if (c == 23)
                        s = "w";
                    if (c == 24)
                        s = "x";
                    if (c == 25)
                        s = "y";
                    if (c == 26)
                        s = "z";
                    #endregion

                    DrawText(veraSmall, s + ": " + items.Dequeue(),
                        new Point2D(650, m), Color.White);
                }

                items.Clear();
            }
            #endregion
            else if (inventorySelect <= currentLevel.creatures[0].inventory.Count) //If the item exists
            {
                if (inventoryMode == 0)
                {
                    DrawText(veraSmall, CapitalizeFirst(currentLevel.creatures[0].inventory[
                        inventorySelect - 1].name), new Point2D(650, 75), Color.White); //Draw selected item's name

                    DrawText(veraSmall, " - [b]reak down", new Point2D(650, 90), Color.White);
                    DrawText(veraSmall, " - [c]ombine craft", new Point2D(650, 105), Color.White);
                    DrawText(veraSmall, " - [d]rop", new Point2D(650, 120), Color.White);
                    DrawText(veraSmall, " - [e]at", new Point2D(650, 135), Color.White);
                    DrawText(veraSmall, " - [f]ire/throw", new Point2D(650, 150), Color.White);
                    DrawText(veraSmall, " - [u]se", new Point2D(650, 165), Color.White);
                    DrawText(veraSmall, " - [w]ield", new Point2D(650, 180), Color.White);
                    DrawText(veraSmall, " - [W]ear", new Point2D(650, 195), Color.White);
                }
                else if (inventoryMode == 1) //Craft this item menu
                {
                    int m = 60;
                    Queue<string> items = new Queue<string>();
                    foreach (Item item in craftableItems)
                    {
                        if (items.Count >= 24)
                        {
                            items.Dequeue(); //Don't let there be more than 24 in the queue
                        }

                        items.Enqueue(CapitalizeFirst(item.name));
                    }

                    int count = items.Count;
                    if (count <= 0)
                        DrawText(veraSmall, "Nothing occurs to you, given what you have.",
                                               new Point2D(650, 75), Color.White);

                    for (int c = 1; c <= count; c++)
                    {
                        m += 15; //Skip down 15 pixels

                        #region Get item letter
                        string s = "a";
                        if (c == 2)
                            s = "b";
                        if (c == 3)
                            s = "c";
                        if (c == 4)
                            s = "d";
                        if (c == 5)
                            s = "e";
                        if (c == 6)
                            s = "f";
                        if (c == 7)
                            s = "g";
                        if (c == 8)
                            s = "h";
                        if (c == 9)
                            s = "i";
                        if (c == 10)
                            s = "j";
                        if (c == 11)
                            s = "k";
                        if (c == 12)
                            s = "l";
                        if (c == 13)
                            s = "m";
                        if (c == 14)
                            s = "n";
                        if (c == 15)
                            s = "o";
                        if (c == 16)
                            s = "p";
                        if (c == 17)
                            s = "q";
                        if (c == 18)
                            s = "r";
                        if (c == 19)
                            s = "s";
                        if (c == 20)
                            s = "t";
                        if (c == 21)
                            s = "u";
                        if (c == 22)
                            s = "v";
                        if (c == 23)
                            s = "w";
                        if (c == 24)
                            s = "x";
                        if (c == 25)
                            s = "y";
                        if (c == 26)
                            s = "z";
                        #endregion

                        DrawText(veraSmall, s + ": " + items.Dequeue(),
                            new Point2D(650, m), Color.White);
                    }
                    items.Clear();
                }
            }
		}
		static void Draw_Health()
		{	
			Draw_Main();
            SdlGfx.boxColor(screen, (short)(windowSizeX * 0.66), 0, (short)windowSizeX, (short)windowSizeY,
                Color.FromArgb(1, 1, 1, 255).ToArgb()); //Black backdrop
            SdlGfx.rectangleColor(screen, (short)(windowSizeX * 0.66), 0, (short)windowSizeX, (short)windowSizeY,
                Color.White.ToArgb()); //White border

            DrawText(vera, "Status Menu", new Point2D(690, 20), Color.White);
            DrawText(veraSmall, "Cancel: Space", new Point2D(605, windowSizeY - 20), Color.White);

            #region List parts
            int partCount = currentLevel.creatures[0].anatomy.Count;
            int m = 60;
            Queue<string> parts = new Queue<string>();
            Queue<Color> partDamage = new Queue<Color>();

            foreach (BodyPart part in currentLevel.creatures[0].anatomy)
            {
                if (parts.Count >= 24)
                {
                    parts.Dequeue(); //Don't let there be more than 24 in the queue
                    partDamage.Dequeue();
                }

                parts.Enqueue(CapitalizeFirst(part.name));
               
                switch (part.injury)
                {
                    case InjuryLevel.Healthy:
                        partDamage.Enqueue(Color.White);
                        break;
                    case InjuryLevel.Minor:
                        partDamage.Enqueue(Color.Green);
                        break;
                    case InjuryLevel.Broken:
                        partDamage.Enqueue(Color.Yellow);
                        break;
                    case InjuryLevel.Mangled:
                        partDamage.Enqueue(Color.Crimson);
                        break;
                    case InjuryLevel.Destroyed:
                        partDamage.Enqueue(Color.Gray);
                        break;
                    default:
                        throw new Exception($"Unhandled InjuryType '${part.injury.ToString()}' when displaying body parts");
                }
            }

            partCount = parts.Count;
            for (int c = 1; c <= partCount; c++)
            {
                m += 15; //Skip down 15 pixels
                DrawText(veraSmall, parts.Dequeue(),
                    new Point2D(650, m), partDamage.Dequeue());
            }

            parts.Clear();
            #endregion
		}
		static void Draw_GetPos()
		{
			Draw_Main();
            SdlGfx.boxColor(screen, 5, 533, 895, (short)(windowSizeY * 0.992), Color.Black.ToArgb());
            SdlGfx.rectangleColor(screen, 5, 533, 895, (short)(windowSizeY * 0.992), Color.White.ToArgb());
            
            DrawImage(88, new Point2D(cursorPos.X * TILEWIDTH, cursorPos.Y * TILEHEIGHT), Color.Yellow); //Draw Cursor

            #region Description
            int m = 535;
            Queue<string> messages = new Queue<string>();
            Tile thisTile = currentLevel.tileArray[cursorPos.X, cursorPos.Y];

            if (currentLevel.LineOfSight(currentLevel.creatures[0].pos, cursorPos)) //If it can be seen
            {
                foreach (Creature c in currentLevel.creatures)
                {
                    if (c.pos == cursorPos) //If creature is at this position
                    {
                        messages.Enqueue("There is a " + c.name + " here.");
                    }
                }

                if (thisTile.itemList.Count > 0)
                {
                    messages.Enqueue("There is a " + thisTile.itemList[0].name + " here.");
                }

                if (thisTile.fixtureLibrary.Count > 0)
                {
                    if (thisTile.fixtureLibrary[0] is Trap)
                    {
                        Trap t = (Trap)thisTile.fixtureLibrary[0];
                        if (t.visible)
                            messages.Enqueue("There is a " + thisTile.fixtureLibrary[0].type + " here.");
                    }
                    else
                    {
                        messages.Enqueue("There is a " + thisTile.fixtureLibrary[0].type + " here.");
                    }
                }

                if (thisTile.isWall)
                {
                    messages.Enqueue("This is a " + thisTile.material.name + " wall.");
                }
            }
            else
            {
                messages.Enqueue("You cannot see to there at the moment");
            }

            while(messages.Count >= 14)
            {
                messages.Dequeue(); //Don't let there be more than fourteen in the queue
            }

            if (messages.Count <= 0)
            {
                messages.Enqueue("There is nothing noteworthy here.");
            }

            foreach (string message in messages.Reverse())
            {
                m -= 15; //Skip up 15 pixels
                DrawText(veraSmall, message, new Point2D(10, m), Color.White);
            }

            messages.Clear();
            #endregion
		}
		static void Draw_Escape()
		{
	        DrawText(vera, "Main Menu", new Point2D(380, 20), Color.White);
	
	        if (selectionCursor == 1) //If New Game is highlighted
	            DrawText(vera, "<Return to game>", new Point2D(350, windowSizeY / 2), Color.White);
	        else
	            DrawText(vera, "Return to game", new Point2D(350, windowSizeY / 2), Color.Gray);
	
	        if (selectionCursor == 2) //If Quit is highlighted
	            DrawText(vera, "<Quit and Save>", new Point2D(350, windowSizeY / 2 + 30), Color.White);
	        else
	            DrawText(vera, "Quit and Save", new Point2D(350, windowSizeY / 2 + 30), Color.Gray);
		}
		
		static void Draw_HUD()
		{		
            SdlGfx.rectangleColor(screen, 5, 533, 270, (short)(windowSizeY * 0.992), Color.White.ToArgb());
            SdlGfx.rectangleColor(screen, 270, 533, 895, (short)(windowSizeY * 0.992), Color.White.ToArgb());

            #region Stat Box
            Color foodLevelColor = Color.White;
            string foodLevelWord = "Full";
            if (currentLevel.creatures[0].food > 15000)
            {
                foodLevelColor = Color.Cyan;
                foodLevelWord = "Overstuffed";
            }
            if (currentLevel.creatures[0].food < 10000)
            {
                foodLevelColor = Color.LightGreen;
                foodLevelWord = "Hungry";
            }
            if (currentLevel.creatures[0].food < 5000)
            {
                foodLevelColor = Color.Yellow;
                foodLevelWord = "Famished";
            }
            if (currentLevel.creatures[0].food < 2500)
            {
                foodLevelColor = Color.Crimson;
                foodLevelWord = "Starving";
            }
            if (currentLevel.creatures[0].food < 0)
            {
                foodLevelColor = Color.Gray;
                foodLevelWord = "Organ Failure";
            }

            Color injuryColor = Color.White;

            if (currentLevel.creatures[0].hp >= currentLevel.creatures[0].hpMax)
                injuryColor = Color.White;
            else if (currentLevel.creatures[0].hp > currentLevel.creatures[0].hpMax * 0.75)
                injuryColor = Color.LightCyan;
            else if (currentLevel.creatures[0].hp > currentLevel.creatures[0].hpMax * 0.5)
                injuryColor = Color.Green;
            else if (currentLevel.creatures[0].hp > currentLevel.creatures[0].hpMax * 0.25)
                injuryColor = Color.Yellow;
            else if (currentLevel.creatures[0].hp > currentLevel.creatures[0].hpMax * 0.0)
                injuryColor = Color.FromArgb(255,50,50);
            else if (currentLevel.creatures[0].hp <= 0)
                injuryColor = Color.Gray;

            int displayTurn = (int)totalTurnCount;

            DrawText(veraSmall, "Turn: " + displayTurn.ToString(),
                new Point2D(10, 535), Color.White); //Write turn count

            DrawText(veraSmall, "HP: " + currentLevel.creatures[0].hp + "/" + currentLevel.creatures[0].hpMax,
                new Point2D(175, 535), injuryColor); //Write turn count

            DrawText(veraSmall, "XP: " + currentLevel.creatures[0].xp + "/" + currentLevel.creatures[0].xpBorder*2,
                new Point2D(175, 550), Color.White); //Write turn count
			
			DrawText(veraSmall, "GP: " + currentLevel.creatures[0].gold,
                new Point2D(175, 565), Color.White); //Write turn count

            DrawText(veraSmall, "Area: (" + mapPos.X + ", " + mapPos.Y + ", " + mapPos.Z + ")",
                new Point2D(10, 550), Color.White);

            DrawText(veraSmall, "Hunger: ", new Point2D(10, 610),
                Color.White);
            DrawText(veraSmall, foodLevelWord, new Point2D(110, 610), foodLevelColor);

            DrawText(veraSmall, "STR: " + currentLevel.creatures[0].strength +
                " DEX: " + currentLevel.creatures[0].dexterity +
                " CON: " + currentLevel.creatures[0].constitution +
                " INT: " + currentLevel.creatures[0].intelligence +
                " WIS: " + currentLevel.creatures[0].wisdom +
                " CHA: " + currentLevel.creatures[0].charisma, new Point2D(10, 730));
            #endregion

            #region Message Box
            int m = 520;
            Queue<string> messages = new Queue<string>();
            foreach (string message in currentLevel.creatures[0].message)
            {
                if (messages.Count >= 14)
                {
                    messages.Dequeue(); //Don't let there be more than fourteen in the queue
                }

                messages.Enqueue(message);
            }

            foreach (string message in messages.Reverse()) //Display newest first
            {
                m += 15; //Skip down 15 pixels
                DrawText(veraSmall, message, new Point2D(280, m), Color.White);
            }

            messages.Clear();
            #endregion
		}
		static void Draw_Tiles()
		{
            for (int y = 0; y < Level.GRIDH; y++)
			{
                for (int x = 0; x < Level.GRIDW; x++)
                {
                    int imageIndex = -1; //Open floor image
                    Color imageColor = Color.White;
					Tile thisTile = currentLevel.tileArray[x,y]; //Shorthand for this tile
					Creature player = currentLevel.creatures[0]; //Shorthand for the player creature

                    if (currentLevel.levelType == "forest")
                    {
                        imageColor = Color.LightGreen; //Grassy forest floor
                    }
					
					if (thisTile.lastSeenImage > 0) //If we've seen it
                    {
                        imageIndex = thisTile.lastSeenImage; //Draw the remembered tile
                        imageColor = Color.DimGray; //But washed out
                    }

                    bool LOS = ((currentLevel.LineOfSight(player.pos, new Point2D(x, y))) || iCanSeeForever);

                    if (LOS && player.blind <= 0) //If in line of sight and not blind
                    {
						imageIndex = 46; //Open floor tile
                        if (currentLevel.tileArray[x, y].isWall)
                        {
                            imageIndex = 219; //Wall image
                            imageColor = Color.Tan;
                        }

                        if (currentLevel.tileArray[x, y].fixtureLibrary.Count > 0)
                        {
                            int topIndex = currentLevel.tileArray[x, y].fixtureLibrary.Count - 1;
                            bool visibleTrap = true;

                            if (currentLevel.tileArray[x, y].fixtureLibrary[topIndex] is Trap)
                            {
                                Trap t = (Trap)currentLevel.tileArray[x, y].fixtureLibrary[topIndex];
                                if (t.visible)
                                {
                                    visibleTrap = true;
                                }
                                else
                                {
                                    visibleTrap = false;
                                }
                            }

                            if (visibleTrap)
                            {
                                imageIndex = currentLevel.tileArray[x, y].fixtureLibrary[topIndex].imageIndex;
                                imageColor = currentLevel.tileArray[x, y].fixtureLibrary[topIndex].color;
                            }
                            //else
                            //{
                            //    imageIndex = 0; //Open floor image
                            //    imageColor = Color.White;
                            //}
                        }
                    }
					
					if (currentLevel.tileArray[x,y].itemList.Count > 0 && //If there's an item here
					    (currentLevel.creatures[0].detectItem > 0 || (LOS && player.blind <= 0))) //If detecting items or can see them
					{
						int topIndex = currentLevel.tileArray[x, y].itemList.Count - 1;
                        if (currentLevel.tileArray[x, y].itemList[topIndex] != null)
                        {
                            imageIndex = currentLevel.tileArray[x, y].itemList[topIndex].itemImage; //Top item image
                            imageColor = currentLevel.tileArray[x, y].itemList[topIndex].color;
                        }
					}
					
					foreach (Creature c in currentLevel.creatures)
                    {
                        if (c.pos == new Point2D(x, y) && //If there's a creature here
						    ((LOS && player.blind <= 0 && (c.invisibility <= 0 || player.seeInvisible > 0)) || 
						     player.detectMonster > 0)) //And we can see or detect it
                        {
                            imageIndex = c.creatureImage;
                            imageColor = c.color;
                        }
                    }

					if (imageIndex >= 0 && imageIndex < 256) //If existent
					{
						currentLevel.tileArray[x, y].lastSeenImage = imageIndex; //Remember image
						DrawImage(imageIndex, new Point2D(x * TILEWIDTH, y * TILEHEIGHT),
                            imageColor); //Draw this tile's stuff
					}
					
					if (player.pos == new Point2D(x,y)) //PC should always be drawn
					{
						imageIndex = player.creatureImage;
						imageColor = player.color;
                        DrawImage(imageIndex, new Point2D(x * TILEWIDTH, y * TILEHEIGHT),
                            imageColor); //Draw this tile's stuff
                    }
                    //int smellTotal = 0;
                    //foreach (int s in currentLevel.tileArray[x, y].scentMagnitude)
                    //{
                    //    smellTotal += s;
                    //}
                    //DrawText(veraSmall, smellTotal.ToString(), new Vector2(x * TILEWIDTH, y * TILEHEIGHT), Color.Red);
                }
			}
		}

        static void DrawText(IntPtr fontToDraw, string text, Point2D position)
        {
            DrawText(fontToDraw, text, position, Color.White);
        }
        static void DrawText(IntPtr fontToDraw, string text, Point2D position, Color color)
        {
            Sdl.SDL_Surface textImage; //The surface to be rendered on
            IntPtr textPointer; //Points to image data
            Sdl.SDL_Color foreColor = new Sdl.SDL_Color(color.R, color.G, color.B, color.A); //Convert Drawing.Color to Sdl color
            Sdl.SDL_Rect source; //Where to grab from
            Sdl.SDL_Rect target; //Where to display to

            textPointer = SdlTtf.TTF_RenderText_Blended(fontToDraw, text, foreColor); //Render text onto surface              
            textImage = (Sdl.SDL_Surface)Marshal.PtrToStructure(textPointer, typeof(Sdl.SDL_Surface)); //Put the image data in its place

            source.w = target.w = (short)textImage.w; //Get width of image
            source.h = target.h = (short)textImage.h; //Get height of image
            source.x = 0; //Get all the image
            source.y = 0; //Get all the image
            target.x = (short)position.X; //Draw at given x
            target.y = (short)position.Y; //and given y

            Sdl.SDL_BlitSurface(textPointer, ref source, screen, ref target); //Draw text on screen
            Sdl.SDL_FreeSurface(textPointer); //Free the text image
        }
        static void DrawImage(int imageToDraw, Point2D position, Color color)
        {
            target.x = position.X;
            target.y = position.Y;
            ColorSwap(imageToDraw, color);

            Sdl.SDL_BlitSurface(image[imageToDraw], ref source, screen, ref target);
        }
        static unsafe void ColorSwap(int i, Color newColor)
        {
            int* pixels = (int*)imageData[i].pixels.ToPointer();

            for (int y = 0; y < imageData[i].h; y++)
                for (int x = 0; x < imageData[i].w; x++)
                {
                    Color pixel = Color.FromArgb(pixels[x + y * imageData[i].w]);
                    if (pixel.A != 0)
                    {
                        pixel = newColor;
                        pixels[x + y * imageData[i].w] = pixel.ToArgb();
                    }
                }

            pixels = null; //For the love of rng, this memory better be freed            
        } //Changes the color of an image, is pretty hackish but works
        static unsafe void Transparencify(int i, Color tranColor)
        {
            int* pixels = (int*)imageData[i].pixels.ToPointer();

            for (int y = 0; y < imageData[i].h; y++)
                for (int x = 0; x < imageData[i].w; x++)
                {
                    Color pixel = Color.FromArgb(pixels[x + y * imageData[i].w]);
                    if (pixel.R == tranColor.R && pixel.G == tranColor.G && pixel.B == tranColor.B)
                    {
                        pixel = Color.FromArgb(0,pixel.R, pixel.G, pixel.B);
                        pixels[x + y * imageData[i].w] = pixel.ToArgb();
                    }                    
                }

            pixels = null; //For the love of rng, this memory better be freed            
        } //Changes the transparency of an image, is pretty hackish but works
	}
}