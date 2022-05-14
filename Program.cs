using System;

namespace HerosAndMonsters
{
    class Program
    {
        public enum DamageType
        {
            Physical = 0,
            Fire = 1,
            Water = 2
        }

        public abstract class Character
        {
            public bool IsDead() => Life <= 0;
            public void DisplayHealthStatus()
            {
                if (IsDead())
                {
                    Console.WriteLine($"  ** {Name} is dead **");
                    return;
                }

                Console.WriteLine($"  {Name}: {Life} HP");
            }

            public string Name;
            public int Life;

            // Index --> DamageType
            public readonly int[] Resistances = new int[] { 0, 0, 0 }; // 0 = Physical, 1 = Fire, 2 = Water

            public Weapon Weapon;

            public void Attack(Character opponent)
            {
                if (opponent == this)
                {
                    return;
                }

                var random = new Random(DateTime.UtcNow.Millisecond); // Seed the random number generator
                var damage = random.Next(Weapon.MinDamage, Weapon.MaxDamage);

                DisplayAttack(opponent);

                opponent.ReceiveDamage(Weapon, damage);
            }

            protected void ReceiveDamage(Weapon weapon, int damage)
            {
                // Check if there is a resistance or weakness against that weapon type
                var resistance = Resistances[(int)weapon.DamageType];
                var adjustedDamage = Math.Max(0, damage - resistance);

                Life = Math.Max(0, Life - adjustedDamage);

                DisplayDamage(weapon, damage, adjustedDamage);
            }

            #region Display Helpers
            protected abstract void DisplayName();
            
            private void DisplayAttack(Character opponent)
            {
                var defaultColor = Console.ForegroundColor;
                
                DisplayName();
                Console.Write($" attacks ");
                opponent.DisplayName();
                Console.Write(" with a ");
                Weapon.Display();
                Console.Write(" weapon ");
                Console.WriteLine();
            }

            private void DisplayDamage(Weapon weapon, int receivedDamage, int adjustedDamage)
            {
                if (adjustedDamage > receivedDamage)
                {
                    DisplayWeakness(weapon, receivedDamage, adjustedDamage);
                    return;
                }

                if (adjustedDamage < receivedDamage)
                {
                    DisplayResistance(weapon, receivedDamage, adjustedDamage);
                    return;
                }

                var defaultColor = Console.ForegroundColor;

                Console.Write("  ");
                DisplayName();
                Console.Write($" was hit by ");
                weapon.Display();
                Console.WriteLine($" weapon: {receivedDamage} damage points");
            }

            private void DisplayResistance(Weapon weapon, int receivedDamage, int adjustedDamage)
            {
                Console.Write("  ");
                DisplayName();
                Console.Write(" resists to ");
                weapon.Display();
                Console.WriteLine($" weapons: {-receivedDamage} --> {-adjustedDamage} damage points");
            }

            private void DisplayWeakness(Weapon weapon, int receivedDamage, int adjustedDamage)
            {
                Console.Write("  ");
                DisplayName();
                Console.Write(" has a weakness to ");
                weapon.Display();
                Console.WriteLine($" weapons: {receivedDamage} --> {adjustedDamage} damage points");
            }
            #endregion
        }

        public class Player : Character
        {
            protected override void DisplayName()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Name);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public class Monster : Character
        {
            protected override void DisplayName()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Name);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public class Weapon
        {
            public DamageType DamageType = DamageType.Physical;
            public string Name;
            public int MinDamage;
            public int MaxDamage;

            public void Display()
            {
                Console.ForegroundColor = DamageType switch
                {
                    DamageType.Physical => ConsoleColor.Magenta,
                    DamageType.Fire => ConsoleColor.DarkYellow,
                    DamageType.Water => ConsoleColor.Cyan,
                    _ => throw new InvalidOperationException($"Unknown damageType '{DamageType}'")
                };
                
                Console.Write(DamageType);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine();

            // -------------------------
            //  Create player & monster
            // -------------------------
            var player = new Player()
            {
                Name = "Arthur",
                Life = 100,
                Weapon = new Weapon()
                {
                    MinDamage = 5,
                    MaxDamage = 10,
                    Name = "Excalibur",
                    DamageType = DamageType.Physical
                }
            };

            var monster = new Monster()
            {
                Name = "Lucifer",
                Life = 100,
                Weapon = new Weapon()
                {
                    MinDamage = 2,
                    MaxDamage = 20,
                    Name = "Spear",
                    DamageType = DamageType.Fire
                }
            };

            // --------------------------------
            //  Set resistances and weaknesses
            // --------------------------------
            player.Resistances[(int)DamageType.Fire] = 0;
            player.Resistances[(int)DamageType.Water] = 2;
            
            monster.Resistances[(int)DamageType.Fire] = 3;
            monster.Resistances[(int)DamageType.Physical] = -2;
            
            // ----------
            //  Gameloop
            // ----------
            while (!player.IsDead() && !monster.IsDead())
            {
                player.DisplayHealthStatus();
                monster.DisplayHealthStatus();

                Console.WriteLine("");

                player.Attack(monster);
                monster.Attack(player);
                
                Console.WriteLine("");
            }

            // --------------
            //  Final status
            // --------------
            player.DisplayHealthStatus();
            monster.DisplayHealthStatus();

            Console.ReadLine();
        }
    }
}
