using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
 
namespace CalcTrans
{
    static class Program
    {
        static public double Main(string Expression, double x)
        {
            string expr = Expression;
            Parser p = new Parser();
			Regex regEx = new Regex("x", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, x.ToString());
			double y = p.Evaluate(expr);
			return y;
        }
       class ParserException : ApplicationException
		{
			public ParserException(string str) : base(str) { }

			public override string ToString()
			{
				return Message;
			}
		}

		class Parser
		{
			//Типы лексем
			enum Types { NONE, DELIMITER, VARIABLE, NUMBER};

			//Типы ошибок.
			enum Errors { SYNTAX, UNBALPARENS, NOEXP, DIVBYZERO };

			string exp; // ссылка на строку выражения.
			int expIdx;//текущий индекс в выражении.
			string token; // текущая лексема.
			Types tokType; //тип лексемы.

			//Входная точка анализатора.
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
						SyntaxErr(Errors.NOEXP);//выражение отсутствует

						return 0.0;
					}
					EvalExp2(out result);
					if (token != "")//последняя лексема должна быть null-значением.
						SyntaxErr(Errors.SYNTAX);
					return result;
				}
				catch (ParserException exc)
				{
					return 0.0;
				}
			}
			//сложение или вычитание двух членов выражения.
			void EvalExp2(out double result)
			{
				string op;
				double partialResult;
				EvalExp3(out result);
				while((op = token) == "+" || op == "-")
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
			//умножение или деление двух множителей
			void EvalExp3(out double result)
			{
				string op;
				double partialResult = 0.0;
				EvalExp4(out result);
				while((op=token) == "*")
				{
					GetToken();
					EvalExp4(out partialResult);
					switch(op)
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
			//возведение в степень.
			void EvalExp4(out double result)
			{
				double partialResult, ex;
				int t;
				EvalExp5(out result);
				if(token == "^")
				{
					GetToken();
					EvalExp4(out partialResult);
					ex = result;
					if(partialResult == 0.0)
					{
						result = 1.0;
						return;
					}
					for (t = (int)partialResult - 1; t > 0; t--)
						result = result * (double)ex;
				}
			}
			//Выполнение операции унарного + или -.
			void EvalExp5(out double result)
			{
				string op;
				op = "";
				if((tokType==Types.DELIMITER)&& token == "+"|| token == "-")
				{
					op = token;
					GetToken();
				}
				EvalExp6(out result);
				if (op == "-") result = -result;
			}
			//Обработка выражения в круглых скобках.
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
			void Atom(out double result)
			{
				switch (tokType)
				{
					case Types.NUMBER:
						try
						{
							result = Double.Parse(token);
						}
						catch(FormatException)
							{
							result = 0.0;
							SyntaxErr(Errors.SYNTAX);
						}
						GetToken();
						return;
					default:
						result = 0.0;
						SyntaxErr(Errors.SYNTAX);
						break;
				}
			}
			//обрабатываем синтаксическую ошибку.
			void SyntaxErr(Errors error)
			{
				string[] err =
				{
					"Синтаксическая ошибка",
					"Дисбаланс скобок",
					"Выражение отсутствует",
					"Деление на ноль"
				};
				throw new ParserException(err[(int)error]);
			}
			//Получаем следующую лексему.
			void GetToken()
			{
				tokType = Types.NONE;
				token = "";
				if (expIdx == exp.Length) return;
				while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;
				//хвостовой пробел завершает выражение.
				if (expIdx == exp.Length) return;
				if (IsDelim(exp[expIdx]))
				{
					token += exp[expIdx];
					expIdx++;
					tokType = Types.DELIMITER;
				}
				else if (Char.IsLetter(exp[expIdx]))
				{
					while (!IsDelim(exp[expIdx]))
					{
						token += exp[expIdx];
						expIdx++;
						if (expIdx >= exp.Length)
							break;
					}
					tokType = Types.VARIABLE;
				}
				else if (Char.IsDigit(exp[expIdx]))
				{
					while (!IsDelim(exp[expIdx]))
					{
						token += exp[expIdx];
						expIdx++;
						if (expIdx >= exp.Length)
							break;
					}
					tokType = Types.NUMBER;
				}
			}
			//true если символ - разделитель
			bool IsDelim(char c)
			{
				if (("+-/*%^".IndexOf(c) != -1)) return true;
				return false;
			}
		}
			
    }
}