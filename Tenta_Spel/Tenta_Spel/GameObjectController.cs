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

        public Game1 gctrl;
        public BackgroundManager bm;

        public List<GameObject> gos = new List<GameObject>();
        public List<Ship> ships = new List<Ship>();
        public List<Planet> planets = new List<Planet>();
        public List<Bullet> bullets = new List<Bullet>();
        public Ship player;

        public Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

        public Vector2 cameraPos = new Vector2(25, 25);

        public GameObjectController()
        {

        }

        public void Activate()
        {
            bm = new BackgroundManager(this);

            player = new Ship(this, "Ship0", new Vector2(0, 0));
            gos.Add(player);
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
    }
}
