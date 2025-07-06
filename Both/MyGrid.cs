namespace Both
{
	public class MyGrid
	{
		public static readonly int GridSize = 10;
		public XY Zero = new XY(0, 0);
		public MyChunk[] Chunks;

		public MyGrid()
		{
			Chunks = new MyChunk[GridSize * GridSize];
		}
	}
}