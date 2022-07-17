using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TestTaskA2.DAL.IDAL;
using TestTaskA2.Entities;

namespace TestTaskA2.DAL.ADONET_Implementation
{
	public class BuyerDAL : ICustomer
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["TestA2BD"].ConnectionString;
		private static int nameFieldMaxLength = 300;

		//public void Add(Customer buyer)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		var command = new SqlCommand("Insert into Buyer (Inn,Name) VALUES (@Inn, @Name) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];", con);
		//		command.Parameters.Add(new SqlParameter("@Inn", buyer.Inn));
		//		command.Parameters.Add(new SqlParameter("@Name", buyer.Name != null && buyer.Name.Length > nameFieldMaxLength ? buyer.Name.Substring(0, nameFieldMaxLength) : buyer.Name));

		//		con.Open();
		//		var reader = command.ExecuteReader();

		//		if (reader.Read())
		//		{
		//			buyer.Id = (int)(decimal)reader["SCOPE_IDENTITY"];
		//		}
		//	}
		//}

		public void BulkInsertOrFillId(List<Customer> buyers)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			DataTable table = new DataTable();
			table.TableName = "InnNamePairTableType";

			var uniqBuyers = buyers.GroupBy(x => x.Inn).Select(x => x.First()).ToList();

			table.Columns.Add(nameof(Buyer.Inn), typeof(string));
			table.Columns.Add(nameof(Buyer.Name), typeof(string));

			foreach (var b in uniqBuyers)
			{
				var row = table.NewRow();

				row[nameof(Buyer.Inn)] = Helper.GetDBValue(b.Inn);
				row[nameof(Buyer.Name)] = Helper.GetDBValue(b.Name);

				table.Rows.Add(row);
			}

			//Console.WriteLine($"Inn Max Length : {buyers.Max(x => x.Inn.Length)} Name Max Length :  {buyers.Max(x => x.Name.Length)}");

			using (var con = new SqlConnection(connectionString))
			{
				var command = new SqlCommand("EXECUTE BulkInsertOrFillIdBuyers @table", con);
				command.CommandTimeout = 400;

				var param = command.Parameters.AddWithValue("@table", table);
				param.SqlDbType = SqlDbType.Structured;
				param.TypeName = "dbo.InnNamePairTableType";

				con.Open();
				var reader = command.ExecuteReader();

				int i = 0;

				while (reader.Read())
				{
					int id = (int)reader["Id"];
					string inn = (string)reader["Inn"];

					if (!result.ContainsKey(inn))
					{
						result.Add(inn, id);
					}
				}
			}

			Console.WriteLine($"Кол-во уникальных покупателей : {result.Count}");

			foreach (var b in buyers)
			{
				if (result.ContainsKey(b.Inn))
				{
					b.Id = result[b.Inn];
				}
				else
				{
					//Тут должно быть логирование не обработанных данных
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Ошибка чтения из БД Buyer {b}");
					Console.ResetColor();
				}
			}
		}

		/// <summary>
		/// Получение Id по Inn
		/// </summary>
		/// <param name="inn"></param>
		/// <returns>Возвращает Id, если Inn найдено. 0 - если не найдено</returns>
		//public int GetIdByInn(string inn)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		var command = new SqlCommand("SELECT TOP (1) Id FROM Buyer WHERE Inn = @Inn", con);
		//		command.Parameters.Add(new SqlParameter("@Inn", inn));
		//		con.Open();
		//		var reader = command.ExecuteReader();

		//		if (reader.Read())
		//		{
		//			return (int)reader["Id"];
		//		}
		//		else
		//		{
		//			return 0;
		//		}
		//	}
		//}
	}
}
