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
    //klassen för att hantera ui element
    class UIManager
    {
        public List<UIContainer> uiCs = new List<UIContainer>();
        GameObjectController goc;
        public Texture2D block;

        public UIManager(GameObjectController gocSet)
        {
            goc = gocSet;
            block = goc.GetTexture("");
        }
    }

    //klassen för att hantera underårdnade uielement
    class UIContainer
    {
        public UIManager uim;

        public UIElement rendObj;
        public int borderSize = 2;

        public List<UIElement> uiElements = new List<UIElement>();

        public UIContainer(UIManager uimSet, Vector2 posSet, Vector2 sizeSet, Color backgroundColor, int borderSizeSet)
        {
            uim = uimSet;
            uim.uiCs.Add(this);

            rendObj = new UIElement(this, posSet, sizeSet, backgroundColor);
            borderSize = borderSizeSet;

            
        }

        //retunerar bakgrundsuiobjektets rect (position och dimensioner)
        public Rectangle dims()
        {
            return new Rectangle(Convert.ToInt32(rendObj.pos.X), Convert.ToInt32(rendObj.pos.Y), Convert.ToInt32(rendObj.size.X), Convert.ToInt32(rendObj.size.Y));
        }

        //funktion för att rita underordnade uiobjekt
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            rendObj.Draw(spriteBatch, graphics);
            foreach (UIElement uie in uiElements)
            {
                uie.Draw(spriteBatch, graphics);
            }
        }
    }

    //grundklassen för uiobjekt
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
            uic.uiElements.Add(this);

            texture = uic.uim.block;
        }

        //retunerar rect
        Rectangle Rect()
        {
            return new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), Convert.ToInt32(size.X), Convert.ToInt32(size.Y));
        }

        //funktion för att rita uiobjekt
        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.Draw(texture, Rect(), clr);
        }
    }

    //klass för att rita enkla uiobjekt (samma som UIElement klassen)
    class UIBlock : UIElement
    {
        public UIBlock(UIContainer uic, Vector2 posSet, Vector2 sizeSet, Color colorSet) : base(uic, posSet, sizeSet, colorSet)
        {

        }
    }

    //klassen för ui text
    class UIText : UIElement
    {
        SpriteFont font;
        public string text;

        public UIText(UIContainer uic, Vector2 posSet, Vector2 sizeSet, Color colorSet, SpriteFont spritefont, string textSet) : base(uic, posSet, sizeSet, colorSet)
        {
            text = textSet;
            font = spritefont;
        }

        //funktion för att rita objektet
        public override void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            spriteBatch.DrawString(font, text, pos, clr);
        }
    }

    //klassen för ui knappar
    class UIButton : UIElement
    {
        Color baseColor;
        Color pressedColor;
        Color hoverColor;

        public UIButton(UIContainer uic, Vector2 posSet, Vector2 sizeSet, Color colorSet) : base(uic, posSet, sizeSet, colorSet)
        {
            baseColor = colorSet;
            hoverColor = new Color(Convert.ToInt32(colorSet.R * 0.66), Convert.ToInt32(colorSet.R * 0.66), Convert.ToInt32(colorSet.R * 0.66));
            pressedColor = new Color(Convert.ToInt32(colorSet.R * 0.33), Convert.ToInt32(colorSet.R * 0.33), Convert.ToInt32(colorSet.R * 0.33));

        }

        //funktion som retunerar om knappen blivit tryckt
        public bool ButtonPressed()
        {
            MouseState ms = Mouse.GetState();

            clr = baseColor;
            if (ms.X > pos.X && ms.Y > pos.Y && ms.X < pos.X + size.X && ms.Y < pos.Y + size.Y)
            {
                clr = hoverColor;
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    clr = pressedColor;
                    return true;
                }
            }

            return false;
        }
    }
}
