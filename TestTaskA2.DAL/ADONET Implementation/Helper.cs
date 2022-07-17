using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskA2.DAL.ADONET_Implementation
{
	internal static class Helper
	{
		public static object GetDBValue(object o)
		{
			return o ?? (object)DBNull.Value;
		}
	}
}
