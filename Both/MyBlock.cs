using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Both
{
	public class MyBlock
	{
		public static readonly double blockSize = 50;

		public BlockType Type;
		public XY Pos;

		public static SolidColorBrush GetBlockBrush(BlockType type)
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

	public enum BlockType
	{
		Grass, Stone, Wood, Water, 
	}
}
