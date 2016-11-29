using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum CardinalPoint { None = 0, N = 1, E = 2, W = 3, S = 4 }
[System.Serializable]
public enum RoadDirection { EW, NE, NEWS, NS, NW, SE, SW }
public enum RoadRotationGreen { NE = 0, SE = 1, SW = 2, NW = 3 }
public enum RoadRotationBlue { EW = 0, NS = 1 }
public enum RoadPositionPurple { None = 0, N = 1, E = 2, S = 3, W = 4 }

#region extensions

public static class CardinalPointExtensions
{
    public static List<CardinalPoint> GetAll(this CardinalPoint thisCP)
    {
        List<CardinalPoint> cps = ((CardinalPoint[])Enum.GetValues(typeof(CardinalPoint))).ToList();
        //rds.Remove(RoadDirection.NEWS);
        return cps;
    }
    public static CardinalPoint Reverse(this CardinalPoint thisCardinalPoint)
    {
        switch (thisCardinalPoint)
        {
            case CardinalPoint.None:
                return CardinalPoint.None;
            case CardinalPoint.N:
                return CardinalPoint.S;
            case CardinalPoint.E:
                return CardinalPoint.W;
            case CardinalPoint.W:
                return CardinalPoint.E;
            case CardinalPoint.S:
                return CardinalPoint.N;
            default:
                return CardinalPoint.None;
        }
    }
    public static CardinalPoint TurnRight(this CardinalPoint thisCardinalPoint)
    {
        CardinalPoint r = CardinalPoint.None;
        switch (thisCardinalPoint)
        {
            case CardinalPoint.None:
                break;
            case CardinalPoint.N:
                r = CardinalPoint.E;
                break;
            case CardinalPoint.E:
                r = CardinalPoint.S;
                break;
            case CardinalPoint.W:
                r = CardinalPoint.N;
                break;
            case CardinalPoint.S:
                r = CardinalPoint.W;
                break;
            default:
                break;
        }
        return r;  
    }
    public static CardinalPoint TurnRight(this CardinalPoint thisCardinalPoint, int numberOfTimes)
    {
        CardinalPoint r = thisCardinalPoint;
        for (int i = 0; i < numberOfTimes; i++)
        {
            r = r.TurnRight();
        }
        return r;

    }
    public static CardinalPoint TurnLeft(this CardinalPoint thisCardinalPoint)
    {
        CardinalPoint r = CardinalPoint.None;
        switch (thisCardinalPoint)
        {
            case CardinalPoint.None:
                break;
            case CardinalPoint.N:
                r = CardinalPoint.W;
                break;
            case CardinalPoint.E:
                r = CardinalPoint.N;
                break;
            case CardinalPoint.W:
                r = CardinalPoint.S;
                break;
            case CardinalPoint.S:
                r = CardinalPoint.E;
                break;
            default:
                break;
        }
        return r;
    }
    public static CardinalPoint TurnLeft(this CardinalPoint thisCardinalPoint, int numberOfTimes)
    {
        CardinalPoint r = thisCardinalPoint;
        for (int i = 0; i < numberOfTimes; i++)
        {
            r = r.TurnLeft();
        }
        return r;

    }
    public static string ToString(this CardinalPoint thisCardinalPoint, bool Verbose)
    {
        switch (thisCardinalPoint)
        {
            case CardinalPoint.None:
                return "None";                
            case CardinalPoint.N:
                if (Verbose) return "North";
                if (!Verbose) return "N";
                break;
            case CardinalPoint.E:
                if (Verbose) return "East";
                if (!Verbose) return "E";
                break;
            case CardinalPoint.W:
                if (Verbose) return "West";
                if (!Verbose) return "W";
                break;
            case CardinalPoint.S:
                if (Verbose) return "South";
                if (!Verbose) return "S";
                break;
            default:
                return "None";
         
        }
        return "None";

    }
    /// <summary>
    /// Compose a new RoadDirection from 2 CardinalPoints. (This and the parameter)
    /// </summary>
    /// <param name="thisCardinalPoint"></param>
    /// <param name="OtherCardinalPoint"></param>
    /// <returns></returns>
    public static RoadDirection ComposeRoadDirection(this CardinalPoint thisCardinalPoint, CardinalPoint OtherCardinalPoint)
    {
        List<RoadDirection> result = new List<RoadDirection>();
        RoadDirection dummy = RoadDirection.NEWS;
        result = dummy.GetAll().FindAll(x => x.HasDirection(thisCardinalPoint) && x.HasDirection(OtherCardinalPoint));
        result.Remove(RoadDirection.NEWS);
        return result[0];
    }

}

public static class RoadDirectionExtensions
{
    public const string EW = "EW";
    public const string NS = "NS";
    public const string SE = "SE";
    public const string SW = "SW";
    public const string NW = "NW";
    public const string NE = "NE";
    public const string NEWS = "NEWS";

    /// <summary>
    /// Gets the Other exit, besides the input.
    /// on an NS road with input N it will return S. on an NW road with input W it will return N.
    /// </summary>
    /// <param name="road"></param>
    /// <param name="KnownCardinalPoint"></param>
    /// <returns></returns>
    public static CardinalPoint GetOther(this RoadDirection road,CardinalPoint KnownCardinalPoint)
    {
        List<CardinalPoint> list = KnownCardinalPoint.GetAll();
        list.Remove(CardinalPoint.None);
        list.Remove(KnownCardinalPoint);
        List<CardinalPoint> list2 = new List<CardinalPoint>(list);
        for (int i = 0; i < list.Count; i++)
        {
            if (!road.HasDirection(list[i]))
            {
                list2.Remove(list[i]);
            }

        }
        if (list.Count == 0) return CardinalPoint.None;
        return list2[0];

    }

    public static string ToString(this RoadDirection road, int i)
    {
        string s;
        switch (road)
        {
            case RoadDirection.EW:
                s = EW;
                break;
            case RoadDirection.NE:
                s = NE;
                break;
            case RoadDirection.NEWS:
                s = NEWS;
                break;
            case RoadDirection.NS:
                s = NS;
                break;
            case RoadDirection.NW:
                s = NW;
                break;
            case RoadDirection.SE:
                s = SE;
                break;
            case RoadDirection.SW:
                s = SW;
                break;
            default:
                s = "";
                break;
        }
        return s;
    }


    /// <summary>
    /// Compara dos "RoadDirection" Devuelve las similitudes en array de Truckdirection
    /// mediante el RETURN.
    /// Devuelve las diferencias en array de CardinalPoint mediante out.
    /// Ambas arrays varian entre 0 y 2 elementos
    /// </summary>
    /// <param name="thisRoad">selfroad</param>
    /// <param name="otherRoad">La otra carretera a comparar</param>
    /// <param name="diferences">OUT de array de TruckDirection</param>
    /// <param name="focusDifferencesOnOtherRoad">Si es false, el out "differences" devuelve=> ¿Que tiene esta carretera que no tenga "otherRoad"?. Si es true => ¿Que tiene la otra carretera que no tenga esta?</param>
    /// <returns>array de TruckDirection</returns>
    public static CardinalPoint[] Compare(this RoadDirection thisRoad, RoadDirection otherRoad, out CardinalPoint[] diferences, bool focusDifferencesOnOtherRoad = false)
    {
        bool b = focusDifferencesOnOtherRoad; //alias
        //Arrays temporales a llenar
        CardinalPoint[] diff = new CardinalPoint[2];
        CardinalPoint[] result = new CardinalPoint[2];
        //contadores para las arrays
        int i = 0; //result
        int e = 0; //diff

        //MainLoop de 1 a 4. el ENUM TruckDirection puede ser casteado a INT (y viceversa)
        //Nota: Enum Truckdirection. None = 0, N = 1, E = 2, W = 3, S = 4.
        for (int index = 1; index <= 4; index++)
        {
            //Sacamos la dirección que vamos a comprobar en este loop:
            CardinalPoint dir = (CardinalPoint)index; //Cast de int a TruckDirection

            //La comprobamos!

            //Si ambos tienen la direción, guaramos la similitud y se acaba el loop
            if (thisRoad.HasDirection(dir) && otherRoad.HasDirection(dir))
            {
                result[i] = dir;
                i++;
            }
            else //Si no comprobamos si alguno de los dos tiene esa dirección
            {
                //Nota: b es el focus. 
                //Sobre Que carretera queremos devolver las diferencias
                // !b = esta carretera
                // b = la otra carretera
                if (thisRoad.HasDirection(dir) && !b)
                {
                    diff[e] = dir;
                    e++;
                }
                if (otherRoad.HasDirection(dir) && b)
                {
                    diff[e] = dir;
                    e++;
                }
            }
        }
        //End of Main Loop

        //reconstruimos ambas arrays y devolvemos.
        CardinalPoint[] resultd = new CardinalPoint[result.Length];
        for (int x = 0; x < result.Length; x++)
        {
            resultd[x] = result[x];
        }

        diferences = new CardinalPoint[diff.Length];
        for (int y = 0; y < diff.Length; y++)
        {
            diferences[y] = diff[y];
        }
        return resultd;
    }


    /// <summary>
    /// Consulta si esta carretera tiene la salida indicada
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static bool HasDirection(this RoadDirection roadDirection, CardinalPoint direction)
    {
        bool r = false;
        switch (direction)
        {
            case CardinalPoint.N:
                r = roadDirection.HasN();
                break;
            case CardinalPoint.E:
                r = roadDirection.HasE();
                break;
            case CardinalPoint.W:
                r = roadDirection.HasW();
                break;
            case CardinalPoint.S:
                r = roadDirection.HasS();
                break;
            case CardinalPoint.None:
                r = true;
                break;
            default:
                r = false;
                break;
        }
        return r;
    }

    public static bool HasN(this RoadDirection roadDirection)
    {
        bool r = false;
        switch (roadDirection)
        {
            case RoadDirection.NE:
                r = true;
                break;
            case RoadDirection.NEWS:
                r = true;
                break;
            case RoadDirection.NS:
                r = true;
                break;
            case RoadDirection.NW:
                r = true;
                break;

            default:
                r = false;
                break;
        }
        return r;

    }
    public static bool HasW(this RoadDirection roadDirection)
    {
        bool r = false;
        switch (roadDirection)
        {
            case RoadDirection.EW:
                r = true;
                break;

            case RoadDirection.NEWS:
                r = true;
                break;

            case RoadDirection.NW:
                r = true;
                break;

            case RoadDirection.SW:
                r = true;
                break;

            default:
                r = false;
                break;
        }
        return r;
    }
    public static bool HasE(this RoadDirection roadDirection)
    {
        bool r = false;

        switch (roadDirection)
        {
            case RoadDirection.EW:
                r = true;
                break;
            case RoadDirection.NE:
                r = true;

                break;
            case RoadDirection.NEWS:
                r = true;
                break;

            case RoadDirection.SE:
                r = true;
                break;

            default:
                r = false;
                break;
        }
        return r;
    }
    public static bool HasS(this RoadDirection roadDirection)
    {
        bool r = false;

        switch (roadDirection)
        {
            case RoadDirection.NEWS:
                r = true;
                break;
            case RoadDirection.NS:
                r = true;
                break;
            case RoadDirection.SE:
                r = true;
                break;
            case RoadDirection.SW:
                r = true;
                break;
            default:
                r = false;
                break;
        }
        return r;
    }
    /// <summary>
    /// Devuelve todas las posibilidades del ENUM ROADDIRECTION en una LISTA
    /// </summary>
    /// <param name="thisRD"></param>
    /// <returns></returns>
    public static List<RoadDirection> GetAll(this RoadDirection thisRD)
    {
        List<RoadDirection> rds = ((RoadDirection[])Enum.GetValues(typeof(RoadDirection))).ToList();
        //rds.Remove(RoadDirection.NEWS);
        return rds;
    }
    /// <summary>
    /// Devuelve una lista de CardinalPoints con todas las salidas del RoadDirection actual
    /// Ejemplo: RoadDirection= SE  Devolveria una Lista de "S" y "E", como Cardinal Points.
    /// </summary>
    /// <param name="thisRD"></param>
    /// <returns></returns>
    public static List<CardinalPoint> GetExits(this RoadDirection thisRD)
    {
        List<CardinalPoint> r = ((CardinalPoint[])Enum.GetValues(typeof(CardinalPoint))).ToList();
        r.Remove(CardinalPoint.None);
        List<CardinalPoint> r2 = new List<CardinalPoint>(r);        
        for (int i = 0; i < r.Count; i++)
        {
            if (!(thisRD.HasDirection(r[i]))){
                r2.Remove(r[i]);
            }
        }

        return r2;
    }
    /// <summary>
    /// Gets a list of CardinalPoints that are NOT the exits on this road.
    /// </summary>
    /// <param name="thisRD"></param>
    /// <returns></returns>
    public static List<CardinalPoint> GetNoExits(this RoadDirection thisRD)
    {
        List<CardinalPoint> r = ((CardinalPoint[])Enum.GetValues(typeof(CardinalPoint))).ToList();
        r.Remove(CardinalPoint.None);
        List<CardinalPoint> r2 = new List<CardinalPoint>(r);
        for (int i = 0; i < r.Count; i++)
        {
            if ((thisRD.HasDirection(r[i])))
            {
                r2.Remove(r[i]);
            }
        }
        return r2;

    }


    /// <summary>
    /// Devuelve el CardinalPoint hacia el que un camion sobre ESTA carretera dada la dirección del camion    /// 
    /// </summary>
    /// <param name="thisRD"></param>
    /// <param name="EntryDirection">La dirección del camion</param>
    /// <returns></returns>
    public static CardinalPoint ResolveRotation(this RoadDirection thisRD, CardinalPoint EntryDirection, CardinalPoint LastTruckDirection, out bool ForceCheckNextRoad)
    {
        ForceCheckNextRoad = false;
        List<CardinalPoint> exits = thisRD.GetExits();
        if (exits.Count < 2) return EntryDirection;

        //Si se trata de una carretera RECTA
        if (exits[0] == exits[1].Reverse() && exits.Count == 2)
        {
            if (exits.Exists(x => x == EntryDirection))
            {
                return EntryDirection;
            }else
            {
                ForceCheckNextRoad = true;
                return LastTruckDirection;
            }
        }

        //Si se trata de carreteras NO RECTOS ---

        //Si exite el reverse de la entrada, devuelve la otra salida
        if (exits.Exists(x => x == EntryDirection.Reverse()))
        {
            //Busca la entrada por la que entró el camion a la carretera.
            //Si el camion va hacia el norte: entro por el sur. (REVERSE). Si lleva dirección ESTE entró por el OESTE
            //Esta opción la quitamos de la LISTA
            //La opción que queda debe ser la salida
            exits.Remove(EntryDirection.Reverse());

            return exits[0];
        }else
        {//Si no exite el contrario de la entrada (es decir en que dirección entró)
            //Quiere decir que seguramente el giro ya habrá ocurrido y esto haya sido lanzado por evento de comprobación de giro.
            //así que devolvemos la dirección actual
            return EntryDirection;

        }

        
    }

    public static RoadDirection TurnRight (this RoadDirection thisRD, int times = 0)
    {
        List<CardinalPoint> exits = thisRD.GetExits();
        for (int i = 0; i < exits.Count; i++)
        {
            exits[i] = exits[i].TurnRight(times);
        }
        if (exits.Count != 2) { Debug.LogError("CouldntRotate this RD, not enought elements on it!"); return thisRD; }
        return exits[0].ComposeRoadDirection(exits[1]);
    }
    public static RoadDirection TurnLeft(this RoadDirection thisRD, int times = 0)
    {
        List<CardinalPoint> exits = thisRD.GetExits();
        for (int i = 0; i < exits.Count; i++)
        {
            exits[i] = exits[i].TurnLeft(times);
        }
        if (exits.Count != 2) { Debug.LogError("CouldntRotate this RD, not enought elements on it!"); return thisRD; }
        return exits[0].ComposeRoadDirection(exits[1]);
    }


}

public static class RoadRotationGreenExtensions
{

    public static RoadDirection toRoadDirection(this RoadRotationGreen greenRoadDirection)
    {
        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadDirection.NE;
            case RoadRotationGreen.SE:
                return RoadDirection.SE;
            case RoadRotationGreen.SW:
                return RoadDirection.SW;
            case RoadRotationGreen.NW:
                return RoadDirection.NW;
            default:
                return RoadDirection.NE;
        }
    }
    public static RoadRotationGreen Next(this RoadRotationGreen greenRoadDirection)
    {

        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadRotationGreen.SE;
            case RoadRotationGreen.SE:
                return RoadRotationGreen.SW;
            case RoadRotationGreen.SW:
                return RoadRotationGreen.NW;
            case RoadRotationGreen.NW:
                return RoadRotationGreen.NE;
            default:
                return RoadRotationGreen.NE;
        }
    }
    public static RoadRotationBlue GreenToBlue(this RoadRotationGreen greenRoadDirection)
    {
        switch (greenRoadDirection)
        {
            case RoadRotationGreen.NE:
                return RoadRotationBlue.EW;
            case RoadRotationGreen.SE:
                return RoadRotationBlue.NS;
            case RoadRotationGreen.SW:
                return RoadRotationBlue.EW;
            case RoadRotationGreen.NW:
                return RoadRotationBlue.NS;
            default:
                return RoadRotationBlue.EW;
        }
    }
}

public static class RoadRotationBlueExtensions
{

    public static RoadDirection toRoadDirection(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadDirection.EW;
            case RoadRotationBlue.NS:
                return RoadDirection.NS;
            default:
                return RoadDirection.EW;
        }
    }
    public static RoadRotationBlue Next(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadRotationBlue.NS;

            case RoadRotationBlue.NS:
                return RoadRotationBlue.EW;

            default:
                return RoadRotationBlue.EW;
        }
    }
    public static RoadRotationGreen BlueToGreen(this RoadRotationBlue blueRoadDirection)
    {
        switch (blueRoadDirection)
        {
            case RoadRotationBlue.EW:
                return RoadRotationGreen.SE;

            case RoadRotationBlue.NS:
                return RoadRotationGreen.NW;

            default:
                return RoadRotationGreen.NE;

        }
    }

}

public static class RoadRotationPurpleExtensions
{
    /// <summary>
    /// Calculates Next Direction (Currently deprecate?)
    /// </summary>
    /// <param name="purpleRoadcurrent"></param>
    /// <param name="purpleRoadAnchor"></param>
    /// <param name="Direction"></param>
    /// <returns></returns>
    public static RoadPositionPurple NextPurpleCardinalPoint(this RoadPositionPurple purpleRoadcurrent, RoadPositionPurple purpleRoadAnchor, ref int Direction)
    {
        if (Direction != -1 && Direction != 1) return RoadPositionPurple.None;
        int currentPos = (int)purpleRoadcurrent;
        int anchorPos = (int)purpleRoadAnchor;
        currentPos = currentPos.AddwithBoundaries(Direction, 1, 4);
        if (currentPos == anchorPos)
        {
            //We bounce
            Direction = -Direction;
            //Now we advance 2 (on the other direction)
            currentPos = currentPos.AddwithBoundaries(Direction * 2, 1, 4);
        }
        return (RoadPositionPurple)currentPos;
    }
    /// <summary>
    /// Compose a RoadDirection from 2 RoadPositionPurple (this, and parameter)
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static RoadDirection ComposeRoadDirection(this RoadPositionPurple A, RoadPositionPurple B)
    {
        List<RoadDirection> result = new List<RoadDirection>();
        RoadDirection dummy = RoadDirection.NEWS;
        result = dummy.GetAll().FindAll(x => x.HasDirection(A.ToCardinalPoint()) && x.HasDirection(B.ToCardinalPoint()));
        result.Remove(RoadDirection.NEWS);
        return result[0];
    }

    /// <summary>
    /// Since Order for RoadDirectionPurple (90º turns) and order Cardinal Point (NEWS) are different we change it wit this
    /// </summary>
    /// <param name="roadPositionPurple"></param>
    /// <returns></returns>
    public static CardinalPoint ToCardinalPoint(this RoadPositionPurple roadPositionPurple)
    {
        switch (roadPositionPurple)
        {
            case RoadPositionPurple.None:
                return CardinalPoint.None;
            case RoadPositionPurple.N:
                return CardinalPoint.N;
            case RoadPositionPurple.E:
                return CardinalPoint.E;
            case RoadPositionPurple.S:
                return CardinalPoint.S;
            case RoadPositionPurple.W:
                return CardinalPoint.W;
            default:
                return CardinalPoint.None;
                //break;
        }

    }

    public static RoadPositionPurple TurnRight(this RoadPositionPurple roadPositionPurple)
    {
        switch (roadPositionPurple)
        {
            case RoadPositionPurple.N:
                return RoadPositionPurple.E;                
            case RoadPositionPurple.E:
                return RoadPositionPurple.S;                
            case RoadPositionPurple.S:
                return RoadPositionPurple.W;                
            case RoadPositionPurple.W:
                return RoadPositionPurple.N;                         
        }
        return roadPositionPurple;

    }
    public static RoadPositionPurple TurnRight(this RoadPositionPurple roadPositionPurple, int times)
    {
        RoadPositionPurple RPP = roadPositionPurple;
        for (int i = 0; i < times; i++)
        {
            RPP = RPP.TurnRight();
        }
        return RPP;
    }
    public static RoadPositionPurple TurnLeft(this RoadPositionPurple roadPositionPurple)
    {
        switch (roadPositionPurple)
        {
            case RoadPositionPurple.N:
                return RoadPositionPurple.W;
            case RoadPositionPurple.E:
                return RoadPositionPurple.N;
            case RoadPositionPurple.S:
                return RoadPositionPurple.E;
            case RoadPositionPurple.W:
                return RoadPositionPurple.S;
        }
        return roadPositionPurple;

    }
    public static RoadPositionPurple TurnLeft(this RoadPositionPurple roadPositionPurple, int times)
    {
        RoadPositionPurple RPP = roadPositionPurple;
        for (int i = 0; i < times; i++)
        {
            RPP = RPP.TurnLeft();
        }
        return RPP;
    }
}
#endregion

