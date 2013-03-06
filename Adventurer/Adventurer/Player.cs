// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Kalasen Games">
// GNU GPL
// </copyright>
// -----------------------------------------------------------------------

namespace Adventurer
{
    /// <summary>
    /// Represents the creature that is the player.
    /// </summary>
    public class Player : Creature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="image">
        /// The image to represent the player.
        /// </param>
        public Player(ImageName image)
            : base(image)
        {
        }
    }
}
