using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public bool HasKey = false;

    private void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame) HasKey = !HasKey;
    }
}
