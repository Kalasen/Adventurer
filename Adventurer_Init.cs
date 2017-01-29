using System; //General C# functions
using System.Linq;
using System.Drawing; //The colors, man
using System.IO; //So we can read and write files
using System.Runtime.InteropServices; //For use in wrangling pointers in their place
using Tao.Sdl;
using KalaGame;

namespace Adventurer
{
	public partial class Adventurer
	{
        public static int baseSeed = (int)DateTime.Now.Ticks;
        public static int worldSeed = baseSeed;

        static void Init_PreInitialize()
        {
            Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO); //Set up the video display
            SdlTtf.TTF_Init(); //Set up the text rendering

            screen = Sdl.SDL_SetVideoMode(windowSizeX, windowSizeY, 8, Sdl.SDL_SWSURFACE); //Set up the screen.
            screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface)); //Put the screen data in its place

            screenArea.w = target.w = (short)screenData.w; //Get width of image
            screenArea.h = target.h = (short)screenData.h; //Get height of image
            screenArea.x = 0; //Get all the image
            screenArea.y = 0; //Get all the image

            Sdl.SDL_WM_SetCaption("Adventurer", "Adventurer");
        } //Stuff that goes before load content
        static void Init_LoadContent()
        {
            vera = SdlTtf.TTF_OpenFont("Content/Fonts/Vera.ttf", 24); //Load in 12-point Vera font.
            veraData = (SdlTtf.TTF_Font)Marshal.PtrToStructure(vera, typeof(SdlTtf.TTF_Font)); //Put the font data in its place            

            veraSmall = SdlTtf.TTF_OpenFont("Content/Fonts/Vera.ttf", 10); //Load in 12-point Vera font.
            veraSmallData = (SdlTtf.TTF_Font)Marshal.PtrToStructure(vera, typeof(SdlTtf.TTF_Font)); //Put the font data in its place

            DrawText(veraSmall, "Pre-initialization complete", new Vector2(15, 15));
            DrawText(veraSmall, "Fonts loaded; loading images...", new Vector2(15, 30));
            Sdl.SDL_Flip(screen); //Update screen
			
			IntPtr rwop = Sdl.SDL_RWFromFile("Content/Tiles/ASCII_Tileset.PNG", "rb");			
			IntPtr tileSet = SdlImage.IMG_LoadPNG_RW(rwop);
            
            short n = 0;
            target.w = source.w = TILEWIDTH; //I love this combination thing
            target.h = source.h = TILEHEIGHT; //It's so elegant
            
            for (byte y = 0; y < 16; y++)
                for (byte x = 0; x < 16; x++) //16x16 tiles
                {
                    source.x = (short)(x * 17); //X to grab from
                    source.y = (short)(y * 17); //Y to grab from

                    image[n] = Sdl.SDL_CreateRGBSurface(Sdl.SDL_SWSURFACE, TILEWIDTH, TILEHEIGHT, 32, 0, 0, 0, 0); //Allocate image area
                    Sdl.SDL_BlitSurface(tileSet, ref source, image[n], ref target);
                    image[n] = Sdl.SDL_DisplayFormatAlpha(image[n]);
                    imageData[n] = (Sdl.SDL_Surface)Marshal.PtrToStructure(image[n], typeof(Sdl.SDL_Surface)); //Put the image data in its place
                    Transparencify(n, Color.Magenta); //Magenta is the transparent color
                    image[n] = Sdl.SDL_DisplayFormatAlpha(image[n]);
                    imageData[n] = (Sdl.SDL_Surface)Marshal.PtrToStructure(image[n], typeof(Sdl.SDL_Surface)); //Put the image data in its place
                    n++;
                }
			
            DrawText(veraSmall, "Creatures loaded. External content loading complete.", new Vector2(15, 120));
            DrawText(veraSmall, "Post-initializing...", new Vector2(15, 135));
            Sdl.SDL_Flip(screen); //Update screen
        } //Loads in the external content
        static void Init_PostInitialize()
        {
            source.w = target.w = (short)imageData[1].w; //Get width of image
            source.h = target.h = (short)imageData[1].h; //Get height of image
            source.x = 0; //Get all the image
            source.y = 0; //Get all the image

            Console.WriteLine("Welcome to Adventurer. Errors will be reported here and in ErrorLogs/crash.txt.");

            for (int z = 0; z < 100; z++)
                for (int y = 0; y < 100; y++)
                    for (int x = 0; x < 100; x++)
                    {
                        levelSeed[x, y, z] = rng.Next(); //Seed the world
                    }

            rock = content.materials.First(m => m.name ==  "shale"); //Shale is our default rock
            air = content.materials.First(m => m.name == "air"); //TODO: figure out why we're holding on to air and rock, we have Linq now

            DrawText(veraSmall, "Post-initialization complete. Starting main menu...", new Vector2(15, 150));
            Sdl.SDL_Flip(screen); //Update screen      

            //Thread.Sleep(10000); //Pause to allow review of loading steps
        } //Sets up everything needed after load content		
	}
}

