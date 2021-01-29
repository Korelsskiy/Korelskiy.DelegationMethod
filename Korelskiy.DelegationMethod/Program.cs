using System;
using System.Collections.Generic;
using System.Data.SqlClient;
// Техника делегирования
namespace Korelskiy.DelegationMethod
{
    class Weapon
    {
        public string Name { get; }
        public int Damage { get; }
        public Weapon(string name)
        {
            Name = name;
            Damage = new Random().Next(25, 50);
        }
        public void Shoot(Soldier target)
        {
            target.Health -= Damage;
            
            Console.WriteLine($"Нанесено {Damage} урона по {target.Name}");
        }
    }
    class Soldier
    {
        public string Name { get; }
        public string LastName { get; set; }
        public int Health { get; set; }
        public bool IsDead { get; set; }
        public Weapon Weapon { get; }
        private readonly DbManager db = new DbManager();
        public Soldier(string weaponName)
        {
            Name = db.GetRandomName();
            LastName = db.GetRandomLastName();
            Health = new Random().Next(75, 125);
            IsDead = false;
            Weapon = new Weapon(weaponName);

            DisplaySoldier();
        }
        private void DisplaySoldier()
        {
            Console.WriteLine($"БОТ {Name} {LastName} Здоровье - {Health}");
        }
        public void Shoot(Soldier target)
        {
            Weapon.Shoot(target);
            if (target.Health <= 0)
            {
                target.IsDead = true;
                Console.WriteLine($"Боец {target.Name} погиб");
                Console.ReadKey();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Soldier playerSoldier = new Soldier("Винтовка Мосина");
            Soldier targetSoldier = new Soldier("Винтовка Маузера");

            do
            {
                Console.ReadKey();
                playerSoldier.Shoot(targetSoldier);
            } while (!targetSoldier.IsDead);

            Console.ReadKey();
        }
    }

    class DbManager
    {
        private readonly SqlConnection connection;
        public DbManager()
        {
            connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BattleAppDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
        public string GetRandomName()
        {
            List<string> names = new List<string>();
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT USSRName FROM Names", connection);
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    names.Add(dataReader["USSRName"].ToString());
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return names[new Random().Next(0, names.Count)];
        }

        public string GetRandomLastName()
        {
            List<string> lastNames = new List<string>();
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT USSRLastName FROM LastNames", connection);
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    lastNames.Add(dataReader["USSRLastName"].ToString());
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return lastNames[new Random().Next(0, lastNames.Count)];
        }
    }
}
