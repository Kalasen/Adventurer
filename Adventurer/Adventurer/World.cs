// -----------------------------------------------------------------------
// <copyright file="World.cs" company="Kalasen Games">
// GNU GPL
// </copyright>
// -----------------------------------------------------------------------

namespace Adventurer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a universe. Seriously.
    /// </summary>
    public class World
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class. 
        /// </summary>
        public World()
        {
            this.creatures = new List<Creature>();
            this.tiles = new Dictionary<Vector3, Tile>();

            Vector3 tilePos = new Vector3();

            for (int z = -10; z < 5; z++)
                for (int y = -25; y < 25; y++)
                    for (int x = -25; x < 25; x++)
                    {
                        tilePos.X = x;
                        tilePos.Y = y;
                        tilePos.Z = z;
                        this.tiles.Add(tilePos, new Tile());
                    }
        }

        /// <summary>
        /// Gets or sets. A list of creatures in the world.
        /// </summary>
        public List<Creature> creatures { get; set; }

        /// <summary>
        /// Gets or sets. A dictionary of tiles in the world.
        /// </summary>
        public Dictionary<Vector3, Tile> tiles { get; set; }
    }
}
