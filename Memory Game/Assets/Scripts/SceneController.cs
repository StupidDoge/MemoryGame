using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const int GridRows = 2;
    public const int GridCols = 4;
    public const float offsetX = 2f;
    public const float offsetY = 2.5f;
    private int _score = 0;

    private MemoryCard _firstRevealed;
    private MemoryCard _secondRevealed;

    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private TextMesh scoreLabel;

    void Start()
    {
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3 };
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < GridCols; i++)
        {
            for (int j = 0; j < GridRows; j++)
            {
                MemoryCard card;
                if (i == 0 && j == 0)
                    card = originalCard;
                else
                    card = Instantiate(originalCard) as MemoryCard;

                int index = j * GridCols + i;
                int id = numbers[index];
                card.SetCard(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = (offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY - 2, startPos.z);
            }
        }
    }

    private int[] ShuffleArray(int[] numbers) // Алгоритм тасования Кнута/Фишера-Йетса
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int randomPosition = Random.Range(i, newArray.Length);
            int tmp = newArray[i];
            newArray[i] = newArray[randomPosition];
            newArray[randomPosition] = tmp;
        }

        return newArray;
    }

    public bool CanReveal
    {
        get { return _secondRevealed == null; }
    }

    public void CardRevealed(MemoryCard card)
    {
        if (_firstRevealed == null)
            _firstRevealed = card;
        else
        {
            _secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if (_firstRevealed.Id == _secondRevealed.Id)
        {
            _score++;
            scoreLabel.text = "Score: " + _score;
        } 
        else
        {
            yield return new WaitForSeconds(.5f);
            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
        }

        _firstRevealed = null;
        _secondRevealed = null;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
