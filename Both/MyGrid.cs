using System.Numerics;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Both
{
	public class MyGrid
	{
		public Canvas Canvas;
		public static readonly int GridSize = 10;
		public XY Zero = new(0, 0);
		public MyChunk[] Chunks;

		public MyGrid(Canvas canvas)
		{
			Canvas = canvas;

			Chunks = new MyChunk[GridSize * GridSize];
			_blockRectangles = [];

			Random rand = new Random();
			for (int chunkX = 0; chunkX < GridSize; chunkX++)
			{
				for (int chunkY = 0; chunkY < GridSize; chunkY++)
				{
					MyChunk chunk = new MyChunk();
					for (int blockX = 0; blockX < MyChunk.ChunkSize; blockX++)
					{
						for (int blockY = 0; blockY < MyChunk.ChunkSize; blockY++)
						{
							BlockType type = (BlockType)rand.Next(Enum.GetValues(typeof(BlockType)).Length);

							MyBlock myBlock = new MyBlock
							{
								Type = type,
								Pos = new XY(
									(chunkX * MyChunk.ChunkSize + blockX) * MyBlock.blockSize,
									(chunkY * MyChunk.ChunkSize + blockY) * MyBlock.blockSize),
							};

							chunk.Blocks[blockX + blockY * MyChunk.ChunkSize] = myBlock;

							Rectangle block = new()
							{
								Width = MyBlock.blockSize,
								Height = MyBlock.blockSize,
								Fill = MyBlock.GetBlockBrush(type)
							};


							_blockRectangles.Add(myBlock, block);
							Canvas.Children.Add(block);
						}
					}
					Chunks[chunkX + chunkY * GridSize] = chunk;
				}
			}
		}

		public Dictionary<MyBlock, Rectangle> _blockRectangles;

		public void MoveWorld(XY playerPos)
		{
			if (!playerPos.Equals(LastPos))
			{
				LastPos = playerPos;
				
				// Обновляем координаты видимых блоков
				foreach (var pair in _blockRectangles)
				{
					MyBlock block = pair.Key;
					Rectangle rect = pair.Value;
					double screenX = block.Pos.X - playerPos.X + MyScreen.Size.X / 2;
					double screenY = block.Pos.Y + playerPos.Y + MyScreen.Size.Y / 2;
					Canvas.SetLeft(rect, screenX);
					Canvas.SetTop(rect, screenY);
				}
			}

			
		}
		private XY LastPos = XY.Zero;
	}
}