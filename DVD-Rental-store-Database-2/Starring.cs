using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Database
{
    class Starring
    {
		public static readonly string table = "starring";
		public int ActorId { get; private set; }
		public int MovieId { get; set; }
		public Starring(int actorId, int movieId)
		{
			this.ActorId = actorId;
			this.MovieId = movieId;
		}
		//Removing by using ActorId and MovieId
		public void Delete()
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"DELETE FROM {table} WHERE actor_id = @ActorId AND movie_id = @MovieId", conn))
				{
					command.Parameters.AddWithValue("@ActorId", ActorId);
					command.Parameters.AddWithValue("@MovieId", MovieId);
					command.ExecuteNonQuery();
				}
			}
		}

		public static IEnumerable<Starring> GetAll()
		{
			List<Starring> Starrings = new List<Starring>();

			using (NpgsqlConnection conn = new NpgsqlConnection(connection_string))
			{
				conn.Open();
				using (var command = new NpgsqlCommand($"SELECT * FROM {table}", conn))
				{
					NpgsqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
						Starrings.Add(new Starring((int)reader["actor_id"], (int)reader["movie_id"]));

					if (Starrings.Count() != 0)
						return Starrings;
				}
			}
			return null;
		}
	}
}
