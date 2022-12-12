using System;
using System.Collections.Generic;
using ConsoleTables;

namespace Interpolation
{
    internal class Program
    {
        static void Main()
        {
            double a = 1;
            double b = 2.5; //верхня та нижні границі
            double x = a, fx, lx, locx;
            double h = 0.1; //крок
            int digits = 7; //числа після коми

             double[] x1 = {1,1.23,1.37,1.44,1.5,1.57,1.63,1.76,1.79,1.86,2.16,2.27,2.35,2.4,2.55};
             double[] y1 = {0.94630,0.47828,-0.00531,-0.26867,-0.48679,-0.71268,-0.86508,-0.99898,-0.98354,-0.87410,0.42812,0.86226,0.99668,0.97885,0.45492};
            

            List<double> locals = new List<double>(); //список значень, отриманих з локальної інтерполяції
            var table = new ConsoleTable("X", "f(x)", "L(x)", "Loc(x)");
            var table1 = new ConsoleTable("X", "f’(x)", "f’aprx");
            var table2 = new ConsoleTable("X", "f’’(x)", "f’’aprx");
            for (; x <= b + h; x += h)
            {
                fx = Func(x, digits);
                lx = Lagr(x1, y1, x, digits);
                locx = LocalInterpolation(x1, y1, x, digits);
                locals.Add(locx);
                table.AddRow(x, fx, lx, locx);
            }
            table.Write(Format.Alternative);
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("\n \n \t \t Differentiation \n \n");

            x = a;
            for (int i = 0; x <= b + h; x += h, i++)
            {
                double dif1 = Diff1(x, digits);
                double dif2 = Diff2(x, digits);

                DiffAprx(locals, i, h, out var dif1Aprx, out var dif2Aprx, x1.Length);

                table1.AddRow(x, dif1, Math.Round(dif1Aprx, digits));
                table2.AddRow(x, dif2, Math.Round(dif2Aprx, digits));
            }
            table1.Write(Format.Alternative);
            table2.Write(Format.Alternative);

        }

        static void DiffAprx(List<double> locals, int i, double h, out double dif1Aprx, out double dif2Aprx, int lenghth) // перша та друга похідні, наближено
        {
            if (i == 0)
            {
                dif1Aprx = (locals[1] - locals[i]) / h;
                dif2Aprx = (locals[2] - 2 * locals[1] + locals[i]) / (h * h);
            }
            else if (i == lenghth)
            {
                dif1Aprx = (locals[i] - locals[i - 1]) / h;
                dif2Aprx = (locals[i] - 2 * locals[i - 1] + locals[i - 2]) / (h * h);
            }
            else
            {
                dif1Aprx = (locals[i + 1] - locals[i - 1]) / (2 * h);
                dif2Aprx = (locals[i + 1] - 2 * locals[i] + locals[i - 1]) / (h * h);
            }
        }

        static int FindIndexForParabola(double[] x1, double[] y1, double x)
        {
            int index = -1;
            for (int i = 1; i < y1.Length; i++) //Знаходження двох точок, між якими лежить х
            {
                if (x >= x1[i - 1] && x <= x1[i])
                {
                    index = i;
                    break;
                }
            }
            if (index >= x1.Length - 1) //перевірка на крайню праву параболу
            {
                return x1.Length - 2;
            }
            if (index >= 2) //перевірка на крайню ліву параболу
            {
                if (x1[index - 1] - x1[index - 2] < x1[index + 1] - x1[index]) //Знаходження третьї точки, до якої відстань найкоротша
                {
                    return index - 1;
                }
            }
            //Тоді беремо параболу по трьом точкам : (index - 1), (index), (index + 1)
            return index;
        }

        static double LocalInterpolation(double[] x1, double[] y1, double x, int digits) //локальна інтерполяція 
        {
            int i = FindIndexForParabola(x1, y1, x);
            double[] indexes = new double[3];
            indexes[2] = ((y1[i + 1] - y1[i - 1]) / ((x1[i + 1] - x1[i - 1]) * (x1[i + 1] - x1[i]))) -
                         ((y1[i] - y1[i - 1]) / ((x1[i] - x1[i - 1]) * (x1[i + 1] - x1[i])));

            indexes[1] = (y1[i] - y1[i - 1]) / (x1[i] - x1[i - 1]) - indexes[2] * (x1[i] + x1[i - 1]);

            indexes[0] = y1[i - 1] - indexes[1] * x1[i - 1] - indexes[2] * x1[i - 1] * x1[i - 1];

            double localX = indexes[0] + indexes[1] * x + indexes[2] * x * x; // значення ф(х) на параболі
            localX = Math.Round(localX, digits);

            return localX;
        }



        static double Func(double x, int digits) //sin(x)/x
        {
            return Math.Round(Math.Sin(x) / x, digits);
        }


        static double Lagr(double[] x1, double[] y1, double xF, int digits) //Многочлен лагранжа 
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++) //Обчислення сумми 
            {
                double li = 0;

                for (int j = 0; j < x1.Length; j++) //Обчислення добутку 
                {
                    if (j != i)
                    {
                        if (li == 0) li = (xF - x1[j]) / (x1[i] - x1[j]);
                        else li *= (xF - x1[j]) / (x1[i] - x1[j]);
                    }
                }
                li *= y1[i];
                sum += li;
            }

            return Math.Round(sum, digits);
        }

        static double Diff1(double x, int digits)
        {
            x = (x * Math.Cos(x) - Math.Sin(x)) / x * x;
            return Math.Round(x, digits);
        }

        static double Diff2(double x, int digits)
        {
            x = ((-1 * Math.Sin(x)) - (2 * Math.Cos(x) / x) + (2 * Math.Sin(x) / x * x)) / x;
            return Math.Round(x, digits);
        }
    }
}




    