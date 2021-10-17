using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDThreeNet;
namespace testIDthree
{
     class Program
     {
          static void Main(string[] args)
          {
               Dataset data = new Dataset();
               data.Extract();
               Algorithm id3 = new Algorithm(data);
               Node node = id3.ID3(id3.GetData("Play"), "Play",data.GetTrainingAttributes("Play"));
               Console.WriteLine(id3.printTree(node, 0.0));
               Console.Read();
          }
     }
}
