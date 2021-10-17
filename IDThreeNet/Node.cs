using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDThreeNet
{
     /// <summary>
     /// 
     /// </summary>
     /// <typeparam name="T1">The type of the attribute.</typeparam>
     /// <typeparam name="T2">The type of the label.</typeparam>
     public struct Node
    {
          private string Attribute;
          private bool isLeaf;
          private int? label;
          private string split;
          private IList<Node> children;

          public Node(string attribute, bool isleaf,string split,int? label=null)
          {
               this.Attribute = attribute;
               this.label = label;
               this.isLeaf = isleaf;
               this.split = split;
               children = new List<Node>();
          }
          public string GetSplit { get { return split; } }
          public string GetAttribute { get { return Attribute; }}
          public int? GetLabel { get { return label; }}
          public bool IsLeaf { get { return isLeaf; }}
          public IList<Node> GetChildren
          {
               get
               {
                    if (isLeaf)
                    {
                         return null;
                    }
                    else
                    {
                         return children;
                    }
               }
          }
     }
}
