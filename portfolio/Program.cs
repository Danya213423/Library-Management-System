using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ConsoleApp16.Program;

namespace ConsoleApp16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool s = true;
            List<Books> books = new List<Books>();
            string filePath = "Home.json";
            Console.WriteLine($"Файл данных: {Path.GetFullPath(filePath)}");
            LoadFromFile(ref books, filePath);

            do
            {
                Console.WriteLine("\n=== Библиотека книг ===");
                Console.WriteLine("1 - Добавить книгу");
                Console.WriteLine("2 - Список всех книг");
                Console.WriteLine("3 - Поиск по названию");
                Console.WriteLine("4 - Удалить книгу");
                Console.WriteLine("5 - Выход");
                Console.Write("\nВыберите действие: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        AddBook(books, filePath);
                        break;
                    case "2":
                        Console.Clear();
                        ShowAllBooks(books);
                        break;
                    case "3":
                        Console.Clear();
                        SearchBook(books);
                        break;
                    case "4":
                        Console.Clear();
                        DeleteBook(books, filePath);
                        break;
                    case "5":
                        Console.Clear();
                        s = false;
                        SaveToFile(books, filePath);
                        break;
                    default:
                        Console.WriteLine("Ошибка: Неверный пункт меню.");
                        break;
                }
            }while (s);
        }

        public record Books(string Kniga, string Aftor, string Clasifecate, string Have);


        static void DeleteBook(List<Books> books, string filePath)
        {
            Console.Write("Введите название книги для удаления: ");
            string searchNam = Console.ReadLine();

            var foundBook = books
                .Where(book => book.Kniga.ToLower() == searchNam.ToLower())
                .ToList();

            if (foundBook.Count == 1)
            {
                books.Remove(foundBook[0]);
                Console.WriteLine("Книга удалена!");
            }
            else
            {
                Console.WriteLine($"Найдено несколько книг ({foundBook.Count}):");
                for (var i = 0; i < foundBook.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {foundBook[i].Kniga} (Автор: {foundBook[i].Aftor})");
                }

                Console.Write("Введите номер книги для удаления (или 0 для отмены): ");
                var input2 = Console.ReadLine();

                if (int.TryParse(input2, out var choice2) && choice2 > 0 && choice2 <= foundBook.Count)
                {
                    var bookToDelete = foundBook[choice2 - 1];
                    books.Remove(bookToDelete);
                    Console.WriteLine("Выбранная книга удалена!");
                }
                else
                {
                    Console.WriteLine("Удаление отменено.");
                }
            }
            Console.ReadKey();
            Console.Clear();
            SaveToFile(books, filePath);
        }
        static void SearchBook(List<Books> books)
        {
            Console.Write("Введите название книги: ");
            string searchName = Console.ReadLine();

            var foundBooks = books
            .Where(book => book.Kniga.ToLower() == searchName.ToLower())
            .ToList();

            if (foundBooks.Count == 0)
            {
                Console.WriteLine("Не найдено");
            }
            else
            {
                foreach (var book in foundBooks)
                {
                    Console.WriteLine("=============================");
                    Console.WriteLine($"Название: {book.Kniga}");
                    Console.WriteLine($"Автор: {book.Aftor}");
                    Console.WriteLine($"Раздел: {book.Clasifecate}");
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine($"Содержимое: {book.Have}");
                    Console.WriteLine("=============================");
                }
            }
            Console.ReadKey();
            Console.Clear();
        }

        static void ShowAllBooks(List<Books> books)
        {
            Console.WriteLine("\n=== Список всех книг в библиотеке ===");

            if (books.Count == 0)
            {
                Console.WriteLine("Библиотека пока пуста.");
            }
            else
            {
                for (int i = 0; i < books.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {books[i].Kniga} ({books[i].Aftor})");
                }
            }

            Console.WriteLine($"{books.Count + 1}. Назад в меню");
            Console.Write("Введите номер книги: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var choice))
            {
                if (choice > 0 && choice <= books.Count)
                {
                    var book = books[choice - 1];

                    Console.Clear();
                    Console.WriteLine("=== Детальная информация ===");
                    Console.WriteLine($"Название: {book.Kniga}");
                    Console.WriteLine($"Автор: {book.Aftor}");
                    Console.WriteLine($"Раздел: {book.Clasifecate}");
                    Console.WriteLine(new string('-', 30));
                    Console.WriteLine($"Содержимое:\n{book.Have}");
                    Console.WriteLine(new string('=', 30));
                }
                else if (choice == books.Count + 1)
                {
                    Console.WriteLine("Возвращаемся в меню...");
                }
                else
                {
                    Console.WriteLine("Ошибка: Книги под таким номером нет.");
                }
            }
            else
            {
                Console.WriteLine("Ошибка: Введите число!");
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
            Console.Clear();
        }

        static void AddBook(List<Books> books, string filePath)
        {
            Console.WriteLine("\n--- Добавление новой записи ---");

            Console.Write("Введите название книги: ");
            string title = Console.ReadLine();
            title = (string.IsNullOrWhiteSpace(title) ? null : title) ?? "Без названия";

            Console.Write("Введите автора: ");
            string author = Console.ReadLine();
            author = (string.IsNullOrWhiteSpace(author) ? null : author) ?? "Автор не указан";

            Console.Write("Введите раздел: ");
            string razdel = Console.ReadLine();
            razdel = (string.IsNullOrWhiteSpace(razdel) ? null : razdel) ?? "Общий раздел";

            Console.Write("Введите содержимое: ");
            string content = Console.ReadLine();
            content = (string.IsNullOrWhiteSpace(content) ? null : content) ?? "Содержимое отсутствует";

            var newBook = new Books(title, author, razdel, content);
            books.Add(newBook);

            SaveToFile(books, filePath);

            Console.WriteLine("\n[Успех]: Данные записаны в базу.");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
            Console.Clear();
        }






            static void SaveToFile(List<Books> students, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(students, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Сохранено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }
        static void LoadFromFile(ref List<Books> students, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден. Начинаем новую библиотеку.");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                students = JsonConvert.DeserializeObject<List<Books>>(json) ?? new List<Books>();
                Console.WriteLine($"Загружено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                students = new List<Books>();
            }
        }

    }
}
