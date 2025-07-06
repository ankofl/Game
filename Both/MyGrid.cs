using System.Numerics;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Both
{
	public class MyGrid
	{
		public static readonly int GridSize = 10;
		public XY Zero = new(0, 0);
		public MyChunk[] Chunks;

		public MyGrid()
		{
			Chunks = new MyChunk[GridSize * GridSize];

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

							
						}
					}
					Chunks[chunkX + chunkY * GridSize] = chunk;
				}
			}
		}
	}
}