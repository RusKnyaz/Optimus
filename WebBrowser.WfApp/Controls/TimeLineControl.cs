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
			Loading, Executing
		}

		struct Interval
		{
			public double Start;
			public double Duration;
			public IntervalTypes Type;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_timeLine == null || _timeLine.Lines.Count == 0)
				return;

			var lines = _timeLine.Lines;
			var size = ClientSize;

			var intervals = new List<Interval>[lines.Count];

			var defTime = new DateTime();

			var now = DateTime.Now;
			bool hasOpenInterval = false;
			//Calculate scale
			var startTime = lines[0][0].DateTime;
			//var endTime = DateTime.Now;
			var endTime = startTime;
			for (var i = 0; i < intervals.Length; i++)
			{
				intervals[i] = new List<Interval>();
				var line = lines[i];
				var requestTime = line.First(x => x.EventType == TimeLineEvents.Request).DateTime;
				var receivedTime = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Received).DateTime;
				if (receivedTime == defTime)
				{
					receivedTime = now;
					hasOpenInterval = true;
				}

				intervals[i].Add(new Interval
				{
					Start = (requestTime - startTime).TotalMilliseconds,
					Duration = (receivedTime - requestTime).TotalMilliseconds,
					Type = IntervalTypes.Loading
				});

				if (endTime < receivedTime)
					endTime = receivedTime;

				var startExec = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executing).DateTime;
				if (startExec != defTime)
				{
					var endExec = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executed).DateTime;
					if (endExec == defTime)
					{
						endExec = DateTime.Now;
						hasOpenInterval = true;
					}
					intervals[i].Add(new Interval
					{
						Start = (startExec - startTime).TotalMilliseconds,
						Duration = (endExec - startExec).TotalMilliseconds,
						Type = IntervalTypes.Executing
					});

					if (endTime < endExec)
						endTime = endExec;
				}
			}
			
			if (endTime == startTime)
				return;

			var g = e.Graphics;
			var scale = size.Width / (endTime - startTime).TotalMilliseconds;

			using (var titleBrush = new SolidBrush(Color.Black))
			using(var titleFont = new Font(new FontFamily("Arial"), 8))
			using(var receiveBrush = new SolidBrush(Color.CornflowerBlue))
			using(var executeBrush = new SolidBrush(Color.CadetBlue))
			{
				for (var i = 0; i < intervals.Length; i++)
				{
					foreach (var interval in intervals[i])
					{
						DrawBar(g, (float)interval.Start, (float)interval.Duration, scale, i, 
							interval.Type == IntervalTypes.Loading ? receiveBrush : executeBrush);
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
