using System;

namespace ClassesLibrary
{
	public class Orientation
	{
		public enum FixedOrientation {
			North,
			East,
			South,
			West,

			First = North,
			Last = West
		};

		public Orientation ()
		{
			m_fAzimuth = 0.0;
		}

		/// Устанавливает ориентацию по сторонам света.
		public void SetFixedOrientation (FixedOrientation fixedOrientation)
		{
			double[] fixedAngles = new double[] { 0.0, 90.0, 180.0, 270.0 };

			m_fAzimuth = fixedAngles[ (int)fixedOrientation ];
		}

		/// Устанавливает ориентацию либо по букве (N, E, S или W), либо по числу, представляющему азимут
		public void SetOrientation (String s)
		{
			char[] nesw = new char[] { 'N','E','S','W' };
			if (0 == s.IndexOfAny (nesw)) {
				for( FixedOrientation fo = FixedOrientation.First; fo <= FixedOrientation.Last; ++fo ) {
					int index = (int)fo;
					if( nesw[ index ] == s[0] ) {
						SetFixedOrientation (fo);
					}
				}
				return;
			}

			m_fAzimuth = 0.0;
			try {
				m_fAzimuth = Convert.ToDouble (s);

			} catch (FormatException) {
				// В случае ошибки преобразования получим нуль
			}

		}

		public double Azimuth {
			get {
				return m_fAzimuth;
			}
			set {
				m_fAzimuth = value;
			}
		}

		/// Увеличивает значение азимута на указанный угол в градусах с приведением к интервалу [0..360)
		public void AddAzimuth (double angle)
		{
			m_fAzimuth += angle;

			int step = (int)Math.Floor( m_fAzimuth / 360.0 );
			m_fAzimuth -= 360.0 * step;

			if( m_fAzimuth < 0.0 ) m_fAzimuth += 360.0;
			if( 360.0 <= m_fAzimuth ) m_fAzimuth -= 360.0;
		}

		private double m_fAzimuth;
	}
}

