namespace Possan.Distributed.Manager
{
	public enum WorkerState
	{
		Dead,
		Idle,
		Allocated,
		JobCreated,
		JobRunning,
		JobDone,
		Crashed
	}
}