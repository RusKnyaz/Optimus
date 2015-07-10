using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebBrowser.Tools
{
	class SignleThreadSynchronizationContext : SynchronizationContext
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


/*
	class SignleThreadSynchronizationContext : SynchronizationContext
	{
		private int _threadId;
		private SingleThreadTaskScheduler sched;

		public SignleThreadSynchronizationContext()
		{
			sched = new SingleThreadTaskScheduler();
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			var t = new Task(() => d(state));
			t.Start(sched);
			t.Wait();
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			var t = new Task(() => d(null));
			t.Start(sched);
			t.Wait();
		}
	}

*/

	public class SingleThreadTaskScheduler : TaskScheduler, IDisposable
	{
		[ThreadStatic]
		private static bool _currentThreadIsProcessingItems;
		private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks) 
		Thread _thread;
		private AutoResetEvent _queued = new AutoResetEvent(false);
		private ManualResetEvent _disposed = new ManualResetEvent(false);

		public SingleThreadTaskScheduler()
		{
			_thread = new Thread(Work);
		}

		~SingleThreadTaskScheduler()
		{
			if(_disposed.WaitOne(0))
				_disposed.Set();
		}

		private void Work()
		{
			var waiters = new WaitHandle[] {_queued, _disposed};

			while (!_disposed.WaitOne(0))
			{
				WaitHandle.WaitAny(waiters);
				System.Console.WriteLine("Work ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
				Task[] arr;
				lock (_tasks)
				{
					arr = _tasks.ToArray();
					_queued.Reset();
				}

				foreach (var task in arr)
				{
					TryExecuteTask(task);	
				}
			}
		}

		// Queues a task to the scheduler.  
		protected sealed override void QueueTask(Task task)
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

				if(_thread.ThreadState == ThreadState.Unstarted)
					_thread.Start();
			}
		}

		protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (taskWasPreviouslyQueued)
				if (TryDequeue(task))
					return base.TryExecuteTask(task);
				else
					return false;
			else
				return base.TryExecuteTask(task);
		}

		protected sealed override bool TryDequeue(Task task)
		{
			lock (_tasks) return _tasks.Remove(task);
		}

		protected sealed override IEnumerable<Task> GetScheduledTasks()
		{
			bool lockTaken = false;
			try
			{
				Monitor.TryEnter(_tasks, ref lockTaken);
				if (lockTaken) return _tasks;
				else throw new NotSupportedException();
			}
			finally
			{
				if (lockTaken) Monitor.Exit(_tasks);
			}
		}

		public void Dispose()
		{
			_disposed.Set();
		}
	}

}