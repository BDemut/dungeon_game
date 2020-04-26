using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
    abstract class Creature
    {
        public int Hp { protected set; get; }
        public virtual int Strength { protected set; get; }
        public virtual int Toughness { protected set; get; }
        public int Speed { protected set; get; }
        public virtual int Magic { protected set; get; }        //enemies don't use magic as of yet but might use in future
        public Creature(int h, int s, int t, int sp, int m)
        {
            Hp = h;
            Strength = s;
            Toughness = t;
            Speed = sp;
            Magic = m;
        }

        public virtual void Damage(int dmg)
        {
            Hp -= dmg;
        }

        public int Hit(Creature hitee)
        {
            int s = this.Strength + 1 - Game.rng.Next(3);
            int t = hitee.Toughness + 1 - Game.rng.Next(3);
            int dmg = s > t ? s - t : 0;
            hitee.Damage(dmg);
            return dmg;
        }
    }

    class Enemy : Creature
    {
        public String Name { private set; get; }
        public EnemyStrength StrengthType { private set; get; }

        public static readonly String bossName = "Queen of Dragons"; //made it a constant in case a better name comes by
        public enum EnemyStrength
        {
            WEAK,
            MEDIUM,
            STRONG,
            BOSS
        }
        public Enemy(String n, int h, int s, int t, int sp, int m, EnemyStrength str):  base(h,s,t,sp,m)
        {
            Name = n;
            StrengthType = str;
        }
        static public Enemy GetBoss()
        {
            return new Enemy(bossName, 60, 20, 10, 15, 0, EnemyStrength.BOSS);
        }
        static public Enemy GenerateEnemy(EnemyStrength strength)
        {
            int rollType = Game.rng.Next(5);
            int rollStrength = Game.rng.Next(10);
            int rollTough = Game.rng.Next(10);
            int rollHp = Game.rng.Next(10);

            int str = (rollStrength / 5) + 1;
            int tgh = (rollTough / 7);
            int hp = (rollHp / 4) + 5;
            int speed = Game.rng.Next(5)+1;

            switch (strength)
            {
                case EnemyStrength.WEAK:
                    switch (rollType)
                    {
                        case 0:
                            return new Enemy("Rat", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                        case 1:
                            return new Enemy("Bat", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                        case 2:
                            return new Enemy("Slime", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                        case 3:
                            return new Enemy("Wolf", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                        case 4:
                            return new Enemy("Spider", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                        default:
                            return new Enemy("Rat", hp, str, tgh, speed, 0, EnemyStrength.WEAK);
                    }
                case EnemyStrength.MEDIUM:
                    str += (rollStrength / 4 ) + 2;
                    tgh += 1;
                    hp += (rollHp / 4) + 7;
                    switch (rollType)
                    {
                        case 0:
                            return new Enemy("Skeleton", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                        case 1:
                            return new Enemy("Zombie", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                        case 2:
                            return new Enemy("Ork", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                        case 3:
                            return new Enemy("Bear", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                        case 4:
                            return new Enemy("Mad Convict", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                        default:
                            return new Enemy("Skeleton", hp, str, tgh, speed, 0, EnemyStrength.MEDIUM);
                    }
                case EnemyStrength.STRONG:
                    str += (rollStrength / 2) + 6;
                    tgh += (rollTough / 3) + 1;
                    hp += (rollHp / 2) + 15;
                    switch (rollType)
                    {
                        case 0:
                            return new Enemy("Stone Golem", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                        case 1:
                            return new Enemy("Small Dragon", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                        case 2:
                            return new Enemy("Fire Monster", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                        case 3:
                            return new Enemy("Giant", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                        case 4:
                            return new Enemy("Queen of Spiders", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                        default:
                            return new Enemy("Stone Golem", hp, str, tgh, speed, 0, EnemyStrength.STRONG);
                    }
                default:
                    throw new System.SystemException("enemy generator");

            }
        }
    }

    class Player : Creature 
    {
        public int Level { private set; get; }
        public Item RightHand { private set; get; }
        public Item LeftHand { private set; get; }
		public int Gold { set; get; }
        public int Exp { private set; get; }
        public int ExpCap { private set; get; }
        public int HealthCap { private set; get; }
        //overrides to account for items held
        public override int Strength   
        {
            get
            {
                int add = 0;
                if (RightHand!=null && RightHand.GetType() == typeof(Sword))
                {
                    var s = (Sword)RightHand;
                    add += s.Dmg;
                }
                else if (LeftHand!=null && LeftHand.GetType() == typeof(Sword))
                {
                    var s = (Sword)LeftHand;
                    add += s.Dmg;
                }
                return base.Strength + add;
            }
            
            protected set => base.Strength = value; 
        }

        public override int Toughness 
        { 
            get
            {
                int add = 0;
                if (RightHand != null && RightHand.GetType() == typeof(Shield))
                {
                    var s = (Shield)RightHand;
                    add += s.Block;
                }
                else if (LeftHand != null && LeftHand.GetType() == typeof(Shield))
                {
                    var s = (Shield)LeftHand;
                    add += s.Block;
                }
                return base.Toughness + add;
            }

            protected set => base.Toughness = value; 
        }

        public override int Magic 
        { 
            get
            {
                int add = 0;
                if (RightHand != null && RightHand.GetType() == typeof(Wand))
                {
                    var s = (Wand)RightHand;
                    add += s.Power;
                }
                else if (LeftHand != null && LeftHand.GetType() == typeof(Wand))
                {
                    var s = (Wand)LeftHand;
                    add += s.Power;
                }
                return base.Magic + add;
            }

            protected set => base.Magic = value; 
        }

        public Player() : base(10,2,1,1,1)
        {
            Level = 1;
            Exp = 0;
            ExpCap = 10;
            HealthCap = 10;
            Gold = 0;
            RightHand = null;
            LeftHand = null;
        }
		public void PrintInfo()
		{
			int addStr = 0;
			int addTgh = 0;
            int addM = 0;
            if (LeftHand != null)
                if (LeftHand.GetType() == typeof(Sword))
                {
                    var sw = (Sword)LeftHand;
                    addStr += sw.Dmg;
                }
                else if (LeftHand.GetType() == typeof(Shield))
                {
                    var sh = (Shield)LeftHand;
                    addTgh += sh.Block;
                }
                else if (LeftHand.GetType() == typeof(Wand))
                {
                    var w = (Wand)LeftHand;
                    addM += w.Power;
                }
			if (RightHand != null)
				if (RightHand.GetType() == typeof(Sword))
				{
					var sw = (Sword)RightHand;
					addStr += sw.Dmg;
				}
				else if (RightHand.GetType() == typeof(Shield))
				{
					var sh = (Shield)RightHand;
					addTgh += sh.Block;
				}
                else if (RightHand.GetType()==typeof(Wand))
                {
                    var w = (Wand)RightHand;
                    addM += w.Power;
                }

            //a complete mess of a function call - it's beautiful
            Console.WriteLine("Level {6}, Exp {8}/{9}, Health {0}/{7}, Gold {5}\nStrength {1} + {2}, Toughness {3} + {4}, Magic {11} + {12}, Speed {10}",
                Hp, base.Strength, addStr, base.Toughness, addTgh, Gold, Level, HealthCap, Exp, ExpCap, Speed, base.Magic, addM);

			String rHand = RightHand == null ? "Empty" : RightHand.Name;
			String lHand = LeftHand == null ? "Empty" : LeftHand.Name;
			Console.WriteLine("Right hand: " + rHand);
			Console.WriteLine("Left hand: " + lHand);
			Console.WriteLine("");
		}
        
        public void GainExp(int exp)
        {
            if (Exp + exp >= ExpCap)
            {
                exp = (Exp + exp) % ExpCap;
                LevelUp(exp);
            }
            else
                Exp += exp;
        }

        public override void Damage(int dmg)
        {
            if (Hp - dmg > HealthCap)
                Hp = HealthCap;
            else
                Hp -= dmg;
        }
        public void LevelUp(int newExp)
        {
            ++Level;
            Exp = newExp;
            ExpCap += 5;
            HealthCap += 2;
            Hp += 2;
            Console.Clear();
            PrintInfo();
            Console.WriteLine("You have leveled up!");
            if (Level == 1)
                Console.WriteLine("Your max health has increased and you can spend 2 points on upgrades!");
            
            for (int i = 0; i < 2; ++i)
            {
                while (true)
                {
                    Console.WriteLine("S - upgrade strength\nT - upgrade toughness\nP - upgrade speed\nM - upgrade magic");
                    char a = Console.ReadKey().KeyChar;
                    switch (a)
                    {
                        case 's':
                            ++base.Strength;
                            break;
                        case 't':
                            ++base.Toughness;
                            break;
                        case 'p':
                            ++base.Speed;
                            break;
                        case 'm':
                            ++base.Magic;
                            break;
                        default:
                            Console.Clear();
                            PrintInfo();
                            continue;
                    }
                    Console.Clear();
                    PrintInfo();
                    break;
                }
            }
        }

        public Item PickUpItem(Item item, bool rightHand)
        {
            if (rightHand)
            {
                Item ret = this.RightHand;
                this.RightHand = item;
                return ret;
            }
            else
            {
                Item ret = this.LeftHand;
                this.LeftHand = item;
                return ret;
            }
        }

        // a single enemy encounter is wholy contained within this function
        // potential for refactoring
        public bool Fight(Enemy enemy)
        {
            int fireStack = 0;
			Console.WriteLine("A " + enemy.Name + " charges you!");
            while (enemy.Hp>0 && this.Hp>0)
            {
                Console.WriteLine("A - attack, S - spell, F - attempt to flee");
                char move = Console.ReadKey().KeyChar;
                Console.Clear();
                PrintInfo();
                if (move == 'S' || move == 's')             //magic mechanics
                {
                    Console.WriteLine("You used a spell on the enemy.");
                    double stunChance = Math.Pow(0.92, this.Magic);
                    double fireChance = Math.Pow(0.90, this.Magic);
                    double suckChance = Math.Pow(0.95, this.Magic);
                    double stunRoll = Game.rng.NextDouble();
                    double fireRoll = Game.rng.NextDouble();
                    double suckRoll = Game.rng.NextDouble();
                    bool did = false;
                    if (fireRoll > fireChance && fireStack < 5)
                    {
                        Console.WriteLine("{0} is on fire!",enemy.Name);
                        ++fireStack;
                        did = true;
                    }
                    if (suckRoll > suckChance)
                    {
                        int life = this.Magic / 2;
                        this.Damage(-life);
                        enemy.Damage(life);
                        Console.WriteLine("You stole {0} health!",life);
                        did = true;
                    }
                    if (enemy.Hp <= 0)
                    {
                        switch (enemy.StrengthType)
                        {
                            case Enemy.EnemyStrength.WEAK:
                                GainExp(1);
                                break;
                            case Enemy.EnemyStrength.MEDIUM:
                                GainExp(2);
                                break;
                            case Enemy.EnemyStrength.STRONG:
                                GainExp(4);
                                break;
                            default:
                                GainExp(4);
                                break;
                        }
                        Console.WriteLine("You killed the " + enemy.Name + ".\n");
                        break;
                    }
                    if (stunRoll > stunChance)
                    {
                        Console.WriteLine("You stunned the {0}!\nYou get a free move!\n\n", enemy.Name);
                        continue;
                        
                    }
                    else if (!did)
                    {
                        Console.WriteLine("It did nothing! It seems like you lack practice.");
                    }
                }

                else if (move == 'A' || move == 'a')        //normal attack
                {
                    int Pdmg = this.Hit(enemy);
                    Console.WriteLine(String.Format("You hit the {0} for {1} damage!", enemy.Name, Pdmg));
                    if (enemy.Hp <= 0)
                    {
                        switch (enemy.StrengthType)
                        {
                            case Enemy.EnemyStrength.WEAK:
                                GainExp(1);
                                break;
                            case Enemy.EnemyStrength.MEDIUM:
                                GainExp(2);
                                break;
                            case Enemy.EnemyStrength.STRONG:
                                GainExp(4);
                                break;
                            default:
                                GainExp(4);
                                break;
                        }
                        Console.WriteLine("You killed the " + enemy.Name + ".\n");
                        break;
                    }
                }

                else if (move == 'F' || move == 'f')        //run away
                {
                    if (enemy.Speed + Game.rng.Next(6) > this.Speed + Game.rng.Next(6))
                    {
                        if (enemy.Name == Enemy.bossName)
                            Console.WriteLine("It's too late to escape now, the dragon is to quick!");
                        else
                            Console.WriteLine("You failed to escape!");
                    }
                    else
                    {
                        Console.WriteLine("You managed to escape!\nPress any key");
                        Console.ReadKey();
                        return true;
                    }
                }

                else
                {
                    Console.Clear();
                    PrintInfo();
                    continue;
                }

                int Edmg = enemy.Hit(this);
                Console.WriteLine(String.Format("The {0} hit you for {1} damage!", enemy.Name, Edmg));

                if (fireStack > 0)
                {
                    enemy.Damage(fireStack);
                    Console.WriteLine("The {1} took {0} fire damage!", fireStack, enemy.Name);
                    if (enemy.Hp <= 0)
                    {
                        switch (enemy.StrengthType)
                        {
                            case Enemy.EnemyStrength.WEAK:
                                GainExp(1);
                                break;
                            case Enemy.EnemyStrength.MEDIUM:
                                GainExp(2);
                                break;
                            case Enemy.EnemyStrength.STRONG:
                                GainExp(4);
                                break;
                            default:
                                GainExp(4);
                                break;
                        }
                        Console.WriteLine("You killed the " + enemy.Name + ".\n");
                        break;
                    }
                }
                Console.WriteLine("");
            }
            return false;
        }
    }
 
}
