using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookParser
{
    public class WordTracker
    {
        public string name { get; set; }
        public Dictionary<string, int> wordList { get; } = new Dictionary<string, int>();
        public int wordCount { set; get; } = 0;
        private int totalAlphabeticalCharacters = 0;

        public WordTracker(string name)
        {
            this.name = name;
        }

        public void addWord(string word)
        {
            totalAlphabeticalCharacters += word.Length;
            wordCount++;//increment total number of words in chapter
            if (wordList.ContainsKey(word)) //if this word already there
            {
                int newVal = wordList[word] + 1;
                wordList.Remove(word);
                wordList.Add(word, newVal);
            }
            else
            {
                wordList.Add(word, 1);
            }
        }

        public string getStats()
        {
            string maxCountValue = "";
            int maxCount = -1;
            foreach (KeyValuePair<string, int> pair in wordList)
            {
                if (!Parser.isTrivialWord(pair.Key))
                {
                    if (pair.Value == maxCount)
                    {
                        maxCountValue += " & " + pair.Key;
                    }
                    else if (pair.Value > maxCount)
                    {
                        maxCount = pair.Value;
                        maxCountValue = pair.Key;
                    }
                }
            }
            //maxCountValue = Char.ToUpper(maxCountValue[0]) + maxCountValue.Substring(1);

            string toReturn = name;
            toReturn += "\nNum. Words " + wordCount;
            toReturn += "\nNum. Unique words: " + wordList.Count;
            toReturn += "\nMost common non-trivial word(s): " + maxCountValue.ToUpper();
            toReturn += "\nTop three words related to each MCNTW:\n" + Parser.getRelatedWords(Parser.format(maxCountValue), 3);
            toReturn += "\nAverage word length: " + ("" + (Double)totalAlphabeticalCharacters / wordCount).Substring(0, 4);
            toReturn += searchForTerm();

            return toReturn + "\n\n";
        }

        public string searchForTerm()
        {
            String term = InterfacePage.getSearchTerm();
            term = term.ToLower().Trim();

            string toReturn = "\n";

            if (String.IsNullOrEmpty(term))
            {
                return toReturn;
            }

            if (wordList.ContainsKey(term))
            {
                toReturn += term.ToUpper() + ": " + wordList[term] + " occurences.";
            }
            else
            {
                toReturn += "(Term not found.)";
            }

            return toReturn;
        }

        public int sortValue()
        {
            if (!wordList.ContainsKey(Parser.format(InterfacePage.getSearchTerm())))
            {
                return -1;
            }
            return wordList[Parser.format(InterfacePage.getSearchTerm())];
        }

    }
}
