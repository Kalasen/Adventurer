using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Armor : Item //Armor is an item that can be worn
    {
        public int aC {get;set;}
        public string shape {get;protected set;}
        public List<string> covers {get;protected set;}
        
		public Armor(float mass, float volume, int aC, string shape, List<Item> components, string name,
            List<string> covers, Color color, List<string> uses)
            : base(mass, volume, name, color, components, uses)
        {
            this.aC = aC;
            this.covers = covers;
            this.shape = shape;
            base.componentList = components;
            base.itemImage = 91; //The armor image '['
        }
		public Armor(Armor a):base(a)
		{
			this.aC = a.aC;
			this.shape = a.shape;
			this.covers = new List<string>();
			foreach(string s in a.covers)
				this.covers.Add(s);
		}
    }
}
