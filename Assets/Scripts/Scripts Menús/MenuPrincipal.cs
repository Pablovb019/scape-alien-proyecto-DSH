using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipal : MonoBehaviour
{
    // Volumen
    [Header("Configuración del Audio")]
    [SerializeField] private TMP_Text ValorVolumen = null;
    [SerializeField] private Slider SliderVolumen = null;
    [SerializeField] private float VolumenPorDefecto = 3f;

    [Header("Configuración de Gráficos")]
    [SerializeField] private Slider SliderBrillo = null;
    [SerializeField] private TMP_Text ValorTextoBrillo = null;
    [SerializeField] private float BrilloPorDefecto = 3f;

    [SerializeField] private TMP_Dropdown DropdownCalidad = null;
    [SerializeField] private Toggle CheckboxPantallaCompleta = null;

    private int _nivelCalidad;
    private bool _pantallaCompleta;
    private float _nivelBrillo;

    [Header("Confirmación")]
    [SerializeField] private GameObject confirmacion = null;

    [Header("Tipos de Resolución")]
    public TMP_Dropdown resolucionDropdown;
    private Resolution[] resoluciones;

    private void Start(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        resoluciones = Screen.resolutions;
        resolucionDropdown.ClearOptions();

        List<string> opciones = new List<string>();

        int indiceResolucionActual = 0;

        for(int i = 0; i < resoluciones.Length; i++){
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if(resoluciones[i].width == Screen.width && resoluciones[i].height == Screen.height){
                indiceResolucionActual = i;
            }
        }

        resolucionDropdown.AddOptions(opciones);
        resolucionDropdown.value = indiceResolucionActual;
        resolucionDropdown.RefreshShownValue();
    }

    public void AplicarResolucion(int resolucionElegida){
        Resolution resolucion = resoluciones[resolucionElegida];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }
    
    // Menu Principal

    public void NuevoJuegoSi(){
        SceneManager.LoadScene("LobbyVivox",LoadSceneMode.Single);
    }

    public void Salir(){
        Debug.Log("Fin del Juego");
        Application.Quit();
    }

    public void EstablecerVolumen(float volumen){
        AudioListener.volume = volumen;
        ValorVolumen.text = volumen.ToString("0.0");
    }

    public void AplicarVolumen(){
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(Confirmar());
    }

    public void AplicarBrillo(float brillo){
        _nivelBrillo = brillo;
        ValorTextoBrillo.text = brillo.ToString("0.0");
    }

    public void AplicarPantallaCompleta(bool PantallaCompleta){
        _pantallaCompleta = PantallaCompleta;
    }

    public void AplicarCalidad(int calidad){
        _nivelCalidad = calidad;
    }

    public void AplicarGraficos(){
        PlayerPrefs.SetFloat("masterBrightness", _nivelBrillo);

        PlayerPrefs.SetInt("masterQuality", _nivelCalidad);
        QualitySettings.SetQualityLevel(_nivelCalidad);

        PlayerPrefs.SetInt("masterFullScreen", _pantallaCompleta ? 1 : 0);
        Screen.fullScreen = _pantallaCompleta;

        StartCoroutine(Confirmar());
    }

    public void ResetVolumen(string MenuConfig){
        if(MenuConfig == "Gráficos"){
            SliderBrillo.value = BrilloPorDefecto;
            ValorTextoBrillo.text = BrilloPorDefecto.ToString("0.0");

            DropdownCalidad.value = 4;
            QualitySettings.SetQualityLevel(4);

            CheckboxPantallaCompleta.isOn = false;
            Screen.fullScreen = false;

            Resolution resolucionActual = Screen.currentResolution;
            Screen.SetResolution(resolucionActual.width, resolucionActual.height, Screen.fullScreen);
            resolucionDropdown.value = 0;
            AplicarGraficos();
        }

        if (MenuConfig == "Audio"){
            AudioListener.volume = VolumenPorDefecto;
            SliderVolumen.value = VolumenPorDefecto;
            ValorVolumen.text = VolumenPorDefecto.ToString();
            AplicarVolumen();
        }
    }

    public IEnumerator Confirmar(){
        confirmacion.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        confirmacion.SetActive(false);
    }    
}
