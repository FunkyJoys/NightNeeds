using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ClassesLibrary
{
	/// <summary>
	/// Парсер файлов-описателей уровня.
	/// Основан на модифицированном формате XPM.
	/// Два основных метода: LoadFromFile() - загрузка; и Item() - получение описателя.
	/// </summary>
	public class GameLevelParser
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClassesLibrary.GameLevelParser"/> class.
		/// </summary>
		public GameLevelParser ()
		{
			Clear();
			m_lastError = null;
			m_fileName = null;

			m_Dictionary = new StringDictionary();
			m_LevelArray = new DoubleArray<String>();
		}

		/// <summary>
		/// Загрузка информации об уровне из файла.
		/// </summary>
		/// <returns>
		/// Признак успешной загрузки данных
		/// </returns>
		/// <param name='fileName'>
		/// Имя файла с описанием игрового уровня
		/// </param>
		/// <param name="encoding">
		/// Кодировка файла
		/// </param>
		public bool LoadFromFile (String fileName, Encoding encoding )
		{
			m_lastError = null;
			m_fileName = fileName;
			Clear();

			// Синтетический цикл, выход из которого означает ошибку парсинга файла
			do {
				m_lastError = "File not found";
				if (fileName == null || !File.Exists (fileName)) break;

				// Логика работы:
				// 1. Разбиваем весь файл на строки.
				String[] allLines = File.ReadAllLines (fileName, encoding); // Все строки файла
				int nLineCount = allLines.Length; // Общее число строк в файле

				m_lastError = @"Empty file";
				if (nLineCount == 0) break;

				int nCurrLine = 0; // Счетчик текущей строки

				// 2. Удаляем строку, содержащую / * GAMELEVEL * / (слово GAMELEVEL - вместо XPM). Проверяем, что это самая первая строка в файле.
				const String sHeaderLine = @"/* GAMELEVEL */";
				m_lastError = @"File must be started with /* GAMELEVEL */";
				if (nLineCount <= nCurrLine || sHeaderLine != allLines [nCurrLine++]) break;

				// 3. Следующая строка должна начинаться на **static char * ** и заканчиваться символами **_xpm[] = {**. Все, что между этими символами - название уровня. Удаляем строку.
				m_lastError = @"Not enought lines";
				if (nLineCount <= nCurrLine) break;

				String sStaticLine = allLines [nCurrLine++];
				const String sStaticLineStart = @"static char * ";
				const String sStaticLineEnd = @"_xpm[] = {";
				m_lastError = @"Pattern 'static char * <name>_xpm[] = {' not found on second line";
				if (!sStaticLine.StartsWith (sStaticLineStart) || !sStaticLine.EndsWith (sStaticLineEnd)) break;

				bool bParseSucceeded = false;
				bool bParametersLineFound = false;
				int nLevelLinesCount = 0; // Счетчик строк уровня

				// 4. Со всех остальных строк снимаем:
				while (nCurrLine < nLineCount) {
					bParseSucceeded = false;

					String sLine = allLines [nCurrLine++];
					m_lastError = String.Format( "Too short line {0}", nCurrLine-1 );
					if( sLine.Length < 3 ) break;

					//    - первый символ, который должен быть двойной кавычкой;
					m_lastError = String.Format( "Line {0} doesn't starts with '\"'", nCurrLine );
					if( !sLine[0].Equals('"') ) break;
					sLine = sLine.Remove(0,1);

					//    - проверяем последний символ. Если это **"};** - значит эту строку считаем последней и после нее прекращаем обработку. Удаляем эту последовательность символов;
					bool bIsLastLine = sLine.EndsWith( "\"};" );
					if( bIsLastLine ) {
						int pos = sLine.LastIndexOf( "\"};" );
						sLine = sLine.Remove( pos );
					}else {
						//    - иначе последними символами должны являться **",**. Если это так - удаляем их.
						int pos = sLine.LastIndexOf( "\"," );
						m_lastError = String.Format( "Line {0} doesn't ends with '\",'", nCurrLine );
						if( pos < 0 ) break;
						sLine = sLine.Remove( pos );
					}

					// 5. Первая из оставшихся строк - четыре числа, разделенные пробелом. Получаем **ширину**, **высоту**, **количество цветов**, **количество символов в цвете**. Проверяем, что количество символов в цвете == 1. Удаляем строку.
					if( !bParametersLineFound ) {
						bParametersLineFound = true;
						// Разбираем строку параметров
						String[] parameters = sLine.Split(' ');
						m_lastError = String.Format( "Line {0} must containt 4 parameters divided with single space", nCurrLine );
						if( parameters.Length != 4 ) break;

						int nDictionarySize = 0;
						try {
							m_width = Convert.ToInt32( parameters[0] );
							m_height = Convert.ToInt32( parameters[1] );

							m_LevelArray.SetSize( m_width, m_height );

							nDictionarySize = Convert.ToInt32( parameters[2] );
							m_nDictionaryKeySize = Convert.ToInt32( parameters[3] );

						}catch (ApplicationException ex) {
							m_lastError = String.Format( "Error try convert number: {0}", ex.Message );
							break;
						}

						// 6. Проверяем, что **высота** равна количеству оставшихся строк.
						// Будет сделано в конце.
												
						// 7. По следующим **количество цветов** строк составляем словарь:
						int nKeyIndex = 0;
						bool bDictionaryOK = true;
						while (bDictionaryOK && nCurrLine < nLineCount && nKeyIndex++ < nDictionarySize) {
							sLine = allLines [nCurrLine++];

							// Причесываем строку
							{
								// удаляем начальную кавычку
								bDictionaryOK = sLine[0].Equals('"');
								m_lastError = String.Format( "Line {0} must starts with double quote", nCurrLine );
								if( !bDictionaryOK ) break;

								sLine = sLine.Remove(0,1); 

								// удаляем окончательные '",'
								int pos = sLine.LastIndexOf( "\"," );
								bDictionaryOK = 0 < pos;
								m_lastError = String.Format( "Line {0} must ends with '\",'", nCurrLine );
								if( !bDictionaryOK ) break;

								sLine = sLine.Remove( pos );
							}

							//    - первый символ - **код цвета**, дальше **разделитель** (символ табуляции, "c", пробел) и **описание**. Удаляем строку.
							String[] s = sLine.Split( '\t' );

							bDictionaryOK = s.Length == 2;
							m_lastError = String.Format( "The code and the descriptor in line {0} must be devided by TAB and 'c '", nCurrLine );
							if( !bDictionaryOK ) break;

							String key = s[0];
							bDictionaryOK = m_nDictionaryKeySize == key.Length;
							m_lastError = String.Format( "The code in line {0} must contains {1} symbols(s)", nCurrLine, m_nDictionaryKeySize );
							if( !bDictionaryOK ) break;

							String value = s[1];
							bDictionaryOK = value.StartsWith( "c " );
							m_lastError = String.Format( "Symbols 'c ' are not found in line {0} as divider", nCurrLine );
							if( !bDictionaryOK ) break;

							value = value.Remove( 0, 2 );

							bDictionaryOK = !m_Dictionary.ContainsValue( value );
							m_lastError = String.Format( "Duplicate descriptor '{0}' in line {1}", value, nCurrLine );
							if( !bDictionaryOK ) break;

							m_Dictionary.Add( key, value );
						}

						if( !bDictionaryOK ) break;

						continue;
					}

					// 8. Оставшиеся строки представляют собой сам уровень.
					//    - организуем цикл по всем строкам **высота**;
					//    - цикл по всем столбцам **ширина**;
					//    - берем символ [строка][столбец] и сохраняем его во внутренний массив;
					//    - удаляем строку.
					m_lastError = String.Format( "Line {0} expected items count: {1}, found: {2}", nCurrLine, m_width, sLine.Length );
					if( sLine.Length != m_width * m_nDictionaryKeySize ) break;

					bParseSucceeded = true;
					for( int col = 0; col < m_width && bParseSucceeded; ++col ) {
						String symbol = null;
						for( int symbolIndex = 0; symbolIndex < m_nDictionaryKeySize; ++symbolIndex ) {
							symbol += sLine[ symbolIndex + col * m_nDictionaryKeySize ];
						}

						bParseSucceeded = m_Dictionary.ContainsKey( symbol );
						if( !bParseSucceeded ) {
							m_lastError = String.Format( "Line {0}: found unknown symbol in position {1}: '{2}'", nCurrLine, col+1, symbol );
						}
					
						m_LevelArray.SetItem( nLevelLinesCount, col, symbol );
					}

					if( !bParseSucceeded ) break;

					++nLevelLinesCount;

					// 9. Если парсер на каком-либо из шагов алгоритма обнаружил ошибку, внутренний массив обнуляется, и размеры **ширина** и **высота** устанавливаются в ноль. 
					// 10. По требованию извне (публичный метод) по указанной ячейке [строка][столбец] выдаем соответствующее хранимому символу с этими координатами **описание** текстовой строкой.
					// 11. По требованию извне (публичный метод) предоставляется результат проверки корректности парсинга уровня: (0 < **ширина** * **высота**).

					// Если мы дошли до конца, значит ошибки в текущей строке не было
					// Если был обнаружен признак последней строки, то выходим из цикла строк
					if( bIsLastLine ) {
						// Все хорошо лишь тогда, когда совпало число строк уровня с высотой уровня, указанной в параметрах
						bParseSucceeded = nLevelLinesCount == m_height;
						break;
					}
				} // while (nCurrLine < nLineCount)

				// Выход из цикла обработки строк может быть как корректным, так и по ошибке.
				// Если вышли по ошибке, то выходим из синтетического цикла
				if( bParseSucceeded ) {
					// Иначе - все хорошо
					m_lastError = @"Loaded OK";
					return true;
				}

			}while (false);

			// Раз вышли из синтетического цикла - значит все плохо
			Clear();

			return false;
		}// LoadFromFile()

		/// <summary>
		/// Указывает на успешность загрузки уровня
		/// </summary>
		/// <returns>
		/// <c>true</c> если загрузка была успешной; иначе - <c>false</c>.
		/// </returns>
		public bool IsLoaded ()
		{
			return 0 < m_width * m_height;
		}

		/// <summary>
		/// Возвращает ширину уровня
		/// </summary>
		/// <value>
		/// Ширина уровня
		/// </value>
		public int Width {
			get {
				return m_width;
			}
		}

		/// <summary>
		/// Возвращает высоту уровня
		/// </summary>
		/// <value>
		/// Высота уровня
		/// </value>
		public int Height {
			get {
				return m_height;
			}
		}

		/// <summary>
		/// Возвращает описатель для указанной ячейки уровня
		/// </summary>
		/// <param name='row'>
		/// Номер строки уровня
		/// </param>
		/// <param name='col'>
		/// Номер столбца уровня
		/// </param>
		public String Item (int row, int col)
		{
			if (row < m_height && col < m_width) {
				String code = m_LevelArray.Item (row, col);
				return m_Dictionary[code];
			}

			return null;
		}

		/// <summary>
		/// Возвращает найденную ошибку парсинга файла или null в случае, если парсинга не было
		/// </summary>
		/// <value>
		/// Сообщение об ошибке парсинга
		/// </value>
		public String LastError {
			get {
				return m_lastError;
			}
		}

		/// <summary>
		/// Возвращает имя последнего файла, для которого была осуществлена попытка загрузки
		/// </summary>
		/// <value>
		/// Имя файла
		/// </value>
		public String FileName {
			get {
				return m_fileName;
			}
		}

		private int m_width;
		private int m_height;
		private int m_nDictionaryKeySize;
		private StringDictionary m_Dictionary; // Код - описание
		private DoubleArray<String> m_LevelArray; // Двумерный массив кодов
		private String m_lastError;
		private String m_fileName;

		/// <summary>
		/// Очистка парсера уровня
		/// </summary>
		private void Clear ()
		{
			m_width = 0;
			m_height = 0;
			if (null != m_Dictionary) {
				m_Dictionary.Clear ();
			}
			if (null != m_LevelArray) {
				m_LevelArray.Clear ();
			}
			m_nDictionaryKeySize = 1;
		}


	}// class GameLevelParser
}// namespace ClassesLibrary

