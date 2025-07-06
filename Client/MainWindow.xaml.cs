using Both;
using Client.Utilities;
using System;
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
		private readonly double _speed = 100.0; // Скорость в пикселях/с
		private double _lastUpdateTime; // Время последнего обновления в секундах
		private MyInput _input; // Экземпляр для обработки ввода
		
		private double _fpsCounter; // Счетчик кадров для FPS
		private double _fpsTimer; // Таймер для накопления времени FPS
		private MyGrid _worldGrid;

		public MyPlayer Player;

		public MyRender Render;

		public MainWindow()
		{
			InitializeComponent();

			Width = MyScreen.Size.X;
			Height = MyScreen.Size.Y;

			_timer = UtilsTime.GetTimer();
			_timer.Tick += Update;

			Render = new MyRender(MainCanvas);

			// Инициализация ввода
			_input = new MyInput(this);

			
			// Инициализация мира
			_worldGrid = new MyGrid();

			Player = new MyPlayer();
			MainCanvas.Children.Add(Player.Shape);

			

			// Устанавливаем начальное время
			_lastUpdateTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			_fpsTimer = 0;
			_fpsCounter = 0;
		}

		// Обработчик нажатия кнопки "Выход"
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		// Метод Update, вызываемый ~60 FPS
		private void Update(object sender, EventArgs e)
		{
			// Вычисляем DeltaTime
			double currentTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			double deltaTime = currentTime - _lastUpdateTime;
			_lastUpdateTime = currentTime;

			// Обновляем FPS
			_fpsCounter++;
			_fpsTimer += deltaTime;
			if (_fpsTimer >= 1.0)
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
				Player.Pos += dir * _speed * deltaTime;

				_worldGrid.MoveWorld(Player.Pos);

				PositionText.Text = $"Игрок: {Player.Pos}";
			}
		}
	}
}