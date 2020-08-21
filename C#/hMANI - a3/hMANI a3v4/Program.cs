using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace hMANI_a3v4
{
    class Program
    {
        const int NMAX = 10;                //maximum size of each name string
        const int LSIZE = 20;               //number of actual name strings in array

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
            OutLists(nam, wght, "UNSORTED ARRAY DATA", "NAME", "WEIGHT");

            WriteLine("\n\nYou must sort once before searching.\n\n");

            //work Arrays of name and weight
            string[] WKnam = new string[LSIZE];
            int[] WKwght = new int[LSIZE];

            char choice;                                     //holds the user choice(1-3 for sorting and 4 for searching)
            bool oneSort = false;                            //tracks if a sort was done (false value to disable search and true enables it)
            bool done = false;                               //false by default to force enter the loop

            while (!done)
            {
                PutMenu();                                  //prints out choices (sorts and search)
                choice = GetChoice(oneSort);                //returns valid choice
                Clear();

                if (choice < '4')                           //any of the sort was chosen
                {           
                    CopyLists(WKnam, WKwght);               //refresh work arrays with unsorted data
                    DoSort(choice, WKnam, WKwght);
                    oneSort = true;                         //search section now enabled
                }
                else if (oneSort && choice == '4')          //sort done, user ready to search
                {
                    Bsrch();
                    done = true;                            //sort and search were done, forces out of the loop
                }
                else
                    WriteLine("\nERROR! You must sort once before searching.");
            }

            ReadLine();
        }

        static void OutLists(string[] n, int[] w, string t, string h_nam, string h_wght)
        {
            WriteLine(t.PadLeft(30));
            WriteLine();
            Write(h_nam.PadLeft(15));
            WriteLine(h_wght.PadLeft(15));

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
            ch = ReadKey(false).KeyChar;                                //gets user choice, reads every key
                                                                        //deletes the key buffer

            while (ch < '1' || ch > '4')                                //invalid choice
            {
                WriteLine("\nERROR! Please choose only between numbers 1 to 4 inclusively.");
                Write("Please enter your choice:  ");
                ch = ReadKey(false).KeyChar;                            //gets user choice, reads every key
                                                                        //deletes the key buffer
            }

            return ch;                                                 //returns valid choice
        }



        static void DoSort(char ch, string[] wkN, int[] wkW)
        {
            switch (ch)
            {
                case '1':
                    {
                        InsertSort(wkN, wkW);
                        OutLists(wkN, wkW, "INSERTION SORT OUTPUT", "NAME", "WEIGHT");
                        break;
                    }

                case '2':
                    {
                        SelectSort(wkN, wkW);
                        OutLists(wkN, wkW, "SELECTION SORT OUTPUT", "NAME", "WEIGHT");
                        break;
                    }

                case '3':
                    {
                        ShellSort(wkN, wkW);
                        OutLists(wkN, wkW, "SHELL SORT OUTPUT", "NAME", "WEIGHT");
                        break;
                    }

                default:
                    {
                        WriteLine("Development ERROR.");
                        break;
                    }
            }
        }

        static void InsertSort(string[] n, int[] w)
        {
            int k = 1;
            int size = n.Length;
            string yNam;
            int yWght;
            int i;
            bool found;

            do
            {
                yNam = n[k];
                yWght = w[k];
                i = k - 1;
                found = false;

                while(i >= 0 && !found)
                {
                    if (string.Compare(n[i], yNam) > 0)
                    {
                        n[i + 1] = n[i];
                        w[i + 1] = w[i];
                        i--;
                    }
                    else
                        found = true;
                }

                n[i + 1] = yNam;
                w[i + 1] = yWght;
                k++;

            } while (k <= (size - 1));

        }
        static void SelectSort(string[] n, int[] w)
        {
            int i = n.Length - 1;
            string bigNam;
            int bigWght;
            int where;
            int j;

            do
            {
                bigNam = n[0];
                bigWght = w[0];
                where = 0;
                j = 1;

                do
                {
                    if(string.Compare(n[j], bigNam) > 0)
                    {
                        bigNam = n[j];
                        bigWght = w[j];
                        where = j;
                    }

                    j++;

                } while (j <= i);

                n[where] = n[i];
                n[i] = bigNam;
                w[where] = w[i];
                w[i] = bigWght;
                i--;

            } while (i > 0);

        }

        static void ShellSort(string[] n, int[] w)
        {
            int[] gaplist = new int[LSIZE];
            int numgaps = GetGaps(gaplist);
            int i = numgaps - 1;
            int gap;
            int j;
            int k;
            int size = n.Length;
            string yNam;
            int yWght;
            bool found;

            do
            {
                gap = gaplist[i];
                j = gap;

                do
                {
                    yNam = n[j];
                    yWght = w[j];
                    k = j - gap;
                    found = false;

                    while (k >= 0 && !found)
                    {
                        if (string.Compare(n[k], yNam) > 0)
                        {
                            n[k + gap] = n[k];
                            w[k + gap] = w[k];
                            k = k - gap;
                        }
                        else
                            found = true;
                    }

                    n[k + gap] = yNam;
                    w[k + gap] = yWght;
                    j++;

                } while (j <= (size - 1));

                i--;

            } while (i >= 0);

        }

        static int GetGaps(int[] g)
        {
            int gap = 1;                                    //generate gap array
            int pos = 1;                                    //numgaps is passed by value and gaplist[] by reference to n and g[]

            while(gap < LSIZE)
            {
                g[pos] = gap;
                gap = gap * 3;
                pos = pos + 1;
            }

            return pos;                                     //returns valid number of gaps in the gaplist[]
        }

        static void Bsrch()
        {
            WriteLine("Welcome to Binary Search.....");
        }
    }
}
