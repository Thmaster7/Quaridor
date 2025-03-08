
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Color colorOfPiece;

    [SerializeField] private int orderInTurn;

    public bool hasWon;

    public Vector3 forwardVector;
    public int NumPlaysForward;
    public BoardPiece currentBoardPiece;
    public int NumOfBlockPieces;

    private bool isTurnDone;
    [SerializeField] private bool isCurrentlyPlaying;

    public bool IsCurrentlyPlaying
    {
        get { return isCurrentlyPlaying; }
        set { isCurrentlyPlaying = value; }
    }

    public bool IsTurnDone
    {
        get { return isTurnDone; }
        set { isTurnDone = value; }
    }

    

    public void setOrderInTurn(int _orderInTurn)
    {
        orderInTurn = _orderInTurn;
    }

    public int getOrderInTurn()
    {
        return orderInTurn;
    }
    void Start()
    {
        setColor();
        isTurnDone = false;
        isCurrentlyPlaying = false;
        hasWon = false;
        setBlockerPieces();
        
        
    }

    void Update()
    {
        forwardVector = transform.forward;
    }

    void setBlockerPieces()
    {
        QuoridorController QC = GameObject.FindWithTag("GameController").GetComponent<QuoridorController>();

        if (QC.NumberOfPlayers == 2)
        {
            NumOfBlockPieces = 10;
        }
        else if (QC.NumberOfPlayers == 4)
        {
            NumOfBlockPieces = 5;
        }
    }

    void setColor()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        colorOfPiece = GetComponent<Renderer>().material.color;
    }

    public void MakeAMove(BoardPiece _selectedBoardPiece, Vector3 _forwardV)
    {
        if (_selectedBoardPiece.PieceCanBeMovedHere)
        {
            transform.position = _selectedBoardPiece.getPos() + new Vector3(0, transform.localScale.y / 2, 0);
        }

        if (transform.position == _selectedBoardPiece.getPos() + new Vector3(0, transform.localScale.y / 2, 0))
        {
            if (NumPlaysForward != 8)
            {
                isTurnDone = true;

            }
        }
    

    BoardPiece boardForward = null;
    BoardPiece boardForwardTwice = null;
    BoardPiece boardBackWard = null;
    BoardPiece boardBackWardTwice = null;


        if (Mathf.Approximately(Vector3.Dot(_forwardV, Vector3.forward), 1))
		{
			boardForward = currentBoardPiece.FrontBoard;
			boardForwardTwice = currentBoardPiece.frontFrontBoard;
			boardBackWard = currentBoardPiece.BackBoard;
			boardBackWardTwice = currentBoardPiece.backBackBoard;
		}
        else if (Mathf.Approximately(Vector3.Dot(_forwardV, Vector3.back), 1))
        {
            boardForward = currentBoardPiece.BackBoard;
            boardForwardTwice = currentBoardPiece.backBackBoard;
            boardBackWard = currentBoardPiece.FrontBoard;
            boardBackWardTwice = currentBoardPiece.frontFrontBoard;
        }
        else if (Mathf.Approximately(Vector3.Dot(_forwardV, Vector3.right), 1))
        {
            boardForward = currentBoardPiece.RightBoard;
            boardForwardTwice = currentBoardPiece.rightRightBoard;
            boardBackWard = currentBoardPiece.LeftBoard;
            boardBackWardTwice = currentBoardPiece.leftLeftBoard;
        }
        else if (Mathf.Approximately(Vector3.Dot(_forwardV, Vector3.left), 1))
        {
            boardForward = currentBoardPiece.LeftBoard;
            boardForwardTwice = currentBoardPiece.leftLeftBoard;
            boardBackWard = currentBoardPiece.RightBoard;
            boardBackWardTwice = currentBoardPiece.rightRightBoard;
        }
        if(_selectedBoardPiece == boardForward && _selectedBoardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward++;
        }
        else if(_selectedBoardPiece == boardForwardTwice && _selectedBoardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward = NumPlaysForward + 2;
        }
        else if (_selectedBoardPiece == boardBackWard && _selectedBoardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward--;
        }
        else if (_selectedBoardPiece == boardBackWardTwice && _selectedBoardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward = NumPlaysForward - 2;
        }

    }





}
