namespace Possan.Distributed.Sandbox
{
	public interface ISandboxedJob
	{
		string Run(ISandboxedJobArgs args);
	}
}