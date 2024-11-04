using Microsoft.EntityFrameworkCore;
using System;
using Npgsql;
using System.Collections.Generic;
using (ApplicationContext db = new ApplicationContext())
{
    // создаем два объекта Employee
    Employee tom = new Employee { FullName = "Tom" };
    Employee alice = new Employee { FullName = "Alice" };

    // добавляем их в бд
    db.Employee.Add(tom);
    db.Employee.Add(alice);
    db.SaveChanges();
    Console.WriteLine("Объекты успешно сохранены");

    // получаем объекты из бд и выводим на консоль
    var Employee = db.Employee.ToList();
    Console.WriteLine("Список объектов:");
    foreach (Employee u in Employee)
    {
        Console.WriteLine($"{u.FullName}.{u.BirthDate} - {u.Gender}");
    }
}

public class Employee
{
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }

    public void InsertIntoDatabase()
    {
        string connString = "Host=localhost;Username=myuser;Password=mypassword;Database=mydatabase";
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("INSERT INTO employees (full_name, birth_date, gender) VALUES (@FullName, @BirthDate, @Gender)", conn))
            {
                cmd.Parameters.AddWithValue("FullName", FullName);
                cmd.Parameters.AddWithValue("BirthDate", BirthDate);
                cmd.Parameters.AddWithValue("Gender", Gender);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public int CalculateAge()
    {
        int age = DateTime.Today.Year - BirthDate.Year;
        if (BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        return age;
    }
}



public class ApplicationContext : DbContext
{
    public DbSet<Employee> Employee => Set<Employee>();
    public ApplicationContext() => Database.EnsureCreated();
    public DbSet<Employee> Worker { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=staffapp.db");
    }
}

partial class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 5)
        {
            Console.WriteLine("Недостаточно аргументов. Пример использования: myApp 2 \"Ivanov Petr Sergeevich\" 2009-07-12 Male");
            return;
        }

        string fullName = args[2];
        DateTime birthDate = DateTime.Parse(args[3]);
        string gender = args[4];

        Employee newEmployee = new Employee
        {
            FullName = fullName,
            BirthDate = birthDate,
            Gender = gender
        };

        newEmployee.InsertIntoDatabase();
        Console.WriteLine("Запись успешно добавлена в базу данных.");
    }
}



