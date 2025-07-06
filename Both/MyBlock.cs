using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Both
{
	public class MyBlock
	{
		public BlockType Type;
		public double WorldX; // Мировая координата X
		public double WorldY; // Мировая координата Y
	}

	public enum BlockType
	{
		Grass, Stone, Wood, Water, 
	}
}
