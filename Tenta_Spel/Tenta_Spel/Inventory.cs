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
    //klassen för föråd
    class Inventory
    {
        int slots = 10;
        int money = 0;
        public Dictionary<string, Item> content = new Dictionary<string, Item>();
        public Item[] contentList;

        //lägga till saker till föråd
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

        //tar bort saker från föråd
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

        //pengarhantering
        /* pengar används inte (ännu ;) )
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
        //*/

        //skapar en lista av allt som finns i förådet
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

        //retunerar debug- och errorföremål
        Item ItemZero()
        {
            return new Item("0", 0);
        }

        //funktion för att rita förrådet och hur man skapar föremål
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, SpriteFont gamefont)
        {
            int i = 0;
            spriteBatch.DrawString(gamefont, "[F] +Fuel (Uranium {1}, Aluminium {2})", new Vector2(0, 2 * 18 + 18 * i), Color.Beige);
            i++;

            spriteBatch.DrawString(gamefont, "[R] +Ammunition (Crystal {2}, Iron {1})", new Vector2(0, 2 * 18 + 18 * i), Color.Beige);
            i++;

            spriteBatch.DrawString(gamefont, "[J] +Health (Iron {2}, Titanium {1})", new Vector2(0, 2 * 18 + 18 * i), Color.Beige);
            i++;

            spriteBatch.DrawString(gamefont, "[L] +Mineral (Mineral Stone {3})", new Vector2(0, 2 * 18 + 18 * i), Color.Beige);
            i++;

            //inv
            i++;
            foreach (KeyValuePair<string, Item> itm in content)
            {
                spriteBatch.DrawString(gamefont, itm.Key + " : " + itm.Value.quantity, new Vector2(0, 2 * 18 + 18 * i), Color.Yellow);
                i++;
            }
        }
    }

    //grundklassen för förådsaker
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

    //klassen för mineraler
    class Mineral : Item
    {
        static Random r = new Random();

        static string[] minerals = new string[6] { "Crystal", "Iron", "Uranium", "Aluminium", "Mineral Stone", "Titanium" };

        public Mineral(string nameSet, int quantitySet = 1) : base(nameSet, quantitySet)
        {
            quantity = r.Next(1, 4);
            name = minerals[r.Next(minerals.Length)];
        }
    }
}
