using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LectorDeOjetes", menuName = "MenuOjete/Esfinter")]


public class OjeteEscriptable : ScriptableObject
{
    public TipoOjete TipoOjete;

    public void Pedo()
    {
        Debug.Log("El ojete de tipo " + TipoOjete + " ha tenido un escape");
    }
}

public enum TipoOjete { Prieto, Suelto, Miguel, None }