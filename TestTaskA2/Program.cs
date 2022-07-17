using System;

namespace TestTaskA2
{
	internal class Program
	{
		static void Main(string[] args)
		{
			ParserLesegais p = new ParserLesegais(50000);
			p.Start();

			Console.ReadLine();
		}
	}
}
