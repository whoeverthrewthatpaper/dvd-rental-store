using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Database
{
    class Copy
    {
		private static readonly string table = "copies";
		public int Id { get; private set; }
		public bool Available { get; set; }
		public int MovieId { get; set; }
		public Copy(int id, bool available, int movieId)
		{
			this.Id = id;
			this.Available = available;
			this.MovieId = movieId;
		}

		public Copy(bool available, int movieId)
		{
			this.Id = Copy.GetAll().Max(c => c.Id) + 1;
			this.Available = available;
			this.MovieId = movieId;
		}
		//Removing by using CopyId
		public void Delete()
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"DELETE FROM {table} WHERE copy_id = @Id", conn))
				{
					command.Parameters.AddWithValue("@Id", Id);
					command.ExecuteNonQuery();
				}
			}
		}

		public static IEnumerable<Copy> GetAll()
		{
			List<Copy> copies = new List<Copy>();

			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"SELECT * FROM {table}", conn))
				{
					NpgsqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
						copies.Add(new Copy((int)reader["copy_id"], (bool)reader["available"], (int)reader["movie_id"]));

					if (copies.Count() != 0)
						return copies;
				}
			}
			return null;
		}

		public static Copy GetByID(int id)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"SELECT * FROM {table} WHERE copy_id = @Id", conn))
				{
					command.Parameters.AddWithValue("@Id", id);

					NpgsqlDataReader reader = command.ExecuteReader();
					if (reader.HasRows)
					{
						reader.Read();
						return new Copy(id, (bool)reader["available"], (int)reader["movie_id"]);
					}
				}
			}
			return null;
		}

		public void Save(NpgsqlTransaction transaction = null)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();

				using (var command = new NpgsqlCommand($"INSERT INTO {table}(copy_id, available, movie_id) " +
					"VALUES (@Id, @Available, @MovieId) " +
					"ON CONFLICT (copy_id) DO UPDATE " +
					"SET available = @Available, movie_id = @MovieId", conn))
				{
					if (transaction != null) command.Transaction = transaction;

					command.Parameters.AddWithValue("@Id", Id);
					command.Parameters.AddWithValue("@Available", Available);
					command.Parameters.AddWithValue("@MovieId", MovieId);

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
