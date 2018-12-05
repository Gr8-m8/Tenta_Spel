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
        UIManager uim = new UIManager();

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
            goc.Activate();
            player = new Player(goc);
            new Ship(goc,"Ship1", new Vector2(0, 5));
            new Planet(goc, "Planet0", new Vector2(0, 0));
            new Explosion(goc, "Explosion0", new Vector2(0, 0), 100);

            MediaPlayer.Play(Content.Load<Song>("Sound/Mars"));

            UIContainer statusbar = new UIContainer(uim, new Vector2(0, graphics.PreferredBackBufferHeight/90), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight/10), Color.Black ,2);

            //WINDOWINPUTSCALE
            wm.WindowScaleSet(1f);
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
                spriteBatch.Draw(goc.textureList["BG1"], goc.bm.pos[i], null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }

            foreach (GameObject go in goc.gos)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (go.rendLayer == i)
                    {
                        go.Draw(spriteBatch, graphics);
                    }
                }
            }

            
            //spriteBatch.Draw(goc.player.sprite, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, goc.player.rotation, goc.player.Raduis(), 1f, SpriteEffects.None, 0);
            

            //UI

            foreach (UIContainer uc in uim.uiCs)
            {
                foreach (UIElement ue in uc.uiElements)
                {

                }
            }
            //*
            spriteBatch.DrawString(gamefont, (new Vector2(Convert.ToInt32(goc.player.pos.X / 100), Convert.ToInt32(goc.player.pos.Y / 100))).ToString(), new Vector2(2, 2), Color.Yellow);
            /*
            spriteBatch.Draw(goc.GetTexture(""), statusbar.dims(), Color.Black);
            for (int i = 0; i < 6; i++)
            {
                spriteBatch.Draw(goc.GetTexture(""), new Rectangle(uiPos[0] + borderSize + i * blockSize[0], uiPos[1] + borderSize, blockSize[0] - borderSize, blockSize[1]), uiclr);
                //spriteBatch.Draw(goc.GetTexture(""), new Rectangle(uiPos[0] + 2 * borderSize + i * blockSize[0], uiPos[1] + borderSize, barsize[0], barsize[1]), Color.Red);
                //spriteBatch.Draw(goc.GetTexture(""), new Rectangle(uiPos[0] + borderSize + i * blockSize[0], uiPos[1] + borderSize, barsize[0], barsize[1]), Color.Green);
            }
            */
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
