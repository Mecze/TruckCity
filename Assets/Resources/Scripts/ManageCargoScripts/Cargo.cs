using UnityEngine;
using System.Collections;



public class Cargo : MonoBehaviour {
    [SerializeField]
    TruckEntity myTruck;
    
    #region Loaded BOOL Property
    [Header("Cargo")]
    /// <summary>
    /// Indica si el vehiculo está cargado
    /// </summary>
    [SerializeField]
    bool _loaded = false;
    /// <summary>
    /// Indica si el vehiculo está cargado
    /// </summary>
    public bool Loaded
    {
        get
        {
            return _loaded;
        }

        protected set
        {
            _loaded = value;
        }
    }
    #endregion


    #region CARGO Property
    [SerializeField]
    CargoType _cargo;
    /// <summary>
    /// La carga actual del vehiculo
    /// Cambiar esto actualiza el material tambien.
    /// </summary>
    public CargoType cargo
    {
        get
        {
            return _cargo;
        }

        set
        {
            _cargo = value;
            ChangeLoad(_cargo);
        }
    }

    #endregion

    




    void ChangeLoad(CargoType value)
    {
        if (value == CargoType.None)
        {
            _loaded = false;
        }else
        {
            _loaded = true;
            myTruck.PlayHonk();
        }
        Renderer rend = GetComponent<Renderer>();
        if (GameConfig.s != null)
        {
            rend.material = Resources.Load<Material>(GameConfig.s.materialsPath + GameConfig.s.cargoMaterialFileName + GameConfig.s.cargoSpriteFileName[(int)cargo]);
        }
        else
        {
            rend.material = Resources.Load<Material>("Materials\\Cargo_None");
            
        }
        
        

    }

    void Start()
    {
        ChangeLoad(cargo);
    }
}