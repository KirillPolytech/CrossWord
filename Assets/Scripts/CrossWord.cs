using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossWord : MonoBehaviour
{
    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private readonly HashSet<CharacterLogic> _spawnedWords = new HashSet<CharacterLogic>();

    private CrosswordData _crosswordData;
    private CrosswordUI _crosswordUI;

    private string[] _words, _descriptions;
    private int[] horizontalWordIndexes, verticalWordIndexes;
    private int _horizontalWordInd, _verticalWordInd;
    private int _indTest = 0;

    private readonly Vector3 _horizontalDir = Vector3.right, _verticalDir = -Vector3.up;
    private void Awake()
    {
        _crosswordUI = FindAnyObjectByType<CrosswordUI>();
        _crosswordData = GetComponent<CrosswordData>();

        _words = _crosswordData.words.text.Split("\r\n");
        _descriptions = _crosswordData.descriptions.text.Split("\r\n");

        for (int i = 0; i < _words.Length; i++)
            _words[i] = _words[i].ToLower();
    }

    public void Generate()
    {
        int wordInd = GetRandomWordIndex(_words.Length);
        Destroy();

        horizontalWordIndexes = new int[_crosswordData.crosswordLength + 1];
        verticalWordIndexes = new int[_crosswordData.crosswordLength + 1];

        GenerateCrossword(wordInd, _words, _descriptions);
        _crosswordUI.SetDescription(_spawnedWords);
    }

    private void Destroy()
    {
        for (int i = 0; i < _crosswordData.canvas.transform.childCount; i++)
        {
            Destroy(_crosswordData.canvas.transform.GetChild(i).gameObject);
        }

        _occupiedPositions.Clear();
        _spawnedWords.Clear();
        _horizontalWordInd = _verticalWordInd = 0;
    }

    private void GenerateCrossword(int wordInd, string[] words, string[] descriptions)
    {
        Vector3 spawnStartPos = Vector3.zero;
        Vector3 wordSpawnDirection = _verticalDir * _crosswordData.distanceBetweenBlocks;
        CharacterData sameChar = null;

        CharacterLogic word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], null, wordInd);
        _spawnedWords.Add(word);
        verticalWordIndexes[_verticalWordInd++] = wordInd;
        _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, verticalWordIndexes.Length - 1);

        for (int j = 0; j < _crosswordData.crosswordLength * 2 - 1; j++)
        {
            wordSpawnDirection = (j + 1) % 2 == 0 ?
                _verticalDir * _crosswordData.distanceBetweenBlocks : 
                _horizontalDir * _crosswordData.distanceBetweenBlocks;

            WordOrientation currenOrientation = (j + 1) % 2 == 0 ? WordOrientation.vertical : WordOrientation.horizontal;

            _indTest = 0;
            do
            {
                wordInd = Mathf.Clamp(_indTest, 0, words.Length - 1);

                if (_indTest >= words.Length - 1)
                {
                    Debug.LogWarning("cant find word");
                    break;
                }

                if (_spawnedWords.Where( x => x.wordData.wordIndex == wordInd).FirstOrDefault() != null)
                    continue;

                // find revelant word.
                CharacterData[] sameChars = null;
                for (int i = 0; i < _spawnedWords.Count; i++)
                {
                    if (_spawnedWords.ElementAt(i).wordData.orientation == currenOrientation)
                        continue;

                    sameChar = FindSameCharacter(_spawnedWords.ElementAt(i).wordData.characters, words[wordInd]);

                    if (sameChar != null)
                    {
                        // initalize start pos.
                        spawnStartPos = sameChar.transform.localPosition - wordSpawnDirection * FindCharIndexInWord(words[wordInd], sameChar.desiredChar);
                        // find all intersections.
                        sameChars = FindIntersectionChars(words[wordInd], spawnStartPos, wordSpawnDirection);

                        if (sameChars != null && sameChars.Count() < words[wordInd].Length - _crosswordData.wordIntersection)
                            break;
                    }
                }

                if (sameChars == null || sameChars.Count() >= words[wordInd].Length - _crosswordData.wordIntersection)
                {
                    continue;
                }

                // spawn word.
                word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], sameChars, wordInd);

                _spawnedWords.Add(word);

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
                break;
            } while (_indTest++ <= words.Length - 1);            
        }
    }

    private CharacterLogic SpawnWord(Vector3 start, Vector3 dir, string word, string wordDescription, CharacterData[] intersectedChar, int wordIndex)
    {
        // spawn parent.
        CharacterLogic charLogic = Instantiate(_crosswordData.wordParentPrefab, _crosswordData.canvas.transform).GetComponent<CharacterLogic>();
        charLogic.wordDescription = wordDescription;
        charLogic.name = word;

        Vector3 currentPos;
        CharacterData[] letter = new CharacterData[word.Length];
        for (int f = 0; f < word.Length; f++)
        {
            currentPos = start + dir * f;

            // miss intersection.
            if (intersectedChar != null)
            {
                var ch = intersectedChar.Where(x => x.transform.position == currentPos).FirstOrDefault();
                if (ch != null)
                {
                    letter[f] = ch;
                    continue;
                }
            }

            // spawn each char.
            GameObject block = Instantiate(_crosswordData.blockPrefab, charLogic.transform);
            block.transform.SetLocalPositionAndRotation(currentPos, Quaternion.identity);
            block.name = word[f].ToString();

            _occupiedPositions.Add(currentPos);

            letter[f] = new CharacterData(
                block.GetComponentInChildren<TMP_InputField>(),
                _crosswordData.characterColor,
                f,
                word[f],
                block.transform
                );

            letter[f].inputField.onValueChanged.AddListener((s) => charLogic.CheckWordCompletion());
        }

        charLogic.wordData.characters = letter;

        if (dir == _verticalDir)
        {
            charLogic.wordData.characters[0].transform.gameObject.GetComponentInChildren<Text>().text = $"{_verticalWordInd + 1}";
            charLogic.wordData.orientation = WordOrientation.vertical;
        }
        else
        {
            charLogic.wordData.characters[0].transform.gameObject.GetComponentInChildren<Text>().text = $"{_horizontalWordInd + 1}";
            charLogic.wordData.orientation = WordOrientation.horizontal;
        }

        charLogic.wordData.wordIndex = wordIndex;
        _spawnedWords.Add(charLogic);

        return charLogic;
    }

    private int FindCharIndexInWord(string word, char chr)
    {
        if (word == null || chr == 0)
            return -1;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == chr )
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
                if (word[i].desiredChar.ToString() == nextWord[z].ToString())
                {
                    //Debug.Log($"Same characters: {word[i].character.text} and {s}");

                    return word[i];
                }
            }
        }

        return null;
    }

    private CharacterData[] FindIntersectionChars(string word, Vector3 start, Vector3 dir)
    {
        if (_occupiedPositions.Contains(start - dir) == true || _occupiedPositions.Contains(start + dir * word.Length))
            return null;

        Vector3 pos, perpendicular;

        perpendicular = Vector3.Dot(dir, _verticalDir) == 0 ? _verticalDir : _horizontalDir;

        List<CharacterData> sameChars = new List<CharacterData>();

        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            pos = start + dir * i;
            
            if (_occupiedPositions.Contains(pos))
            {
                // find word.
                CharacterLogic z = _spawnedWords.Where(x => x.wordData.characters.Where(x => x.transform.position == pos).FirstOrDefault() != null).FirstOrDefault();

                // check if they have same char.
                CharacterData posChar = z.wordData.characters.Where(x => 
                    x.desiredChar.ToString() == word[i].ToString() && x.transform.position == pos).FirstOrDefault();

                if (posChar != null)
                {
                    sameChars.Add(posChar);
                    continue;
                }
                else
                {
                    sameChars = null;
                    break;
                }
            }
            
        }

        return sameChars?.ToArray();
    }

    private int GetRandomWordIndex(int wordsLength)
    {
        int randomWord = Random.Range(0, wordsLength);

        while (_spawnedWords.Where(x => x.wordData.wordIndex == randomWord).FirstOrDefault() != null)
        {
            randomWord = Random.Range(0, wordsLength);
        }

        return randomWord;
    }
}

/*
CharacterData[] intersectedChars = new CharacterData[sameChars.Count() + 1];                
for (int i = 0; i < intersectedChars.Length - 1; i++)
{
    intersectedChars[i] = sameChars[i];
}

intersectedChars[intersectedChars.Length - 1] = sameChar;
*/

/*
if (sameChars == null || sameChars.Count() >= words[wordInd].Length - _crosswordData.wordIntersection)
{
    continue;
}
else
{
    break;
}
*/