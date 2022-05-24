using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Teleporter : NetworkBehaviour
{
    public Transform TeleportTo1;
    public Transform StartTeleporter1;
    public Transform TeleportTo2;
    public Transform StartTeleporter2;
    public Transform TeleportTo3;
    public Transform StartTeleporter3;
    private bool isTeleporting = true;

    void Awake()
    {
        TeleportTo1 = GameObject.FindGameObjectWithTag("Teleporter1Destino").transform;
        StartTeleporter1 = GameObject.FindGameObjectWithTag("Teleporter1Origen").transform;
        TeleportTo2 = GameObject.FindGameObjectWithTag("Teleporter2Destino").transform;
        StartTeleporter2 = GameObject.FindGameObjectWithTag("Teleporter2Origen").transform;
        TeleportTo3 = GameObject.FindGameObjectWithTag("Teleporter3Destino").transform;
        StartTeleporter3 = GameObject.FindGameObjectWithTag("Teleporter3Origen").transform;
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Teleporter1Origen") && isTeleporting)
        {
            transform.position = TeleportTo1.transform.position;
            StartCoroutine(TP());
        }
        
        if(other.gameObject.CompareTag("Teleporter1Destino") && isTeleporting)
        {
            transform.position = StartTeleporter1.transform.position;
            StartCoroutine(TP());
        }

        if(other.gameObject.CompareTag("Teleporter2Origen") && isTeleporting)
        {
            transform.position = TeleportTo2.transform.position;
            StartCoroutine(TP());
        }
        
        if(other.gameObject.CompareTag("Teleporter2Destino") && isTeleporting)
        {
            transform.position = StartTeleporter2.transform.position;
            StartCoroutine(TP());
        }

        if(other.gameObject.CompareTag("Teleporter3Origen") && isTeleporting)
        {
            transform.position = TeleportTo3.transform.position;
            StartCoroutine(TP());
        }
        
        if(other.gameObject.CompareTag("Teleporter3Destino") && isTeleporting)
        {
            transform.position = StartTeleporter3.transform.position;
            StartCoroutine(TP());
        }
    }

    IEnumerator TP()
    {
        isTeleporting = false;
        yield return new WaitForSeconds(2);
        isTeleporting = true;
    }

}
