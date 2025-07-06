using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Both
{
	public class MyChunk
	{
		public static readonly int ChunkSize = 3;
		public MyBlock[] Blocks;

		public MyChunk()
		{
			Blocks = new MyBlock[ChunkSize * ChunkSize];
		}
	}
}
