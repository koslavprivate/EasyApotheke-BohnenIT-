using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleForTestingSomething
{
    class Program
    {
        static void Main(string[] args)
        {

            string atr = "bit_easytrinkwasser";
            string atr2 = "easyT rinkwasser";

            string atrName = atr.Replace("bit_", "sit_") + "_2022";
            string atrName2 = atr2.ToLower().Replace(" ", ""); 
            //Console.WriteLine(atrName);
            Console.WriteLine(atrName2);
            Console.ReadLine();
        }
    }
}
