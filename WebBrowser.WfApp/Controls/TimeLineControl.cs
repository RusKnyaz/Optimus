using System;
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

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_timeLine.Lines.Count == 0)
				return;

			var lines = _timeLine.Lines;
			var size = ClientSize;

			//Calculate scale
			var startTime = lines[0][0].DateTime;
			//var endTime = DateTime.Now;
			var endTime = startTime;
			for (var i = 0; i < lines.Count; i++)
			{
				for (var j = 0; j < lines[i].Count; j++)
				{
					var pt = lines[i][j];
					if (endTime < pt.DateTime)
						endTime = pt.DateTime;
				}
			}
			
			if (endTime == startTime)
				return;

			var g = e.Graphics;
			var cx = size.Width / (endTime - startTime).TotalMilliseconds;

			var defTime = new DateTime();

			bool hasOpenInterval = false;

			using (var titleBrush = new SolidBrush(Color.Black))
			using(var titleFont = new Font(new FontFamily("Arial"), 8))
			using(var receiveBrush = new SolidBrush(Color.CornflowerBlue))
			using(var executeBrush = new SolidBrush(Color.CadetBlue))
			{
				for (var i = 0; i < lines.Count; i++)
				{
					var line = lines[i].ToArray();
					
					var requestTime = line.First(x => x.EventType == TimeLineEvents.Request).DateTime;
					var receivedTime = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Received).DateTime;
					if (receivedTime == defTime)
					{
						receivedTime = DateTime.Now;
						hasOpenInterval = true;
					}

					DrawBar(g, requestTime, receivedTime, startTime, cx, i, receiveBrush);

					var startExec = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executing).DateTime;
					if (startExec != defTime)
					{
						var endExec = line.FirstOrDefault(x => x.EventType == TimeLineEvents.Executed).DateTime;
						if (endExec == defTime)
						{
							endExec = DateTime.Now;
							hasOpenInterval = true;
						}

						DrawBar(g, startExec, endExec, startTime, cx, i, executeBrush);
					}

					g.DrawString(line[0].ResourceId, titleFont, titleBrush, 0, i * LineHeight);
				}
			}

			if (hasOpenInterval)
			{
				timer1.Start();
			}
			
		}

		private static void DrawBar(Graphics g, DateTime requestTime, DateTime receivedTime, DateTime startTime, double cx, int i, SolidBrush receiveBrush)
		{
			var width = (float) ((receivedTime - requestTime).TotalMilliseconds*cx);

			if (width < 1)
				width = 1;

			var rect = new RectangleF(
				(float) ((requestTime - startTime).TotalMilliseconds*cx),
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
