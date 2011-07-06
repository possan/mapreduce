using System;

namespace Possan.Distributed.Worker
{
	class Program
	{
		static void Syntax()
		{
			Console.WriteLine("Syntax: Possan.Distributed.Worker.exe -m [manager-url] {-p [port]} {-t [maxthreads]}");
		}

		static void Main(string[] args)
		{
			Syntax();
		}
	}
}
