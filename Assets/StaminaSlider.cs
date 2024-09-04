using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Player.Events;
using UnityEngine;
using UnityEngine.UI;
using Utils.EventBus;

public class StaminaSlider : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Slider slider;
    void Start()
    {
        slider.maxValue = 100;
        EventBus<PlayerStaminaChange>.Register(new EventBinding<PlayerStaminaChange>((e) =>
        {
            slider.value = e.Current;
        }));
    }

}
