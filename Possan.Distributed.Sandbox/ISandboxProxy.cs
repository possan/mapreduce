using System;

namespace Possan.Distributed.Sandbox
{
	public interface ISandboxProxy
	{
		// void CallMapper(string mappertype);
		string RunJob(AppDomain domain, string jobtype, ISandboxedJobArgs args);
	}
}