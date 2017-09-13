using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : Photon.PunBehaviour
{
    enum ConnectionMode {QuickJoin, CreateRoom, JoinLobby, SinglePlayer, None}

    const string VERSION = "v1.3";
    public string roomName = "Block Tower Default";
    static NetworkManager networkManager;
    static int roomId = 0;

    GameControl.ControlMode _controlMode;

    public MainPanelUI mainPanelUI;
    public LobbyUIManager lobbyManager;

    public GameObject[] dontDestoryObject;

    [SerializeField]
    InputField roomNameInputField;

    ConnectionMode connectionMode;

    void Awake()
    {
        DontDestroyOnLoad(this);

        connectionMode = ConnectionMode.QuickJoin;

        if (networkManager == null)
        {
            networkManager = this;
        }
        else
        {
            DestroyObject(gameObject);
        }

		//Photon network tweak
		//PhotonNetwork.networkingPeer.CrcEnabled = true;
		//PhotonNetwork.isMessageQueueRunning = false;
		//PhotonNetwork.networkingPeer.MaximumTransferUnit = 520;


		//photon network initialization
		PhotonNetwork.sendRate = 20;
		PhotonNetwork.sendRateOnSerialize = 15;

		PhotonNetwork.MaxResendsBeforeDisconnect = 10;

	}

	void Start()
    {
			_controlMode = GameControl.ControlMode.PC_Controller;
		
    }

	private void LateUpdate()
	{
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.SendOutgoingCommands();
		}
	}


	/// Start the connection process. 
	/// - If already connected, we attempt joining a random room
	/// - if not yet connected, Connect this application instance to Photon Cloud Network
	public void QuickConnect()
    {
        //avoid UI navigation on controller
        if (lobbyManager.lobbyStage == LobbyUIManager.LobbyStage.initialize)
            return;

        connectionMode = ConnectionMode.QuickJoin;


        TurnGameManager.controlMode = _controlMode;



            Debug.Log("Not connected, connect to master server");
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(VERSION);

    }

    public void CreateGameWithSettings()
    {
        //avoid UI navigation on controller
        if (lobbyManager.lobbyStage == LobbyUIManager.LobbyStage.initialize)
            return;

        connectionMode = ConnectionMode.CreateRoom;

        TurnGameManager.maxTurnNum = mainPanelUI._maxTurn;
        TurnGameManager.timeForEachTurn = mainPanelUI._timeTurn;
        TurnGameManager.checkTime = mainPanelUI._checkTime;

        switch (mainPanelUI._dropDownValue)
        {
            case 1:
                _controlMode = GameControl.ControlMode.PC_Keyboard;
                break;
            case 2:
                _controlMode = GameControl.ControlMode.PC_Controller;
                break;
            case 3:
                _controlMode = GameControl.ControlMode.PC_VR;
                break;
            case 4:
                _controlMode = GameControl.ControlMode.Mobile;
                break;
            default:
                _controlMode = GameControl.ControlMode.PC_Keyboard;
                break;
        }


		TurnGameManager.controlMode = _controlMode;


        PhotonNetwork.ConnectUsingSettings(VERSION);
    }

    public void MatchMakingJoin()
    {
        connectionMode = ConnectionMode.JoinLobby;

        PhotonNetwork.ConnectUsingSettings(VERSION);
    }

    public void MatchMakingQuit()
    {
        connectionMode = ConnectionMode.None;

        if(PhotonNetwork.connected)
            PhotonNetwork.Disconnect();
    }


    public void SinglePlayerMode()
    {
        //avoid UI navigation on controller
        if (lobbyManager.lobbyStage == LobbyUIManager.LobbyStage.initialize)
            return;


        connectionMode = ConnectionMode.SinglePlayer;

        PhotonNetwork.offlineMode = true;

        TurnGameManager.maxTurnNum = mainPanelUI._maxTurn;
        TurnGameManager.timeForEachTurn = mainPanelUI._timeTurn;
        TurnGameManager.checkTime = mainPanelUI._checkTime;

        switch (mainPanelUI._dropDownValue)
        {
            case 1:
                _controlMode = GameControl.ControlMode.PC_Keyboard;
                break;
            case 2:
                _controlMode = GameControl.ControlMode.PC_Controller;
                break;
            case 3:
                _controlMode = GameControl.ControlMode.PC_VR;
                break;
            case 4:
                _controlMode = GameControl.ControlMode.Mobile;
                break;
            default:
                _controlMode = GameControl.ControlMode.PC_Controller;
                break;
        }



        TurnGameManager.controlMode = _controlMode;

        lobbyManager.CloseLoadingCanvas();

        SceneManager.LoadScene(2);

    }


    public override void OnConnectedToMaster()
    {
        //Debug.Log("Master Serve Connected");

        if (connectionMode == ConnectionMode.QuickJoin)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()  
            PhotonNetwork.JoinRandomRoom();
        }
        else if (connectionMode == ConnectionMode.CreateRoom)
        {
            //Doesn't add property in current stage, the property of the room was made static in TurnGameManager
            PhotonNetwork.CreateRoom(roomName);
        }
        else if (connectionMode == ConnectionMode.JoinLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnCreatedRoom()
    {
        //Debug.Log("Room successfully created");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        //Debug.Log("Connection failed");

        lobbyManager.ShowMainMenu();
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        //Debug.Log("Connection failed");

        lobbyManager.ShowMainMenu();
    }


    public override void OnDisconnectedFromPhoton()
    {
        if (connectionMode == ConnectionMode.None)
            return;


        SceneManager.LoadScene(0);
        lobbyManager.alreadyLoaded = true;
        lobbyManager.ShowMainMenu();


        foreach (Player p in GameControl.PlayerList.Values)
		{
			Destroy(p);
		}
		

		Cube.selectNumEachTurn = 0;
		Cube.cubeFallGround = 0;
		Cube.currentCubeID = 0;
		Cube.globalID = 0;
		TurnGameManager.turnNum = 1;
		Player.playerGlobalID = 0;

		GameControl.PlayerList.Clear();
		GameControl.Cubes.Clear();
		GameControl.playerRegistered = 0;
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Join the room");

		for (int i = 0; i < dontDestoryObject.Length; i++)
		{
			DontDestroyOnLoad(dontDestoryObject[i]);
		}

		DontDestroyOnLoad(this);

		Cube.selectNumEachTurn = 0;
		Cube.cubeFallGround = 0;
		Cube.currentCubeID = 0;
		Cube.globalID = 0;
		TurnGameManager.turnNum = 1;
		Player.playerGlobalID = 0;

		GameControl.PlayerList.Clear();
		GameControl.Cubes.Clear();
		GameControl.playerRegistered = 0;

        if (PhotonNetwork.offlineMode)
        {

            Debug.Log("Load single player map");
        }
        else
        {
            PlayerManager.SpawnPlayer();

            SceneManager.LoadScene(1);

            lobbyManager.CloseLoadingCanvas();

            FileWriter.SetupFileWriter();
        }
    }

    public void DisconnectMasterClient()
    {
        PhotonNetwork.Disconnect();
    }

    public void ConfirmRoomName()
    {
        //if (roomNameInputField.text != null)
        //    roomName = roomNameInputField.text;
        //else
        roomName = "Room " + roomId.ToString();
        roomId++;

        Debug.Log("Room Name: " + roomName);
    }


}
