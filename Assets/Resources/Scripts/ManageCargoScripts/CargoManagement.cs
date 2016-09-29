using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// Este Script es el Padre de otros Scprits (ProducesCargo, AcceptsCargo)
//////////////////////////////
public abstract class CargoManagement
{
    public CargoType CargoType;
    public List<CardinalPoint> direction;
    public List<CargoSprite> myCargoSpriteReference;
    public CargoBuilding myBuilding;

    public abstract void TruckOnPointListener(CardinalPoint cp, Cargo cargo, CargoBuilding building);

    public abstract void UpdateMyCargoSprites();






}
