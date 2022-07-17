using System.Collections.Generic;
using TestTaskA2.Entities;

namespace TestTaskA2.DAL.IDAL
{
	public interface ICustomer
	{
		void BulkInsertOrFillId(List<Customer> customer);

		//int GetIdByInn(string inn);
		//void Add(Customer customer);
	}
}
