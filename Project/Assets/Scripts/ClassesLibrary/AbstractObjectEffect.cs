using System;

namespace ClassesLibrary {
    /// Абстрактное описание эффекта игрового объекта.
    public abstract class AbstractObjectEffect : SelfRegistrable
    {
        /// Устанавливает статическую ссылку на игровую логику.
        static public void SetGameLogic(ref GameLogic gameLogic)
        {
            m_GameLogic = gameLogic;
        }

        /// Возвращает класс игровой логики.
        protected GameLogic GetGameLogic()
        {
            return m_GameLogic;
        }

        public AbstractObjectEffect()
        {
        }

        /// Основное действие эффекта.
        public abstract void Play();

        static private GameLogic    m_GameLogic;

    }
}

