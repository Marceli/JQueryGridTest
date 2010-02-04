using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using Core.Data;

namespace MVCApplicationTest
{
	[TestFixture]
	public class SqlConnectionTest
	{
		[Test]
		public void CanOpenConnection()
		{
			Console.WriteLine(SessionProvider.MSSQLConnectionString);
			using (var conn = new SqlConnection(SessionProvider.MSSQLConnectionString))
			{
				conn.Open();
			}
		}
	}
}
