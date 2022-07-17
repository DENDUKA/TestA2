using Newtonsoft.Json;

namespace TestTaskA2.Entities
{
	public class ReportWoodDealFromJSON
	{
		[JsonProperty("sellerName")]
		public string sellerName;
		[JsonProperty("sellerInn")]
		public string sellerInn;
		[JsonProperty("buyerName")]
		public string buyerName;
		[JsonProperty("buyerInn")]
		public string buyerInn;

		[JsonProperty("dealDate")]
		public string dealDate;
		[JsonProperty("dealNumber")]
		public string dealNumber;
		[JsonProperty("woodVolumeBuyer")]
		public float woodVolumeBuyer;
		[JsonProperty("woodVolumeSeller")]
		public float woodVolumeSeller;
	}
}