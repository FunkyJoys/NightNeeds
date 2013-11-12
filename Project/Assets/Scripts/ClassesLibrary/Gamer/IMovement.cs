using System;

namespace ClassesLibrary {
    namespace Gamer {
        /// Интерфейс "Перемещение игрока".
        public interface IMovement
        {
            /// Повернуть игрока влево на 90 градусов.
            void TurnLeft();

            /// Повернуть игрока вправо на 90 градусов.
            void TurnRight();

            /// Сделать шаг вперед на одну клетку.
            void MoveForward();

            /// Сделать шаг назад на одну клетку.
            void StepBackward();
        }
    }
}
