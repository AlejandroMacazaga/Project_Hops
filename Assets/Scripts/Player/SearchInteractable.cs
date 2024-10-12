using System.Collections;
using Entities;
using KBCore.Refs;
using Player.Classes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    
    public class SearchInteractable : ValidatedMonoBehaviour
    {

        public IInteractable CurrentInteractable;
        [SerializeField, Self] private CharacterClass owner;
        [SerializeField] private float interactableRange;
        private Camera cam;
        public virtual void OnEnable()
        {
            cam = Camera.main;
            StartCoroutine(nameof(HandleInteractableSearch));
        }

        public virtual void OnDisable()
        {
            StopCoroutine(nameof(HandleInteractableSearch));
        }
        
        public IEnumerator HandleInteractableSearch()
        {
            while (enabled)
            {
                if(cam && Physics.Raycast(
                       cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)),
                       cam.transform.forward,
                       out var hit,
                       interactableRange))
                {
                    if (hit.transform.TryGetComponent<IInteractable>(out var interactable))
                    {
                        Debug.Log("This is an interactable component");
                        if (interactable != CurrentInteractable)
                        {
                            CurrentInteractable?.IsNotHovered();
                            CurrentInteractable = interactable;
                            
                        }
                        CurrentInteractable?.IsHovered();
                        // TODO: Do what needs to be done when something can be interacted with
                    }
                    else
                    {
                        if (CurrentInteractable != null)
                        {
                            CurrentInteractable?.IsNotHovered();
                            CurrentInteractable = null;
                        }
                    }
                    
                }
                else
                {
                    if (CurrentInteractable != null)
                    {
                        CurrentInteractable?.IsNotHovered();
                        CurrentInteractable = null;
                    }
                }
                yield return Utils.Helpers.GetWaitForSeconds(0.1f);
            }
        }

        
    }
}