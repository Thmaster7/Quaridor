using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class QuoridorController : MonoBehaviour
{
    public GameObject BoardPiece;
    public GameObject PlayPiece;
    public int NumberOfPlayers;
    
    int boardSize;
    
    GameObject Board, Piece;
    

    Vector3 side1Start, side2Start, side3Start, side4Start;
    List<Vector3> sidesToPlacePiece = new List<Vector3>();
    List<Piece> piecesOnBoard = new List<Piece>();

    private bool isplaying;
    [SerializeField] private int actualTurn;

    private Piece movablePiece;
    private Board board;
    private Camera cam;

    private int numberOfTurns;

    private void Start()
    {
        CreateBoard();
        SetPlayPieces(NumberOfPlayers);
        ChooseOrder(NumberOfPlayers);
        numberOfTurns = 0;
        ChangeTurn();
    }
    void CreateBoard()
    {
        Board = new GameObject();
        Board.name = "BoardContainer";

        isplaying = true;
        actualTurn = 0;

        board = new Board();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        side1Start = side2Start = side3Start = side4Start = new Vector3(-1, -1, -1);

        boardSize = 9;
        float dx = 0;
        float dz = 0;
        float offset = 0.2f;

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                GameObject boardPiece = Instantiate(BoardPiece, new Vector3(dx, 0, dz), Quaternion.identity);
                boardPiece.transform.parent = Board.gameObject.transform;
                BoardPiece b = boardPiece.GetComponent<BoardPiece>();
                b.setColumn(j);
                b.setRow(i + 1);
                b.setPos(boardPiece.gameObject.transform.position);
                board.AddPiece(b);

                if(i == 0 && j == 4)
                {
                    side1Start = boardPiece.gameObject.transform.position;
                }
                if (i == 4 && j == boardSize - 1)
                {
                    side2Start = boardPiece.gameObject.transform.position;
                }
                if (i == 4 && j == 0)
                {
                    side3Start = boardPiece.gameObject.transform.position;
                }
                if (i == boardSize -1 && j == 4)
                {
                    side4Start = boardPiece.gameObject.transform.position;
                }

                dx = dx + BoardPiece.transform.localScale.x + offset;
            }
            dx = 0;
            dz = dz + BoardPiece.transform.localScale.z + offset;
        }
        sidesToPlacePiece.Add(side1Start);
        sidesToPlacePiece.Add(side4Start);
        sidesToPlacePiece.Add(side2Start);
        sidesToPlacePiece.Add(side3Start);
    }
    void SetPlayPieces(int _numOfPlayers)
    {
        Piece = new GameObject();
        Piece.name = "PieceContainer";

        if ( _numOfPlayers <= 1 || _numOfPlayers > 4)
        {
            Debug.Log("You can only hace from 2 to 4 players");
            return;
        }

        for(int i = 0; i <_numOfPlayers; i++)
        {
            GameObject playPiece = Instantiate(PlayPiece, sidesToPlacePiece[i] + new Vector3(0,0.5f,0), Quaternion.identity);
            playPiece.transform.parent = Piece.gameObject.transform;
            piecesOnBoard.Add(playPiece.GetComponent<Piece>());
        }
    }

    void ChooseOrder(int _numOfPlayers)
    {
        int chooseStarter = 0;

        if (chooseStarter == 0)
        {
            int order = 1;
            foreach (var piece in piecesOnBoard)
            {
                piece.setOrderInTurn(order);
                order++;
            }
        }
    }
    private void Update()
    {
        StartGame();
    }
    void ChangeTurn()
    {
        if(numberOfTurns > 0 )
        {
            movablePiece.IsCurrentlyPlaying = false;
            movablePiece.currentBoardPiece = null;

        }
        
        if (actualTurn > NumberOfPlayers - 1 )
        {
            actualTurn = 0;
        }
        actualTurn++;

        for(int i = 0; i < NumberOfPlayers;i++)
        {
            if (piecesOnBoard[i].getOrderInTurn()==actualTurn)
            {
                Debug.Log("Player " + piecesOnBoard[i].getOrderInTurn() + " turn");
                movablePiece = piecesOnBoard[i];
            }

            piecesOnBoard[i].IsTurnDone = false;
        }
        foreach (var _pieceOfBoard in board.BoardPieces)
        {
            _pieceOfBoard.HasActivePlayerOnTop = false;
            _pieceOfBoard.PieceCanBeMovedHere = false;
        }
        numberOfTurns++;
    }

    void StartGame()
    {
        if (isplaying)
        {
            WaitForMove();
            CheckWhoIsCurrentlyPlaying();
            HighlightBoardPiece();
            CheckWhereIsPlayer();
            
            if(movablePiece.IsTurnDone)
            {
                ChangeTurn();
            }
        }
    }

    void CheckWhoIsCurrentlyPlaying()
    {
        if(movablePiece.getOrderInTurn() == actualTurn)
        {
            movablePiece.IsCurrentlyPlaying = true;
        }
    }

    void CheckWhereIsPlayer()
    {
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            foreach (var _boardPiece in board.BoardPieces)
            {

                _boardPiece.checkIfActivePlayerOnTop(movablePiece);
            }
            foreach (var _boardPiece in board.BoardPieces)
            {
                _boardPiece.checkIfAnyPlayerOnTop(piecesOnBoard[i]);
            }


        }
        for (int i = 0; i < board.GetNumberOfPieces(); i++)
        {
            if (board.BoardPieces[i].HasActivePlayerOnTop)
            {
                movablePiece.currentBoardPiece = board.BoardPieces[i];
            }
        }

    }

    void WaitForMove()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                return;
            }

            if (hit.transform.gameObject.tag == "Board_Segment")
            {
                BoardPiece _boardPiece = hit.transform.GetComponent<BoardPiece>();
                if(!_boardPiece.HasActivePlayerOnTop)
                {
                    movablePiece.MakeAMove(_boardPiece, movablePiece.transform.forward);
                }
            }
            
        }
    }

    void HighlightBoardPiece()
    {
        foreach(var _boardPiece in board.BoardPieces)
        {
            if(_boardPiece.HasActivePlayerOnTop)
            {
                if(_boardPiece.FrontBoard != null)
                {
                    if(!_boardPiece.FrontBoard.hasPlayerOnTop)
                    {
                        if (_boardPiece.FrontBoard)
                        {
                            _boardPiece.FrontBoard.PieceCanBeMovedHere = true;
                        }
                            
                    }
                    else
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                            .transform.position, transform.TransformDirection(Vector3.forward), out hit2))
                        {
                            if (_boardPiece.frontFrontBoard.hasPlayerOnTop)
                            {
                                if (_boardPiece.frontFrontBoard)
                                {
                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;
                                }
                                    

                            }
                            else
                            {
                                if (_boardPiece.frontFrontBoard)
                                {
                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                }
                                    


                            }
                        }
                        else
                        {
                            if (_boardPiece.frontFrontBoard)
                                _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;

                        }
                    }
                    
                }
                /*if (_boardPiece.RightBoard != null)
                {
                    _boardPiece.RightBoard.PieceCanBeMovedHere = true;
                }
                if (_boardPiece.LeftBoard != null)
                {
                    _boardPiece.LeftBoard.PieceCanBeMovedHere = true;
                }
                if (_boardPiece.BackBoard != null)
                {
                    _boardPiece.BackBoard.PieceCanBeMovedHere = true;
                }*/
            }
        }

        RaycastHit hit;

        if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            BoardPiece _boardPiece = hit.transform.GetComponent<BoardPiece>();

            if(!cam.GetComponent<SimpleCameraController>().IsCameraMoving)
            {
                _boardPiece.setHighlight(true, movablePiece, board);
            }
        }
        else
        {
            foreach(var boardPiece in board.BoardPieces)
            {
                boardPiece.setHighlight(false, movablePiece, board);
            }
        }



    }
}
