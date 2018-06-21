using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookParser
{
    public class Parser
    {
        private string[] text;
        public ObservableCollection<WordTracker> chapters { get; set; } = new ObservableCollection<WordTracker>();
        private WordTracker curChapter;
        public WordTracker fullTextWordTracker { get; }
        public string searchTerm { set; get; } = "";
        public static Dictionary<string, Dictionary<string, int>> relatopedia;
        private LinkedList<string> wordList;  //for the last some number of words that have been visited
        private static int listSize = 15;

        public static string[] trivialWords = new string[] //a few of the most common english words, with synonyms or other forms of said word on the same line
        {
                "the",
                "be", "are", "am", "is", "was", "were", "being", "been",
                "to",
                "of",
                "and",
                "a", "an",
                "in",
                "that", "those",
                "have", "has", "had",
                "I", "me", "my", "mine",
                "it", "its", "it's",
                "for",
                "not",
                "on",
                "with",
                "he", "his",
                "as",
                "you", "your", "thou", "thine", "thee", "thy",
                "do", "does", "did", "doing",
                "at",
                "this", "these",
                "but",
                "his", "him",
                "by",
                "from",
                "they", "them", "their", "theirs",
                "we", "us",
                "say", "said",
                "her", "hers",
                "she",
                "or",
                "will",
                "my",
                "all",
                "would",
                "there",
                "their", "theirs",
                "what",
                "so",
                "up",
                "out",
                "if",
                "some",
                "may",
                "do", "don't",
 

                //other common ones
                "which",
                "when",
                "while",
                "one",
                "than",
                "then",
                "no",
        };

        public Parser(string textPath)
        {
            text = System.IO.File.ReadAllLines(textPath);
            fullTextWordTracker = new WordTracker(textPath.Substring(textPath.IndexOf('/') + 1, textPath.Length - 4 - "Assets/".Length));
            relatopedia = new Dictionary<string, Dictionary<string, int>>();
            wordList = new LinkedList<string>();

            foreach(String line in text)
            {
                if (line.StartsWith("//"))
                {
                    curChapter = new WordTracker(line.Substring(2));
                    chapters.Add(curChapter);
                }
                else if(curChapter != null)
                {
                    foreach (String word in line.Split(' '))
                    {
                        if (isValid(word)) { //if this is an actual sequence of letters and not literal nothing

                            string formatted = format(word);

                            if (!isTrivialWord(formatted)) //we only care about the connotations of non-trivial words
                            {
                                int distance = 0;
                                foreach (string s in wordList)
                                {
                                    relatopediaAdd(formatted, s, listSize - distance);
                                    relatopediaAdd(s, formatted, listSize - distance);
                                    distance++;
                                }

                                enqueue(formatted);
                            }
                            
                            fullTextWordTracker.addWord(formatted);
                            curChapter.addWord(formatted);
                        }     
                    }
                }

            }


            //parse that boi

        }

        public static string getRelatedWords(string words, int amount)
        {

            string toWord;
            string toReturn = "";
            string temp = "";
            bool newlined = true;

            foreach (string s in words.Split('&'))
            {

                newlined = true;
                if (toReturn != "")
                {
                    toReturn += "\n";
                }

                toWord = s.Trim();

                for (int i = 0; i < amount; i++)
                {

                    if (toReturn != "" && !newlined)
                    {
                        toReturn += ", ";
                    }

                    if (relatopedia.ContainsKey(toWord))
                    {
                        int max = -1000;
                        foreach (KeyValuePair<string, int> pair in relatopedia[toWord])
                        {
                            if (pair.Value > max && !toReturn.Contains(pair.Key.ToUpper()))
                            {
                                max = pair.Value;
                                temp = pair.Key.ToUpper();
                            }
                        }
                    }

                    newlined = false;
                    toReturn += temp;

                }

            }

            if (toReturn == "")
            {
                return "(Word is non-existent or trivial)";
            }
            return toReturn;
        }

        private void enqueue(String word)
        {
            if(wordList.Count > listSize)
            {
                wordList.RemoveLast();
            }

            wordList.AddFirst(word);
        }

        private void relatopediaAdd(string key, string relateTo, int relation)
        {
            if(key == relateTo)//words should not relate to themselves
            {
                return;
            }

            if (relatopedia.ContainsKey(key))
            {
                if (relatopedia[key].ContainsKey(relateTo))
                {
                    relatopedia[key][relateTo] += relation;
                }
                else
                {
                    relatopedia[key][relateTo] = relation;
                }
            }
            else
            {
                relatopedia[key] = new Dictionary<string, int>();
                relatopedia[key].Add(relateTo, relation);
            }
        }

        public string getAllChapters()
        {
            string all = "";

            foreach(WordTracker c in chapters)
            {
                all += c.getStats();
            }

            return all;
        }


        private bool isValid(string word) //determines if a given word is a word and isn't nonexistent or composed of nonalphabetical characters
        {
            if (String.IsNullOrEmpty(word)) return false;

            foreach(char c in word)
            {
                if (Char.IsLetter(c))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool isPossessive(string word)
        {
            if (word.EndsWith("'s"))
            {
                return true;
            }
            return false;
        }

        public static string format(string word) //formats/cleans the word
        {
            word = word.ToLower().Trim();
            while(!Char.IsLetter(word[word.Length - 1]))//while the last element of the string isn't a letter
            {
                word = word.Substring(0, word.Length - 1);
            }
            while(!Char.IsLetter(word[0])){//while the first element of the string isn't a letter
                word = word.Substring(1);
            }
            if (isPossessive(word))
            {
                word = word.Substring(0, word.Length - 3);
            }
            return word;
        }

        public static bool isTrivialWord(string word)
        {
            foreach(string t in trivialWords)
            {
                if(word.ToLower() == t.ToLower()) //if the word is in the list of trivial words
                {
                    return true;
                }
            }
            return false;
        }

        public void sortByPrevalence() //sorts the list of chapters by the frequency of the current search term, with chapters that have no occurences sorted to the bottom
        {

            int length = chapters.Count;
            WordTracker temp = null;

            for(int i = 0; i < length; i++)
            {
                for(int j = i; j < chapters.Count(); j++)
                {
                    if(chapters[j].sortValue() > chapters[i].sortValue())
                    {
                        temp = chapters[i];
                        chapters[i] = chapters[j];
                        chapters[j] = temp;
                    }
                }
            }

            InterfacePage.updateCurParserChapters(chapters);

        }

    }
}
