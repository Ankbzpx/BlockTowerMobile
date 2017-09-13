using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {
    //definition of enums
    enum TutorialStage { Beginning, Select, Deselect, Move1, Move2, RotateCamera, SwitchCube, End, None};
    enum PlayMode { Mode_1, Mode_2 };
    enum Direction { Left, Right, Up, Down };

    PlayMode playMode;
    TutorialStage tutorialStage;
    
    //the reference of playercontrol
    PlayerControl _playerControl;
    //the reference of photonview
    PhotonView photonView;
    //the reference of audiomanager
    AudioManager audioManager;


    //temporarily disable it
    public static bool hasShownTutorial = false;



    //the target field of view
    float targetFoV = 10f;
    //How quickly the zoom reaches the target value
    float Dampening = 10.0f;
    // The minimum field of view value we want to zoom to
    public float MinFov = 15.0f;
    // The maximum field of view value we want to zoom to
    public float MaxFov = 80.0f;

    //the factors that can be changed through inspector
    public float wheelSpeed = 50f;
    public float rotateSpeed = 50f;
    public float controllerFactor = 3f;
	public float cubeThreshold = 0.15f;
	public float heightThreshold = 0.4f;
	public float moveFactor = 25f;
    public float rotationFactor = 10f;

    //temporary variale used for rotation (convert axis to button)
    Vector3 tempPosi = Vector3.zero;
    bool isUsing = false;

    //temp variable for storing the input form x,y axis
    float H_input;
    float V_input;

    //temp variable for storing the input from axis
    float move_3;
    float move_4;
    float move_5;
    float move_6;

    //whether is is allow for switch the cubes to realize discrete cube switch
    bool allowSwitch = false;

    //temp cube reference for the hightlighted cube
    Cube hightedCube;

    //the list of potential cubes in certain direction
    List<Cube> potentialCubes;



    //the UI reference
    [SerializeField]
    Canvas _playerCanvas;
    [SerializeField]
    Canvas tutorialCanvas;

    [SerializeField]
    GameObject BeginningPanel;
    [SerializeField]
    GameObject SelectPanel;
    [SerializeField]
    GameObject DeselectPanel;
    [SerializeField]
    GameObject Move1Panel;
    [SerializeField]
    GameObject Move2Panel;
    [SerializeField]
    GameObject RotateCameraPanel;
    [SerializeField]
    GameObject SwitchCubePanel;
    [SerializeField]
    GameObject EndPanel;

	float longPressTime;
	float switchCubeTime;

	void Start() {
        _playerControl = GetComponent<PlayerControl>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        if (!hasShownTutorial)
            tutorialStage = TutorialStage.Beginning;
        else
            tutorialStage = TutorialStage.None;

        _playerControl.enabled = false;
        _playerCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(true);

        CloseAllPanel();

        Debug.Log(tutorialStage);

        Cube.showHighlight = true;
		
		Cube.highlightedCubeID = 4;
    }

    void Update() {
        if (tutorialStage == TutorialStage.Beginning)
        {
            if (!BeginningPanel.activeSelf)
                BeginningPanel.SetActive(true);

            if (Input.GetButtonDown("Submit"))
                tutorialStage = TutorialStage.SwitchCube;
            else if (Input.GetButtonDown("Cancel"))
                tutorialStage = TutorialStage.None;

        }
        else if (tutorialStage == TutorialStage.SwitchCube)
        {
            if (BeginningPanel.activeSelf)
                BeginningPanel.SetActive(false);
            if (!SwitchCubePanel.activeSelf)
                SwitchCubePanel.SetActive(true);

            if (Cube.currentCubeID == 0)
            {
                H_input = Input.GetAxisRaw("Horizontal");
                V_input = Input.GetAxisRaw("Vertical");

                int tempHighlightID = Cube.highlightedCubeID;
				if (H_input != 0f || V_input != 0f)
				{
					longPressTime += Time.deltaTime;

					if (longPressTime > 0.3f)
						allowSwitch = true;
				}
				else
				{
					longPressTime = 0f;
				}

				if (allowSwitch)
				{
					switchCubeTime += Time.deltaTime;

					if (switchCubeTime < 0.1f)
						return;

					if (H_input != 0f || V_input != 0f)
					{
						if (Mathf.Abs(H_input) >= Mathf.Abs(V_input))
						{
							//left
							if (H_input <= 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Left);
							}
							//right
							else if (H_input > 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Right);
							}
						}
						else if (Mathf.Abs(H_input) < Mathf.Abs(V_input))
						{
							//up
							if (V_input >= 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Up);
							}
							//down
							else if (V_input < 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Down);
							}
						}

					}

					if (Cube.highlightedCubeID != tempHighlightID)
					{
						allowSwitch = false;
						switchCubeTime = 0;
					}
				}
                else
                {
                    if (H_input == 0f && V_input == 0f)
                    {
                        allowSwitch = true;
                    }
                }
            }

            if(Input.GetButtonDown("Submit"))
                tutorialStage = TutorialStage.Select;

        }
        else if (tutorialStage == TutorialStage.Select)
        {
            if (SwitchCubePanel.activeSelf)
                SwitchCubePanel.SetActive(false);
            if (!SelectPanel.activeSelf)
                SelectPanel.SetActive(true);

            if (Input.GetButtonDown("A") && Cube.currentCubeID == 0)
            {

                if (Cube.highlightedCubeID != 0)
                {

                    if (Cube.highlightedCubeID != 0)
                    {
                        if (GameControl.IsAtTop(Cube.highlightedCubeID))
                        {
                            if (audioManager != null)
                                audioManager.Play("Step");

                            Debug.Log("Cube at top");
                        }
                        else
                        {
                            if (audioManager != null)
                                audioManager.Play("Click");

                                GameControl.GetCube(Cube.highlightedCubeID).Select();

                            tutorialStage = TutorialStage.Move1;
                        }

                    }
                }
            }
            else if (Cube.currentCubeID == 0)
            {
                H_input = Input.GetAxisRaw("Horizontal");
                V_input = Input.GetAxisRaw("Vertical");

                int tempHighlightID = Cube.highlightedCubeID;

				if (H_input != 0f || V_input != 0f)
				{
					longPressTime += Time.deltaTime;

					if (longPressTime > 0.3f)
						allowSwitch = true;
				}
				else
				{
					longPressTime = 0f;
				}

				if (allowSwitch)
				{
					switchCubeTime += Time.deltaTime;

					if (switchCubeTime < 0.1f)
						return;

					if (H_input != 0f || V_input != 0f)
					{
						if (Mathf.Abs(H_input) >= Mathf.Abs(V_input))
						{
							//left
							if (H_input <= 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Left);
							}
							//right
							else if (H_input > 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Right);
							}
						}
						else if (Mathf.Abs(H_input) < Mathf.Abs(V_input))
						{
							//up
							if (V_input >= 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Up);
							}
							//down
							else if (V_input < 0)
							{

								//function to switch highlighted cube
								SwitchCubes(Direction.Down);
							}
						}

					}

					if (Cube.highlightedCubeID != tempHighlightID)
					{
						allowSwitch = false;
						switchCubeTime = 0;
					}
				}
                else
                {
                    if (H_input == 0f && V_input == 0f)
                    {
                        allowSwitch = true;
                    }
                }
            }

        }

        else if (tutorialStage == TutorialStage.Move1)
        {
            if (SelectPanel.activeSelf)
                SelectPanel.SetActive(false);
            if (!Move1Panel.activeSelf)
                Move1Panel.SetActive(true);

            if (Cube.currentCubeID != 0)
            {
                H_input = Input.GetAxisRaw("Horizontal");
                V_input = Input.GetAxisRaw("Vertical");

                //calculate the local direction
                Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);


                Vector3 movement = localForward * V_input / moveFactor + localRight * H_input / moveFactor;
                if (movement != Vector3.zero)
                {
                    if (!PhotonNetwork.offlineMode)
                    {
                        photonView.RPC("RPCMoveCube_1", PhotonTargets.MasterClient, Cube.currentCubeID,
                            movement);
                    }
                    else
                    {
                        Vector3 newPosition = GameControl.GetCube(Cube.currentCubeID).transform.position + movement;
                        GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>().MovePosition(newPosition);
                    }

                }

            }

            if (Input.GetButtonDown("Submit"))
                tutorialStage = TutorialStage.Move2;
        }
        else if (tutorialStage == TutorialStage.Move2)
        {
            if (Move1Panel.activeSelf)
                Move1Panel.SetActive(false);
            if (!Move2Panel.activeSelf)
                Move2Panel.SetActive(true);

            Vector3 movement = Vector3.zero;


            if (Input.GetButton("Y"))
            {
                playMode = PlayMode.Mode_2;
            }
            else
            {
                playMode = PlayMode.Mode_1;
            }

            H_input = Input.GetAxisRaw("Horizontal");
            V_input = Input.GetAxisRaw("Vertical");

            if (Cube.currentCubeID != 0)
            {
                Rigidbody _rb = GameControl.GetCube(Cube.currentCubeID).GetComponent<Rigidbody>();

				//calculate the local direction
				Vector3 localForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
				Vector3 localRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

				if (Cube.currentCubeID != 0)
				{
					if (playMode == PlayMode.Mode_1)
					{

						movement = localForward * V_input / moveFactor + localRight * H_input / moveFactor;
						if (movement != Vector3.zero)
						{
							if (!PhotonNetwork.offlineMode)
							{
								photonView.RPC("RPCMoveCube_1", PhotonTargets.MasterClient, Cube.currentCubeID,
									movement);
							}
							else
							{
								_rb.MovePosition(_rb.position + movement);
							}

						}

						move_5 = Input.GetAxis("5th");
						move_6 = Input.GetAxis("6th");

						Vector3 movement_2 = Vector3.zero;

						if (Mathf.Abs(move_6) >= Mathf.Abs(move_5))
						{
							movement_2 = move_6 / moveFactor * Vector3.up;
						}
						else
						{
							movement_2 = move_5 / moveFactor * new Vector3(rotateSpeed, 0, 0);
						}


						if (movement_2 != Vector3.zero)
						{

							Vector3 newPosition = _rb.position + new Vector3(0, movement_2.y, 0);
							float rotateRadius = movement_2.x;

							_rb.MovePosition(newPosition);
							GameControl.GetCube(Cube.currentCubeID).transform.Rotate(Vector3.up, rotateRadius, Space.World);
						}
					}
					else if (playMode == PlayMode.Mode_2)
					{

						movement = Vector3.zero;


						if (Mathf.Abs(V_input) >= Mathf.Abs(H_input))
						{
							movement = V_input / moveFactor * Vector3.up;
						}
						else
						{
							movement = H_input / moveFactor * new Vector3(rotateSpeed, 0, 0);
						}


						if (movement != Vector3.zero)
						{
							if (!PhotonNetwork.offlineMode)
							{
								photonView.RPC("RPCMoveCube_2", PhotonTargets.MasterClient, Cube.currentCubeID,
								movement);
							}
							else
							{
								Vector3 newPosition = GameControl.GetCube(Cube.currentCubeID).transform.position + new Vector3(0, movement.y, 0);
								float rotateRadius = movement.x;

								_rb.MovePosition(newPosition);
								GameControl.GetCube(Cube.currentCubeID).transform.Rotate(Vector3.up, rotateRadius, Space.World);
							}
						}
					}
				}
			}

            if (Input.GetButtonDown("Submit"))
                    tutorialStage = TutorialStage.RotateCamera;    
        }
        else if (tutorialStage == TutorialStage.RotateCamera)
        {
            if (Move2Panel.activeSelf)
                Move2Panel.SetActive(false);
            if (!RotateCameraPanel.activeSelf)
                RotateCameraPanel.SetActive(true);

            move_3 = Input.GetAxis("3rd");
            move_4 = Input.GetAxis("4th");
            move_5 = Input.GetAxis("5th");

            //rotate camera by controller
            if (move_3 != 0f || move_4 != 0f && isUsing)
            {
                Vector2 rotateVector = new Vector2(move_3, move_4);

                Vector3 newDir = Input.mousePosition;

                Vector3 deltaRotation = rotateSpeed * rotateVector;

                if ((-deltaRotation.y < 0 && Camera.main.transform.localRotation.x >= 0f) || (-deltaRotation.y > 0 && Camera.main.transform.localRotation.x <= 0.45f))
                {
                    Camera.main.transform.Rotate(Vector3.right, -deltaRotation.y / rotationFactor / 2f, Space.Self);

                }

                Camera.main.transform.parent.Rotate(Vector3.up, deltaRotation.x / rotationFactor);

                tempPosi = newDir;

                isUsing = false;
            }

            if (move_3 == 0f || move_4 == 0f)
            {
                tempPosi = new Vector2(move_3, move_4);
                isUsing = true;
            }

            //zoom by scoller
            if (/*move_5 >= 0f ||*/ Input.GetButton("Button 6"))
            {
                if (move_5 == 0f)
                    move_5 = 1f;

                targetFoV = Mathf.Clamp(Camera.main.fieldOfView - wheelSpeed * move_5, MinFov, MaxFov);

                // The framerate independent damping factor
                var factor = 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime);

                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFoV, factor);
            }
            else if (/*move_5 <= 0f ||*/ Input.GetButton("Button 7"))
            {
                if (move_5 == 0f)
                    move_5 = -1f;

                targetFoV = Mathf.Clamp(Camera.main.fieldOfView - wheelSpeed * move_5, MinFov, MaxFov);

                // The framerate independent damping factor
                var factor = 1.0f - Mathf.Exp(-Dampening * Time.fixedDeltaTime);

                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFoV, factor);
            }

            if (Input.GetButtonDown("Submit"))
                tutorialStage = TutorialStage.Deselect;
        }

        else if (tutorialStage == TutorialStage.Deselect)
        {
            if (RotateCameraPanel.activeSelf)
                RotateCameraPanel.SetActive(false);
            if (!DeselectPanel.activeSelf)
                DeselectPanel.SetActive(true);

            if (Input.GetButtonDown("A") && Cube.currentCubeID != 0)
            {
                if (audioManager != null)
                    audioManager.Play("Click");

                if (!PhotonNetwork.offlineMode)
                    photonView.RPC("RPCSelectCube", PhotonTargets.All, Cube.currentCubeID);
                else
                    GameControl.GetCube(Cube.currentCubeID).Select();

                tutorialStage = TutorialStage.End;
            }
        }
        else if (tutorialStage == TutorialStage.End)
        {
            if (DeselectPanel.activeSelf)
                DeselectPanel.SetActive(false);
            if (BeginningPanel.activeSelf)
                BeginningPanel.SetActive(false);
            if (!EndPanel.activeSelf)
                EndPanel.SetActive(true);

            if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Submit"))
            {
                _playerControl.enabled = true;
                _playerCanvas.gameObject.SetActive(true);
                tutorialCanvas.gameObject.SetActive(false);
                hasShownTutorial = true;
                CloseAllPanel();
                tutorialStage = TutorialStage.None;
            }
        }
        else if (tutorialStage == TutorialStage.None)
        {
            tutorialCanvas.gameObject.SetActive(false);
            CloseAllPanel();
            _playerControl.enabled = true;
            _playerCanvas.gameObject.SetActive(true);

            hasShownTutorial = true;
            tutorialStage = TutorialStage.None;
        }
    }

	void SwitchCubes(Direction _dir)
	{
		if (Cube.highlightedCubeID == 0)
			return;

		hightedCube = GameControl.GetCube(Cube.highlightedCubeID);

		potentialCubes = new List<Cube>();

		if (GameControl.Cubes.Values.Count != 0)
		{
			switch (_dir)
			{
				case Direction.Left:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, new Vector3(-Camera.main.transform.right.x, 0, -Camera.main.transform.right.z).normalized) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) <= cubeThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Right:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) <= cubeThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Up:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, Vector3.up) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold && Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) < heightThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					if (potentialCubes.Count == 0)
					{
						foreach (Cube _cube in GameControl.Cubes.Values)
						{
							Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

							if (Vector3.Dot(_cDir, Vector3.up) > 0f)
							{
								if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold)
								{
									potentialCubes.Add(_cube);
								}
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes, true);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				case Direction.Down:

					foreach (Cube _cube in GameControl.Cubes.Values)
					{
						Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

						if (Vector3.Dot(_cDir, Vector3.down) > 0f)
						{
							if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold && Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) < heightThreshold)
							{
								potentialCubes.Add(_cube);
							}
						}
					}

					if (potentialCubes.Count == 0)
					{
						foreach (Cube _cube in GameControl.Cubes.Values)
						{
							Vector3 _cDir = (_cube.transform.position - hightedCube.transform.position).normalized;

							if (Vector3.Dot(_cDir, Vector3.down) > 0f)
							{
								if (Mathf.Abs(_cube.transform.position.y - hightedCube.transform.position.y) > cubeThreshold)
								{
									potentialCubes.Add(_cube);
								}
							}
						}
					}

					Cube.highlightedCubeID = GetClosestCubeID(potentialCubes, true);

					if (Cube.highlightedCubeID != hightedCube.GetCubeLocalID())
						if (audioManager != null)
							audioManager.Play("Ding");

					break;
				default:
					break;
			}
		}

		potentialCubes.Clear();
	}

	int GetClosestCubeID(List<Cube> _potentialCubes, bool isNearCamera = false)
	{
		if (_potentialCubes.Count == 0)
			return Cube.highlightedCubeID;

		int id = Cube.highlightedCubeID;

		float minDistance = 50f;

		if (isNearCamera)
		{
			for (int i = 0; i < _potentialCubes.Count; i++)
			{
				Vector3 refPoint = new Vector3(Camera.main.transform.position.x, _potentialCubes[i].transform.position.y, Camera.main.transform.position.z);

				float _tempDis = (_potentialCubes[i].transform.position - hightedCube.transform.position).magnitude + 2 * (refPoint - _potentialCubes[i].GetComponent<Collider>().ClosestPoint(refPoint)).magnitude;
				if (_tempDis < minDistance && Cube.highlightedCubeID != _potentialCubes[i].transform.GetComponent<Cube>().localID)
				{
					id = _potentialCubes[i].transform.GetComponent<Cube>().localID;
					minDistance = _tempDis;
				}
			}
		}
		else
		{
			for (int i = 0; i < _potentialCubes.Count; i++)
			{
				float _tempDis = (_potentialCubes[i].transform.position - hightedCube.transform.position).magnitude;
				if (_tempDis < minDistance && Cube.highlightedCubeID != _potentialCubes[i].transform.GetComponent<Cube>().localID)
				{
					id = _potentialCubes[i].transform.GetComponent<Cube>().localID;
					minDistance = _tempDis;
				}
			}
		}


		return id;
	}

	void CloseAllPanel()
    {

        BeginningPanel.SetActive(false);
        SelectPanel.SetActive(false);
        DeselectPanel.SetActive(false);
        Move1Panel.SetActive(false);
        Move2Panel.SetActive(false);
        RotateCameraPanel.SetActive(false);
        SwitchCubePanel.SetActive(false);
        EndPanel.SetActive(false);
    }
}
