using System;
using System.Collections.Generic;

namespace ClassesLibrary {
    /// <summary>
    /// Обобщенная фабрика саморегистрирующихся объектов.
    /// </summary>
    public class RegistrableFactory
    {
        public RegistrableFactory()
        {
            m_AllDelegates = new Dictionary<String,ConstructorDelegate>( );
        }

        /// <summary>
        /// Регистрация нового объекта игрового уровня.
        /// </summary>
        /// <param name='levelObject'>
        /// Игровой объект.
        /// </param>
        public void Register(String name, ConstructorDelegate constructorDelegate)
        {
            if( m_AllDelegates.ContainsKey( name ) || null == constructorDelegate ) { 
                return;
            }

            m_AllDelegates.Add( name, constructorDelegate );
        }

        /// <summary>
        /// Возвращает объект игрового уровня по его описателю.
        /// </summary>
        /// <returns>
        /// Объект игрового уровня, либо null, если соответствующего описателю объекта не найдено.
        /// </returns>
        /// <param name='sObjectDescriptor'>
        /// Описатель объекта.
        /// </param>
        public SelfRegistrable GetObjectByDescriptor(String name)
        {
            if( !m_AllDelegates.ContainsKey( name ) ) {
                return null;
            }

            ConstructorDelegate constructorDelegate = m_AllDelegates[ name ];
            return constructorDelegate( );
        }

        private Dictionary<String,ConstructorDelegate>  m_AllDelegates;
    }
}

