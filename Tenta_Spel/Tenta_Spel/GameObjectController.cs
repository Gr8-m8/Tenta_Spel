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
    class GameObjectController
    {
        public BackgroundManager bm;

        public List<GameObject> gos = new List<GameObject>();
        //public List<Ship> ships = new List<Ship>();
        //public List<Planet> planets = new List<Planet>();
        //public List<Bullet> bullets = new List<Bullet>();
        public Ship player;

        public Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

        public Vector2 cameraPos = new Vector2(25, 25);

        public GameObjectController()
        {

        }

        public void Activate()
        {
            bm = new BackgroundManager(this);

        }

        public Texture2D GetTexture(string skey)
        {
            if (textureList.ContainsKey(skey))
            {
                return textureList[skey];
            }
            else
            {
                return textureList["Blank"];
            }
        }

        public void Update(GraphicsDeviceManager graphics)
        {
            bm.Move();

            List<GameObject> markedForDelete = new List<GameObject>();

            foreach (GameObject go in gos)
            {
                go.Update();

                if (go.markForDelete)
                {
                    markedForDelete.Add(go);
                }
            }

            while (markedForDelete.Count > 0)
            {
                GameObject last = markedForDelete.Last();
                if (last.explode)
                {
                    new Explosion(this, "Explosion0", last.pos, last.Raduis().X);
                }

                gos.Remove(last);
                markedForDelete.Remove(last);
            }

            SpawnerComet(graphics);
            SpawnerPlanet(graphics);
        }

        public void SpawnerComet(GraphicsDeviceManager graphics)
        {
            Random r = new Random();
            int randomCometChance = 10000;
            if (r.Next(randomCometChance) > randomCometChance - 20)
            {
                Vector2 spawnPos = Vector2.Zero;
                switch (r.Next(4))
                {
                    case 0:
                        spawnPos = new Vector2(this.player.pos.X + graphics.PreferredBackBufferHeight, this.player.pos.Y + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2));
                        break;

                    case 1:
                        spawnPos = new Vector2(this.player.pos.X - graphics.PreferredBackBufferHeight, this.player.pos.Y + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2));
                        break;

                    case 2:
                        spawnPos = new Vector2(this.player.pos.X + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2), this.player.pos.Y + graphics.PreferredBackBufferWidth);
                        break;

                    case 3:
                        spawnPos = new Vector2(this.player.pos.X + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2), this.player.pos.Y - graphics.PreferredBackBufferWidth);
                        break;
                }

                Bullet cmt = new Bullet(this, "Asteroid0", spawnPos, (this.player.pos - spawnPos) * 0.004f, 34, 0);
                //Bullet cmt = new Bullet(this, "Asteroid0", spawnPos, new Vector2(Convert.ToInt32(r.Next(-1, 1)),Convert.ToInt32(r.Next(-1, 1))) * 3, 34, 0);
            }
        }

        public int SpawnerPlanet(GraphicsDeviceManager graphics)
        {
            Random r = new Random();
            int randomPlanetChance = 10000;
            if (r.Next(randomPlanetChance) > randomPlanetChance - 10)
            {
                Vector2 spawnPos = this.player.pos + this.player.Forward() * 5000;
                foreach (GameObject go in this.gos)
                {
                    if (go.T == typeof(Planet))
                    {
                        float dist = (float)Math.Sqrt((go.pos.X - spawnPos.X) * (go.pos.X - spawnPos.X) + (go.pos.Y - spawnPos.Y) * (go.pos.Y - spawnPos.Y));

                        if (dist < 1000)
                        {
                            return 1;
                        }
                    }
                }

                Planet plnt = new Planet(this, "Planet0", spawnPos);
                //Console.WriteLine(spawnPos.X/100 + " " + spawnPos.Y/100);
                return 0;
            }
            return -1;
        }
    }

    class Camera
    {
        public Vector2 pos;
        float zoom;

        public void PosSet(Vector2 posSet)
        {
            pos = posSet;
        }

        public void Move(Vector2 moveBy)
        {
            pos += moveBy;
        }
    }

    class BackgroundManager
    {
        GameObjectController goc;
        public Texture2D[] textures = new Texture2D[4];

        public Vector2[] pos = new Vector2[4];
        public Vector2 dim;
        public float speed = 0.05f;

        public BackgroundManager(GameObjectController gocSet)
        {
            goc = gocSet;
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = goc.textureList["BG1"];
            }

            dim = new Vector2(textures[0].Width, textures[0].Height);
        }

        public void Move()
        {

            pos[0] = goc.player.pos * -speed;
            pos[0].X %= dim.X;
            pos[0].X -= dim.X;
            pos[0].X %= dim.X;
            pos[0].Y %= dim.Y;
            pos[0].Y -= dim.Y;
            pos[0].Y %= dim.Y;

            pos[0] = pos[0] + new Vector2(0, 0);
            pos[1] = pos[0] + new Vector2(dim.X, 0);
            pos[2] = pos[0] + new Vector2(0, dim.Y);
            pos[3] = pos[0] + new Vector2(dim.X, dim.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(goc.textureList["BG1"], pos[i], null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }
        }
    }
}
