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

namespace Tenta_Spel
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gamefont;

        WindowManager wm;
        GameObjectController goc = new GameObjectController();
        Player player;
        UIManager uim;// = new UIManager();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            

            wm = new WindowManager(graphics);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gamefont = Content.Load<SpriteFont>("Utskrift/GameFont");
            
            string imgPath = @"..\..\..\..\Tenta_SpelContent\Sprites\";

            for (int i = 0; i < System.IO.Directory.GetFiles(imgPath).Length; i++)
            {
                string tempPath = System.IO.Directory.GetFiles(imgPath)[i];
                Texture2D tempTexture = Texture2D.FromStream(GraphicsDevice, new System.IO.FileStream(tempPath, System.IO.FileMode.Open));
                goc.textureList.Add(System.IO.Directory.GetFiles(imgPath)[i].Substring(tempPath.LastIndexOf('\\') + 1).Split('.')[0], tempTexture);
            }
            uim = new UIManager(goc);
            goc.Activate();

            player = new Player(goc);
            new Ship(goc,"Ship1", new Vector2(0, 5));
            new Planet(goc, "Planet0", new Vector2(0, 0));
            new Explosion(goc, "Explosion0", new Vector2(0, 0), 100);

            MediaPlayer.Play(Content.Load<Song>("Sound/Mars"));

            UIContainer statusbar = new UIContainer(uim, new Vector2(0, graphics.PreferredBackBufferHeight/90), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight/10), Color.Black ,2);

            //WINDOWINPUTSCALE
            wm.WindowScaleSet(0.6f);
            wm.WindowScale(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                wm.WindowScaleScale(0.1f);
                wm.WindowScale(GraphicsDevice);
            }

            if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                wm.WindowScaleScale(-0.1f);
                wm.WindowScale(GraphicsDevice);
            }

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                foreach (GameObject go in goc.gos)
                {
                    Console.Write(go.T + " | " + typeof(Bullet) + " |");
                    if (go.T == typeof(Bullet))
                    {
                        Console.Write(" isTrue");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("--- --- ---");

            }
            //UPDATE

            goc.Update(graphics);

            player.Update(keyboardState);


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            for (int i = 0; i < 4; i++)
            {
                goc.bm.Draw(spriteBatch);
                //spriteBatch.Draw(goc.textureList["BG1"], goc.bm.pos[i], null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }

            for (int i = 6; i > 0; i--)
            {
                foreach (GameObject go in goc.gos)
                {
                    if (Math.Floor((double)go.rendLayer) == i)
                    {
                        go.Draw(spriteBatch, graphics);
                    }
                }
            }

            //UI

            foreach (UIContainer uc in uim.uiCs)
            {
                //uc.Draw(spriteBatch, graphics);
            }

            new UIContainer(uim, new Vector2(0, graphics.PreferredBackBufferHeight - 30), new Vector2(graphics.PreferredBackBufferWidth, 30), Color.Red, 2).Draw(spriteBatch,graphics);
            spriteBatch.DrawString(gamefont, (new Vector2(Convert.ToInt32(goc.player.pos.X / 100), Convert.ToInt32(goc.player.pos.Y / 100))).ToString(), new Vector2(2, 2), Color.Yellow);


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
