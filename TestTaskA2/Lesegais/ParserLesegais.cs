using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Timers;
using TestTaskA2.Entities;
using TestTaskA2.Lesegais;

namespace TestTaskA2
{
	internal class ParserLesegais
	{
		private string domain = @"https://www.lesegais.ru";
		private string resourcePoint = @"/open-area/graphql";
		private bool inProgress = false;
		private int itemsByPage;
		private int allRows = 0;

		Stopwatch sw = new Stopwatch();
		Stopwatch onTimeSW = new Stopwatch();

		public ParserLesegais(int itemsByPage = 15000)
		{
			this.itemsByPage = itemsByPage;
		}

		public void Start()
		{
			Parse();
			var timer = new Timer(600000);			
			timer.Elapsed += Timer_Elapsed;
			timer.Enabled = true;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Parse();
		}

		private void Parse()
		{
			onTimeSW.Restart();
			foreach (var res in GetAllFromPage())
			{
				Console.WriteLine($"{res.Count} прочитано");

				SaveToBD(res);
			}

			onTimeSW.Stop();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Один полный проход за {onTimeSW.ElapsedMilliseconds}. При {itemsByPage} на странице.");
			Console.ResetColor();
		}

		private void SaveToBD(List<ReportWoodDealFromJSON> res)
		{
			sw.Restart();

			var deals = res.Select(x => new WoodDeal(x)).ToList();

			sw.Stop();

			Console.WriteLine($"Конвертация : {sw.ElapsedMilliseconds}");

			sw.Restart();

			BDLogic.AddWoodDealBatch(deals);

			sw.Stop();

			Console.WriteLine($"Сохранение в БД завершено за {sw.ElapsedMilliseconds}. Элементов : {res.Count}");

			allRows += res.Count;

			Console.WriteLine($"Всего обработано {allRows} строк.");

			Console.WriteLine("_______________");
		}

		/// <summary>
		/// Считывание данных с сервера
		/// </summary>
		private IEnumerable<List<ReportWoodDealFromJSON>> GetAllFromPage()
		{
			var client = new RestClient(domain);

			var request = new RestRequest(resourcePoint);

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "*/*");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");

			inProgress = true;
			int page = 0;

			while (inProgress)
			{
				Console.WriteLine("Посылка запроса");

				sw.Restart();

				//Удаление предыдущего JsonBody
				request.Parameters.RemoveParameter("");
				request.AddJsonBody(GetRequestBody(itemsByPage, page));

				var response = client.Post(request);

				//TODO
				if (response.StatusCode != HttpStatusCode.OK)
				{
					inProgress = false;
					yield break;
				}

				Console.WriteLine("Ответ от сервера получен");

				var content = response.Content;

				var data = (JObject)JsonConvert.DeserializeObject(content);
				content = data["data"]["searchReportWoodDeal"]["content"].ToString();

				var result = JsonConvert.DeserializeObject<List<ReportWoodDealFromJSON>>(content);

				sw.Stop();

				Console.WriteLine($"Данные {result.Count} получены за {sw.ElapsedMilliseconds}");

				if (result.Count == 0)
				{
					inProgress = false;
					yield break;
				}

				page++;

				yield return result;
			}

			yield break;
		}

		private string GetRequestBody(int count, int pageNumber)
		{
			return $"{{\"query\":\"query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {{\\n  searchReportWoodDeal(filter: $filter, pageable: {{number: $number, size: $size}}, orders: $orders) {{\\n    content {{\\n      sellerName\\n      sellerInn\\n      buyerName\\n      buyerInn\\n      woodVolumeBuyer\\n      woodVolumeSeller\\n      dealDate\\n      dealNumber\\n      __typename\\n    }}\\n    __typename\\n  }}\\n}}\\n\",\"variables\":{{\"size\":{count},\"number\":{pageNumber},\"filter\":null,\"orders\":[{{\"property\":\"dealDate\",\"direction\":\"DESC\"}}]}},\"operationName\":\"SearchReportWoodDeal\"}}";
		}
	}
}