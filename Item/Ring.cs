using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Adventurer
{
    class Ring : Item
    {
        public Effect effect {get;set;}
		
		public Ring():this("missingring"){}
		public Ring(string name):this(name, Color.White){}
		public Ring(string name, Effect effect):this(name, Color.White, effect){}
		public Ring(string name, Color color):this(name, color, new Effect()){}
		public Ring(string name, Color color, Effect effect):this(1f,1,name,color, effect){}
        public Ring(float mass, int volume, string name, Color color, Effect effect)
            : base(mass, volume, name, color)
        {
            base.itemImage = 61; //The = symbol
            this.effect = effect;
        }
		public Ring(Ring r):base(r){}
    } //Bling
}
