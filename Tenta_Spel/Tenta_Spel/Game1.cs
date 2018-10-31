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

        GameObjectController goc = new GameObjectController();

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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            string imgPath = @"..\..\..\..\Tenta_SpelContent\Sprites\";//"..\..\..\Resources\Sprites";

            for (int i = 0; i < System.IO.Directory.GetFiles(imgPath).Length; i++)
            {
                string tempPath = System.IO.Directory.GetFiles(imgPath)[i];
                Texture2D tempTexture = Texture2D.FromStream(GraphicsDevice, new System.IO.FileStream(tempPath, System.IO.FileMode.Open));
                goc.textureList.Add(System.IO.Directory.GetFiles(imgPath)[i].Substring(tempPath.LastIndexOf('\\') +1).Split('.')[0], tempTexture);
            }
            
            goc.AddGameObject(new GameObject("Ship1", new Vector2(100, 100)));
            foreach (GameObject go in goc.gos)
            {
                go.SetSprite(goc.textureList[go.spritePath]);
            }
            //goc.player.SetSprite(Content.Load<Texture2D>(goc.player.spritePath));
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
                
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                goc.player.ForceAdd(goc.player.Forward(), 1f);
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                goc.player.rotation += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                goc.player.rotation -= 0.1f;
            }

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                goc.player.Shoot(goc, 10, 1);
            }


            foreach (GameObject go in goc.gos)
            {
                go.ForceMove();
            }
            
            goc.cameraPosSet(new Vector2(Window.ClientBounds.X, Window.ClientBounds.Y), goc.player.pos + goc.player.Raduis());

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(goc.textureList["BG1"], new Rectangle(0,0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.DarkGray);
            foreach (GameObject go in goc.gos)
            {
                if (go != null /*goc.player*/)
                {
                    spriteBatch.Draw(go.sprite, go.pos - goc.player.pos + new Vector2(graphics.PreferredBackBufferWidth/2,graphics.PreferredBackBufferHeight/2), null, Color.White, go.rotation, go.Raduis(), 1f, SpriteEffects.None, 0);
                }
            }
            //spriteBatch.Draw(goc.player.sprite, new Vector2(Window.ClientBounds.X/0.75f + goc.player.Raduis().X, Window.ClientBounds.Y/0.75f + +goc.player.Raduis().X), null, Color.White, goc.player.rotation, goc.player.Raduis(), 1f, SpriteEffects.None, 0);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }

    class GameObjectController
    {
        public List<GameObject> gos = new List<GameObject>();
        public Ship player;

        public Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();

        public Vector2 cameraPos = new Vector2(25, 25);
        
        public GameObjectController()
        {
            player = new Ship("Ship0", new Vector2(100, 100));
            gos.Add(player);
        }

        public void AddGameObject(GameObject obj)
        {
            obj.SetSprite(textureList[obj.spritePath]);
            gos.Add(obj);
        }

        public void cameraPosSet(Vector2 window, Vector2 posSet)
        {
            //cameraPos = new Vector2( posSet.X - (window.X /2) , posSet.Y - (window.Y /2));
            cameraPos = window/2 - (player.pos + player.Raduis());
        }
    }

    class BackgroundManager
    {
        Texture2D tex;

        float move = 0.1f;

        public BackgroundManager()
        {

        }
    }

    class GameObject
    {
        public string spritePath;
        public Texture2D sprite;

        float speed = 0.1f;
        public Vector2 pos;
        public float rotation;

        int hitboxRadius;

        public Vector2 velocity;
        float fullThrust = 0.3f;
        float friction = 0.99f;
        bool isGravity = false;

        public GameObject(string spritePathSet, Vector2 startPos)
        {
            spritePath = spritePathSet;
            pos = startPos;

            //hitbox = new BoundingSphere(pos + Origin(), Origin().Y);
        }

        public void SetSprite(Texture2D spriteSet)
        {
            sprite = spriteSet;
        }

        public bool Collision(GameObject other)
        {
            if (pos.X - other.pos.X + pos.Y - other.pos.Y < hitboxRadius + other.hitboxRadius)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public Vector2 Raduis()
        {
            return new Vector2(sprite.Width/2, sprite.Height/2);
        }

        public Vector2 Forward()
        {
            return new Vector2((float)-Math.Sin(rotation), (float)Math.Cos(rotation));
        }

        public void Move(Vector2 moveTo)
        {
            pos = moveTo;
        }

        public void ForceMove()
        {
            velocity *= friction;
            pos += velocity;
        }

        public void ForceAdd(Vector2 dir, float acc)
        {
            velocity += dir * acc * fullThrust;
        }
    }

    class Ship : GameObject
    {
        public Ship(string spritePathSet, Vector2 startPos) : base(spritePathSet, startPos)
        {
            spritePath = spritePathSet;
            pos = startPos;
        }

        int hp = 100;
        Vector2 canonPos;

        public void Shoot(GameObjectController goc, float speed, int dmg)
        {
            canonPos = pos + Forward() * Raduis();
            goc.AddGameObject(new Bullet("Bullet", canonPos, Forward() * speed, dmg));
        }

        public void TakeDMG(int amount)
        {
            hp -= amount;
        }

        public void Beam()
        {

        }
    }

    class Bullet : GameObject
    {
        int dmg;
        public Bullet(string spritePathSet, Vector2 startPos, Vector2 dir, int dmgSet) : base(spritePathSet, startPos)
        {
            spritePath = "Bullet";
            pos = startPos;
            velocity = dir;
            dmg = dmgSet;
        }

        public void ForceMove()
        {
            pos += velocity;
        }
    }
}
