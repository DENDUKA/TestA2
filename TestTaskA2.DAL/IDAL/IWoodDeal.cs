using System.Collections.Generic;
using TestTaskA2.Entities;

namespace TestTaskA2.DAL.IDAL
{
	public interface IWoodDeal
	{
		void BulkInsert(List<WoodDeal> deals);

		//bool IsExistByDealNumber(string dealNumber);
		//void Add(WoodDeal deal);
	}
}
