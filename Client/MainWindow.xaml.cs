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

		public MainWindow()
		{
			InitializeComponent();

			// Инициализация ввода
			_input = new MyInput(this);

			// Инициализация виртуальной позиции
			playerPos = new XY(0, 0);

			// Инициализация мира
			_worldGrid = new MyGrid();
			_blockRectangles = new Dictionary<MyBlock, Rectangle>();
			InitializeWorld();

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

			// Инициализация таймера для максимального FPS
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1) // Минимальный интервал
			};
			_timer.Tick += Update;
			_timer.Start();

			// Устанавливаем начальное время
			_lastUpdateTime = DateTime.Now.Ticks / (double)TimeSpan.TicksPerSecond;
			_fpsTimer = 0;
			_fpsCounter = 0;

			// Инициализация текста чанков
			UpdateChunksText();
		}

		// Инициализация мира
		private void InitializeWorld()
		{
			Random rand = new Random();
			for (int chunkX = 0; chunkX < MyGrid.GridSize; chunkX++)
			{
				for (int chunkY = 0; chunkY < MyGrid.GridSize; chunkY++)
				{
					MyChunk chunk = new MyChunk(); // Конструктор инициализирует Blocks
					for (int blockX = 0; blockX < MyChunk.ChunkSize; blockX++)
					{
						for (int blockY = 0; blockY < MyChunk.ChunkSize; blockY++)
						{
							BlockType type = (BlockType)rand.Next(Enum.GetValues(typeof(BlockType)).Length);
							chunk.Blocks[blockX + blockY * MyChunk.ChunkSize] = new MyBlock { Type = type };

							Rectangle blockRect = new Rectangle
							{
								Width = _blockSize,
								Height = _blockSize,
								Fill = GetBlockBrush(type)
							};
							double worldX = (chunkX * MyChunk.ChunkSize + blockX) * _blockSize;
							double worldY = (chunkY * MyChunk.ChunkSize + blockY) * _blockSize;
							Canvas.SetLeft(blockRect, worldX);
							Canvas.SetTop(blockRect, worldY);
							MainCanvas.Children.Add(blockRect);
							_blockRectangles[chunk.Blocks[blockX + blockY * MyChunk.ChunkSize]] = blockRect;
						}
					}
					_worldGrid.Chunks[chunkX + chunkY * MyGrid.GridSize] = chunk;
				}
			}
		}

		// Получение кисти для типа блока
		private static SolidColorBrush GetBlockBrush(BlockType type)
		{
			return type switch
			{
				BlockType.Grass => Brushes.Green,
				BlockType.Stone => Brushes.Gray,
				BlockType.Wood => Brushes.Brown,
				BlockType.Water => Brushes.Blue,
				_ => Brushes.Black
			};
		}

		// Проверка, виден ли чанк
		

		// Обновление текста количества чанков
		private void UpdateChunksText()
		{
			int totalChunks = MyGrid.GridSize * MyGrid.GridSize;
			int loadedChunks = 0;

			for (int chunkX = 0; chunkX < MyGrid.GridSize; chunkX++)
			{
				for (int chunkY = 0; chunkY < MyGrid.GridSize; chunkY++)
				{
					if (IsChunkVisible(chunkX, chunkY))
					{
						loadedChunks++;
					}
				}
			}

			ChunksText.Text = $"Чанки: {loadedChunks}/{totalChunks}";
		}
		private bool IsChunkVisible(int chunkX, int chunkY)
		{
			// Мировые координаты чанка
			double chunkWorldX = chunkX * MyChunk.ChunkSize * _blockSize;
			double chunkWorldY = chunkY * MyChunk.ChunkSize * _blockSize;
			double chunkWidth = MyChunk.ChunkSize * _blockSize;
			double chunkHeight = chunkWidth;

			// Экранные координаты чанка (с учетом смещения мира)
			double screenX = chunkWorldX - playerPos.X + MainCanvas.ActualWidth / 2;
			double screenY = chunkWorldY - playerPos.Y + MainCanvas.ActualHeight / 2;

			// Проверка пересечения с окном
			return screenX + chunkWidth >= 0 && screenX <= MainCanvas.ActualWidth &&
				   screenY + chunkHeight >= 0 && screenY <= MainCanvas.ActualHeight;
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
			UpdateChunksText();
		}

		// Обработчик нажатия кнопки "Выход"
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
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
			if (_fpsTimer >= 1.0)
			{
				FpsText.Text = $"FPS: {(int)(_fpsCounter / _fpsTimer)}";
				_fpsCounter = 0;
				_fpsTimer = 0;
				UpdateChunksText(); // Обновляем чанки раз в секунду
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

			// Перемещаем блоки
			foreach (var chunk in _worldGrid.Chunks)
			{
				foreach (var block in chunk.Blocks)
				{
					var rect = _blockRectangles[block];
					double currentX = Canvas.GetLeft(rect);
					double currentY = Canvas.GetTop(rect);
					double newX = currentX - move.X;
					double newY = currentY + move.Y;
					Canvas.SetLeft(rect, newX);
					Canvas.SetTop(rect, newY);
				}
			}

			// Обновляем текст с позицией игрока
			PositionText.Text = $"Игрок: {playerPos}";
		}
	}
}