using System;
using System.Collections.Generic;
using System.Linq;

namespace QwertyKeyboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new Utils().TypoDistance("qwe", "awe"));
        }
    }

    class InsertionAction
    {
        private int index;
        private char c;

        public InsertionAction(int index, char c)
        {
            this.index = index;
            this.c = c;
        }

        public double Cost(string s)
        {
            return 0.0;
        }

        public string Perform(string s)
        {
            return s.Substring(0, index) + c + s.Substring(index, s.Length - 1);
        }
    }

    class SubstitutionAction
    {
        private int index;
        private char c;

        public SubstitutionAction(int index, char c)
        {
            this.index = index;
            this.c = c;
        }

        public double Cost(string s)
        {
            return 0.0;
        }

        public string Perform(string s)
        {
            return s.Substring(0, index) + c + s.Substring(index + 1, s.Length - 1);
        }
    }

    class DeleteAction
    {
        private int index;

        public DeleteAction(int index)
        {
            this.index = index;
        }

        public double Cost(string s)
        {
            return 0.0;
        }

        public string Perform(string s)
        {
            return s.Substring(0, index) + s.Substring(index + 1, s.Length - 1);
        }
    }

    class Utils
    {
        private const double SHIFT_COST = 3.0;
        private const double INSERTION_COST = 1.0;
        private const double DELETION_COST = 1.0;
        private const double SUBTITION_COST = 1.0;

        private static char[][] qwertyKeyboardArray = {
            new char[] { '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '='},
            new char[] {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\'},
            new char[] {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\''},
            new char[] {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/'},
            new char[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
        };
        private static char[][] qwertyShiftedKeyboardArray = {
            new char[] { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+' },
            new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{', '}', '|' },
            new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '"' },
            new char[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?' },
            new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' }
        };

        struct QWERTY
        {
            public char[][] qwertyKeyboardArray => Utils.qwertyKeyboardArray;
            public char[][] qwertyShiftedKeyboardArray => Utils.qwertyShiftedKeyboardArray;
        }

        public char[][] ArrayForChar(char c)
        {
            QWERTY q = new QWERTY();
            if (q.qwertyKeyboardArray.SelectMany(x => x).Any(x => x.Equals(c))) return q.qwertyKeyboardArray;
            if (q.qwertyShiftedKeyboardArray.SelectMany(x => x).Any(x => x.Equals(c))) return q.qwertyShiftedKeyboardArray;
            throw new ArgumentException("Invalid character");
        }

        public int[] GetCharacterCoord(char c, char[][] keyboard)
        {
            int row = -1, column = -1;
            
            for(int i=0; i< keyboard.Length; i++)
            {
                char[] r = keyboard[i];
                if (r.Any(x => x.Equals(c)))
                {
                    return new int[] { i, Array.IndexOf(r, c)};
                }
            }
            return null;
        }

        public double EuclideanKeyboardDistance(char c1, char c2)
        {
            int[] coord1 = GetCharacterCoord(c1, ArrayForChar(c1));
            int[] coord2 = GetCharacterCoord(c2, ArrayForChar(c2));
            if(coord1 == null || coord2 == null)
                throw new ArgumentException("Incorrect character");
            return Math.Pow((Math.Pow(coord1[0] - coord2[0], 2) + Math.Pow(coord1[1] - coord2[1], 2)), 0.5);
        }

        public double InsertionCost(string s, int index, char c)
        {
            if(String.IsNullOrEmpty(s) || index >= s.Length)
                return INSERTION_COST;
            double cost = INSERTION_COST;
            if (!ArrayForChar(s[index]).Equals(ArrayForChar(c)))
                cost += SHIFT_COST;
            cost += EuclideanKeyboardDistance(s[index], c);
            return cost;
        }

        public double DeletionCost(string s, int index)
        {
            return DELETION_COST;
        }

        public double SubstitutionCost(string s, int index, char c)
        {
            double cost = SUBTITION_COST;
            if(s.Length == 0 || index >= s.Length)
                return INSERTION_COST;
            if (!ArrayForChar(s[index]).Equals(ArrayForChar(c)))
                cost += SHIFT_COST;
            cost += EuclideanKeyboardDistance(s[index], c);
            return cost;
        }

        public double TypoDistance(string s, string t)
        {
            double[,] d = new double[s.Length + 1, t.Length];

            for (int i = 0; i < s.Length + 1; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    d[i, 0] += DeletionCost(s, j - 1);
                }
            }

            for (int i = 0; i < t.Length + 1; i++)
            {
                string intermediateString = "";
                double cost = 0.0;
                for (int j = 0; j < i; j++)
                {
                    cost += InsertionCost(intermediateString, j - 1, t[j - 1]);
                    intermediateString = intermediateString + t[j - 1];
                }
                d[0, i] = cost;
            }

            for (int j = 1; j < t.Length + 1; j++)
            {
                for (int i = 1; i < s.Length + 1; i++)
                {
                    if (s[i - 1] == t[j - 1])
                        d[i, j] = d[i - 1, j - 1];
                    else
                    {
                        double deletionCost = DeletionCost(s, i - 1);
                        double insertionCost = InsertionCost(s, i, t[j - 1]);
                        double substitutionCost = SubstitutionCost(s, i - 1, t[j - 1]);
                        d[i, j] = new List<double>()
                        {
                            d[i - 1, j] + deletionCost,
                            d[i, j - 1] + insertionCost,
                            d[i - 1, j - 1] + substitutionCost
                        }.Min();
                    }
                }
            }

            return d[s.Length, d.Length];
        }
    }
}
