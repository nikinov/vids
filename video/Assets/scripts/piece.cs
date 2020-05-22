﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piece : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    public bool IsForceToMove(piece[,]board, int x, int y)
    {
        if(isWhite || isKing)
        {
            // Top Left
            if(x >=2 && y <=5)
            {
                piece p = board[x - 1, y + 1];

                if(p != null && p.isWhite != isWhite)
                {
                    if(board[x - 2,y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Top Right
            if (x <= 2 && y <= 5)
            {
                piece p = board[x + 1, y + 1];

                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if(!isWhite || isKing)
        {
            // Bottom Left
            if (x >= 2 && y >= 2)
            {
                piece p = board[x - 1, y - 1];

                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Bottom Right
            if (x <= 2 && y >= 2)
            {
                piece p = board[x + 1, y - 1];

                if (p != null && p.isWhite != isWhite)
                {
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;

    }
    public bool ValidMove(piece[,]board, int x1, int y1, int x2, int y2)
    {
        if(board[x2, y2] != null)
        {
            return false;
        }
        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;
        if(isWhite || isKing)
        {
            if(deltaMove == 1)
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if(deltaMove == 2)
            {
                if(deltaMoveY == 2)
                {
                    piece p = board[(x1+x2)/2, (y1 + y2)/ 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        if (!isWhite || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        return false;
    }

}
