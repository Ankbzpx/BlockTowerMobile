using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//this script requires the PlayerControl script
[RequireComponent(typeof(PlayerControl))]
public class Player : MonoBehaviour {
    //used for player registeration
    public static int playerGlobalID = 0;
    int localID;

    PhotonView photonView;
    PlayerControl playerControl;

    [SerializeField]
    GameObject QuitButton;

    int _updateTime;

    //accessed by the TurnGameManager script
    public bool isPlaying = false;
    public bool isWaiting = false;
    public bool isChecking = false;
    public bool isWin = false;
    public bool isLose = false;
    public bool isTie = false;

    //UI element reference
    [SerializeField]
    GameObject _playingTurnPanel;

    [SerializeField]
    GameObject _waitingTurnPanel;

    [SerializeField]
    GameObject _checkCubeFallPanel;

    [SerializeField]
    GameObject _winPanel;

    [SerializeField]
    GameObject _losePanel;

    [SerializeField]
    GameObject _tiePanel;

    [SerializeField]
    GameObject _quitPanel;

    [SerializeField]
    GameObject _initialingPanel;

    [SerializeField]
    Text _playerName;

    [SerializeField]
    Text _turnNum;

    [SerializeField]
    Text _playingTimeText;

    [SerializeField]
    Text _WaitingTimeText;

    private void Awake()
    {
        if (PhotonNetwork.offlineMode)
        {
            isPlaying = true;
        }
        else
        {
            DontDestroyOnLoad(this);
        }
	}

	// Use this for initialization
	void Start () {

        //give the player ID
        playerGlobalID++;
        if (PhotonNetwork.isMasterClient)
        {
            localID = playerGlobalID;
        }
        else
        {
            localID = 3- playerGlobalID;
        }

        GameControl.RegisterPlayer(localID.ToString(), this);
        //the name may be editable by the player
        name = "Player " + localID;
        PhotonNetwork.playerName = name;

        _playerName.text = name;

        photonView = GetComponent<PhotonView>();
        playerControl = GetComponent<PlayerControl>();

        if (!photonView.isMine && PhotonNetwork.connected)
        {
            playerControl.enabled = false;
            gameObject.SetActive(false);
            enabled = false;
        }

        CloseAllPanel();
    }
	
	// Update is called once per frame
	void Update () {

        if (Cube.cubeFallGround > 3)
        {
            isLose = true;
        }

        _turnNum.text = TurnGameManager.turnNum.ToString();
        _updateTime = (int)TurnGameManager.UpdateTime;

        UpdatePlayerState();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (PhotonNetwork.offlineMode && !Tutorial.hasShownTutorial)
                return;

            _quitPanel.SetActive(!_quitPanel.activeSelf);

            GetComponent<GamepadUIEvent>().SetEventObject();
        }
    }


    void UpdatePlayerState()
    {
        if (isWin)
        {
            CloseAllPanel();
            playerControl.isTurn = false;
            _winPanel.SetActive(true);
            //Debug.Log("isWin");
        }
        else if (isTie)
        {
            CloseAllPanel();
            playerControl.isTurn = false;
            _tiePanel.SetActive(true);
            //Debug.Log("isTie");
        }
        else if (isLose)
        {
            CloseAllPanel();
            playerControl.isTurn = false;
            _losePanel.SetActive(true);
            //Debug.Log("isLose");
        }
        else
        {
            if (isPlaying)
            {
				if(!Cube.showHighlight)
					Cube.showHighlight = true;

				_playingTimeText.text = _updateTime.ToString();
                CloseAllPanel();

                playerControl.isTurn = true;

                _playingTurnPanel.SetActive(true);
                //Debug.Log("isPlaying");
            }
            else if (isWaiting)
            {
				if (Cube.showHighlight)
					Cube.showHighlight = false;

				_WaitingTimeText.text = _updateTime.ToString();
                CloseAllPanel();

                playerControl.isTurn = false;

                _waitingTurnPanel.SetActive(true);
                //Debug.Log("isWaiting");
            }
            else if (isChecking)
            {
                CloseAllPanel();

                playerControl.isTurn = false;

                _checkCubeFallPanel.SetActive(true);
                //Debug.Log("isChecking");
            }
            else
            {
                CloseAllPanel();
                playerControl.isTurn = false;
                _initialingPanel.SetActive(true);


            }
        }
    }

    void CloseAllPanel()
    {
        _playingTurnPanel.SetActive(false);
        _waitingTurnPanel.SetActive(false);
        _checkCubeFallPanel.SetActive(false);
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
        _tiePanel.SetActive(false);
        _initialingPanel.SetActive(false);
    }

    //return the player local ID
    public int GetPlayerLocalID()
    {
        return localID;
    }

    public void ClientDisconnect()
    {

        //need debug!
        if (photonView.isMine)
        {
            GameControl.UnregisterPlayer(localID.ToString());
            Cube.cubeFallGround = 0;

            if (!PhotonNetwork.offlineMode)
            {
                FileWriter.WriteData();
            }
        }
        

        PhotonNetwork.Disconnect();

    }

    void OnDestroy()
    {
        //unregister the player when the player is destroyed
        isPlaying = false;
        isWaiting = false;
        isChecking = false;
        isWin = false;
        isLose = false;
        isTie = false;

		if (photonView.isMine)
		{
			GameControl.UnregisterPlayer(localID.ToString());
			Cube.cubeFallGround = 0;
		}
    }
}
