using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Knyaz.Optimus.WfApp.Controls
{
	public partial class TimeLineControl : UserControl
	{
		private Engine _engine;
		private TimeLineModel _timeLine;
		private double _scale;

		public TimeLineControl()
		{
			InitializeComponent();
			timer1.Start();
		}

		void OnListDrawItem(object sender, DrawListViewItemEventArgs e)
		{
			if ((e.State & ListViewItemStates.Selected) != 0)
			{
				using (var bg = new SolidBrush(Color.CornflowerBlue))
				{
					e.Graphics.FillRectangle(bg, e.Bounds);
				}
			}
			else
			{
				e.DrawBackground();
			}
			e.DrawText();
		}

		void OnListDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			var line = e.SubItem.Tag as List<Interval>;
			if (line == null || line.Count == 0)
				return;
			var g = e.Graphics;

			//draw time line
			using (var durBrush = new SolidBrush(Color.DimGray))
			using (var titleFont = new Font(new FontFamily("Arial"), 8))
			using (var receiveBrush = new SolidBrush(Color.CornflowerBlue))
			using (var executeBrush = new SolidBrush(Color.CadetBlue))
			using (var executionFail = new SolidBrush(Color.Brown))
			{
				foreach (var interval in line)
				{
					if (interval.Type != IntervalTypes.Gap)
					{
						var width = (float) (interval.Duration* _scale);
						if (width < 1)
							width = 1;

						var rect = new RectangleF(
							(float) (interval.Start*_scale) + e.Bounds.Left,
							e.Bounds.Top,
							width,
							e.Bounds.Height);

						g.FillRectangle(interval.Type == IntervalTypes.Loading
							? receiveBrush
							: interval.Type == IntervalTypes.Executing
								? executeBrush
								: executionFail, rect);
					}

					g.DrawString(interval.Duration + "ms", titleFont, durBrush,
						(float)((interval.Start + interval.Duration / 2) * _scale) + e.Bounds.Left, e.Bounds.Top);
				}
			}
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

		enum IntervalTypes
		{
			Loading, Executing, Gap, ExecutionFail
		}

		class Interval
		{
			public long Start;
			public long Duration;
			public IntervalTypes Type;
		}

		private void OnTick(object sender, EventArgs e)
		{
			InvalidateLines();
		}

		private void InvalidateLines()
		{
			if (_timeLine == null || _timeLine.Lines.Count == 0)
				return;

			var lines = _timeLine.Lines;
			var now = DateTime.Now;
			//Calculate _scale
			var startTime = lines[0][0].DateTime;
			var endTime = startTime;

			//avoid to use foreach due to multithread collection access
			for (var i = 0; i < lines.Count; i++)
			{
				var line = lines[i];

				var intervalsLine = new List<Interval>();

				var item = _listView.Items.Count > i ? _listView.Items[i] : null;
				if (item == null)
				{
					item = new ListViewItem(line[0].ResourceId);
					var subItem = new ListViewItem.ListViewSubItem(item, "ASD");
					item.SubItems.Add(subItem);

					_listView.Items.Add(item);
					subItem.Tag = intervalsLine;
				}

				for (var timePointIndex = 0; timePointIndex < line.Count; timePointIndex++)
				{
					var timePoint = line[timePointIndex];

					if (timePoint.DateTime > endTime)
						endTime = timePoint.DateTime;

					switch (timePoint.EventType)
					{
						case TimeLineEvents.Request:
							intervalsLine.Add(new Interval
							{
								Start = (long) (timePoint.DateTime - startTime).TotalMilliseconds,
								Duration = (long) (now - timePoint.DateTime).TotalMilliseconds,
								Type = IntervalTypes.Loading
							});
							break;
						case TimeLineEvents.Received:
						{
							var last = intervalsLine.LastOrDefault(x => x.Type == IntervalTypes.Loading);
							if (last != null)
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
			}

			if (endTime == startTime)
				return;

			_scale = _listView.Columns[1].Width / (endTime - startTime).TotalMilliseconds;
			_scale = Math.Max(0.1, _scale);

			Invalidate();
		}

		private void TimeLineControl_SizeChanged(object sender, EventArgs e)
		{
			_listView.Columns[1].Width = ClientSize.Width - _listView.Columns[0].Width - SystemInformation.VerticalScrollBarWidth;
		}
	}
}
