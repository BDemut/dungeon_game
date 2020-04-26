using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    //this is the class where all the fun stuff happens
    class Level
    {
        public Dictionary<Coordinates, Tile> Tiles { get; set; }
        public Coordinates Begin { private set; get; }
        public Coordinates End { private set; get; }
        private readonly List<Coordinates> _seenCoordinates;        //tiles the player has seen - all tiles he visited + the wall tiles around them
        public Coordinates PlayerPos { get; set; }
		public int LevelNum { private set; get; }
        public Level(int l) 
        {
            Begin = new Coordinates();
            End = new Coordinates();
            Tiles = new Dictionary<Coordinates, Tile>();
			this.LevelNum = l;
            _seenCoordinates = new List<Coordinates>();
        }

        public void PrintMap()
        {
            Console.Write("\nLevel:{0} Map:",LevelNum);
            for (int i = 9; i > 0; --i)
            {
                Console.Write("\n");
                for (int j = 1; j < 10; ++j)
                {
                    var coords = new Coordinates(j, i);
                    if (_seenCoordinates.Contains(coords))
                        if (coords == PlayerPos)
                            Console.Write('x');
                        else
                            switch (Tiles[coords].Type)
                            {
                                case Tile.TileType.GAMESTART:
                                case Tile.TileType.LEVELSTART:
                                    Console.Write('S');
                                    break;
                                case Tile.TileType.ROOM:
                                case Tile.TileType.TREASURE:
                                case Tile.TileType.SPECIAL:
                                    Console.Write(' ');
                                    break;
                                case Tile.TileType.WALL:
                                    Console.Write('0');
                                    break;
                                case Tile.TileType.LEVELEND:
                                    Console.Write('E');
                                    break;
                                case Tile.TileType.STORE:
                                    Console.Write('$');
                                    break;
                                case Tile.TileType.BOSS:
                                    Console.Write('B');
                                    break;
                                default:
                                    break;
                            }
                        else
                            Console.Write('/');
                }
            }
        }
        // called by a session when the player enters a level
        // returns a status code for the session (similar to Tile.Enter)
        // 0 if player died, 4 if won game, otherwise 1-3 for the number of next level
        public int PlayLevel(ref Player player, bool startAtBegin = true) 
        {
            var prevPos = Begin;

            if (startAtBegin)
                PlayerPos = Begin;
            else
            {
                PlayerPos = End;
                prevPos = End;
            }

			int status = 0;
			while (true)
			{
                _seenCoordinates.Add(PlayerPos);
				status = Tiles[PlayerPos].Enter(ref player);

                if (status == 0)
                    return 0;
                else if (status == 1)
                {
                    PlayerPos = prevPos;
                    continue;
                }
                else if (Tiles[PlayerPos].Type == Tile.TileType.BOSS && status == 2)
                    return 4;
                else if (Tiles[PlayerPos].Type == Tile.TileType.LEVELEND && status == 3)
                    return this.LevelNum + 1;
                else if (Tiles[PlayerPos].Type == Tile.TileType.LEVELSTART && status == 3)
                    return this.LevelNum - 1;

				Coordinates[] clock = { null, null, null, null }; //clock because the directions are clockwise (north, east, south, west)
                if (PlayerPos.Y < 9)
                    clock[0] = new Coordinates(PlayerPos.X, PlayerPos.Y + 1);
                if (PlayerPos.X < 9)
                    clock[1] = new Coordinates(PlayerPos.X + 1, PlayerPos.Y);
                if (PlayerPos.Y > 1)
                    clock[2] = new Coordinates(PlayerPos.X, PlayerPos.Y - 1);
                if (PlayerPos.X > 1)
                    clock[3] = new Coordinates(PlayerPos.X - 1, PlayerPos.Y);
                
                
                bool deadEnd = true;


                foreach (var x in clock)
                    if (x != null)
                        if (Tiles[x].Type != Tile.TileType.WALL)
                            deadEnd = false;
                        else
                            _seenCoordinates.Add(x);
               
                while (true)
                {
                    if (deadEnd)
                    {
                        Console.WriteLine("It's a dead end!");
                    }
                    else
                    {
                        if (Tiles[PlayerPos].Type == Tile.TileType.GAMESTART)
                            Console.WriteLine("Which door to enter?\nPress W to go north\nPress A to go west" +
                                "\nPress S to go south\nPress D to go east");
                    }
                    String[] strClock = { "north", "east", "south", "west" };
                    char[] clockKeys = { 'w', 'd', 's', 'a' };

                    Console.WriteLine("");

                    PrintMap();
                    char a = Console.ReadKey().KeyChar;
                    if (a == 'a' || a == 'w' || a == 's' || a == 'd')
                    {
                        int go = -1;
                        for (int i = 0; i < 4; ++i)
                            if (a == clockKeys[i])
                                go = i;
                        if (clock[go] != null && Tiles[clock[go]].Type != Tile.TileType.WALL)
                        {
                            prevPos = PlayerPos;
                            PlayerPos = clock[go];
                            break;
                        }
                    }
                    Console.Clear();
                    player.PrintInfo();
                }
			}
        }
        //STATIC FUNCTIONS:
        //GenerateLevel returns a ready Level variable
        //GenerateLayout returns a dictionary (key - coordinates, value - Tile.TileType) for GenerateLevel
        //GeneratePath is a recursive part of GenerateLayout


        static public Level GenerateLevel(int levelNum)
        {
            Level level = new Level(levelNum);

            var layout = GenerateLayout(levelNum);

            for (int x = 1; x < 10; ++x)
                for (int y = 1; y < 10; ++y)
                {
                    var coords = new Coordinates(x, y);
                    if (layout.ContainsKey(coords))
                    {
                        level.Tiles.Add(coords, Tile.TileGenerator(levelNum, layout[coords]));
                        if (level.Tiles[coords].Type == Tile.TileType.LEVELSTART ||
                            level.Tiles[coords].Type == Tile.TileType.GAMESTART)
                            level.Begin = coords;
                        else if (level.Tiles[coords].Type == Tile.TileType.BOSS ||
                            level.Tiles[coords].Type == Tile.TileType.LEVELEND)
                            level.End = coords;
                    }
                }

            level._seenCoordinates.Add(level.Begin);

            return level;
        }
        
        // creates the map layout
        // GeneratePath does most of heavy lifting,
        // this function only decides where the beggining and the end of the level is and does some store adjustments
        // It also checks if layout provided by GeneratePath is valid - if not creates another until it gets it right (hence the infinite loop)
        
        static private Dictionary<Coordinates, Tile.TileType> GenerateLayout(int levelNum)
        {
            int rollStartPos = Game.rng.Next(10);
            var begin = new Coordinates();
            begin.Y = 1;
            begin.X = (rollStartPos < 4 ? 5 : (rollStartPos < 6 ? 4 :
            rollStartPos < 8 ? 6 : (rollStartPos == 8 ? 7 : 3)));

            while (true)
            {
                var tileTypes = new Dictionary<Coordinates, Tile.TileType>();
                if (levelNum == 1)
                    tileTypes.Add(begin, Tile.TileType.GAMESTART);
                else
                    tileTypes.Add(begin, Tile.TileType.LEVELSTART);

                int stores = 0;
                GeneratePath(ref tileTypes, begin, levelNum, 0, ref stores);

                if (stores==0)
                {
                    while (true)
                    {
                        var coords = new Coordinates(Game.rng.Next(9) + 1, Game.rng.Next(9) + 1);
                        if (tileTypes.ContainsKey(coords) || tileTypes[coords]!=Tile.TileType.WALL)
                        {
                            tileTypes[coords] = Tile.TileType.STORE;
                            break;
                        }
                    }
                }

                int y7 = 0, y8 = 0, y9 = 0;

                for (int x = 1; x < 10; ++x)
                {
                    var coords = new Coordinates(x, 7);
                    if (tileTypes.ContainsKey(coords))
                        if (tileTypes[coords] != Tile.TileType.WALL)
                            ++y7;
                }
                for (int x = 1; x < 10; ++x)
                {
                    var coords = new Coordinates(x, 8);
                    if (tileTypes.ContainsKey(coords))
                        if (tileTypes[coords] != Tile.TileType.WALL)
                            ++y8;
                }
                for (int x = 1; x < 10; ++x)
                {
                    var coords = new Coordinates(x, 9);
                    if (tileTypes.ContainsKey(coords))
                        if (tileTypes[coords] != Tile.TileType.WALL)
                            ++y9;
                }

                var endType = levelNum == 3 ? Tile.TileType.BOSS : Tile.TileType.LEVELEND;
                
                if (y9 > 0 || y8 > 0 || y7 > 1)
                {
                    if (y9 > 0)
                    {
                        int rollX = Game.rng.Next(y9) - 1;
                        for (int x = 1; x < 10; ++x)
                        {
                            var coords = new Coordinates(x, 9);
                            if (tileTypes.ContainsKey(coords))
                                if (tileTypes[coords] != Tile.TileType.WALL)
                                    if (rollX == 0)
                                    {
                                        tileTypes[coords] = endType;
                                        return tileTypes;
                                    }
                                    else
                                        --rollX;
                        }
                    }
                    if (y8 > 0)
                    {
                        int rollX = Game.rng.Next(y8) - 1;
                        for (int x = 1; x < 10; ++x)
                        {
                            var coords = new Coordinates(x, 8);
                            if (tileTypes.ContainsKey(coords))
                                if (tileTypes[coords] != Tile.TileType.WALL)
                                    if (rollX == 0)
                                    {
                                        tileTypes[coords] = endType;
                                        return tileTypes;
                                    }
                                    else
                                        --rollX;
                        }
                    }
                    if (y7 > 1)
                    {
                        int rollX = Game.rng.Next(y7) - 1;
                        for (int x = 1; x < 10; ++x)
                        {
                            var coords = new Coordinates(x, 7);
                            if (tileTypes.ContainsKey(coords))
                                if (tileTypes[coords] != Tile.TileType.WALL)
                                    if (rollX == 0)
                                    {
                                        tileTypes[coords] = endType;
                                        return tileTypes;
                                    }
                                    else
                                        --rollX;
                        }
                    }


                }
            }

        }

        // recursive function - might replace it by some sort of stack algorithm
        // generates most of the layout - first decides what tiles should be rooms and what tiles should be walls
        // then if the tile should be a room it decides what kind of room it should be

        // not the most effective algorithm - very random and sometimes spews out an almost empty level 
        // which is then discarded by GenerateLayout and the whole process starts from scratch
        static private void GeneratePath(ref Dictionary<Coordinates, Tile.TileType> tileTypes,
            Coordinates current, int levelNum, int path, ref int stores)
        {
            int rollWest = Game.rng.Next(100);
            int rollEast = Game.rng.Next(100);
            int rollNorth = Game.rng.Next(100);
            int rollSouth = Game.rng.Next(100);

            Coordinates newC = current.X > 1 ? new Coordinates(current.X - 1, current.Y) : current;
            if (!tileTypes.ContainsKey(newC))
            {
                if (rollWest > (20 + path * 4 - newC.Y*3)) 
                {
                    if (rollWest % (20 * (stores + 1)) == 0)
                    {
                        tileTypes.Add(newC, Tile.TileType.STORE);
                        ++stores;
                    }
                    else if (rollWest % 12 == 0)
                        tileTypes.Add(newC, Tile.TileType.TREASURE);
                    else if (rollWest % 30 == 0)
                        tileTypes.Add(newC, Tile.TileType.SPECIAL);
                    else
                        tileTypes.Add(newC, Tile.TileType.ROOM);
                    GeneratePath(ref tileTypes, newC, levelNum, path+1, ref stores);
                }
                else
                {
                    tileTypes.Add(newC, Tile.TileType.WALL);
                }
                
            }

            newC = current.Y < 9 ? new Coordinates(current.X, current.Y + 1) : current;
            if (!tileTypes.ContainsKey(newC))
            {
                if (rollNorth > (20 + path * 4 - newC.Y * 3))
                {
                    if (rollNorth % (20 * (stores + 1)) == 0)
                    {
                        tileTypes.Add(newC, Tile.TileType.STORE);
                        ++stores;
                    }
                    else if (rollNorth % 12 == 0)
                        tileTypes.Add(newC, Tile.TileType.TREASURE);
                    else if (rollNorth % 30 == 0)
                        tileTypes.Add(newC, Tile.TileType.SPECIAL);
                    else
                        tileTypes.Add(newC, Tile.TileType.ROOM);
                    GeneratePath(ref tileTypes, newC, levelNum, path + 1, ref stores);
                }
                else
                {
                    tileTypes.Add(newC, Tile.TileType.WALL);
                }
                
            }


            newC = current.X < 9 ? new Coordinates(current.X + 1, current.Y) : current;
            if (!tileTypes.ContainsKey(newC))
            {
                
                if (rollEast > (20 + path * 4 - newC.Y * 3))
                {
                    if (rollEast % (20 * (stores + 1)) == 0)
                    {
                        tileTypes.Add(newC, Tile.TileType.STORE);
                        ++stores;
                    }
                    else if (rollEast % 12 == 0)
                        tileTypes.Add(newC, Tile.TileType.TREASURE);
                    else if (rollEast % 30 == 0)
                        tileTypes.Add(newC, Tile.TileType.SPECIAL);
                    else
                        tileTypes.Add(newC, Tile.TileType.ROOM);
                    GeneratePath(ref tileTypes, newC, levelNum, path + 1, ref stores);
                }
                else
                {
                    tileTypes.Add(newC, Tile.TileType.WALL);
                }

            }

            newC = current.Y > 1 ? new Coordinates(current.X, current.Y - 1) : current;
            if (!tileTypes.ContainsKey(newC))
            {
                if (rollSouth > (20 + path * 4 - newC.Y * 3))
                {
                    if (rollSouth % (20 * (stores + 1)) == 0)
                    {
                        tileTypes.Add(newC, Tile.TileType.STORE);
                        ++stores;
                    }
                    else if (rollSouth % 12 == 0)
                        tileTypes.Add(newC, Tile.TileType.TREASURE);
                    else if (rollSouth % 30 == 0)
                        tileTypes.Add(newC, Tile.TileType.SPECIAL);
                    else
                        tileTypes.Add(newC, Tile.TileType.ROOM);
                    GeneratePath(ref tileTypes, newC, levelNum, path + 1, ref stores);
                }
                else
                {
                    tileTypes.Add(newC, Tile.TileType.WALL);
                }
            }
        }
    }
}
