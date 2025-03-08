using UnityEngine;


public class Blocker : MonoBehaviour
{
    public bool isBeingDragged;
    public bool hasBeenPlaced;

    [SerializeField]
    public enum OrientationEnum
    {
        Horizontal,
        Vertical
    }

    public OrientationEnum orientation;

    private void Awake()
    {
        isBeingDragged = false;
        hasBeenPlaced = false;
        orientation = OrientationEnum.Horizontal;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(isBeingDragged)
        {
            AttachBlockerToCursor();
            setOrientation();
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(!hasBeenPlaced)
            {
                Destroy(this);
                Destroy(this.gameObject);
            }
        }
    }
    public void setOrientation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (orientation == OrientationEnum.Horizontal)
            {
                orientation = OrientationEnum.Vertical;
            }
            else
            {
                orientation = OrientationEnum.Horizontal;
            }
        }


        if (orientation == OrientationEnum.Horizontal)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (orientation == OrientationEnum.Vertical)
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    public void AttachBlockerToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(10);

        transform.position = rayPoint;
    }


    public void PlaceBlockerOnBoard(GameObject placeHere)
    {
        isBeingDragged = false;

        transform.position = placeHere.transform.position + new Vector3(0, transform.localScale.y / 2, 0);

        GetComponent<BoxCollider>().enabled = true;

        hasBeenPlaced = true;
    }
}
