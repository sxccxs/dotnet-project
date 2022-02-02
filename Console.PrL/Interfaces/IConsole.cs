using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.PrL.Interfaces
{
    public interface IConsole
    {
        string Input(string text);

        void Print(string text = "", string end = "\n");
    }
}
