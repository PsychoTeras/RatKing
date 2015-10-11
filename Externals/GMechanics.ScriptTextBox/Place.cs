
namespace GMechanics.ScriptTextBox
{
    /// <summary>
    /// Line index and char index
    /// </summary>
    public struct Place
    {
        public bool Equals(Place other)
        {
            return iChar == other.iChar && iLine == other.iLine;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Place && Equals((Place) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (iChar*397) ^ iLine;
            }
        }

        public int iChar;
        public int iLine;

        public Place(int iChar, int iLine)
        {
            this.iChar = iChar;
            this.iLine = iLine;
        }

        public void Offset(int dx, int dy)
        {
            iChar += dx;
            iLine += dy;
        }

        public static bool operator !=(Place p1, Place p2)
        {
            return !p1.Equals(p2);
        }

        public static bool operator ==(Place p1, Place p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator <(Place p1, Place p2)
        {
            if (p1.iLine < p2.iLine) return true;
            if (p1.iLine > p2.iLine) return false;
            if (p1.iChar < p2.iChar) return true;
            return false;
        }

        public static bool operator <=(Place p1, Place p2)
        {
            if (p1.Equals(p2)) return true;
            if (p1.iLine < p2.iLine) return true;
            if (p1.iLine > p2.iLine) return false;
            if (p1.iChar < p2.iChar) return true;
            return false;
        }

        public static bool operator >(Place p1, Place p2)
        {
            if (p1.iLine > p2.iLine) return true;
            if (p1.iLine < p2.iLine) return false;
            if (p1.iChar > p2.iChar) return true;
            return false;
        }

        public static bool operator >=(Place p1, Place p2)
        {
            if (p1.Equals(p2)) return true;
            if (p1.iLine > p2.iLine) return true;
            if (p1.iLine < p2.iLine) return false;
            if (p1.iChar > p2.iChar) return true;
            return false;
        }

        public static Place Empty
        {
            get { return new Place(); }
        }

        public override string ToString()
        {
            return "(" + iChar + "," + iLine + ")";
        }
    }
}
