using UnityEngine;
using System.Collections;

public class PauseButtonTweenerAspectRatioAdapter : MonoBehaviour {

	
    void Start()
    {
        TweenPosition myPosTween = GetComponent<TweenPosition>();
        if (myPosTween == null) return;      
        myPosTween.from = transform.localPosition;
        Vector3 pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        UISprite mySprite = GetComponent<UISprite>();
        if (mySprite == null) return;
        pos.x = pos.x - mySprite.localSize.x;
        myPosTween.to = pos;

        mySprite.topAnchor.absolute = 0;
        mySprite.bottomAnchor.absolute = 0;
    }
}




