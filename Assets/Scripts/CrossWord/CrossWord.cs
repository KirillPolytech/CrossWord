using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable All

public class CrossWord : MonoBehaviour
{
    private readonly HashSet<Vector3> _occupiedPositions = new HashSet<Vector3>();
    private readonly HashSet<CharacterLogic> _spawnedWords = new HashSet<CharacterLogic>();

    private CrosswordData _crosswordData;
    private CrosswordUI _crosswordUI;
    
    private string[] _words, _descriptions;
    private int[] _horizontalWordIndexes, _verticalWordIndexes;
    private int _horizontalWordInd, _verticalWordInd;
    private int _indTest = 0;

    private readonly Vector3 _horizontalDir = Vector3.right, _verticalDir = -Vector3.up;
    private void Awake()
    {
        _crosswordUI = FindFirstObjectByType<CrosswordUI>();
        _crosswordData = GetComponent<CrosswordData>();
    }

    private void Start()
    {
        _words = _crosswordData.words.text.Split("\r\n");
        _descriptions = _crosswordData.descriptions.text.Split("\r\n");
        
        for (int i = 0; i < _words.Length; i++)
            _words[i] = _words[i].ToLower();
    }

    public void Generate()
    {
        int wordInd = GetRandomWordIndex(_words.Length);
        Destroy();

        _horizontalWordIndexes = new int[_crosswordData.crosswordLength + 1];
        _verticalWordIndexes = new int[_crosswordData.crosswordLength + 1];

        StartGeneration(wordInd, _words, _descriptions);

        if (_crosswordUI == null)
        {
            Debug.LogWarning("CrosswordUI null");
            
        }
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

    private void StartGeneration(int wordInd, string[] words, string[] descriptions)
    {
        Vector3 spawnStartPos = Vector3.zero;
        Vector3 wordSpawnDirection = _verticalDir * _crosswordData.distanceBetweenBlocks;
        CharacterData sameChar = null;

        CharacterLogic word = SpawnWord(spawnStartPos, wordSpawnDirection, words[wordInd], descriptions[wordInd], null, wordInd);
        _spawnedWords.Add(word);
        _verticalWordIndexes[_verticalWordInd++] = wordInd;
        _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, _verticalWordIndexes.Length - 1);

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

                if (_spawnedWords.FirstOrDefault(x => x.wordData.wordIndex == wordInd) != null)
                    continue;

                // find relevant word.
                CharacterData[] sameChars = null;
                for (int i = 0; i < _spawnedWords.Count; i++)
                {
                    if (_spawnedWords.ElementAt(i).wordData.orientation == currenOrientation)
                        continue;

                    sameChar = WordManipulator.FindSameCharacter(_spawnedWords.ElementAt(i).wordData.characters, words[wordInd]);

                    if (sameChar == null) 
                        continue;
                    
                    // initialize start pos.
                    //spawnStartPos = sameChar.transform.localPosition - wordSpawnDirection * WordManipulator.FindCharIndexInWord(words[wordInd], sameChar.desiredChar);
                    spawnStartPos = sameChar.transform.position - wordSpawnDirection * WordManipulator.FindCharIndexInWord(words[wordInd], sameChar.DesiredChar);
                    // find all intersections.
                    sameChars = FindIntersectionChars(words[wordInd], spawnStartPos, wordSpawnDirection);

                    if (sameChars != null && sameChars.Count() < words[wordInd].Length - _crosswordData.wordIntersection)
                        break;
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
                    _verticalWordIndexes[_verticalWordInd++] = wordInd;
                    _verticalWordInd = Mathf.Clamp(_verticalWordInd, 0, _verticalWordIndexes.Length - 1);
                }
                else
                {
                    _horizontalWordIndexes[_horizontalWordInd++] = wordInd;
                    _horizontalWordInd = Mathf.Clamp(_horizontalWordInd, 0, _horizontalWordIndexes.Length - 1);
                }
                break;
            } while (_indTest++ <= words.Length - 1);            
        }
    }

    private CharacterLogic SpawnWord(Vector3 start, Vector3 dir, string word, string wordDescription, CharacterData[] intersectedChar, int wordIndex)
    {
        // spawn parent.
        CharacterLogic charLogic = Instantiate(_crosswordData.wordParentPrefab, _crosswordData.canvas.transform).GetComponent<CharacterLogic>();
        //charLogic.SetPosition(start + dir * (word.Length - 1) / 2);
        
        charLogic.wordDescription = wordDescription;
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
            data.CharacterLogic = charLogic;
            data.MeshRenderer = block.GetComponentInChildren<MeshRenderer>();
            data.gameObject = block;
            data.transform = block.transform;

            data.CurrentChar.text = data.DesiredChar.ToString();//"";

            letter[f] = data;
        }

        charLogic.wordData.characters = letter;
        
        if (dir == _verticalDir)
        {
            charLogic.wordData.characters[0].gameObject.GetComponentInChildren<Text>().text = $"{_verticalWordInd + 1}";
            charLogic.wordData.orientation = WordOrientation.vertical;
        }
        else
        {
            charLogic.wordData.characters[0].gameObject.GetComponentInChildren<Text>().text = $"{_horizontalWordInd + 1}";
            charLogic.wordData.orientation = WordOrientation.horizontal;
        }

        charLogic.wordData.wordIndex = wordIndex;
        charLogic.Initialize(_horizontalDir, _verticalDir, dir);

        _spawnedWords.Add(charLogic);

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
            if (_occupiedPositions.Contains(tempStart + perpendicular) || _occupiedPositions.Contains(tempStart - perpendicular))
            {
                return null;
            }

            tempStart += dir * i;
        }      

        if (_occupiedPositions.Contains(start - dir) == true || _occupiedPositions.Contains(start + dir * word.Length ))
            return null;

        List<CharacterData> sameChars = new List<CharacterData>();

        // check if intersections.
        for (int i = 0; i < word.Length; i++)
        {
            pos = start + dir * i;

            if (!_occupiedPositions.Contains(pos)) 
                continue;
            
            // find word.
            CharacterLogic z = _spawnedWords.FirstOrDefault(x => x.wordData.characters.FirstOrDefault(y => y.transform.position == pos) != null);

            if(z == null)
            {
                sameChars = null;
                break;
            }

            // check if they have same char.
            CharacterData posChar = z.wordData.characters.FirstOrDefault(x => x.DesiredChar.ToString() == word[i].ToString() && x.transform.position == pos);

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

        while (_spawnedWords.FirstOrDefault(x => x.wordData.wordIndex == randomWord) != null)
        {
            randomWord = Random.Range(0, wordsLength);
        }

        return randomWord;
    }
}