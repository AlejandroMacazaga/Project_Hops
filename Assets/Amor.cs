using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Script1.Evento += Funcion;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Funcion(ObjetoScriptable os, bool b)
    {
        if (b)
        {
            os.Pedo();
        }
    }

    private void OnDestroy()
    {
        Script1.Evento -= Funcion;
    }
}
