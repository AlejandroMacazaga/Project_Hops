using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;
using Utils.EventBus;
using Weapons;

public class UIHoldSlider : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Slider slider;
    void Start()
    {
        EventBus<AttackHoldEvent>.Register(new EventBinding<AttackHoldEvent>((e) =>
        {
            slider.value = e.Progress;
        }));
    }


    
}
