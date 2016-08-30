using UnityEngine;
using System.Collections;

[System.Serializable]
public class Vector3Int  {
    [SerializeField]
    int _x;

    public int x
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value;
        }
    }
    [SerializeField]
    int _y;

    public int y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value;
        }
    }
    [SerializeField]
    int _z;

    public int z
    {
        get
        {
            return _z;
        }

        set
        {            
            _z = value;
        }
    }

    #region Constructor

    public Vector3Int (int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;

    }

    public Vector3Int()
    {
        _x = 0;
        _y = 0;
        _z = 0;
    }


    #endregion

    #region Overrides

    public override int GetHashCode()
    {
        return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
    }
    public override bool Equals(object obj)
    {
        Vector3Int a = (Vector3Int)obj;
        return ((_x == a.x) && (_y == a.y) && (_z == a.z));
    }


    #endregion

    /// <summary>
    /// Si este Vector3Int y "other" no son adyacentes devuelve "TruckDirection.None"
    /// Si lo son, devuelve la dirección (TruckDirection.N, E, W, S) hacia donde está "other", el otro Vector3Int
    /// </summary>
    /// <param name="other">El otro Vector3Int</param>
    /// <returns>Norte, Sur, Este, Oeste (N, S, E, W) ó None</returns>
    public bool CheckAdjacencyWith(Vector3Int other, out TruckDirection result)
    {
        bool r = false;
        result = TruckDirection.None;
        //Diferencia ejeX
        int deltaX = x - other.x;
        //Diferencia ejeZ
        int deltaZ = z - other.z;

        //Estan en el mismo eje Z, pueden ser adyacentes en ejeX
        if (deltaZ == 0)
        {
            //Segun "x - other.x" este vector estaba a la derecha de 'other'
            //La conexión esta al Oeste
            if (deltaX == 1)
            {
                r = true;
                result = TruckDirection.W;
            }
            //Del reves
            if (deltaX == -1)
            {
                r = true;
                result = TruckDirection.E;
            }
            //Nota: si es mayor o menor de (+/-)1 no son adyacentes
        }
        if (deltaX == 0)
        {
            //Segun "z - other.z" este vector estaba encima de 'other'
            //La conexión está al Sur
            if (deltaZ == 1)
            {
                r = true;
                result = TruckDirection.S;
            }
            //Del reves
            if (deltaZ == -1)
            {
                r = true;
                result = TruckDirection.N;
            }
        }

        if (deltaX == 0 && deltaZ == 0) r = true;


        return r;





    }




}