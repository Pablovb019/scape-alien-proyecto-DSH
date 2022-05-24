using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class ProgressBar : NetworkBehaviour
{
    public KeyCode Key = KeyCode.E;
    public GameObject bar;
    public GameObject progressbar;
    public int time;
    public float aumentar = 0;
    private bool RepRapido = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerControlAuthorative.PowerUpReparacion && Input.GetKeyDown(KeyCode.R))
        {
            RepRapido = true;
        }


        if(Input.GetKey(Key) && progressbar.activeSelf == true && aumentar < 1) 
        {
            if (RepRapido)
            {
                aumentar = aumentar + 0.003f;
            }
            else
            {
                aumentar = aumentar + 0.0015f;
            }
            
            if(aumentar > 1)
            {
                aumentar = 1;
                PlayerControlAuthorative.PowerUpReparacion = false;
                RepRapido = false;
                PlayerControlAuthorative.BoostRepararAct.gameObject.SetActive(false);
                PlayerControlAuthorative.BoostRepararDes.gameObject.SetActive(true);
            }

            bar.transform.localScale = new Vector3(aumentar, 1, 1);
        }
        else if(progressbar.activeSelf == false)
        {
            bar.transform.localScale = new Vector3(0, 1, 1);
            aumentar = 0;
        } 
    }
}
