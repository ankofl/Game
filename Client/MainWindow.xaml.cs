using Both;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Client
{
	public partial class MainWindow : Window
	{
		private DispatcherTimer _timer;
		private bool _isWPressed, _isAPressed, _isSPressed, _isDPressed;
		private readonly double _speed = 150.0; // Скорость в пикселях/с
		private double _lastUpdateTime; // Время последнего обновления в секундах

		public MainWindow()
		{
			InitializeComponent();

			// Инициализация таймера для 30 FPS (1000 мс / 30 ≈ 33.33 мс)
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1000.0 / 30.0)
			};
			_timer.Tick += Update;
			_timer.Start();

			// Устанавливаем начальное время
			_lastUpdateTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;

			// Подключаем обработчики событий клавиатуры
			KeyDown += MainWindow_KeyDown;
			KeyUp += MainWindow_KeyUp;
		}

		// Обработчик нажатия кнопки "Move Right"
		private void MoveRight_Click(object sender, RoutedEventArgs e)
		{
			MoveSquare(XY.Right * _speed * (1000.0 / 30.0 / 1000.0)); // Перемещение вправо за один кадр
		}

		// Обработчик нажатия клавиш
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			// Отмечаем, какие клавиши нажаты
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
		private void MainWindow_KeyUp(object sender, KeyEventArgs e)
		{
			// Отмечаем, какие клавиши отпущены
			if (e.Key == Key.A)
				_isAPressed = false;
			if (e.Key == Key.D)
				_isDPressed = false;
			if (e.Key == Key.W)
				_isWPressed = false;
			if (e.Key == Key.S)
				_isSPressed = false;
		}

		// Метод Update, вызываемый 30 раз в секунду
		private void Update(object sender, System.EventArgs e)
		{
			// Вычисляем DeltaTime
			double currentTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			double deltaTime = currentTime - _lastUpdateTime;
			_lastUpdateTime = currentTime;

			var dir = XY.Zero;

			// Формируем вектор направления на основе нажатых клавиш
			if (_isAPressed)
				dir.X -= 1;
			if (_isDPressed)
				dir.X += 1;
			if (_isWPressed)
				dir.Y += 1; // W увеличивает Y (движение вверх из-за инверсии)
			if (_isSPressed)
				dir.Y -= 1; // S уменьшает Y (движение вниз из-за инверсии)

			// Нормализуем направление, чтобы скорость была одинаковой при движении по диагонали
			if (dir != XY.Zero)
			{
				dir = dir.Normalize();
				MoveSquare(dir * _speed * deltaTime);
			}
		}

		// Метод для перемещения квадрата
		private void MoveSquare(XY move)
		{
			// Получаем текущие координаты
			double currentX = Canvas.GetLeft(Square);
			double currentY = Canvas.GetTop(Square);

			// Вычисляем новые координаты, инвертируя Y
			double newX = currentX + move.X;
			double newY = currentY - move.Y; // Инверсия: вычитаем move.Y

			// Проверяем, чтобы квадрат не выходил за пределы Canvas
			newX = Math.Max(0, Math.Min(newX, MainCanvas.ActualWidth - Square.Width));
			newY = Math.Max(0, Math.Min(newY, MainCanvas.ActualHeight - Square.Height));

			// Устанавливаем новые координаты
			Canvas.SetLeft(Square, newX);
			Canvas.SetTop(Square, newY);

			// Обновляем текст с координатами
			PositionText.Text = $"Position: ({newX}, {newY})";
		}
	}
}