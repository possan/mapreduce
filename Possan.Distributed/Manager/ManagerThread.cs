using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Possan.MapReduce.Net.Manager
{
	class ManagerThread
	{
		public ManualResetEvent KillSignal;
		public Thread Thread;
		public TcpListener TcpListener;

		void GotConnection(IAsyncResult res)
		{
			// res.
			Socket s = null;
			try
			{
				s = TcpListener.EndAcceptSocket(res);
			}
			catch(Exception z)
			{
				
			}

			TcpListener.BeginAcceptSocket(GotConnection, null);

			if (s == null)
				return;
			try
			{
				Console.WriteLine("got async connection " + s.RemoteEndPoint + ", starting listener again.");
				HandleRequest(s);
			}
			catch (Exception e)
			{

			}
		}
		 
		void HandleRequest(Socket sock)
		{
			sock.NoDelay = true;
			// sock.ReceiveBufferSize = 1;
			// sock.SendBufferSize = 1;
			
			// var n = 1;
			var requestheader = new StringBuilder();
			var buffer = new byte[100];
			while (sock.Available > 0)
			{
				int n = sock.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				var part = Encoding.ASCII.GetString(buffer, 0, n);
				// Console.WriteLine("Got part request: " + part);
				// Console.WriteLine("n="+n);
				requestheader.Append(part);
			}

			Console.WriteLine("Got request: " + requestheader);

			string s = "HTTP/1.1 200 OK\n\rContent-Type: text/plain\n\r\n\r";
			var hb = Encoding.ASCII.GetBytes(s);
			SendFlush(sock, hb);

			// Thread.Sleep(1000);
			SendFlush(sock, "Hej efter ett tag 1!" );
			Thread.Sleep(1000);

			SendFlush(sock, "Hej efter ett tag 2!");
			Thread.Sleep(1000);

			SendFlush(sock,  "Hej efter ett tag 3!");
			
			Console.WriteLine("closing socket" + sock.RemoteEndPoint);
			sock.Shutdown(SocketShutdown.Both);
			sock.Close();
		}

		public void Run()
		{
			Console.WriteLine("ManagerThread: Started.");
			TcpListener.BeginAcceptSocket(GotConnection, null);
			KillSignal.WaitOne();
			Console.WriteLine("ManagerThread: Stopped.");
		}
	}
}