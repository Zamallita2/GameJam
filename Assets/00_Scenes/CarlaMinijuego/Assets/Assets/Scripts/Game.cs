using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;


public class Game : MonoBehaviour
{
    public Transform puzzleCenter;

    // Use this for initialization
    void Start()
    {
        gameState = GameState.Start;

        SetPiecesScale(0.4f);

        int index = Constants.MaxSize - 1;
        go[index].SetActive(false);

        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                if (go[i * Constants.MaxColumns + j].activeInHierarchy)
                {
                    Vector3 point = GetScreenCoordinatesFromVieport(i, j);
                    go[i * Constants.MaxColumns + j].transform.position = point;

                    Matrix[i, j] = new Piece();
                    Matrix[i, j].GameObject = go[i * Constants.MaxColumns + j];
                    Matrix[i, j].OriginalI = i;
                    Matrix[i, j].OriginalJ = j;
                    Matrix[i, j].CurrentI = i;
                    Matrix[i, j].CurrentJ = j;

                    if (Matrix[i, j].GameObject.GetComponent<BoxCollider2D>() == null)
                        Matrix[i, j].GameObject.AddComponent<BoxCollider2D>();
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
            go[c].transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
        }
    }

    private void Shuffle()
    {
        int shuffleMoves = 10;

        Vector2Int lastMove = Vector2Int.zero;

        // mezclar normalmente
        for (int move = 0; move < shuffleMoves; move++)
        {
            int emptyI = -1;
            int emptyJ = -1;

            // encontrar espacio vacío
            for (int i = 0; i < Constants.MaxColumns; i++)
            {
                for (int j = 0; j < Constants.MaxRows; j++)
                {
                    if (Matrix[i, j] == null)
                    {
                        emptyI = i;
                        emptyJ = j;
                    }
                }
            }

            List<Vector2Int> possibleMoves =
                new List<Vector2Int>();

            // arriba
            if (emptyI > 0 &&
                lastMove != Vector2Int.down)
            {
                possibleMoves.Add(Vector2Int.up);
            }

            // abajo
            if (emptyI < Constants.MaxColumns - 1 &&
                lastMove != Vector2Int.up)
            {
                possibleMoves.Add(Vector2Int.down);
            }

            // izquierda
            if (emptyJ > 0 &&
                lastMove != Vector2Int.right)
            {
                possibleMoves.Add(Vector2Int.left);
            }

            // derecha
            if (emptyJ < Constants.MaxRows - 1 &&
                lastMove != Vector2Int.left)
            {
                possibleMoves.Add(Vector2Int.right);
            }

            Vector2Int dir =
                possibleMoves[
                    Random.Range(0, possibleMoves.Count)];

            int targetI = emptyI;
            int targetJ = emptyJ;

            if (dir == Vector2Int.up)
                targetI--;

            else if (dir == Vector2Int.down)
                targetI++;

            else if (dir == Vector2Int.left)
                targetJ--;

            else if (dir == Vector2Int.right)
                targetJ++;

            Swap(targetI, targetJ, emptyI, emptyJ);

            lastMove = dir;
        }

        // -----------------------------------
        // asegurar hueco abajo derecha
        // -----------------------------------

        int finalEmptyI = -1;
        int finalEmptyJ = -1;

        // buscar nuevamente el hueco
        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                if (Matrix[i, j] == null)
                {
                    finalEmptyI = i;
                    finalEmptyJ = j;
                }
            }
        }

        // mover verticalmente
        while (finalEmptyI < Constants.MaxColumns - 1)
        {
            Swap(finalEmptyI + 1, finalEmptyJ,
                 finalEmptyI, finalEmptyJ);

            finalEmptyI++;
        }

        // mover horizontalmente
        while (finalEmptyJ < Constants.MaxRows - 1)
        {
            Swap(finalEmptyI, finalEmptyJ + 1,
                 finalEmptyI, finalEmptyJ);

            finalEmptyJ++;
        }
    }

    private void Swap(int i, int j, int random_i, int random_j)
    {
        // evitar error si ambos son null
        if (Matrix[i, j] == null &&
            Matrix[random_i, random_j] == null)
        {
            return;
        }

        // intercambio
        Piece temp = Matrix[i, j];
        Matrix[i, j] = Matrix[random_i, random_j];
        Matrix[random_i, random_j] = temp;

        // actualizar posición visual
        if (Matrix[i, j] != null)
        {
            Matrix[i, j].GameObject.transform.position =
                GetScreenCoordinatesFromVieport(i, j);

            Matrix[i, j].CurrentI = i;
            Matrix[i, j].CurrentJ = j;
        }

        if (Matrix[random_i, random_j] != null)
        {
            Matrix[random_i, random_j].GameObject.transform.position =
                GetScreenCoordinatesFromVieport(random_i, random_j);

            Matrix[random_i, random_j].CurrentI = random_i;
            Matrix[random_i, random_j].CurrentJ = random_j;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    Shuffle();
                    gameState = GameState.Playing;
                }
                break;
            case GameState.Playing:
                CheckPieceInput();
                break;
            case GameState.Animating:
                AnimateMovement(PieceToAnimate, Time.deltaTime);
                CheckIfAnimationEnded();
                break;
            case GameState.End:
                break;
            default:
                break;
        }


    }

   
    /// <summary>
    /// boring UI, waiting for uGUI framework :)
    /// </summary>
    void OnGUI()
    {
        switch (gameState)
        {
            case GameState.Start:
                GUI.Label(new Rect(0, 0, 100, 100), "Tap to start!");
                break;
            case GameState.Playing:
                break;
            case GameState.End:
                GUI.Label(new Rect(0, 0, 100, 100), "Congrats, tap to start over!");
                break;
            default:
                break;
        }
    }


    void CheckPieceInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Click en: " + hit.collider.gameObject.name);

                int iFound = -1;
                int jFound = -1;

                for (int i = 0; i < Constants.MaxColumns; i++)
                {
                    for (int j = 0; j < Constants.MaxRows; j++)
                    {
                        if (Matrix[i, j] == null)
                            continue;

                        if (Matrix[i, j].GameObject == hit.collider.gameObject)
                        {
                            iFound = i;
                            jFound = j;
                            break;
                        }
                    }
                }

                if (iFound == -1 || jFound == -1)
                {
                    Debug.Log("No encontré la pieza en la matriz");
                    return;
                }

                bool pieceFound = false;

                if (iFound > 0 && Matrix[iFound - 1, jFound] == null)
                {
                    pieceFound = true;
                    toAnimateI = iFound - 1;
                    toAnimateJ = jFound;
                }
                else if (jFound > 0 && Matrix[iFound, jFound - 1] == null)
                {
                    pieceFound = true;
                    toAnimateI = iFound;
                    toAnimateJ = jFound - 1;
                }
                else if (iFound < Constants.MaxColumns - 1 && Matrix[iFound + 1, jFound] == null)
                {
                    pieceFound = true;
                    toAnimateI = iFound + 1;
                    toAnimateJ = jFound;
                }
                else if (jFound < Constants.MaxRows - 1 && Matrix[iFound, jFound + 1] == null)
                {
                    pieceFound = true;
                    toAnimateI = iFound;
                    toAnimateJ = jFound + 1;
                }

                if (pieceFound)
                {
                    screenPositionToAnimate = GetScreenCoordinatesFromVieport(toAnimateI, toAnimateJ);
                    PieceToAnimate = Matrix[iFound, jFound];
                    gameState = GameState.Animating;
                }
                else
                {
                    Debug.Log("La pieza no está junto al espacio vacío");
                }
            }
            else
            {
                Debug.Log("No golpeó ningún collider");
            }
        }
    }


    private void AnimateMovement(Piece toMove,  float time)
    {
        //animate it
        //Lerp could also be used, but I prefer the MoveTowards approach :)
        toMove.GameObject.transform.position = Vector2.MoveTowards(toMove.GameObject.transform.position, 
          screenPositionToAnimate , time * AnimSpeed);
    }

    /// <summary>
    /// A simple check to see if the animation has finished
    /// </summary>
    private void CheckIfAnimationEnded()
    {
        if(Vector2.Distance(PieceToAnimate.GameObject.transform.position, 
            screenPositionToAnimate) < 0.1f)
        {
            //make sure they swap, exchange positions and stuff
            Swap(PieceToAnimate.CurrentI, PieceToAnimate.CurrentJ, toAnimateI, toAnimateJ);
            gameState = GameState.Playing;
            //check if the use has won
            CheckForVictory();
        }
    }

    private void CheckForVictory()
    {
        int correctIndex = 0;

        for (int i = 0; i < Constants.MaxColumns; i++)
        {
            for (int j = 0; j < Constants.MaxRows; j++)
            {
                // última casilla vacía
                if (i == Constants.MaxColumns - 1 &&
                    j == Constants.MaxRows - 1)
                {
                    if (Matrix[i, j] != null)
                        return;
                }
                else
                {
                    if (Matrix[i, j] == null)
                        return;

                    if (Matrix[i, j].OriginalI != i ||
                        Matrix[i, j].OriginalJ != j)
                    {
                        return;
                    }
                }

                correctIndex++;
            }
        }

        gameState = GameState.End;

        Debug.Log("PUZZLE COMPLETADO");

        transform.root.gameObject.SetActive(false);
    }

    private void ScalePieces() {
        SpriteRenderer spriteRenderer = go[0].GetComponent<SpriteRenderer>();
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight / Screen.height * Screen.width;
        float width = screenWidth / spriteRenderer.sprite.bounds.size.x / 4;
        float height = screenHeight / spriteRenderer.sprite.bounds.size.y / 4;
        for (int c = 0; c < go.Length; c++) {
            go[c].transform.localScale = new Vector3(width, height, 1f);
        }
    }

    private Vector3 GetScreenCoordinatesFromVieport(int i, int j)
    {
        float pieceSize = 1.7f;

        float offsetX = -pieceSize;
        float offsetY = pieceSize;

        Vector3 point = puzzleCenter.position + new Vector3(
            offsetX + (j * pieceSize),
            offsetY - (i * pieceSize),
            0
        );

        return point;
    }

    Vector3 screenPositionToAnimate;
    private Piece PieceToAnimate;
    private int toAnimateI, toAnimateJ;

    public Piece[,] Matrix = new Piece[Constants.MaxColumns, Constants.MaxRows];
    private GameState gameState;
    public GameObject[] go;
    public float AnimSpeed = 10f;
}
