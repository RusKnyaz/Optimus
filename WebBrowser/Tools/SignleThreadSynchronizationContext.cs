using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebBrowser.Tools
{
	class SignleThreadSynchronizationContext : SynchronizationContext, IDisposable
	{
		public override void Send(SendOrPostCallback d, object state)
		{
			Action a = () => d(state);

			//avoid deadlocks;
			if (_thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
			{
				TryExecuteTask(a);
				return;
			}

			var completeSignal = new ManualResetEvent(false);

			QueueTask(() =>
			{
				TryExecuteTask(a);
				completeSignal.Set();
			});
			completeSignal.WaitOne();
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			Action a = () => d(state);

			//avoid deadlocks;
			if (_thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
			{
				TryExecuteTask(a);
				return;
			}

			QueueTask(a);
		}

		protected void QueueTask(Action task)
		{
			if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
			{
				TryExecuteTask(task);
				return;
			}

			lock (_tasks)
			{
				_tasks.AddLast(task);
				_queued.Set();
			}
		}

		private void TryExecuteTask(Action task)
		{
			try
			{
				task();
			}
			catch
			{
				//todo: handle error
			}
		}

		private readonly LinkedList<Action> _tasks = new LinkedList<Action>(); // protected by lock(_tasks) 
		Thread _thread;
		private AutoResetEvent _queued = new AutoResetEvent(false);
		private ManualResetEvent _disposed = new ManualResetEvent(false);

		public SignleThreadSynchronizationContext()
		{
			_thread = new Thread(Work);
			_thread.Start();
		}

		~SignleThreadSynchronizationContext()
		{
			if(!_disposed.WaitOne(0))
				_disposed.Set();
		}

		public void Dispose()
		{
			if (!_disposed.WaitOne(0))
				_disposed.Set();
		}

		private void Work()
		{
			var waiters = new WaitHandle[] { _queued, _disposed };

			while (!_disposed.WaitOne(0))
			{
				WaitHandle.WaitAny(waiters);
				Action[] arr;
				lock (_tasks)
				{
					arr = _tasks.ToArray();
					_tasks.Clear();
					_queued.Reset();
				}

				foreach (var task in arr)
				{
					TryExecuteTask(task);
				}
			}
		}
	}
}