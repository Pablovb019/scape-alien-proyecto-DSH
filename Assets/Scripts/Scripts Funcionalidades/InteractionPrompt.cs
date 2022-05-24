using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    private Camera _mainCam;
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private GameObject _uiPanel2;
    [SerializeField] private TextMeshProUGUI _promptText;

    private void Start()
    {
        _mainCam = Camera.main;
        _uiPanel.SetActive(false);
        _uiPanel2.SetActive(false);
    }

    /* private void LateUpdate()
    {
        var rotation = _mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    } */

    public bool IsDisplayed = false;

    public void SetUp(string promptText)
    {
        _promptText.text = promptText;
        _uiPanel.SetActive(true);
        _uiPanel2.SetActive(true);
        IsDisplayed = true;
    }

    public void SetUpChest(string promptText)
    {
        _promptText.text = promptText;
        _uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        _uiPanel.SetActive(false);
        _uiPanel2.SetActive(false);
        IsDisplayed = false;
    }
}
