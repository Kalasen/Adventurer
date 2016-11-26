using System; //The big book of everything
using System.IO; //Mainly used for reading and writing files
using System.Collections.Generic;
using System.Runtime.InteropServices; //For use in wrangling pointers in their place (ew SDL)
using System.Drawing; //Some system color drawing and stuff
using Tao.Sdl; //Graphics Library

namespace Adventurer
{
    public partial class Adventurer
    {
        #region Data Declarations
        public const double PI = 3.14159265; //Circumference/Diameter
        public const byte TURN_THRESHOLD = 72; //Timing system threshold for getting a turn

        static int exploredLevels = 0;

        static ContentEncyclopedia content = new ContentEncyclopedia();

        static Level currentLevel {get;set;} //The level we're working with
        public static Random rng = new Random(baseSeed); //The will of the random number god
        public static Dice rngDie = new Dice(baseSeed);
        static float totalTurnCount = 1;
        static bool iCanSeeForever = false;
        static bool debugMode = false;
		
        static string sessionName = String.Empty; //The title of the current session
        static int inventorySelect = 0; //For use in the inventory menu
        static int inventoryMode = 0; //Main inventory mode  
        static int selectionCursor = 1; //A variable to track what part of the menu a cursor is on.        
        static int[, ,] levelSeed = new int[100, 100, 100]; //50x50x50 map x,y,z
        static Vector2 cursorPos = new Vector2(10, 10); //A position for ranged selection
        static Vector3 mapPos = new Vector3(50, 50, 1); //We start at 50, 50, 1

        static bool run = true; //Whether the game should continue running

        static List<Item> craftableItems = new List<Item>(); //For use in inventory menu
		
		public static Material air, rock;

        static int ticks = 0;
        #endregion

        //The code starts running here
        static void Main() //The code starts running in here
        {
            //try  //Try to run the game
            //{                
                Test(); //Test random junk
                Init_PreInitialize(); //Load in stuff that needs to be done first
                content.LoadAll();
                Init_LoadContent(); //Load in external content
                Init_PostInitialize(); //Set everything up after the content loading

                while (run) //Loop update and draw until ready to close
                {
                    Draw(); //Draw everything to the screen
                    Update(); //Run the main bits of logic
                }
            //}
            //catch (Exception e) //If it can't
            //{
            //#region Error Catch
            //    List<string> lines = new List<string>();
            //    lines.Add("The dungeon collapses. Rocks fall, everyone dies. Kalasen apologizes for the instability.");
            //    lines.Add("Perhaps you can salvage some understanding of the collapse from this arcane scroll.");
            //    lines.Add(String.Empty);
            //    lines.Add("**********************************");
            //    lines.Add(String.Empty);
            //    lines.Add(e.ToString());
            //    lines.Add(String.Empty);
            //    lines.Add("**********************************");

            //    if (File.Exists("ErrorLogs/crash.txt"))
            //        File.Delete("ErrorLogs/crash.txt"); //Clear it if it's there

            //    string[] eStr = new string[1];
            //    eStr[0] = e.ToString();
            //    File.WriteAllLines("ErrorLogs/crash.txt", eStr);

            //    foreach (string s in lines)
            //        Console.WriteLine(s); //Display the error on screen

            //    Console.ReadLine(); //Pause for the player to read it
            //    throw;
            //#endregion
            //}
        }

        //Converts a letter to its number, aka 'a' == 1, 'b' == 2, etc
        //TODO: Common library extension method?
        static int LetterIndexToNumber(string letter)
        {
            switch (letter)
            {
                case "a":
                    return 1;

                case "b":
                    return 2;

                case "c":
                    return 3;

                case "d":
                    return 4;

                case "e":
                    return 5;

                case "f":
                    return 6;

                case "g":
                    return 7;

                case "h":
                    return 8;

                case "i":
                    return 9;

                case "j":
                    return 10;

                case "k":
                    return 11;

                case "l":
                    return 12;

                case "m":
                    return 13;

                case "n":
                    return 14;

                case "o":
                    return 15;

                case "p":
                    return 16;

                case "q":
                    return 17;

                case "r":
                    return 18;

                case "s":
                    return 19;

                case "t":
                    return 20;

                case "u":
                    return 21;

                case "v":
                    return 22;

                case "w":
                    return 23;

                case "x":
                    return 24;

                case "y":
                    return 25;

                case "z":
                    return 26;

                default:
                    return -1;
            }
        }

        //TODO: Common library extension method?
        static string CapitalizeFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty; //Return empty if null or empty

            char[] a = s.ToCharArray(); //Convert to character array for speed
            a[0] = char.ToUpper(a[0]); //Capitalize first letter
            return new string(a); //Return the capitalized string
        }

        //TODO: This should go on Level, shouldn't it?
        static void GenLevel(string levelType, bool load)
        {
            exploredLevels++;

            if (currentLevel != null)
            {
                FileS_Level(currentLevel);
            }

            currentLevel = null;            

            string levelPath = "Saves/" + sessionName + "/(" + mapPos.X.ToString() + ", " + mapPos.Y.ToString() + ", " +
                mapPos.Z.ToString() + ").txt"; //The path to the level

            if (File.Exists(levelPath) && load)
            {
                FileL_Level(mapPos);
                
                if (currentLevel.creatures.Count <= 0) //If it failed
                {

                    bool success = false;

                    while (!success)
                    {
                        try
                        {
                            //TODO: Make a new ContentEncyclopedia to pass in here with only the creatures and such that should appear on this level
                            currentLevel = new Level(rng.Next(8, 10), rng.Next(6, 16), levelType, content, levelSeed[mapPos.X, mapPos.Y, mapPos.Z], mapPos);
                            success = true;
                        }
                        catch (Exception e)
                        {
                            if (e.Message != "New level has isolated room")
                                throw; //If it's not an isolated room error                        
                            success = false;
                        }
                    }
                }
            }
            else
            {
                bool success = false;

                while (!success)
                {
                    try
                    {
                        //TODO: Make a new content object with just the creatures that should appear in this level
                        currentLevel = new Level(rng.Next(8, 10), rng.Next(6, 16), levelType, content, levelSeed[mapPos.X, mapPos.Y, mapPos.Z], mapPos);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "New level has isolated room")
                            throw; //If it's not an isolated room error                        
                        levelSeed[mapPos.X, mapPos.Y, mapPos.Z] = rng.Next(); // New level layout
                        success = false;
                    }
                }
            }
        }		
        static void NewGame()
        {
            #region Race Select
            gameState = CREATURE_SELECT; //Yup, creature selection            

            int numberOfPages = (int)Math.Ceiling((double)content.bestiary.Count / 26f); //Get the number of pages we'll need
            string[,] creaturePages = new string[numberOfPages, 26]; //Only as many pages as we need
            for (int i = 0; i < numberOfPages; i++)
                for (int c = 0; c < 26; c++)
                {
                    if (i * 26 + c >= content.bestiary.Count) //If we've gone out of bounds
                        break;
                    creaturePages[i, c] = content.bestiary[i * 26 + c].name; //Add the name of the creature to the right page and position
                }
            
            int pageNumber = 0; //Whatever page we are on            
            int inputIndex = 0; //The creature we've selected.

            #region Draw New Page
            Sdl.SDL_FillRect(screen, ref screenArea, 0); //Clear for next draw cycle
            screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface)); //Put the screen data in its place
            DrawText(vera, "Creature Select", new Vector2(350, 20)); //Draw the title

            int m = 60;
            Queue<string> items = new Queue<string>();

            for (int i = 0; i < 26; i++)
            {
                items.Enqueue(CapitalizeFirst(creaturePages[pageNumber, i]));
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

                if (items.Peek() != String.Empty)
                    DrawText(veraSmall, s + ": " + items.Dequeue(),
                        new Vector2(350, m), Color.White);
            }

            items.Clear();
            Sdl.SDL_Flip(screen);
            #endregion
            bool done = false;
            while (!done)
            {
                string input = Update_GetKey();
                switch (input)
                {
                    case "a":
                    case "b":
                    case "c":
                    case "d":
                    case "e":
                    case "f":
                    case "g":
                    case "h":
                    case "i":
                    case "j":
                    case "k":
                    case "l":
                    case "m":
                    case "n":
                    case "o":
                    case "p":
                    case "q":
                    case "r":
                    case "s":
                    case "t":
                    case "u":
                    case "v":
                    case "x":
                    case "y":
                    case "z":                        
                        inputIndex = (26 * pageNumber) + LetterIndexToNumber(input) - 1;
                        if(inputIndex < content.bestiary.Count)
                            done = true;
                        break;

                    case "Enter":
                    case "Space":
                        pageNumber++;
                        if (pageNumber > numberOfPages - 1)
                            pageNumber = 0; //Cycle back to the beginning.

                        #region Draw New Page
                        Sdl.SDL_FillRect(screen, ref screenArea, 0); //Clear for next draw cycle
                        screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface)); //Put the screen data in its place
                        DrawText(vera, "Creature Select", new Vector2(350, 20)); //Draw the title

                        m = 60;
                        items = new Queue<string>();

                        for (int i = 0; i < 26; i++)
                        {                                                   
                            items.Enqueue(CapitalizeFirst(creaturePages[pageNumber,i]));
                        }

                        count = items.Count;
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

                        if (items.Peek() != String.Empty)
                            DrawText(veraSmall, s + ": " + items.Dequeue(),
                                new Vector2(350, m), Color.White);
                        }

                        items.Clear();
                        Sdl.SDL_Flip(screen);
                        #endregion
                        break;
                }
            }
            #endregion

            GenLevel("dungeon", true);

            Vector2 playerPos = currentLevel.creatures[0].pos;
            currentLevel.creatures[0] = content.bestiary[inputIndex].GenerateCreature("monster", content.items, rng.Next()); //Player is given creature
			currentLevel.creatures[0].hpMax = currentLevel.creatures[0].hp += 5; //Add 5 for being an adventurer
            currentLevel.creatures[0].message.Add("Welcome to Adventurer!");
            currentLevel.creatures[0].message.Add("You are a " + currentLevel.creatures[0].name + ".");
            if (rngDie.Roll(100) != 1) //99 out of every 100 times            
                currentLevel.creatures[0].message.Add("Try not to die too soon.");
            else            
                currentLevel.creatures[0].message.Add("You are going to die soon."); //Freak 'em out.
            currentLevel.creatures[0].pos = playerPos;
            currentLevel.creatures[0].hpMax += rngDie.Roll(currentLevel.creatures[0].constitution) / 2;
            currentLevel.creatures[0].hp = currentLevel.creatures[0].hpMax;

            #region Create initial save data
            string folderPath = "Saves/" + sessionName;

            Directory.CreateDirectory(folderPath); //Open a folder for this world's save

            List<string> data = new List<string>(); //The data to write

            data.Add("[SEED] " + worldSeed.ToString()); //Add the seed
            data.Add("[NAME] " + sessionName.ToString()); //Add the session name

            File.AppendAllText("Saves/SaveStockpile.txt", "[NAME] " + sessionName.ToString() + "\r\n");
            File.AppendAllText("Saves/SaveStockpile.txt", "[SEED] " + worldSeed.ToString() + "\r\n");
            data.Clear();

            data.Add("[SEED] " + worldSeed.ToString()); // Track the world seed
            data.Add("[PLAYERPOS] (" + mapPos.X.ToString() +
                ", " + mapPos.Y.ToString() +
                ", " + mapPos.Z.ToString() + ")"); //Track player's position

            //File.WriteAllLines(filePath, data.ToArray()); //Write the data

            //SaveLevel(currentLevel); //Track the first level
            #endregion
        }

        // For testing code snippets
        static void Test()
        {
        } 
    }
}