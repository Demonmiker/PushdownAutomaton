using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    
    struct PACInput
    {
        public string state;
        public char input;
        public char stack;

        public PACInput(string state, char input, char stack)
        {
            this.state = state;
            this.input = input;
            this.stack = stack;
        }

        public void Write(PACOutput o)
        {
            Console.WriteLine($"{state} {input} {stack} -> {o.state} {o.stack}");
            Console.ReadKey();
        }

        public override string ToString()
        {
            return $"{state} {input} {stack}";
        }
    }
    struct PACOutput
    {
        public string state;
        public string stack;

        public PACOutput(string state, string stack)
        {
            this.state = state;
            this.stack = stack;
        }
    }
}
