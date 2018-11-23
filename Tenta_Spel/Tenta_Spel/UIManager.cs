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
    class UIManager
    {
        public List<UIContainer> uiCs = new List<UIContainer>();

        public UIManager()
        {

        }
    }

    class UIContainer
    {
        UIManager uim;

        UIElement rendObj;
        public int borderSize = 2;

        public List<UIElement> uiElements = new List<UIElement>();

        public UIContainer(UIManager uimSet, Vector2 posSet, Vector2 sizeSet, Color backgroundColor, int borderSizeSet)
        {
            rendObj = new UIElement(this, posSet, sizeSet, backgroundColor);
            borderSize = borderSizeSet;

            uim = uimSet;
            uim.uiCs.Add(this);
        }

        public int GridX()
        {
            return Convert.ToInt32(rendObj.size.X / 10) + borderSize;
        }

        public int GridY()
        {
            return Convert.ToInt32(rendObj.size.Y / 10) + borderSize;
        }

        public Rectangle dims()
        {
            return new Rectangle(Convert.ToInt32(rendObj.pos.X), Convert.ToInt32(rendObj.pos.Y), Convert.ToInt32(rendObj.size.X), Convert.ToInt32(rendObj.size.Y));
        }
    }

    class UIElement
    {
        public Type T;
        UIContainer uic;

        Texture2D texture;

        public Vector2 pos;
        public Vector2 size;

        public Color clr;

        public UIElement(UIContainer uicSet, Vector2 posSet, Vector2 sizeSet, Color colorSet)
        {
            pos = posSet;
            size = sizeSet;
            clr = colorSet;

            T = this.GetType();
            uic = uicSet;
        }
    }

    

    class UIBlock : UIElement
    {
        public UIBlock(UIContainer uic, Vector2 posSet, Vector2 sizeSet, Color colorSet) : base(uic, posSet, sizeSet, colorSet)
        {

        }
    }

    class UIText : UIElement
    {
        public string text;

        public UIText(UIContainer uic, Vector2 posSet, Vector2 sizeSet, Color colorSet, string textSet) : base(uic, posSet, sizeSet, colorSet)
        {
            text = textSet;
        }
    }
}
