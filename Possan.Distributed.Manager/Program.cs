using System;

namespace Possan.Distributed.Manager
{
	class Program
	{
		static void Syntax()
		{
			Console.WriteLine("Syntax: Possan.Distributed.Manager.exe {-h [hostname]} {-p [port]}");
		}

		static void Main(string[] args)
		{
			Syntax();
		}
	}
}
