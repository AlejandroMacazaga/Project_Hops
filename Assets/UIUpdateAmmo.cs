using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils.EventBus;

public class UIUpdateAmmo : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent <TextMeshProUGUI>();
        EventBus<AmmoChangeEvent>.Register(new EventBinding<AmmoChangeEvent>(ChangeAmmo));
    }
    void ChangeAmmo(AmmoChangeEvent e)
    {
        _text.text = e.CurrentAmmo.ToString();
    }
}
