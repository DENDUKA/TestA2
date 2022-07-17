using System;

namespace TestTaskA2.Entities
{
	public class WoodDeal
	{
		public WoodDeal(ReportWoodDealFromJSON rwf)
		{
			if (DateTime.TryParse(rwf.dealDate, out DateTime dealDate))
			{
				DealDate = dealDate;
			}

			DealNumber = rwf.dealNumber;
			WoodVolumeBuyer = rwf.woodVolumeBuyer;
			WoodVolumeSeller = rwf.woodVolumeSeller;

			Buyer = new Buyer(rwf.buyerInn, rwf.buyerName);
			Seller = new Seller(rwf.sellerInn, rwf.sellerName);
		}
		public int Id { get; set; }
		public DateTime DealDate { get; set; }
		public string DealNumber { get; set; }
		public float WoodVolumeBuyer { get; set; }
		public float WoodVolumeSeller { get; set; }

		public Buyer Buyer { get; set; }
		public Seller Seller { get; set; }
	}
}
