using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HttpServer;
using HttpServer.HttpModules;
using HttpServer.Sessions;

namespace Possan.Distributed.Worker
{
	class WorkerRequestHandler : HttpModule
	{
		private readonly Worker _worker;

		public WorkerRequestHandler(Worker worker)
		{
			_worker = worker;
		}

		public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
		{
			if (request.UriPath == "/createjob")
			{
				var reader = new StreamReader(request.Body);
				string posttext = reader.ReadToEnd();
				var jobid = _worker.JobController.CreateJob(posttext);
				Console.WriteLine("Worker: Created job #" + jobid + ": Posted text: " + posttext);
				response.Encoding = Encoding.UTF8;
				response.Body = new MemoryStream(response.Encoding.GetBytes(jobid));
				return true;
			}

			var m1 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/assemblies");
			if (m1.Success)
			{
				var jobid = m1.Groups[1].Value;
				var bytes = Utilities.ReadAllBytes(request.Body);
				_worker.JobController.AddAssembly(jobid,bytes);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			var m2 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/start");
			if (m2.Success)
			{
				var jobid = m2.Groups[1].Value;
				var reader = new StreamReader(request.Body);
				_worker.JobController.Start(jobid);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			Console.WriteLine("Unhandled worker request: " + request.UriPath);

			/*

			Console.WriteLine("Worker request: "+request.UriPath);
			response.Encoding = Encoding.UTF8;
			string body = "Hej från worker; localpath=" + request.UriPath + ", querystring=" + request.QueryString+", method="+request.Method;
			response.Body = new MemoryStream(response.Encoding.GetBytes(body));
			return true;*/
			return false;
		}
	}
}