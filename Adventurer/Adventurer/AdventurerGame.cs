using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Adventurer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AdventurerGame : Game
    {
        /// <summary>
        /// The game's GraphicsDeviceManager
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// The game's SpriteBatch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The world the player is currently in
        /// </summary>
        private World currentWorld;

        /// <summary>
        /// A dictionary of images and their names
        /// </summary>
        private Dictionary<ImageName, Texture2D> imageDictionary = new Dictionary<ImageName, Texture2D>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdventurerGame"/> class.
        /// </summary>
        public AdventurerGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.currentWorld = new World();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.imageDictionary.Add(ImageName.HUMAN, this.Content.Load<Texture2D>("Human"));
            this.imageDictionary.Add(ImageName.GRASS, this.Content.Load<Texture2D>("Grass"));
        }

        /// <summary>
        /// Code to run after LoadContent, but before the game loop starts
        /// </summary>
        protected override void BeginRun()
        {
            Creature player = new Creature(ImageName.HUMAN);
            this.currentWorld.creatures.Add(player);

            base.BeginRun();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            foreach (Creature creature in this.currentWorld.creatures)
            {
                this.spriteBatch.Draw(this.imageDictionary[creature.image], new Vector2(100, 100), Color.White);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
