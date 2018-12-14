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
    class Inventory
    {
        int slots = 10;
        int money = 0;
        public Dictionary<string, Item> content = new Dictionary<string, Item>();
        public Item[] contentList;

        public Item AddItem(Item itemAdd)
        {
            if (itemAdd != null)
            {
                if (content.Count <= slots)
                {
                    if (content.ContainsKey(itemAdd.name))
                    {
                        content[itemAdd.name].quantity += itemAdd.quantity;
                    }
                    else
                    {
                        if (itemAdd.quantity > 0)
                        {
                            content.Add(itemAdd.name, itemAdd);
                        }
                    }
                    ContentToList();
                    return ItemZero();
                }
                else
                {
                    Console.WriteLine("Inventory full");
                    ContentToList();
                    return itemAdd;
                }
            }

            ContentToList();
            return ItemZero();
        }


        //*
        public Item RemoveItem(Item itemRem)
        {
            if (content.ContainsKey(itemRem.name))
            {
                if (content[itemRem.name].quantity - itemRem.quantity > 0)
                {
                    content[itemRem.name].quantity -= itemRem.quantity;

                    ContentToList();
                    return ItemZero();
                }
                else
                {
                    itemRem.quantity -= content[itemRem.name].quantity;
                    content.Remove(itemRem.name);
                    ContentToList();
                    return itemRem;
                }
            }

            ContentToList();
            return itemRem;
        }
        //*/

        public int MoneyTransfer(int amount)
        {
            if (money + amount >= 0)
            {
                money += amount;
            }
            else
            {
                money = 0;
                return money + amount;
            }

            return 0;
        }

        void ContentToList()
        {
            contentList = new Item[content.Count];
            int i = 0;
            foreach (KeyValuePair<string, Item> de in content)
            {
                contentList[i] = de.Value;
                i++;
            }
        }

        Item ItemZero()
        {
            return new Item("0", 0);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, SpriteFont gamefont)
        {
            int i = 0;
            foreach (KeyValuePair<string, Item> itm in content)
            {
                spriteBatch.DrawString(gamefont, itm.Key + " : " + itm.Value.quantity, new Vector2(graphics.PreferredBackBufferWidth / 4, 100 + 18 * i), Color.Yellow);
                i++;
            }
        }
    }

    class Item
    {

        public string name;
        public int quantity;

        public Item(string nameSet, int quantitySet = 1)
        {
            name = nameSet;
            quantity = quantitySet;
        }
    }

    class Mineral : Item
    {
        string[] minerals = new string[5] { "Coal", "Iron", "Aluminium", "Uranium", "Stone" };

        public Mineral(string nameSet, int quantitySet = 1) : base(nameSet, quantitySet)
        {
            name = minerals[new Random().Next(5)];
            quantity = new Random().Next(1, 3);
        }
    }
}
