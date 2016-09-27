using UnityEngine;
using System.Collections;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script se usa para configurar desde Inspector 
/// edificios que producen ITEMS.
/// Ademas, Lleva los Cooldowns de como se producen items.
/// Hereda de CargoManagement
//////////////////////////////

public enum TimeToProduce { ThreeSeconds = 3, SixSeconds = 6, TwelveSeconds = 12, TwentyFourSeconds = 24 }

[System.Serializable]
public class ProducesCargo : CargoManagement
{
    //All of this goes to Inspector for configuration when 
    //the level is created    
    public int maxProduced;
    public int startingAmount;
    public TimeToProduce timeToProduce;
    /*
    public event OnProducedDelegate OnProduce;
    public event OnLoadCargoDelegate OnLoadCargo;
    */

    #region properties
    int _amountOfItems;
    /// <summary>
    /// Inner amount of items, Communicates Changes to its asociated sprites
    /// </summary>
    public int amountOfItems
    {
        get
        {
            return _amountOfItems;
        }

        set
        {
            _amountOfItems = value;
            if (myCargoSpriteReference != null) foreach (CargoSprite cs in myCargoSpriteReference) cs.amountOfItems = _amountOfItems;
        }
    }

    float _timer;
    /// <summary>
    /// InnerTimer, Comunicates changes to its asociated sprites
    /// </summary>
    public float timer
    {
        get
        {
            return _timer;
        }

        set
        {
            _timer = value;
            if (myCargoSpriteReference != null) foreach (CargoSprite cs in myCargoSpriteReference) cs.timer = _timer;
        }
    }

    #endregion

    #region Listener: Truck on Station Event    
    public override void TruckOnPointListener(CardinalPoint cp, Cargo cargo, CargoBuilding building)
    {
        if (!direction.Contains(cp))
        {
            return; //Si no ha pasado por nuestro lado no hacemos nada
        }
        if (cargo.cargo != CargoType.None)
        {
            return; //Si est� lleno no hacemos nada
        }
        if (myCargoSpriteReference == null)
        {
            return; //FAILSAFE, en caso de que no haya referencia escrita por ManagesCargo
        }
        if (amountOfItems == 0)
        {
            return; //Si no hay items disponibles no hacemos nada
        }
        if (myBuilding != building)
        {
            return;// Check the building
        }

        cargo.cargo = CargoType; //Cargamos el vehiculo
        bool startcd = false;
        if (amountOfItems == maxProduced) startcd = true;

        amountOfItems--; //Restamos uno a la cantidad de items que tenemos 
        if (startcd) StartCooldown();
    }
    #endregion

    #region update CargoSprites
    /// <summary>
    /// Actualiza los sprites asociados
    /// </summary>
    public override void UpdateMyCargoSprites()
    {
        foreach (CargoSprite cs in myCargoSpriteReference)
        {
            cs.maxAmountOfItems = maxProduced;
            cs.timeToProduce = (int)timeToProduce;
            //cs.cargoType = CargoType;
            cs.produced = true;
            cs.SetColor(GameConfig.s.cargoColors[(int)CargoType], GameConfig.s.cargoTextColors[(int)CargoType]);
        }
    }

    #endregion




    #region TIMER LOGIC
    public void Initialize()
    {
        if (amountOfItems < maxProduced)
        {
            TimeController.OnStartClock += CooldownTick;
            amountOfItems = amountOfItems;
        }
    }

    void StartCooldown()
    {
        int delta = 1;
        if (TimeController.s.decrement) delta = -2;
        TimeController.s.timer.AddAction(((int)TimeController.s.currentTime) + delta, CooldownTick);
    }


    void CooldownTick()
    {
        //Debug.Log(TimeController.s.currentTime.ToString() + " CooldownTick: " + timer.ToString());
        timer = timer + 1f;
        if (timer > (int)timeToProduce)
        {
            timer = 0f;
            amountOfItems++;

        }
        if (amountOfItems != maxProduced)
        {
            StartCooldown();
        }
    }
    #endregion







    #region Operators and HashCode
    public static bool operator ==(ProducesCargo x, ProducesCargo y)
    {
        if (object.ReferenceEquals(x, null))
        {
            return object.ReferenceEquals(y, null);
        }
        return x.Equals(y);
    }
    public static bool operator !=(ProducesCargo x, ProducesCargo y)
    {
        return !(x == y);
    }
    public override bool Equals(object obj)
    {
        ProducesCargo other = (ProducesCargo)obj;
        return (this.CargoType == other.CargoType &&
            this.maxProduced == other.maxProduced &&
            this.direction == other.direction &&
            this.startingAmount == other.startingAmount &&
            this.timeToProduce == other.timeToProduce);
    }
    public override int GetHashCode()
    {
        int a = 0;
        foreach (CardinalPoint cp in direction)
        {
            a = int.Parse(a.ToString() + ((int)cp).ToString());
        }
        return int.Parse(((int)CargoType).ToString() + maxProduced.ToString() + startingAmount.ToString() + (((int)timeToProduce).ToString() + a.ToString()));
    }
    #endregion

}

