using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    [SerializeField] BoardCoordinate boardCoordinate;

    int column;
    int row;
    Vector3 pos;
    private Color col;
    

    [SerializeField] private bool isHighlighted;
    [SerializeField] private bool pieceCanBeMovedHere;

    public bool PieceCanBeMovedHere
    {
        get { return pieceCanBeMovedHere; }
        set { pieceCanBeMovedHere = value; }
    }

    [SerializeField] private BoardPiece frontBoard;
    [SerializeField] private BoardPiece rightBoard;
    [SerializeField] private BoardPiece leftBoard;
    [SerializeField] private BoardPiece backBoard;
    public BoardPiece diagonalBoard;
    public BoardPiece rightRightBoard;
    public BoardPiece leftLeftBoard;
    public BoardPiece backBackBoard;
    public BoardPiece frontFrontBoard;

    public bool hasPlayerOnTop;
    public Piece playerOnTop;

    public BoardPiece FrontBoard
    {
        get { return frontBoard; }
        set { frontBoard = value; }
    }
    public BoardPiece RightBoard
    {
        get { return rightBoard; }
        set { rightBoard = value; }
    }
    public BoardPiece LeftBoard
    {
        get { return leftBoard; }
        set { leftBoard = value; }
    }
    public BoardPiece BackBoard
    {
        get { return backBoard; }
        set { backBoard = value; }
    }

    [SerializeField] private bool hasActivePlayerOnTop;

    public bool HasActivePlayerOnTop
    {
        get { return hasActivePlayerOnTop; }
        set { hasActivePlayerOnTop = value; }
    }

    public void setColumn(int _column)
    {
        column = _column;
    }
    public void setRow(int _row)
    {
        row = _row;
    }
    public void setPos(Vector3 _pos)
    {
        pos = _pos;
    }

    public Vector3 getPos()
    {
        return pos;
    }

    private void Start()
    {
        boardCoordinate = new BoardCoordinate(column, row, transform.position);
        isHighlighted = false;
        col = gameObject.GetComponent<Renderer>().material.color;
        frontBoard = leftBoard = rightBoard = backBoard = null;
        hasActivePlayerOnTop = false;
        pieceCanBeMovedHere = false;
        hasPlayerOnTop = false;
        playerOnTop = null;

        FindNeighbors();
    }

    private void Update()
    {
        
    }

    public void checkIfActivePlayerOnTop(Piece _piece)
    {
        if(Mathf.Approximately(transform.position.x, _piece.gameObject.transform.position.x)&& Mathf.Approximately(transform.position.z, _piece.gameObject.transform.position.z))
        {
            hasActivePlayerOnTop = true;
        }
    }

    public void checkIfAnyPlayerOnTop(Piece _piece)
    {
        if (Mathf.Approximately(transform.position.x, _piece.gameObject.transform.position.x) && Mathf.Approximately(transform.position.z, _piece.gameObject.transform.position.z))
        {
            hasPlayerOnTop = true;
            playerOnTop = _piece;
        }
    }

    void FindNeighbors()
    {
        RaycastHit hit;
        GameObject frontBoard2 = null;
        GameObject rightBoard2 = null;
        GameObject leftBoard2 = null;
        GameObject backBoard2 = null;


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            frontBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            frontBoard2 = hit.transform.gameObject;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit))
        {
            rightBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            rightBoard2 = hit.transform.gameObject;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit))
        {
            leftBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            leftBoard2 = hit.transform.gameObject;
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit))
        {
            backBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            backBoard2 = hit.transform.gameObject;
        }

        
        if (frontBoard)
        {
            if (Physics.Raycast(frontBoard2.transform.position,
                transform.TransformDirection(Vector3.forward), out hit))
            {
                frontFrontBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            }
        }

        if (rightBoard)
        {
            if (Physics.Raycast(rightBoard2.transform.position,
                transform.TransformDirection(Vector3.right), out hit))
            {
                rightRightBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            }
        }
        if (leftBoard)
        {
            if (Physics.Raycast(leftBoard2.transform.position,
                transform.TransformDirection(Vector3.left), out hit))
            {
                leftLeftBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            }
        }
        if (backBoard)
        {
            if (Physics.Raycast(backBoard2.transform.position,
                transform.TransformDirection(Vector3.back), out hit))
            {
                backBackBoard = hit.transform.gameObject.GetComponent<BoardPiece>();
            }
        }
    }
    public Vector3 getBoardPiecePos()
    {
        return boardCoordinate.getPos();
    }

    public void setHighlight(bool _isHighlighted, Piece _piece, Board _board)
    {
        isHighlighted = _isHighlighted;

        if(isHighlighted)
        {
            if(transform.position.x == _piece.transform.position.x && transform.position.z == _piece.transform.position.z)
            {
                gameObject.GetComponent<Renderer>().material.color = col;
            }
            else
            {
                if(pieceCanBeMovedHere)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(0, 0.74f, 0);
                }
            }
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = col;
        }
    }
}

[System.Serializable]
public class BoardCoordinate
{
    [SerializeField]
    public enum ColumnEmun
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h,
        i
    }
    [SerializeField]
    int row;
    [SerializeField]
    ColumnEmun column;
    [SerializeField]
    Vector3 pos;

    public BoardCoordinate(int _column, int _row, Vector3 _pos)
    {
        row = _row;
        column = (ColumnEmun)_column;
        pos = _pos;
    }
    public int getRow()
    {
        return row;
    }
    public ColumnEmun getColumn()
    {
        return column;
    }
    public Vector3 getPos()
    {
        return pos;
    }
}
