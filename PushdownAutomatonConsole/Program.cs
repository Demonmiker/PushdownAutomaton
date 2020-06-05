using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automaton;

namespace PushdownAutomatonConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "Program.txt";
            do
            {
                Console.Write("File:");
                string s = Console.ReadLine();
                if(s=="q")
                    break;
                if (s != "p")
                    file = s;
                PushdownAutomaton PA = PushdownAutomaton.fromFile(file);
                if (PA.FindCycle(out string cc)) { Console.WriteLine("Найден цикл!!"); Console.WriteLine($"Зацикливающая конфигурация: {cc}"); }
                else
                {
                    Console.Write("Input:");
                    Console.WriteLine(PA.Process(Console.ReadLine()));
                    Console.ReadKey();
                    Console.Clear();
                }
                
            }
            while (true);
            



        }
    }
}
