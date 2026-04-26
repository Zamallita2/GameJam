using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Game : MonoBehaviour
{
    [Header("Centro del puzzle")]
    public Transform puzzleCenter;

    [Header("Máquina dueña")]
    public MachineInteraction machineOwner;

    [Header("Piezas en orden")]
    public GameObject[] go;

    [Header("Escala de piezas")]
    public float pieceScale = 0.45f;

    [Header("Separación entre piezas")]
    public float pieceSpacing = 0.20f;

    [Header("Velocidad animación")]
    public float AnimSpeed = 10f;

    [Header("Mezcla")]
    public int shuffleMoves = 20;

    [Header("Click")]
    public Camera puzzleCamera;
    public float maxScreenClickDistance = 80f;

    [Header("Final")]
    public float closePanelDelay = 1f;

    private Vector3 screenPositionToAnimate;
    private Piece pieceToAnimate;
    private int toAnimateI;
    private int toAnimateJ;

    public Piece[,] Matrix = new Piece[Constants.MaxColumns, Constants.MaxRows];
    private GameState gameState;

    private bool completed = false;
    private bool initialized = false;
    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip slideSound;
    public AudioClip completeSound;

    private void Awake() {
        if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    void OnEnable()
    {
        InitPuzzle();
    }

    public void SetMachineOwner(MachineInteraction owner)
    {
        machineOwner = owner;
    }

    void InitPuzzle()
    {
        completed = false;
        initialized = false;

        if (puzzleCenter == null)
        {
            Debug.LogError("[Game] Falta asignar puzzleCenter.");
            enabled = false;
            return;
        }

        if (go == null || go.Length != Constants.MaxSize)
        {
            Debug.LogError("[Game] Debes asignar exactamente 9 piezas.");
            enabled = false;
            return;
        }

        if (puzzleCamera == null)
            puzzleCamera = Camera.main;

        PreparePieces();
        BuildMatrix();
        RefreshAllPiecePositions();

        gameState = GameState.Start;
        initialized = true;
    }

    void Update()
    {
        if (!initialized || completed) return;

        switch (gameState)
        {
            case GameState.Start:
                Shuffle();
                gameState = GameState.Playing;
                break;

            case GameState.Playing:
                CheckPieceInput();
                break;

            case GameState.Animating:
                AnimateMovement(pieceToAnimate, Time.deltaTime);
                CheckIfAnimationEnded();
                break;

            case GameState.End:
                break;
        }
    }

    private void PreparePieces()
    {
        SetPiecesScale(pieceScale);

        for (int i = 0; i < go.Length; i++)
        {
            if (go[i] != null)
                go[i].SetActive(true);
        }

        int emptyIndex = Constants.MaxSize - 1;

        if (go[emptyIndex] != null)
            go[emptyIndex].SetActive(false);
    }

    private void BuildMatrix()
    {
        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                int index = i * Constants.MaxColumns + j;

                if (go[index] != null && go[index].activeInHierarchy)
                {
                    Matrix[i, j] = new Piece
                    {
                        GameObject = go[index],
                        OriginalI = i,
                        OriginalJ = j,
                        CurrentI = i,
                        CurrentJ = j
                    };
                }
                else
                {
                    Matrix[i, j] = null;
                }
            }
        }
    }

    private void SetPiecesScale(float scaleValue)
    {
        for (int c = 0; c < go.Length; c++)
        {
            if (go[c] != null)
                go[c].transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
        }
    }

    private void RefreshAllPiecePositions()
    {
        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                if (Matrix[i, j] != null)
                    Matrix[i, j].GameObject.transform.position = GetBoardPosition(i, j);
            }
        }
    }

    private Vector3 GetBoardPosition(int i, int j)
    {
        float totalWidth = (Constants.MaxRows - 1) * pieceSpacing;
        float totalHeight = (Constants.MaxColumns - 1) * pieceSpacing;

        float startX = -totalWidth * 0.5f;
        float startY = totalHeight * 0.5f;

        Vector3 localOffset = new Vector3(
            startX + (j * pieceSpacing),
            startY - (i * pieceSpacing),
            0f
        );

        return puzzleCenter.position
             + puzzleCenter.right * localOffset.x
             + puzzleCenter.up * localOffset.y;
    }

    private void Shuffle()
    {
        Vector2Int lastMove = Vector2Int.zero;

        for (int move = 0; move < shuffleMoves; move++)
        {
            int emptyI;
            int emptyJ;

            FindEmptyCell(out emptyI, out emptyJ);

            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            if (emptyI > 0 && lastMove != Vector2Int.down)
                possibleMoves.Add(Vector2Int.up);

            if (emptyI < Constants.MaxColumns - 1 && lastMove != Vector2Int.up)
                possibleMoves.Add(Vector2Int.down);

            if (emptyJ > 0 && lastMove != Vector2Int.right)
                possibleMoves.Add(Vector2Int.left);

            if (emptyJ < Constants.MaxRows - 1 && lastMove != Vector2Int.left)
                possibleMoves.Add(Vector2Int.right);

            if (possibleMoves.Count == 0) return;

            Vector2Int dir = possibleMoves[Random.Range(0, possibleMoves.Count)];

            int targetI = emptyI;
            int targetJ = emptyJ;

            if (dir == Vector2Int.up) targetI--;
            else if (dir == Vector2Int.down) targetI++;
            else if (dir == Vector2Int.left) targetJ--;
            else if (dir == Vector2Int.right) targetJ++;

            Swap(targetI, targetJ, emptyI, emptyJ);
            lastMove = dir;
        }

        RefreshAllPiecePositions();
    }

    private void FindEmptyCell(out int emptyI, out int emptyJ)
    {
        emptyI = -1;
        emptyJ = -1;

        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                if (Matrix[i, j] == null)
                {
                    emptyI = i;
                    emptyJ = j;
                    return;
                }
            }
        }
    }

    private void Swap(int i, int j, int targetI, int targetJ)
    {
        Piece temp = Matrix[i, j];
        Matrix[i, j] = Matrix[targetI, targetJ];
        Matrix[targetI, targetJ] = temp;

        if (Matrix[i, j] != null)
        {
            Matrix[i, j].GameObject.transform.position = GetBoardPosition(i, j);
            Matrix[i, j].CurrentI = i;
            Matrix[i, j].CurrentJ = j;
        }

        if (Matrix[targetI, targetJ] != null)
        {
            Matrix[targetI, targetJ].GameObject.transform.position = GetBoardPosition(targetI, targetJ);
            Matrix[targetI, targetJ].CurrentI = targetI;
            Matrix[targetI, targetJ].CurrentJ = targetJ;
        }
        audioSource.PlayOneShot(slideSound);
    }

    private void CheckPieceInput()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        if (puzzleCamera == null)
        {
            Debug.LogError("[Game] No hay cámara asignada.");
            return;
        }

        Vector3 mouseScreenPos = Input.mousePosition;

        int iFound = -1;
        int jFound = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                if (Matrix[i, j] == null || Matrix[i, j].GameObject == null)
                    continue;

                Vector3 pieceScreenPos = puzzleCamera.WorldToScreenPoint(Matrix[i, j].GameObject.transform.position);

                if (pieceScreenPos.z < 0f)
                    continue;

                float dist = Vector2.Distance(
                    new Vector2(mouseScreenPos.x, mouseScreenPos.y),
                    new Vector2(pieceScreenPos.x, pieceScreenPos.y)
                );

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    iFound = i;
                    jFound = j;
                }
            }
        }

        if (iFound == -1 || jFound == -1)
            return;

        if (bestDistance > maxScreenClickDistance)
            return;

        int emptyI;
        int emptyJ;

        FindEmptyCell(out emptyI, out emptyJ);

        int manhattanDistance = Mathf.Abs(iFound - emptyI) + Mathf.Abs(jFound - emptyJ);

        if (manhattanDistance != 1)
        {
            Debug.Log($"Movimiento inválido. Pieza=({iFound},{jFound}) Vacío=({emptyI},{emptyJ})");
            return;
        }

        toAnimateI = emptyI;
        toAnimateJ = emptyJ;
        screenPositionToAnimate = GetBoardPosition(toAnimateI, toAnimateJ);
        pieceToAnimate = Matrix[iFound, jFound];
        gameState = GameState.Animating;
    }

    private void AnimateMovement(Piece toMove, float time)
    {
        if (toMove == null || toMove.GameObject == null)
            return;

        toMove.GameObject.transform.position = Vector3.MoveTowards(
            toMove.GameObject.transform.position,
            screenPositionToAnimate,
            time * AnimSpeed
        );
    }

    private void CheckIfAnimationEnded()
    {
        if (pieceToAnimate == null || pieceToAnimate.GameObject == null)
            return;

        if (Vector3.Distance(pieceToAnimate.GameObject.transform.position, screenPositionToAnimate) < 0.05f)
        {
            Swap(pieceToAnimate.CurrentI, pieceToAnimate.CurrentJ, toAnimateI, toAnimateJ);
            gameState = GameState.Playing;
            CheckForVictory();
        }
    }

    private void CheckForVictory()
    {
        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                bool isLastCell = (i == Constants.MaxColumns - 1 && j == Constants.MaxRows - 1);

                if (isLastCell)
                {
                    if (Matrix[i, j] != null)
                        return;
                }
                else
                {
                    if (Matrix[i, j] == null)
                        return;

                    if (Matrix[i, j].OriginalI != i || Matrix[i, j].OriginalJ != j)
                        return;
                }
            }
        }

        completed = true;
        gameState = GameState.End;

        Debug.Log("PUZZLE COMPLETADO");
        audioSource.PlayOneShot(completeSound);

        Invoke(nameof(CompletePanel), closePanelDelay);
    }

    void CompletePanel()
    {
        if (machineOwner != null)
        {
            machineOwner.MarcarMaquinaReparada();
            machineOwner.CerrarPanelDesdeMinijuego();
        }
        else
        {
            Debug.LogWarning("[Game] No hay MachineOwner asignado.");
        }
    }
}