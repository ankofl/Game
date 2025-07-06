using Both;
using System.Windows;
using System.Windows.Input;

namespace Client.Utilities
{
	internal class MyInput
	{
		private bool _isWPressed, _isAPressed, _isSPressed, _isDPressed;

		// Конструктор, подключающий обработчики событий
		public MyInput(Window window)
		{
			window.KeyDown += KeyDownHandler;
			window.KeyUp += KeyUpHandler;
		}

		// Обработчик нажатия клавиш
		private void KeyDownHandler(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.A)
				_isAPressed = true;
			if (e.Key == Key.D)
				_isDPressed = true;
			if (e.Key == Key.W)
				_isWPressed = true;
			if (e.Key == Key.S)
				_isSPressed = true;
		}

		// Обработчик отпускания клавиш
		private void KeyUpHandler(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.A)
				_isAPressed = false;
			if (e.Key == Key.D)
				_isDPressed = false;
			if (e.Key == Key.W)
				_isWPressed = false;
			if (e.Key == Key.S)
				_isSPressed = false;
		}

		// Метод для получения вектора направления
		public XY GetDirection()
		{
			var dir = XY.Zero;

			if (_isAPressed)
				dir.X -= 1;
			if (_isDPressed)
				dir.X += 1;
			if (_isWPressed)
				dir.Y += 1; // W увеличивает Y (движение вверх из-за инверсии)
			if (_isSPressed)
				dir.Y -= 1; // S уменьшает Y (движение вниз из-за инверсии)

			// Нормализуем направление, чтобы скорость была одинаковой при движении по диагонали
			return dir != XY.Zero ? dir.Normalize() : dir;
		}
	}
}