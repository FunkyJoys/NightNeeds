using System;
using System.Collections.Generic;

namespace ClassesLibrary {
    /// <summary>
    /// Двумерный массив
    /// </summary>
    public class DoubleArray<T>
    {
        public DoubleArray(int width = 0, int height = 0)
        {
            m_data = new List<List<T> >();
            SetSize( width, height );
        }

        public void Clear()
        {
            m_data.Clear();
        }

        public int Width {
            get {
                if( Height <= 0 ) return 0;

                return m_data[ 0 ].Count;
            }
        }

        public int Height {
            get { return m_data.Count; }
        }

        public T Item(int row, int col)
        {
            if( row < Height && col < Width ) return m_data[ row ][ col ];
            return default(T);
        }

        /// <summary>
        /// Установка нового размера массива с очисткой данных
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void SetSize(int width, int height)
        {
            Clear();

            for( int row = 0; row < height; ++row ) {
                List<T> oneRow = new List<T>();
                for( int col = 0; col < width; ++col ) {
                    oneRow.Add( default(T) );
                }
                m_data.Add( oneRow );
            }
        }

        public void SetItem(int row, int col, T value)
        {
            if( row < Height && col < Width ) m_data[ row ][ col ] = value;
        }

        private List< List<T> >	m_data;
    }
}

