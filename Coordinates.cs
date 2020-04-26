using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    //an X and Y value pair - possible values are 1-9.
    //it's possible to raise the upper limit for a bigger map but watch out for gethashcode implementation
    class Coordinates
    {
        private int _x;
        public int X
        {
            get
            { return _x; }
            set
            {
                if (value > 9 || value < 1)
                    throw new System.SystemException("x coordinate out of range");
                else
                    _x = value;
            }
        }
        private int _y;
        public int Y
        {
            get
            { return _y; }
            set
            {
                if (value > 9 || value < 1)
                    throw new System.SystemException("y coordinate out of range");
                else
                    _y = value;
            }
        }

        public Coordinates()
        {
            _x = 1;
            _y = 1;
        }
        public Coordinates(int x=1,int y=1)
        {
            if (x < 1 || x > 9 || y < 1 || y > 9)
                throw new System.SystemException("bad coords constructor");
            this._x = x;
            this._y = y;
        }

        public static bool operator !=(Coordinates c1, Coordinates c2)
        {
			if (c1 == null && c2 == null)
				return false;
			if (c1 == null || c2 == null)
				return true;
			if (c1.X == c2.X && c1.Y == c2.Y)
                return false;
            else return true;
        }
        public static bool operator == (Coordinates c1, Coordinates c2)
        {
			if (c1 is null && c2 is null)
				return true;
			if (c1 is null || c2 is null)
				return false;
			if (c1.X == c2.X && c1.Y == c2.Y)
                return true;
            else return false;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType()==typeof(Coordinates))
            {
                var c = (Coordinates)obj;
				if (this == null && c == null)
					return true;
				if (this == null || c == null)
					return false;
				if (this.X == c.X && this.Y == c.Y)
                    return true;
                else return false;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _x*10+_y;
        }
    }
}
