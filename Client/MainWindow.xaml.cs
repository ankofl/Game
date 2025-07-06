using Both;
using Client.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client
{
	public partial class MainWindow : Window
	{
		private DispatcherTimer _timer;
		private readonly double _speed = 150.0; // Скорость в пикселях/с
		private double _lastUpdateTime; // Время последнего обновления в секундах
		private MyInput _input; // Экземпляр класса для обработки ввода
		private List<Rectangle> _greenSquares; // Список зеленых квадратов
		private Rectangle _blueSquare; // Синий квадрат (персонаж)
		private Rectangle _backgroundRect; // Фон
		private XY _worldPosition; // Виртуальная позиция персонажа в мире
		private double _fpsCounter; // Счетчик кадров для вычисления FPS
		private double _fpsTimer; // Таймер для накопления времени для FPS

		public MainWindow()
		{
			InitializeComponent();

			// Инициализация ввода
			_input = new MyInput(this);

			// Инициализация списка зеленых квадратов
			_greenSquares = new List<Rectangle>();

			// Инициализация виртуальной позиции
			_worldPosition = new XY(0, 0);

			// Создаем синий квадрат (персонаж)
			_blueSquare = new Rectangle
			{
				Name = "Square",
				Width = 50,
				Height = 50,
				Fill = Brushes.Blue
			};
			UpdateBlueSquarePosition();
			MainCanvas.Children.Add(_blueSquare);

			// Создаем 10 зеленых квадратов
			for (int i = 0; i < 100; i++)
			{
				Rectangle greenSquare = new Rectangle
				{
					Width = 50,
					Height = 50,
					Fill = Brushes.Green
				};
				Canvas.SetLeft(greenSquare, 100 + (i + 1) * 60);
				Canvas.SetTop(greenSquare, 100);
				MainCanvas.Children.Add(greenSquare);
				_greenSquares.Add(greenSquare);
			}

			// Сохраняем ссылку на фон
			_backgroundRect = BackgroundRect;
			Canvas.SetLeft(_backgroundRect, 0);
			Canvas.SetTop(_backgroundRect, 0);

			// Инициализация таймера для максимального FPS (интервал 1 мс)
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1) // Минимальный интервал для максимального FPS
			};
			_timer.Tick += Update;
			_timer.Start();

			// Устанавливаем начальное время
			_lastUpdateTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			_fpsTimer = 0;
			_fpsCounter = 0;
		}

		// Обновление позиции синего квадрата (всегда в центре)
		private void UpdateBlueSquarePosition()
		{
			Canvas.SetLeft(_blueSquare, MainCanvas.ActualWidth / 2 - _blueSquare.Width / 2);
			Canvas.SetTop(_blueSquare, MainCanvas.ActualHeight / 2 - _blueSquare.Height / 2);
		}

		// Обработчик изменения размера окна
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			UpdateBlueSquarePosition(); // Центрируем синий квадрат при изменении размера
		}

		// Обработчик нажатия кнопки "Move Right"
		private void MoveRight_Click(object sender, RoutedEventArgs e)
		{
			// Перемещаем зеленые квадраты и фон влево
			MoveWorld(XY.Right * 10); // Фиксированное перемещение для заметности
		}

		// Метод Update, вызываемый с максимальной частотой
		private void Update(object sender, EventArgs e)
		{
			// Вычисляем DeltaTime
			double currentTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			double deltaTime = currentTime - _lastUpdateTime;
			_lastUpdateTime = currentTime;

			// Обновляем FPS
			_fpsCounter++;
			_fpsTimer += deltaTime;
			if (_fpsTimer >= 1.0) // Обновляем FPS каждую секунду
			{
				FpsText.Text = $"FPS: {(int)(_fpsCounter / _fpsTimer)}";
				_fpsCounter = 0;
				_fpsTimer = 0;
			}

			// Получаем направление из MyInput
			var dir = _input.GetDirection();

			// Перемещаем мир, если есть ненулевое направление
			if (dir != XY.Zero)
			{
				MoveWorld(dir * _speed * deltaTime);
			}
		}

		// Метод для перемещения мира (зеленых квадратов и фона)
		private void MoveWorld(XY move)
		{
			// Обновляем виртуальную позицию персонажа
			_worldPosition += move;

			// Перемещаем зеленые квадраты в противоположном направлении
			foreach (var square in _greenSquares)
			{
				double currentX = Canvas.GetLeft(square);
				double currentY = Canvas.GetTop(square);
				double newX = currentX - move.X; // Противоположное направление
				double newY = currentY + move.Y; // Инверсия Y сохранена

				Canvas.SetLeft(square, newX);
				Canvas.SetTop(square, newY);
			}

			// Перемещаем фон в противоположном направлении
			double bgX = Canvas.GetLeft(_backgroundRect);
			double bgY = Canvas.GetTop(_backgroundRect);
			Canvas.SetLeft(_backgroundRect, bgX - move.X);
			Canvas.SetTop(_backgroundRect, bgY + move.Y);

			// Обновляем текст с виртуальной позицией персонажа
			PositionText.Text = $"Position: ({_worldPosition.X:F2}, {_worldPosition.Y:F2})";
		}
	}
}