using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CrossWord : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject wordParentPrefab;
    [SerializeField] private GameObject blockPrefab;

    [Range(0f,1f)][SerializeField] private float distanceBetweenBlocks = 1f;

    [Range(2,32)][SerializeField] private int crosswordLength = 10;

    private readonly string _pathToWords = "Assets/Resources/Words.txt";

    private readonly HashSet<int> chosenWords = new HashSet<int>();
    private readonly HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private const int ITERATIONLIMIT = 10000;

    private string[] words;
    private int[] horizontalWordIndexes, verticalWordIndexes;
    private void Awake()
    {
        words = new StreamReader(_pathToWords).ReadToEnd().Split("\r\n");

        for (int i = 0; i < words.Length; i++)
            words[i] = words[i].ToLower();       

        horizontalWordIndexes = new int[crosswordLength];
        verticalWordIndexes = new int[crosswordLength];
    }

    public void Generate()
    {
        int wordInd = GetRandomWordIndex(words.Length);
        Destroy();
        GenerateCrossword(wordInd, words);
    }

    public void Destroy()
    {
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }

        chosenWords.Clear();
        occupiedPositions.Clear();
    }

    private void GenerateCrossword(int wordInd, string[] words)
    {
        Vector3 startPos = Vector3.zero;
        Vector3 dir = -Vector3.up * distanceBetweenBlocks;

        CharacterData sameChar = new CharacterData();
        CharacterData[] wordCharacters = SpawnWord(startPos, dir, words[wordInd], sameChar.inputField?.text);
        chosenWords.Add(wordInd);
        startPos = Vector3.zero - dir * FindCharIndex(wordCharacters, words[wordInd]);
        for (int j = 0; j < crosswordLength; j++)
        {
            if ((j + 1) % 2 == 0)
                dir = -Vector3.up * distanceBetweenBlocks;
            else
                dir = Vector3.right * distanceBetweenBlocks;

            int y = 0;
            do
            {
                if (y > ITERATIONLIMIT)
                    break;
                y++;

                // get random word ind.
                wordInd = GetRandomWordIndex(words.Length);
                // check if words have same char.
                sameChar = FindSameCharacter(wordCharacters, words[wordInd]);

                if (sameChar == null)
                {
                    continue;
                }

                // initalize start pos.
                startPos = sameChar.transform.localPosition - dir * FindCharIndex(wordCharacters, words[wordInd]);

                CharacterData[] tempWord = wordCharacters;
                // spawn word.
                wordCharacters = SpawnWord(startPos, dir, words[wordInd], sameChar.inputField?.text);

                if (wordCharacters[0]?.inputField == null)
                {
                    wordCharacters = tempWord;
                    continue;
                }
                chosenWords.Add(wordInd);
                break;
            } while (sameChar == null);
        }
    }

    private int FindCharIndex(CharacterData[] word, string nextWord)
    {
        for (int i = 0; i < word.Length; i++)
        {
            for (int z = 0; z < nextWord.Length; z++)
            {
                string s = nextWord[z].ToString();
                if (word[i]?.inputField != null && word[i].inputField.text == s)
                {
                    return z;
                }
            }
        }

        return -1;
    }

    private int FindCharIndexInWord(string word, string chr)
    {
        if (word == null || chr == null)
            return -1;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == chr[0])
                return i;
        }

        return -1 ;
    }

    private CharacterData FindSameCharacter(CharacterData[] word, string nextWord)
    {
        for (int i = 0; i < word.Length; i++)
        {
            for (int z = 0; z < nextWord.Length; z++)
            {
                string s = nextWord[z].ToString();
                string g = word[i]?.inputField?.text;
                if (g == s)
                {
                    //Debug.Log($"Same characters: {word[i].character.text} and {s}");
                    return word[i];
                }
            }
        }

        return null;
    }

    private CharacterData[] SpawnWord(Vector3 start, Vector3 dir, string word, string intersectedChar)
    {
        int j = FindCharIndexInWord(word, intersectedChar);

        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            Vector3 pos = start + dir * i;
            if (occupiedPositions.Contains(pos) && j != i)
            {
                return new CharacterData[1];
            }
        }

        CharacterData[] letter = new CharacterData[word.Length];

        GameObject wordParent = Instantiate(wordParentPrefab, canvas.transform);
        wordParent.name = word;

        bool isDestroyedIntesection = false;
        for (int f = 0; f < word.Length; f++)
        {
            // miss intersection.
            if (intersectedChar != null && isDestroyedIntesection == false)
            {
                if (intersectedChar[0] == word[f])
                {
                    isDestroyedIntesection = true;
                    continue;
                }
            }                          

            CharacterLogic logic = Instantiate(blockPrefab, wordParent.transform).GetComponent<CharacterLogic>();
            logic.transform.SetLocalPositionAndRotation(start + dir * f, Quaternion.identity);

            occupiedPositions.Add(start + dir * f);

            logic.name = word[f].ToString();
            logic.characterData.charIndex = f;

            letter[f] = logic.characterData;
            letter[f].inputField.text = word[f].ToString();
            letter[f].transform = logic.transform;
        }
        return letter;
    }

    private int GetRandomWordIndex(int wordsLength)
    {
        int randomWord = Random.Range(0, wordsLength);

        while (chosenWords.Contains(randomWord) == true)
        {
            randomWord = Random.Range(0, wordsLength);
        }

        return randomWord;
    }
}