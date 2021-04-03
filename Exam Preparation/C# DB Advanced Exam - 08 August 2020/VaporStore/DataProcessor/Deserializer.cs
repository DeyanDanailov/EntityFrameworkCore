namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		private const string ErrorMessage = "Invalid Data";
		private const string SuccessfullyAddedGame = "Added {0} ({1}) with {2} tags";
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            StringBuilder sb = new StringBuilder();

            List<Game> gamestoAdd = new List<Game>();

            ImportGamesDto[] games = JsonConvert.DeserializeObject<ImportGamesDto[]>(jsonString);
            List<Developer> developers = new List<Developer>();
            List<Genre> genres = new List<Genre>();
            List<Tag> tags = new List<Tag>();
            foreach (var game in games)
            {
                if (!IsValid(game))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }

                DateTime releaseDate;
                bool isDateValid = DateTime.TryParseExact(game.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out releaseDate);
                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (game.Tags.Length == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Game gametoAdd = new Game()
                {
                    Name = game.Name,
                    Price = game.Price,
                    ReleaseDate = releaseDate
                };
                var dev = developers.FirstOrDefault(d => d.Name == game.Developer);
                if (dev == null)
                {
                    dev = new Developer()
                    {
                        Name = game.Developer
                    };

                    developers.Add(dev);
                }

                gametoAdd.Developer = dev;

                var genre = genres.FirstOrDefault(g => g.Name == game.Genre);
                if (genre == null)
                {
                    genre = new Genre()
                    {
                        Name = game.Genre
                    };
                    genres.Add(genre);
                }

                gametoAdd.Genre = genre;

                foreach (var tagItem in game.Tags)
                {
                    if (String.IsNullOrEmpty(tagItem))
                    {
                        continue;
                    }

                    Tag tag = tags.FirstOrDefault(x => x.Name == tagItem);
                    if (tag == null)
                    {
                        Tag t = new Tag()
                        {
                            Name = tagItem
                        };
                        tags.Add(t);

                        gametoAdd.GameTags.Add(new GameTag()
                        {
                            Game = gametoAdd,
                            Tag = t
                        });
                    }
                    else
                    {
                        gametoAdd.GameTags.Add(new GameTag()
                        {
                            Game = gametoAdd,
                            Tag = tag
                        });
                    }

                }

                if (!gametoAdd.GameTags.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                gamestoAdd.Add(gametoAdd);
                sb.AppendLine(string.Format(SuccessfullyAddedGame, gametoAdd.Name, gametoAdd.Genre.Name,
                    gametoAdd.GameTags.Count));
            }

            context.Games.AddRange(gamestoAdd);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			throw new NotImplementedException();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}