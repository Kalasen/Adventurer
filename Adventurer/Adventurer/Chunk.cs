// -----------------------------------------------------------------------
// <copyright file="Chunk.cs" company="Kalasen Games">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Adventurer
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a group of tiles
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// How many tiles wide a chunk is from the center
        /// </summary>
        public const int WIDTH = 25;

        /// <summary>
        /// How many tiles long a chunk is from the center
        /// </summary>
        public const int LENGTH = 25;

        /// <summary>
        /// How many tiles high a chunk is from the center
        /// </summary>
        public const int HEIGHT = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk"/> class.
        /// </summary>
        public Chunk()
        {
            this.tiles = new Dictionary<Vector3, Tile>();
            this.creatures = new List<Creature>();

            for (int z = -HEIGHT; z < HEIGHT; z++)
                for (int y = -LENGTH; y < LENGTH; y++)
                    for (int x = -WIDTH; x < WIDTH; x++)
                    {
                        this.tiles.Add(new Vector3(x, y, z), new Tile());
                    }
        }

        /// <summary>
        /// Gets or sets. Dictionary of tiles in this chunk
        /// </summary>
        public Dictionary<Vector3, Tile> tiles { get; set; }

        /// <summary>
        /// Gets or sets. List of creatures in this chunk.
        /// </summary>
        public List<Creature> creatures { get; set; }
    }
}
