﻿namespace InfiniteConvergentSeries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class ICSCalculator
    {
        private static double precision = 0.0;
        private static int digitsToUse = 0;

        private static List<string> functionElements = new List<string>();
        private static List<char> functionSighns = new List<char>();

        private static double answer = 0;
        private static double prevAnswer = 0;

        private static bool useOneSide = false;

        private static bool factorielUP = false;
        private static bool factorielDown = false;

        private static double elementToUseNextUP = 0;
        private static double elementToUseNextDown = 0;

        private static double elementCreatorUp = 0;
        private static double elementCreatorDown = 0;

        private static int counterToShowAsInformation = 0;

        private static Action ICSOperations;

        private static Func<double, double, double> getNextElementUP;
        private static Func<double, double, double> getNextElementDown;

        public static void Main()
        {
            ReadInput();

            PutFirstFunctionElementAsAnswer();

            InicializeDelegateWithProperActions();

            GenerateNextElementGenerator();

            GenerateFirstElementsToUse();

            Console.WriteLine("--------------");

            Console.WriteLine("Element {0} is {1} ", ++counterToShowAsInformation, answer);

            while (Math.Abs(answer - prevAnswer) > precision)
            {
                ICSOperations();
                if (counterToShowAsInformation >= 100000)
                {
                    Console.WriteLine("This series is not convergent !");
                    return;
                }
            }

            Console.WriteLine("--------------");
            Console.WriteLine("Final answer : ");
            Console.WriteLine(Math.Round(answer, digitsToUse));
        }

        private static double Sum(double one, double two)
        {
            return one + two;
        }

        private static double Multiplication(double one, double two)
        {
            return one * two;
        }

        private static double GetFactoriel(double number)
        {
            double answer = 1;
            for (int i = 1; i <= number; i++)
            {
                answer *= i;
            }

            return answer;
        }

        private static void VoidSum()
        {
            prevAnswer = answer;

            double createElement = CreateNextElement();

            Console.WriteLine("Element {0} is {1} ", ++counterToShowAsInformation, createElement);

            answer = prevAnswer + createElement;
        }

        private static void VoidSubstraction()
        {
            prevAnswer = answer;

            double createElement = CreateNextElement();

            Console.WriteLine("Element {0} is {1} ", ++counterToShowAsInformation, createElement);

            answer = prevAnswer - createElement;
        }

        private static double CreateNextElement()
        {
            double createElement = elementToUseNextUP;
            if (factorielUP)
            {
                createElement = GetFactoriel(createElement);
            }

            if (!useOneSide)
            {
                double botElement = elementToUseNextDown;

                if (factorielDown)
                {
                    botElement = GetFactoriel(botElement);
                }

                createElement = createElement / botElement;

                elementToUseNextDown = getNextElementDown(elementToUseNextDown, elementCreatorDown);
            }

            elementToUseNextUP = getNextElementUP(elementToUseNextUP, elementCreatorUp);

            return createElement;
        }

        private static void GenerateFirstElementsToUse()
        {
            if (!useOneSide)
            {
                string[] upDown = functionElements[0].Split('/');

                elementToUseNextUP = double.Parse(upDown[0].TrimEnd('!'));
                elementToUseNextDown = double.Parse(upDown[1].TrimEnd('!'));
            }
            else
            {
                elementToUseNextUP = double.Parse(functionElements[0].TrimEnd('!'));
            }
        }

        private static double GetElementFromStringOperations(string stringOperations)
        {
            double answerToReturn = 0;

            if (stringOperations.Contains('/'))
            {
                string[] upDown = stringOperations.Split('/');
                double up = 0;
                double down = 0;

                if (upDown[0].EndsWith("!"))
                {
                    up = GetFactoriel(double.Parse(upDown[0].TrimEnd('!')));
                }
                else
                {
                    up = double.Parse(upDown[0]);
                }

                if (upDown[1].EndsWith("!"))
                {
                    down = GetFactoriel(double.Parse(upDown[1].TrimEnd('!')));
                }
                else
                {
                    down = double.Parse(upDown[1]);
                }

                answerToReturn = up / down;
            }
            else
            {
                if (stringOperations.EndsWith("!"))
                {
                    answerToReturn = GetFactoriel(Math.Abs(double.Parse(stringOperations.TrimEnd('!'))));
                }
                else
                {
                    answerToReturn = Math.Abs(double.Parse(stringOperations));
                }
            }

            return answerToReturn;
        }

        private static void PutFirstFunctionElementAsAnswer()
        {
            answer = GetElementFromStringOperations(functionElements[0]);

            functionElements.RemoveAt(0);

            if (functionSighns.Count == 5)
            {
                if (functionSighns[0] == '-')
                {
                    answer *= -1;
                }

                functionSighns.RemoveAt(0);
            }
        }

        private static void GenerateNextElementGenerator()
        {
            int countDivisionSighns = functionElements.Count(x => x.Contains('/'));

            if (countDivisionSighns == 0)
            {
                GenerateNextElementGeneratorWithNoDivision();
            }
            else if (countDivisionSighns == 4)
            {
                GenerateNextElementGeneratorWithAllDivision();
            }
            else
            {
                throw new ArgumentException("Can not find a logical pattern");
            }
        }

        private static void GenerateNextElementGeneratorWithAllDivision()
        {
            double[] upperElements = new double[4];
            double[] bottomElements = new double[4];

            for (int i = 0; i < 4; i++)
            {
                string[] upDown = functionElements[i].Split('/');

                if (upDown[0].EndsWith("!"))
                {
                    factorielUP = true;
                    upDown[0] = upDown[0].TrimEnd('!');
                }

                if (upDown[1].EndsWith("!"))
                {
                    factorielDown = true;
                    upDown[1] = upDown[1].TrimEnd('!');
                }

                upperElements[i] = double.Parse(upDown[0]);
                bottomElements[i] = double.Parse(upDown[1]);
            }

            if (upperElements[1] - upperElements[0] == upperElements[2] - upperElements[1] &&
                upperElements[3] - upperElements[2] == upperElements[2] - upperElements[1])
            {
                elementCreatorUp = upperElements[1] - upperElements[0];
                getNextElementUP = Sum;
            }
            else if (upperElements[1] / upperElements[0] == upperElements[2] / upperElements[1] &&
                     upperElements[3] / upperElements[2] == upperElements[2] / upperElements[1])
            {
                elementCreatorUp = upperElements[1] / upperElements[0];
                getNextElementUP = Multiplication;
            }
            else
            {
                throw new ArgumentException("Can not find suitable pattern!");
            }

            if (bottomElements[1] - bottomElements[0] == bottomElements[2] - bottomElements[1] &&
                bottomElements[3] - bottomElements[2] == bottomElements[2] - bottomElements[1])
            {
                elementCreatorDown = bottomElements[1] - bottomElements[0];
                getNextElementDown = Sum;
            }
            else if (bottomElements[1] / bottomElements[0] == bottomElements[2] / bottomElements[1] &&
                     bottomElements[3] / bottomElements[2] == bottomElements[2] / bottomElements[1])
            {
                elementCreatorDown = bottomElements[1] / bottomElements[0];
                getNextElementDown = Multiplication;
            }
            else
            {
                throw new ArgumentException("Can not find suitable pattern!");
            }
        }

        private static void GenerateNextElementGeneratorWithNoDivision()
        {
            useOneSide = true;

            if (functionElements[0].EndsWith("!"))
            {
                factorielUP = true;
                functionElements = functionElements.Select(x => x.TrimEnd('!')).ToList();
            }

            double[] elements = functionElements.Select(double.Parse).ToArray();

            if (elements[1] - elements[0] == elements[2] - elements[1] &&
                elements[3] - elements[2] == elements[2] - elements[1])
            {
                elementCreatorUp = elements[1] - elements[0];
                getNextElementUP = Sum;
            }
            else if (elements[1] / elements[0] == elements[2] / elements[1] &&
                     elements[3] / elements[2] == elements[2] / elements[1])
            {
                elementCreatorUp = elements[1] / elements[0];
                getNextElementUP = Multiplication;
            }
            else
            {
                throw new ArgumentException("Can not find suitable pattern!");
            }
        }

        private static void InicializeDelegateWithProperActions()
        {
            for (int i = 0; i < 4; i++)
            {
                if (functionSighns[i] == '+')
                {
                    ICSOperations += VoidSum;
                }
                else
                {
                    ICSOperations += VoidSubstraction;
                }
            }
        }

        private static void ReadInput()
        {
            Console.WriteLine("Enter the first 5 elements of the function: ");
            Console.WriteLine("Examples :");
            Console.WriteLine("1! + 2! – 3! + 4! -5!");
            Console.WriteLine("1 + 1/2 + 1/3 + 1/4 + 1/5");
            Console.WriteLine("1 + 1/2! + 1/4! + 1/8! + 1/16!");
            Console.WriteLine("1 + 1/2! + 1/3! + 1/4! + 1/5!");
            Console.WriteLine("1 + 1/2 - 1/4 + 1/8 - 1/16");
            Console.WriteLine("1 + 1/2 + 1/4 + 1/8 + 1/16");
            Console.WriteLine("Enter the first 5 elements of the function: ");

            string function = Console.ReadLine();

            function = function.Replace(" ", string.Empty);

            functionElements = function.Split(new char[] { '-', '+' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            functionSighns = function.Where(x => x == '-' || x == '+').ToList();

            Console.WriteLine("Enter how many signs precision do you want: ");
            digitsToUse = int.Parse(Console.ReadLine());
            precision = 1.0 / Math.Pow(10, digitsToUse);
        }
    }
}