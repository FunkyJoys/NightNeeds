using System;

namespace ClassesLibrary {
    /// <summary>
    /// Саморегистрирующися в собственной синглтоновской фабрике объект.
    /// </summary>
    /// <description>
    /// Для того, чтобы это все заработало, нужно реализовать статический метод
    /// static public ConstructorDelegate Constructor(), который бы возвращал 
    /// делегата для создания экземпляра класса.
    /// После того, как будет описан новый класс (Derived), который должен попасть в общую
    /// фабрику этих объектов, где-то в основном коде нужно будет его зарегистрировать:
    /// SelfRegistrable.Register( "DerivedName", Derived.Constructor() )
    /// В дальнейшем коде, чтобы получить новый экземпляр по его имени, достаточно вызвать такой метод:
    /// MyObjectType my = SelfRegistrable.GetObjectByDescriptor( "DerivedName" ) as MyObjectType;
    /// </description>
    public abstract class SelfRegistrable
    {
        public SelfRegistrable()
        {
            if( null == m_RegistrableFactory ) {
                m_RegistrableFactory = new RegistrableFactory();
            }
        }

        /// <summary>
        /// Регистрация делегата для создания объекта в фабрике объектов под указанным именем.
        /// </summary>
        /// <param name='name'>
        /// Имя, под которым будет вызываться новый объект из фабрики объектов.
        /// </param>
        /// <param name='ctor'>
        /// Делегат для создания объекта, который необходимо зарегистрировать.
        /// </param>
        static public void Register(String name, ConstructorDelegate constructorDelegate)
        {
            m_RegistrableFactory.Register( name, constructorDelegate );
        }

        /// <summary>
        /// Возвращает объект по его имени.
        /// </summary>
        /// <returns>
        /// Объект либо null, если соответствующего имени объекта не найдено.
        /// </returns>
        /// <param name='sObjectDescriptor'>
        /// Имя объекта.
        /// </param>
        static public SelfRegistrable GetObjectByDescriptor(String name)
        {
            return m_RegistrableFactory.GetObjectByDescriptor( name );
        }

        static protected RegistrableFactory m_RegistrableFactory;
    }
    /// <summary>
    /// Делегат создания объектов.
    /// </summary>
    public delegate SelfRegistrable ConstructorDelegate();
}

