using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PicaVoxel;
using System.Linq;

public enum PlayPause { Stop = 0, Play = 1, Pause = 2 }

[RequireComponent(typeof(RoadEnt))]
public class TrafficRoad : MonoBehaviour {
    [SerializeField]
    List<Volume> listOfLights;

    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    TrafficLightColl ENCollider; //East or North collider

    [SerializeField]
    TrafficLightColl WSCollider; //West or South collider

    PlayPause _status = PlayPause.Play;

    public PlayPause status
    {
        get
        {
            return _status;
        }

        set
        {
            if (_status != value)
            {
                _status = value;
                switch (_status)
                {                    
                    case PlayPause.Play:
                        sprite.sprite = (Sprite)Resources.Load<Sprite>("IMG\\PlayOverlay");
                        ENCollider.Green = true;
                        WSCollider.Green = true;
                        listOfLights.ForEach(x => x.SetFrame(0));
                        break;
                    case PlayPause.Pause:
                        sprite.sprite = (Sprite)Resources.Load<Sprite>("IMG\\PauseOverlay");
                        ENCollider.Green = false;
                        WSCollider.Green = false;
                        listOfLights.ForEach(x => x.SetFrame(1));
                        break;                 
                }


            }
            

        }
    }

    void Awake()
    {

    }


    void OnPress(bool press)
    {
        if (!press) return;
        switch (_status)
        {
            case PlayPause.Stop:
                status = PlayPause.Play;
                break;
            case PlayPause.Play:
                status = PlayPause.Pause;
                break;
            case PlayPause.Pause:
                status = PlayPause.Play;
                break;
            default:
                break;
        }


    }


}
