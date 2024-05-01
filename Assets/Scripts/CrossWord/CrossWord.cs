using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CrosswordData))]
public class CrossWord : MonoBehaviour
{
    public readonly HashSet<CharacterLogic> SpawnedWords = new HashSet<CharacterLogic>();

    public event Action<string> OnGenerationFinish;

    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private CrosswordData _crosswordData;
    private string[] _words, _descriptions;
    private int[] _horizontalWordIndexes, _verticalWordIndexes;
    private int _horizontalWordInd, _verticalWordInd;
    private int _indTest;
    private readonly Vector3 _horizontalDir = Vector3.right, _verticalDir = -Vector3.up;
    
    private const int IterationsLimit = 10;

    private void Start()
    {
        _crosswordData = GetComponent<CrosswordData>();

        _words = _crosswordData.words.Split("\n");
        _descriptions = _crosswordData.descriptions.Split("\n");

        _words = _words.Where(s => s != string.Empty).ToArray();
        _descriptions = _descriptions.Where(s => s != string.Empty).ToArray();
        
        if (_words.Length != _descriptions.Length)
            throw new Exception("Words and description lengths are not the same.");

        for (int i = 0; i < _words.Length; i++)
            _words[i] = _words[i].ToLower();
    }

    public void StartGenerate()
    {
        OnGenerationFinish?.Invoke(string.Empty);
        
        if (_words.Length <= _crosswordData.crosswordLength + _crosswordData.crosswordLength)
        {
            Debug.LogWarning("Error: No words.");
            OnGenerationFinish?.Invoke("Error: No words.");
            return;
        }

        int wordInd = GetRandomWordIndex(_words.Length);
        Destroy();

        _horizontalWordIndexes = new int[_crosswordData.crosswordLength + 1];
        _verticalWordIndexes = new int[_crosswordData.crosswordLength + 1];

        for (int i = 0; i < IterationsLimit; i++)
        {
            bool temp = Generate(wordInd, _words, _descriptions);

            if (temp == false)
            {
                Debug.LogWarning("Error: generation failure. Try again.");
                OnGenerationFinish?.Invoke("Error: generation failure. Try again.");
            }
            
            if (temp == true)
                break;
        }
    }

    private void Destroy()
    {
        for (int i = 0; i < _crosswordData.canvas.transform.childCount; i++)
        {
            Destroy(_crosswordData.canvas.transform.GetChild(i).gameObject);
        }

        _occupiedPositions.Clear();
        SpawnedWords.Clear();
        _horizontalWordInd = _verticalWordInd = 0;
    }

    private bool Generate(int wordInd, IReadOnlyList<string> words, IReadOnlyList<string> descriptions)
    {
        Vector3 spawnStartPos = Vector3.zero;
        Vector3 wordSpawnDirection = _verticalDir * _crosswordData.distanceBetweenBlocks;
        CharacterData sameChar;

        CharacterLogic word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], null,
            wordInd);
        SpawnedWords.Add(word);
        _verticalWordIndexes[_verticalWordInd++] = wordInd;
        _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, _verticalWordIndexes.Length - 1);
        
        for (int j = 0; j < _crosswordData.crosswordLength * 2 - 1; j++)
        {
            wordSpawnDirection = (j + 1) % 2 == 0
                ? _verticalDir * _crosswordData.distanceBetweenBlocks
                : _horizontalDir * _crosswordData.distanceBetweenBlocks;

            WordOrientation currenOrientation =
                (j + 1) % 2 == 0 ? WordOrientation.vertical : WordOrientation.horizontal;

            _indTest = 0;
            do
            {
                wordInd = Mathf.Clamp(_indTest, 0, words.Count - 1);

                if (_indTest >= words.Count - 1)
                {
                    //Debug.LogWarning("Can't find word.");
                    Destroy();
                    return false;
                }

                if (_words[wordInd] == string.Empty)
                {
                    Debug.LogWarning("Empty word.");
                    return false;
                }

                if (SpawnedWords.FirstOrDefault(x => x.WordData.WordIndex == wordInd) != false)
                    continue;

                // find relevant word.
                CharacterData[] sameChars = null;
                for (int i = 0; i < SpawnedWords.Count; i++)
                {
                    if (SpawnedWords.ElementAt(i).WordData.Orientation == currenOrientation)
                        continue;

                    sameChar = TextTools.FindSameCharacter(SpawnedWords.ElementAt(i).WordData.Characters,
                        words[wordInd]);

                    if (sameChar == null)
                        continue;

                    // initialize start pos.
                    spawnStartPos = sameChar.transform.position - wordSpawnDirection *
                        TextTools.FindCharIndexInWord(words[wordInd], sameChar.DesiredChar);
                    // find all intersections.
                    sameChars = FindIntersectionChars(words[wordInd], spawnStartPos, wordSpawnDirection);

                    if (sameChars != null &&
                        sameChars.Count() < words[wordInd].Length - _crosswordData.wordIntersection)
                        break;
                }

                if (sameChars == null || sameChars.Count() >= words[wordInd].Length - _crosswordData.wordIntersection)
                {
                    continue;
                }

                // spawn word.
                word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], sameChars,
                    wordInd);

                SpawnedWords.Add(word);

                if ((j + 1) % 2 == 0)
                {
                    _verticalWordIndexes[_verticalWordInd++] = wordInd;
                    _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, _verticalWordIndexes.Length - 1);
                }
                else
                {
                    _horizontalWordIndexes[_horizontalWordInd++] = wordInd;
                    _horizontalWordInd = Mathf.Clamp(_horizontalWordInd, 0, _horizontalWordIndexes.Length - 1);
                }
                break;
            } while (_indTest++ <= words.Count - 1);
        }

        return true;
    }

    private CharacterLogic SpawnWord(Vector3 start, Vector3 dir, string word, string wordDescription,
        CharacterData[] intersectedChar, int wordIndex)
    {
        // spawn parent.
        CharacterLogic charLogic = Instantiate(_crosswordData.wordParentPrefab, _crosswordData.canvas.transform)
            .GetComponent<CharacterLogic>();

        charLogic.name = word;

        _occupiedPositions.Add(start - dir);
        _occupiedPositions.Add(start + dir * word.Length);

        Vector3 currentPos = Vector3.zero;
        CharacterData[] letter = new CharacterData[word.Length];
        for (int f = 0; f < word.Length; f++)
        {
            currentPos = start + dir * f;

            // miss intersection.
            if (intersectedChar != null)
            {
                var ch = intersectedChar.FirstOrDefault(x => x.transform.position == currentPos);
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

            CharacterData data = new CharacterData();

            _occupiedPositions.Add(currentPos);

            data.CurrentChar = block.GetComponentInChildren<TextMeshProUGUI>();
            data.CharIndex = f;
            data.DesiredChar = word[f];
            data.MeshRenderer = block.GetComponentInChildren<MeshRenderer>();
            data.gameObject = block;
            data.transform = block.transform;

            data.CurrentChar.text = ""; //data.DesiredChar.ToString();//"";

            letter[f] = data;
        }

        charLogic.WordData.Characters = letter;

        charLogic.WordData.Word = word;
        charLogic.WordData.WordDescription = wordDescription;

        if (dir == _verticalDir)
        {
            charLogic.WordData.Characters[0].gameObject.GetComponentInChildren<Text>().text = $"{_verticalWordInd + 1}";
            charLogic.WordData.Orientation = WordOrientation.vertical;
        }
        else
        {
            charLogic.WordData.Characters[0].gameObject.GetComponentInChildren<Text>().text =
                $"{_horizontalWordInd + 1}";
            charLogic.WordData.Orientation = WordOrientation.horizontal;
        }

        charLogic.WordData.WordIndex = wordIndex;
        charLogic.Initialize(_horizontalDir, _verticalDir, dir);

        SpawnedWords.Add(charLogic);

        return charLogic;
    }

    private CharacterData[] FindIntersectionChars(string word, Vector3 start, Vector3 dir)
    {
        Vector3 pos;

        Vector3 perpendicular = Vector3.Dot(dir, _verticalDir) == 0 ? _verticalDir : _horizontalDir;

        // if word close to another.
        Vector3 tempStart = start;
        for (int i = 0; i < word.Length; i++)
        {
            if (_occupiedPositions.Contains(tempStart + perpendicular) ||
                _occupiedPositions.Contains(tempStart - perpendicular))
            {
                return null;
            }

            tempStart += dir * i;
        }

        if (_occupiedPositions.Contains(start - dir) == true || _occupiedPositions.Contains(start + dir * word.Length))
            return null;

        List<CharacterData> sameChars = new List<CharacterData>();

        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            pos = start + dir * i;

            if (!_occupiedPositions.Contains(pos))
                continue;

            // find word.
            CharacterLogic z = SpawnedWords.FirstOrDefault(x =>
                x.WordData.Characters.FirstOrDefault(y => y.transform.position == pos) != null);

            if (z == false)
            {
                sameChars = null;
                break;
            }

            // check if they have same char.
            CharacterData posChar = z.WordData.Characters.FirstOrDefault(x =>
                x.DesiredChar.ToString() == word[i].ToString() && x.transform.position == pos);

            if (posChar != null)
            {
                sameChars.Add(posChar);
                continue;
            }

            sameChars = null;
            break;
        }

        return sameChars?.ToArray();
    }

    private int GetRandomWordIndex(int wordsLength)
    {
        int randomWord = Random.Range(0, wordsLength);

        while (SpawnedWords.FirstOrDefault(x => x.WordData.WordIndex == randomWord) == true)
        {
            randomWord = Random.Range(0, wordsLength);
        }

        return randomWord;
    }
}