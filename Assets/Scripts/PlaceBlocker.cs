using UnityEngine;

public class PlaceBlocker : MonoBehaviour
{
    public bool hasBlocker;
    public Blocker blocker;
    private QuoridorController QC;

    public void setBlocker(Blocker B)
    {
        blocker = B;
    }

    public PlaceBlocker frontBlocker;
    public PlaceBlocker rightBlocker;
    public PlaceBlocker leftBlocker;
    public PlaceBlocker backBlocker;
    public PlaceBlocker frontRightBlocker;
    public PlaceBlocker frontLeftBlocker;
    public PlaceBlocker backRightBlocker;
    public PlaceBlocker backLeftBlocker;

    public BoardPiece BP1;
    public BoardPiece BP2;
    public BoardPiece BP3;
    public BoardPiece BP4;

    private bool hasCheckedBoardPieces;

    void Start()
    {
        hasBlocker = false;
        blocker = null;
        QC = GameObject.FindWithTag("GameController").GetComponent<QuoridorController>();
        FindNeighbors();
        BP1 = null;
        BP2 = null;
        BP3 = null;
        BP4 = null;
        hasCheckedBoardPieces = false;

    }
    void FindNeighbors()
    {
        RaycastHit hit;



        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            frontBlocker = hit.transform.gameObject.GetComponent<PlaceBlocker>();

        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit))
        {
            rightBlocker = hit.transform.gameObject.GetComponent<PlaceBlocker>();

        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit))
        {
            leftBlocker = hit.transform.gameObject.GetComponent<PlaceBlocker>();

        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit))
        {
            backBlocker = hit.transform.gameObject.GetComponent<PlaceBlocker>();

        }


        if (rightBlocker)
        {
            if (Physics.Raycast(rightBlocker.transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                frontRightBlocker = hit.transform.gameObject.GetComponent<PlaceBlocker>();

            }
        }





    }
    void Update()
    {





        if (hasBlocker)
        {
            isActive(false);
        }


        if (rightBlocker)
        {
            if (rightBlocker.hasBlocker)
            {
                if (rightBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal)
                {   
                    if (QC.actualBlocker)
                    {
                        if (QC.actualBlocker.orientation == Blocker.OrientationEnum.Horizontal)
                        {
                            isActive(false);
                        }
                        else
                        {
                            isActive(true);
                        }
                    }

                }
            }
        }

        if (leftBlocker)
        {
            if (leftBlocker.hasBlocker)
            {
                if (leftBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal)
                {
                    if (QC.actualBlocker)
                    {
                        if (QC.actualBlocker.orientation == Blocker.OrientationEnum.Horizontal)
                        {
                            isActive(false);
                        }
                        else
                        {
                            isActive(true);
                        }
                    }

                }
            }
        }

        if (frontBlocker)
        {
            if (frontBlocker.hasBlocker)
            {
                if (frontBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        if (QC.actualBlocker.orientation == Blocker.OrientationEnum.Vertical)
                        {
                            isActive(false);
                        }
                        else
                        {
                            isActive(true);
                        }
                    }

                }
            }
        }

        if (backBlocker)
        {
            if (backBlocker.hasBlocker)
            {
                if (backBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        if (QC.actualBlocker.orientation == Blocker.OrientationEnum.Vertical)
                        {
                            isActive(false);
                        }
                        else
                        {
                            isActive(true);
                        }
                    }

                }
            }
        }

        if (leftBlocker && backBlocker)
        {
            if (leftBlocker.hasBlocker && backBlocker.hasBlocker)
            {
                if (leftBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal &&
                    backBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        isActive(false);
                    }
                }
            }
        }

        if (rightBlocker && backBlocker)
        {
            if (rightBlocker.hasBlocker && backBlocker.hasBlocker)
            {
                if (rightBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal &&
                    backBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        isActive(false);
                    }
                }
            }
        }

        if (rightBlocker && frontBlocker)
        {
            if (rightBlocker.hasBlocker && frontBlocker.hasBlocker)
            {
                if (rightBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal &&
                    frontBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        isActive(false);
                    }
                }
            }
        }

        if (leftBlocker && frontBlocker)
        {
            if (leftBlocker.hasBlocker && frontBlocker.hasBlocker)
            {
                if (leftBlocker.blocker.orientation == Blocker.OrientationEnum.Horizontal &&
                    frontBlocker.blocker.orientation == Blocker.OrientationEnum.Vertical)
                {
                    if (QC.actualBlocker)
                    {
                        isActive(false);
                    }
                }
            }
        }


        

        if (hasBlocker)
        {
            isActive(false);
        }


        

    }

    void isActive(bool a)
    {
        GetComponent<Renderer>().enabled = a;
        GetComponent<SphereCollider>().enabled = a;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Board_Segment")
        {
            BP1 = col.gameObject.GetComponent<BoardPiece>();
            BP2 = col.gameObject.GetComponent<BoardPiece>().RightBoard;
            BP3 = col.gameObject.GetComponent<BoardPiece>().diagonalBoard;
            BP4 = col.gameObject.GetComponent<BoardPiece>().BackBoard;
            hasCheckedBoardPieces = true;
        }


    }

}
