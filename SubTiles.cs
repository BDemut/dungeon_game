using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    // I put both subclasses of tile in a seperate file
    // for more info on Enter go to Tile.cs
    class StoreTile : Tile
    {
        public List<Item> Wares { private set; get; }
        public StoreTile(List<Item> i) :
            base(TileType.STORE, null, 0, null)
        {
            Wares = i;
        }

        public override int Enter(ref Player player, Coordinates prev = null)
        {
            while (true)
            {
                Console.Clear();
                player.PrintInfo();
                Console.WriteLine("You have found a store!\nAvailable wares:");
                int i = 1;
                foreach (var item in Wares)
                {
                    if (item != null)
                    {
                        Console.WriteLine("{0}\t - {1} gold, press {2} to buy", item.Name, item.BuyPrice, i);
                        ++i;
                    }
                }
                Console.WriteLine("");
                if (player.RightHand != null)
                    Console.WriteLine("Press R to sell your {1} for {2} gold.", i, player.RightHand.Name, player.RightHand.SellPrice);
                if (player.LeftHand != null)
                    Console.WriteLine("Press L to sell your {1} for {2} gold.", i, player.LeftHand.Name, player.LeftHand.SellPrice);

                Console.WriteLine("");
                Console.WriteLine("Press H to buy food for 5 gold and heal for 2 hp.");
                Console.WriteLine("Press E to exit.");


                char a = Console.ReadKey().KeyChar;
                Console.Clear();
                player.PrintInfo();

                Item sold = null;

                if (a > '0' && a < '9')
                {
                    if (player.RightHand != null && player.LeftHand != null)
                    {
                        Console.WriteLine("You have to sell one of your items first!\nPress 1 to sell the {0} for {1} gold\n" +
                            "Press 2 to sell the {2} for {3} gold ", player.RightHand.Name, player.RightHand.SellPrice,
                            player.LeftHand.Name, player.LeftHand.SellPrice);
                        char pick = Console.ReadKey().KeyChar;
                        if (pick == '1')
                        {
                            if (player.Gold + player.RightHand.SellPrice - Wares[a - '0' - 1].BuyPrice >= 0)
                            {
                                player.Gold = player.Gold + player.RightHand.SellPrice - Wares[a - '0' - 1].BuyPrice;
                                sold = player.PickUpItem(Wares[a - '0' - 1], true);
                            }
                            else
                            {
                                Console.WriteLine("You can't afford to do that!");
                                Console.WriteLine("Press a key to continue.");
                                Console.ReadKey();
                                continue;
                            }
                        }
                        else if (pick == '2')
                        {
                            if (player.Gold + player.LeftHand.SellPrice - Wares[a - '0' - 1].BuyPrice >= 0)
                            {
                                player.Gold = player.Gold + player.LeftHand.SellPrice - Wares[a - '0' - 1].BuyPrice;
                                sold = player.PickUpItem(Wares[a - '0' - 1], false);
                            }
                            else
                            {
                                Console.WriteLine("You can't afford to do that!");
                                Console.WriteLine("Press a key to continue.");
                                Console.ReadKey();
                                continue;
                            }
                        }
                        else
                            continue;
                    }
                    else
                    {
                        if (player.Gold - Wares[a - '0' - 1].BuyPrice >= 0)
                            if (player.RightHand == null)
                            {
                                player.Gold = player.Gold - Wares[a - '0' - 1].BuyPrice;
                                player.PickUpItem(Wares[a - '0' - 1], true);
                            }
                            else
                            {
                                player.Gold = player.Gold - Wares[a - '0' - 1].BuyPrice;
                                player.PickUpItem(Wares[a - '0' - 1], false);
                            }
                        else
                        {
                            Console.WriteLine("You can't afford to do that!");
                            Console.WriteLine("Press a key to continue.");
                            Console.ReadKey();
                            continue;
                        }
                    }
                    Wares.RemoveAt(a - '0' - 1);
                    if (sold!=null)
                        Wares.Add(sold);
                }
                if (a == 'r' && player.RightHand != null)
                {
                    player.Gold += player.RightHand.SellPrice;
                    Wares.Add(player.PickUpItem(null, true));
                }
                if (a == 'l' && player.LeftHand != null)
                {
                    player.Gold += player.LeftHand.SellPrice;
                    Wares.Add(player.PickUpItem(null, false));
                }
                if (a == 'h')
                    if (player.Gold >= 5)
                    {
                        player.Gold -= 5;
                        player.Damage(-2);
                    }
                    else
                    {
                        Console.WriteLine("You can't afford to do that!");
                        Console.WriteLine("Press a key to continue.");
                        Console.ReadKey();
                        continue;
                    }
                if (a == 'e')
                {
                    Console.Clear();
                    player.PrintInfo();
                    return 2;
                }
            }
        }

        static public StoreTile GenerateStore(int level)
        {
            int wareNum = 3 + Game.rng.Next(4);
            List<Item> wares = new List<Item> { };
            for (int i = 0; i < wareNum; ++i)
                wares.Add(Item.Generate(level));
            return new StoreTile(wares);
        }

    }

    class SpecialTile : Tile
    {
        public SpecType SType { private set; get; }
        bool _completed = false;

        public enum SpecType
        {
            HEAL,
            TRAP,
            TEACH,
        }

        public SpecialTile(SpecType t) :
            base(TileType.SPECIAL, null, 0, null)
        {
            SType = t;
        }
        public static SpecialTile GenerateSpecial()
        {
            int rollType = Game.rng.Next(4);
            switch (rollType)
            {
                case 0:
                case 1:
                    return new SpecialTile(SpecType.HEAL);
                case 2:
                    return new SpecialTile(SpecType.TRAP);
                case 3:
                    return new SpecialTile(SpecType.TEACH);
                default:
                    throw new SystemException("generate special not good");
            }
        }

        public override int Enter(ref Player player, Coordinates prev = null)
        {
            Console.Clear();
            player.PrintInfo();
            if (_completed)
                return 2;
            switch (SType)
            {
                case SpecType.HEAL:
                    player.Damage(-(player.HealthCap - player.Hp));
                    Console.Clear();
                    player.PrintInfo();

                    Console.WriteLine("The only thing in this room is a cup filled with water.\nYou decide to drink it because why not.\n");
                    Console.WriteLine("You have been fully healed!");
                    break;
                case SpecType.TRAP:
                    player.Gold = 0;
                    Console.Clear();
                    player.PrintInfo();

                    Console.WriteLine("You have been hit in the head and fell unconcious.\n\nWhen you woke up your gold was gone!");
                    break;
                case SpecType.TEACH:
                    Console.WriteLine("You see an old man chained to the wall.\nYou set him free and in return he offers to train you.\n\n");
                    Console.WriteLine("You gain {0} exp!\nPress any key", player.ExpCap);       //you always gain enough xp needed for a next level or more
                    Console.ReadKey();
                    player.GainExp(player.ExpCap);
                    break;
            }
            _completed = true;
            return 2;
        }
    }
}
