using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WebBrowser.WfApp.Controls
{
	public partial class TimeLineControl : UserControl
	{
		private Engine _engine;
		private TimeLineModel _timeLine;

		public TimeLineControl()
		{
			InitializeComponent();
			timer1.Start();
		}

		public Engine Engine
		{
			get { return _engine; }
			set
			{
				if (_timeLine != null)
				{
					_timeLine.Dispose();
				}
				
				_engine = value;

				if (_engine != null)
				{
					_timeLine = new TimeLineModel(_engine);
					_timeLine.Changed += OnTimeLineChanged;
				}
			}
		}

		private void OnTimeLineChanged()
		{
			Invalidate();
		}

		private const int LineHeight = 20;

		enum IntervalTypes
		{
			Undefined,
			Loading, Executing, Gap,
			ExecutionFail
		}

		class Interval
		{
			public long Start;
			public long Duration;
			public IntervalTypes Type;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_timeLine == null || _timeLine.Lines.Count == 0)
				return;

			var lines = _timeLine.Lines;
			var size = ClientSize;

			var intervalLines = new List<Interval>[lines.Count];

			var defTime = new DateTime();

			var now = DateTime.Now;
			bool hasOpenInterval = false;
			//Calculate scale
			var startTime = lines[0][0].DateTime;
			//var endTime = DateTime.Now;
			var endTime = startTime;
			for (var i = 0; i < intervalLines.Length; i++)
			{
				intervalLines[i] = new List<Interval>();
				var intervalsLine = intervalLines[i];
				var line = lines[i].ToArray();

				foreach (var timePoint in line)
				{
					if (timePoint.DateTime > endTime)
						endTime = timePoint.DateTime;

					switch (timePoint.EventType)
					{
						case TimeLineEvents.Request:
							intervalsLine.Add(new Interval
							{
								Start = (long)(timePoint.DateTime - startTime).TotalMilliseconds,
								Duration = (long) (now - timePoint.DateTime).TotalMilliseconds,
								Type = IntervalTypes.Loading
							});
						break;
						case TimeLineEvents.Received:
						{
							var last = intervalsLine.LastOrDefault(x => x.Type == IntervalTypes.Loading);
							if(last != null)
								last.Duration = (long) ((timePoint.DateTime - startTime).TotalMilliseconds - last.Start);
						}
						break;
						case TimeLineEvents.Executing:
						{
							intervalsLine.Add(new Interval
							{
								Start = (long) (timePoint.DateTime - startTime).TotalMilliseconds,
								Duration = (long) (now - timePoint.DateTime).TotalMilliseconds,
								Type = IntervalTypes.Executing
							});
						}
						break;
							case TimeLineEvents.Executed:
							case TimeLineEvents.ExecutionFailed:
						{
							var last = intervalsLine.LastOrDefault(x => x.Type == IntervalTypes.Executing);
							if (last != null)
							{
								last.Duration = (long) ((timePoint.DateTime - startTime).TotalMilliseconds - last.Start);
								last.Type = timePoint.EventType == TimeLineEvents.Executed
									? IntervalTypes.Executing
									: IntervalTypes.ExecutionFail;
							}
						}
						break;
					}
				}


				/*DateTime receivedTime = defTime;
				var requestTime = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Request).DateTime;
				if (requestTime != defTime)
				{
					receivedTime = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Received).DateTime;
					if (receivedTime == defTime)
					{
						receivedTime = now;
						hasOpenInterval = true;
					}

					intervalLines[i].Add(new Interval
					{
						Start = (long) (requestTime - startTime).TotalMilliseconds,
						Duration = (long) (receivedTime - requestTime).TotalMilliseconds,
						Type = IntervalTypes.Loading
					});
					if (endTime < receivedTime)
						endTime = receivedTime;
				}
				
				var startExec = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executing).DateTime;
				if (startExec != defTime)
				{
					if (receivedTime != defTime)
					{
						intervalLines[i].Add(new Interval()
						{
							Start = (long) (receivedTime - startTime).TotalMilliseconds,
							Duration = (long) (startExec - receivedTime).TotalMilliseconds,
							Type = IntervalTypes.Gap
						});
					}
				}

				if (startExec != defTime)
				{
					var endExecEvent = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executed
						|| x.EventType == TimeLineEvents.ExecutionFailed);

					var endExec = endExecEvent.DateTime;
					if (endExec == defTime)
					{
						endExec = DateTime.Now;
						hasOpenInterval = true;
					}
					intervalLines[i].Add(new Interval
					{
						Start = (long) (startExec - startTime).TotalMilliseconds,
						Duration = (long) (endExec - startExec).TotalMilliseconds,
						Type = endExecEvent.EventType == TimeLineEvents.ExecutionFailed ? 
						IntervalTypes.ExecutionFail : 
						IntervalTypes.Executing
					});

					if (endTime < endExec)
						endTime = endExec;


				}*/
			}
			
			if (endTime == startTime)
				return;

			var g = e.Graphics;
			var scale = size.Width / (endTime - startTime).TotalMilliseconds;

			using (var titleBrush = new SolidBrush(Color.Black))
			using(var durBrush = new SolidBrush(Color.DimGray))
			using(var titleFont = new Font(new FontFamily("Arial"), 8))
			using(var receiveBrush = new SolidBrush(Color.CornflowerBlue))
			using(var executeBrush = new SolidBrush(Color.CadetBlue))
			using(var executionFail = new SolidBrush(Color.Brown))
			{
				for (var i = 0; i < intervalLines.Length; i++)
				{
					foreach (var interval in intervalLines[i])
					{
						if (interval.Type != IntervalTypes.Gap)
						{
							DrawBar(g, (float) interval.Start, (float) interval.Duration, scale, i,
								interval.Type == IntervalTypes.Loading ? receiveBrush 
								: interval.Type == IntervalTypes.Executing ? executeBrush 
								: executionFail);
						}

						g.DrawString(interval.Duration+"ms", titleFont, durBrush, 
							(float) ((interval.Start + interval.Duration/2)*scale), i * LineHeight);
					}

					g.DrawString(lines[i][0].ResourceId, titleFont, titleBrush, 0, i * LineHeight);
				}
			}

			if (hasOpenInterval)
			{
				timer1.Start();
			}
			else
			{
				timer1.Stop();
			}
		}

		private static void DrawBar(Graphics g, float startTime, float duration, double cx, int i, SolidBrush receiveBrush)
		{
			var width = (float) (duration*cx);

			if (width < 1)
				width = 1;

			var rect = new RectangleF(
				(float) (startTime*cx),
				(float) i*LineHeight,
				width,
				(float) LineHeight - 1);

			g.FillRectangle(receiveBrush, rect);
		}

		private void OnTick(object sender, EventArgs e)
		{
			Invalidate();
		}
	}
}
