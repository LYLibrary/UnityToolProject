using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMenuTe : MonoBehaviour
{
    public LoopCircleGridsCounterClockwise loop;
    public GameObject toolPanel;
    

    public bool enableTestFunction = false;

    private void Start()
    {
        if (enableTestFunction)
        {
            CreateItem(6);
        }
    }

    public List<GameObject> CreateItem(int count)
    {
        if (count > 0)
        {
            loop.RemoveAllCell();
            loop.AngleStep.z = 360 / count;
            //loop.CellCountPerPage = 

            List<GameObject> allItem = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                allItem.Add(loop.AddCell(i + 1));
            }
            return allItem;
        }
        return null;
    }

    public void TabToolPanelActive()
    {
        if (toolPanel!=null)
        {
            toolPanel.SetActive(!toolPanel.activeSelf);
        }
    }


}


