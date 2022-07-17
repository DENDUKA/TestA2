using System.ComponentModel.DataAnnotations;

namespace TestTaskA2.Entities
{
	public class Seller : Customer
	{
		public Seller(string buyerInn, string buyerName) 
			: base(buyerInn, buyerName)
		{
		}
	}
}