using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallaBotonesManager : MonoBehaviour
{
    [Header("Botones")]
    public List<PuzzleButton> buttons = new List<PuzzleButton>();

    [Header("Feedback")]
    public Image feedbackLight;
    public Color neutralColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Configuración")]
    public float showDelay = 0.6f;

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool isShowingSequence = false;
    private bool canPlay = false;

    public void SetupLevel(int level)
    {
        StopAllCoroutines();

        sequence.Clear();
        playerInput.Clear();
        isShowingSequence = false;
        canPlay = false;

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Init(this, i);
            buttons[i].SetNormal();
        }

        int sequenceLength = 3;

        if (level == 1) sequenceLength = 3;
        else if (level == 2) sequenceLength = 5;
        else sequenceLength = 6;

        GenerateSequence(sequenceLength);
        StartCoroutine(ShowSequence());
    }

    void GenerateSequence(int length)
    {
        sequence.Clear();
        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, buttons.Count));
        }
    }

    IEnumerator ShowSequence()
    {
        isShowingSequence = true;
        canPlay = false;

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < sequence.Count; i++)
        {
            int index = sequence[i];
            buttons[index].SetActive();
            yield return new WaitForSeconds(showDelay);
            buttons[index].SetNormal();
            yield return new WaitForSeconds(0.2f);
        }

        isShowingSequence = false;
        canPlay = true;
    }

    public void PlayerPressed(int buttonId)
    {
        if (!canPlay || isShowingSequence) return;

        playerInput.Add(buttonId);

        int currentIndex = playerInput.Count - 1;

        if (playerInput[currentIndex] != sequence[currentIndex])
        {
            StartCoroutine(WrongRoutine());
            return;
        }

        if (playerInput.Count == sequence.Count)
        {
            StartCoroutine(CorrectRoutine());
        }
    }

    IEnumerator CorrectRoutine()
    {
        canPlay = false;

        if (feedbackLight != null)
            feedbackLight.color = correctColor;

        yield return new WaitForSeconds(1f);

        PuzzleLevelManager.Instance.CompleteLevel();
    }

    IEnumerator WrongRoutine()
    {
        canPlay = false;

        if (feedbackLight != null)
            feedbackLight.color = wrongColor;

        yield return new WaitForSeconds(1f);

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;

        playerInput.Clear();
        StartCoroutine(ShowSequence());
    }
}