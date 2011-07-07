namespace Possan.Distributed
{
	public interface ISandboxedJob
	{
		string Run(IJobArgs args);
	}
}