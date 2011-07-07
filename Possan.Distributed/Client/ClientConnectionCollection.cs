using System;
using System.Collections.Generic;
using System.Threading;
using Possan.MapReduce.Util;

namespace Possan.Distributed.Client
{
	public class ClientConnectionCollection : List<ClientConnection>
	{
		public void Add(ClientConfig cfg)
		{
			Add(new ClientConnection(cfg));
		}

		public void StartAll()
		{
			Console.WriteLine("==============================================");
			Console.WriteLine("Starting " + Count + " connections");
			Console.WriteLine("==============================================");
			foreach (var conn in this)
				conn.Start();
			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();

		}

		public void WaitAll()
		{
			Console.WriteLine("==============================================");
			Console.WriteLine("Waiting for " + Count + " connections...");
			Console.WriteLine("==============================================");

			var timer = new Timing("All");
			bool keep_polling = true;

			while (keep_polling)
			{
				// poll all.
				foreach (var conn in this)
					conn.Poll();

				bool any_still_running = false;
				foreach (var conn in this)
				{
					if (conn.State == ClientConnectionState.Started)
					{
						any_still_running = true;
						Console.WriteLine("Job " + conn.JobId + " is still running...");
					}
				}

				if (!any_still_running)
				{
					Console.WriteLine("All jobs seems to have stopped.");
					keep_polling = false;
				}
				else
				{
					Thread.Sleep(1000);
				}
			}

			Console.WriteLine("----------------------------------------------");
			timer.End();

			Console.WriteLine();
		}

		public void StartAllAndWait()
		{
			StartAll();
			WaitAll();
		}

		public void StartAllSynchronous()
		{
			Console.WriteLine("==============================================");
			Console.WriteLine("Running " + Count + " connections sequential");
			Console.WriteLine("==============================================");

			var timer2 = new Timing("All");
			foreach (var conn in this)
			{
				conn.Start();
				var timer = new Timing("One connection");
				Console.WriteLine("----------------------------------------------");
				bool still_running = true;
				while (still_running)
				{
					conn.Poll();
					if (conn.State == ClientConnectionState.Done)
						still_running = false;
					else
						Thread.Sleep(1000);
				}
				timer.End();
			}
			Console.WriteLine("----------------------------------------------");
			timer2.End();
		}
	}
}
