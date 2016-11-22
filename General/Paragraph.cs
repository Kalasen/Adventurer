using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    class Paragraph //TODO: Make public?
    {
        public List<string> lines = new List<string>(); //The lines of the paragraph

        public Paragraph()
        {
        }
        public Paragraph(string[] text)
        {
            this.lines = new List<string>(text);
        }
        public Paragraph(string[] text, int charPerLine)
        {
            this.lines = new List<string>(text);
            WordWrap(charPerLine); //Adjust
        }

        public void Add(string s)
        {
            lines.Add(s);
        }
        public void Clear()
        {
            lines.Clear();
        }
        public string[] WordWrap(int charPerLine)
        {
            List<string> allWords = new List<string>();
            List<string> newData = new List<string>();

            foreach (string s in lines) //For every line...
            {                
                foreach (string w in s.Split(' ')) //Split by space to get all the words
                {
                    allWords.Add(w);
                }
            }

            string newLine = String.Empty;
            foreach (string s in allWords) //For each word
            {
                if (newLine.Length + s.Length < charPerLine)
                {
                    newLine += s + " "; //Add the word to the line
                }
                else
                {
                    newData.Add(newLine);
                    newLine = s + " "; //Start a new line
                }
            }
            newData.Add(newLine); //Add the final line
            newData[newData.Count - 1] = newData[newData.Count - 1].Trim(); //Trim off that extra space on the end

            lines = new List<string>(newData); //Set the paragraph's data to the new data.
            
            return lines.ToArray(); //Return the newly word wrapped paragraph
        }
    } //For sorting text into paragraphs
}
