using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Both
{
	public static class UtilsTime
	{
		public static DispatcherTimer GetTimer()
		{
			// Инициализация таймера для ~60 FPS
			var _timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)
			};
			_timer.Start();

			return _timer;
		}
	}
}
