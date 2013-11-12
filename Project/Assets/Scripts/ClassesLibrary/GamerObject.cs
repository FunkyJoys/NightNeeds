using System;

namespace ClassesLibrary
{
	/// Объект "Игрок".
	public class GamerObject : Gamer.IState, Gamer.IMovement, Gamer.IVision, Gamer.IBackLight
	{
		public GamerObject ()
		{
			m_Orientation = new Orientation();
		}

		/// Повернуть игрока влево на 90 градусов.
		public void TurnLeft ()
		{
			m_Orientation.AddAzimuth( -90.0 );
		}

		/// Повернуть игрока вправо на 90 градусов.
		public void TurnRight()
		{
			m_Orientation.AddAzimuth( +90.0 );
		}

		/// Сделать шаг вперед на одну клетку.
		public void MoveForward()
		{
		}

		/// Сделать шаг назад на одну клетку.
		public void StepBackward()
		{
		}

		/// Установить игрока в конкретную позицию на игровом поле.
		public void PaceIntoPosition(Point<int> position)
		{
			m_Position = position;
		}

		/// Ориентация игрока (взгляда) в пространстве
		public Orientation Orientation { 
			get { return m_Orientation; }
			set { m_Orientation = value; } 
		}

		/// Текущая позиция игрока на игровом поле
		public Point<int> CurrentPosition {
			get {
				return m_Position;
			}
		}

		/// Отнимает у игрока на этом уровне в текущей игре количество времени
		public void TakeTime (double penalty)
		{
			TimeLeft -= penalty;
			if (TimeLeft <= 0) {
				m_GameLogic.TimeIsUp();
			}
		}


		/// Увеличивает количество шагов, проделанное на этом уровне в текущей игре
		public void MakeSteps (int stepsDone)
		{
			++StepsInGame;
		}

		/// Оставшееся количество времени до конца текущего уровня
		public double TimeLeft { get; set; }

		/// Количество шагов, проделанное в этом уровне в текущей игре
		public int StepsInGame { get; set; }	

		/// Устанавливает статические ссылки на игровую логику
		static public void SetStatics (ref GameLogic gameLogic)
		{
			m_GameLogic = gameLogic;
		}
		

		private Point<int>			m_Position;
		private Orientation			m_Orientation;
		static private GameLogic	m_GameLogic;
		
	}
}

