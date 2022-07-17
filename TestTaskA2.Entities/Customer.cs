using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskA2.Entities
{
	public abstract class Customer
	{
		public Customer(string buyerInn , string buyerName)
		{
			this.Inn = String.IsNullOrEmpty(buyerInn) ? "Физическое лицо" : buyerInn.Trim();
			this.Name = buyerName is null ? "" : buyerName;
		}
		public int Id { get; set; }
		public string Inn { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return $"{Id} {Inn} {Name}";
		}
	}
}
