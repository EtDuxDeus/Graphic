using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace graphic
{
	class Parser
	{
		static public double Calculate(string input, double x)//"входной" метод класса
		{
			input = input.Replace(" ", "");
			Regex regEx = new Regex(@"(?<=[\d\)])(?=[a-df-z\(])|(?<=pi)(?=[^\+\-\*\/\\^!)])|(?<=\))(?=\d)|(?<=[^\/\*\+\-])(?=exp)", RegexOptions.IgnoreCase);
			input = regEx.Replace(input, "*");
			regEx = new Regex("x", RegexOptions.IgnoreCase);
			input = regEx.Replace(input, x.ToString());
			regEx = new Regex("pi", RegexOptions.IgnoreCase);
			input = regEx.Replace(input, Math.PI.ToString());
			regEx = new Regex("sin", RegexOptions.IgnoreCase);
			input = regEx.Replace(input, "c");
			regEx = new Regex("cos", RegexOptions.IgnoreCase);
			input = regEx.Replace(input, "k");

			string output = GetExpression(input);//преобразование выражения в постфиксную запись
			double result = Counting(output);//решение выражения
			return result;

		}



		static public string GetExpression(string input)//метод перевода выражения в постфиксную запись
		{
			string output = string.Empty; //Строка для хранения выражения
			Stack<char> operStack = new Stack<char>(); //Стек для хранения операторов

			for (int i = 0; i < input.Length; i++) //Для каждого символа в входной строке
			{

				//Разделители пропускаем
				if (IsDelimeter(input[i]))
					continue; //Переходим к следующему символу




				//проверка на отрицательное число: если знак "-" в начале строки или перед знаком "-" нет числа 
				if (input[i] == '-' && ((i > 0 && !Char.IsDigit(input[i - 1])) || i == 0))
				{
					i++;
					output += "-";//в переменную для чисел добавляется знак "-"    
				}


				//Если символ - цифра, то считываем все число
				if (Char.IsDigit(input[i])) //Если цифра
				{
					//Читаем до разделителя или оператора, что бы получить число
					while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
					{
						output += input[i]; //Добавляем каждую цифру числа к нашей строке
						i++; //Переходим к следующему символу

						if (i == input.Length) break; //Если символ - последний, то выходим из цикла
					}

					output += " "; //Дописываем после числа пробел в строку с выражением
					i--; //Возвращаемся на один символ назад, к символу перед разделителем
				}

				//Если символ - оператор
				if (IsOperator(input[i])) //Если оператор
				{

					if (input[i] == '(') //Если символ - открывающая скобка
						operStack.Push(input[i]); //Записываем её в стек
					else if (input[i] == ')') //Если символ - закрывающая скобка
					{
						//Выписываем все операторы до открывающей скобки в строку
						char s = operStack.Pop();

						while (s != '(')
						{
							output += s.ToString() + ' ';

							s = operStack.Pop();
						}
					}
					else //Если любой другой оператор
					{
						if (operStack.Count > 0) //Если в стеке есть элементы
							if (GetPriority(input[i]) <= GetPriority(operStack.Peek())) //И если приоритет нашего оператора меньше или равен приоритету оператора на вершине стека
								output += operStack.Pop().ToString() + " "; //То добавляем последний оператор из стека в строку с выражением

						operStack.Push(char.Parse(input[i].ToString())); //Если стек пуст, или же приоритет оператора выше - добавляем операторов на вершину стека

					}
				}
			}

			//Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
			while (operStack.Count > 0)
				output += operStack.Pop() + " ";

			return output; //Возвращаем выражение в постфиксной записи
		}
		static private double Counting(string output)//метод решения OPN
		{
			string result;

			string[] mas = output.Split(' ');

			for (int i = 0; i < mas.Length; i++)

				switch (mas[i])
				{
					case "+"://если найдена операция сложения
						result = (double.Parse(mas[i - 2]) + double.Parse(mas[i - 1])).ToString();//выполняем сложение и переводим ее в строку
						mas[i - 2] = result;//на место 1-ого операнда записывается результат (как если бы a=a+b)
						for (int j = i - 1; j < mas.Length - 2; j++)//удаляем из массива второй операнд и знак арифм действия
							mas[j] = mas[j + 2];
						Array.Resize(ref mas, mas.Length - 2);//обрезаем массив элементов на 2 удаленнх элемента
						i -= 2;
						break;


					case "-"://далее все аналогично
						result = (double.Parse(mas[i - 2]) - double.Parse(mas[i - 1])).ToString();
						mas[i - 2] = result;
						for (int j = i - 1; j < mas.Length - 2; j++)
							mas[j] = mas[j + 2];
						Array.Resize(ref mas, mas.Length - 2);
						i -= 2;
						break;

					case "*":
						result = (double.Parse(mas[i - 2]) * double.Parse(mas[i - 1])).ToString();
						mas[i - 2] = result;
						for (int j = i - 1; j < mas.Length - 2; j++)
							mas[j] = mas[j + 2];
						Array.Resize(ref mas, mas.Length - 2);
						i -= 2;
						break;

					case "/":
						result = (double.Parse(mas[i - 2]) / double.Parse(mas[i - 1])).ToString();
						mas[i - 2] = result;
						for (int j = i - 1; j < mas.Length - 2; j++)
							mas[j] = mas[j + 2];
						Array.Resize(ref mas, mas.Length - 2);
						i -= 2;
						break;


					case "^":
						result = (Math.Pow(double.Parse(mas[i - 2]), double.Parse(mas[i - 1]))).ToString();
						mas[i - 2] = result;
						for (int j = i - 1; j < mas.Length - 2; j++)
							mas[j] = mas[j + 2];
						Array.Resize(ref mas, mas.Length - 2);
						i -= 2;
						break;
					case "c":
						result = (Math.Sin(double.Parse(mas[i - 1])).ToString());
						mas[i - 1] = result;
						for (int j = i; j < mas.Length - 1; j++)
							mas[j] = mas[j + 1];
						Array.Resize(ref mas, mas.Length - 1);
						i -= 1;
						break;

					case "k":
						result = (Math.Cos(double.Parse(mas[i - 1])).ToString());
						mas[i - 1] = result;
						for (int j = i; j < mas.Length - 1; j++)
							mas[j] = mas[j + 1];
						Array.Resize(ref mas, mas.Length - 1);
						i -= 1;
						break;
						


				}
			return double.Parse(mas[0]);
		}

		//Метод возвращает приоритет оператора
		static private byte GetPriority(char s)
		{
			switch (s)
			{
				case '(': return 0;
				case ')': return 1;
				case '+': return 2;
				case '-': return 3;
				case '*': return 4;
				case '/': return 4;
				case '^': return 5;
				case 'c': return 6;
				case 'k': return 6;
				default: return 7;
			}
		}
		//Метод возвращает true, если проверяемый символ - оператор
		static private bool IsOperator(char с)
		{
			if (("+-/*^()ck".IndexOf(с) != -1))
				return true;
			return false;
		}

		//Метод возвращает true, если проверяемый символ - разделитель ("пробел" или "равно")
		static private bool IsDelimeter(char c)
		{
			if ((" =".IndexOf(c) != -1))
				return true;
			return false;
		}
	}
}
//класс исключающий для ошибок для анализатора
class ParserException : ApplicationException
{
	public ParserException(string str) : base(str) { }
	public override string ToString()
	{ return Message; }
}

class Parser
{
	//перчисляем типы лексем.
	enum Types { NONE, DELIMITER, VARIABLE, NUMBER };
	// Перечисляем типы ошибок.
	enum Errors { SYNTAX, UNBALPARENS, NOEXP, DIVBYZERO };

	string exp; // Ссылка на строку выражения,
	int expIdx; // Текущий индекс в выражении,
	string token; // Текущая лексема.
	Types tokType; // Тип лексемы.

	// Массив для переменных,
	double[] vars = new double[26];

	public Parser()
	{
		// Инициализируем переменные нулевыми значениями.
		for (int i = 0; i < vars.Length; i++)
			vars[i] = 0.0;
	}
	// Входная точка анализатора.
	public double Evaluate(string expstr)
	{
		double result;
		exp = expstr;
		expIdx = 0;
		try
		{
			GetToken();
			if (token == "")
			{
				SyntaxErr(Errors.NOEXP); // Выражение отсутствует,
				return 0.0;
			}
			//  EvalExp1(out result); // В этом варианте анализатора
			// сначала вызывается
			// метод EvalExpl().
			EvalExp2(out result);
			if (token != "") // Последняя лексема должна
							 // быть нулевой.
				SyntaxErr(Errors.SYNTAX);
			return result;
		}
		catch (ParserException exc)
		{
			// При желании добавляем здесь обработку ошибок.
			Console.WriteLine(exc);
			return 0.0;
		}
	}

	// Обрабатываем присвоение,
	/* void EvalExp1(out double result)
	 {
		 int varIdx;
		 Types ttokType;
		 string temptoken;
		 if (tokType == Types.VARIABLE)
		 {
			 // Сохраняем старую лексему,
			 temptoken = String.Copy(token);
			 ttokType = tokType;
			 // Вычисляем индекс переменной,
			 varIdx = Char.ToUpper(token[0]) - 'A';
			 GetToken();
			 if (token != "=")
			 {
				 PutBack();// Возвращаем текущую лексему в поток
				 //и восстанавливаем старую,
				 // поскольку отсутствует присвоение.
				 token = String.Copy(temptoken);
				 tokType = ttokType;
			 }
			 else
			 {
				 GetToken();// Получаем следующую часть
				 // выражения ехр.
				 EvalExp2(out result);
				 vars[varIdx] = result;
				 return;
			 }
		 }

		 EvalExp2(out result);
	 }*/

	// Складываем или вычитаем два члена выражения.
	void EvalExp2(out double result)
	{
		string op;
		double partialResult;

		EvalExp3(out result);
		while ((op = token) == "+" || op == "-")
		{
			GetToken();
			EvalExp3(out partialResult);
			switch (op)
			{
				case "-":
					result = result - partialResult;
					break;
				case "+":
					result = result + partialResult;
					break;
			}
		}
	}

	// Выполняем умножение или деление двух множителей.
	void EvalExp3(out double result)
	{
		string op;
		double partialResult = 0.0;
		EvalExp4(out result);
		while ((op = token) == "*" || op == "/" || op == "%")
		{
			GetToken();
			EvalExp4(out partialResult);
			switch (op)
			{
				case "*":
					result = result * partialResult;
					break;
				case "/":
					if (partialResult == 0.0)
						SyntaxErr(Errors.DIVBYZERO);
					result = result / partialResult;
					break;
				case "%":
					if (partialResult == 0.0)
						SyntaxErr(Errors.DIVBYZERO);
					result = (int)result % (int)partialResult;
					break;
			}
		}
	}

	// выполняем возведение в степень
	void EvalExp4(out double result)
	{
		double partialResult, ex;
		int t;
		EvalExp5(out result);
		if (token == "^")
		{
			GetToken();
			EvalExp4(out partialResult);
			ex = result;
			if (partialResult == 0.0)
			{
				result = 1.0;
				return;
			}
			for (t = (int)partialResult - 1; t > 0; t--)
				result = result * (double)ex;
		}
	}
	// Выполненяем операцию унарного + или -.
	void EvalExp5(out double result)
	{
		string op;

		op = "";
		if ((tokType == Types.DELIMITER) && token == "+" || token == "-")
		{
			op = token;
			GetToken();
		}
		EvalExp6(out result);
		if (op == "-") result = -result;
	}

	// обрабатываем выражение в круглых скобках
	void EvalExp6(out double result)
	{
		if ((token == "("))
		{
			GetToken();
			EvalExp2(out result);
			if (token != ")")
				SyntaxErr(Errors.UNBALPARENS);
			GetToken();
		}
		else Atom(out result);
	}
	// Получаем значение числа или переменной.
	void Atom(out double result)
	{
		switch (tokType)
		{
			case Types.NUMBER:
				try
				{
					result = Double.Parse(token);
				}
				catch (FormatException)
				{
					result = 0.0;
					SyntaxErr(Errors.SYNTAX);
				}
				GetToken();
				return;
			case Types.VARIABLE:
				result = FindVar(token);
				GetToken();
				return;
			default:
				result = 0.0;
				SyntaxErr(Errors.SYNTAX);
				break;
		}
	}
	// Возвращаем значение переменной.
	double FindVar(string vname)
	{
		if (!Char.IsLetter(vname[0]))
		{
			SyntaxErr(Errors.SYNTAX);
			return 0.0;
		}
		return vars[Char.ToUpper(vname[0]) - 'A'];
	}
	// Возвращаем лексему во входной поток.
	void PutBack()
	{
		for (int i = 0; i < token.Length; i++) expIdx--;
	}
	// Обрабатываем синтаксическую ошибку
	void SyntaxErr(Errors error)
	{
		string[] err ={
						 "Синтаксическая оошибка",
						 "Дисбаланс скобок",
						 "Выражение отсутствет",
						 "Деление на ноль"};
		throw new ParserException(err[(int)error]);
	}
	// получем следующую лексему.
	void GetToken()
	{
		tokType = Types.NONE;
		token = "";
		if (expIdx == exp.Length) return; // Конец выражения.
										  // Опускаем пробел.
		while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;
		// Хвостовой пробел завершает выражение.
		if (expIdx == exp.Length) return;
		if (IsDelim(exp[expIdx]))
		{
			token += exp[expIdx];
			expIdx++;
			tokType = Types.DELIMITER;
		}
		else if (Char.IsLetter(exp[expIdx]))
		{
			// Это переменная?
			while (!IsDelim(exp[expIdx]))
			{
				token += exp[expIdx];
				expIdx++;
				if (expIdx >= exp.Length) break;
			}
			tokType = Types.VARIABLE;
		}
		else if (Char.IsDigit(exp[expIdx]))
		{
			// Это число?
			while (!IsDelim(exp[expIdx]))
			{
				token += exp[expIdx];
				expIdx++;
				if (expIdx >= exp.Length) break;
			}
			tokType = Types.NUMBER;
		}
	}
	// Метод возвращает значение true,
	// если с -- разделитель.
	bool IsDelim(char c)
	{
		if (("+-/*%^=()".IndexOf(c) != -1))
			return true;
		return false;
	}
}
