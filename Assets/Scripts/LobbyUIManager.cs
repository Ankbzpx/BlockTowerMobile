using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LobbyUIManager : MonoBehaviour {

    public enum LobbyStage {none, initialize, mainMenu, settings, cubeScene, matchMaking, secondMatchMaking, thirdMatchMaking };

    [SerializeField]
    Transform initialLocation;
    [SerializeField]
    Transform mainMenuLocation;
    [SerializeField]
    Transform settingsMenuLocation;
    [SerializeField]
    Transform cubeSceneLocation;
    [SerializeField]
    Transform matchMakingMenuLocation;
    [SerializeField]
    Transform secondMatchMakingMenuLocation;
    [SerializeField]
    Transform thirdMatchMakingMenuLocation;


    [SerializeField]
    GameObject initialCanvas;
    [SerializeField]
    GameObject mainMenuCanvas;
    [SerializeField]
    GameObject settingsCanvas;
    [SerializeField]
    GameObject matchMakingCanvas;
    [SerializeField]
    GameObject secondMatchMakingCanvas;
    [SerializeField]
    GameObject thirdMatchMakingCanvas;
    [SerializeField]
    GameObject loadingCanvas;


    public LobbyStage lobbyStage;
    AudioManager audioManager;

    public float speed = 5f;
    public float rotSpeed = 5f;
    public bool alreadyLoaded = false;

    Vector3 originalPos;
    Quaternion originalRot;

    bool isSoundPlayed = false;

    [SerializeField]
    Dropdown settingsDropDown;

    void Awake()
    {
        Screen.SetResolution(1280, 800, true);

        TurnGameManager.controlMode = GameControl.ControlMode.PC_Keyboard;
        Camera.main.transform.position = initialLocation.position;
        Camera.main.transform.rotation = initialLocation.rotation;

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        //mainMenuCanvasUIGamepadTrigger = mainMenuCanvas.GetComponent<CanvasUIGamepadTrigger>();
        //settingsCanvasUIGamepadTrigger = settingsCanvas.GetComponent<CanvasUIGamepadTrigger>();
        //matchMakingCanvasUIGamepadTrigger = matchMakingCanvas.GetComponent<CanvasUIGamepadTrigger>();
        //secondMatchMakingCanvasUIGamepadTrigger = secondMatchMakingCanvas.GetComponent<CanvasUIGamepadTrigger>();
        //thirdMatchMakingCanvasCanvasUIGamepadTrigger = thirdMatchMakingCanvas.GetComponent<CanvasUIGamepadTrigger>();
    }

    // Use this for initialization
    void Start ()
    {
        if(!alreadyLoaded)
            lobbyStage = LobbyStage.initialize;

        //close unnecessary canvas
        initialCanvas.SetActive(true);
        DisableWorldSpaceCanvas();
        loadingCanvas.SetActive(false);

        settingsDropDown.onValueChanged.AddListener(delegate { SettingsDropDownOnValueChanged(); });
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(audioManager == null)
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        originalPos = Camera.main.transform.position;
        originalRot = Camera.main.transform.rotation;


        if (lobbyStage == LobbyStage.initialize)
        {

            if (Input.anyKeyDown)
            {
                //close the initial canvas
                initialCanvas.SetActive(false);

                ShowMainMenu();
            }
        }
        else if (lobbyStage == LobbyStage.mainMenu)
        {
            if (initialCanvas.activeSelf)
                initialCanvas.SetActive(false);
            if (loadingCanvas.activeSelf)
                loadingCanvas.SetActive(false);

            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }

            if (originalPos == mainMenuLocation.position && originalRot == mainMenuLocation.rotation)
                return;

            if (EventSystem.current.currentSelectedGameObject == null)
                mainMenuCanvas.GetComponent<GamepadUIEvent>().SetEventObject();


            Vector3 targetPos = Vector3.MoveTowards(originalPos, mainMenuLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, mainMenuLocation.rotation, rotSpeed);

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
        else if (lobbyStage == LobbyStage.cubeScene)
        {
            if(!loadingCanvas.activeSelf)
                loadingCanvas.SetActive(true);
            if (originalPos == cubeSceneLocation.position && originalRot == cubeSceneLocation.rotation)
                return;

            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }



            Vector3 targetPos = Vector3.MoveTowards(originalPos, cubeSceneLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, cubeSceneLocation.rotation, rotSpeed);

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
        else if (lobbyStage == LobbyStage.settings)
        {
            if (loadingCanvas.activeSelf)
                loadingCanvas.SetActive(false);
            if (originalPos == settingsMenuLocation.position && originalRot == settingsMenuLocation.rotation)
                return;

            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }

            if (EventSystem.current.currentSelectedGameObject == null)
                settingsCanvas.GetComponent<GamepadUIEvent>().SetEventObject();

            Vector3 targetPos = Vector3.MoveTowards(originalPos, settingsMenuLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, settingsMenuLocation.rotation, rotSpeed);

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
        else if (lobbyStage == LobbyStage.matchMaking)
        {
            if (loadingCanvas.activeSelf)
                loadingCanvas.SetActive(false);
            if (originalPos == matchMakingMenuLocation.position && originalRot == matchMakingMenuLocation.rotation)
                return;
            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }

            if (EventSystem.current.currentSelectedGameObject == null)
                matchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();

            Vector3 targetPos = Vector3.MoveTowards(originalPos, matchMakingMenuLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, matchMakingMenuLocation.rotation, rotSpeed);

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
        else if (lobbyStage == LobbyStage.secondMatchMaking)
        {
            if (loadingCanvas.activeSelf)
                loadingCanvas.SetActive(false);
            if (originalPos == secondMatchMakingMenuLocation.position && originalRot == secondMatchMakingMenuLocation.rotation)
                return;

            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }

            if (EventSystem.current.currentSelectedGameObject == null)
                secondMatchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();

            Vector3 targetPos = Vector3.MoveTowards(originalPos, secondMatchMakingMenuLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, secondMatchMakingMenuLocation.rotation, rotSpeed );

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
        else if (lobbyStage == LobbyStage.thirdMatchMaking)
        {
            if (loadingCanvas.activeSelf)
                loadingCanvas.SetActive(false);
            if (originalPos == secondMatchMakingMenuLocation.position && originalRot == secondMatchMakingMenuLocation.rotation)
                return;

            if (!isSoundPlayed) { CameraMoveSound(); isSoundPlayed = true; }

            if (EventSystem.current.currentSelectedGameObject == null)
                thirdMatchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();

            Vector3 targetPos = Vector3.MoveTowards(originalPos, thirdMatchMakingMenuLocation.position, speed);
            Quaternion targetRot = Quaternion.RotateTowards(originalRot, thirdMatchMakingMenuLocation.rotation, rotSpeed);

            Camera.main.transform.position = Vector3.Lerp(originalPos, targetPos, 0.2f);
            Camera.main.transform.rotation = Quaternion.Lerp(originalRot, targetRot, 1f);
        }
    }


    public void ShowCubeSceneCanvas()
    {
        if (lobbyStage == LobbyStage.initialize)
            return;

        isSoundPlayed = false;
        lobbyStage = LobbyStage.cubeScene;

        DisableWorldSpaceCanvas();
    }

    public void ShowMainMenu()
    {
        isSoundPlayed = false;
        lobbyStage = LobbyStage.mainMenu;

        DisableWorldSpaceCanvas();
        mainMenuCanvas.SetActive(true);
        mainMenuCanvas.GetComponent<GamepadUIEvent>().SetEventObject();
    }

    public void ShowSettingsMenu()
    {
        if (lobbyStage == LobbyStage.initialize)
            return;


        isSoundPlayed = false;
        lobbyStage = LobbyStage.settings;

        DisableWorldSpaceCanvas();
        settingsCanvas.SetActive(true);
        settingsCanvas.GetComponent<GamepadUIEvent>().SetEventObject();
    }

    public void ShowMatchMakingCanvas()
    {
        isSoundPlayed = false;
        lobbyStage = LobbyStage.matchMaking;

        DisableWorldSpaceCanvas();
        matchMakingCanvas.SetActive(true);
        matchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();
    }

    public void ShowSecondMatchMakingCanvas()
    {
        if (lobbyStage == LobbyStage.initialize)
            return;


        isSoundPlayed = false;
        lobbyStage = LobbyStage.secondMatchMaking;

        DisableWorldSpaceCanvas();
        secondMatchMakingCanvas.SetActive(true);
        secondMatchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();
    }

    public void ShowThirdMatchMakingCanvas()
    {
        if (lobbyStage == LobbyStage.initialize)
            return;


        isSoundPlayed = false;
        lobbyStage = LobbyStage.thirdMatchMaking;

        DisableWorldSpaceCanvas();
        thirdMatchMakingCanvas.SetActive(true);
        thirdMatchMakingCanvas.GetComponent<GamepadUIEvent>().SetEventObject();
    }

    public void CloseLoadingCanvas()
    {
        if (loadingCanvas.activeSelf)
            loadingCanvas.SetActive(false);

        isSoundPlayed = false;
        lobbyStage = LobbyStage.none;

        DisableWorldSpaceCanvas();
    }

    public void QuitGame()
    {

        Application.Quit();
    }

    void CameraMoveSound()
    {
        audioManager.Play("Step");
        audioManager.Play("Step");
    }

    public void PlayHoverSound()
    {
        audioManager.Play("Ding");
    }

    void SettingsDropDownOnValueChanged()
    {
        if (lobbyStage == LobbyStage.initialize)
            return;


        switch (settingsDropDown.value)
        {
            case 1:
                TurnGameManager.controlMode = GameControl.ControlMode.PC_Keyboard;
                break;
            case 2:
                TurnGameManager.controlMode = GameControl.ControlMode.PC_Controller;
                break;
            case 3:
                TurnGameManager.controlMode = GameControl.ControlMode.PC_VR;
                break;
            case 4:
                TurnGameManager.controlMode = GameControl.ControlMode.Mobile;
                break;
            default:
                TurnGameManager.controlMode = GameControl.ControlMode.PC_Keyboard;
                break;
        }

        Debug.Log("TurnGameManager.controlMode: "+ TurnGameManager.controlMode);
    }


    void DisableWorldSpaceCanvas()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        matchMakingCanvas.SetActive(false);
        secondMatchMakingCanvas.SetActive(false);
        thirdMatchMakingCanvas.SetActive(false);


    }

    void OnDestroy()
    {
        settingsDropDown.onValueChanged.RemoveAllListeners();
    }
}
