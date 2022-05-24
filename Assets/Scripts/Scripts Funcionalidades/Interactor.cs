using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPrompt _interactionPrompt;
    [SerializeField] private GameObject barra;
    [SerializeField] private GameObject gen;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject escape;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Transform child;
    [SerializeField] private Transform childChest;
    [SerializeField] private Transform childEscape;
    [SerializeField] private int barreraFinal = 0;
    [SerializeField] private GameObject palancaSalida;
    [SerializeField] private Animator animacionVerja;
    [SerializeField] private Animator animacionChest;
    [SerializeField] private bool verjaAbierta = false;
    [SerializeField] private Interactable _interactable;

    [SerializeField] private GameObject PowerUpChest1;
    [SerializeField] private GameObject PowerUpChest2;
    [SerializeField] private GameObject PowerUpChest3;
    [SerializeField] private GameObject PowerUpChest4;
    [SerializeField] private GameObject PowerUpChest5;
    [SerializeField] private GameObject PowerUpChest6;
    [SerializeField] private GameObject PowerUpChest7;

    private readonly Collider[] _colliders = new Collider[3];
    private ChestState oldChestState = ChestState.Closed;

    [SerializeField] private int _numFound;

    void Start()
    {
        palancaSalida = GameObject.FindGameObjectWithTag("palancaSalida (Generator)");
    }

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            _interactable = _colliders[0].GetComponent<Interactable>();

            if (_interactable != null)
            {
                if (!_interactionPrompt.IsDisplayed && _interactable.ToString().Contains("gen")) _interactionPrompt.SetUp(_interactable.InteractionPrompt);

                if (!_interactionPrompt.IsDisplayed && _interactable.ToString().Contains("palancaSalida")) _interactionPrompt.SetUp(_interactable.InteractionPrompt);

                if (!_interactionPrompt.IsDisplayed && _interactable.ToString().Contains("chest")) _interactionPrompt.SetUpChest(_interactable.InteractionPrompt);

                if (!_interactionPrompt.IsDisplayed && _interactable.ToString().Contains("escape")) _interactionPrompt.SetUpChest(_interactable.InteractionPrompt);

                if (Keyboard.current.eKey.wasPressedThisFrame) _interactable.Interact(this);

                gen = GameObject.FindWithTag(_interactable.ToString());

                if (_interactable.ToString().Contains("chest"))
                {
                    chest = GameObject.FindWithTag(_interactable.ToString());
                    childChest = chest.transform.GetChild(0);
                }

                if (_interactable.ToString().Contains("escape"))
                {
                    escape = GameObject.FindWithTag(_interactable.ToString());
                    childEscape = escape.transform.GetChild(11);
                }

                if (gen.transform.childCount > 0 && _interactable.ToString().Contains("gen"))
                {
                    gen = GameObject.FindWithTag(_interactable.ToString());
                    child = gen.transform.GetChild(1);
                    particles = gen.GetComponentInChildren<ParticleSystem>();
                }

                if (barreraFinal >= 5 && verjaAbierta == false)
                {
                    Destroy(GameObject.FindGameObjectWithTag("endGame1"));
                    Destroy(GameObject.FindGameObjectWithTag("endGame2"));
                    if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "palancaSalida (Generator)")
                    {
                        Destroy(palancaSalida.GetComponent<Generator>());
                        Destroy(GameObject.FindGameObjectWithTag("puertaAvion"));
                        animacionVerja = GameObject.FindGameObjectWithTag("verja").GetComponent<Animator>();
                        animacionVerja.SetBool("open", true);
                        verjaAbierta = true;
                    }
                }

                if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "escape (Generator)")
                {
                    NetworkManager.Singleton.Shutdown();
                    SceneManager.LoadScene("HasGanado");
                }

                if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "gen1 (Generator)")
                {
                    Destroy(child.GetComponent<Generator>());
                    particles.Stop();
                    particles.Clear();
                    barreraFinal++;

                    if (barreraFinal > KillerControlAuthorative.NumGeneradoresHechos)
                    {
                        KillerControlAuthorative.NumGeneradoresHechos = barreraFinal;
                    }
                }

                else if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "gen2 (Generator)")
                {
                    Destroy(child.GetComponent<Generator>());
                    particles.Stop();
                    particles.Clear();
                    barreraFinal++;

                    if (barreraFinal > KillerControlAuthorative.NumGeneradoresHechos)
                    {
                        KillerControlAuthorative.NumGeneradoresHechos = barreraFinal;
                    }
                }

                else if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "gen3 (Generator)")
                {
                    Destroy(child.GetComponent<Generator>());
                    particles.Stop();
                    particles.Clear();
                    barreraFinal++;

                    if (barreraFinal > KillerControlAuthorative.NumGeneradoresHechos)
                    {
                        KillerControlAuthorative.NumGeneradoresHechos = barreraFinal;
                    }
                }

                else if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "gen4 (Generator)")
                {
                    Destroy(child.GetComponent<Generator>());
                    particles.Stop();
                    particles.Clear();
                    barreraFinal++;

                    if (barreraFinal > KillerControlAuthorative.NumGeneradoresHechos)
                    {
                        KillerControlAuthorative.NumGeneradoresHechos = barreraFinal;
                    }
                }

                else if (barra.transform.localScale == new Vector3(1, 1, 1) && _interactable.ToString() == "gen5 (Generator)")
                {
                    Destroy(child.GetComponent<Generator>());
                    particles.Stop();
                    particles.Clear();
                    barreraFinal++;

                    if (barreraFinal > KillerControlAuthorative.NumGeneradoresHechos)
                    {
                        KillerControlAuthorative.NumGeneradoresHechos = barreraFinal;
                    }
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest1 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    animacionChest.SetTrigger("Open");
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest2 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    PowerUpChest2 = chest.transform.GetChild(4).gameObject;
                    StartCoroutine(WaitingTime(PowerUpChest2));
                    PlayerControlAuthorative.PowerUpReparacion = true;
                    PlayerControlAuthorative.BoostRepararDes.gameObject.SetActive(false);
                    PlayerControlAuthorative.BoostRepararAct.gameObject.SetActive(true);
                    PlayerControlAuthorative.BoostRepararAct.transform.position = PlayerControlAuthorative.BoostRepararDes.transform.position;
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest3 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    PowerUpChest3 = chest.transform.GetChild(4).gameObject;
                    StartCoroutine(WaitingTime(PowerUpChest3));
                    PlayerControlAuthorative.PowerUpReparacion = true;
                    PlayerControlAuthorative.BoostRepararDes.gameObject.SetActive(false);
                    PlayerControlAuthorative.BoostRepararAct.gameObject.SetActive(true);
                    PlayerControlAuthorative.BoostRepararAct.transform.position = PlayerControlAuthorative.BoostRepararDes.transform.position;
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest4 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    PowerUpChest4 = chest.transform.GetChild(4).gameObject;
                    StartCoroutine(WaitingTime(PowerUpChest4));
                    PlayerControlAuthorative.PowerUpReparacion = true;
                    PlayerControlAuthorative.BoostRepararDes.gameObject.SetActive(false);
                    PlayerControlAuthorative.BoostRepararAct.gameObject.SetActive(true);
                    PlayerControlAuthorative.BoostRepararAct.transform.position = PlayerControlAuthorative.BoostRepararDes.transform.position;
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest5 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    PowerUpChest5 = chest.transform.GetChild(4).gameObject;
                    StartCoroutine(WaitingTime(PowerUpChest5));
                    PlayerControlAuthorative.PowerUpReparacion = true;
                    PlayerControlAuthorative.BoostRepararDes.gameObject.SetActive(false);
                    PlayerControlAuthorative.BoostRepararAct.gameObject.SetActive(true);
                    PlayerControlAuthorative.BoostRepararAct.transform.position = PlayerControlAuthorative.BoostRepararDes.transform.position;
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest6 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    animacionChest.SetTrigger("Open");
                }

                else if (Keyboard.current.eKey.wasPressedThisFrame && _interactable.ToString() == "chest7 (Generator)")
                {
                    Destroy(childChest.GetComponent<Generator>());
                    animacionChest = chest.GetComponent<Animator>();
                    animacionChest.SetTrigger("Open");
                }
            }
        }
        else
        {
            if (_interactable != null) _interactable = null;
            if (_interactionPrompt.IsDisplayed) _interactionPrompt.Close();
        }
    }

    private IEnumerator WaitingTime(GameObject powerUp)
    {
        animacionChest.SetTrigger("Open");
        yield return new WaitForSeconds(4f);
        Destroy(powerUp);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
