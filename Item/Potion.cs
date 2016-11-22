using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Potion : Item
    {
		public Potion():this("missingpotion"){}
		public Potion(string name):this(name,new Effect()){}
		public Potion(string name, Effect effect):this(name, effect, Color.White){}
		public Potion(string name, Effect effect, Color color):this(name, color, new List<Item>(), effect){}
        public Potion(string name, Color color, List<Item> componentList, Effect effect)
            : base(name, color, componentList)
        {
            base.edible = true;
            base.eatEffect = effect;
            base.itemImage = 33; // "!"
        }
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
