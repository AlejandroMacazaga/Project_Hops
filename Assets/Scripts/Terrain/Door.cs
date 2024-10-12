using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool _isOpen = false;
    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public void IsHovered()
    {
        //_outline.enabled = true;
    }

    public void IsNotHovered()
    {
        //_outline.enabled = false;
    }

    public void Interact()
    {
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (!_isOpen ? 2.5f : -2.5f), transform.localPosition.z);
        _isOpen = !_isOpen;
    }
}
