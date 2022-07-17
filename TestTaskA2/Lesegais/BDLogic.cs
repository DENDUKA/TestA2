using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestTaskA2.DAL.ADONET_Implementation;
using TestTaskA2.DAL.IDAL;
using TestTaskA2.Entities;

namespace TestTaskA2.Lesegais
{
	/// <summary>
	/// Этот класс нужно былобы вынести на дрйгой слой и передавать в него Интерфейсы для БД как DI, но в рамках этой задачи достаточно и такой реализации
	/// </summary>
	public static class BDLogic
	{
		// от такой жесткой привязки композиции нужно избавляться, но в рамках этой задачи это лишнее время на разработку
		private static ICustomer buyerDAL = new BuyerDAL();
		private static ICustomer sellerDAL = new SellerDAL();
		private static IWoodDeal woodDealDAL = new WoodDealDAL();

		private static Stopwatch sw = new Stopwatch();

		public static void AddWoodDealBatch(List<WoodDeal> wds)
		{
			List<WoodDeal> toInsert = new List<WoodDeal>();
			List<WoodDeal> withError = new List<WoodDeal>();

			Console.WriteLine($"Начало проверки в БД Seller Buyer");

			sw.Restart();


			var insBuyTask = new Task(() => buyerDAL.BulkInsertOrFillId(wds.Select(x => (Customer)x.Buyer).ToList()));
			var insSelTask = new Task(() => sellerDAL.BulkInsertOrFillId(wds.Select(x => (Customer)x.Seller).ToList()));

			insBuyTask.Start();
			insSelTask.Start();

			Task.WaitAll(new Task[] { insBuyTask, insSelTask });

			foreach (var wd in wds)
			{
				if (wd.Seller.Id != 0 && wd.Buyer.Id != 0)
				{
					toInsert.Add(wd);
				}
				else
				{
					withError.Add(wd);
				}
			}

			if (withError.Count > 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Количество WoodDeal с ошибкой : {withError.Count}");
				Console.ResetColor();
			}

			sw.Stop();

			Console.WriteLine($"Окончание проверки Buyers Sellers в БД {wds.Count} эл-ов за {sw.ElapsedMilliseconds}");

			sw.Restart();

			woodDealDAL.BulkInsert(toInsert);

			sw.Stop();

			Console.WriteLine($"Время вставки {toInsert.Count} эл-ов за {sw.ElapsedMilliseconds}");
		}
	}
}