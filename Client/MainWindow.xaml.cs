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
		private Rectangle _player; // Игрок (черный квадрат)
		private XY playerPos; // Виртуальная позиция игрока в мире
		private double _fpsCounter; // Счетчик кадров для FPS
		private double _fpsTimer; // Таймер для накопления времени FPS
		private MyGrid _worldGrid; // Сетка мира
		private readonly double _blockSize = 50; // Размер блока в пикселях
		private Dictionary<MyBlock, Rectangle> _blockRectangles; // Отображение блоков на прямоугольники
		private XY _lastPlayerPos; // Последняя позиция для отслеживания изменений
		private bool _needsChunkUpdate; // Флаг для обновления чанков
		private XY _lastRenderedPos; // Последняя позиция для обновления текста

		public MainWindow()
		{
			InitializeComponent();

			// Инициализация ввода
			_input = new MyInput(this);

			// Инициализация виртуальной позиции
			playerPos = new XY(0, 0);
			_lastPlayerPos = new XY(0, 0);
			_lastRenderedPos = new XY(0, 0);

			// Инициализация мира
			_worldGrid = new MyGrid();
			_blockRectangles = new Dictionary<MyBlock, Rectangle>();
			MyGridUtils.Initialize(_worldGrid, _blockRectangles, MainCanvas, _blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight);

			// Создаем игрока (черный квадрат)
			_player = new Rectangle
			{
				Name = "Square",
				Width = 15,
				Height = 15,
				Fill = Brushes.Black
			};
			UpdatePlayerPosition();
			MainCanvas.Children.Add(_player);

			// Инициализация таймера для ~60 FPS
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(16)
			};
			_timer.Tick += Update;
			_timer.Start();

			// Устанавливаем начальное время
			_lastUpdateTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			_fpsTimer = 0;
			_fpsCounter = 0;

			// Инициализация текста чанков
			_needsChunkUpdate = true;
			UpdateChunksText();
			PositionText.Text = $"Игрок: {playerPos}";
		}

		// Обновление текста количества чанков
		private void UpdateChunksText()
		{
			int totalChunks = MyGrid.GridSize * MyGrid.GridSize;
			int loadedChunks = 0;

			var (minChunkX, maxChunkX, minChunkY, maxChunkY) = MyGridUtils.GetVisibleChunkRange(_blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight);
			for (int chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
			{
				for (int chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
				{
					if (chunkX >= 0 && chunkX < MyGrid.GridSize && chunkY >= 0 && chunkY < MyGrid.GridSize &&
						MyGridUtils.IsChunkVisible(chunkX, chunkY, _blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight))
					{
						loadedChunks++;
					}
				}
			}

			ChunksText.Text = $"Чанки: {loadedChunks}/{totalChunks}";
			_needsChunkUpdate = false;
		}

		// Проверка, виден ли чанк
		private bool IsChunkVisible(int chunkX, int chunkY)
		{
			return MyGridUtils.IsChunkVisible(chunkX, chunkY, _blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight);
		}

		// Обновление позиции игрока (всегда в центре)
		private void UpdatePlayerPosition()
		{
			Canvas.SetLeft(_player, MainCanvas.ActualWidth / 2 - _player.Width / 2);
			Canvas.SetTop(_player, MainCanvas.ActualHeight / 2 - _player.Height / 2);
		}

		// Обработчик изменения размера окна
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			UpdatePlayerPosition();
			MyGridUtils.UpdateVisibleBlocks(_worldGrid, _blockRectangles, MainCanvas, _blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight);
			_needsChunkUpdate = true;
			UpdateChunksText();
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
				if (_needsChunkUpdate)
				{
					UpdateChunksText();
				}
			}

			// Получаем направление из MyInput
			var dir = _input.GetDirection();

			// Перемещаем мир, если есть ненулевое направление
			if (dir != XY.Zero)
			{
				MoveWorld(dir * _speed * deltaTime);
			}
		}

		// Метод для перемещения мира (блоков)
		private void MoveWorld(XY move)
		{
			// Обновляем виртуальную позицию игрока
			playerPos += move;

			// Ограничиваем позицию игрока
			double worldWidth = MyGrid.GridSize * MyChunk.ChunkSize * _blockSize;
			double worldHeight = worldWidth;
			playerPos.X = Math.Max(0, Math.Min(playerPos.X, worldWidth));
			playerPos.Y = Math.Max(0, Math.Min(playerPos.Y, worldHeight));

			// Обновляем координаты видимых блоков
			foreach (var pair in _blockRectangles)
			{
				MyBlock block = pair.Key;
				Rectangle rect = pair.Value;
				double screenX = block.WorldX - playerPos.X + MainCanvas.ActualWidth / 2;
				double screenY = block.WorldY + playerPos.Y + MainCanvas.ActualHeight / 2;
				Canvas.SetLeft(rect, screenX);
				Canvas.SetTop(rect, screenY);
			}

			// Проверяем, изменилась ли позиция достаточно для обновления чанков
			double deltaX = Math.Abs(playerPos.X - _lastPlayerPos.X);
			double deltaY = Math.Abs(playerPos.Y - _lastPlayerPos.Y);
			double chunkSize = MyChunk.ChunkSize * _blockSize;
			if (deltaX >= chunkSize || deltaY >= chunkSize)
			{
				MyGridUtils.UpdateVisibleBlocks(_worldGrid, _blockRectangles, MainCanvas, _blockSize, playerPos, MainCanvas.ActualWidth, MainCanvas.ActualHeight);
				_needsChunkUpdate = true;
				_lastPlayerPos = new XY(playerPos.X, playerPos.Y);
			}

			// Обновляем текст позиции игрока с порогом для оптимизации
			if (Math.Abs(playerPos.X - _lastRenderedPos.X) > 0.1 || Math.Abs(playerPos.Y - _lastRenderedPos.Y) > 0.1)
			{
				PositionText.Text = $"Игрок: {playerPos}";
				_lastRenderedPos = new XY(playerPos.X, playerPos.Y);
			}
		}
	}
}