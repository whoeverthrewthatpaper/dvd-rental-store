﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Database
{
    class Movie
    {
		public string connection_string = "Server=localhost;User Id=postgres;Password=Postgres@1029;Database=rentals;";

		public static readonly string table = "movies";

		public int Id { get; private set; }
		public string Title { get; set; }
		public int Year { get; set; }
		public int AgeRestriction { get; set; }
		public float Price { get; set; }
		public Movie(int id, string title, int year, int AgeRestriction, float price)
		{
			this.Id = id;
			this.Title = title;
			this.Year = year;
			this.AgeRestriction = AgeRestriction;
			this.Price = price;
		}

		public Movie(string title, int year, int AgeRestriction, float price)
		{
			this.Id = Movie.GetAll().Max(c => c.Id) + 1;
			this.Title = title;
			this.Year = year;
			this.AgeRestriction = AgeRestriction;
			this.Price = price;
		}
		//Deleting by using MovieId
		public void Delete()
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"DELETE FROM {table} WHERE movie_id = @Id", conn))
				{
					command.Parameters.AddWithValue("@Id", Id);
					command.ExecuteNonQuery();
				}
			}
		}

		public static IEnumerable<Movie> GetAll()
		{
			List<Movie> Movies = new List<Movie>();

			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"SELECT * FROM {table}", conn))
				{
					NpgsqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
						Movies.Add(new Movie((int)reader["movie_id"], (string)reader["title"], (int)reader["year"], (int)reader["age_restriction"], (float)reader["price"]));

					if (Movies.Count() != 0)
						return Movies;
				}
			}
			return null;
		}
		//Fetching by using Movie Id
		public static Movie GetByID(int id)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"SELECT * FROM {table} WHERE movie_id = @Id", conn))
				{
					command.Parameters.AddWithValue("@Id", id);

					NpgsqlDataReader reader = command.ExecuteReader();
					if (reader.HasRows)
					{
						reader.Read();
						return new Movie(id, (string)reader["title"], (int)reader["year"], (int)reader["age_restriction"], (float)reader["price"]);
					}
				}
			}
			return null;
		}
		//saving on the Memory
		public void Save(NpgsqlTransaction transaction = null)
		{
		using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
		{
			conn.Open();

			using (var command = new NpgsqlCommand($"INSERT INTO {table}(movie_id, title, year, age_restriction, price) " +
				"VALUES (@Id, @Title, @Year, @AgeRestriction, @Price) " +
				"ON CONFLICT (movie_id) DO UPDATE " +
				"SET title = @Title, year = @Year, age_restriction = @AgeRestriction, price = @Price", conn))
			{
				if (transaction != null) command.Transaction = transaction;

				command.Parameters.AddWithValue("@Id", Id);
				command.Parameters.AddWithValue("@Title", Title);
				command.Parameters.AddWithValue("@Year", Year);
				command.Parameters.AddWithValue("@AgeRestriction", AgeRestriction);
				command.Parameters.AddWithValue("@Price", Price);

				command.ExecuteNonQuery();
			}
		}
		}
	}
}
