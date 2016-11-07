using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script se usa para configurar desde Inspector 
/// edificios que ACEPTAN Cargo.
/// Ademas, Lleva los Cooldowns de como se producen items.
/// Hereda de CargoManagement
//////////////////////////////


[System.Serializable]
public class AcceptsCargo : CargoManagement
{
    public int moneyGained;

   

    public override void TruckOnPointListener(CardinalPoint cp, Cargo cargo, CargoBuilding building)
    {
        if (!direction.Contains(cp)) return; //Si no ha pasado por nuestro lado no hacemos nada
        if (cargo.cargo != CargoType) return; //Si est� vacio no hacemos nada
        if (myCargoSpriteReference == null) return; //FAILSAFE, en caso de que no haya referencia escrita por ManagesCargo
        if (myBuilding != building) return; //We check the building

        CargoDelivered CD = GameController.s.CargosDelivered.Find(x => x.type == cargo.cargo);
        if (CD == null)
        {
            Debug.LogError("UNLOAD: Cannot Find CargoType: " + cargo.ToString());
            return;
        }
        GameController.s.CargosDelivered[0].delivered += 1; //Cargo of type "none" (all cargo delivered)
        CD.delivered += 1;
        building.TruckGotUnloaded(cp, cargo);
        GameController.s.FloatingTextSpawn(building.TriggersTransform[(int)cp], "+1", enumColor.Green, "CargoSpritev2_Load", GameConfig.s.cargoColors[(int)cargo.cargo], 0f);
        cargo.cargo = CargoType.None; //Cargamos el vehiculo
        GameController.s.money += moneyGained;        
        GameController.s.FloatingTextSpawn(building.TriggersTransform[(int)cp], "+"+ moneyGained.ToString(), enumColor.Green, "Money", Color.black, 1.5f);



    }


    public override void UpdateMyCargoSprites()
    {
        foreach (CargoSprite cs in myCargoSpriteReference)
        {
            //cs.cargoType = CargoType;
            cs.produced = false;
            cs.moneyOnDelivery = moneyGained;
            if (GameConfig.s != null)
            {
                cs.SetColor(GameConfig.s.cargoColors[(int)CargoType], GameConfig.s.cargoTextColors[(int)CargoType]);
            }else
            {
                cs.SetColor(Color.white, Color.black);
            }
            
        }
    }

    #region Operators and Hashcode

    public static bool operator ==(AcceptsCargo x, AcceptsCargo y)
    {
        if (object.ReferenceEquals(x, null))
        {
            return object.ReferenceEquals(y, null);
        }
        return x.Equals(y);
    }
    public static bool operator !=(AcceptsCargo x, AcceptsCargo y)
    {
        return !(x == y);
    }
    public override bool Equals(object obj)
    {
        AcceptsCargo other = (AcceptsCargo)obj;
        return (this.CargoType == other.CargoType && this.direction == other.direction && this.moneyGained == other.moneyGained);
    }
    public override int GetHashCode()
    {
        int a = 0;
        foreach (CardinalPoint cp in direction)
        {
            a = int.Parse(a.ToString() + ((int)cp).ToString());
        }
        return int.Parse(((int)CargoType).ToString() + a.ToString() + moneyGained.ToString());
    }
    #endregion
}