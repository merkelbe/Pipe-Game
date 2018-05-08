using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PipeGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SpriteFont winSpriteFont;
        KeyFilter keyFilter;
        List<Keys> pressedKeys;
        MouseFilter mouseFilter;

        KeyboardState keyboardState;
        MouseState mouseState; 

        int WINDOW_WIDTH = 1200;
        int WINDOW_HEIGHT = 700;
        
        VisualPipeGame visualPipeGame;
        
        Sprite pipe1Sprite;
        Sprite pipe1FilledSprite;
        Sprite pipe2Sprite;
        Sprite pipe2FilledSprite;
        Sprite pipe2bSprite;
        Sprite pipe2bFilledSprite;
        Sprite pipe3Sprite;
        Sprite pipe3FilledSprite;
        Sprite pipe4Sprite;
        Sprite pipe4FilledSprite;

        List<Sprite> orderedSpriteList;

        List<ClickInfo> currentClickInputs;

        List<Color> allColorOptions = new List<Color>() { Color.AliceBlue, Color.AntiqueWhite, Color.Aqua, Color.Aquamarine, Color.Azure, Color.Beige, Color.Bisque, Color.Black, Color.BlanchedAlmond, Color.Blue, Color.BlueViolet, Color.Brown, Color.BurlyWood, Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.CornflowerBlue, Color.Cornsilk, Color.Crimson, Color.Cyan, Color.DarkBlue, Color.DarkCyan, Color.DarkGoldenrod, Color.DarkGray, Color.DarkGreen, Color.DarkKhaki, Color.DarkMagenta, Color.DarkOliveGreen, Color.DarkOrange, Color.DarkOrchid, Color.DarkRed, Color.DarkSalmon, Color.DarkSeaGreen, Color.DarkSlateBlue, Color.DarkSlateGray, Color.DarkTurquoise, Color.DarkViolet, Color.DeepPink, Color.DeepSkyBlue, Color.DimGray, Color.DodgerBlue, Color.Firebrick, Color.FloralWhite, Color.ForestGreen, Color.Fuchsia, Color.Gainsboro, Color.GhostWhite, Color.Gold, Color.Goldenrod, Color.Gray, Color.Green, Color.GreenYellow, Color.Honeydew, Color.HotPink, Color.IndianRed, Color.Indigo, Color.Ivory, Color.Khaki, Color.Lavender, Color.LavenderBlush, Color.LawnGreen, Color.LemonChiffon, Color.LightBlue, Color.LightCoral, Color.LightCyan, Color.LightGoldenrodYellow, Color.LightGray, Color.LightGreen, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen, Color.LightSkyBlue, Color.LightSlateGray, Color.LightSteelBlue, Color.LightYellow, Color.Lime, Color.LimeGreen, Color.Linen, Color.Magenta, Color.Maroon, Color.MediumAquamarine, Color.MediumBlue, Color.MediumOrchid, Color.MediumPurple, Color.MediumSeaGreen, Color.MediumSlateBlue, Color.MediumSpringGreen, Color.MediumTurquoise, Color.MediumVioletRed, Color.MidnightBlue, Color.MintCream, Color.MistyRose, Color.Moccasin, Color.NavajoWhite, Color.Navy, Color.OldLace, Color.Olive, Color.OliveDrab, Color.Orange, Color.OrangeRed, Color.Orchid, Color.PaleGoldenrod, Color.PaleGreen, Color.PaleTurquoise, Color.PaleVioletRed, Color.PapayaWhip, Color.PeachPuff, Color.Peru, Color.Pink, Color.Plum, Color.PowderBlue, Color.Purple, Color.Red, Color.RosyBrown, Color.RoyalBlue, Color.SaddleBrown, Color.Salmon, Color.SandyBrown, Color.SeaGreen, Color.SeaShell, Color.Sienna, Color.Silver, Color.SkyBlue, Color.SlateBlue, Color.SlateGray, Color.Snow, Color.SpringGreen, Color.SteelBlue, Color.Tan, Color.Teal, Color.Thistle, Color.Tomato, Color.Transparent, Color.Turquoise, Color.Violet, Color.Wheat, Color.White, Color.WhiteSmoke, Color.Yellow, Color.YellowGreen };
        Color backgroundColor = Color.CornflowerBlue;
        Color textColor = Color.White;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set resolution and make mouse visible
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            
            currentClickInputs = new List<ClickInfo>();

            keyFilter = new KeyFilter();
            mouseFilter = new MouseFilter();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            pipe1Sprite = new Sprite(Content, "pipe1");
            pipe1FilledSprite = new Sprite(Content, "pipe1_s");
            pipe2Sprite = new Sprite(Content, "pipe2");
            pipe2FilledSprite = new Sprite(Content, "pipe2_s");
            pipe2bSprite = new Sprite(Content, "pipe2b");
            pipe2bFilledSprite = new Sprite(Content, "pipe2b_s");
            pipe3Sprite = new Sprite(Content, "pipe3");
            pipe3FilledSprite = new Sprite(Content, "pipe3_s");
            pipe4Sprite = new Sprite(Content, "pipe4");
            pipe4FilledSprite = new Sprite(Content, "pipe4_s");

            orderedSpriteList = new List<Sprite>() { pipe1Sprite, pipe1FilledSprite, pipe2Sprite, pipe2FilledSprite, pipe2bSprite, pipe2bFilledSprite, pipe3Sprite, pipe3FilledSprite, pipe4Sprite, pipe4FilledSprite };

            spriteFont = Content.Load<SpriteFont>("Arial");
            

            // max size that fits in window is 46, 26
            visualPipeGame = new VisualPipeGame(orderedSpriteList ,WINDOW_WIDTH-200,WINDOW_HEIGHT,200,0);
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
            
            mouseState = Mouse.GetState();
            mouseFilter.Update(mouseState, ref currentClickInputs);
            if(currentClickInputs.Count > 0)
            {
                visualPipeGame.HandleMouseInput(currentClickInputs[0]);
                currentClickInputs.RemoveAt(0);
            }

            keyboardState = Keyboard.GetState();
            pressedKeys = keyFilter.GetPressedKeys(keyboardState);
            if (pressedKeys.Contains(Keys.Escape))
            {
                this.Exit();
            }
            else if (pressedKeys.Contains(Keys.C))
            {
                backgroundColor = allColorOptions[m.Random.Next(allColorOptions.Count)];
                textColor = allColorOptions[m.Random.Next(allColorOptions.Count)];
            }

            visualPipeGame.HandleKeyboardInput(pressedKeys);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            visualPipeGame.Draw(spriteBatch);
            
            //spriteBatch.DrawString(spriteFont, "X: " + mouseState.X + ", Y: " + mouseState.Y, new Vector2((float)(WINDOW_WIDTH * (5f / 6f)), (float)(WINDOW_HEIGHT * (0f / 6f))), Color.White);
            spriteBatch.DrawString(spriteFont, "Controls:", new Vector2(20, 20),textColor);
            spriteBatch.DrawString(spriteFont, "Click pipe to rotate", new Vector2(20, 60), textColor);
            spriteBatch.DrawString(spriteFont, "N - New puzzle", new Vector2(20, 100), textColor);
            spriteBatch.DrawString(spriteFont, "R - Reset puzzle", new Vector2(20,140), textColor);
            spriteBatch.DrawString(spriteFont, "H - Hint", new Vector2(20, 180), textColor);
            spriteBatch.DrawString(spriteFont, "S - Solve puzzle", new Vector2(20, 220), textColor);
            spriteBatch.DrawString(spriteFont, "C - Change Colors", new Vector2(20, 260), textColor);
            spriteBatch.DrawString(spriteFont, "Arrows - Change", new Vector2(20, 300), textColor);
            spriteBatch.DrawString(spriteFont, "Puzzle Dimensions", new Vector2(20, 325), textColor);

            if (visualPipeGame.AllFilled)
            {
                spriteBatch.DrawString(spriteFont, visualPipeGame.PuzzleSolved ? "You win!" : "You win?", new Vector2(550, 290), Color.Black, 0, new Vector2(0,0), 5, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
    }
}
