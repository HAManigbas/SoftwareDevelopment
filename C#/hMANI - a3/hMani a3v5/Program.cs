/*********************************************************************************************************************************************
 * Due Date:             October 22, 2018
 * Sofware Designer:     Hazel Ann Manigbas
 * Course:               420-306-AB (Fall 2018)
 * Deliverable:          Assignment  #3 --- Sorting and Searching
 * 
 * Description:          The program consist of a Sort Section and a Search Section.
 *                       An unsorted array 'nam' of string first-names and an array 'wght' of corresponding integer body 
 *                       weights[in pounds], are initialized.
 *                       It sorts the two arrays with respect to name.
 *                       Once the array are sorted, then the program processes a series of simple search transactions as follows:
 *                          - program reads string input names 'xnam' from the keyboard, and generates 1 of 2 possible responses
 *                              depending whether 'xnam' was found within the array 'nam' or not
 *********************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace hMani_a3v5
{
    class Program
    {
        //==================================================GLOBAL VARIABLE=================================================================//
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
        //==================================================================================================================================//

        static void Main(string[] args)
        {
            //work Arrays of name and weight
            string[] WKnam = new string[LSIZE];
            int[] WKwght = new int[LSIZE];

            char choice;                                     //holds the user choice(1-3 for sorting and 4 for searching)
            bool oneSort = false;                            //tracks if a sort was done (false value to disable search and true enables it)
            bool done = false;                               //false by default to force enter the loop

            //====================================================MAIN BLOCK=================================================================//
            OutLists(nam, wght, "UNSORTED ARRAY DATA", "NAME", "WEIGHT");

            WriteLine("\n\nYou must sort once before searching.");

            while (!done)                                  //no sort nor search is done
            {
                PutMenu();                                  //prints out choices (sorts and search)
                choice = GetChoice();                       //returns valid choice
                Clear();

                if (choice < '4')                           //any of the sort was chosen
                {
                    CopyLists(WKnam, WKwght);               //refresh work arrays with unsorted data
                    DoSort(choice, WKnam, WKwght);
                    oneSort = true;                         //search section now enabled
                }
                else if (oneSort && choice == '4')          //sort done, user ready to search
                {
                    SearchSection(WKnam, WKwght);
                    done = true;                            //sort and search were done, forces out of the loop
                }
                else                                        //no sort done yet
                    WriteLine("\nERROR! You must sort once before searching.");
            }

            ReadLine();
            //==================================================================================================================================//
        }


        //===============================================================OUTLIST================================================================//
        //  *Prints out the unsorted/sorted list of the arrays, passing the arrays, title and column headings
        //======================================================================================================================================//
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

        //==============================================================COPYLIST================================================================//
        //  *Copy two original arrays into 2 work arrays which will actually be sorted
        //======================================================================================================================================//
        static void CopyLists(string[] wkN, int[] wkW)
        {
            for (int i = 0; i < LSIZE; i++)
            {
                wkN[i] = nam[i];
                wkW[i] = wght[i];
            }
        }

        //==============================================================PUT MENU================================================================//
        //  *Shows three sort menu choices and a search choice which can't be done unless one sort is done
        //======================================================================================================================================//
        static void PutMenu()
        {
            WriteLine("\n\nMenu");
            WriteLine("1. Insertion Sort");
            WriteLine("2. Selection Sort");
            WriteLine("3. Shell Sort");
            WriteLine("4. Binary Search\n");
        }

        //============================================================GET CHOICE================================================================//
        //  *Prompt the user, get a single-character as a choice for the sort algorithm
        //======================================================================================================================================//
        static char GetChoice()
        {
            char ch;                                                    //value to be returned, valid choice

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


        //============================================================DO SORT===================================================================//
        //  *Executes the chosen sort algorithm
        //======================================================================================================================================//
        static void DoSort(char ch, string[] wkN, int[] wkW)
        {
            switch (ch)
            {
                case '1':                                                               //insertion sort chosen
                    {
                        InsertSort(wkN, wkW);
                        OutLists(wkN, wkW, "INSERTION SORT OUTPUT", "NAME", "WEIGHT");  //Prints out the result of insertion sort
                        break;
                    }

                case '2':                                                               //Selection sort chosen
                    {
                        SelectSort(wkN, wkW);
                        OutLists(wkN, wkW, "SELECTION SORT OUTPUT", "NAME", "WEIGHT");  //Prints out the result of selection sort
                        break;
                    }

                case '3':                                                               //Shell sort chosen
                    {
                        ShellSort(wkN, wkW);                                            
                        OutLists(wkN, wkW, "SHELL SORT OUTPUT", "NAME", "WEIGHT");      //Prints out result of shell sort
                        break;
                    }

                default:                                                                //default case
                    {
                        WriteLine("Development ERROR.");                                //will be showed when there's an error with the codes
                        break;
                    }
            }

            Write("\n\nPress any key to continue....");
            ReadKey();
        }

        //==========================================================INSERTION SORT==============================================================//
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

                while (i >= 0 && !found)
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

        //==========================================================SELECTION SORT==============================================================//
        static void SelectSort(string[] n, int[] w)
        {
            int i = n.Length - 1;                                   //current biggest index
            string bigNam;                                          //current biggest name value
            int bigWght;                                            //current biggest weight value
            int where;                                              //position of current biggest
            int j;                                                  //position of the current value to be compared with the current biggest value

            do
            {
                bigNam = n[0];                                      //current biggest name starts/resets at index 0
                bigWght = w[0];                                     //current biggest weight strats/resets at index 0
                where = 0;                                          //position of current biggest starts/resets at index 0
                j = 1;                                              //position of the current value to be compared starts/resets at index 1

                do
                {
                    if (string.Compare(n[j], bigNam) > 0)           //new current biggest value found
                    {
                        bigNam = n[j];                              //resets current biggest name
                        bigWght = w[j];                             //resets current biggest weight
                        where = j;                                  //resets position of the current biggest
                    }

                    j++;                                            //resets the position of the current value to be compared

                } while (j <= i);                                   //do til biggest array index is reached

                n[where] = n[i];                                    //swaps the name position i to posiition of current biggest
                n[i] = bigNam;                                      //name position i gets the biggest value found
                w[where] = w[i];                                    //swaps the weight position i to posiition of current biggest
                w[i] = bigWght;                                     //weight position i gets the biggest value found
                i--;                                                //resets current highest index

            } while (i > 0);

        }

        //===========================================================SHELL SORT===============================================================//
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

        //===========================================================GET GAPS===============================================================//
        static int GetGaps(int[] g)
        {
            int gap = 1;                                    //generate gap array
            int pos = 1;                                    //numgaps is passed by value and gaplist[] by reference to n and g[]

            while (gap < LSIZE)
            {
                g[pos] = gap;
                gap = gap * 3;
                pos = pos + 1;
            }

            return pos;                                     //returns valid number of gaps in the gaplist[]
        }

        //==========================================================BINARY SEARCH=============================================================//
        static int Bsrch(string[] wn, string nam)
        {
            int bsrch = -1;                                   //index of the search value, default at -1(not found)
            int mid;                                          //current middle index
            int size = wn.Length;                             //array size
            int low = 0;                                      //current lowest index
            int high = size - 1;                              //current highest index
            nam = nam.ToLower();                              //in case the user types in uppercase

            while(low <= high)
            {
                mid = (low + high) / 2;                       //resets current middle index

                if(nam == wn[mid])                            //search name found
                {
                    bsrch = mid;                              //search name index
                    low = size;                               //forces out of the loop
                }
                else
                {
                    if (string.Compare(wn[mid], nam) > 0)    //shrinks the array values to be compared
                        high = mid - 1;                      //resets current highest index
                    else
                        low = mid + 1;                       //rests current lowest index
                }
            }

            return bsrch;                                   //returns the index of the search value
        }

        //========================================================SEARCH SECTION================================================================//
        //  *Prompt the user, gets the 'name' they want to search
        //  *Calls out the Bsrch() to do the search
        //  *Prints search result(found/notfound)
        //======================================================================================================================================//
        static void SearchSection(string[] n, int[] w)
        {
            string xnam;                            //name to be searched
            int xIndex;                             //position of the found name

            WriteLine("Welcome to search section...........");

            Write("\nPlease enter the name you wish to search:  ");
            xnam = ReadLine();

            while(xnam == "")                        //first search can't be empty
            {
                WriteLine("\nERROR! Can't complete an empty search. You must search at least once");
                Write("\nPlease enter the name you wish to search:  ");
                xnam = ReadLine();
            }
        
            while(xnam != "")                        //entering the search loop
            {
                xIndex = Bsrch(n, xnam);             //returns the position of the found name

                if (xIndex < 0)                     //name not found
                    WriteLine("\n" + xnam + " was not found");
                else                                //name found
                    WriteLine("\n" + xnam + " was found in position " + (xIndex + 1) + " and has the body weight of " + w[xIndex]);

                Write("\n\nPress any key to continue....");
                ReadKey();
                Clear();

                Write("\nPlease enter the name you wish to search (or press ENTER to exit):  ");
                xnam = ReadLine();
            }
        }
    }
}
