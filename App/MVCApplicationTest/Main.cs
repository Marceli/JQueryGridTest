using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCApplicationTest
{
	public static class Program
	{
		public static void Main()
		{
			NHibernateExplanationTestSQLServer tf = new NHibernateExplanationTestSQLServer();
			tf.CreateFactory();
			tf.FuturesTest();
			tf.DisposeFactory();
			Console.ReadKey();
		}
	}
}
