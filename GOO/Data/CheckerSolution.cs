using System;
using System.Collections.Generic;
using System.Text;

namespace GOO.Utilities
{
    // day;truck;sequence number;order;
    public class Solution
    {
        public List<SolutionItem> Solution { get; private set; }

        public Solution() { }

        public Solution(string[] solution)
        {
            Solution = new List<SolutionItem>();

            for (int i = 0; i < solution.Length; i++)
            {
                string[] items = solution[i].Split(';');

                AddItem(new SolutionItem(new string[] { items[0], items[1], items[2], items[3] }));
            }

            SortList();
        }

        public void AddItem(SolutionItem Item)
        {
            Solution.Add(Item);
        }

        public void AddItem(string[] Item)
        {
            Solution.Add(new SolutionItem(Item));
        }

        private void SortList()
        {
            Solution.Sort(delegate(SolutionItem one, SolutionItem two) // This will sort the list on day, then truck and then sequence number
            {
                int compareDay = one.Day.CompareTo(two.Day);

                if (compareDay == 0)
                {
                    int compareTruck = one.TruckID.CompareTo(two.TruckID);

                    if (compareTruck == 0)
                        return one.SequenceNumber.CompareTo(two.SequenceNumber);

                    return compareTruck;
                }

                return compareDay;
            });
        }

        public void PrintSolution()
        {
            SortList();
            Console.WriteLine("The current solution:");
            for (int i = 0; i < Solution.Count; i++)
                Solution[i].Print();
        }

        public string GetSolution()
        {
            SortList();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Solution.Count; i++)
                sb.AppendLine(Solution[i].ToString());

            string tmp = sb.ToString();
            sb.Clear();
            sb = null;

            return tmp;
        }

        private class SolutionItem
        {
            public int Day { get; private set; }
            public int TruckID { get; private set; }
            public int SequenceNumber { get; private set; }
            public int OrderID { get; private set; }

            // Something along the lines of ...
            public Order Order { get; set; }

            public SolutionItem(string[] info)
            {
                Day = Convert.ToInt32(info[0]);
                TruckID = Convert.ToInt32(info[1]);
                SequenceNumber = Convert.ToInt32(info[2]);
                OrderID = Convert.ToInt32(info[3]);
            }

            public void Print()
            {
                Console.WriteLine("{0};{1};{2};{3}", Day, TruckID, SequenceNumber, OrderID);
            }

            public string ToString()
            {
                return String.Format("{0};{1};{2};{3}", Day, TruckID, SequenceNumber, OrderID);
            }
        }
    }

}