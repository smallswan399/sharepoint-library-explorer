using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NativeExports
{
    class Program
    {
        [DllExport]
        static void NE_Rainbow()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('R');
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write('a');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('i');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('n');
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('b');
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write('o');
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write('w');

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
