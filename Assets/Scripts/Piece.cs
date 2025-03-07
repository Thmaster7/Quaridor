using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Color colorOfPiece;

    [SerializeField] private int orderInTurn;

    public bool hasWon;

    public Vector3 forwardVector;
    public int NumPlaysForward;
    public BoardPiece currentBoardPiece;

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
        
        
    }

    void Update()
    {
        forwardVector = transform.forward;
    }

    void setColor()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        colorOfPiece = GetComponent<Renderer>().material.color;
    }

    public void MakeAMove(BoardPiece _boardPiece, Vector3 _forwardV)
    {
        if (_boardPiece.PieceCanBeMovedHere)
        {
            transform.position = _boardPiece.getPos() + new Vector3(0, transform.localScale.y / 2, 0);
        }

        if (transform.position == _boardPiece.getPos() + new Vector3(0, transform.localScale.y / 2, 0))
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
        if(_boardPiece == boardForward && _boardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward++;
        }
        else if(_boardPiece == boardForwardTwice && _boardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward = NumPlaysForward + 2;
        }
        else if (_boardPiece == boardBackWard && _boardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward--;
        }
        else if (_boardPiece == boardBackWardTwice && _boardPiece.PieceCanBeMovedHere)
        {
            NumPlaysForward = NumPlaysForward - 2;
        }

    }





}
