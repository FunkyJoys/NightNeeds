using System;

namespace ClassesLibrary
{
	/// Простая пара целочисленных координат.
	public struct Point<T>
	{
		public T X { get; set; }
		public T Y { get; set; }
	}
}

