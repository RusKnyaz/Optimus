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
			DoubleBuffered = true;
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

				var lastEvent = line.Last();
				hasOpenInterval |= lastEvent.EventType == TimeLineEvents.Executing ||
					lastEvent.EventType == TimeLineEvents.Request;
			}
			
			if (endTime == startTime)
				return;

			var g = e.Graphics;
			var scale = size.Width / (endTime - startTime).TotalMilliseconds;
			scale = Math.Max(0.1, scale);
			//draw scales
			var count = (endTime - startTime).TotalMilliseconds/100;
			using(var verLinePen = new Pen(Color.Beige))
			for (int i = 0; i < count*scale*100; i+=(int)(scale*100.0))
			{
				g.DrawLine(verLinePen, i, 0, i, size.Height);
			}
			
			//draw time line
			using (var titleBrush = new SolidBrush(Color.Black))
			using (var durBrush = new SolidBrush(Color.DimGray))
			using (var horLinePen = new Pen(Color.Azure))
			using (var titleFont = new Font(new FontFamily("Arial"), 8))
			using (var receiveBrush = new SolidBrush(Color.CornflowerBlue))
			using (var executeBrush = new SolidBrush(Color.CadetBlue))
			using (var executionFail = new SolidBrush(Color.Brown))
			{
				for (var i = 0; i < intervalLines.Length; i++)
				{
					foreach (var interval in intervalLines[i])
					{
						if (interval.Type != IntervalTypes.Gap)
						{
							DrawBar(g, (float)interval.Start, (float)interval.Duration, scale, i,
								interval.Type == IntervalTypes.Loading
									? receiveBrush
									: interval.Type == IntervalTypes.Executing
										? executeBrush
										: executionFail);
						}

						g.DrawString(interval.Duration + "ms", titleFont, durBrush,
							(float)((interval.Start + interval.Duration / 2) * scale), i * LineHeight);
					}

					g.DrawString(lines[i][0].ResourceId, titleFont, titleBrush, 0, i * LineHeight);
					g.DrawLine(horLinePen, 0, (int)(i + 1) * LineHeight - 1, (int)size.Width, (int)(i + 1) * LineHeight - 1);
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
