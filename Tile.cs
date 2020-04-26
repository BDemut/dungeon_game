using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    //tile is a one single "place" for a player to be in - it's represented by a coordinate
    class Tile
    {
        public enum TileType
        {
            GAMESTART,
            LEVELSTART,
            LEVELEND,
            BOSS,
            WALL,
            ROOM,
            TREASURE,
            SPECIAL,
            STORE
        }

        public TileType Type { private set; get; }
        public List<Enemy> Enemies { private set; get; }
        public int Gold { private set; get; }
        public Item Loot { private set; get; }

        public Tile(TileType t, List<Enemy> e, int g, Item l)
        {
            Enemies = e;
            Gold = g;
            Loot = l;
            Type = t;
        }

        // called by a level when the player enters the tile
        // returns a status code so the level knows whats going on
        // 0 if dead, 1 if escaped, 2 if clear, 3 if clear and used room (for example used a staircase)
        public virtual int Enter(ref Player player, Coordinates prev=null)
        {
            Console.Clear();
            switch (Type)
            {
                case TileType.ROOM:
                    break;
                case TileType.GAMESTART:
                    break;
                case TileType.LEVELEND:
                    Console.WriteLine("You find a staircase leading down.");
                    Console.WriteLine("Press D to descend.\nPress any other key to stay upstairs.");
                    if (Console.ReadKey().KeyChar == 'd')
                        return 3;
                    break;
                case TileType.LEVELSTART:
                    Console.WriteLine("The room you find yourself in is even darker and damper than the ones upstairs.");
                    Console.WriteLine("Press U to go upstairs.\nPress any other key to continue.");
                    if (Console.ReadKey().KeyChar == 'u')
                        return 3;
                    break;
                case TileType.BOSS:
                    // great writing 101
                    Console.WriteLine("You peek inside the next door and see a beautiful, decorated room.");
                    Console.WriteLine("It's nothing like the dark corridors you've been roaming earlier.");
                    Console.WriteLine("It feels like it's inviting you in.");
                    Console.WriteLine("You take a step forward and freeze.");
                    Console.WriteLine("In the very center of the room lies a huge dragon.");
                    Console.WriteLine("You have heard legends of a dragon guarding the city dungeons,\nbut you have never believed them.");
                    Console.WriteLine("It seems to be sleeping, so you might have a chance to step back.");
                    Console.WriteLine("Escape? Press Y if yes");
                    if (Console.ReadKey().KeyChar == 'y')
                        return 1;
                    break;
            }

            Console.Clear();
            player.PrintInfo();

            if (Enemies != null && Enemies.Count > 0) 
            {
                int end = Enemies.Count;
				Console.WriteLine("You've been attacked!");
                for (int i = 0; i < end; ++i) 
                {
					bool escaped = player.Fight(Enemies[0]);
					if (escaped)
					{
						return 1;
					}
					else if (player.Hp < 1)
						return 0;
					else
						Enemies.RemoveAt(0);
                }
				Console.WriteLine("There seem to be no more enemies.\n");
			}
			if (Gold!=0)
			{
                if (Type == TileType.TREASURE)
                    Console.WriteLine("You have found a treasure! It contains {0} gold!", Gold);
                else
				    Console.WriteLine("You found {0} gold!", Gold);
				player.Gold += Gold;
				Gold = 0;
			}
			if (Loot!=null)
			{
				Console.WriteLine("You found a {0}!", Loot.Name);
				if (player.RightHand==null)
				{
					Console.WriteLine("Pick it up? Press Y if yes.");
					char a = Console.ReadKey().KeyChar;
					if (a == 'Y' || a == 'y')
						Loot = player.PickUpItem(Loot, true);
					Console.Clear();
					player.PrintInfo();
				}
				else if (player.LeftHand==null)
				{
					Console.WriteLine("Pick it up? Press Y if yes.");
					char a = Console.ReadKey().KeyChar;
					if (a == 'Y' || a == 'y')
						Loot = player.PickUpItem(Loot, false);

					Console.Clear();
					player.PrintInfo();
				}
				else
				{
					Console.WriteLine("Swap it with your {0} or {1}?\nPress 1 to swap with {0}\nPress 2 to swap with {1}",
						player.RightHand.Name, player.LeftHand.Name);
					char a = Console.ReadKey().KeyChar;
					if (a=='1')
						Loot = player.PickUpItem(Loot, true);
					else if (a=='2')
						Loot = player.PickUpItem(Loot, false);
					Console.Clear();
					player.PrintInfo();
				}
			}
			return 2;
        }

        // generating a tile:
        // LevelGenerator -> TileGenerator -> GenerateRoom/GenerateStore... etc.
        static public Tile TileGenerator(int level, TileType type)
        {
            int rollEnemies = Game.rng.Next(10);
            int rollGold = Game.rng.Next(10);
            int rollLoot = Game.rng.Next(10);

            switch (type)
            {
                case TileType.WALL:
                case TileType.GAMESTART:
                case TileType.LEVELSTART:
                case TileType.LEVELEND:
                    return new Tile(type,null,0,null);
                case TileType.BOSS:
                    return new Tile(type, new List<Enemy> { Enemy.GetBoss() }, 0, null);
                case TileType.STORE:
                    return StoreTile.GenerateStore(level);
                case TileType.ROOM:
                    return GenerateRoom(rollEnemies, rollGold, rollLoot, level);
                case TileType.TREASURE:
                    return GenerateTreasure(rollGold, rollLoot, level);
                case TileType.SPECIAL:
                    return SpecialTile.GenerateSpecial();
                default:
                    throw new SystemException("tile generator");
            }

        }

        static private Tile GenerateRoom(int rollEnemies, int rollGold, int rollLoot, int level)
        {
            int wEnemyNum = 0;
            int mEnemyNum = 0;
            int sEnemyNum = 0;
            int gold = 0;
            Item loot = (rollLoot==9 ? Item.Generate(level) : null);

            //the higher the level the stronger the enemies
            switch (level)
            {
                // comments show the number and type of enemies, for example:
                // 7-8 1med
                // means that if rollEnemies (which is a 0-9 integer passed from TileGenerator)
                // is 7 or 8 the level will have 0 weak enemies, 1 medium enemy and 0 strong enemies
                case 1:
                    wEnemyNum = (rollEnemies < 4 ? 0 :        // 0-3 empty ; 4-7 1weak; 8 2weak; 9 3weak;
                        (rollEnemies < 8 ? 1 : (rollEnemies == 8 ? 2 : 3)));

                    gold = (rollGold+1)*2/ 3 - 1;
                    if (gold < 0)
                        gold = 0;

                    break;
                case 2:
                    wEnemyNum = (rollEnemies < 3 || rollEnemies > 6 ? 0 :
                       (rollEnemies == 6 ? 2 : 1));                      // 0-2 empty; 3-4 1weak; 5 1med and 1weak; 6 2weak; 7-8 1med; 9 2med; 
                    mEnemyNum = (rollEnemies == 9 ? 2 :
                       (rollEnemies < 5 || rollEnemies == 6 ? 0 : 1));

                    gold = (rollGold + 1) * 3 / 4 - 1;
                    if (gold < 0)
                        gold = 0;

                    break;
                case 3:
                    wEnemyNum = (rollEnemies == 2 || rollEnemies == 5 ? 1 :
                        (rollEnemies == 4 ? 3 : 0));                              // 0-1 empty; 2 1weak; 3 1med; 4 3weak; 5 1weak 1med; 6 2med; 7 1strong; 8 1med 1 strong; 9 2strong;
                    mEnemyNum = (rollEnemies == 3 || rollEnemies == 5 || rollEnemies == 8 ? 1 :
                        (rollEnemies == 6 ? 2 : 0));
                    sEnemyNum = (rollEnemies < 7 ? 0 : (rollEnemies == 9 ? 2 : 1));

                    gold = (rollGold + 1) * 4 / 5 - 1;
                    if (gold < 0)
                        gold = 0;

                    break;
                default:
                    throw new SystemException("room generator");
            }

            List<Enemy> enemies = new List<Enemy> { };
            for (int i = 0; i < wEnemyNum; ++i)
                enemies.Add(Enemy.GenerateEnemy(Enemy.EnemyStrength.WEAK));
            for (int i = 0; i < mEnemyNum; ++i)
                enemies.Add(Enemy.GenerateEnemy(Enemy.EnemyStrength.MEDIUM));
            for (int i = 0; i < sEnemyNum; ++i)
                enemies.Add(Enemy.GenerateEnemy(Enemy.EnemyStrength.STRONG));

            return new Tile(TileType.ROOM,enemies,gold,loot);
        }

        static private Tile GenerateTreasure(int rollGold, int rollLoot,int level)
        {
            int gold = (level == 1 ? 5+rollGold : (level==2 ? 7 + (rollGold*5)/4 : 10 + (rollGold*3)/2));
            Item loot = (rollLoot > 5 ? Item.Generate(level) : null);
            return new Tile(TileType.TREASURE, null, gold, loot);
        }
        
    }
}
