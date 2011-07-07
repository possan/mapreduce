namespace Possan.Distributed
{
	public interface ISandboxProxy
	{
		// void CallMapper(string mappertype);
		string RunJob(string jobtype, IJobArgs args);
	}
}