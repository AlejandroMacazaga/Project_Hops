using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseDeDatos", menuName = "MenuObjeto/objetito")]
public class ObjetoScriptable : ScriptableObject
{
    public TipoObjeto TipoObjeto;

    public void Pedo()
    {
        Debug.Log("el objeto se ha tirado un pedo");
    }
}

public enum TipoObjeto {Objeto1, Objeto2, Objeto3};
