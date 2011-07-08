using System;

namespace Possan.Distributed
{
	public interface ISandboxProxy
	{
		// void CallMapper(string mappertype);
		string RunJob(AppDomain domain, string jobtype, IJobArgs args);
	}
}