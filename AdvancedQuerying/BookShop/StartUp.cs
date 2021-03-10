namespace BookShop
{
    using Data;
    using Initializer;
    using System.Text;
    using System;
    using System.Linq;
    using BookShop.Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            //var command = Console.ReadLine().ToLower();

            //Console.WriteLine(GetBooksByAgeRestriction(db, command));
            Console.WriteLine(GetGoldenBooks(db));

        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command) 
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context
                        .Books
                        .Where(b => b.AgeRestriction == ageRestriction)
                        .Select(b => b.Title)
                        .OrderBy(t => t)
                        .ToList();

            var result = String.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context
                        .Books
                        .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                        .Select(b => new 
                        {
                            Id = b.BookId,
                            Title = b.Title
                        })
                        .OrderBy(b=>b.Id)
                        .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().Trim();
        }
    }
}
