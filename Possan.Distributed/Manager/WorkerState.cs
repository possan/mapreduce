namespace Possan.Distributed.Manager
{
	enum WorkerState
	{
		Dead,
		Idle,
		Allocated,
		JobCreated,
		JobRunning,
		JobDone
	}
}