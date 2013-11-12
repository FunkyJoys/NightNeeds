using System;

namespace ClassesLibrary {
    namespace Gamer {

        /// Интерфейс "Состояние игрока".
        public interface IState
        {
            /// Отнимает у игрока на этом уровне в текущей игре количество времени
            void TakeTime(double penalty);

            /// Увеличивает количество шагов, проделанное на этом уровне в текущей игре
            void MakeSteps(int stepsDone = 1);

            /// Ориентация игрока (взгляда) в пространстве
            Orientation Orientation { get; set; }

            /// Текущая позиция игрока на игровом поле
            Point<int> CurrentPosition { get; }
        }
    }
}

