using System;

namespace Desk.com
{
	internal class Demo
	{
		private static void Main()
		{
			var userData = new Multipass.UserData();
			userData.uid = "123abc";
			userData.expires = DateTime.Now.AddMinutes(2).ToString("yyyy-MM-ddTHH:mm:sszzz");
			userData.customer_email = "testuser@yoursite.com";
			userData.customer_name = "Aiman Zidan";

			var theLink = Multipass.BuildLink(userData);
			Console.WriteLine(theLink);
		}
	}
}