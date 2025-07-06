using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Both
{
	public class MyPlayer
	{
		public void UpdatePlayerPosition()
		{
			Canvas.SetLeft(Shape, MyScreen.Size.X / 2 - Shape.Width / 2);
			Canvas.SetTop(Shape, MyScreen.Size.Y / 2 - Shape.Height / 2);
		}

		public MyPlayer()
		{
			// Инициализация виртуальной позиции
			Pos = new XY(0, 0);

			// Создаем игрока (черный квадрат)
			Shape = new Rectangle
			{
				Name = "Square",
				Width = 15,
				Height = 15,
				Fill = Brushes.Black
			};

			UpdatePlayerPosition();
		}
		public XY Pos; // Виртуальная позиция игрока в мире
		public Rectangle Shape; // Игрок (черный квадрат)
	}
}
