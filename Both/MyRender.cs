using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Both
{
	public class MyRender
	{
		public MyRender(Canvas canvas)
		{
			Canvas = canvas;

			Dict = new();
		}
		public Dictionary<MyBlock, Rectangle> Dict;
		public Canvas Canvas;

		public void Update(MyPlayer player, MyGrid grid)
		{
			Rectangle block;
			if (!Dict.TryGetValue(myBlock, out block))
			{


			}
			else
			{
				block = new()
				{
					Width = MyBlock.blockSize,
					Height = MyBlock.blockSize,
					Fill = MyBlock.GetBlockBrush(myBlock.Type)
				};

				Dict.Add(myBlock, block);
				Canvas.Children.Add(block);
			}
			// Обновляем координаты видимых блоков
			foreach (var pair in Dict)
			{
				MyBlock block = pair.Key;
				Rectangle rect = pair.Value;

				if (player.Pos.Distance(block.Pos) > 200)
				{

				}
				else
				{
					double screenX = block.Pos.X - player.Pos.X + MyScreen.Size.X / 2;
					double screenY = block.Pos.Y + player.Pos.Y + MyScreen.Size.Y / 2;
					Canvas.SetLeft(rect, screenX);
					Canvas.SetTop(rect, screenY);
				}
			}

		}

	}
}
