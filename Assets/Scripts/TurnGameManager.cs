using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnGameManager : MonoBehaviour
{
    //fix the maximum players to be 2
    const int totalPlayerNum = 2;

    //determined be host and sync accross network

    //let other class to get this number without having a reference
    public static float UpdateTime;
    public static int turnNum = 1;

    //temperorily store the two players
    //string array because of the Kyes.Copyto
    //string[] playerIDArray;
    //the array index
    public int currentPlayerIndex;

    public static int maxTurnNum = 36;
    public static float timeForEachTurn = 90;
    public static float checkTime = 3;
    public static GameControl.ControlMode controlMode = GameControl.ControlMode.PC_Controller;

    float _checkTime;
    //the maximum time for checking
    float checkTimeLimit = 6f;
    public bool exceedLimit = false;

    //true if the game is setup
    public bool isSetup = false;

    public bool startTiming = false;

    //Sync variable
    //true is the current turn is stoped
    [SerializeField]
    bool stopCurrentTurn = false;

    //true if starts checking
    public bool startChecking = false;

    //Sync variable
    //true if the checking is stopped
    [SerializeField]
    bool stopChecking = false;

    //Sync variable
    //true is the game is end
    public bool isGameEnd = false;

    //true if the function in game end is already called
    [SerializeField]
    bool alreadyCalled = false;


    //true if the game ties
    public bool isGameTie = false;

    //make it public for the recording
    Player player1, player2;

    PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.offlineMode)
            enabled = false;
    }

    void FixedUpdate()
    {
		if (PhotonNetwork.offlineMode)
            return;
        //if there is only one player, enable the control or do something else
        if (!isSetup && GameControl.playerRegistered != totalPlayerNum)
        {

        }

        //if the game is not setup and the total player 
        if (!isSetup && GameControl.playerRegistered == totalPlayerNum)
        {
            //playerIDArray = new string[GameControl.PlayerList.Count];

            ////copy the keys in dictionary to the string array
            //GameControl.PlayerList.Keys.CopyTo(playerIDArray, 0);

            //make two references of the two players
            player1 = GameControl.PlayerList["1"];
            player2 = GameControl.PlayerList["2"];

            //if two players are not null, the game is ready to setup
            if (player1 != null && player2 != null)
            {
                //add function to setup the game
                if (PhotonNetwork.isMasterClient)
                {
                    Debug.Log("Initializing setup");
                    photonView.RPC("RPCSetupGame", PhotonTargets.All, maxTurnNum, timeForEachTurn, checkTime, controlMode);             
                }

                //Debug.Log("Game start!");
            }
        }

        //if a player is disconned after the game is setup, the game will be end
        if (isSetup && GameControl.playerRegistered != totalPlayerNum)
        {
			//Debug.Log("playerRegistered: " + GameControl.playerRegistered);
            //need some work to determine who wins
            if (player1 == null)
            {
                currentPlayerIndex = 0;
            }
            else if (player2 == null)
            {
                currentPlayerIndex = 1;
            }
            //Debug.Log("Player disconnected");
            //isGameEnd = true;

			photonView.RPC("RpcForceGameEnd", PhotonTargets.All);

		}

		//if the game is setup, timing not starts and game does not end
		if (isSetup && !startTiming && !isGameEnd && !startChecking)
        {
            //if neither of the player is playing nor waiting
            if (player1.isPlaying == false && player1.isWaiting == false && player2.isPlaying == false && player2.isWaiting == false)
            {
                //Debug.Log("Ready for a new turn");
                if (turnNum != 1)
                    FileWriter.AfterCheckingRecord(turnNum - 1);


                //players are ready to start
                PlayerReady();

                //setup time for each turn
                UpdateTime = timeForEachTurn;
            }
        }

        //if game is setup and timing starts
        if (isSetup && startTiming && !isGameEnd && !startChecking && !stopCurrentTurn)
        {
            //starts timing
            UpdateTime -= Time.fixedDeltaTime;

            if (IsCubeFalling())
            {
				Debug.Log("Cube falling");

                //isGameEnd = true;

				photonView.RPC("RpcForceGameEnd", PhotonTargets.All);

			}

			//player doesn't move in a given time
			if (UpdateTime <= 0f)
            {
                Debug.Log("Timing ended");

                //isGameEnd = true;

				photonView.RPC("RpcForceGameEnd", PhotonTargets.All);

			}
			//if one cube is selected
			else if (Cube.selectNumEachTurn == 1)
            {
                exceedLimit = false;
            }
            //if time is not end but have select more than 1 cube
            else if (Cube.selectNumEachTurn >= 2)
            {
                //Debug.Log("Go for checking");


                //Debug.Log("It is the end of this turn");

                ResetCubeSelection();

                //stopCurrentTurn = true;

				photonView.RPC("RpcForceStopCurrentTurn", PhotonTargets.All);
            }
        }

        //function to stop the current turn
        if (stopCurrentTurn && !startChecking)
        {
            //Debug.Log("Stop current turn");

            player1.isPlaying = false;
            player1.isWaiting = false;
            player2.isPlaying = false;
            player2.isWaiting = false;

            startTiming = false;
            startChecking = true;
            stopChecking = false;

            Cube.selectNumEachTurn = 0;

            _checkTime = checkTime;
            checkTimeLimit = 7.5f;
            player1.isChecking = true;
            player2.isChecking = true;

        }

        //function to check start checking
        if (startChecking && !isGameEnd)
        {
            if (!GameControl.isCubeMoving())
            {
                _checkTime -= Time.fixedDeltaTime;
            }

            checkTimeLimit -= Time.fixedDeltaTime;

            //Debug.Log("IsCubeMoving " + GameControl.isCubeMoving());
            //Debug.Log("_checkTime " + _checkTime);

            if (IsCubeFalling())
            {
                Debug.Log("Cube is already falling");

                //isGameEnd = true;

				photonView.RPC("RpcForceGameEnd", PhotonTargets.All);

			}
			else if (_checkTime <= 0f || checkTimeLimit <= 0f)
            {
                if (_checkTime > 0f && checkTimeLimit <= 0f)
                {
                    exceedLimit = true;
                }

                if (turnNum == maxTurnNum)
                {
					Debug.Log("Last turn");

                    //isGameEnd = true;

					photonView.RPC("RpcForceGameEnd", PhotonTargets.All);

				}
				else
                {



					player1.isChecking = false;
                    player2.isChecking = false;

                    stopCurrentTurn = false;
                    startChecking = false;
                    //stopChecking = true;

					photonView.RPC("RpcForceStopChecking", PhotonTargets.All);

					////next turn
					turnNum++;
				}

                //Debug.Log("Start Timing: " + startTiming);
                //Debug.Log("isGameEnd" + isGameEnd);
                //Debug.Log("isSetup" + isSetup);
                //Debug.Log("startChecking" + startChecking);
                //Debug.Log("stopCurrentTurn" + stopCurrentTurn);
            }
            else if (stopChecking)
            {
                FileWriter.AfterCheckingRecord(turnNum);

                //Debug.Log("Is ok to move to next turn");

                //next turn
                turnNum++;
                player1.isChecking = false;
                player2.isChecking = false;

                stopCurrentTurn = false;
                startChecking = false;
            }
        }

        //if the game is end
        //need some work to determine who is win
        if (isGameEnd && !alreadyCalled)
        {
            FileWriter.AfterCheckingRecord(turnNum);

            player1.isPlaying = false;
            player2.isPlaying = false;
            player1.isWaiting = false;
            player2.isWaiting = false;
            player1.isChecking = false;
            player2.isChecking = false;

            if (turnNum == maxTurnNum && !IsCubeFalling())
            {
                player1.isTie = true;
                player2.isTie = true;

                isGameTie = true;
            }
            else
            {
                //if a player haven't select but the game ends
                if (exceedLimit && startTiming && Cube.selectNumEachTurn == 0)
                {
                    if (currentPlayerIndex == 0)
                    {
                        //Debug.Log("Player2 is lose");

                        player2.isLose = true;
                        player1.isWin = true;
                        alreadyCalled = true;
                    }
                    else
                    {
                        //Debug.Log("Player1 is lose");

                        player1.isLose = true;
                        player2.isWin = true;
                        alreadyCalled = true;

                    }
                }
                else
                {
                    if (currentPlayerIndex == 0)
                    {
                        //Debug.Log("Player1 is lose");

                        player1.isLose = true;
                        player2.isWin = true;
                        alreadyCalled = true;
                    }
                    else
                    {
                        //Debug.Log("Player2 is lose");

                        player2.isLose = true;
                        player1.isWin = true;
                        alreadyCalled = true;
                    }
                }
            }

            FileWriter.WriteData();

        }
    }

    //players are ready to start
    void PlayerReady()
    {

        //if it is the first turn
        if (turnNum == 1)
        {
            //Debug.Log("It is the first turn.");

            //rendom a player(doesn't work right now)
            RandomPlayerID();

            if (currentPlayerIndex == 0)
            {
                //Debug.Log("Player 1 is playing");

                player1.isPlaying = true;
                player2.isWaiting = true;

                //startTiming
                startTiming = true;

                //Debug.Log("Start Timing: " + startTiming);
                //Debug.Log("isGameEnd" + isGameEnd);
                //Debug.Log("isSetup" + isSetup);
                //Debug.Log("startChecking" + startChecking);
                //Debug.Log("stopCurrentTurn" + stopCurrentTurn);
            }
            else
            {
                //Debug.Log("Player 2 is playing");


                player2.isPlaying = true;
                player1.isWaiting = true;

                //startTiming
                startTiming = true;

                //Debug.Log("Start Timing: " + startTiming);
                //Debug.Log("isGameEnd" + isGameEnd);
                //Debug.Log("isSetup" + isSetup);
                //Debug.Log("startChecking" + startChecking);
                //Debug.Log("stopCurrentTurn" + stopCurrentTurn);
            }
        }
        //if it is the last turn
        //> or >= ?
        else
        {
            NextPlayerID();

            if (currentPlayerIndex == 0)
            {
                //Debug.Log("Player 1 is playing");


                player1.isPlaying = true;
                player2.isWaiting = true;

                //startTiming
                startTiming = true;
                //Debug.Log("Start Timing: " + startTiming);
                //Debug.Log("isGameEnd" + isGameEnd);
                //Debug.Log("isSetup" + isSetup);
                //Debug.Log("startChecking" + startChecking);
                //Debug.Log("stopCurrentTurn" + stopCurrentTurn);
            }
            else
            {
                //Debug.Log("Player 2 is playing");


                player2.isPlaying = true;
                player1.isWaiting = true;

                //startTiming
                startTiming = true;
                //Debug.Log("Start Timing: " + startTiming);
                //Debug.Log("isGameEnd" + isGameEnd);
                //Debug.Log("isSetup" + isSetup);
                //Debug.Log("startChecking" + startChecking);
                //Debug.Log("stopCurrentTurn" + stopCurrentTurn);
            }
        }
    }

    //random a player by change the current player index
    //doesn't work right now. Need debugging
    void RandomPlayerID()
    {
        if (!photonView.isMine)
            return;

        float _rand = UnityEngine.Random.Range(0f, 1f);

        if (_rand >= 0.5f)
        {
            currentPlayerIndex = 0;
        }
        else
        {
            currentPlayerIndex = 0;
        }
    }

    //switch between players by changing the current player index
    void NextPlayerID()
    {
        if (currentPlayerIndex == 0)
        {
            currentPlayerIndex = 1;
        }
        else if (currentPlayerIndex == 1)
        {
            currentPlayerIndex = 0;
        }
    }

    //the function to check whether cube is falling(need modification)
    bool IsCubeFalling()
    {
        if (Cube.cubeFallGround > 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //reset the cubes that being selected
    void ResetCubeSelection()
    {
        foreach (Cube _cube in GameControl.Cubes.Values)
        {
            if (_cube.GetCubeState() == true)
            {
                //deselect the selected cube
                _cube.Select();
            }
        }
        GameControl.UpdateCube();
    }

    [PunRPC]
    void RPCSetupGame(int _maxTurnNum, float _timeForEachTurn ,float _checkTime, GameControl.ControlMode _controlMode)
    {
        maxTurnNum = 36;
        timeForEachTurn = 90;
        checkTime = 3;
        controlMode = _controlMode;
        Debug.Log("Paramaters Setup");

        isSetup = true;
    }


    //the function sync variables across network
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        // We own this player: send the others our data
    //        stream.SendNext(isGameEnd);
    //        stream.SendNext(stopCurrentTurn);
    //        stream.SendNext(stopChecking);
    //    }
    //    else
    //    {
    //        // Network player, receive data
    //        isGameEnd = (bool)stream.ReceiveNext();
    //        stopCurrentTurn = (bool)stream.ReceiveNext();
    //        stopChecking = (bool)stream.ReceiveNext();
    //    }
    //}


	[PunRPC]
	void RpcForceGameEnd()
	{
		isGameEnd = true;
	}

	[PunRPC]
	void RpcForceStopCurrentTurn()
	{
		stopCurrentTurn = true;
	}

	[PunRPC]
	void RpcForceStopChecking()
	{
		//Debug.Log("Is ok to move to next turn");
		FileWriter.AfterCheckingRecord(turnNum);
		stopChecking = true;
	}
}
