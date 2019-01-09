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
    //klassen för att hantera probramfönstret
    class WindowManager
    {
        GraphicsDeviceManager graphics;
        float windowScale = 0.50f;

        public WindowManager(GraphicsDeviceManager graphicsSet)
        {
            graphics = graphicsSet;
        }

        //om programfönstret är 100% av skärmen blir det fullskärm
        public void WindowScale(GraphicsDevice graphicsDevice)
        {
            graphics.PreferredBackBufferWidth = Convert.ToInt32(graphicsDevice.DisplayMode.Width * windowScale);
            graphics.PreferredBackBufferHeight = Convert.ToInt32(graphicsDevice.DisplayMode.Height * windowScale);
            if (windowScale >= 1)
            {
                windowScale = 1f;
                graphics.IsFullScreen = true;
            } else
            {
                graphics.IsFullScreen = false;
            }
            graphics.ApplyChanges();
        }

        //skalar programfönstret relativt
        public void WindowScaleScale(float amount)
        {
            windowScale = Math.Min(1, Math.Max(0.3f, windowScale + amount));

        }

        //sätter programfönstrets storlek
        public void WindowScaleSet(float amount)
        {
            windowScale = Math.Min(1, Math.Max(0.3f, amount));
        }
    }
}
