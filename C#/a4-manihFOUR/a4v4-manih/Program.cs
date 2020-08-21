/*********************************************************************************************************************************************
 * Due Date:             November 19, 2018
 * Software Designer:     Hazel Ann Manigbas
 * Course:               420-306-AB (Fall 2018)
 * Deliverable:          Assignment  #4 --- State Analysis and Parsing (Version 4)
 * 
 * Description:          This program will process a given array of string, converting each line to array of characters. It will then
 *                       processe each character of the current line obtaining its character-type variable and stays in the state-handling 
 *                       function as long as the incoming character does not cause a state-transition. It also converts each numeric constant 
 *                       to either int or double type variable depending on its form or if it's a word, it will keep track of the length 
 *                       of each word. When a state-transition occurs, the current int or double value will be added to their corresponding
 *                       array of results and the frequency for each word length will be updated. After processing all the lines, the program
 *                       will display the results.
 *********************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace a4v4_manih
{
    class Program
    {
        #region.GLOBALS
        //========================================================================================================================================//
        //                                              GLOBAL VARIABLES
        //========================================================================================================================================//
        enum StateType { white, word, num, dble, expo };                                       //for the state variable
        enum CharType { whsp, lett, expo, digit, plus, minus, point, quote, endstr };          //for the character category
        const int MAX = 30;                                                                    // result's array size
        static char[] line;                                                                    //current line being processed
        static char ch;                                                                        //current character being processed
        static CharType chtype;                                                                //variable to store the type of the current character
        static StateType state;                                                                //current state of the application
        static int wlen;                                                                       //length of the current word
        static int k;                                                                          //subscript indicating present position within the current line
        static int len;                                                                        //length of the current line
        static int theInt;                                                                     //current integer encountered
        static double theDble;                                                                 //current double encountered
        static int theExpo;                                                                    //current exponent encountered
        static int sign;                                                                       //holds the sign of the current numeric constant encountered
        static int eSign;                                                                      //holds the sign of the current exponent encountered
        static int power;                                                                      //keeps track the number of decimal places on the double
        static int[] mywords = new int[MAX];                                                   //keeps track of the number of words of various lengths
        static int[] myints = new int[MAX];                                                    //holds all the integer constants encountered
        static double[] mydbles = new double[MAX];                                             //holds all converted double constants encountered
        static int i;                                                                          //subscript indicating position within the array 'myints'
        static int d;                                                                          //subscript indicating position within the array 'mydbles'
        static string upperBorder = "╔═══════════════════════════════╗";                       //upperBorder, lowerBorder, dividerLine, FORMAT
        static string lowerBorder = "╚═══════════════════════════════╝";                       // -- are needed to print the results inside the box
        static string dividerLine = "╠═══════════════════════════════╣";
        const string FORMAT = "║{0,-10} {1,20}║";
        #endregion.GLOBALS
        #region.MAINBLOCK
        static void Main(string[] args)
        {
            //========================================================================================================================================//
            //                                              MAIN BLOCK
            //========================================================================================================================================//
            int nLines = 4;                                                                 //array of lines to be processed
            int[] llen = new int[4];                                                        //holds length of each line
            string[] lines = new string[] {                                                 //lines to be processed
                "    first 123		and then -.1234 but you'll need 123.456		 and 7e-4 plus one like +321. all quite avant-",
                "garde   whereas ellen's true favourites are 123.654E-2	exponent-form which can also be -54321E-03 or this -.9E+5",
                "We'll prefer items like			fmt1-decimal		+.1234567e+05 or fmt2-dec -765.3245 or fmt1-int -837465 and vice-",
                "versa or even format2-integers -19283746   making one think of each state's behaviour for 9 or even 3471e-7 states " };

            //  PRINT OUT THE TEXT LINES AS SINGLE STRINGS FOLLOWED BY THIER LENGTH.
            Console.WriteLine("\n\nHERE ARE THE TEXT LINES PRINTED OUT AS SINGLE STRINGS ... EACH FOLLOWED BY ITS LENGTH. \n\n");
            for (k = 0; k < nLines; k++)
            {
                WriteLine(lines[k], "\n");                                                  //Prints current line
                llen[k] = lines[k].Length;                                                  //grab length of the current line
                WriteLine(llen[k]);                                                         //Prints length of current line
            }
            WriteLine("\n\n");
            ReadLine();

            for (int s = 0; s < MAX; s++)
                mywords[s] = 0;                                                             //initial values of mywords[]

            state = StateType.white;                                                       //state type starting point
            i = -1;                                                                        //myints[] is still empty
            d = -1;                                                                        //mydbles[] is still emppty

            for (int x = 0; x < nLines; x++)
            {
                line = lines[x].ToCharArray();                                            // change ith string into an array-of-characters
                len = line.Length;                                                        // grab length of this array
                ch = line[0];                                                             //grab the 1st character of the current line
                chtype = GetType(ch);                                                     //get the character type of the current character
                k = 0;                                                                    //resets position of the character to be processed

                while (k < len)
                {
                    switch (state)
                    {
                        case StateType.white:                                            //White State
                            WhiteState();
                            break;
                        case StateType.word:                                             //Word State
                            WordState();
                            break;
                        case StateType.num:                                              //Number State
                            NumState();
                            break;
                        case StateType.dble:                                             //Double State
                            DbleState();
                            break;
                        case StateType.expo:                                             //Exponent State
                            ExpoState();
                            break;
                    }
                }

                if (ch == '-' && state == StateType.word)                                //last character of the line ends in hyphen and in "Word State"
                    state = StateType.word;                                              //stays in "Word State"
                else
                    state = StateType.white;                                             //restarts in "White State"
            }

            WriteLine("\n\tANALYSIS RESULTS");                                           //displays each result inside the box
            WriteLine("---------------------------------");
            WriteLine("\n\tWORD RESULTS:");                                              //frequency of each word results
            WriteLine(upperBorder);
            WriteLine(FORMAT, "LENGTH", "FREQUENCY");
            for (int u = 0; u < MAX; u++)
            {
                if(mywords[u] > 0)                                                      //word length was encountered
                {
                    WriteLine(dividerLine);
                    WriteLine(FORMAT, u+1, mywords[u]);
                }
            }
            WriteLine(lowerBorder);
            WriteLine("\n\tINTEGER RESULTS:");                                          //all the integers encountered
            WriteLine(upperBorder);
            WriteLine(FORMAT, "INDEX", "VALUE");
            for (int u = 0; u <= i; u++)
            {
                WriteLine(dividerLine);
                WriteLine(FORMAT,u,myints[u]);
            }
            WriteLine(lowerBorder);
            WriteLine("\n\tDOUBLE RESULTS:");                                          //all the doubles encountered
            WriteLine(upperBorder);
            WriteLine(FORMAT, "INDEX", "VALUE");
            for (int u = 0; u <= d; u++)
            {
                WriteLine(dividerLine);
                WriteLine(FORMAT,u,mydbles[u]);
            }
            WriteLine(lowerBorder);
            WriteLine("\n\nPress any key to exit the program.............");
            ReadKey();
        }
        #endregion.MAINBLOCK
        #region.CharacterTypeMethods
        //========================================================================================================================================//
        // GET CHARACTER TYPE --> returns character type of the current character
        //========================================================================================================================================//
        static CharType GetType(char chr)
        {
            CharType type = CharType.whsp;                                                 //default character type

            if (IsSpace(chr))                                                              //current character is a space
                type = CharType.whsp;
            else if (IsAlpha(chr))                                                         //current character is a letter
            {
                if (ToUpper(chr) == 'E')                                                   //exception: 'E' is an exponent type of character
                    type = CharType.expo;
                else
                    type = CharType.lett;
            }
            else if (IsDigit(chr))                                                         //current character is a number
                type = CharType.digit;
            else
            {
                switch (chr)
                {
                    case '+':                                                             //current character is a plus(+)
                        type = CharType.plus;
                        break;
                    case '-':                                                             //current character is a minus(-)
                        type = CharType.minus;
                        break;
                    case '.':
                        type = CharType.point;                                           //current character is a point(.)
                        break;
                    case '\'':
                        type = CharType.quote;                                           //current character is a quotation(')
                        break;
                }
            }
            return type;
        }
        //========================================================================================================================================//
        //  IsSpace --> returns true when the character is a space, tab or a new line                                 
        //========================================================================================================================================//
        static bool IsSpace(char c)
        {
            bool x;
            if (c == ' ')
                x = true;
            else if (c == '\t')
                x = true;
            else if (c == '\n')
                x =  true;
            else
                x = false;

            return x;
        }
        //========================================================================================================================================//
        //  IsDigit --> returns true when the character is a number                               
        //========================================================================================================================================//
        static bool IsDigit(char c)
        { return (c >= '0' && c <= '9'); }
        //========================================================================================================================================//
        //  IsAlpha --> returns true when the character is a letter                               
        //========================================================================================================================================//
        static bool IsAlpha(char c)
        { return (ToUpper(c) >= 'A' && ToUpper(c) <= 'Z'); }
        //========================================================================================================================================//
        //  ToUpper --> returns the character and if it is a letter, return it in UPPERCASE format                             
        //========================================================================================================================================//
        static char ToUpper(char cha)
        {
            if (cha >= 'a' && cha <= 'z')                                           //character is in lowercase
                cha = (char)(cha - ('a' - 'A'));                                    //converts letter to Uppercase

            return cha;
        }
        #endregion.CharacterTypeMethods
        #region.StateTransitionMethods
        #region.WhiteState
        //========================================================================================================================================//
        // WHITE STATE --> stays on this state as long as the incoming character is in white space type and until the end of line
        //========================================================================================================================================//
        static void WhiteState()
        {
            while (state == StateType.white && k < len)
            {
                switch (chtype)
                {
                    case CharType.lett:                                                 //state transition(White to Word), forces out of the loop
                    case CharType.expo:
                        WhiteToWord();
                        break;
                    case CharType.digit:                                                //state transition(White to Num), forces out of the loop
                    case CharType.plus:
                    case CharType.minus:
                        WhiteToNum();
                        break;
                    case CharType.point:                                                //state transition(White to Double), forces out of the loop
                        WhiteToDble();
                        break;
                }
                if (k < len - 1)                                                         //not end of line
                {
                    ch = line[++k];                                                      //get new current character
                    chtype = GetType(ch);                                                //get current character type
                }
                else
                    k++;                                                                //end of line, forces out of the loop
            }
        }
        //========================================================================================================================================//
        //                                                  WHITE TO WORD
        //========================================================================================================================================//
        static void WhiteToWord()
        {
            state = StateType.word;                                                     //updates state type
            wlen = 1;                                                                   //starting length of the word
        }
        //========================================================================================================================================//
        //                                                  WHITE TO NUM
        //========================================================================================================================================//
        static void WhiteToNum()
        {
            state = StateType.num;                                                      //updates state type
            sign = 1;                                                                   //default sign is positive
            theInt = 0;                                                                 //theInt starting point

            if (!(ch == '-' || ch == '+'))                                              //current character is not a sign
                theInt = theInt * 10 + ch - '0';                                        //converts character to int
            else if (ch == '-')                                                         //current character is a sign and a minus
                sign = -1;                                                              //change sign to negative
        }
        //========================================================================================================================================//
        //                                                  WHITE TO DOUBLE
        //========================================================================================================================================//
        static void WhiteToDble()
        {
            state = StateType.dble;                                                     //updates state type
            power = 1;                                                                  //power starting point
            theDble = 0;                                                                //theDble starting point
            sign = 1;                                                                   //default sign is positive
        }
        #endregion.WhiteState
        #region.WordState
        //========================================================================================================================================//
        // WORD STATE --> stays on this state as long as the incoming character is not a white space type and until the end of line
        //========================================================================================================================================//
        static void WordState()
        {
            while (state == StateType.word && k < len)
            {
                if (chtype == CharType.whsp)                                           //state transition, will force out of the loop
                    WordToWhite();
                else                                                                   //still in word state
                    wlen++;                                                            //updates word length

                if (k < len - 1)                                                       //not end of line                                                 
                {
                    ch = line[++k];                                                    //get new current character
                    chtype = GetType(ch);                                              //get current character type
                }
                else                                                                   //end of line
                    k++;                                                               //forces out of the loop
            }
        }
        //========================================================================================================================================//
        //                                              WORD TO WHITE
        //========================================================================================================================================//
        static void WordToWhite()
        {
            state = StateType.white;                                                    //updates state type
            mywords[wlen - 1]++;                                                        //updates frequency of the corresponding word length
        }
        #endregion.WordState
        #region.NumState
        //========================================================================================================================================//
        // NUM STATE --> stays on this state as long as the incoming character a number and until the end of line
        //========================================================================================================================================//
        static void NumState()
        {
            while (state == StateType.num && k < len)
            {
                switch (chtype)
                {
                    case CharType.expo:                                                 //state transition to exponent
                        state = StateType.expo;                                         //updates state type, forces out of the loop
                        theDble = theInt * sign;                                        //int value transferred to double value in exponential form
                        break;
                    case CharType.point:                                                //state transition to double
                        state = StateType.dble;                                         //updates state type, forces out of the loop
                        theDble = theInt;                                               //int value transferred to double value in normal form
                        power = 1;                                                      //power starting point
                        break;
                    case CharType.whsp:                                                 //state transition to white
                        state = StateType.white;                                        //updates state type, forces out of the loop
                        theInt = theInt * sign;                                         //theInt final value
                        myints[++i] = theInt;                                           //adds the current theInt to the array of integers
                        break;
                    default:                                                            //still in num state
                        theInt = theInt * 10 + ch - '0';                                //convert current character to int, updates current theInt value
                        break;
                }
                if (k < len - 1)                                                         //not end of line
                {
                    ch = line[++k];                                                      //get new current character
                    chtype = GetType(ch);                                                //get current character type
                }
                else
                    k++;                                                                //end of line, forces out of the loop
            }
        }
        #endregion.NumState
        #region.DoubleState
        //========================================================================================================================================//
        // DOUBLE STATE --> stays on this state as long as the incoming character is not a white space type or not 'E'(exponent) 
        //                      and until the end of line
        //========================================================================================================================================//
        static void DbleState()
        {
            while (state == StateType.dble && k < len)
            {
                switch (chtype)
                {
                    case CharType.expo:                                                 //state transition to exponent
                        state = StateType.expo;                                         //updates state type, forces out of the loop
                        theDble = theDble / power * sign;                               //transfers double normal form to exponential form
                        break;
                    case CharType.whsp:                                                 //state transtion to white
                        state = StateType.white;                                        //updates state type, forces out of the loop
                        theDble = theDble / power * sign;                               //get theDble(in normal form) final value
                        mydbles[++d] = theDble;                                         //adds the current theDble value to the array of doubles
                        break;
                    default:                                                            //still in double state
                        power = power * 10;                                             //updates power value
                        theDble = theDble * 10 + ch - '0';                              //convert current character to double, updates current theDble value
                        break;
                }
                if (k < len - 1)                                                         //not end of line
                {
                    ch = line[++k];                                                      //get new current character
                    chtype = GetType(ch);                                                //get current character type
                }
                else
                    k++;                                                                //end of line, forces out of the loop
            }
        }
        #endregion.DoubleState
        #region.ExpoState
        //========================================================================================================================================//
        // EXPONENT STATE --> stays on this state as long as the incoming character is a number and not a white space type and until the end of line
        //========================================================================================================================================//
        static void ExpoState()
        {
            eSign = 1;                                                                   //default exponent sign is positive
            theExpo = 0;                                                                 //exponent's starting point

            if (ch == '-' || ch == '+')                                                  //current character is a sign, skips it
            {
                if (ch == '-')                                                           //current character is a minus
                    eSign = -1;                                                          //change exponent sign to negative

                if (k < len - 1)                                                         //not end of line
                {
                    ch = line[++k];                                                      //get new current character
                    chtype = GetType(ch);                                                //get current character type
                }
                else
                    k++;                                                                 //end of line, forces out of the loop
            }
            else                                                                         //current character is not a sign
                theExpo = theExpo * 10 + ch - '0';                                       //updates theExpo's value

            while (state == StateType.expo && k < len)
            {
                if (chtype == CharType.whsp)                                             //state transition to white
                {
                    state = StateType.white;                                            //updates state type, forces out of the loop

                    if (eSign == 1)                                                      //exponent sign is positive
                    {
                        for (int m = 0; m < theExpo; m++)
                            theDble = theDble * 10;                                     //updates theDble(in exponential form) to gets its final value
                    }
                    else                                                                //exponent sign is negative
                    {
                        for (int m = 0; m < theExpo; m++)                               
                            theDble = theDble / 10;                                     //updates theDble(in exponential form) to gets its final value
                    }
                    mydbles[++d] = theDble;                                             //adds the current theDble value to the array of doubles
                }
                else                                                                    //still in exponent state
                    theExpo = theExpo * 10 + ch - '0';                                  //updates theExpo's value

                if (k < len - 1)                                                         //not end of line
                {
                    ch = line[++k];                                                      //get new current character
                    chtype = GetType(ch);                                                //get current character type
                }
                else
                    k++;                                                                 //end of line, forces out of the loop
            }

            if (k == len)                                                               //end of line is reached
            {
                if (eSign == 1)                                                         //sign is positive
                {
                    for (int m = 0; m < theExpo; m++)
                        theDble = theDble * 10;                                         //updates theDble(in exponential form) to gets its final value
                }
                else
                {
                    for (int m = 0; m < theExpo; m++)
                        theDble = theDble / 10;                                        //updates theDble(in exponential form) to gets its final value
                }
                mydbles[++d] = theDble;                                               //adds the current theDble value to the array of doubles
            }
        }
        #endregion.ExpoState
        #endregion.StateTransitionMethods
    }
}
