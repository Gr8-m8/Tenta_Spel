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
    //classen för att kontrollera alla objekt
    class GameObjectController
    {
        public bool gocActivate = false;
        public BackgroundManager bm;

        public List<GameObject> gos = new List<GameObject>();
        public Player player;

        public Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

        public Vector2 cameraPos = new Vector2(25, 25);

        public GameObjectController(Dictionary<string, Texture2D> textureListSet)
        {
            textureList = textureListSet;
        }

        //aktiverar speldelen av spelet
        public void Activate()
        {
            gocActivate = true;
            bm = new BackgroundManager(this);

            player = new Player(this);

            new Planet(this, "Planet0", new Vector2(0, 0));

            fade = 0;
            colorChange = 0;
        }

        //avaktioverar speldelen av spelet och aktiverar huvudmeny
        public void DeActivate()
        {
            gocActivate = false;
            gos = new List<GameObject>();
        }

        //Animation för att vinna eller förlora
        float fade = 0;
        float colorChange = 0;
        public void DrawFade(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, UIManager uimSet)
        {
            if (player.Win() || player.Lose())
            {
                new UIContainer(uimSet, new Vector2(0, 0), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), new Color(Convert.ToInt32(colorChange), Convert.ToInt32(colorChange), Convert.ToInt32(colorChange), Convert.ToInt32(fade)), 0).Draw(spriteBatch, graphics);

                if (player.Lose())
                {
                    if (fade <= 255)
                    {
                        fade += 2;
                    }

                    if (fade >= 255)
                    {
                        colorChange += 6f;
                    }

                    if (colorChange >= 255)
                    {
                        DeActivate();
                    }
                }

                if (player.Win())
                {
                    if (fade <= 255)
                    {
                        fade += 6;
                    }

                    if (colorChange <= 255)
                    {
                        colorChange += 5;
                    }

                    if (fade >= 255)
                    {
                        DeActivate();
                    }
                }
            }
        }

        //funktion för att retunera en exsisterande textur från de som finns.
        //Retunerar "Blank" textur om nyckeln för texturen inte finns
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

        //sköter updatering av objekt (fysik och rörelse)
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

            //raderar oanvända objekt
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

        //skapar kometer
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
                        spawnPos = new Vector2(this.player.ship.pos.X + graphics.PreferredBackBufferHeight, this.player.ship.pos.Y + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2));
                        break;

                    case 1:
                        spawnPos = new Vector2(this.player.ship.pos.X - graphics.PreferredBackBufferHeight, this.player.ship.pos.Y + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2));
                        break;

                    case 2:
                        spawnPos = new Vector2(this.player.ship.pos.X + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2), this.player.ship.pos.Y + graphics.PreferredBackBufferWidth);
                        break;

                    case 3:
                        spawnPos = new Vector2(this.player.ship.pos.X + r.Next(-graphics.PreferredBackBufferHeight / 2, graphics.PreferredBackBufferHeight / 2), this.player.ship.pos.Y - graphics.PreferredBackBufferWidth);
                        break;
                }

                Bullet cmt = new Bullet(this, "Asteroid0", spawnPos, (this.player.ship.pos - spawnPos) * 0.004f, 34, 0);
                //Bullet cmt = new Bullet(this, "Asteroid0", spawnPos, new Vector2(Convert.ToInt32(r.Next(-1, 1)),Convert.ToInt32(r.Next(-1, 1))) * 3, 34, 0);
            }
        }

        //skapar planeter
        public int SpawnerPlanet(GraphicsDeviceManager graphics)
        {
            Random r = new Random();
            int randomPlanetChance = 10000;
            if (r.Next(randomPlanetChance) > randomPlanetChance - 6)
            {
                int lng = 5000;
                Vector2 spawnPos = this.player.ship.pos + this.player.ship.Forward() * lng;
                foreach (GameObject go in this.gos)
                {
                    if (go.T == typeof(Planet))
                    {
                        float dist = (float)Math.Sqrt((go.pos.X - spawnPos.X) * (go.pos.X - spawnPos.X) + (go.pos.Y - spawnPos.Y) * (go.pos.Y - spawnPos.Y));

                        if (dist < lng)
                        {
                            return 0;
                        }
                    }
                }

                Planet plnt = new Planet(this, "Planet0", spawnPos);
                return 1;
            }
            return 0;
        }
    }

    //klass för att hantera bakgrund
    class BackgroundManager
    {
        GameObjectController goc;
        public Texture2D texture;

        public Vector2[] pos = new Vector2[4];
        public Vector2 dim;
        public float speed = 0.05f;

        public BackgroundManager(GameObjectController gocSet)
        {
            goc = gocSet;
            texture = goc.GetTexture("BG0");

            dim = new Vector2(texture.Width, texture.Height);
        }

        //flyttar bakgrunden så den loopar oändligt för spelaren
        public void Move()
        {
            pos[0] = goc.player.ship.pos * -speed;
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

        //funktion för att rita bakgrunden
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 winPos = goc.player.ship.pos;
            if (winPos.X < 0) { winPos.X *= -1; }
            if (winPos.Y < 0) { winPos.Y *= -1; }

            int fade = 255;
            int fadeDist = 100 * 10;
            int playerGoalDist = Player.winDistance - Convert.ToInt32(Math.Max(winPos.X, winPos.Y));

            if (playerGoalDist < fadeDist)
            {
                fade = (int)(255f * (float)playerGoalDist / fadeDist);
            }

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(texture, pos[i], null, new Color(255, 255, 255, Math.Max(0, 100 + fade)), 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }
        }
    }
}
