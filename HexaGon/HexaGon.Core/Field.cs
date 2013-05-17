using System;

namespace HexaGon
{
	public class Field
	{

		public int FieldState;
		public int UnitState;
		public int x;
		public int y;

		public Field (int fs = 0, int us = 0, int x_ = 0, int y_ = 0)
		{

			FieldState = fs;
			UnitState = us;
			x = x_;
			y = y_;

		}
	}
}

