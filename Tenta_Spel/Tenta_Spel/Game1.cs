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
        //Grundvariabler
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gamefont;

        WindowManager wm;
        GameObjectController goc;
        UIManager uim;

        string menuMessage = "EDGE OF THE UNIVERSE";

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
            //initciering

            wm = new WindowManager(graphics);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gamefont = Content.Load<SpriteFont>("Utskrift/GameFont");

            
            Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();
            string[] loadTextures = System.IO.Directory.GetFiles("Content/Sprites/");
            Console.WriteLine(loadTextures);

            for (int i = 0; i < loadTextures.Length; i++)
            {
                //Console.Write(loadTextures[i] + " | ");
                string setter = loadTextures[i].Split(new char[1]{'/'})[2].Split(new char[1]{'.'})[0];
                //Console.WriteLine(setter);
                textureList.Add(setter, Content.Load<Texture2D>("Sprites/" + setter));
                
            }
            
            goc = new GameObjectController(textureList);
            uim = new UIManager(goc);

            wm.WindowScaleSet(1f);
            wm.WindowScale(GraphicsDevice);

            IsMouseVisible = true;
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            
            //För att skala spelfönstret, fungerar inte bra med huvudmenyn
            /* Fungerar bara bra vid debug
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
            //*/
            
            //UPDATE

            //Om spelet inte är startat, Update
            if (!goc.gocActivate)
            {
                IsMouseVisible = true;
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    //startar speldelen av spelet
                    goc.Activate();

                    IsMouseVisible = false;
                }
                
            }

            //Om spelet är startat, Update
            if (goc.gocActivate)
            {
                goc.Update(graphics);
                goc.player.Update(keyboardState);

                if (goc.player.Win() || goc.player.Lose())
                {
                    if (goc.player.Win())
                    {
                        menuMessage = "YOU WON!";
                    }

                    if (goc.player.Lose())
                    {
                        menuMessage = "YOU LOSE!";
                    }
                }
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            
            //grafik för om speldelen av spelet är startat
            if (goc.gocActivate)
            {
                //bakgrund
                goc.bm.Draw(spriteBatch);

                //loopar igenom alla objekt efter deras renderlayer vilket gör att de ritas på varandra i rätt ordning, ex. planeter ritas bakom skepp och kometer
                for (int r = 6; r > 0; r--)
                {
                    foreach (GameObject go in goc.gos)
                    {
                        if (Math.Floor((double)go.rendLayer) == r)
                        {
                            go.Draw(spriteBatch, graphics);
                        }
                    }
                }

                
            
                //UI

                //rita spelarens föråd
                goc.player.inv.Draw(spriteBatch, graphics, gamefont);

                //ui för hälsa och värden
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

                //Animation för att gå i måll eller förlora
                goc.DrawFade(spriteBatch, graphics, uim);
            }

            //grafik för om huvudmenyn visas
            if (!goc.gocActivate)
            {
                //ui för huvudmenyn
                UIContainer startmenu = new UIContainer(uim, new Vector2(400, 200), new Vector2(graphics.PreferredBackBufferWidth - 400*2, graphics.PreferredBackBufferHeight - 400), Color.Gray, 5);
                UIButton startButton = new UIButton(startmenu, new Vector2(500, 450), new Vector2(graphics.PreferredBackBufferWidth - 800 -200, graphics.PreferredBackBufferHeight - 600-100), Color.DarkSlateBlue);
                UIText startText = new UIText(startmenu, startButton.pos + new Vector2(startButton.size.X/2, startButton.size.Y/3), startButton.size, new Color(-startButton.clr.R, -startButton.clr.G, -startButton.clr.B), gamefont, "START");
                
                UIText titleText = new UIText(startmenu, startText.pos + new Vector2(0, -200), startButton.size, Color.Red, gamefont, menuMessage);
                titleText.pos.X -= (titleText.text.Length)*9/1.5f;
                startText.pos.X -= (startText.text.Length-1)*9;

                //starta spelet om startknappen är tryckt
                if (startButton.ButtonPressed())
                {
                    goc.Activate();
                    IsMouseVisible = false;
                }

                //ui för hjälpmenyn
                UIContainer helpMenu = new UIContainer(uim, new Vector2(10, 20*8), new Vector2(100, 25), new Color(0, 0, 0, 20), 2);
                new UIText(helpMenu, helpMenu.rendObj.pos, helpMenu.rendObj.size, Color.Gray, gamefont, "[H] Help");

                //visas bara om man tryckt på H knappen för att få hjälp
                if (Keyboard.GetState().IsKeyDown(Keys.H))
                {
                    int i = 2;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "[ESC] Exit Game");
                    i++;
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Movement:");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "[W] Forward");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "[A][D] Rotate");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "[K] Shoot");
                    i++;
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Goal:");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Reach the Edge of the Universe (at");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, Player.winDistance/100 + " possitive or negative on the x or y axsis)");
                    i++;
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Obsticles:");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Comets can strike from any direction");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Limited Resources (Fuel, Health, Ammunition)");
                    i++;
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Crafting");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "Gather Resources on planets to craft, and ");
                    i++;
                    new UIText(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * i), helpMenu.rendObj.size, Color.Gray, gamefont, "replenish Vitality (Fuel, Health, Ammunition)");
                    i++;

                    new UIBlock(helpMenu, helpMenu.rendObj.pos + new Vector2(0, 20 * 2), helpMenu.rendObj.size + new Vector2(285, 20 * (i-3) +3), new Color(0, 0, 0, 30));

                }

                startmenu.Draw(spriteBatch, graphics);
                helpMenu.Draw(spriteBatch, graphics);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
