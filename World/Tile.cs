using KalaGame;
using System.Collections.Generic;

namespace Adventurer
{
    //Tiles that the dungeon is made of
    public class Tile
    {
		public const float ROOMTEMP = 20; //Room temperature, in celsius: 20 celsius == 68 fahrenheit
        public int lastSeenImage {get;set;}
        public float temperature {get;set;}
        public string engraving {get;set;}
        public List<int> scentMagnitude {get;set;}
        public List<string> scentIdentifier {get;set;}
        public bool isInLOS, hasBeenSeen, isTransparent, isPassable, isCorridorEdge, isRoomable, isRoomEdge, isWall, isDoor;
        public bool hasBeenDug {get;set;}
        public Material material {get;set;}
        public int tileImage {get;set;}
		public int adjacentToRoomN {get;set;}
		public Point2D pos {get;set;}
        public List<Item> itemList {get;set;}
        public List<Fixture> fixtureLibrary {get;set;}

		public Tile():this(Adventurer.air){}
        public Tile(bool passable, Material mat, int image, bool roomable, bool transparent)
        {
			this.itemList = new List<Item>();
			this.scentIdentifier = new List<string>();
			this.scentMagnitude = new List<int>();
			this.fixtureLibrary = new List<Fixture>();
            this.temperature = ROOMTEMP; //20 Celsius = 68 Fahrenheit
            this.isPassable = passable;
            material = mat;
            tileImage = image;
            isRoomable = roomable;
            isTransparent = transparent;
            hasBeenSeen = false;
            adjacentToRoomN = 0;
        }
		public Tile(Material m)
		{			
			this.fixtureLibrary = new List<Fixture>();
			this.material = m;
			if (m.density < 1.5f) //If not too dense
			{
				isPassable = true;
			}
		}
		public Tile(Tile t)
		{
			
		}

        public void MakeOpen()
        {
            isTransparent = true;
            isPassable = true;
            isWall = false;
        }
    }
}
