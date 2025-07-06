using System;

namespace Both
{
	public class XY
	{
		public static XY Zero => new(0, 0);
		public static XY Up => new(0, 1);
		public static XY Down => new(0, -1);
		public static XY Left => new(-1, 0);
		public static XY Right => new(1, 0);

		public XY Normalize()
		{
			double length = Length();
			if (length == 0)
				throw new InvalidOperationException("Cannot normalize a zero-length vector.");
			return this / length;
		}

		public double X;
		public double Y;

		// Конструктор
		public XY(double x, double y)
		{
			X = x;
			Y = y;
		}

		// Оператор сложения (+)
		public static XY operator +(XY a, XY b)
		{
			return new XY(a.X + b.X, a.Y + b.Y);
		}

		// Оператор вычитания (-)
		public static XY operator -(XY a, XY b)
		{
			return new XY(a.X - b.X, a.Y - b.Y);
		}

		// Оператор умножения на скаляр
		public static XY operator *(XY a, double scalar)
		{
			return new XY(a.X * scalar, a.Y * scalar);
		}

		// Умножение скаляра на вектор (коммутативность)
		public static XY operator *(double scalar, XY a)
		{
			return a * scalar;
		}

		// Оператор деления на скаляр
		public static XY operator /(XY a, double scalar)
		{
			if (scalar == 0)
				throw new DivideByZeroException("Division by zero is not allowed.");
			return new XY(a.X / scalar, a.Y / scalar);
		}

		// Унарный минус (отрицание вектора)
		public static XY operator -(XY a)
		{
			return new XY(-a.X, -a.Y);
		}

		// Оператор равенства (==)
		public static bool operator ==(XY a, XY b)
		{
			if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
				return true;
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
				return false;
			return Math.Abs(a.X - b.X) < 1e-10 && Math.Abs(a.Y - b.Y) < 1e-10;
		}

		// Оператор неравенства (!=)
		public static bool operator !=(XY a, XY b)
		{
			return !(a == b);
		}

		// Скалярное произведение (Dot Product)
		public static double Dot(XY a, XY b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		// Псевдоскалярное произведение (Cross Product в 2D, возвращает скаляр)
		public static double Cross(XY a, XY b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		// Длина вектора (Length)
		public double Length()
		{
			return Math.Sqrt(X * X + Y * Y);
		}

		// Квадрат длины вектора (для оптимизации, чтобы избежать корня)
		public double LengthSquared()
		{
			return X * X + Y * Y;
		}

		// Угол между двумя векторами (в радианах)
		public static double AngleBetween(XY a, XY b)
		{
			double dot = Dot(a, b);
			double lengths = a.Length() * b.Length();
			if (lengths == 0)
				throw new InvalidOperationException("Cannot compute angle with zero-length vector.");
			double cosTheta = Math.Clamp(dot / lengths, -1.0, 1.0);
			return Math.Acos(cosTheta);
		}

		// Переопределение Equals для сравнения
		public override bool Equals(object obj)
		{
			if (obj is XY other)
				return this == other;
			return false;
		}

		// Переопределение GetHashCode
		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}

		// Строковое представление
		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}