// -----------------------------------------------------------------------
// <copyright file="World.cs" company="Kalasen Games">
// GNU GPL
// </copyright>
// -----------------------------------------------------------------------

namespace Adventurer
{
    using System.Collections.Generic;

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
            this.chunks = new Dictionary<Vector3, Chunk>();

            this.player = new Player(ImageName.HUMAN);

            this.currentChunk = new Chunk();
            this.currentChunk.creatures.Add(this.player);
            this.chunks.Add(new Vector3(0, 0, 0), this.currentChunk);
        }

        /// <summary>
        /// Gets or sets. A dictionary of tiles in the world.
        /// </summary>
        public Dictionary<Vector3, Chunk> chunks { get; set; }

        /// <summary>
        /// Gets or sets. The current chunk the player is in.
        /// </summary>
        public Chunk currentChunk { get; set; }

        /// <summary>
        /// Gets or sets. The creature representing the player.
        /// </summary>
        public Player player { get; set; }
    }
}
