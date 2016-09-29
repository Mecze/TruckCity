using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoGrid : MonoBehaviour {
    [Header("Auto (overrrides Manual)")]
    [SerializeField]
    bool auto=false;
    [SerializeField]
    int autoMaxCol = 2;

    [Header("ForceSquare (Manual or Auto)")]
    [SerializeField]
    bool forceSquare = true;

    [Header("Manual")]
    [SerializeField]
    int row;
    [SerializeField]
    int col;



    void Awake()
    {
        Adjust();
    }


    public void Adjust()
    {
        RectTransform rt = GetComponent<RectTransform>();
        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();
        int myCol = col;
        int myRow = row;
        if (auto)
        {
            int childs = transform.childCount;

            if (childs <= autoMaxCol){ myCol = 2; myRow = 1;}

            if (childs <= 1) { myCol = 1; myRow = 1; }
            if (childs > autoMaxCol)
            {
                myCol = autoMaxCol;
                myRow = Mathf.CeilToInt((float)childs / (float)autoMaxCol);
            }

        }
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = myCol;
        float cellXSize = rt.rect.width / myCol;
        float cellYSize = rt.rect.height / myRow;

        if (forceSquare)
        {//Cramps to the smallest
            if (cellXSize < cellYSize)
            {
                cellYSize = cellXSize;
            }else
            {
                cellXSize = cellYSize;
            }
        }

        glg.cellSize = new Vector2(cellXSize, cellYSize);




    }

}
