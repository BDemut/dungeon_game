using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    class Game
    {
        static public Random rng = new Random();            // a pseudo-random number generator for the entire program
        static void Main(string[] args)
        {
            bool play = true;
            while (play)
            {
                Console.WriteLine("Generating world");
                Session session = new Session();
                Console.WriteLine("World generated");
                Console.Clear();
                session.Play();

                Console.WriteLine("Play again?");
                Console.WriteLine("Press 'Y' if yes");
                char a = Console.ReadKey().KeyChar;
                if (!(a == 'Y' || a == 'y'))
                    play = false;
                Console.Clear();
            }
        }
    }
    // Structure of the game:
    // Game.Main hosts sessions (only one at a time) and handles going in and out of sessions (just the "play again?" code pretty much)
    // A Session is one game playtrough - it has 3 levels and a Player variable. It handles going in and out of the levels.
    // A Level is... well a level. It consists of a 9x9 table of tiles. It handles going in and out of the tiles and communication between them.
    // A Tile is a smallest map division - a room. It might have some enemies, gold, a store etc. It handles everything that is happening within it.

    // I decided not to make classes like factories or builders and put all the code that creates and instance of a class
    // inside that class as static methods (in simple cases it's all done by a constructor).
    class Session
    {
        private readonly Level[] levels;
        private Player player;

        public Session()
        {
            levels = new Level[4];
            levels[0] = null;
            for (int i = 1; i < 4; ++i)
            {
                levels[i] = Level.GenerateLevel(i);
            }
            player = new Player();
        }

        public void Play()
        {
            Console.WriteLine("You have been convicted of theft and thrown into a dungeon.");
            Console.WriteLine("Find a way to escape or die of hunger.\n");
            Console.WriteLine("You find yourself in a damp, dark room.");
            Console.WriteLine("A human skeleton lies in the corner.\nYou know that the only way to survive is to move forward.\n");
            Console.WriteLine("Press a key.");
            Console.ReadKey();
            Console.Clear();
            int next = 1;
            int prev = 1;
            while (!(next == 0 || next == 4))
            {
                if (prev <= next)
                {
                    prev = next;
                    next = levels[next].PlayLevel(ref player);
                }
                else
                {
                    prev = next;
                    next = levels[next].PlayLevel(ref player, false);
                }

            }
            if (next == 0)
            {
                Console.WriteLine("You died!");
                return;
            }
            else if (next == 4)
            {
                Console.WriteLine("Congratulations, you won!");
                return;
            }
            else
                throw new System.SystemException("wrong 'next' value in session.play()");
        }
    }
}
