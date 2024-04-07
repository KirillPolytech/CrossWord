using System.Linq;

public class WordManipulator
{
    public static int FindCharIndexInWord(string word, char chr)
    {
        if (word == null || chr == 0)
            return -1;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == chr)
                return i;
        }

        return -1;
    }

    public static CharacterData FindSameCharacter(CharacterData[] word, string nextWord)
    {
        return word.FirstOrDefault(t => nextWord.Any(t1 => t.desiredChar.ToString() == t1.ToString()));
    }
}
