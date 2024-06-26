using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Script1 : MonoBehaviour

{

    public GameObject objeto;

    [SerializeField] private GameObject campo;
    [SerializeField] private ObjetoScriptable ObjetoScriptable;
    [SerializeField] private InputReader IR;

    public static event UnityAction<ObjetoScriptable, bool> Evento = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(objeto);
        InvokeRepeating(nameof(Blancos),0f,0.01f);

        ObjetoScriptable.Pedo();

        var variable = GameObject.FindAnyObjectByType<Script1>();

        variable.GetComponent<Script1>()?.Blancos(); //la interrogación comprueba si es null

        IR.Dash += Funcion1;

        //StartCoroutine(nameof(Negros));
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);

    }

    //se ejecuta x veces por segundo
    private void FixedUpdate()
    {
        //Evento.Invoke(ObjetoScriptable, true);
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        
    }

    IEnumerator Negros()
    {
        while (true)
        {
            gameObject.transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Blancos()
    {
        objeto.transform.position = new Vector3(objeto.transform.position.x + 0.01f, transform.position.y, transform.position.z);
    }

    void Funcion1(bool b) {
        Debug.Log("he clicado");
    }
}
