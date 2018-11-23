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
            //new Ship(goc,"Ship1", new Vector2(0, 5));
            new Planet(goc, "Planet0", new Vector2(0, 0), Color.White);

            MediaPlayer.Play(Content.Load<Song>("Sound/Mars"));

            UIContainer statusbar = new UIContainer(uim, new Vector2(0, graphics.PreferredBackBufferHeight/90), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight/10), Color.Black ,2);

            float windowScale = Math.Min(1f, 1f);
            graphics.PreferredBackBufferWidth = Convert.ToInt32(GraphicsDevice.DisplayMode.Width * windowScale);
            graphics.PreferredBackBufferHeight = Convert.ToInt32(GraphicsDevice.DisplayMode.Height * windowScale);
            if (windowScale == 1)
            {
                graphics.IsFullScreen = true;
            }
            graphics.ApplyChanges();
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

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                foreach (GameObject go in goc.gos)
                {
                    Console.Write(go + " |");
                }
                Console.WriteLine();
            }

            player.Movement(keyboardState);

            goc.bm.Move();

            List<GameObject> markedForDelete = new List<GameObject>();

            foreach (GameObject go in goc.gos)
            {
                go.ForceMove();

                if (go.markForDelete)
                {
                    markedForDelete.Add(go);
                }
            }

            while(markedForDelete.Count > 0)
            {
                goc.gos.Remove(markedForDelete.Last<GameObject>());
                markedForDelete.Remove(markedForDelete.Last<GameObject>());
            }

            foreach (GameObject go in goc.bullets)
            {
                go.ForceMove();

                if (go.markForDelete)
                {
                    markedForDelete.Add(go);
                }
            }

            while (markedForDelete.Count > 0)
            {
                goc.bullets.Remove(markedForDelete.Cast<Bullet>().Last<Bullet>());
                markedForDelete.Remove(markedForDelete.Last<GameObject>());
            }

            foreach (GameObject go in goc.ships)
            {
                go.ForceMove();

                if (go.markForDelete)
                {
                    markedForDelete.Add(go);
                }
            }

            while (markedForDelete.Count > 0)
            {
                goc.ships.Remove(markedForDelete.Cast<Ship>().Last<Ship>());
                markedForDelete.Remove(markedForDelete.Last<GameObject>());
            }

            foreach (GameObject go in goc.planets)
            {
                go.ForceMove();

                if (go.markForDelete)
                {
                    markedForDelete.Add(go);
                }
            }

            while (markedForDelete.Count > 0)
            {
                goc.planets.Remove(markedForDelete.Cast<Planet>().Last<Planet>());
                markedForDelete.Remove(markedForDelete.Last<GameObject>());
            }

            Random r = new Random();
            if (r.Next(10000) > 10000 - 2)
            {
                Vector2 spawnPos = new Vector2(goc.player.pos.X + graphics.PreferredBackBufferHeight, goc.player.pos.Y + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2));
                Bullet go = new Bullet(goc, "Asteroid0", spawnPos, (goc.player.pos - spawnPos) * 0.004f, 50, 10000);
            }

            goc.player.tickShootCooldown = Math.Min(goc.player.tickShootCooldown + 1, goc.player.shootCooldown);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(goc.textureList["BG1"], goc.bm.pos[i], null, Color.White);
            }

            if (goc.planets.Count > 0)
            {
                foreach (Planet go in goc.planets)
                {
                    if (go != null)
                    {
                        spriteBatch.Draw(go.sprite, go.pos - goc.player.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, go.planetColor, go.rotation, go.Raduis(), go.scale, SpriteEffects.None, 0);
                    }
                }
            }

            foreach (GameObject go in goc.gos)
            {
                if (go != null)
                {
                    spriteBatch.Draw(go.sprite, go.pos - goc.player.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, go.rotation, go.Raduis(), 1f, SpriteEffects.None, 0);
                }
            }
            
            foreach (GameObject go in goc.ships)
            {
                if (go != null)
                {
                    if (go != goc.player)
                    {
                        spriteBatch.Draw(go.sprite, go.pos - goc.player.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, go.rotation, go.Raduis(), 1f, SpriteEffects.None, 0);
                    }
                }
            }

            if (goc.bullets.Count > 0)
            {
                foreach (GameObject go in goc.bullets)
                {
                    if (go != null)
                    {
                        spriteBatch.Draw(go.sprite, go.pos - goc.player.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, go.rotation, go.Raduis(), 1f, SpriteEffects.None, 0);
                    }
                }
            }

            spriteBatch.Draw(goc.player.sprite, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, goc.player.rotation, goc.player.Raduis(), 1f, SpriteEffects.None, 0);


            //UI

            foreach (UIContainer uc in uim.uiCs)
            {
                foreach (UIElement ue in uc.uiElements)
                {

                }
            }
            /*
            spriteBatch.DrawString(gamefont, (new Vector2(Convert.ToInt32(goc.player.pos.X / 100), Convert.ToInt32(goc.player.pos.Y / 100))).ToString(), new Vector2(2, 2), Color.White);

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
