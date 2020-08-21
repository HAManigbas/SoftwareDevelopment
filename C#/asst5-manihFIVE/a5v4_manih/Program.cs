using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace a5v4_manih
{
    class Program
    {
        const int NMAX = 5;        //maximum size of each name string
        const int LSIZE = 5;       //actual number of infix strings in the data array
        const int NOPNDS = 10;     //number of operand symbols in the operand array
        static int IDX;                   //index used to implement conversion stub
        static char[] opnd = new char[NOPNDS] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' }; //operands symbols
        static double[] opndval = new double[NOPNDS] { 3, 1, 2, 5, 2, 4, -1, 3, 7, 187 };           //operand values
        static List<double> OPNDstack = new List<double>();
        static List<char> OPRstack = new List<char>();

        static void Main(string[] args)
        {
            //Console.WindowWidth = 120;
            //Console.WindowHeight = Console.WindowWidth * 9 / 25;
            /*************************************************************************
                                      KEY DECLARATIONS            
            *************************************************************************/
            string[] infix = new string[LSIZE] { "C$A$E",    //array of infix strings
                             "(A+B)*(C-D)",
                             "A$B*C-D+E/F/(G+H)",
                             "((A+B)*C-(D-E))$(F+G)",
                             "A-B/(C*D$E)"  };



            /*************************************************************************
                   PRINT OUT THE OPERANDS AND THEIR VALUES            
             *************************************************************************/
            WriteLine("\nOPERAND SYMBOLS USED:\n");   //title
            for (int i = 0; i < NOPNDS; i++)
                Write(opnd[i].ToString().PadLeft(5));

            WriteLine("\n\n\nCORRESPONDING OPERAND VALUES:\n");   //title
            for (int i = 0; i < NOPNDS; i++)
                Write(opndval[i].ToString().PadLeft(5));

            WriteLine("\n\n");

            /*************************************************************************
                                            OUTPUT LINES
            *************************************************************************/
            WriteLine("\nSAMPLE TEST ON OPERAND STACK FUNCTIONS");                                   //title, testStack() uses function dumpOPNDstack() to test Operand stack functions
            OPNDtestStack();                                                                         //does 5 pushes, then 5 pops
            ReadLine();
            WriteLine("\n\nSAMPLE TEST ON OPERATOR STACK FUNCTIONS");                                //title
            OPRtestStack();                                                                          //tests the OPRpush, OPRpop and OPRpopAndTest
            ReadLine();
            WriteLine("\n\n\nCONVERSION AND EVALUATION RESULTS\n");                                  //title
            WriteLine("Infix Expression".PadRight(30) + "Postfix Expression".PadRight(30) + "Value".PadRight(20)); //title headers
            OutLine(70, '=');

            for (IDX = 0; IDX < LSIZE; IDX++)
            {
                string postfix = ConvertToPostfix(infix[IDX]);                                       //postfix expression result 
                WriteLine(infix[IDX].PadRight(30) + postfix.PadRight(30) + EvaluatePostfix(postfix));//prints the result
            }                                                                                        //current infix expression, current postfix conversion, current evaluation result
            WriteLine("\n\n\n\nPress any key to exit the application.........\n\n");
            ReadKey();
        }

        /***************************************************************************************** 
                FUNCTION OutLine:   formatting function to print n repetitions of char ch
        ******************************************************************************************/
        static void OutLine(int n, char ch)
        {
            for (int q = 0; q < n; q++)
                Write(ch.ToString());

            WriteLine("\n");
        }
        /*************************************************************************
                                CONVERSION FUNCTION     
        *************************************************************************/
        static string ConvertToPostfix(string infix)
        {
            string[] postfix = new string[LSIZE] { "CAE$$",  //array of postfix strings
                             "AB+CD-*",
                             "AB$C*D-EF/GH+/+",
                             "AB+C*DE--FG+$",
                             "ABCDE$*/-"  };
            return postfix[IDX];
        }
        /*************************************************************************
                                EVALUATION FUNCTION  
        *************************************************************************/
        static double EvaluatePostfix(string postfix)
        {
            char[] postFixSym = postfix.ToCharArray();
            double result;
            char s;
            bool isOperand;
            double op2;
            double op1;

            for (int i = 0; i < postFixSym.Length; i++)
            {
                result = 0;
                s = postFixSym[i];
                isOperand = IsOPND(s, ref result);

                if (!isOperand)
                {
                    op2 = OPNDpop();
                    op1 = OPNDpop();

                    switch (s)
                    {
                        case '$':
                            result = Math.Pow(op1, op2);
                            break;
                        case '*':
                            result = op1 * op2;
                            break;
                        case '/':
                            result = op1 / op2;
                            break;
                        case '+':
                            result = op1 + op2;
                            break;
                        case '-':
                            result = op1 - op2;
                            break;
                    }
                    OPNDpush(result);
                }
                else
                    OPNDpush(result);
            }

            result = OPNDpop();

            return result;
        }
        static bool IsOPND(char c, ref double var)
        {
            bool found = false;
            int i = 0;

            while ((i < opnd.Length) && (!found))
            {
                if (c == opnd[i])
                {
                    found = true;
                    var = opndval[i];
                }
                i++;
            }

            return found;
        }
        static void OPNDtestStack()
        {
            var rand = new Random();

            WriteLine("\n\nOperation".PadRight(15) + "Value Pushed/Popped".PadRight(25) + "Stack After Operation");
            OutLine(70, '=');

            for (int i = 0; i < 5; i++)
            {
                double value = ((double)rand.Next(1000, 10000)) / 100; 	// create random double with 2 decimal places
                OPNDpush(value);					// push this value onto the stack
                Write("PUSH".PadRight(15) + value.ToString().PadRight(25));
                dumpOPNDstack();					// display the entire stack after each push
                WriteLine("\n");
            }

            for (int i = 0; i < 5; i++)
            {
                double value = OPNDpop();			// pop the current top-element off the stack	
                Write("POP".PadRight(15) + value.ToString().PadRight(25));
                dumpOPNDstack();					// display the entire stack after each pop
                WriteLine("\n");
            }

        }


        /*************************************************************************
                            STACK FUNCTIONS:
		- the global object "OPNDstack" is an instance of the class "List"
		- ihe contents of "OPNDstack" are doubles
		- see its declaration immediately before the Main block

        *************************************************************************/
        static void OPNDpush(double opnd)
        {
            OPNDstack.Add(opnd);
        }

        static double OPNDpop()
        {
            double last = OPNDstack[OPNDstack.Count - 1];
            OPNDstack.RemoveAt(OPNDstack.Count - 1);
            return last;
        }

        static void dumpOPNDstack()
        {
            foreach (double value in OPNDstack)
                Write(value + " | ");
            if (OPNDstack.Count == 0)
                Write("EMPTY");
        }

        /*************************************************************************
                            STACK FUNCTIONS:
		- the global object "OPRstack" is an instance of the class "List"
		- ihe contents of "OPRstack" are char
		- see its declaration immediately before the Main block

        *************************************************************************/
        static void OPRpush(char opr)
        {
            OPRstack.Add(opr);
        }

        static char OPRpop()
        {
            char last = OPRstack[OPRstack.Count - 1];
            OPRstack.RemoveAt(OPRstack.Count - 1);
            return last;
        }

        static void dumpOPRstack()
        {
            foreach (char value in OPRstack)
                Write(value + " | ");
            if (OPRstack.Count == 0)
                Write("EMPTY");
        }

        static void OPRpopAndTest(ref char topsym, ref bool und)
        {
            if (OPRstack.Count == 0)
                und = true;
            else
            {
                und = false;
                topsym = OPRpop();
            }
        }

        static void OPRtestStack()
        {
            string OprString = "$*/+-";                                                  //random symbols to test the OPRstack functions
            char[] OprChar = OprString.ToCharArray();                                    //array of symbols
            int OprLen = OprChar.Length;                                                 //grab the length of symbols' array
            char value = '+';                                                            //current value to push/pop in the OPRstack
            bool isEmp = false;                                                          //underflow variable


            /*************************************************************************
                               OPRpush AND OPRpop TEST
            *************************************************************************/
            WriteLine("\n\nOPRpush AND OPRpop Function TEST\n");
            WriteLine("\nOperation".PadRight(15) + "Value Pushed/Popped".PadRight(25) + "Stack After Operation"); //title headers
            OutLine(70, '=');

            for (int i = 0; i < OprLen; i++)
            {
                value = OprChar[i];                                                     //grab the next symbol
                OPRpush(value);			                                                //push this value onto the stack		
                Write("PUSH".PadRight(15) + value.ToString().PadRight(25));
                dumpOPRstack();					                                        //display the entire stack after each push
                WriteLine("\n");
            }

            for (int i = 0; i < OprLen; i++)
            {
                value = OPRpop();                                                       //pops the current top-element off the stack
                Write("POP".PadRight(15) + value.ToString().PadRight(25));
                dumpOPRstack();                                                         //display the entire stack after each pop          
                WriteLine("\n");
            }
            ReadLine();


            /*************************************************************************
                            OPRpush AND OPRpopAndTest TEST
            *************************************************************************/
            WriteLine("\n\nOPRpush AND OPRpopAndTest Function Test\n");
            WriteLine("\nOperation".PadRight(15) + "Value Pushed/Popped".PadRight(25) + "Stack After Operation"); //title headers
            OutLine(70, '=');
            for (int i = 0; i < OprLen; i++)
            {
                value = OprChar[i];                                                     //grab the next symbol
                OPRpush(value);					                                        //push this value onto the stack
                Write("PUSH".PadRight(15) + value.ToString().PadRight(25));
                dumpOPRstack();					                                        //display the entire stack after each push
                WriteLine("\n");
            }

            OPRpopAndTest(ref value, ref isEmp);                                        //if stack is not empty,it pops the current top-element off the stack
                                                                                        //and it will enter the loop
            while (!isEmp)
            {
                Write("POP".PadRight(15) + value.ToString().PadRight(25));
                dumpOPRstack();                                                        // display the entire stack after each pop
                WriteLine("\n");
                OPRpopAndTest(ref value, ref isEmp);			                       //pop the current top-element off the stack if it is not empty
            }                                                                          //if stack is empty, it sets isEmp to true which forces out of the loop
            WriteLine("EMPTY");
        }
    }
}
