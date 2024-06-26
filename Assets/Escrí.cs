using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Escrí : MonoBehaviour
{

    // 
    public GameObject ojetePublico;

    // 
    [SerializeField] private GameObject campoSerio;

    [SerializeField] private OjeteEscriptable OjeteMovido;

    [SerializeField] private InputReader input;

    public static event UnityAction<OjeteEscriptable, bool> EventoDeUnidadDeAccion = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(ojetePublico);

        InvokeRepeating(nameof(Africa),0f,0.01f);

        OjeteMovido.Pedo();

        var juegoDeOjetes = GameObject.FindAnyObjectByType<Escrí>();

        juegoDeOjetes.GetComponent<Escrí>()?.Africa();

        input.Dash += Nada;

        //StartCoroutine(nameof(Japon));
    }

    // Update is called once per frame
    void Update()
    {
        // gameObject.transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
    }

    // Que se ejecuta cada X veces por segundo
    private void FixedUpdate()
    {
        // gameObject.transform.position = new Vector3(transform.position.x+0.01f, transform.position.y, transform.position.z);

        // EventoDeUnidadDeAccion.Invoke(OjeteMovido, true);
    }

    // Cuando se instancia el objeto por primera vez antes que Start()
    private void Awake()
    {
        
    }


    private void OnEnable()
    {
        
    }

   
    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator Japon()
    {
        while (true)
        {
            gameObject.transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(0.5f);
        }        
    }

    void Africa()
    {
        ojetePublico.transform.position = new Vector3(ojetePublico.transform.position.x + 0.01f, ojetePublico.transform.position.y, ojetePublico.transform.position.z);
    }

    void Nada(bool b)
    {
        Debug.Log("Nada");
    }

}
