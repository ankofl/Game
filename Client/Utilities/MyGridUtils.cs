using Both;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client.Utilities
{
	public static class MyGridUtils
	{
		private static readonly List<Rectangle> _rectanglePool = new List<Rectangle>();

		public static void Initialize(MyGrid grid, Dictionary<MyBlock, Rectangle> blockRectangles, Canvas canvas, double blockSize, XY playerPos, double canvasWidth, double canvasHeight)
		{
			Random rand = new Random();
			for (int chunkX = 0; chunkX < MyGrid.GridSize; chunkX++)
			{
				for (int chunkY = 0; chunkY < MyGrid.GridSize; chunkY++)
				{
					MyChunk chunk = new MyChunk();
					for (int blockX = 0; blockX < MyChunk.ChunkSize; blockX++)
					{
						for (int blockY = 0; blockY < MyChunk.ChunkSize; blockY++)
						{
							BlockType type = (BlockType)rand.Next(Enum.GetValues(typeof(BlockType)).Length);
							chunk.Blocks[blockX + blockY * MyChunk.ChunkSize] = new MyBlock
							{
								Type = type,
								WorldX = (chunkX * MyChunk.ChunkSize + blockX) * blockSize,
								WorldY = (chunkY * MyChunk.ChunkSize + blockY) * blockSize
							};
						}
					}
					grid.Chunks[chunkX + chunkY * MyGrid.GridSize] = chunk;
				}
			}

			UpdateVisibleBlocks(grid, blockRectangles, canvas, blockSize, playerPos, canvasWidth, canvasHeight);
		}

		public static void UpdateVisibleBlocks(MyGrid grid, Dictionary<MyBlock, Rectangle> blockRectangles, Canvas canvas, double blockSize, XY playerPos, double canvasWidth, double canvasHeight)
		{
			foreach (var pair in blockRectangles.ToList())
			{
				if (!IsBlockVisible(pair.Value, canvasWidth, canvasHeight))
				{
					canvas.Children.Remove(pair.Value);
					_rectanglePool.Add(pair.Value);
					blockRectangles.Remove(pair.Key);
				}
			}

			var (minChunkX, maxChunkX, minChunkY, maxChunkY) = GetVisibleChunkRange(blockSize, playerPos, canvasWidth, canvasHeight);
			for (int chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
			{
				for (int chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
				{
					if (chunkX >= 0 && chunkX < MyGrid.GridSize && chunkY >= 0 && chunkY < MyGrid.GridSize)
					{
						MyChunk chunk = grid.Chunks[chunkX + chunkY * MyGrid.GridSize];
						for (int blockX = 0; blockX < MyChunk.ChunkSize; blockX++)
						{
							for (int blockY = 0; blockY < MyChunk.ChunkSize; blockY++)
							{
								MyBlock block = chunk.Blocks[blockX + blockY * MyChunk.ChunkSize];
								if (!blockRectangles.ContainsKey(block))
								{
									Rectangle blockRect = GetRectangleFromPool(block.Type, blockSize);
									canvas.Children.Add(blockRect);
									blockRectangles[block] = blockRect;
								}
								Rectangle rect = blockRectangles[block];
								double screenX = block.WorldX - playerPos.X + canvasWidth / 2;
								double screenY = block.WorldY - playerPos.Y + canvasHeight / 2;
								Canvas.SetLeft(rect, screenX);
								Canvas.SetTop(rect, screenY);
							}
						}
					}
				}
			}
		}

		private static Rectangle GetRectangleFromPool(BlockType type, double blockSize)
		{
			if (_rectanglePool.Count > 0)
			{
				Rectangle rect = _rectanglePool[_rectanglePool.Count - 1];
				_rectanglePool.RemoveAt(_rectanglePool.Count - 1);
				rect.Fill = GetBlockBrush(type);
				rect.Width = (int)blockSize;
				rect.Height = (int)blockSize;
				return rect;
			}
			return new Rectangle
			{
				Width = (int)blockSize,
				Height = (int)blockSize,
				Fill = GetBlockBrush(type)
			};
		}

		private static bool IsBlockVisible(Rectangle rect, double canvasWidth, double canvasHeight)
		{
			double x = Canvas.GetLeft(rect);
			double y = Canvas.GetTop(rect);
			double blockSize = rect.Width;
			return x + blockSize >= 0 && x <= canvasWidth && y + blockSize >= 0 && y <= canvasHeight;
		}

		public static (int minX, int maxX, int minY, int maxY) GetVisibleChunkRange(double blockSize, XY playerPos, double canvasWidth, double canvasHeight)
		{
			double chunkSize = MyChunk.ChunkSize * blockSize;
			double minScreenX = -canvasWidth / 2 + playerPos.X;
			double minScreenY = -canvasHeight / 2 + playerPos.Y;
			double maxScreenX = minScreenX + canvasWidth;
			double maxScreenY = minScreenY + canvasHeight;

			int minChunkX = Math.Max(0, (int)(minScreenX / chunkSize));
			int maxChunkX = Math.Min(MyGrid.GridSize - 1, (int)(maxScreenX / chunkSize));
			int minChunkY = Math.Max(0, (int)(minScreenY / chunkSize));
			int maxChunkY = Math.Min(MyGrid.GridSize - 1, (int)(maxScreenY / chunkSize));

			return (minChunkX, maxChunkX, minChunkY, maxChunkY);
		}

		public static bool IsChunkVisible(int chunkX, int chunkY, double blockSize, XY playerPos, double canvasWidth, double canvasHeight)
		{
			double chunkWorldX = chunkX * MyChunk.ChunkSize * blockSize;
			double chunkWorldY = chunkY * MyChunk.ChunkSize * blockSize;
			double chunkWidth = MyChunk.ChunkSize * blockSize;
			double chunkHeight = chunkWidth;

			double screenX = chunkWorldX - playerPos.X + canvasWidth / 2;
			double screenY = chunkWorldY - playerPos.Y + canvasHeight / 2;

			return screenX + chunkWidth >= 0 && screenX <= canvasWidth &&
				   screenY + chunkHeight >= 0 && screenY <= canvasHeight;
		}

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
	}
}