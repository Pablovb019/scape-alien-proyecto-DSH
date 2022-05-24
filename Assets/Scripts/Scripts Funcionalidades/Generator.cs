using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, Interactable
{
    [SerializeField] private string _prompt;

    private GameObject bar;

    public string InteractionPrompt => _prompt; 

    void Start()
    {
        bar = GameObject.FindGameObjectWithTag("Barra"); 
    }

    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if(inventory == null)  return false;

        if(inventory.HasKey)
        {
            return true;
        }

        return false;
    }
}
