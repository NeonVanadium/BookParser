using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookParser
{
    class Parser
    {
        private string[] text;
        public ObservableCollection<WordTracker> chapters { get; } = new ObservableCollection<WordTracker>();
        private WordTracker curChapter;
        public WordTracker fullTextWordTracker { get; }

        public static string[] trivialWords = new string[] //a few of the most common english words, with synonyms or other forms of said word on the same line
        {
                "the",
                "be", "are", "am", "is", "was", "were", "being", "been",
                "to",
                "of",
                "and",
                "a", "an",
                "in",
                "that",
                "have", "has", "had",
                "I", "me", "my", "mine",
                "it", "its",
                "for",
                "not",
                "on",
                "with",
                "he",
                "as",
                "you",
                "do", "does", "did",
                "at",
                "this",
                "but",
                "his", "him",
                "by",
                "from",
                "they", "them",
                "we", "us",
                "say", "said",
                "her",
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
 

                //other common ones
                "which",
                "when",
                "while",
                "one",
                "than",
                "then",
        };

        public Parser(string textPath)
        {
            text = System.IO.File.ReadAllLines(textPath);
            fullTextWordTracker = new WordTracker(textPath.Substring(textPath.IndexOf('/') + 1, textPath.Length - 4 - "Assets/".Length));

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
                            fullTextWordTracker.addWord(word);
                            curChapter.addWord(format(word));
                        }     
                    }
                }

            }


            //parse that boi

        }

        public string getTextStats()
        {
            return "";
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

        private bool endsInPunctuation(string word)
        {
            char final = word.Last();
            if (final == ';' || final == ',' || final == '.' || final == '!' || final == '?' || final == ':' || final == '"' || final == ')')
            {
                return true;
            }
            return false;
        }

        private bool startsWithPunctuation(string word)
        {
            char first = word.First();
            if(first == '"' || first == '(')
            {
                return true;
            }
            return false;
        }

        private bool isPossessive(string word)
        {
            if (word.EndsWith("'s"))
            {
                return true;
            }
            return false;
        }

        private string format(string word) //formats/cleans the word
        {
            word = word.Trim();
            while(endsInPunctuation(word))
            {
                word = word.Substring(0, word.Length - 1);
            }
            while(startsWithPunctuation(word)){
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

    }
}
