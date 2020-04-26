using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    class Item
    {
        public String Name { private set; get; }
        public int BuyPrice { private set; get; }
        public int SellPrice { private set; get; }

        public Item(int b=0, int s=0, String n="that's not supposed to happen")
        {
            BuyPrice = b;
            SellPrice = s;
            Name = n;
        }

        //level as in level of the game not of the player
        static public Item Generate(int level, Tile.TileType tileType=Tile.TileType.ROOM) 
        {
            int rollType = Game.rng.Next(3);
            int rollName = Game.rng.Next(5);

            int s_price = (Game.rng.Next(level * 3) + 1) * 2 + Game.rng.Next(1);
          
            if (tileType == Tile.TileType.SPECIAL)
                s_price += 5;

            int b_price = s_price * 5;

            switch (rollType)
            {
                case 0:
                    int damage = ((s_price*3)/4)-Game.rng.Next(4);
                    if (damage > 12)
                        damage = 12;
                    if (damage < 1)
                        damage = 1;
                    switch (rollName)
                    {
                        case 0:
                            return new Sword(damage, b_price, s_price);
                        case 1:
                            return new Sword(damage, b_price, s_price, "Longsword");
                        case 2:
                            return new Sword(damage, b_price, s_price, "Dagger");
                        case 3:
                            return new Sword(damage, b_price, s_price, "Saber");
                        case 4:
                            return new Sword(damage, b_price, s_price, "Broadsword");
                    }
                    break;
                case 1:
                    int block = (s_price/2) - Game.rng.Next(4);
                    if (block > 8)
                        block = 8;
                    if (block < 1)
                        block = 1;
                    switch (rollName)
                    {
                        case 0:
                        case 1:
                        case 2:
                            return new Shield(block, b_price, s_price, "Round shield");
                        case 3:
                            return new Shield(block, b_price, s_price, "Square shield");
                        case 4:
                            return new Shield(block, b_price, s_price, "Fancy shield");
                    }
                    break;
                case 2:
                    int power = ((s_price * 3) / 8) - Game.rng.Next(2);
                    if (power > 6)
                        power = 6;
                    if (power < 1)
                        power = 1;
                    switch (rollName)
                    {
                        case 0:
                            return new Wand(power, b_price, s_price, "Straight wand");
                        case 1:
                            return new Wand(power, b_price, s_price, "Crooked wand");
                        case 2:
                            return new Wand(power, b_price, s_price, "Shiny wand");
                        case 3:
                            return new Wand(power, b_price, s_price, "Staff wand");
                        case 4:
                            return new Wand(power, b_price, s_price, "Small wand");
                    }
                    break;
                default:
                    throw new SystemException("item generator");
            }
            return new Item(); //should never execute; added it here because visual studio is hissy
        }
    }

    class Sword : Item
    {
        public int Dmg { private set; get; }

        public Sword(int d=0, int b=0, int s=0, String n="Sword"): base(b,s,n)
        {
            Dmg = d;
        }
    }
    class Wand : Item
    {
        public int Power { private set; get; }
        
        public Wand(int p=0, int b=0, int s=0, String n="Wand"): base(b,s,n)
        {
            Power = p;
        }
    }
    class Shield : Item
    {
        public int Block { private set; get; }

        public Shield(int b=0, int bb=0, int s=0, String n="Shield"): base(bb,s,n)
        {
            Block = b;
        }
    }


}
