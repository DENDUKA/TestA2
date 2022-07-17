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
	public class SellerDAL : ICustomer
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["TestA2BD"].ConnectionString;

		//public void Add(Customer seller)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		var command = new SqlCommand("Insert into Seller (Inn,Name) VALUES (@Inn, @Name) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];", con);
		//		command.Parameters.Add(new SqlParameter("@Inn", seller.Inn));
		//		command.Parameters.Add(new SqlParameter("@Name", seller.Name != null && seller.Name.Length > nameFieldMaxLength ? seller.Name.Substring(0, nameFieldMaxLength) : seller.Name));

		//		con.Open();
		//		var reader = command.ExecuteReader();

		//		if (reader.Read())
		//		{
		//			seller.Id = (int)(decimal)reader["SCOPE_IDENTITY"];
		//		}
		//	}
		//}

		public void BulkInsertOrFillId(List<Customer> sellers)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			DataTable table = new DataTable();
			table.TableName = "InnNamePairTableType";

			var uniqSellers = sellers.GroupBy(x => x.Inn).Select(x => x.First()).ToList();

			table.Columns.Add(nameof(Buyer.Inn), typeof(string));
			table.Columns.Add(nameof(Buyer.Name), typeof(string));

			foreach (var s in uniqSellers)
			{
				var row = table.NewRow();

				row[nameof(Buyer.Inn)] = Helper.GetDBValue(s.Inn);
				row[nameof(Buyer.Name)] = Helper.GetDBValue(s.Name);

				table.Rows.Add(row);
			}			

			using (var con = new SqlConnection(connectionString))
			{
				var command = new SqlCommand("EXECUTE BulkInsertOrFillIdSellers @table", con);
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

			Console.WriteLine($"Кол-во уникальных продавцов : {result.Count}");

			foreach (var s in sellers)
			{
				if (result.ContainsKey(s.Inn))
				{
					s.Id = result[s.Inn];
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Ошибка чтения из БД Seller {s}");
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
		//		var command = new SqlCommand("SELECT TOP (1) Id FROM Seller WHERE Inn = @Inn", con);
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
