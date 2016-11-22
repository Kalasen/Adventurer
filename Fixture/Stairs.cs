using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Stairs : Fixture
    {
        public bool isDown;
        public Stairs(bool isDown)
            : base("stairs", 30, Color.FromArgb(255,255,255))
        {
            if (isDown)
            {
                this.isDown = true;
                base.imageIndex = 62;
            }
            else
            {
                this.isDown = false;
                base.imageIndex = 60;
            }
        }

        public void FallDown(Creature c)
        {
            Random rng = new Random();
            bool warned = rng.Next(0, 100) > 50;
            if (warned)
                c.message.Add("I warned you about stairs bro!");
            else
                c.message.Add("I told you dog!!!");
        }
        public void FallMultiple(Creature c)
        {
            c.message.Add("IT KEEPS HAPENNING");
        }
    } //Yeah, just don't fall down them
}
