using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    class Tree : Fixture //TODO: Make public?
    {
        public string species, fruit; //Change to enum?

        public Tree(Random rng)
            :base("tree", 6, Color.ForestGreen)
        {
            int treeImageRand = rng.Next(0, 4);
            switch (treeImageRand) //Random tree image
            {
                case 0:
                    base.imageIndex = 5;
                    break;

                case 1:
                    base.imageIndex = 6;
                    break;

                case 2:
                    base.imageIndex = 23;
                    break;

                case 3:
                    base.imageIndex = 24;
                    break;
            }
            this.species = "pine";
            this.fruit = "pinecone";
        }
    }
}
