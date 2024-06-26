using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjoteEscriptable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Escrí.EventoDeUnidadDeAccion += Funcional;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Funcional(OjeteEscriptable oe, bool b)
    {
        if (b)
        {
            oe.Pedo();
        }
    }

    private void OnDestroy()
    {
        Escrí.EventoDeUnidadDeAccion -= Funcional;
    }
}
