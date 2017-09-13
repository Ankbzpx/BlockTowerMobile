using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Cube : MonoBehaviour{

    public static int currentCubeID = 0;
    public static int selectNumEachTurn = 0;
    public static int highlightedCubeID = 0;
    public static bool showHighlight = false;
    public static bool isMovingOutOfTower = false;

    public static int globalID = 0;
    public static int cubeFallGround = 0;

    [SerializeField]
    bool isSelected = false;

	//bool _isSelected = false;

	//List<int> contactCubes;

	public int localID;
    Color originalColor;
    //PhotonView photonView;

    Rigidbody rb;
    Renderer rd;

    //avoid duplicated selection
    static bool selectDelaying = false;

    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>();
        rd = GetComponent<Renderer>();


        //other parameter initialization
        globalID++;

        if(localID == 0)
            localID = globalID;

        originalColor = rd.material.color;
		//photonView = GetComponent<PhotonView>();
		GameControl.RegisterCube(localID, this);

        //contactCubes = new List<int>();
    }

    // Update is called once per frame
    void Update () {

        CheckSelection();

		//if (PhotonNetwork.isMasterClient)
		//{
		//	if (rb.isKinematic && !isSelected)
		//		rb.isKinematic = false;
		//}
		//else if (!PhotonNetwork.isMasterClient)
		//{
		//	if (!rb.isKinematic)
		//		rb.isKinematic = true;
		//}
    }

    //use collider to disable the upper blocks
    public void Select()
    {
        if (!isSelected && currentCubeID == 0)
        {
            isSelected = true;
            currentCubeID = localID;

            if(TurnGameManager.controlMode == GameControl.ControlMode.PC_Controller)
                highlightedCubeID = 0;

            selectNumEachTurn++;

            selectDelaying = true;

            StartCoroutine(Delay());
        }
        else if (isSelected && currentCubeID == localID)
        {
            isSelected = false;

            if (TurnGameManager.controlMode == GameControl.ControlMode.PC_Controller)
                highlightedCubeID = currentCubeID;

            currentCubeID = 0;
            isMovingOutOfTower = false;

            selectNumEachTurn++;

            selectDelaying = true;

            StartCoroutine(Delay());
        }

        GameControl.UpdateCube();
    }

    public bool GetSelectState()
    {
        return isSelected;
    }

    void CheckSelection()
    {
        if (isSelected)
        {
            rb.useGravity = false;
            rb.freezeRotation = true;
            rb.velocity = Vector3.zero;
            rd.material.color = Color.red;
        }
        else if (!isSelected)
        {
            rb.useGravity = true;
            rb.freezeRotation = false;

            if (highlightedCubeID != localID)
            {
                rd.material.color = originalColor;
            }
            else if (highlightedCubeID == localID)
            {
                if (showHighlight)
                    rd.material.color = Color.yellow;
                else
                    rd.material.color = originalColor;
            }
        }

    }


   
    //update the number of cube in ground whenever it is detected
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && localID != currentCubeID)
        {
            cubeFallGround++;

			//Debug.Log("cubeFallGround: " + cubeFallGround);
        }
    }



    //return the cube local ID
    public int GetCubeLocalID()
    {
        return localID;
    }

    //return the cube select state
    public bool GetCubeState()
    {
        return isSelected;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        selectDelaying = false;
    }


  //  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  //  {
  //      if (stream.isWriting)
  //      {
		//	//We own this player: send the others our data

		//	stream.SendNext(isSelected);
		//}
  //      else
  //      {
		//	// Network player, receive data
		//	this.isSelected = (bool)stream.ReceiveNext();
		//}
  //  }
}
