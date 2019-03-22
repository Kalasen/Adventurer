using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Adventurer
{
    class Ring : Item
    {
        public Effect effect {get;set;}

        public Ring() : base() { }
        public Ring(float mass, int volume, string name, Color color, Effect effect, List<Item> components, List<string> uses)
            : base(mass, volume, name, color, components, uses)
        {
            base.itemImage = 61; //The = symbol
            this.effect = effect;
        }
		public Ring(Ring r):base(r){}
    } //Bling
}
