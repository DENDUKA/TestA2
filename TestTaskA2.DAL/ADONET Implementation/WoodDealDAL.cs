using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TestTaskA2.DAL.IDAL;
using TestTaskA2.Entities;

namespace TestTaskA2.DAL.ADONET_Implementation
{
	public class WoodDealDAL : IWoodDeal
	{
		private static string connectionString = ConfigurationManager.ConnectionStrings["TestA2BD"].ConnectionString;

		//string cmdText = @"
		//	insert into WoodDeal (DealNumber,BuyerId,SellerId,DealDate,WoodVolumeBuyer,WoodVolumeSeller)
		//	select DealNumber,BuyerId,SellerId,DealDate,WoodVolumeBuyer,WoodVolumeSeller
		//	from @wooddeal";
		
		public void BulkInsert(List<WoodDeal> deals)
		{
			DataTable table = new DataTable();
			table.TableName = "WoodDealTableType";

			table.Columns.Add(nameof(WoodDeal.DealNumber), typeof(string));
			table.Columns.Add(nameof(WoodDeal.DealDate), typeof(DateTime));
			table.Columns.Add(nameof(WoodDeal.WoodVolumeBuyer), typeof(float));
			table.Columns.Add(nameof(WoodDeal.WoodVolumeSeller), typeof(float));
			table.Columns.Add("BuyerId", typeof(int));
			table.Columns.Add("SellerId", typeof(int));

			foreach (var deal in deals)
			{
				var row = table.NewRow();

				row[nameof(WoodDeal.DealNumber)] = Helper.GetDBValue(deal.DealNumber);
				row[nameof(WoodDeal.DealDate)] = Helper.GetDBValue(deal.DealDate);
				row[nameof(WoodDeal.WoodVolumeBuyer)] = Helper.GetDBValue(deal.WoodVolumeBuyer); 
				row[nameof(WoodDeal.WoodVolumeSeller)] = Helper.GetDBValue(deal.WoodVolumeSeller);
				row["BuyerId"] = deal.Buyer.Id;
				row["SellerId"] = deal.Seller.Id;

				table.Rows.Add(row);
			}

			using (var con = new SqlConnection(connectionString))
			{
				var command = new SqlCommand("EXECUTE BulkInsertWoodDeal @table", con);
				
				var param = command.Parameters.AddWithValue("@table", table);
				param.SqlDbType = SqlDbType.Structured;
				param.TypeName = "dbo.WoodDealTableType";


				con.Open();
				var reader = command.ExecuteReader();
			}

			//using (var bulkInsert = new SqlBulkCopy(connectionString))
			//{
			//	bulkInsert.DestinationTableName = table.TableName;
			//	bulkInsert.WriteToServer(table);
			//}



			//using (var connection = new SqlConnection(connectionString))
			//{
			//	var command = new SqlCommand(cmdText, connection);
			//	var param = command.Parameters.AddWithValue("@wooddeal", deals);
			//	param.TypeName = "dbo.WoodDealTableType";
			//	connection.Open();
			//	command.ExecuteNonQuery();
			//}
		}

		//public void Add(WoodDeal deal)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		var command = new SqlCommand("Insert into WoodDeal(DealNumber,BuyerId,SellerId,DealDate,WoodVolumeBuyer,WoodVolumeSeller) VALUES (@DealNumber,@BuyerId,@SellerId,@DealDate,@WoodVolumeBuyer,@WoodVolumeSeller) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];", con);
		//		command.Parameters.Add(new SqlParameter("@DealNumber", deal.DealNumber));
		//		command.Parameters.Add(new SqlParameter("@BuyerId", deal.Buyer.Id));
		//		command.Parameters.Add(new SqlParameter("@SellerId", deal.Seller.Id));
		//		command.Parameters.Add(new SqlParameter("@DealDate", deal.DealDate));
		//		command.Parameters.Add(new SqlParameter("@WoodVolumeBuyer", deal.WoodVolumeSeller));
		//		command.Parameters.Add(new SqlParameter("@WoodVolumeSeller", deal.WoodVolumeSeller));

		//		con.Open();
		//		var reader = command.ExecuteReader();

		//		if (reader.Read())
		//		{
		//			deal.Id = (int)(decimal)reader["SCOPE_IDENTITY"];
		//		}
		//	}
		//}

		//public bool IsExistByDealNumber(string dealNumber)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		var command = new SqlCommand("SELECT TOP (1) Id FROM WoodDeal WHERE DealNumber = @DealNumber", con);
		//		command.Parameters.Add(new SqlParameter("@DealNumber", dealNumber));

		//		con.Open();
		//		var reader = command.ExecuteReader();

		//		return reader.Read();
		//	}
		//}
	}
}
