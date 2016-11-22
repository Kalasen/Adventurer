using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Weapon : Item
    {		
		public Weapon():this("missingweapon"){}
		public Weapon(string name):this(name, new DNotation(2)){}
		public Weapon(string name, DNotation damage):this(name, damage, new List<Item>()){}
		public Weapon(string name, DNotation damage, List<Item> componentList): this(name, damage, componentList, Color.White){}
        public Weapon(string name, DNotation damage, List<Item> componentList, Color color) : this(100, 100, name, color, damage)
        {
            this.componentList = componentList;
        }
        public Weapon(float mass, float volume, string name, Color color, DNotation damage)
            :base(mass, volume, name, color)
        {
            base.itemImage = 41;
            base.damage = damage;
        }
		public Weapon(Weapon w):base(w){}
    } //Yay violence!
}
