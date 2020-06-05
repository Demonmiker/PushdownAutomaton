using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Automaton
{
    public class PushdownAutomaton
    {
        //
        bool logging = false;
        string startState = "q0";
        char startStack = 'Z';
        string[] finalStates = new string[0];
        //

        Dictionary<PACInput, PACOutput> commands = new Dictionary<PACInput, PACOutput>();
        PACInput curCommand;
        Stack<char> Input = new Stack<char>();
        Stack<char> Stack = new Stack<char>();

        public PushdownAutomaton()
        {

        }

        public void AddCommand(string s)
        {
            string[] buf = s.Split(new char[] { ' ', '\t','-','>' },StringSplitOptions.RemoveEmptyEntries);
            if (buf.Length == 0) return;
            if (buf.Length != 5) throw new Exception($"Комманду {s} не удалось интерпритировать");
      
            commands.Add(new PACInput(buf[0], buf[1][0], buf[2][0]), new PACOutput(buf[3], buf[4]));

        }

        public bool ChangeProperty(string s)
        {
            string[] buf = s.Split(new char[] { ' ', '\t', '-', '>','=',',' }, StringSplitOptions.RemoveEmptyEntries);
            if (buf.Length == 0) return false;
            if (buf[0] == "#")
                return true;
            switch(buf[0].ToLower())
            {
                case "log":
                    logging = bool.Parse(buf[1]);
                    Console.WriteLine($"logging={logging}");
                    break;
                case "start":
                    startState = buf[1];
                    Console.WriteLine($"start state={startStack}");
                    break;
                case "stack":
                    startStack = char.Parse(buf[1]);
                    Console.WriteLine($"start stack symbol={startStack}");
                    break;
                case "f":
                    List<string> f = new List<string>();
                    Console.Write("Finals = ");
                    for(int i=1;i<buf.Length;i++)
                    {
                        Console.Write($"{buf[i]}  ");
                        f.Add(buf[i]);
                    }
                    Console.WriteLine();
                    finalStates = f.ToArray();
                    break;
            }
            return false;
        }

        public static PushdownAutomaton fromFile(string path)
        {
            PushdownAutomaton pa = new PushdownAutomaton();
            StreamReader sr = new StreamReader(path);
            bool cmds = false;

            while (!sr.EndOfStream && !cmds)
            {
                cmds = pa.ChangeProperty(sr.ReadLine());
            }
            while (!sr.EndOfStream)
            {
                pa.AddCommand(sr.ReadLine());
            }
            sr.Close();
            return pa;
        }

        bool processCommand()
        {
            PACOutput po;
            if (commands.ContainsKey(curCommand)) po = commands[curCommand];
            else
            {
                try
                {
                    po = commands.First((kv) => kv.Key.input == '^' && kv.Key.state == curCommand.state && kv.Key.stack == curCommand.stack).Value;
                }
                catch
                {
                    return true;
                }
                curCommand.input = '^';
            }
            //Input
            if (curCommand.input != '^') Input.Pop();
            curCommand.input = Input.Count > 0 ? Input.Peek() : '^';
            //Stack
            Stack.Pop();
            
            foreach (char c in po.stack.Reverse()) if(c!='^')Stack.Push(c);
            curCommand.stack = Stack.Count > 0 ? Stack.Peek() : '^';
            //state
            curCommand.state = po.state;
            return false;
            
        }

        public bool Process(string input)
        {
            //
            Input.Clear();
            foreach (char c in input.Reverse()) Input.Push(c);
            //
            Stack.Clear(); Stack.Push(startStack);
            //
            curCommand = new PACInput(startState, Input.Peek(), Stack.Peek());
            while(Stack.Count>0)
            {
                if(logging)
                    Log();
                if (processCommand())
                    return false;
            }
            return Input.Count == 0 && finalStates.Contains(curCommand.state);
        }

        public void Log()
        {
            Console.Clear();
            Console.WriteLine("<-------------->");
            Console.WriteLine($"State: [ {curCommand.state} ]");
            Console.Write($"Input: [ {(Input.Count > 0 ? Input.Peek() : '^')} ] \t"); 
            foreach (char c in Input)
                Console.Write(c);
            Console.WriteLine();
            Console.Write($"Stack: [ {Stack.Peek()} ] \t");
            foreach (char c in Stack)
                Console.Write(c);
            Console.WriteLine();
            Console.ReadKey();
            
        }

        public bool FindCycle(out string cycle)
        {
            cycle = "";
            int Q = commands.Select((x) => x.Key.state).Distinct().Count();
            List<char> A0 = commands.Values.Aggregate((x, y) => new PACOutput("", x.stack + y.stack)).stack.ToArray().Distinct().ToList();
            A0.Remove('^');
            int A = A0.Count();
            int L = commands.Values.Select((x) => x.stack).Max((x) => x.Length);
            Console.WriteLine($"#Г = {A}");
            Console.WriteLine($"l = {L}");
            Console.WriteLine($"#Q = {Q}");
            int n3 = 0;
            if(A==1)
            {
                n3 = Q*Q;
            }
            else
            {
                n3 = Q * ((int)Math.Pow(A, A * Q * L + 1) - A) / (A - 1);
            }
            var tickStates = from kv in commands
                             where kv.Key.input == '^'
                             select kv;
            foreach (var s in tickStates)
            {
                Input.Clear();
                Input.Push('^');
                //
                Stack.Clear(); Stack.Push(s.Key.stack);
                //
                curCommand = new PACInput(s.Key.state, Input.Peek(), Stack.Peek());
                int count = 0;
                while (Stack.Count > 0 && count<n3+1)
                {
                    if (processCommand()) break;
                    if (curCommand.input != '^') break;
                    count++;
                }
                if (count == n3 + 1) { cycle = $" {s.Key.state} {s.Key.input} {s.Key.stack} "; return true; }
            }
            return false;

        }





    }
}
