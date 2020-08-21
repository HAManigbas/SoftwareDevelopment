using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace hMANI_a3v3
{
    class Program
    {
        const int NMAX = 10;          //maximum size of each name string
        const int LSIZE = 20;         //number of actual name strings in array

        //array of name strings
        static string[] nam = new string[20] { "wendy", "ellen", "freddy", "tom", "susan",
                             "dick", "harry", "aloysius", "zelda", "sammy",
                             "mary", "hortense", "georgie", "ada", "daisy",
                             "paula", "alexander", "louis", "fiona", "bessie"  };

        //array of weights corresponding to these names
        static int[] wght = new int[20] { 120, 115, 195, 235, 138, 177, 163, 150, 128, 142,
                       118, 134, 255, 140, 121, 108, 170, 225, 132, 148 };

        static void Main(string[] args)
        {
              // title and headings
            string title_unsorted = "UNSORTED ARRAY DATA";
            string heading_name = "NAME".PadLeft(15);
            string heading_weight = "WEIGHT".PadLeft(15);

            OutLists(nam, wght, ref title_unsorted, ref heading_name, ref heading_weight);

            WriteLine("\n\nYou must sort once before searching.\n\n");

            //work Arrays of name and weight
            string[] WKnam = new string[LSIZE];
            int[] WKwght = new int[LSIZE];

            char choice;    //holds the user choice(1-3 for sorting and 4 for searching)
            bool oneSort = false;   //tracks if a sort was done (false value to disable search and true enables it)
            bool done = false;  //false by default to force enter the loop

            while(!done)
            {
                PutMenu();  //prints out choices (sorts and search)
                choice = GetChoice(oneSort);    //returns valid choice
                Clear();

                if(choice < '4')    //any of the sort was chosen
                {
                    CopyLists(WKnam, WKwght);    //copies original arrays to working arrays
                    DoSort(choice, WKnam, WKwght);
                    oneSort = true; //search section now enabled
                }
                else if (oneSort && choice == '4')  //sort done, user ready to search
                {
                    Bsrch();
                    done = true;    //sort and search were done, forces out of the loop
                }
            }

            ReadLine();
        }

        static void OutLists(string[] n, int[] w, ref string t, ref string h1, ref string h2)
        {
            WriteLine(t.PadLeft(30));
            WriteLine();
            Write(h1);
            WriteLine(h2);

            WriteLine("=======================================");

            for (int i = 0; i < LSIZE; i++)
            {
                WriteLine(n[i].PadLeft(15) + w[i].ToString().PadLeft(15));
            }

            WriteLine("=======================================\n\n");
        }


        static void CopyLists(string[] wkN, int[] wkW)
        {
            for (int i = 0; i < LSIZE; i++)
            {
                wkN[i] = nam[i];
                wkW[i] = wght[i];
            }
        }

        static void PutMenu()
        {
            WriteLine("\nMenu");
            WriteLine("1. Insertion Sort");
            WriteLine("2. Selection Sort");
            WriteLine("3. Shell Sort");
            WriteLine("4. Binary Search");
            WriteLine();
        }

        static char GetChoice(bool sortDone)
        {
            char ch;

            Write("Please enter your choice:  ");
            //gets user choice, reads every key
            ch = ReadKey(false).KeyChar; //deletes the key buffer

            while (ch < '1' || ch > '4')    //invalid choice
            {
                WriteLine("\nERROR! Please choose only between numbers 1 to 4 inclusively.");
                Write("Please enter your choice:  ");
                //gets user choice, reads every key
                ch = ReadKey(false).KeyChar; //deletes the key buffer

            }

            while(ch == '4' && sortDone == false)   //valid choice but no sort done yet
            {
                WriteLine("\nERROR! You must sort once before searching.");
                Write("Please enter your choice:  ");
                //gets user choice, reads only one character from  the keyboard
                ch = ReadKey(false).KeyChar; //deletes the key buffer
            }

            return ch;
        }

      

        static void DoSort(char ch, string[] wkN, int[] wkW)
        {
            switch(ch)
            {
                case '1':
                    {
                        InsertSort();
                        break;
                    }

                case '2':
                    {
                        SelectSort();
                        break;
                    }

                case '3':
                    {
                        ShellSort();
                        break;
                    }

                default:
                    {
                        WriteLine("ERROR! Something went wrong.");
                        break;
                    }
            }
        }

        static void InsertSort()
        {
            WriteLine("You choose insertion sort.");
        }
        static void SelectSort()
        {
            WriteLine("You choose selection sort.");
        }

        static void ShellSort()
        {
            WriteLine("You choose shell sort.");
        }

        static void Bsrch()
        {
            WriteLine("Welcome to Binary Search.....");
        }
    }
}
