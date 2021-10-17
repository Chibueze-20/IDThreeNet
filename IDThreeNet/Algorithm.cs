using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDThreeNet
{
     public class Algorithm
     {
          Dictionary<string, int> categories = new Dictionary<string, int>();
          Dataset dataset;
          public Algorithm(Dataset dataset)
          {
               this.dataset = dataset;
          }
          public  List<Tuple<List<string>, int>> GetData(string target_attribute)
          {
               // get the index of the target column
               int index_of_Target = dataset.ColIndex(target_attribute);
               //get the integer representation for the unique targets
               Dictionary<string, int> Labels = to_categorical(dataset[target_attribute]);
               //Create Examples
               List<Tuple<List<string>, int>> examples = new List<Tuple<List<string>, int>>();
               //add each dataset row to the dataset
               for (int i = 0; i < dataset[target_attribute].Count; i++)
               {
                    // get row
                    List<string> row = dataset[i];
                    //get target
                    string target = row[index_of_Target];
                    // remove target from row
                    row.RemoveAt(index_of_Target);
                    //add as example .. converting target to its integer representation
                    examples.Add(new Tuple<List<string>, int>(row, categories[target]));
                     
               }
               return examples;
          }
          private Dictionary<string,int> to_categorical(List<string> targets)
          {
               int count = 0;
               foreach (string item in targets)
               {
                    if (categories.Count ==0)
                    {
                         categories.Add(item, count);
                         count++;
                    }
                    else if (!categories.Keys.Contains(item))
                    {
                         categories.Add(item, count);
                         count++;
                    }
               }
               return categories;
          }
          public Node ID3(List<Tuple<List<string>,int>> examples,string target_attribute,List<string> attributes)
          {
               foreach (string label in categories.Keys)
               {
                    int labelCount = examples.Count((x) => x.Item2 == categories[label]);
                    if (labelCount == examples.Count)
                    {
                         return new Node("leaf", true,"", categories[label]);
                    }
               }
               if (attributes.Count == 0)
               {
                    return new Node("leaf", true,"" ,MostCommonTargetValue(examples));
               }
               string attribute = BestAttributeByInformationGain(examples, attributes);
               Node root = new Node(attribute, false,"");
               SortedSet<string> setOfValues = new SortedSet<string>(dataset[attribute]);
               //for all values of attribute
               foreach (string item in setOfValues)
               {
                    //subset of examples with the current attribute with value as item
                    List<Tuple<List<string>, int>> subset = examples.FindAll((x) => x.Item1.Contains(item));
                    // if attribute values in examples does not have this type of possible value
                    if (subset.Count == 0)
                    {
                         root.GetChildren.Add(new Node(root.GetAttribute, true,"", MostCommonTargetValue(examples)));

                    }
                    else
                    {
                         root.GetChildren.Add(ID3(subset, target_attribute, attributes.FindAll((x) => x != root.GetAttribute)));
                    }
               }
               return root;
          }
          private int MostCommonTargetValue(List<Tuple<List<string>, int>> examples)
          {
               int mostCommonLabel=-1;
               int mostCommonLabelValue=-1;
               foreach (string label in categories.Keys)
               {
                    int count = examples.Count((x) => x.Item2 == categories[label]);
                    if (count > mostCommonLabelValue)
                    {
                         mostCommonLabel = categories[label];
                         mostCommonLabelValue = count;
                    }
               }
               return mostCommonLabel;
          }
          public string BestAttributeByInformationGain(List<Tuple<List<string>, int>> examples, List<string> attributes)
          {
               string bestAttribute="";
               double bestAttributeValue=-1;
               double gain;
               foreach (string item in attributes)
               {
                    gain = InformationGain(examples, item);
                    if (gain>bestAttributeValue)
                    {
                         bestAttribute = item;
                         bestAttributeValue = gain;
                    }
               }
               return bestAttribute;
          }
          public double InformationGain(List<Tuple<List<string>, int>> examples, string attribute)
          {
               SortedSet<string> setOfValues = new SortedSet<string>(dataset[attribute]);
               double sum=0;
               foreach (string item in setOfValues)
               {
                    // finding the row that contains the particular value irrespective of attribute column indexing
                    List<Tuple<List<string>, int>> subset = examples.FindAll((x) => x.Item1.Contains(item));
                    sum += (subset.Count / (double)examples.Count) * (Entropy(subset));
               }
               double gain = Entropy(examples) - sum;
               return gain;
          }
          public double Entropy(List<Tuple<List<string>, int>> examples)
          {
               List<int> targetCounts = new List<int>();
               double entropy = 0;
               foreach (string target in categories.Keys)
               {
                    // get the number of examples with target value as target
                    int targetCount = examples.Count((x) => x.Item2 == categories[target]);
                    //add entropy i.e -pi*log2(pi)
                    if (targetCount == 0)
                    {
                         entropy += 0;
                    }
                    else
                    {
                         double pi = targetCount / (double)examples.Count;
                         entropy += -1 * pi * (Math.Log10(pi) / Math.Log10(2));
                    }
               }
               return entropy;
          }
          
          public string printTree(Node root, double level)
          {
               if (root.IsLeaf)
               {
                    return root.GetAttribute + "Label" + categories.Keys.ToList()[root.GetLabel.Value];
               }
               else
               {
                    string node = root.GetAttribute + "-->" + level;
                    for (int i = 0; i < root.GetChildren.Count; i++)
                    {
                         node += "\n" + printTree(root.GetChildren[i], (1 + level + (i / 10.0)));
                    }
                    return node;
               }
          }
     }
     public class Dataset
     {
          Dictionary<string, List<string>> Data = new Dictionary<string, List<string>>();
          public  void Extract()
          {
               Data.Add("Outlook", new List<string>() { "S", "S", "O", "R", "R", "R", "O", "S", "S", "R", "S", "O" });
               Data.Add("Temp", new List<string>() { "H", "H", "H", "M", "C", "C", "C", "M", "C", "M", "M", "M" });
               Data.Add("Humid", new List<string>() { "Hi", "Hi", "Hi", "Hi", "N", "N", "N", "Hi", "N", "N", "N", "Hi" });
               Data.Add("Wind", new List<string>() { "W", "Sw", "W", "W", "W", "Sw", "Sw", "W", "W", "W", "Sw", "Sw" });
               Data.Add("Play", new List<string>() { "N", "N", "Y", "Y", "Y", "N", "Y", "N", "Y", "Y", "Y", "Y" });
          }
          public List<string> GetTrainingAttributes(string target_attribute)
          {
               List<string> attributes = Data.Keys.ToList();
               if (attributes.Remove(target_attribute))
               {
                    return attributes;
               }
               return null;
          }
          public List<string> this [string attribute]
          {
               get
               {
                    return Data[attribute];
               }
          }
          public List<string> this [int index]
          {
               get
               {
                    List<string> row = new List<string>();
                    foreach (string item in Data.Keys)
                    {
                         row.Add(Data[item][index]);
                    }
                    return row;
               }
          }
          public int ColIndex (string attribute)
          {
               
              return Data.Keys.ToList().IndexOf(attribute);     
          } 
     }
}
