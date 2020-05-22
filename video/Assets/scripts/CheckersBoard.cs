using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour
{
    public piece[,] pieces = new piece[8, 8];
    public GameObject whitePiece;
    public GameObject blackPiece;

    public Vector3 boardOfset = new Vector3(-4.0f, 0, -4.0f);
    public Vector3 pieceOfset = new Vector3(0.5f, 0, 0.5f);
    private Vector2 moseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    public bool isWhite;
    private bool isWhiteTurn;
    private bool hasKilled;

    private piece selectedPiece;

    private void Start()
    {
        isWhite = true;
        isWhiteTurn = true;
        GenerateBoard();
        forcedPieces = new List<piece>();
    }

    private void Update()
    {
        UpdateMouseOver();

        //if((isWhite)?isWhiteTurn:!isWhiteTurn)
        {
            int x = (int)moseOver.x;
            int y = (int)moseOver.y;

            if (selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }
            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }
    private void SelectPiece(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return;

        piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = moseOver;
            }
            else
            {
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = moseOver;
            }

        }
    }
    private List<piece> forcedPieces;
    private void UpdateMouseOver()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            moseOver.x = (int)(hit.point.x - boardOfset.x);
            moseOver.y = (int)(hit.point.z - boardOfset.z);
        }
        else
        {
            moseOver.x = -1;
            moseOver.y = -1;
        }
    }
    private void GenerateBoard()
    {
        //generate teams
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                //gamerate piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                //gamerate piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }
    private void UpdatePieceDrag(piece p)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void GeneratePiece(int x, int y)
    {
        bool isWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isWhite) ? whitePiece : blackPiece) as GameObject;
        go.transform.SetParent(transform);
        piece p = go.GetComponent<piece>();
        pieces[x, y] = p;
        movePiece(p, x, y);
    }
    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // Promotions
        if (selectedPiece != null)
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }

        startDrag = Vector2.zero;

        if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
        {
            if(selectedPiece == whitePiece)
            {
                Debug.Log("white");
            }
            else
            {
                Debug.Log("black");
            }
            Debug.Log("going");
            return;
        }

        selectedPiece = null;

        isWhite = !isWhite;
        isWhiteTurn = !isWhiteTurn;
        hasKilled = false;
        CheckVictory();
    }
    private void CheckVictory()
    {

    }
    private List<piece> ScanForPossibleMove(piece p,int x, int y)
    {
        forcedPieces = new List<piece>();

        if (pieces[x, y].IsForceToMove(pieces, x, y))
            forcedPieces.Add(pieces[x, y]);

        return forcedPieces;
    }
    private List<piece> ScanForPossibleMove()
    {
        forcedPieces = new List<piece>();
        // check all the pieces
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);
        return forcedPieces;
    }
    private void movePiece(piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOfset + pieceOfset;
    }
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForPossibleMove();

        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            if (selectedPiece != null)
                movePiece(selectedPiece, x1, y1);
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if (selectedPiece != null)
        {
            // if it has not moved
            if (endDrag == startDrag)
            {
                movePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            // check if its a valid move
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                //did we kill??
                if (Mathf.Abs(x2 - x1) == 2)
                {
                    piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                // where we suposed to kill??
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    movePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                movePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                movePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }

    }
}
