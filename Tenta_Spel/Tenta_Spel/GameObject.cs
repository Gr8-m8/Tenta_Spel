﻿using System;
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
    class GameObject
    {
        public GameObjectController goc;
        public Type T;
        public int rendLayer;

        public Texture2D sprite;
        int renderLayer = 4;


        float speed = 0.1f;
        public Vector2 pos;
        public float rotation;

        int hitboxRadius;

        public Vector2 velocity;
        float fullThrust = 0.6f;
        float friction = 0.99f;
        bool isGravity = false;

        public bool markForDelete = false;
        public bool explode = false;

        public GameObject(GameObjectController gocSet, string spriteKeySet, Vector2 startPos)
        {
            goc = gocSet;

            sprite = goc.GetTexture(spriteKeySet);
            pos = startPos;

            T = this.GetType();
            goc.gos.Add(this);
        }

        public List<GameObject> CollisionObjects()
        {
            List<GameObject> collObj = new List<GameObject>();
            foreach (GameObject go in goc.gos)
            {
                if (go != this)
                {
                    if (Math.Sqrt(((pos.X - go.pos.X) * (pos.X - go.pos.X)) + ((pos.Y - go.pos.Y) * (pos.Y - go.pos.Y))) <= Raduis().X + go.Raduis().X)
                    {
                        collObj.Add(go);
                    }
                }
            }
            return collObj;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {

            spriteBatch.Draw(sprite, pos - goc.player.ship.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, rotation, Raduis(), 1f, SpriteEffects.None, 1);
        }

        public virtual void Collision()
        {
            foreach (GameObject go in CollisionObjects())
            {
                if (go.T != typeof(Planet))
                {
                    ForceAdd(go.velocity, 0.97f);
                }
            }
        }

        public virtual Vector2 Raduis()
        {
            return new Vector2(sprite.Width / 2, sprite.Height / 2);
        }

        public Vector2 Forward()
        {
            return new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));
        }

        public void Move(Vector2 moveTo)
        {
            pos = moveTo;
        }

        public virtual void ForceMove()
        {
            velocity *= friction;
            pos += velocity;
        }

        public void ForceAdd(Vector2 dir, float acc)
        {
            velocity += dir * acc * fullThrust;
        }

        public void Create()
        {
            T = this.GetType();
        }

        public void Delete()
        {
            if (goc.gos.Contains(this))
            {
                goc.gos.Remove(this);
            }
        }

        public void Update()
        {
            ForceMove();
            Collision();
        }
    }

    class Ship : GameObject
    {
        List<Bullet> bullets = new List<Bullet>();

        public Ship(GameObjectController gocSet, string spriteKeySet, Vector2 startPos) : base(gocSet, spriteKeySet, startPos)
        {
            rendLayer = 4;
        }

        public int bulletCount = 100;
        public int shootCooldown = 20;
        public int tickShootCooldown = 0;

        public void Shoot(GameObjectController goc, float speed, int dmg)
        {
            if (hp > 0)
            {
                if (bulletCount > 0)
                {
                    if (tickShootCooldown >= shootCooldown)
                    {
                        tickShootCooldown = 0;
                        Vector2 canonPos = pos + Forward() * Raduis();

                        bulletCount--;
                        bullets.Add(new Bullet(goc, "Bullet", canonPos, velocity + Forward() * speed, dmg, 200));
                    }
                }
            }
        }

        public override void Collision()
        {
            foreach (GameObject go in CollisionObjects())
            {
                //base.Collision();
                if (go.T != typeof(Planet) || go.T != typeof(Bullet))
                {
                    //ForceAdd(go.velocity, 0.97f);
                }

                if (go.T == typeof(Bullet))
                {
                    Bullet blt = (Bullet)go;
                    if (!bullets.Contains(blt))
                    {
                        HeathManager(blt.dmg);
                        go.explode = true;
                        go.markForDelete = true;
                    }
                }
            }
        }

        public int hp = 100;
        public void HeathManager(int amount)
        {
            Console.WriteLine(hp + " | " + amount);
            hp -= amount;

            if (hp < 1)
            {
                explode = true;
                markForDelete = true;

                if (this == goc.player.ship)
                {
                    rendLayer = 9;
                }
            }
        }

        public void Beam()
        {

        }
    }

    class Bullet : GameObject
    {
        int tickAge = 0;
        int lifeSpan;

        public int dmg;

        public Bullet(GameObjectController gocSet, string spriteKeySet, Vector2 startPos, Vector2 dir, int dmgSet, int lifeSpanSet) : base(gocSet, spriteKeySet, startPos)
        {
            velocity = dir;
            dmg = dmgSet;
            lifeSpan = lifeSpanSet;

            rendLayer = 2;

            Create();
        }

        public override void ForceMove()
        {

            pos += velocity;

            float range = (float)Math.Sqrt((pos.X - goc.player.ship.pos.X) * (pos.X - goc.player.ship.pos.X) + (pos.Y - goc.player.ship.pos.Y) * (pos.Y - goc.player.ship.pos.Y));
            int maxRange = 2000;
            if (range > maxRange || range < -maxRange)
            {
                tickAge += 1;
                if (tickAge > lifeSpan)
                {
                    markForDelete = true;
                }
            } else
            {
                tickAge = 0;
            }
        }

        public override void Collision()
        {
            //base.Collision();
            foreach (GameObject go in CollisionObjects())
            {
                if (go.T == typeof(Bullet))
                {
                    if (this.sprite == goc.GetTexture("Asteroid0"))
                    {
                        //new WorldItem(goc, "Mineral0", this.pos);
                    }

                    explode = true;
                    go.markForDelete = true;

                }

                if (go.T == typeof(Ship))
                {
                    if (this.sprite == goc.GetTexture("Asteroid0"))
                    {
                        //new WorldItem(goc, "Mineral0", this.pos);
                    }

                    //explode = true;
                    //markForDelete = true;
                }
            }
        }
    }

    class Planet : GameObject
    {
        public Color planetColor;
        public float scale;
        //float rotationSpeed = 0.0002f - new Random().Next(1);

        public Planet(GameObjectController gocSet, string spriteKeySet, Vector2 startPos) : base(gocSet, spriteKeySet, startPos)
        {
            rendLayer = 4;
            Random r = new Random();
            planetColor = new Color(r.Next(255), r.Next(255), r.Next(255));
            scale = 1 + float.Parse((r.NextDouble() * 5).ToString());

            int minerals = r.Next(3, 3);
            for (int i = 0; i < minerals; i++)
            {
                WorldItem wi = new WorldItem(goc, "Mineral0", pos + 
                    new Vector2(r.Next(Convert.ToInt32(-Raduis().X), Convert.ToInt32(Raduis().X)), r.Next(Convert.ToInt32(-Raduis().X), Convert.ToInt32(Raduis().X))));

            }
        }

        public override void ForceMove()
        {
            rotation += 0.0002f;
            //rotation += rotationSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(sprite, pos - goc.player.ship.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, planetColor, rotation, Raduis(), scale, SpriteEffects.None, 1);
        }
    }

    class Explosion : GameObject
    {
        float lifespanMax = 5;
        float lifespan = 0;

        float radius;

        public Explosion(GameObjectController gocSet, string spriteKeySet, Vector2 startPos, float radiusSet) : base(gocSet, spriteKeySet, startPos)
        {
            rendLayer = 3;
            radius = (radiusSet/Raduis().X) * 1.25f;
        }

        float Scale(float x)
        {
            float t = 0.1f;
            float k = (float) Math.Sqrt(t);

            rotation += new Random().Next(-1, 2) * t;

            x = Math.Max(0, x);
            float y = Math.Max(0, -t*(x * x) + 2 * k * x + 1);

            if(y <= 0)
            {
                markForDelete = true;
            }

            return y * radius;
        }

        public override void Collision()
        {
            //base.Collision();

            foreach(GameObject go in CollisionObjects())
            {
                if (go.T == typeof(Ship))
                {
                    Ship shp = (Ship)go;
                    //shp.HeathManager(Convert.ToInt32(radius));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            lifespan += 0.1f;
            rotation += new Random().Next(-2, 3) * 0.05f;
            spriteBatch.Draw(sprite, pos - goc.player.ship.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, rotation, Raduis(), Scale(lifespan), SpriteEffects.None, 1);
        }
    }

    class WorldItem : GameObject
    {
        Item itm = new Item("", 3);

        public WorldItem(GameObjectController gocSet, string spriteKeySet, Vector2 startPos) : base(gocSet, spriteKeySet, startPos)
        {
            rendLayer = 2;
            
        }

        public override void Collision()
        {
            foreach (GameObject go in CollisionObjects())
            {
                if (go.T == typeof(Ship))
                {
                    if (go == goc.player.ship)
                    {
                        goc.player.inv.AddItem(new Mineral("", 0));
                        markForDelete = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(sprite, pos - goc.player.ship.pos + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.Gold, rotation, Raduis(), 1f, SpriteEffects.None, 1);
        }
    }
}
