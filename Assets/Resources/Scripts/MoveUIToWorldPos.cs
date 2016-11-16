using UnityEngine;
using System.Collections;


public class MoveUIToWorldPos : MonoBehaviour {

    [SerializeField]
    Transform WorldPosObject;

    void OnEnable()
    {
        Reposition();
    }

    public void Reposition()
    {
        if (WorldPosObject == null) return;
        Vector3 pos = NGUIMath.WorldToLocalPoint(WorldPosObject.position, Camera.main, GameController.s.UICamera, this.transform);
        /*
        pos.y = pos.y + (GetComponent<UIWidget>().localSize.y);// / 2);
        pos.x = pos.x + (GetComponent<UIWidget>().localSize.x);// / 2);
        */
        this.transform.localPosition = pos;
    }

}
