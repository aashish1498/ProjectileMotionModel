using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockAI
{
    class SaveMethods
    {
        private const string Delimeter = ",";
        public SaveMethods(string filePath)
        {
            _filePath = filePath;
        }

        private readonly string _filePath;

        public void SaveInputs(RandomPositionGenerator randomPositions)
        {
            int simNumber = File.ReadAllLines(_filePath).Count();
            var allVariables = randomPositions.AllVariables;
            var csv = new StringBuilder();
            csv.Append(simNumber + Delimeter);
            foreach (DictionaryEntry kvp in allVariables)
            {
                if (Math.Abs(Convert.ToDouble(kvp.Key) - (-1)) < 0.1) //Wind value or Radius
                {
                    csv.Append(kvp.Value);
                    csv.Append(Delimeter);
                }
                else
                {
                    csv.Append(kvp.Key);
                    csv.Append(Delimeter);
                    csv.Append(kvp.Value);
                    csv.Append(Delimeter);
                }
            }
            File.AppendAllText(_filePath,csv.ToString());
        }

        public void SaveOutputs(string power, string angle)
        {
            var csv = new StringBuilder();
            csv.Append(power + Delimeter + angle + Delimeter + Environment.NewLine);
            File.AppendAllText(_filePath, csv.ToString());
        }

    }
}
