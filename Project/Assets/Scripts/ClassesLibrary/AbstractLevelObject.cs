using System;
using System.Collections.Generic;

namespace ClassesLibrary
{
	/// <summary>
	/// Абстрактное описание объекта игрового уровня.
	/// </summary>
	/// <remarks>
	/// Перед первым использованием обязательно необходимо установить ссылки на внешние данные
	/// методом SetStatics().
	/// </remarks>
	public abstract class AbstractLevelObject : SelfRegistrable
	{
		/// Создает новый экземпляр класса <see cref="ClassesLibrary.AbstractLevelObject"/>.
		public AbstractLevelObject ()
		{
			m_Effects = new Dictionary<String,AbstractObjectEffect> ();
		}

		/// Создает новый экземпляр класса <see cref="ClassesLibrary.AbstractLevelObject"/>.
		public AbstractLevelObject (String sObjectDescription)
			: this()
		{
			Orientation orientation = null;
			String objectName = null;

			m_sObjectDescriptor = ParseObjectDescriptor( sObjectDescription, out orientation, out objectName );

			ObjectOrientation = orientation;
			ObjectName = objectName;
		}

		/// Событие успешной или безуспешной попытки входа на объект состоялось
		public abstract void OnEntered(bool success);

		/// Проверяет принадлежность указанной координаты игрового уровня этому объекту.
		public virtual bool ContainsPoint (Point<int> point)
		{
			/// Реализация по умолчанию подразумевает, что объект прямоугольный
			return BoundingBox.Contains(point);
		}

		/// Проверяет возможность входа на данный объект по указанным координатам объекта.
		public virtual bool IsAccessibleAtPoint (Point<int> point)
		{
			/// Реализация по умолчанию подразумевает, что объект доступен везде
			return ContainsPoint(point);
		}

		/// <summary>
		/// Попытка входа игрока на территорию данного объекта.
		/// </summary>
		/// <param name='pointFrom'>
		/// Ячейка игрового уровня, откуда совершено движение.
		/// </param>
		/// <param name='pointTo'>
		/// Ячейка игрового уровня, принадлежащая этому объекту, и в которую произведена попытка входа.
		/// </param>
		public void Enter (Point<int> pointFrom, Point<int> pointTo)
		{
			// если **ориентация** объекта совместно с **проходимостью** допускают вход в него с ячейки "откуда происходит движение"
			if (!ContainsPoint (pointTo))
				return;

			bool bEneteredSuccessfully = IsAccessibleAtPoint (pointTo);
			if (bEneteredSuccessfully) {
				// то происходит обновление координат игрока (через интерфейс "*Перемещение игрока по уровню*"), 
				GamerMovement.MoveForward ();

				// а также через интерфейс "*Состояние*" обновление количества пройденных шагов 
				GamerState.MakeSteps ();

				// и модификация оставшегося количества времени у игрока с учетом параметра **пенальти**
				GamerState.TakeTime (Penalty);
			}

			OnEntered( bEneteredSuccessfully );
		}

		/// Устанавливает статические ссылки на игровую логику и интерфейсы игрока.
		static public void SetStatics (ref GameLogic gameLogic, GamerObject gamerObject)
		{
			m_GameLogic = gameLogic;
			m_GamerMovement = gamerObject;
			m_GamerState = gamerObject;
		}

		/// <summary>
		/// Разбирает на составляющие полное описание объекта.
		/// </summary>
		/// <returns>
		/// Описатель объекта.
		/// </returns>
		/// <param name='sObjectDescription'>
		/// Строка описания объекта в формате: <ОПИСАТЕЛЬ>[$<ОРИЕНТАЦИЯ>][\<НОМЕР_ОБЪЕКТА>],
		/// где в угловых скобках показаны обязательные составляющие, а в квадратных - необязательные.
		/// Причем ОРИЕНТАЦИЯ может быть либо в виде одной из букв из набора [N,E,S,W] либо числом, показывающим
		/// азимут (угол поворота по часовой стрелке от севера).
		/// Порядок следования необязательных ОРИЕНТАЦИЯ и НОМЕР_ОБЪЕКТА могут быть произвольным, но только после 
		/// ОПИСАТЕЛЬ. НОМЕР_ОБЪЕКТА - любая допустимая в данном контексте строка символов.
		/// </param>
		/// <param name='orientation'>
		/// Выходное значение - ориентация объекта на уровне.
		/// </param>
		/// <param name='sObjectName'>
		/// Выходное значение - составное имя объекта.
		/// </param>
		static public String ParseObjectDescriptor (
			String sObjectDescription, 
			out Orientation orientation,
			out String sObjectName)
		{
			orientation = new Orientation ();
			int dollarPos = sObjectDescription.IndexOf ('$');
			int backslashPos = sObjectDescription.IndexOf ('\\');
			int length = sObjectDescription.Length;

			int descriptorLen1 = length;

			if (0 <= dollarPos) {
				descriptorLen1 = dollarPos;

				int orientationStart = dollarPos + 1;
				int orientationEnd = length - 1;
				if (0 <= backslashPos) {
					if (dollarPos < backslashPos) {
						orientationEnd = backslashPos - 1;

					} else {
						descriptorLen1 = backslashPos;
					}
				}
				String sAzimuth = sObjectDescription.Substring (orientationStart, orientationEnd - orientationStart + 1);
				orientation.SetOrientation (sAzimuth);
			}

			int descriptorLen2 = length;
			String sObjectNumber = null;
						
			if (0 <= backslashPos) {
				descriptorLen2 = dollarPos;
			
				int numberStart = backslashPos + 1;
				int numberEnd = length - 1;
				if (0 <= dollarPos) {
					if (backslashPos < dollarPos) {
						numberEnd = backslashPos - 1;

					} else {
						descriptorLen2 = dollarPos;
					}
				}
				sObjectNumber = sObjectDescription.Substring (numberStart, numberEnd - numberStart + 1);
			}

			int descriptorLen = Math.Min (descriptorLen1, descriptorLen2);
			String sObjectDescriptor = sObjectDescription.Substring (0, descriptorLen);

			sObjectName = sObjectDescriptor + sObjectNumber;

			return sObjectDescriptor;
		}


		/// Возвращает уникальный описатель объекта
		public String ObjectDescriptor { get { return m_sObjectDescriptor; } }

		/// Возвращает или устанавливает данному объекту имя. 
		/// Это имя впоследствии может быть использовано для создания одноименного объекта на игровой сцене
		public String ObjectName { get; set; }

		/// Описывающий прямоугольник объекта
		public IntRect BoundingBox { get; set; }

		/// Получает и возвращает ориентацию объекта по сторонам света или по азимуту
		public Orientation ObjectOrientation { get; set; }

		/// Количество игровых очков, которое будет потрацено при прохождении этого объекта
		public double Penalty { get; set; }

		/// <summary>
		/// Добавляет эффект к общему списку эффектов.
		/// </summary>
		/// <returns>
		/// Номер добавленного эффекта в списке, либо -1, если добавление не удалось.
		/// </returns>
		/// <param name='effect'>
		/// Эффект
		/// </param>
		protected int AddEffect (AbstractObjectEffect effect, String sEffectName)
		{
			if (null == effect) return -1;

			int size = m_Effects.Count;
			m_Effects.Add( sEffectName, effect );
			return size;
		}

		/// По указанному имени возвращает эффект, либо null, если эффекта с таким именем не найдено.
		public AbstractObjectEffect GetEffect (String sEffectName)
		{
			if (!m_Effects.ContainsKey (sEffectName))
				return null;

			return m_Effects [sEffectName];
		}

		/// Воспроизводит указанный по имени эффект.
		public void PlayEffect (String sEffectName)
		{
			AbstractObjectEffect effect = GetEffect (sEffectName);
			if (null != effect) {
				effect.Play();
			}
		}

		/// Последовательно воспроизводит все эффекты.
		public void PlayAllEffects ()
		{
			foreach (AbstractObjectEffect effect in m_Effects.Values) {
				if (null != effect) {
					effect.Play ();
				}
			}
		}

		/// Возвращает класс игровой логики.
		static protected GameLogic GameLogic {
			get { return m_GameLogic; }
		}

		/// Возвращает интерфейс "Перемещение игрока".
		static protected Gamer.IMovement GamerMovement {
			get { return m_GamerMovement; }
		}

		/// Возвращает интерфейс "Состояние игрока".
		static protected Gamer.IState GamerState {
			get { return m_GamerState; }
		}

		

		private String 									m_sObjectDescriptor;
		private Dictionary<String,AbstractObjectEffect>	m_Effects;
		static private GameLogic						m_GameLogic;
		static private Gamer.IMovement					m_GamerMovement;
		static private Gamer.IState						m_GamerState;
	}
}

