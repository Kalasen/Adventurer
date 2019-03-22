using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;
using Newtonsoft.Json;

namespace Adventurer
{
    public class Potion : Item
    {
        public Potion() : base() { }
        public Potion(string name, float mass, float volume, Color color, List<Item> components, List<string> uses): base(mass, volume, name, color, components, uses) { }
		public Potion(Potion p):base(p){}

        public override Level Eat(Level currentLevel, Creature consumer)
        {
            int creatureIndex = currentLevel.creatures.IndexOf(consumer);
			
			currentLevel.creatures[creatureIndex].Affect(eatEffect, currentLevel); //Apply effect to the consumer

            currentLevel.creatures[creatureIndex].inventory.Remove(this); //It's been drunk, it no longer exists; in the future, rewrite to transfer contents to creature's stomach, and leave behind an empty container in inventory
            return currentLevel;
        }
    } //A potion is an item, that when drunk has an Effect
}
