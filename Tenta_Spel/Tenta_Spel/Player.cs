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
        Ship ship;
        GameObjectController goc;

        bool dead = false;

        public Player(GameObjectController gocSet)
        {
            goc = gocSet;
            ship = new Ship(goc, "Ship0", new Vector2(0, 0));
            goc.player = ship;
            ship.rendLayer = 1;
        }

        public void Update(KeyboardState keyboardState)
        {
            Movement(keyboardState);
            ship.rendLayer = 1;
            ship.tickShootCooldown = Math.Min(ship.tickShootCooldown + 1, goc.player.shootCooldown);
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
                spriteBatch.Draw(goc.player.sprite, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, goc.player.rotation, goc.player.Raduis(), 1f, SpriteEffects.None, 0);
            }
        }
    }
}
