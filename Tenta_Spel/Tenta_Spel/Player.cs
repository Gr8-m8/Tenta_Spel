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

        public Inventory inv = new Inventory();

        bool dead = false;

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
        }

        public void Movement(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                ship.ForceAdd(ship.Forward(), 0.5f);
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                ship.rotation += 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                ship.rotation -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.C) || keyboardState.IsKeyDown(Keys.K))
            {
               ship.Shoot(goc, 10, 1);
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
