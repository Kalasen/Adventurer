using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.Sdl;

namespace Adventurer
{
    public class Graphics
    {
        const int WINDOWSIZEX = 900; //The horizontal size of the screen
        const int WINDOWSIZEY = 750; //The vertical size of the screen

        IntPtr screen;
        Sdl.SDL_Surface screenData; //The screen bitmap 
        Sdl.SDL_Rect screenArea;
        Sdl.SDL_Rect target;

        //Fonts
        public static SdlTtf.TTF_Font veraData;
        static IntPtr vera;
        public static SdlTtf.TTF_Font veraSmallData;
        static IntPtr veraSmall;

        public Graphics()
        {
            //Set up video display
            Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
            SdlTtf.TTF_Init();

            //Set up main window
            screen = Sdl.SDL_SetVideoMode(WINDOWSIZEX, WINDOWSIZEY, 8, Sdl.SDL_SWSURFACE);
            screenData = (Sdl.SDL_Surface)Marshal.PtrToStructure(screen, typeof(Sdl.SDL_Surface));

            //Get the window rectangle for later use
            screenArea.w = target.w = (short)screenData.w;
            screenArea.h = target.h = (short)screenData.h;
            screenArea.x = 0;
            screenArea.y = 0;

            //Load in 24-point Vera
            vera = SdlTtf.TTF_OpenFont("Content/Fonts/Vera.ttf", 24);
            veraData = (SdlTtf.TTF_Font)Marshal.PtrToStructure(vera, typeof(SdlTtf.TTF_Font));        

            //Load in 10-point Vera
            veraSmall = SdlTtf.TTF_OpenFont("Content/Fonts/Vera.ttf", 10);
            veraSmallData = (SdlTtf.TTF_Font)Marshal.PtrToStructure(vera, typeof(SdlTtf.TTF_Font));


            //TODO: Generalize setting the window caption
            //Set the window caption
            Sdl.SDL_WM_SetCaption("Adventurer", "Adventurer");
        }

        public void DrawAll()
        {
            //TODO: Move logic from Adventurer_Draw to here
        }

        public void DrawText(string text, Vector2 position, Color color = default(Color), Fonts font = Fonts.VeraSmall)
        {
            IntPtr textPointer; //Points to image data
            Sdl.SDL_Color foreColor = new Sdl.SDL_Color(color.R, color.G, color.B, color.A); //Convert Drawing.Color to Sdl color
            Sdl.SDL_Rect source; //Where to grab from
            Sdl.SDL_Rect target; //Where to display to

            IntPtr fontToDraw;
            switch(font)
            {
                case Fonts.Vera:
                    fontToDraw = vera;
                    break;
                case Fonts.VeraSmall:
                    fontToDraw = veraSmall;
                    break;
                default:
                    throw new Exception($"Unhandled font type {font}.");
            }

            textPointer = SdlTtf.TTF_RenderText_Blended(fontToDraw, text, foreColor); //Render text onto surface              
            Sdl.SDL_Surface textImage = (Sdl.SDL_Surface)Marshal.PtrToStructure(textPointer, typeof(Sdl.SDL_Surface)); //Put the image data in its place

            //Grab source and target dimensions
            source.w = target.w = (short)textImage.w;
            source.h = target.h = (short)textImage.h;
            source.x = 0;
            source.y = 0;
            target.x = (short)position.X; //Draw at given x
            target.y = (short)position.Y; //and given y

            Sdl.SDL_BlitSurface(textPointer, ref source, screen, ref target); //Draw text on screen
            Sdl.SDL_FreeSurface(textPointer); //Free the text image
        }
    }
}
