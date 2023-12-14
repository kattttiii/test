using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

public class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

// Класс для таблицы рекордов
public static class Leaderboard
{
    private static List<User> leaderboard = new List<User>();
    private static readonly string leaderboardFilePath = "leaderboard.json";

    // Загрузка таблицы рекордов из файла
    public static void LoadLeaderboard()
    {
        if (File.Exists(leaderboardFilePath))
        {
            using (FileStream stream = new FileStream(leaderboardFilePath, FileMode.Open))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<User>));
                leaderboard = (List<User>)serializer.ReadObject(stream);
            }
        }
    }

    // Сохранение таблицы рекордов в файл
    public static void SaveLeaderboard()
    {
        using (FileStream stream = new FileStream(leaderboardFilePath, FileMode.Create))
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<User>));
            serializer.WriteObject(stream, leaderboard);
        }
    }

    // Добавление пользователя в таблицу рекордов
    public static void AddUser(User user)
    {
        leaderboard.Add(user);
        leaderboard.Sort((u1, u2) => u2.CharactersPerMinute.CompareTo(u1.CharactersPerMinute));
        SaveLeaderboard();
    }

    // Вывод таблицы рекордов
    public static void DisplayLeaderboard()
    {
        Console.WriteLine("Leaderboard:");
        foreach (var user in leaderboard)
        {
            Console.WriteLine($"{user.Name}: {user.CharactersPerMinute} CPM, {user.CharactersPerSecond} CPS");
        }
    }
}

// Класс для теста на набор текста
public class TypingTest
{
    private static readonly string TestText = "Your test text goes here..."; // Замените этот текст на ваш тестовый текст

    // Метод для начала теста
    public static void StartTest(string userName)
    {
        User user = new User { Name = userName };

        Console.WriteLine($"Welcome, {userName}! Press Enter to start the typing test.");
        Console.ReadLine();

        Console.WriteLine($"Type the following text:\n{TestText}");

        Stopwatch stopwatch = Stopwatch.StartNew();

        StringBuilder userInput = new StringBuilder();
        ConsoleKeyInfo keyInfo;

        Console.Write("> "); // Добавим знак "больше" для обозначения ввода текста

        do
        {
            keyInfo = Console.ReadKey(true); // true - не выводить нажатые клавиши в консоль
            if (keyInfo.Key != ConsoleKey.Enter)
            {
                userInput.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        } while (keyInfo.Key != ConsoleKey.Enter);

        stopwatch.Stop();

        Console.WriteLine(); // Переходим на новую строку после ввода

        if (userInput.ToString().Equals(TestText))
        {
            // Вычисляем статистику и обновляем пользователя в таблице рекордов
            UpdateUserStatistics(user, TestText.Length, stopwatch);
        }
        else
        {
            Console.WriteLine("Incorrect input.");
        }

        Leaderboard.DisplayLeaderboard();
    }

    // Метод для обновления статистики пользователя и добавления его в таблицу рекордов
    private static void UpdateUserStatistics(User user, int textLength, Stopwatch stopwatch)
    {
        double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
        double charactersPerMinute = textLength / elapsedMinutes;
        double charactersPerSecond = textLength / stopwatch.Elapsed.TotalSeconds;

        user.CharactersPerMinute = (int)charactersPerMinute;
        user.CharactersPerSecond = (int)charactersPerSecond;

        Leaderboard.AddUser(user);

        Console.WriteLine($"Time: {stopwatch.Elapsed:mm\\:ss}");
    }
}

class Program
{
    static void Main()
    {
        Leaderboard.LoadLeaderboard();

        string playAgain;
        do
        {
            Console.Write("Enter your name: ");
            string userName = Console.ReadLine();

            Console.Clear();

            TypingTest.StartTest(userName);

            Console.Write("Do you want to try again? (y/n): ");
            playAgain = Console.ReadLine();
        } while (playAgain?.ToLower() == "y");

    }
}
