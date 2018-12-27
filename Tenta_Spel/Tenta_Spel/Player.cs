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
    class Player
    {
        public Ship ship;
        GameObjectController goc;

        public int fuel = 7000;

        public Inventory inv = new Inventory();

        bool dead = false;
        public int winDistance = 100 * 10000;

        public Player(GameObjectController gocSet)
        {
            goc = gocSet;
            ship = new Ship(goc, "Ship0", new Vector2(0, 0));
            goc.player = this;
            ship.rendLayer = 1;
        }

        public void Update(KeyboardState keyboardState)
        {
            Movement(keyboardState);
            ship.rendLayer = 1;
            ship.tickShootCooldown = Math.Min(ship.tickShootCooldown + 1, goc.player.ship.shootCooldown);

            Win();
        }

        public void Win()
        {
            if (ship.pos.X > winDistance || ship.pos.Y > winDistance || ship.pos.X < -winDistance || ship.pos.Y < -winDistance)
            {
                goc.gocActivate = false;
            }

        }

        public void Movement(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                if (fuel > 0)
                {
                    ship.ForceAdd(ship.Forward(), 0.5f);
                    fuel--;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                ship.rotation += 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                ship.rotation -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.K))
            {
               ship.Shoot(goc, 10, 1);
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                if (inv.content.ContainsKey("Iron"))
                {
                    if (inv.content.ContainsKey("Crystal"))
                    {
                        if (inv.content["Crystal"].quantity >= 2)
                        {
                            inv.RemoveItem(new Item("Iron", 1));
                            inv.RemoveItem(new Item("Crystal", 2));
                            ship.bulletCount = Math.Min(100, ship.bulletCount + 10);
                        }
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.F))
            {
                if (inv.content.ContainsKey("Uranium"))
                {
                    if (inv.content.ContainsKey("Aluminium"))
                    {
                        if (inv.content["Aluminium"].quantity >= 2)
                        {
                            inv.RemoveItem(new Item("Uranium", 1));
                            inv.RemoveItem(new Item("Aluminium", 2));
                            fuel = Math.Min(5000, fuel + 200);
                        }
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.J))
            {
                if (inv.content.ContainsKey("Titanium"))
                {
                    if (inv.content.ContainsKey("Iron"))
                    {
                        if (inv.content["Iron"].quantity >= 2)
                        {
                            inv.RemoveItem(new Item("Titanium", 1));
                            inv.RemoveItem(new Item("Iron", 2));
                            ship.hp = Math.Min(100, ship.hp + 10);
                        }
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.L))
            {
                if (inv.content.ContainsKey("Mineral Stone"))
                {
                    if (inv.content["Mineral Stone"].quantity >= 3)
                    {
                        inv.RemoveItem(new Item("Mineral Stone", 3));
                        inv.AddItem(new Mineral(""));
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            if (!dead)
            {
                spriteBatch.Draw(goc.player.ship.sprite, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, goc.player.ship.rotation, goc.player.ship.Raduis(), 1f, SpriteEffects.None, 0);
            }

            
        }
    }
}
