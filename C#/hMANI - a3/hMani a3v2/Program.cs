using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMani_a3v2
{
    class Program
    {
        const int NMAX = 10;          //maximum size of each name string
        const int LSIZE = 20;         //number of actual name strings in array
        static void Main(string[] args)
        {
            //array of name strings
            string[] nam = new string[20] { "wendy", "ellen", "freddy", "tom", "susan",
                             "dick", "harry", "aloysius", "zelda", "sammy",
                             "mary", "hortense", "georgie", "ada", "daisy",
                             "paula", "alexander", "louis", "fiona", "bessie"  };

            //array of weights corresponding to these names
            int[] wght = new int[20] { 120, 115, 195, 235, 138, 177, 163, 150, 128, 142,
                       118, 134, 255, 140, 121, 108, 170, 225, 132, 148 };

            // title and headings
            string title_unsorted = "UNSORTED ARRAY DATA".PadLeft(30);
            string heading_name = "NAME".PadLeft(15);
            string heading_weight = "WEIGHT".PadLeft(15);

            OutLists(nam, wght, ref title_unsorted, ref heading_name, ref heading_weight);

            Console.ReadLine();
        }

        static void OutLists(string[] n, int[] w, ref string t, ref string h1, ref string h2)
        {
            Console.WriteLine(t);
            Console.WriteLine();
            Console.Write(h1);
            Console.WriteLine(h2);

            Console.WriteLine("=======================================");

            for (int i = 0; i < LSIZE; i++)
            {
                Console.WriteLine(n[i].PadLeft(15) + w[i].ToString().PadLeft(15));
            }

            Console.WriteLine("=======================================");


            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
