using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossWord : MonoBehaviour
{
    private readonly HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private readonly HashSet<CharacterLogic> spawnedWords = new HashSet<CharacterLogic>();

    private string[] words, descriptions;
    private int[] horizontalWordIndexes, verticalWordIndexes;
    private int _horizontalWordInd, _verticalWordInd;
    private CrosswordData _crosswordData;
    private int _indTest = 0;

    private Vector3 _horizontalDir = Vector3.right, _verticalDir = -Vector3.up;
    private void Awake()
    {
        _crosswordData = GetComponent<CrosswordData>();

        words = _crosswordData.words.text.Split("\r\n");
        descriptions = _crosswordData.descriptions.text.Split("\r\n");

        for (int i = 0; i < words.Length; i++)
            words[i] = words[i].ToLower();       
    }

    public void Generate()
    {
        int wordInd = GetRandomWordIndex(words.Length);
        Destroy();

        horizontalWordIndexes = new int[_crosswordData.crosswordLength + 1];
        verticalWordIndexes = new int[_crosswordData.crosswordLength + 1];

        GenerateCrossword(wordInd, words, descriptions);
        SetDescription();
    }

    private void Destroy()
    {
        for (int i = 0; i < _crosswordData.canvas.transform.childCount; i++)
        {
            Destroy(_crosswordData.canvas.transform.GetChild(i).gameObject);
        }

        occupiedPositions.Clear();
        spawnedWords.Clear();
        _horizontalWordInd = _verticalWordInd = 0;
    }

    private void GenerateCrossword(int wordInd, string[] words, string[] descriptions)
    {
        Vector3 spawnStartPos = Vector3.zero;
        Vector3 wordSpawnDirection = _verticalDir * _crosswordData.distanceBetweenBlocks;
        CharacterData sameChar = null;

        CharacterLogic word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], null, wordInd);
        spawnedWords.Add(word);
        verticalWordIndexes[_verticalWordInd++] = wordInd;
        _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, verticalWordIndexes.Length - 1);

        for (int j = 0; j < _crosswordData.crosswordLength * 2 + 2; j++)
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

                if (spawnedWords.Where( x => x.wordData.wordIndex == wordInd).FirstOrDefault() != null)
                    continue;

                
                // check if spawned words and current word have same char.
                for (int i = 0; i < spawnedWords.Count; i++)
                {
                    if (spawnedWords.ElementAt(i).wordData.orientation == currenOrientation)
                        continue;

                    sameChar = FindSameCharacter(spawnedWords.ElementAt(i).wordData.characters, words[wordInd]);

                    if (sameChar != null)
                        break;
                }

                if (sameChar == null)
                {
                    continue;
                }
                

                // initalize start pos.
                spawnStartPos = sameChar.transform.localPosition - wordSpawnDirection * FindCharIndexInWord(words[wordInd], sameChar.desiredChar);

                
                // check if word has other intersections.
                int index = FindCharIndexInWord(words[wordInd], sameChar.desiredChar);

                
                CharacterData[] sameChars = FindIntersectionChars(words[wordInd], spawnStartPos, wordSpawnDirection);

                if (sameChars == null || sameChars.Count() >= words[wordInd].Length - _crosswordData.wordIntersection)
                {
                    continue;
                }

                CharacterData[] d = new CharacterData[sameChars.Count() + 1];
                for (int i = 0; i < d.Length - 1; i++)
                {
                    d[i] = sameChars[i];
                }

                d[d.Length - 1] = sameChar;
                


                // spawn word.
                word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], d, wordInd);

                spawnedWords.Add(word);

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

            occupiedPositions.Add(currentPos);

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
        spawnedWords.Add(charLogic);

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
        if (occupiedPositions.Contains(start - dir) == true || occupiedPositions.Contains(start + dir * word.Length))
            return null;

        Vector3 pos, perpendicular;

        perpendicular = Vector3.Dot(dir, _verticalDir) == 0 ? _verticalDir : _horizontalDir;

        List<CharacterData> sameChars = new List<CharacterData>();

        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            pos = start + dir * i;
            
            if (occupiedPositions.Contains(pos))
            {
                // find word.
                CharacterLogic z = spawnedWords.Where(x => x.wordData.characters.Where(x => x.transform.position == pos).FirstOrDefault() != null).FirstOrDefault();

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

        while (spawnedWords.Where(x => x.wordData.wordIndex == randomWord).FirstOrDefault() != null)
        {
            randomWord = Random.Range(0, wordsLength);
        }

        return randomWord;
    }

    private void SetDescription()
    {
        _crosswordData.description.text = "Horizontal:\n";

        for (int i = 0; i < _horizontalWordInd; i++)
        {
            _crosswordData.description.text += $"{i + 1}) {descriptions[ horizontalWordIndexes[i] ]}\n";
        }

        _crosswordData.description.text += "\nVertical\n";

        for (int i = 0; i < _verticalWordInd; i++)
        {
            _crosswordData.description.text += $"{i + 1}) {descriptions[ verticalWordIndexes[i] ]}\n";
        }
    }
}
//private readonly string _pathToWords = "Resources/Words.txt";
//private readonly string _pathToExplanation = "Resources/WordExplanations.txt";

/*
 private bool HasIntersections(string word, int intersectedCharIndexInWord, Vector3 start, Vector3 dir)
 {
     Vector3 pos, scalar;

     scalar = Vector3.Dot(dir, _verticalDir) == 0 ? _horizontalDir : _verticalDir;

     //if intersection begin and end.        
     //if (occupiedPositions.Contains(start - dir) == true || occupiedPositions.Contains(start + dir * word.Length))
         //return true;

     // check if intersections.
     for (int i = 0; i < word.Length; i++)
     {
         pos = start + dir * i;

         // miss same char
         if (intersectedCharIndexInWord == i)
             continue;

         // if word close to another or intersect it, return true. 
         if (
             occupiedPositions.Contains(pos) //||
             // if intersection around.
             //occupiedPositions.Contains(pos + scalar) ||
             //occupiedPositions.Contains(pos - scalar)

             )

         {
             Vector3 p = occupiedPositions.Where(x => x == pos).FirstOrDefault();
             CharacterLogic z = spawnedWords.Where(x => x.wordData.characters.Where(x => x.transform.position == p).FirstOrDefault() != null ).FirstOrDefault();
             if (z.wordData.characters.Where(x => x.desiredChar.ToString() == word[i].ToString()).FirstOrDefault() != null )
             {
                 continue;
             }

             return true;
         }
     }

     return false;
 }
 */

// miss same char
/*
if (intersectedCharIndexInWord == i)
    continue
*/

/*
if (occupiedPositions.Contains(pos + perpendicular) ||
    occupiedPositions.Contains(pos - perpendicular))
{
    sameChars = null;
    break;
} 
*/

/*
if (occupiedPositions.Contains(pos))
{
    sameChars = null;
    break;
}
*/