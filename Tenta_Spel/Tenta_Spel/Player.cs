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
        GameObject ship;

        public Player(GameObjectController goc)
        {
            ship = new GameObject(goc, "Ship0", new Vector2(0, 0));
            //goc.player = ship;
        }
    }
}
