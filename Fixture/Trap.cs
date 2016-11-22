using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Trap : Fixture //It's a trap!
    {
        public bool armed; //Whether the trap is set
        public bool visible = false; //You can't see it yet
        public Effect effect; //What happens when the trap is triggered

        public Trap(Effect effect)
            :base("trap", 94, Color.Red)
        {
            this.effect = effect;
        }
    }
}
