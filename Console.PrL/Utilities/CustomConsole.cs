using Console.PrL.Interfaces;

namespace Console.PrL.Utilities
{
    internal class CustomConsole : IConsole
    {
        private readonly string inputPrefix;

        private readonly string outputPrefix;

        public CustomConsole(string inputPrefix = "<-", string outputPrefix = "->")
        {
            this.inputPrefix = inputPrefix;
            this.outputPrefix = outputPrefix;
        }

        public string Input(string text)
        {
            System.Console.Write($"{this.inputPrefix} {text}");

            return System.Console.ReadLine().Trim();
        }

        public void Print(string text = "", string end = "\n")
        {
            System.Console.Write($"{this.outputPrefix} {text}{end}");
        }
    }
}
