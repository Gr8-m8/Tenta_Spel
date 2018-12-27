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
        GameObjectController goc;
        UIManager uim;

        string menuMessage = "THE EDGE OF THE UNIVERSE";

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

            goc = new GameObjectController(GraphicsDevice);
            uim = new UIManager(goc);

            //WINDOWINPUTSCALE
            wm.WindowScaleSet(1f);
            wm.WindowScale(GraphicsDevice);

            //MediaPlayer.Play(Content.Load<Song>("Sound/Mars"));

            IsMouseVisible = true;
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

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                if (!goc.gocActivate)
                {
                    goc.Activate();
                    IsMouseVisible = false;
                }
            }

            //UPDATE

            if (goc.gocActivate)
            {
                goc.Update(graphics);
                goc.player.Update(keyboardState);

                if (goc.player.Win() || goc.player.Loose())
                {
                    goc.DeActivate();
                    IsMouseVisible = true;
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            if (goc.gocActivate)
            {
                for (int i = 0; i < 4; i++)
                {
                    goc.bm.Draw(spriteBatch);
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

                Vector2 winPos = goc.player.ship.pos;
                if (winPos.X < 0)
                {
                    winPos.X *= -1;
                }

                if (winPos.Y < 0)
                {
                    winPos.Y *= -1;
                }

                int fade = 0;
                int fadeDist = 100 * 10;
                int playerGoalDist = goc.player.winDistance - Convert.ToInt32(Math.Max(winPos.X, winPos.Y));

                if (playerGoalDist < fadeDist)
                {
                    fade = 255 - (int)(255f * (float)playerGoalDist / fadeDist);                    
                }
                Console.WriteLine();

                new UIContainer(uim, new Vector2(0, 0), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), new Color(fade, fade, fade, fade), 5).Draw(spriteBatch, graphics);
            }

            //UI

            if (goc.gocActivate)
            {
                goc.player.inv.Draw(spriteBatch, graphics, gamefont);

                UIContainer statusbar = new UIContainer(uim, new Vector2(0, graphics.PreferredBackBufferHeight - 30), new Vector2(graphics.PreferredBackBufferWidth, 30), Color.Gray, 2);
                int i = 0;
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5*(i+1) + 200*i, 5), new Vector2(200, 20), Color.Red);
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5*(i+1) + 200*i, 5), new Vector2(200 * (goc.player.fuel/7000f), 20), Color.Green);
                new UIText(statusbar, statusbar.rendObj.pos + new Vector2(5*(i+1) + 200*i, 5), new Vector2(200, 20), Color.Yellow, gamefont, goc.player.fuel/10 + "/700 fuel");
                i++;
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200, 20), Color.Red);
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200 * goc.player.ship.hp/100, 20), Color.Green);
                new UIText(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200, 20), Color.Yellow, gamefont, goc.player.ship.hp.ToString() + "/100 hp");
                i++;
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200, 20), Color.Red);
                new UIBlock(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200 * goc.player.ship.bulletCount/100, 20), Color.Green);
                new UIText(statusbar, statusbar.rendObj.pos + new Vector2(5 * (i + 1) + 200 * i, 5), new Vector2(200, 20), Color.Yellow, gamefont, goc.player.ship.bulletCount.ToString() + "/100 bullets");

                statusbar.Draw(spriteBatch, graphics);

                spriteBatch.DrawString(gamefont, (new Vector2(Convert.ToInt32(goc.player.ship.pos.X / 100), Convert.ToInt32(goc.player.ship.pos.Y / 100))).ToString(), new Vector2(2, 2), Color.Yellow);

            }

            if (!goc.gocActivate)
            {
                
                UIContainer startmenu = new UIContainer(uim, new Vector2(400, 200), new Vector2(graphics.PreferredBackBufferWidth - 400*2, graphics.PreferredBackBufferHeight - 400), Color.Gray, 5);
                UIButton startButton = new UIButton(startmenu, new Vector2(500, 450), new Vector2(graphics.PreferredBackBufferWidth - 800 -200, graphics.PreferredBackBufferHeight - 600-100), Color.DarkSlateBlue);
                new UIText(startmenu, startButton.pos + startButton.size/2, startButton.size, new Color(-startButton.clr.R, -startButton.clr.G, -startButton.clr.B), gamefont, "START");

                new UIText(startmenu, new Vector2(500, 250), startButton.size, Color.Red, gamefont, menuMessage);

                if (startButton.ButtonPressed())
                {
                    goc.Activate();
                    IsMouseVisible = false;
                }

                startmenu.Draw(spriteBatch, graphics);
            }
            

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
