using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTester.service1;

namespace ConsoleTester
{
	class Program
	{
		static void Main(string[] args)
		{
			service1.Service1 client = new Service1Client();


			//var login = client.Login("test@mrbill.se","123456",false);
			var test = client.DoWork();
			Console.WriteLine(test);
			Console.ReadLine();
			

		}
	}
}
