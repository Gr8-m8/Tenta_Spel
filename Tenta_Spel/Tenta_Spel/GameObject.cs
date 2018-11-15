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
    class GameObject
    {
        public GameObjectController goc;

        public Texture2D sprite;
        public string spriteKey;

        float speed = 0.1f;
        public Vector2 pos;
        public float rotation;

        int hitboxRadius;

        public Vector2 velocity;
        float fullThrust = 0.6f;
        float friction = 0.99f;
        bool isGravity = false;

        public bool markForDelete = false;

        public GameObject(GameObjectController gocSet, string spriteKeySet, Vector2 startPos)
        {
            goc = gocSet;

            sprite = goc.GetTexture(spriteKeySet);
            pos = startPos;

            Create();
        }

        public bool Collision(GameObject other)
        {
            if (pos.X - other.pos.X + pos.Y - other.pos.Y < hitboxRadius + other.hitboxRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Vector2 Raduis()
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

        public virtual void Create()
        {
            goc.gos.Add(this);
        }

        public virtual void Delete()
        {
            if (goc.gos.Contains(this))
            {
                goc.gos.Remove(this);
            }
        }
    }

    class Ship : GameObject
    {
        public Ship(GameObjectController gocSet, string spriteKeySet, Vector2 startPos) : base(gocSet, spriteKeySet, startPos)
        {

        }


        public int shootCooldown = 20;
        public int tickShootCooldown = 0;

        public void Shoot(GameObjectController goc, float speed, int dmg)
        {
            if (tickShootCooldown >= shootCooldown)
            {
                tickShootCooldown = 0;
                Vector2 canonPos = pos + Forward() * Raduis();
                new Bullet(goc, "Bullet", canonPos, velocity + Forward() * speed, dmg, 200);
            }
        }

        int hp = 100;
        public void HeathManager(int amount)
        {
            hp -= amount;

            if (hp < 1)
            {
                Console.WriteLine(this + " died!");
            }
        }

        public void Beam()
        {

        }

        public override void Create()
        {
            goc.ships.Add(this);
        }

        public override void Delete()
        {
            if (goc.ships.Contains(this))
            {
                goc.ships.Remove(this);
            }
        }
    }

    class Bullet : GameObject
    {
        int tickAge = 0;
        int lifeSpan;

        int dmg;
        public Bullet(GameObjectController gocSet, string spriteKeySet, Vector2 startPos, Vector2 dir, int dmgSet, int lifeSpanSet) : base(gocSet, spriteKeySet, startPos)
        {
            //sprite = goc.GetTexture("Bullet");
            velocity = dir;
            dmg = dmgSet;
            lifeSpan = lifeSpanSet;

            Create();
        }

        public override void ForceMove()
        {
            pos += velocity;
            tickAge += 1;

            if (tickAge > lifeSpan)
            {
                markForDelete = true;
            }
        }

        public override void Create()
        {
            goc.bullets.Add(this);
        }

        public override void Delete()
        {
            if (goc.bullets.Contains(this))
            {
                goc.bullets.Remove(this);
            }
        }
    }

    class Planet : GameObject
    {
        public Color planetColor;
        public float scale;

        public Planet(GameObjectController gocSet, string spriteKeySet, Vector2 startPos, Color clrSet) : base(gocSet, spriteKeySet, startPos)
        {
            Random r = new Random();
            planetColor = new Color(r.Next(255), r.Next(255), r.Next(255));
            scale = 1 + float.Parse((r.NextDouble() * 5).ToString());

            Console.WriteLine(planetColor + " | " + scale);
        }

        public override void ForceMove()
        {
            rotation += 0.0002f;
        }


        public override void Create()
        {
            goc.planets.Add(this);
        }

        public override void Delete()
        {
            if (goc.planets.Contains(this))
            {
                goc.planets.Remove(this);
            }
        }
    }
}
