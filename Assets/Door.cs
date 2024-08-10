using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void IsHovered()
    {
        var outline = GetComponent<Outline>();
        outline.enabled = true;
    }

    public void IsNotHovered()
    {
        var outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Interact()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 2, transform.localPosition.z);
    }
}
