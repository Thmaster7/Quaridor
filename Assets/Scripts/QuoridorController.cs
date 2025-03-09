using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;


public class QuoridorController : MonoBehaviour
{
    public GameObject BoardPiece;
    public GameObject PlayPiece;
    public int NumberOfPlayers;
    public GameObject centerPiece;
    public Canvas canvas;
    public Text text;

    public GameObject BlockerPiece;
    public GameObject PlaceBlockerPiece;
    public Blocker actualBlocker;

    int boardSize;

    GameObject Board, Piece, Blocker, blockerPlace;
    public GameObject BlockerPlace1
    {
        get { return blockerPlace; }
        set { blockerPlace = value; }
    }

    Vector3 side1Start, side2Start, side3Start, side4Start;
    List<Vector3> sidesToPlacePiece = new List<Vector3>();
    List<Piece> piecesOnBoard = new List<Piece>();

    private bool isplaying;
    [SerializeField] private int actualTurn;

    [SerializeField]private Piece movablePiece;
    private Board board;
    private Camera cam;

    private int numberOfTurns;
    public List<PlaceBlocker> placeBlockerList = new List<PlaceBlocker>();

    public Text BlockerCountText;
    public Text PlayerTurnText;

   void Start()
    {
        CreateBoard();
        SetPlayPieces(NumberOfPlayers);
        ChooseOrder(NumberOfPlayers);
        numberOfTurns = 0;
        ChangeTurn();
    }

    void setBlockerCount()
    {
        BlockerCountText.text = "x " + movablePiece.NumOfBlockPieces;
    }
    void CreateBoard()
    {
        Board = new GameObject();
        Blocker = new GameObject();
        blockerPlace = new GameObject();
        Board.name = "BoardContainer";
        Blocker.name = "BlockerContainer";
        blockerPlace.name = "BlockerPlacementPiecesContainer";

        isplaying = true;
        actualTurn = 0;
        actualBlocker = null;

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

                if (i == 0 && j == 4)
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
                if (i == boardSize - 1 && j == 4)
                {
                    side4Start = boardPiece.gameObject.transform.position;
                }
                if (j == 4 && i == 4)
                {
                    centerPiece = boardPiece.gameObject;
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

        if (_numOfPlayers <= 1 || _numOfPlayers > 4)
        {
            Debug.Log("You can only hace from 2 to 4 players");
            return;
        }

        for (int i = 0; i < _numOfPlayers; i++)
        {
            GameObject playPiece = Instantiate(PlayPiece, sidesToPlacePiece[i] + new Vector3(0, 0.5f, 0), Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 1));
            playPiece.transform.parent = Piece.gameObject.transform;

            playPiece.transform.LookAt(centerPiece.transform.position + new Vector3(0, 0.5f, 0));
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
        if (numberOfTurns > 0)
        {
            movablePiece.IsCurrentlyPlaying = false;
            movablePiece.currentBoardPiece = null;

        }

        if (actualTurn > NumberOfPlayers - 1)
        {
            actualTurn = 0;
        }
        actualTurn++;

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            if (piecesOnBoard[i].getOrderInTurn() == actualTurn)
            {
                Debug.Log("Player " + piecesOnBoard[i].getOrderInTurn() + " turn");
                movablePiece = piecesOnBoard[i];
            }

            piecesOnBoard[i].IsTurnDone = false;
        }
        foreach (var _pieceOfBoard in board.BoardPieces)
        {
            _pieceOfBoard.HasActivePlayerOnTop = false;
            _pieceOfBoard.hasPlayerOnTop = false;
            _pieceOfBoard.PieceCanBeMovedHere = false;
            _pieceOfBoard.playerOnTop = null;
        }
        numberOfTurns++;
    }

    void StartGame()
    {
        if (isplaying)
        {
            CheckIfWin();
            setBlockerCount();
            CheckWhoIsCurrentlyPlaying();
            setPlayerTurnText();
            CheckWhereIsPlayer();
            
            if (actualBlocker)
            {
                findSpotToPlaceBlock();
                
                blockerPlace.SetActive(true);
            }
            else
            {
                WaitForMove();
                HighlightBoardPiece();
                blockerPlace.SetActive(false);
            }

            if (movablePiece.IsTurnDone)
            {
                ChangeTurn();
            }
        }
    }

    void setPlayerTurnText()
    {
        if (isplaying)
        {
            PlayerTurnText.text = "Player's " + movablePiece.getOrderInTurn() + " Turn";
        }
        else
        {
            foreach (var _piece in piecesOnBoard)
            {
                if (_piece.NumPlaysForward == boardSize - 1)
                {

                    PlayerTurnText.text = "Player " + _piece.getOrderInTurn() + " wins!";

                }
            }
        }
    }
    
    public void addBlockerPiece()
    {
        if (movablePiece.NumOfBlockPieces > 0)
        {
            GameObject blockerPiece = Instantiate(BlockerPiece, Vector3.zero, Quaternion.identity);
            blockerPiece.transform.parent = Blocker.gameObject.transform;

            actualBlocker = blockerPiece.GetComponent<Blocker>();

            actualBlocker.isBeingDragged = true;
        }

    }

    public void findSpotToPlaceBlock()
    {
        if (movablePiece.NumOfBlockPieces > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    return;
                }

                if (hit.transform.gameObject.tag == "PlaceBlocker")
                {
                    PlaceBlocker blockerPlace = hit.transform.GetComponent<PlaceBlocker>();
                    actualBlocker.PlaceBlockerOnBoard(hit.transform.gameObject);
                    movablePiece.IsTurnDone = true;
                    blockerPlace.setBlocker(actualBlocker);
                    actualBlocker = null;
                    movablePiece.NumOfBlockPieces--;

                    blockerPlace.hasBlocker = true;
                    if (blockerPlace.BP1)
                    {
                        blockerPlace.BP1.HasSurroundingBlocker = true;
                    }
                        
                    if (blockerPlace.BP2)
                    {
                        blockerPlace.BP2.HasSurroundingBlocker = true;
                    }
                        
                    if (blockerPlace.BP3)
                    {
                        blockerPlace.BP3.HasSurroundingBlocker = true;
                    }
                        
                    if (blockerPlace.BP4)
                    {
                        blockerPlace.BP4.HasSurroundingBlocker = true;
                    }
                        

                }

            }
        }
    }
    void CheckIfWin()
    {
        foreach (var _piece in piecesOnBoard)
        {
            if (_piece.NumPlaysForward == boardSize - 1)
            {
                isplaying = false;
                canvas.gameObject.SetActive(true);
                text.text = "Player " + _piece.getOrderInTurn() + " has won the game!";
            }
        }
    }

    void CheckWhoIsCurrentlyPlaying()
    {
        if (movablePiece.getOrderInTurn() == actualTurn)
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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                return;
            }

            if (hit.transform.gameObject.tag == "Board_Segment")
            {
                BoardPiece _boardPiece = hit.transform.GetComponent<BoardPiece>();
                if (!_boardPiece.hasPlayerOnTop)
                {
                    movablePiece.MakeAMove(_boardPiece, movablePiece.transform.forward);
                }
            }

        }
    }
    


    void HighlightBoardPiece()
    {
       foreach (var _boardPiece in board.BoardPieces)
        {
            if (_boardPiece.HasActivePlayerOnTop)
            {
                if (_boardPiece.FrontBoard != null)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.FrontBoard.hasPlayerOnTop)
                        {
                            if (_boardPiece.FrontBoard)
                                _boardPiece.FrontBoard.PieceCanBeMovedHere = true;
                        }
                        else
                        {

                            RaycastHit hit2;
                            if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                .transform.position, transform.TransformDirection(Vector3.forward), out hit2))
                            {
                                if (_boardPiece.FrontBoard.HasSurroundingBlocker)
                                {

                                    if (_boardPiece.frontFrontBoard.hasPlayerOnTop)
                                    {
                                        if (_boardPiece.frontFrontBoard)
                                            _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;

                                    }
                                    else
                                    {
                                        if (_boardPiece.frontFrontBoard)
                                            _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;


                                    }

                                }
                                else
                                {
                                    if (_boardPiece.frontFrontBoard)
                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.frontFrontBoard)
                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;

                            }

                        }
                    }
                    else if (_boardPiece.HasSurroundingBlocker)
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(movablePiece.transform.position,
                            transform.TransformDirection(Vector3.forward),  0.6f
                            ))
                        {

                            if (_boardPiece.FrontBoard)
                                _boardPiece.FrontBoard.PieceCanBeMovedHere = false;


                        }
                        else
                        {

                            if (_boardPiece.FrontBoard.hasPlayerOnTop)
                            {

                                RaycastHit hit3;
                                if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                    .transform.position, transform.TransformDirection(Vector3.forward), out hit3))
                                {

                                    RaycastHit hit4;
                                    if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.right), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Vertical)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Horizontal)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.frontFrontBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.frontFrontBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;
                                                }


                                            }
                                            else
                                            {
                                                if (_boardPiece.frontFrontBoard)
                                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                            }

                                        }
                                        else
                                        {
                                            if (_boardPiece.frontFrontBoard)
                                                _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;


                                        }

                                    }

                                    if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.left), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Vertical)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Horizontal)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.frontFrontBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.frontFrontBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;
                                                }



                                            }
                                            else
                                            {
                                                if (_boardPiece.frontFrontBoard)
                                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.frontFrontBoard)
                                                _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;

                                        }

                                    }


                                }
                                else
                                {
                                    if (_boardPiece.frontFrontBoard)
                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.FrontBoard)
                                    _boardPiece.FrontBoard.PieceCanBeMovedHere = true;


                            }
                        }

                    }

                }




                if (_boardPiece.RightBoard != null)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.RightBoard.hasPlayerOnTop)
                        {
                            if (_boardPiece.RightBoard)
                                _boardPiece.RightBoard.PieceCanBeMovedHere = true;
                        }
                        else
                        {
                            RaycastHit hit2;
                            if (Physics.Raycast(_boardPiece.RightBoard.playerOnTop.gameObject
                                .transform.position, transform.TransformDirection(Vector3.right), out hit2))
                            {
                                if (_boardPiece.RightBoard.HasSurroundingBlocker)
                                {


                                    if (_boardPiece.rightRightBoard.hasPlayerOnTop)
                                    {
                                        if (_boardPiece.rightRightBoard)
                                            _boardPiece.rightRightBoard.PieceCanBeMovedHere = false;

                                    }
                                    else
                                    {
                                        if (_boardPiece.rightRightBoard)
                                            _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;


                                    }

                                }
                                else
                                {
                                    if (_boardPiece.rightRightBoard)
                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.rightRightBoard)
                                    _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                            }
                        }

                    }
                    else
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(movablePiece.transform.position,
                            transform.TransformDirection(Vector3.right),
                            0.6f))
                        {

                            if (_boardPiece.RightBoard)
                                _boardPiece.RightBoard.PieceCanBeMovedHere = false;


                        }
                        else
                        {
                            if (_boardPiece.RightBoard.hasPlayerOnTop)
                            {
                                RaycastHit hit3;
                                if (Physics.Raycast(_boardPiece.RightBoard.playerOnTop.gameObject
                                    .transform.position, transform.TransformDirection(Vector3.right), out hit3))
                                {
                                    RaycastHit hit4;
                                    if (Physics.Raycast(_boardPiece.RightBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.back), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Horizontal)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Vertical)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.rightRightBoard)
                                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.rightRightBoard)
                                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = false;
                                                }

                                            }
                                            else
                                            {
                                                if (_boardPiece.rightRightBoard)
                                                    _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.rightRightBoard)
                                                _boardPiece.rightRightBoard.PieceCanBeMovedHere = false;
                                        }

                                    }

                                    if (Physics.Raycast(_boardPiece.RightBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.forward), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Horizontal)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Vertical)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.rightRightBoard)
                                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.rightRightBoard)
                                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = false;
                                                }

                                            }
                                            else
                                            {
                                                if (_boardPiece.rightRightBoard)
                                                    _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.rightRightBoard)
                                                _boardPiece.rightRightBoard.PieceCanBeMovedHere = false;
                                        }       

                                    }
                                }
                                else
                                {
                                    if (_boardPiece.rightRightBoard)
                                        _boardPiece.rightRightBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.RightBoard)
                                    _boardPiece.RightBoard.PieceCanBeMovedHere = true;
                            }
                        }
                    }

                }

                if (_boardPiece.LeftBoard != null)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.LeftBoard.hasPlayerOnTop)
                        {
                            if (_boardPiece.LeftBoard)
                                _boardPiece.LeftBoard.PieceCanBeMovedHere = true;

                        }
                        else
                        {
                            RaycastHit hit2;
                            if (Physics.Raycast(_boardPiece.LeftBoard.playerOnTop.gameObject
                                .transform.position, transform.TransformDirection(Vector3.left), out hit2))
                            {
                                if (_boardPiece.LeftBoard.HasSurroundingBlocker)
                                {
                                    if (_boardPiece.leftLeftBoard.hasPlayerOnTop)
                                    {
                                        if (_boardPiece.leftLeftBoard)
                                            _boardPiece.leftLeftBoard.PieceCanBeMovedHere = false;

                                        Debug.Log("jejejeje");

                                    }
                                    else
                                    {
                                        if (_boardPiece.leftLeftBoard)
                                            _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;





                                    }
                                }
                                else
                                {
                                    if (_boardPiece.leftLeftBoard)
                                        _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;


                                }
                            }
                            else
                            {

                                if (_boardPiece.leftLeftBoard)
                                {
                                    _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;
                                }

                            }
                        }

                    }
                    else
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(movablePiece.transform.position,
                            transform.TransformDirection(Vector3.left),
                            0.6f))
                        {

                            if (_boardPiece.LeftBoard)
                                _boardPiece.LeftBoard.PieceCanBeMovedHere = false;

                        }
                        else
                        {
                            if (_boardPiece.LeftBoard.hasPlayerOnTop)
                            {
                                RaycastHit hit3;
                                if (Physics.Raycast(_boardPiece.LeftBoard.playerOnTop.gameObject
                                    .transform.position, transform.TransformDirection(Vector3.left), out hit3))
                                {
                                    RaycastHit hit4;
                                    if (Physics.Raycast(_boardPiece.LeftBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.back), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Horizontal)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Vertical)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.leftLeftBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.leftLeftBoard)
                                                        _boardPiece.frontFrontBoard.PieceCanBeMovedHere = false;
                                                }

                                            }
                                            else
                                            {
                                                if (_boardPiece.leftLeftBoard)
                                                    _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;

                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.leftLeftBoard)
                                                _boardPiece.leftLeftBoard.PieceCanBeMovedHere = false;

                                            Debug.Log("jejejeje");
                                        }

                                    }

                                    if (Physics.Raycast(_boardPiece.LeftBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.forward), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Horizontal)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Vertical)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.leftLeftBoard)
                                                        _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.leftLeftBoard)
                                                        _boardPiece.leftLeftBoard.PieceCanBeMovedHere = false;

                                                    Debug.Log("jejejeje");
                                                }

                                            }
                                            else
                                            {
                                                if (_boardPiece.leftLeftBoard)
                                                    _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;


                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.leftLeftBoard)
                                                _boardPiece.leftLeftBoard.PieceCanBeMovedHere = false;

                                            Debug.Log("jejejeje");
                                        }

                                    }
                                }
                                else
                                {
                                    if (_boardPiece.leftLeftBoard)
                                        _boardPiece.leftLeftBoard.PieceCanBeMovedHere = true;


                                }
                            }
                            else
                            {
                                if (_boardPiece.LeftBoard)
                                    _boardPiece.LeftBoard.PieceCanBeMovedHere = true;


                            }
                        }
                    }

                }

                if (_boardPiece.BackBoard != null)
                {

                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.BackBoard.hasPlayerOnTop)
                        {
                            if (_boardPiece.BackBoard)
                                _boardPiece.BackBoard.PieceCanBeMovedHere = true;
                        }
                        else
                        {
                            RaycastHit hit2;
                            if (Physics.Raycast(_boardPiece.BackBoard.playerOnTop.gameObject
                                .transform.position, transform.TransformDirection(Vector3.back), out hit2))
                            {
                                if (_boardPiece.BackBoard.HasSurroundingBlocker)
                                {
                                    if (_boardPiece.backBackBoard.hasPlayerOnTop)
                                    {
                                        if (_boardPiece.backBackBoard)
                                            _boardPiece.backBackBoard.PieceCanBeMovedHere = false;

                                    }
                                    else
                                    {
                                        if (_boardPiece.backBackBoard)
                                            _boardPiece.backBackBoard.PieceCanBeMovedHere = true;


                                    }
                                }
                                else
                                {
                                    if (_boardPiece.backBackBoard)
                                        _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.backBackBoard)
                                    _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                            }
                        }

                    }
                    else
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(movablePiece.transform.position,
                            transform.TransformDirection(Vector3.back),
                            0.6f))
                        {

                            if (_boardPiece.BackBoard)
                                _boardPiece.BackBoard.PieceCanBeMovedHere = false;

                        }
                        else
                        {
                            if (_boardPiece.BackBoard.hasPlayerOnTop)
                            {
                                RaycastHit hit3;
                                if (Physics.Raycast(_boardPiece.BackBoard.playerOnTop.gameObject
                                    .transform.position, transform.TransformDirection(Vector3.back), out hit3))
                                {
                                    RaycastHit hit4;
                                    if (Physics.Raycast(_boardPiece.BackBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.left), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Vertical)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Horizontal)
                                            {
                                                if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                    == global::Blocker.OrientationEnum.Horizontal)
                                                {
                                                    if (hit3.distance > 1.3f)
                                                    {
                                                        if (_boardPiece.backBackBoard)
                                                            _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                                    }
                                                    else
                                                    {
                                                        if (_boardPiece.backBackBoard)
                                                            _boardPiece.backBackBoard.PieceCanBeMovedHere = false;
                                                    }

                                                }
                                                else
                                                {
                                                    if (_boardPiece.backBackBoard)
                                                        _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                                }
                                            }
                                            else
                                            {
                                                if (_boardPiece.backBackBoard)
                                                    _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.backBackBoard)
                                                _boardPiece.backBackBoard.PieceCanBeMovedHere = false;
                                        }

                                    }

                                    if (Physics.Raycast(_boardPiece.BackBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.right), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                            == global::Blocker.OrientationEnum.Vertical)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Horizontal)
                                            {
                                                if (hit3.distance > 1.3f)
                                                {
                                                    if (_boardPiece.backBackBoard)
                                                        _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                                }
                                                else
                                                {
                                                    if (_boardPiece.backBackBoard)
                                                        _boardPiece.backBackBoard.PieceCanBeMovedHere = false;
                                                }

                                            }
                                            else
                                            {
                                                if (_boardPiece.backBackBoard)
                                                    _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                            }
                                        }
                                        else
                                        {
                                            if (_boardPiece.backBackBoard)
                                                _boardPiece.backBackBoard.PieceCanBeMovedHere = false;
                                        }

                                    }
                                }
                                else
                                {
                                    if (_boardPiece.backBackBoard)
                                        _boardPiece.backBackBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                if (_boardPiece.BackBoard)
                                    _boardPiece.BackBoard.PieceCanBeMovedHere = true;
                            }
                        }
                    }


                }
            }


        }
        int layerMask = LayerMask.GetMask("BoardPieces");

        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))

        {
            BoardPiece _boardPiece = hit.transform.GetComponent<BoardPiece>();

            if (!cam.GetComponent<SimpleCameraController>().IsCameraMoving)
            {
                if (!_boardPiece.hasPlayerOnTop)
                    _boardPiece.setHighlight(true, movablePiece, board);
            }


        }
        else
        {
            foreach (var boardPiece in board.BoardPieces)
            {
                boardPiece.setHighlight(false, movablePiece, board);
            }
        }
        /*foreach (var _boardPiece in board.BoardPieces)
        {
            if (_boardPiece.HasActivePlayerOnTop)
            {
                if (_boardPiece.FrontBoard)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.FrontBoard.hasPlayerOnTop)
                        {


                            _boardPiece.FrontBoard.PieceCanBeMovedHere = true;

                        }
                        else
                        {
                            _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                        }
                    }
                    else
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                .transform.position, transform.TransformDirection(Vector3.forward), out hit2))
                        {
                            if (!_boardPiece.FrontBoard.HasSurroundingBlocker)
                            {
                                if (!_boardPiece.FrontBoard.hasPlayerOnTop)
                                {
                                    if (_boardPiece.FrontBoard)
                                    {
                                        _boardPiece.FrontBoard.PieceCanBeMovedHere = true;
                                    }
                                }
                                else
                                {
                                    _boardPiece.frontFrontBoard.PieceCanBeMovedHere = true;
                                }
                            }
                            else
                            {
                                RaycastHit hit3;
                                if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                    .transform.position, transform.TransformDirection(Vector3.forward), out hit3))

                                {
                                    RaycastHit hit4;
                                    if (Physics.Raycast(_boardPiece.FrontBoard.playerOnTop.gameObject
                                        .transform.position, transform.TransformDirection(Vector3.right), out hit4))
                                    {
                                        if (hit4.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Vertical)
                                        {
                                            if (hit3.transform.gameObject.GetComponent<Blocker>().orientation
                                                == global::Blocker.OrientationEnum.Horizontal)
                                            {
                                                if (hit3.distance > 1.3f && hit4.distance > 1.3f)
                                                {
                                                    if (_boardPiece.FrontBoard.HasSurroundingBlocker)
                                                    {
                                                        _boardPiece.FrontBoard.PieceCanBeMovedHere = false;
                                                    }
                                                    else
                                                    {
                                                        _boardPiece.FrontBoard.PieceCanBeMovedHere = true;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                            }
                        


                        }
                    }
                }
                
                if (_boardPiece.RightBoard)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.RightBoard.hasPlayerOnTop)
                        {

                            _boardPiece.RightBoard.PieceCanBeMovedHere = true;
                        }
                    }
                }
                if (_boardPiece.LeftBoard)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.LeftBoard.hasPlayerOnTop)
                        {
                            _boardPiece.LeftBoard.PieceCanBeMovedHere = true;
                        }
                        
                    }
                }
                if (_boardPiece.BackBoard)
                {
                    if (!_boardPiece.HasSurroundingBlocker)
                    {
                        if (!_boardPiece.BackBoard.hasPlayerOnTop)
                        {
                            
                            _boardPiece.BackBoard.PieceCanBeMovedHere = true;
                        }
                    }
                }
            }
        }*/







    }
}
