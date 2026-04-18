using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallaCablesManager : MonoBehaviour
{
    [Header("Nodos")]
    public List<CableNode> originNodes = new List<CableNode>();
    public List<CableNode> targetNodes = new List<CableNode>();

    [Header("Feedback")]
    public Image feedbackLight;
    public Color neutralColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private CableNode selectedOrigin;
    private Dictionary<int, int> correctConnections = new Dictionary<int, int>();
    private HashSet<int> solvedIds = new HashSet<int>();

    public void SetupLevel(int level)
    {
        StopAllCoroutines();

        selectedOrigin = null;
        correctConnections.Clear();
        solvedIds.Clear();

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;

        int count = 3;
        if (level == 3) count = 3;
        else if (level == 4) count = 5;
        else count = 6;

        for (int i = 0; i < originNodes.Count; i++)
        {
            bool active = i < count;
            originNodes[i].gameObject.SetActive(active);
            targetNodes[i].gameObject.SetActive(active);

            if (active)
            {
                originNodes[i].Init(this, i, true);
                targetNodes[i].Init(this, i, false);
            }
        }

        ShuffleTargets(count);
    }

    void ShuffleTargets(int count)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < count; i++)
            ids.Add(i);

        for (int i = 0; i < ids.Count; i++)
        {
            int randomIndex = Random.Range(0, ids.Count);
            int temp = ids[i];
            ids[i] = ids[randomIndex];
            ids[randomIndex] = temp;
        }

        for (int i = 0; i < count; i++)
        {
            correctConnections[i] = i;
            targetNodes[i].cableId = ids[i];
        }
    }

    public void SelectNode(CableNode node)
    {
        if (node.isOrigin)
        {
            if (solvedIds.Contains(node.cableId)) return;
            selectedOrigin = node;
        }
        else
        {
            if (selectedOrigin == null) return;

            CheckConnection(selectedOrigin, node);
            selectedOrigin = null;
        }
    }

    void CheckConnection(CableNode origin, CableNode target)
    {
        if (origin.cableId == target.cableId)
        {
            solvedIds.Add(origin.cableId);
            StartCoroutine(CorrectFlash());

            if (solvedIds.Count == correctConnections.Count)
            {
                StartCoroutine(CompleteRoutine());
            }
        }
        else
        {
            StartCoroutine(WrongFlash());
        }
    }

    IEnumerator CorrectFlash()
    {
        if (feedbackLight != null)
            feedbackLight.color = correctColor;

        yield return new WaitForSeconds(0.4f);

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;
    }

    IEnumerator WrongFlash()
    {
        if (feedbackLight != null)
            feedbackLight.color = wrongColor;

        yield return new WaitForSeconds(0.7f);

        if (feedbackLight != null)
            feedbackLight.color = neutralColor;
    }

    IEnumerator CompleteRoutine()
    {
        if (feedbackLight != null)
            feedbackLight.color = correctColor;

        yield return new WaitForSeconds(1f);

        PuzzleLevelManager.Instance.CompleteLevel();
    }
}