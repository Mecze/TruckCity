using UnityEngine;
using System.Collections;
using System.Linq;

public class LoadUnload : MonoBehaviour {

    [SerializeField]
    bool Load = false;

    [SerializeField]
    SpriteRenderer mySprite;
    [SerializeField]
    Animator myAnimator;

    [SerializeField]
    CargoType _acceptedCargo;
    public CargoType AcceptedCargo
    {
        get
        {
            return _acceptedCargo;
        }

        set
        {
            _acceptedCargo = value;
            AcceptedCargoChanged(value);
        }
    }



    void AcceptedCargoChanged(CargoType value)
    {
        string s1 = "Unload";
        if (Load) s1 = "Load";
        string s = GameConfig.s.IMGPath + GameConfig.s.cargoSpriteCommonFileName + s1;
        mySprite.sprite = Resources.Load<Sprite>(s);
        mySprite.color = GameConfig.s.cargoColors[(int)AcceptedCargo];
    }

    void Start()
    {
        AcceptedCargoChanged(_acceptedCargo);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Cargo") return;
        if (Load)
        {
            TryToLoad(other.GetComponent<Cargo>());
        }
        else
        {
            TryToUnload(other.GetComponent<Cargo>());
        }

    }

    void TryToLoad(Cargo cargo)
    {
        if (cargo.Loaded) return; //No podemos cargar
        cargo.cargo = AcceptedCargo;
        GameController.s.FloatingTextSpawn(this.transform.position.x, this.transform.position.z, "Loaded", enumColor.Green);
    }
    void TryToUnload(Cargo cargo)
    {
        if (!cargo.Loaded) return; //est� vacio!
        if (cargo.cargo == AcceptedCargo)
        {//Descargamos
            CargoDelivered CD = GameController.s.CargosDelivered.Find(x => x.type == cargo.cargo);
            if (CD == null)
            {
                Debug.LogError("UNLOAD: Cannot Find CargoType: " + cargo.ToString());
                return;
            }
            CD.delivered += 1;
            cargo.cargo = CargoType.None;            
            GameController.s.FloatingTextSpawn(this.transform.position.x, this.transform.position.z, "Unload: +1 Score!", enumColor.Green);
        }


    }




}