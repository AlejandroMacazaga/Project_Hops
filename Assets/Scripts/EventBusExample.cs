using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBusExample : MonoBehaviour
{
    EventBinding<TestEvent> testEventBinding;

    private void OnEnable()
    {
        testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
        EventBus<TestEvent>.Register(testEventBinding);

        EventBus<TestEvent>.Raise(new TestEvent());
    }

    private void OnDisable()
    {
        EventBus<TestEvent>.Deregister(testEventBinding);
    }




    private void HandleTestEvent(TestEvent t) {
        Debug.Log("Event tested");

    }

}
