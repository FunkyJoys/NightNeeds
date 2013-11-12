using System;

namespace ClassesLibrary {
    public struct IntRect
    {
        public IntRect(Point<int> topLeft, Size<int> size)
        {
            m_TopLeft = topLeft;
            m_Size = size;
        }

        public Point<int> TopLeft { 
            get { return m_TopLeft; }
            set { m_TopLeft = value; } 
        }

        public Size<int>  Size { 
            get { return m_Size; }
            set { m_Size = value; }
        }

        public bool Contains(Point<int> point)
        {
            return 
                m_TopLeft.X <= point.X && point.X <= m_TopLeft.X + m_Size.Width &&
                m_TopLeft.Y <= point.Y && point.Y <= m_TopLeft.Y + m_Size.Height;
        }

        private Point<int> m_TopLeft;
        private Size<int>  m_Size;
    }
}

