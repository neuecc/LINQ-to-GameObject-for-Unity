using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using ZLinq;

[assembly: ZLinq.ZLinqDropInAttribute("", ZLinq.DropInGenerateTypes.Everything, DisableEmitSource = false)]

namespace ConsoleAppNetFramework48
{
    class Program
    {
        static void Main(string[] args)
        {
            var seq = ValueEnumerable.Range(1, 10);
            foreach (var item in seq)
            {
                Console.WriteLine(item);
            }
        }
    }
}
