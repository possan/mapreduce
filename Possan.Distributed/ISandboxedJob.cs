namespace Possan.Distributed
{
	public interface ISandboxedJob
	{
		string Run(string[] args);
	}
}