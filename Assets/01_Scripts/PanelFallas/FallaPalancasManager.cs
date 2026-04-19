using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallaPalancasManager : MonoBehaviour
{
    [Header("Palancas")]
    public List<PuzzleLever> levers = new List<PuzzleLever>();

    [Header("Feedback")]
    public Image feedbackLight;
    public Color neutralColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private List<int> correctOrder = new List<int>();
    private List<int> playerOrder = new List<int>();
    private bool canInteract = true;

    public void SetupLevel(int level)
    {
        StopAllCoroutines();

        correctOrder.Clear();
        playerOrder.Clear();
        canInteract = true;

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;

        int count = 3;
        if (level == 5) count = 3;
        else if (level == 6) count = 5;
        else count = 6;

        for (int i = 0; i < levers.Count; i++)
        {
            bool active = i < count;
            levers[i].gameObject.SetActive(active);

            if (active)
                levers[i].Init(this, i);
        }

        GenerateOrder(count);
    }

    void GenerateOrder(int count)
    {
        correctOrder.Clear();

        List<int> pool = new List<int>();
        for (int i = 0; i < count; i++)
            pool.Add(i);

        while (pool.Count > 0)
        {
            int index = Random.Range(0, pool.Count);
            correctOrder.Add(pool[index]);
            pool.RemoveAt(index);
        }

        Debug.Log("Orden correcto: " + string.Join(",", correctOrder));
    }

    public void PlayerPulledLever(int leverId)
    {
        if (!canInteract) return;

        playerOrder.Add(leverId);

        int currentIndex = playerOrder.Count - 1;

        if (playerOrder[currentIndex] != correctOrder[currentIndex])
        {
            StartCoroutine(WrongRoutine());
            return;
        }

        if (playerOrder.Count == correctOrder.Count)
        {
            StartCoroutine(CorrectRoutine());
        }
    }

    IEnumerator CorrectRoutine()
    {
        canInteract = false;

        if (feedbackLight != null)
            feedbackLight.color = correctColor;

        yield return new WaitForSeconds(1f);

        PuzzleLevelManager.Instance.CompleteLevel();
    }

    IEnumerator WrongRoutine()
    {
        canInteract = false;

        if (feedbackLight != null)
            feedbackLight.color = wrongColor;

        yield return new WaitForSeconds(1f);

        playerOrder.Clear();

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;

        canInteract = true;
    }
}