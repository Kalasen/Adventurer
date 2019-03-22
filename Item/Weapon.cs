using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;
using KalaGame;
using Newtonsoft.Json;

namespace Adventurer
{
    public class Weapon : Item
    {
        public Weapon() : base() { }
        public Weapon(float mass, float volume, string name, Color color, DNotation damage, List<Item> components, List<string> uses)
            :base(mass, volume, name, color, components, uses)
        {
            base.itemImage = 41;
            base.damage = damage;
        }
		public Weapon(Weapon w):base(w){}
    } //Yay violence!
}
