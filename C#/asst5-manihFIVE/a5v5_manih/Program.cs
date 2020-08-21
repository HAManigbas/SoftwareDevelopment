/*******************************************************************************************************************************************************
 * Due Date:              December 3, 2018
 * Software Designer:     Hazel Ann Manigbas
 * Course:                420-306-AB (Fall 2018)
 * Deliverable:           Assignment  #5 --- Stacks & Expression Evaluation (Version 5)
 * 
 * Description:           This program has a given array of OPERAND symbols and its corresponding values. An array of infix expressions using the 
 *                        operand symbols are also given, then it converts each infix expression to postfix expression. After conversion, 
 *                        the postfix expression will be evaluated with the corresponding values of the operand symbols, then prints the infix 
 *                        expression with the postfix expression and its evaluation result.
 *                        
 *                        Before the conversion and evaluation, it will first output the operand symbols and its corresponding values. And also
 *                        the outputs of the sample test on the operand and operator stack functions.
 *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace a5v5_manih
{
    class Program
    {
        /*************************************************************************
                                   GLOBAL VARIABLES           
        *************************************************************************/
        const int NMAX = 5;                                                                          //maximum size of each name string
        const int LSIZE = 5;                                                                         //actual number of infix strings in the data array
        const int NOPNDS = 10;                                                                       //number of operand symbols in the operand array
        static int IDX;                                                                              //index used to implement conversion stub
        static char[] opnd = new char[NOPNDS] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };  //operands symbols
        static double[] opndval = new double[NOPNDS] { 3, 1, 2, 5, 2, 4, -1, 3, 7, 187 };            //operand values
        static List<double> OPNDstack = new List<double>();                                          //stack of operands
        static List<char> OPRstack = new List<char>();                                               //stack of operators
        static string upperBorder = "╔═══════════════════════════════╗";                             //upperBorder, lowerBorder, dividerLine, FORMAT
        static string lowerBorder = "╚═══════════════════════════════╝";                             // -- are needed for the outputs to be inside the box
        static string dividerLine = "╠═══════════════════════════════╣";
        const string FORMAT = "║{0,-10} {1,20}║";

        static void Main(string[] args)
        {
            //Console.WindowWidth = 120;
            //Console.WindowHeight = Console.WindowWidth * 9 / 25;
            /*************************************************************************
                                      KEY DECLARATIONS            
            *************************************************************************/
            string[] infix = new string[LSIZE] { "C$A$E",                                            //array of infix expressions
                             "(A+B)*(C-D)",
                             "A$B*C-D+E/F/(G+H)",
                             "((A+B)*C-(D-E))$(F+G)",
                             "A-B/(C*D$E)"  };


            /*************************************************************************
                   PRINT OUT THE OPERANDS AND THEIR VALUES            
             *************************************************************************/
            WriteLine("\nOPERAND SYMBOLS USED WITH ITS CORRESPONDING VALUES\n");                     //title
            WriteLine(upperBorder);
            WriteLine(FORMAT, "SYMBOLS", "VALUES");                                                  // title headers
            for (int i = 0; i < NOPNDS; i++)
            {
                WriteLine(dividerLine);
                WriteLine(FORMAT, opnd[i], opndval[i]);                                              //operand symbols, operand values
            }
            WriteLine(lowerBorder);
            WriteLine("\n\n");
            ReadLine();


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
            }                                                                                        //infix expression, postfix conversion, evaluation result
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


        /***********************************************************************************************************
          CONVERSION FUNCTION: converts infix expression to postfix expression and return the conversion result
        **********************************************************************************************************/
        static string ConvertToPostfix(string infix)
        {
            string postfixResult = "";                                                               //postfix conversion result
            char[] infixSymbols = infix.ToCharArray();                                               //array of characters of the infix symbols
            int infixLen = infixSymbols.Length;                                                      //grab the length of the infix symbols 
            char topSym = '+';                                                                       //top operator symbol, initialize to enter the Prcd function 
            char s;                                                                                  //current infix symbol for the conversion
            bool isOpnd;                                                                             //tracks if the infix symbol is an operand
            bool und = true;                                                                         //checks for underflow, if OPRstack is empty or not

            for(int c = 0; c < infixLen; c++)
            {
                s = infixSymbols[c];                                                                 //grab next symbol candidate
                isOpnd = IsOperand(s);                                                               //check if current symbol is operand

                if (isOpnd)                                                                          //current symbol is operand
                    postfixResult += s.ToString();                                                   //join the current symbol to postfix string
                else
                {                                                                                    //current symbol is operator
                    OPRpopAndTest(ref topSym, ref und);                                              //test the OPRstack

                    while(!und && Prcd(topSym, s))                                                   //OPRstack not empty and Precedence rule is true
                    {                                                                                
                        postfixResult += topSym.ToString();                                          //join the top symbol to postfix string
                        OPRpopAndTest(ref topSym, ref und);                                          //test the OPRstack,
                    }                                                                                //--if not empty, pop current top-element off the OPRstack
                                                                                                     //--if empty, it forces out of the loop

                    if (!und)                                                                        //top symbol is either a scope-opener or lower priority than the current symbol
                        OPRpush(topSym);                                                             //push back top symbol to OPRstack

                    if (und || s != ')')                                                             //OPRstack is empty or current symbol is not a scope-closer
                        OPRpush(s);                                                                  //push current symbol to the stack except for scope-closer
                    else
                        topSym = OPRpop();                                                           //pop current-top element off the OPRstack 
                }                                                                                    //discard top and current symbol
            }
                                                                                                     //no more incoming infix symbol
            while(OPRstack.Count !=0)                                                                //OPRstack not empty
            {
                postfixResult += OPRpop().ToString();                                                //join the postfix string
            }
            return postfixResult;
        }


        /*************************************************************************
          IsOperand Function: returns true is the symbol is an operand  
        *************************************************************************/
        static bool IsOperand(char m)
        { return m >= 'A' && m <= 'J'; }


        /**************************************************************************************************************
                                PRECEDENCE FUNCTION
          RULES:    top symbol or current infix symbol is a scope opener then it is false
                    both top symbol and current infix symbol are exponent operator($) then it is false
                    current infix symbol is a scope-closer then it is true
                    top symbol is either greater than or equal priority of the current infix symbol then it is true
        *************************************************************************************************************/
        static bool Prcd(char top, char s)
        {
            bool x;
            if (top == '(')                                                                          //top symbol is a scope-opener
                x = false;
            else if (s == '(')                                                                       //current symbol is a scope-opener
                x = false;
            else if (s == ')')                                                                       //current symbol is scope-closer
                x = true;
            else if (top == '$' && s == '$')                                                         //top symbol and current symbol are exponent operator($)
                x = false;
            else if (Priority(top) >= Priority(s))                                                   //top symbol >= priority of the current symbol
                x = true;
            else                                                                                     //anything not mentioned on the rules
                x = false;
            return x;
        }


        /********************************************************************************
                                OPERATOR PRIORITY FUNCTION
          HIGHEST TO LOWEST PRIORITY RANK:
                    -- exponent ($)
                    -- multiplication (*), division (/)
                    -- addition (+), subtraction (-)
        *******************************************************************************/
        static int Priority(char z)
        {
            int rank = 0;                                                                          //intialize the priority rank
            switch(z)                      
            {
                case '$':                                                                          //symbol is exponent operator
                    rank = 3;
                    break;
                case '*':                                                                          //symbol is multiplication operator
                case '/':                                                                          //symbol is division operator
                    rank = 2;
                    break;
                case '-':                                                                          //symbol is subraction operator
                case '+':                                                                          //symbol is addition operator
                    rank = 1;
                    break;
            }
            return rank;
        }


        /*************************************************************************
                            OPERATOR STACK FUNCTIONS:
		- the global object "OPRstack" is an instance of the class "List"
		- ihe contents of "OPRstack" are char
		- see its declaration immediately before the Main block
        *************************************************************************/

        /*************************************************************************
          OPERATOR STACK PUSH FUNCTION: push operator symbol to the OPRstack 
        *************************************************************************/
        static void OPRpush(char opr)
        { OPRstack.Add(opr); }


        /*************************************************************************
                                OPERATOR STACK POP FUNCTION  
        *************************************************************************/
        static char OPRpop()
        {
            char last = OPRstack[OPRstack.Count - 1];                                       //pop the current top-element off the OPRstack
            OPRstack.RemoveAt(OPRstack.Count - 1);                                          //remove current top-element from the OPRstack
            return last;
        }


        /*************************************************************************
          DUMP OPERATOR STACK FUNCTION: prints the entire stack ofter each action 
        *************************************************************************/
        static void dumpOPRstack()
        {
            foreach (char value in OPRstack)
                Write(value + " | ");
            if (OPRstack.Count == 0)
                Write("EMPTY");
        }


        /****************************************************************************************
         OPERATOR STACK POP AND TEST FUNCTION: test the stack if empty and 
                            if not pop the top most elemet of the OPRstack to top symbol
        ****************************************************************************************/
        static void OPRpopAndTest(ref char topsym, ref bool un)
        {
            if (OPRstack.Count == 0)                                                      //OPRstack is empty
                un = true;
            else                                                                          //OPRstack is not empty
            {
                un = false;
                topsym = OPRpop();                                                        //pop the current top-element off the OPRstack
            }
        }


        /*************************************************************************
                                OPERATOR STACK TEST FUNCTION  
        *************************************************************************/
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
            }                                                                          //if stack is empty, it will force out of the loop
            WriteLine("EMPTY");
        }


        /***************************************************************************************
          EVALUATION FUNCTION: evaluates the postfix expression and return the final result
        ***************************************************************************************/
        static double EvaluatePostfix(string postfix)
        {
            char[] postFixSym = postfix.ToCharArray();                                           //array of characters of the postfix expression
            double result = 0;                                                                   //evluation result
            double val;                                                                          //current operand value
            char s;                                                                              //current postfix symbol
            bool isOperand;                                                                      //tracks if the postfix symbol is an operand(true)
            double op2;                                                                          //second operand value on the evaluation
            double op1;                                                                          //first operand value on the evaluation

            for (int i = 0; i < postFixSym.Length; i++)
            {
                val = 0;                                                                        //initialize current operand value
                s = postFixSym[i];                                                              //grab next current symbol
                isOperand = IsOPND(s, ref val);                                                 //check if current symbol is operand
                                                                                                //if operand, grab the corresponding operand value

                if (!isOperand)                                                                 //current symbol is an operator
                {
                    op2 = OPNDpop();                                                            //pop current top-element of the OPNDstack to op2
                    op1 = OPNDpop();                                                            //pop the second top-element of OPNDstack to op1

                    switch (s)                                                                  //checks which operator to be used
                    {
                        case '$':                                                               //exponent operator
                            result = Math.Pow(op1, op2);                                        //evaluate
                            break;
                        case '*':                                                               //multiplication operator
                            result = op1 * op2;                                                 //evaluate
                            break;
                        case '/':                                                               //division operator
                            result = op1 / op2;                                                 //evaluate
                            break;
                        case '+':                                                               //addition operator
                            result = op1 + op2;                                                 //evaluate                                                
                            break;
                        case '-':                                                               //subraction operator
                            result = op1 - op2;                                                 //evaluate
                            break;
                    }
                    OPNDpush(result);                                                           //after evaluation, push the result to the OPNDstack
                }
                else                                                                            //current symbol is operand
                    OPNDpush(val);                                                              //push the operand value to the OPNDstack
            }

            result = OPNDpop();                                                                 //evaluation done, pop the final result

            return result;
        }


        /*************************************************************************
          IS OPERAND FUNCTION: returns true if the postfix symbol is an operand
                                and grab the corresponding operand value
        *************************************************************************/
        static bool IsOPND(char c, ref double var)
        {
            bool found = false;                                                               //assume that the symbol is operator
            int i = 0;

            while ((i < opnd.Length) && (!found))                                             //not end of array, symbol not operand
            {
                if (c == opnd[i])                                                             //symbol is operand
                {
                    found = true;                                                             //force out of the loop
                    var = opndval[i];                                                         //grab the corresponding operand value
                }
                i++;              
            }
            return found;
        }


        /*************************************************************************
                                OPERAND STACK TEST FUNCTION  
        *************************************************************************/
        static void OPNDtestStack() 
        {
            var rand = new Random();
            WriteLine("\n\nOPNDpush AND OPNDpop Function Test\n");
            WriteLine("\nOperation".PadRight(15) + "Value Pushed/Popped".PadRight(25) + "Stack After Operation"); //title headers
            OutLine(70, '=');

            for (int i = 0; i < 5; i++)
            {
                double value = ((double)rand.Next(1000, 10000)) / 100; 	                      // create random double with 2 decimal places
                OPNDpush(value);					                                          // push this value onto the stack
                Write("PUSH".PadRight(15) + value.ToString().PadRight(25));
                dumpOPNDstack();					                                          // display the entire stack after each push
                WriteLine("\n");
            }

            for (int i = 0; i < 5; i++)
            {
                double value = OPNDpop();			                                          // pop the current top-element off the stack	
                Write("POP".PadRight(15) + value.ToString().PadRight(25));
                dumpOPNDstack();					                                          // display the entire stack after each pop
                WriteLine("\n");
            }

        }


        /*************************************************************************
                            OPERAND STACK FUNCTIONS:
		- the global object "OPNDstack" is an instance of the class "List"
		- ihe contents of "OPNDstack" are doubles
		- see its declaration immediately before the Main block
        *************************************************************************/

        /*************************************************************************
          OPERAND STACK PUSH FUNCTION: push operand value to the OPNDstack 
        *************************************************************************/
        static void OPNDpush(double opnd)
        {   OPNDstack.Add(opnd);    }


        /*************************************************************************
                                OPERAND STACK POP FUNCTION  
        *************************************************************************/
        static double OPNDpop()
        {
            double last = OPNDstack[OPNDstack.Count - 1];                                   //pop the current top-element off the OPNDstack
            OPNDstack.RemoveAt(OPNDstack.Count - 1);                                        //remove current top-element from the OPNDstack 
            return last;
        }


        /*************************************************************************
          DUMP OPERAND STACK FUNCTION: prints the entire stack ofter each action 
        *************************************************************************/
        static void dumpOPNDstack()
        {
            foreach (double value in OPNDstack)
                Write(value + " | ");
            if (OPNDstack.Count == 0)
                Write("EMPTY");
        }
    }
}
