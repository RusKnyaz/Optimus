using System.Threading;
using System.Threading.Tasks;

namespace WebBrowser.Tools
{
	class SignleThreadSynchronizationContext : SynchronizationContext
	{
		private readonly TaskFactory _taskFactory;

		public SignleThreadSynchronizationContext()
		{
			var sched = new ConcurrentExclusiveSchedulerPair();
			var exclSched = sched.ExclusiveScheduler;
			_taskFactory = new TaskFactory(exclSched);
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			var task = _taskFactory.StartNew(x => d(x), state);
			task.Wait();
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			_taskFactory.StartNew(x => d(x), state);
		}
	}
}