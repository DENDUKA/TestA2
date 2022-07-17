using System.ComponentModel.DataAnnotations;

namespace TestTaskA2.Entities
{
	public class Buyer : Customer
	{
		public Buyer(string buyerInn, string buyerName) 
			: base(buyerInn, buyerName)
		{			
		}
	}
}