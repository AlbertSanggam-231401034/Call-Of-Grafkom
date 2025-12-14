using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [Header("Walls")]
    public GameObject wallTop;
    public GameObject wallBottom;
    public GameObject wallLeft;
    public GameObject wallRight;

    public bool IsVisited { get; private set; } = false;

    public void Visit()
    {
        IsVisited = true;
    }

    public void ClearWall(int wallID)
    {
        // 1=Top, 2=Bottom, 3=Left, 4=Right
        switch (wallID)
        {
            case 1: if (wallTop != null) wallTop.SetActive(false); break;
            case 2: if (wallBottom != null) wallBottom.SetActive(false); break;
            case 3: if (wallLeft != null) wallLeft.SetActive(false); break;
            case 4: if (wallRight != null) wallRight.SetActive(false); break;
        }
    }
}