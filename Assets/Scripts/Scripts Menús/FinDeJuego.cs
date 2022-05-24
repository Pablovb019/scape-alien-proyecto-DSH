using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDeJuego : MonoBehaviour
{
    void Start(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void VolverMenuPrincipal(){
        Debug.Log("Fin del Juego");
        Application.Quit();
    }
}