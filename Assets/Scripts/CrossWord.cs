using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class CrossWord : MonoBehaviour
{
    private readonly string _pathToWords = "Assets/Resources/Words.txt";
    private readonly string _pathToExplanation = "Assets/Resources/WordExplanations.txt";

    private readonly HashSet<int> chosenWords = new HashSet<int>();
    private readonly HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private const int ITERATIONLIMIT = 10000;

    private string[] words, descriptions;
    private int[] horizontalWordIndexes, verticalWordIndexes;
    private int _horizontalWordInd, _verticalWordInd;
    private CrosswordData _crosswordData;
    private void Awake()
    {
        _crosswordData = GetComponent<CrosswordData>();

        words = new StreamReader(_pathToWords).ReadToEnd().Split("\r\n");
        descriptions = new StreamReader(_pathToExplanation).ReadToEnd().Split("\r\n");

        for (int i = 0; i < words.Length; i++)
            words[i] = words[i].ToLower();       

        horizontalWordIndexes = new int[_crosswordData.crosswordLength];
        verticalWordIndexes = new int[_crosswordData.crosswordLength];
    }

    public void Generate()
    {
        int wordInd = GetRandomWordIndex(words.Length);
        Destroy();
        GenerateCrossword(wordInd, words, descriptions);
        SetDescription();
    }

    public void Destroy()
    {
        for (int i = 0; i < _crosswordData.canvas.transform.childCount; i++)
        {
            Destroy(_crosswordData.canvas.transform.GetChild(i).gameObject);
        }

        chosenWords.Clear();
        occupiedPositions.Clear();
    }

    private void GenerateCrossword(int wordInd, string[] words, string[] descriptions)
    {
        Vector3 startPos = Vector3.zero;
        Vector3 dir = -Vector3.up * _crosswordData.distanceBetweenBlocks;
        CharacterData sameChar = null;

        CharacterData[] wordCharacters = SpawnWord(startPos, dir, words[wordInd], descriptions[wordInd], null);
        chosenWords.Add(wordInd);
        startPos = Vector3.zero - dir * FindCharIndex(wordCharacters, words[wordInd]);
        for (int j = 0; j < _crosswordData.crosswordLength + _crosswordData.crosswordLength; j++)
        {
            if ((j + 1) % 2 == 0)
                dir = -Vector3.up * _crosswordData.distanceBetweenBlocks;
            else
                dir = Vector3.right * _crosswordData.distanceBetweenBlocks;

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

                if (CheckIntersections(words[wordInd], sameChar.inputField?.text, startPos, dir) == true)
                {
                    continue;
                }
                // spawn word.
                wordCharacters = SpawnWord(startPos, dir, words[wordInd], descriptions[wordInd], sameChar);//sameChar.inputField?.text);

                if (wordCharacters == null)
                {
                    wordCharacters = tempWord;
                    continue;
                }
                chosenWords.Add(wordInd);
                break;
            } while (sameChar == null);

            if ((j + 1) % 2 == 0)
            {
                verticalWordIndexes[_verticalWordInd++] = wordInd;
                _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, verticalWordIndexes.Length - 1);
            }                
            else
            {
                horizontalWordIndexes[_horizontalWordInd++] = wordInd;
                _horizontalWordInd = Mathf.Clamp(_horizontalWordInd, 0, horizontalWordIndexes.Length - 1);
            }
        }
    }

    private CharacterData[] SpawnWord(Vector3 start, Vector3 dir, string word, string wordDescription, CharacterData intersectedChar)
    {
        // spawn parent.
        CharacterLogic charLogic = Instantiate(_crosswordData.wordParentPrefab, _crosswordData.canvas.transform).GetComponent<CharacterLogic>();
        charLogic.wordDescription = wordDescription;
        charLogic.name = word;

        bool isDestroyedIntesection = false;
        CharacterData[] letter = new CharacterData[word.Length];
        for (int f = 0; f < word.Length; f++)
        {
            // miss intersection.
            if (intersectedChar?.desiredChar == word[f] && isDestroyedIntesection == false)
            {
                letter[f] = intersectedChar;
                isDestroyedIntesection = true;
                continue;
            }

            // spawn each char.
            GameObject block = Instantiate(_crosswordData.blockPrefab, charLogic.transform);
            block.transform.SetLocalPositionAndRotation(start + dir * f, Quaternion.identity);
            block.name = word[f].ToString();

            occupiedPositions.Add(start + dir * f);

            letter[f] = new CharacterData(
                block.GetComponentInChildren<TMP_InputField>(),
                _crosswordData.characterColor,
                f,
                word[f],
                block.transform,
                word[f].ToString());

            letter[f].inputField.onValueChanged.AddListener((s) => charLogic.CheckWordCompletion());
        }

        charLogic.wordData.characters = letter;

        return letter;
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

    private bool CheckIntersections(string word, string intersectedChar, Vector3 start, Vector3 dir)
    {
        int j = FindCharIndexInWord(word, intersectedChar);
        Vector3 pos;
        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            pos = start + dir * i;
            if (occupiedPositions.Contains(pos) && j != i)
            {
                return true;
            }
        }

        return false;
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

    private void SetDescription()
    {
        _crosswordData.description.text = "";

        for (int i = 0; i < chosenWords.Count; i++)
        {
            _crosswordData.description.text += $"{i + 1}) {descriptions[ chosenWords.ElementAt(i) ]}\n";
        }
    }
}