﻿/*********************************************************************************************************************************************
 * Due Date:             November 12, 2018
 * Sofware Designer:     Hazel Ann Manigbas
 * Course:               420-306-AB (Fall 2018)
 * Deliverable:          Assignment  #4 --- State Analysis and Parsing (Milestone 3)
 * 
 * Description:          This program obtains the value of the character-type variable, staying in the state-handling function 
 *                       as long as the incoming character does not cause a state-transition. When a state-transition occurs, it 
 *                       prints out the state-transition itself i.e. STi-j where i is the source state and j is the destination state.
 *                       It also prints the result of the previous state, the word lenght for word state, the integer value for number
 *                       state and either the normal or exponent form value of the double numeric constant then the program starts 
 *                       a new output line, prints out the next characters on the new state.
 *********************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace a4v3_manih
{
    class Program
    {
        #region.GLOBALS
        //========================================================================================================================================//
        //                                              GLOBAL VARIABLES
        //========================================================================================================================================//
        enum StateType { white, word, num, dble, expo };                                       //for the state variable
        enum CharType { whsp, lett, expo, digit, plus, minus, point, quote, endstr };          //for the character category
        static char[] line;                                                                    //current line being processed
        static char ch;                                                                        //current character being processed
        static CharType chtype;                                                                //variable to store the type of the current character
        static StateType state;                                                                //current state of the application
        static int wlen;                                                                       //length of the current word
        static int k;                                                                          //subscript indicating present position within the current line
        static int len;                                                                        //length of the current line
        static bool isStateTransition;                                                         //keeps track of the state transition
        static int theInt;                                                                     //current integer encountered
        static double theDble;                                                                 //current double encountered
        static int theExpo;                                                                    //current exponent encountered
        static int sign;                                                                       //holds the sign of the current numeric constant encounterd
        static int eSign;                                                                      //holds the sign of the current exponent encountered
        static int power;                                                                      //keeps track the number of decimal places on the double
        #endregion.GLOBALS
        #region.MAIN
        static void Main(string[] args)
        {
            //========================================================================================================================================//
            //                                              MAIN BLOCK
            //========================================================================================================================================//
            int nLines = 4;
            int[] llen = new int[4];
            string[] lines = new string[] {
                "    first 123		and then -.1234 but you'll need 123.456		 and 7e-4 plus one like +321. all quite avant-",
                "garde   whereas ellen's true favourites are 123.654E-2	exponent-form which can also be -54321E-03 or this -.9E+5",
                "We'll prefer items like			fmt1-decimal		+.1234567e+05 or fmt2-dec -765.3245 or fmt1-int -837465 and vice-",
                "versa or even format2-integers -19283746   making one think of each state's behaviour for 9 or even 3471e-7 states " };

            //  PRINT OUT THE TEXT LINES AS SINGLE STRINGS FOLLOWED BY THIER LENGTH.
            Console.WriteLine("\n\nHERE ARE THE TEXT LINES PRINTED OUT AS SINGLE STRINGS ... EACH FOLLOWED BY ITS LENGTH. \n\n");
            for (k = 0; k < nLines; k++)
            {
                WriteLine(lines[k], "\n");
                llen[k] = lines[k].Length;
                WriteLine(llen[k]);
            }
            WriteLine("\n\n");
            ReadLine();

            //NOW PRINT OUT THE LINES 1 CHARACTER AT A TIME.
            WriteLine("\nHERE ARE THE SAME LINES AGAIN ... this time printed character by character.\n\n");

            state = StateType.white;                                                   //state type starting point

            for (int x = 0; x < nLines; x++)
            {
                line = lines[x].ToCharArray();                                          // change ith string into an array-of-characters
                len = line.Length;                                                      // grab length of this array

                for (int j = 0; j < len; j++)
                {
                    ch = line[j];                                                       // Grab jth character from current line array.
                    Write(ch);                                                          // Display each character               
                }

                Write("\n\n");

                ch = line[0];                                                           //grap the 1st character of the current line
                chtype = GetType(ch);                                                   //get the character type of the current character
                k = 0;                                                                  //resets position of the character to be processed

                while (k < len)
                {
                    switch (state)
                    {
                        case StateType.white:
                                WhiteState();
                                break;
                        case StateType.word:
                                WordState();
                                break;
                        case StateType.num:
                                NumState();
                                break;
                        case StateType.dble:
                                DbleState();
                                break;
                        case StateType.expo:
                                ExpoState();
                                break;
                    }

                }

                //last character of the line ends in hypen and in "Word State", stays in "Word State" else restarts at "White State"
                if (ch == '-' && state == StateType.word)
                    state = StateType.word;
                else
                    state = StateType.white;

                WriteLine("\n\nPress any key to continue...........\n\n");
                ReadKey();
            }

            ReadLine();
        }
        #endregion.MAIN
        #region.CharacterTypeMethods
        //========================================================================================================================================//
        //                                              Get Character Type
        //========================================================================================================================================//
        static CharType GetType(char chr)
        {
            CharType type = CharType.whsp;

            if (IsSpace(chr))
                type = CharType.whsp;
            else if (IsAlpha(chr))
            {
                if (ToUpper(chr) == 'E')
                    type = CharType.expo;
                else
                    type = CharType.lett;
            }
            else if (IsDigit(chr))
                type = CharType.digit;
            else
            {
                switch (chr)
                {
                    case '+':
                        type = CharType.plus;
                        break;
                    case '-':
                        type = CharType.minus;
                        break;
                    case '.':
                        type = CharType.point;
                        break;
                    case '\'':
                        type = CharType.quote;
                        break;
                }
            }
            return type;
        }
        static bool IsSpace(char c)
        {
            if (c == ' ')
                return true;
            else if (c == '\t')
                return true;
            else if (c == '\n')
                return true;
            else
                return false;
        }

        static bool IsDigit(char c)
        {   return (c >= '0' && c <= '9');}

        static bool IsAlpha(char c)
        {   return (ToUpper(c) >= 'A' && ToUpper(c) <= 'Z'); }

        static char ToUpper(char cha)
        {
            if (cha >= 'a' && cha <= 'z')
                cha = (char)(cha - ('a' - 'A'));

            return cha;
        }
        #endregion.CharacterTypeMethods
        #region.StateTransitionMethods
            #region.WhiteState
        //========================================================================================================================================//
        //                                                      WHITE STATE
        //========================================================================================================================================//
        static void WhiteState()
        {
            while (state == StateType.white && k < len)
            {
                switch (chtype)
                {
                    case CharType.lett:
                    case CharType.expo:
                            WhiteToWord();
                            isStateTransition = true;
                            break;
                    case CharType.digit:
                    case CharType.plus:
                    case CharType.minus:
                            WhiteToNum();
                            isStateTransition = true;
                            break;
                    case CharType.point:
                            WhiteToDble();
                            isStateTransition = true;
                            break;
                    default:
                            if (isStateTransition)
                                isStateTransition = false;

                            Write(ch);
                            break;
                }
                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }
        }
        //========================================================================================================================================//
        //                                                  WHITE TO WORD
        //========================================================================================================================================//
        static void WhiteToWord()
        {
            state = StateType.word;
            Write("ST1-2\n" + ch);
            wlen = 1;
        }
        //========================================================================================================================================//
        //                                              WHITE TO NUM
        //========================================================================================================================================//
        static void WhiteToNum()
        {
            state = StateType.num;
            Write("ST1-3\n" + ch);
            sign = 1;
            theInt = 0;

            if (ch == '-')
                sign = -1;

            if (!(ch == '-' || ch == '+'))
                theInt = theInt * 10 + ch - '0';
        }
        //========================================================================================================================================//
        //                                              WHITE TO DOUBLE
        //========================================================================================================================================//
        static void WhiteToDble()
        {
            state = StateType.dble;
            Write("ST1-4\n" + ch);
            power = 1;
            theDble = 0;
            sign = 1;
        }
        #endregion.WhiteState
            #region.WordState
        //========================================================================================================================================//
        //                                                  WORD STATE
        //========================================================================================================================================//
        static void WordState()
        {
            while (state == StateType.word && k < len)
            {
                if (chtype == CharType.whsp)
                    WordToWhite();
                else
                {
                    if (isStateTransition)
                        isStateTransition = false;

                    Write(ch);
                    wlen++;
                }

                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }
        }
        //========================================================================================================================================//
        //                                              WORD TO WHITE
        //========================================================================================================================================//
        static void WordToWhite()
        {
            state = StateType.white;
            Write("ST2-1\tWORD LENGTH: " + wlen + "\n" + ch);
        }
        #endregion.WordState
            #region.NumState
        //========================================================================================================================================//
        //                                                  NUM STATE
        //========================================================================================================================================//
        static void NumState()
        {
            while (state == StateType.num && k < len)
            {
                switch (chtype)
                {
                    case CharType.expo:
                            state = StateType.expo;
                            theDble = (double)theInt * sign;
                            Write("ST3-5\n" + ch);
                            isStateTransition = true;
                            break;
                    case CharType.point:
                            state = StateType.dble;
                            Write("ST3-4\n" + ch);
                            theDble = (double)theInt;
                            power = 1;
                            isStateTransition = true;
                            break;
                    case CharType.whsp:
                            state = StateType.white;
                            theInt = theInt * sign;
                            Write("ST3-1\tInteger Value: " + theInt + "\n" + ch);
                            isStateTransition = true;
                            break;
                    default:
                            if (isStateTransition)
                                isStateTransition = false;

                            Write(ch);
                            theInt = theInt * 10 + ch - '0';
                            break;
                }

                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }
            if (k == len)
                Write("ST3-1\tInteger Value: " + theInt);
        }
        #endregion.NumState
            #region.DoubleState
        //========================================================================================================================================//
        //                                              DOUBLE STATE
        //========================================================================================================================================//
        static void DbleState()
        {
            while (state == StateType.dble && k < len)
            {
                switch (chtype)
                {
                    case CharType.expo:
                            state = StateType.expo;
                            theDble = theDble / power * sign;
                            Write("ST4-5\n" + ch);
                            isStateTransition = true;
                            break;
                    case CharType.whsp:
                            state = StateType.white;
                            theDble = theDble / power * sign;
                            Write("ST4-1\tNormal-Format double value: " + theDble + "\n" +ch);
                            isStateTransition = true;
                            break;
                    default:
                            if (isStateTransition)
                                isStateTransition = false;

                            Write(ch);
                            power = power * 10;
                            theDble = theDble * 10 + ch - '0';
                            break;
                }

                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }
            if (k == len)
                Write("ST4-1\tNormal-Format double value: " + theDble);
        }
        #endregion.DoubleState
            #region.ExpoState
        //========================================================================================================================================//
        //                                              EXPONENT STATE
        //========================================================================================================================================//
        static void ExpoState()
        {
            eSign = 1;
            theExpo = 0;

            if (ch == '-')
                eSign = -1;

            if(ch == '-' || ch == '+')
            {
                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }
            else
                theExpo = theExpo * 10 + ch - '0';

            while (state == StateType.expo && k < len)
            {
                if (chtype == CharType.whsp)
                {
                    state = StateType.white;

                    if (eSign == 1)
                    {
                        for (int m = 0; m < theExpo; m++)
                            theDble = theDble * 10;
                    }
                    else
                    {
                        for (int m = 0; m < theExpo; m++)
                            theDble = theDble / 10;
                    }

                    Write("ST4-1\tExponent-Format double value: " + theDble + "\n" + ch);
                    isStateTransition = true;
                }
                else
                {
                    if (isStateTransition)
                        isStateTransition = false;

                    Write(ch);
                    theExpo = theExpo * 10 + ch - '0';
                }

                if (k < len - 1)
                {
                    ch = line[++k];
                    chtype = GetType(ch);
                }
                else
                    k++;
            }

            if(k == len)
            {
                if (eSign == 1)
                {
                    for (int m = 0; m < theExpo; m++)
                        theDble = theDble * 10;
                }
                else
                {
                    for (int m = 0; m < theExpo; m++)
                        theDble = theDble / 10;
                }

                Write("ST4-1\tExponent-Format double value: " + theDble);
            }

        }
        #endregion.ExpoState
        #endregion.StateTransitionMethods
    }
}
