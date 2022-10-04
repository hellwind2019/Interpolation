using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;

namespace Interpolation
{
    internal class Program
    {
        static void Main()
        {
            double a = 1;
            double b = 2.5;
            double x = a, fx, lx, locx;
            double h = 0.1;
            double[] x1 = {1,1.23,1.37,1.44,1.5,1.57,1.63,1.76,1.79,1.86,2.16,2.27,2.35,2.4,2.55};
            double[] y1= {0.94630,0.47828,-0.00531,-0.26867,-0.48679,-0.71268,-0.86508,-0.99898,-0.98354,
                         -0.87410,0.42812,0.86226,0.99668,0.97885,0.45492};
//1 1,23 1,37 1,44 1,5 1,57 1,63 1,76 1,79 1,86 2,16 2,27 2,35 2,4 2,55
//0,94630 0,47828 -0,00531 -0,26867 -0,48679 -0,71268 -0,86508 -0,99898 -0,98354 -0,87410 0,42812 0,86226 0,99668 0,97885 0,45492
          
            Console.WriteLine($"|    X   ||     f(x)      ||      L(x)     ||     Loc(x)    |");
            Console.WriteLine($"|--------||---------------||---------------||---------------|");
            for (; x <= b + h; x += h )
            {
                fx = Func(x);
                lx = Lagr(x1, y1, x);
                int index = FindDots(x1, y1, x);
                indexesOfParabola(x1, y1, x, index, out locx);
                TableOut(x, fx, lx, locx);
            }
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine(FindDots(x1,y1, 2.30));
        }

        static int FindDots(double[] x1, double[] y1, double x)
        {
            int index = -1;
            for (int i = 1; i < y1.Length; i++) //Знаходження двох точок, між якими лежить х
            {
                if (x >= x1[i-1] && x <= x1[i])
                {
                    index = i;
                    break;
                }
            }

            if (index >= x1.Length -1) //крайня права парабола
            {
                return x1.Length - 1;
            }
            if (index >= 2) //крайня ліва парабола
            {
                if (x1[index-1] - x1[index - 2] < x1[index + 1] - x1[index]) //Знаходження третьї точки, до якої відстань найкоротша
                {
                   return index-1;
                }
            }
            //Тоді беремо параболу по трьом точкам : (index - 1), (index), (index + 1)
            return index;
        }
        
        static void TableOut(double x, double fx, double lx, double locx)
        {
            Console.Write($"|  {x:F2}  |");
            if (fx>0) Console.Write($"|   {fx:F8}  |");
            else      Console.Write($"|  {fx:F8}  |");
            if (lx>0) Console.Write($"|   {lx:F8}  |");
            else      Console.Write($"|  {lx:F8}  |");
            if (locx>0) Console.Write($"|   {locx:F8}  |");
            else      Console.Write($"|  {locx:F8}  |");
            Console.WriteLine();
        }

        static double[] indexesOfParabola(double[] x1, double[] y1, double x, int i, out double LocalX)
        {
            double[] indexes = new double[3];
            if (i >= x1.Length -1)
            {
                i = x1.Length - 2;  //крайня права парабола
            }
            indexes[2] = ((y1[i + 1] - y1[i - 1]) / ((x1[i + 1] - x1[i - 1]) * (x1[i + 1] - x1[i]))) -
                         ((y1[i] - y1[i - 1]) / ((x1[i] - x1[i - 1]) * (x1[i + 1] - x1[i])));
            
            indexes[1] = (y1[i] - y1[i - 1]) / (x1[i] - x1[i - 1]) - indexes[2] * (x1[i] + x1[i - 1]);

            indexes[0] = y1[i - 1] - indexes[1] * x1[i - 1] - indexes[2] * x1[i - 1] * x1[i - 1];
            
            LocalX = indexes[0] + indexes[1] * x + indexes[2] * x * x; // значення ф(х) на параболі
            
            return indexes;
        }

        static double LocalInterpol(double[] x1, double[] y1, double x, double[] indexes)
        {
            double y = indexes[0] * x * x + indexes[1] * x + indexes[2];
            return y;
        }
        

        static double Func(double x)
        {
            return Math.Sin(x * x + x - 0.1);
        }
        

        static double Lagr(double[] x1, double[] y1, double xF) //Многочлен лагранжа 
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++) //Обчислення сумми 
            {
                double Li = 0; 
               
                for (int j = 0; j < x1.Length; j++) //Обчислення добутку 
                {
                    if (j != i)
                    {
                        if (Li == 0) Li = (xF - x1[j]) / (x1[i] - x1[j]);
                        else Li *= (xF - x1[j]) / (x1[i] - x1[j]);
                    }
                }
                Li *= y1[i];
                sum += Li;
            }

            return sum;
        }
    }
}
    