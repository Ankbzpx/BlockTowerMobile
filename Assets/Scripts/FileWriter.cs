using UnityEngine;
using System.IO;
using System;

public class FileWriter : MonoBehaviour {
    //non-static in the future
    static TurnGameManager _tuneGameManager;

    static FileInfo _fileInfo;
    static StreamWriter _streamWriter;
    static string fileName;

    //make it public to let score board access
    static int[] turnNum;
    static int[] player_index;
    static float[] player_totalTime;
    static float[] player_selectTime;
    static float[] player_moveOutTime;
    static float[] moveOutStability;
    static float[] player_placeTime;
    static float[] checkTime;
    static int[] selectedCubeID;
    static float[] maxHeight;
    static float[] totalMovement;
    static float[] distance;
    static float[] FinalStability;
    static int[] TopComplexity;

    static bool initialFile = false;
    static bool isFileWritten = false;

    static int _selectCubeId;
    static Vector3 initialPosi;
    Vector3 lastPosi;
    //avoid the first turn not included
	static int lastTurnNum = -1;

    static bool isInitialPositionRecorded = false;
    static bool isLastPositionRecorded = false;
    static bool isMoveOutStabilityRecored = false;
    static bool isComplexityRecored = false;
    static float tempMovement = 0f;

    private void Start()
    {
        if (PhotonNetwork.offlineMode)
            return;


        _tuneGameManager = GetComponent<TurnGameManager>();
        initialFile = false;

        if (PhotonNetwork.offlineMode)
            enabled = false;
    }

    public static void SetupFileWriter()
    {
        if (PhotonNetwork.offlineMode)
            return;

        fileName = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_data.txt";

        if (Application.platform == RuntimePlatform.Android)
		{
            
			_fileInfo = new FileInfo(Application.persistentDataPath + "/" + fileName);
		}
		else
		{
			_fileInfo = new FileInfo(Application.dataPath + "\\" + fileName);

		}

        if (!_fileInfo.Exists)
        {
            _streamWriter = _fileInfo.CreateText();
            Debug.Log("file created");
        }
        else
        {
            _streamWriter = _fileInfo.AppendText();
            Debug.Log("file existed");
        }

        isFileWritten = false;

    }

    //calculate the needed data
    //write the file
    void FixedUpdate()
    {
        if (PhotonNetwork.offlineMode)
            return;


        if (!initialFile)
        {
            //wait until the information is updated
            if (_tuneGameManager.isSetup)
            {
                _streamWriter.WriteLine(DateTime.Now);

                _streamWriter.WriteLine("Block Tower Mobile");
                _streamWriter.WriteLine("Max Turn Number\t" + TurnGameManager.maxTurnNum);
                _streamWriter.WriteLine("Time for Each Turn\t" + TurnGameManager.timeForEachTurn);
                _streamWriter.WriteLine("TurnNum\tPlayer\tSelectedBlockID\tTotalTime\tSelectTime\tMoveOutTime\tPlaceTime\tCheckTime\tMoveOutStability\tMaxHeight\tTotalMovement\tDistance\tFinalStability\tTopComplexity");

                turnNum = new int[TurnGameManager.maxTurnNum];
                player_index = new int[TurnGameManager.maxTurnNum];
                player_totalTime = new float[TurnGameManager.maxTurnNum];
                player_selectTime = new float[TurnGameManager.maxTurnNum];
                player_moveOutTime = new float[TurnGameManager.maxTurnNum];
                moveOutStability = new float[TurnGameManager.maxTurnNum];
                player_placeTime = new float[TurnGameManager.maxTurnNum];
                checkTime = new float[TurnGameManager.maxTurnNum];
                selectedCubeID = new int[TurnGameManager.maxTurnNum];
                maxHeight = new float[TurnGameManager.maxTurnNum];
                totalMovement = new float[TurnGameManager.maxTurnNum];
                distance = new float[TurnGameManager.maxTurnNum];
                FinalStability = new float[TurnGameManager.maxTurnNum];
                TopComplexity = new int[TurnGameManager.maxTurnNum];

                initialFile = true;
                Debug.Log("File initialized");
            }
        }
        else
        {
            RecordData();
        }
    }


    //need function to record player index
    void RecordData()
    {
        if (PhotonNetwork.offlineMode)
            return;

        //make a reference of the current ID
        if (Cube.currentCubeID != 0)
            _selectCubeId = Cube.currentCubeID;


        if (_tuneGameManager.isSetup && !_tuneGameManager.isGameEnd)
        {
            int _turnNum = TurnGameManager.turnNum;
            turnNum[_turnNum - 1] = _turnNum;

            player_index[_turnNum - 1] = _tuneGameManager.currentPlayerIndex;


            if (_tuneGameManager.startTiming)
            {
                player_totalTime[_turnNum - 1] += Time.fixedDeltaTime;

                if (!isComplexityRecored)
                {
                    TopComplexity[_turnNum - 1] = GameControl.ObtainTopComplexity();
                    isComplexityRecored = true;
                }

                //when no cube is selected
                if (Cube.currentCubeID == 0)
                {
                    player_selectTime[_turnNum - 1] += Time.fixedDeltaTime;
                }
                //when a cube is selected
                else if (Cube.currentCubeID != 0)
                {
                    //Debug.Log("Is moving...");

                    if (!isInitialPositionRecorded)
                    {
                        initialPosi = GameControl.GetCube(Cube.currentCubeID).transform.position;

                        isInitialPositionRecorded = true;
                    }

                    if (isLastPositionRecorded)
                    {
                        tempMovement += (GameControl.GetCube(Cube.currentCubeID).transform.position - lastPosi).magnitude;
						totalMovement[_turnNum - 1] = tempMovement;
						isLastPositionRecorded = false;
                    }

                    if (!isLastPositionRecorded)
                    {
                        lastPosi = GameControl.GetCube(Cube.currentCubeID).transform.position;
                        isLastPositionRecorded = true;
                    }


                    lastPosi = GameControl.GetCube(Cube.currentCubeID).transform.position;

                    if (selectedCubeID[_turnNum - 1] != Cube.currentCubeID)
                        selectedCubeID[_turnNum - 1] = Cube.currentCubeID;


                    if (!Cube.isMovingOutOfTower)
                    {
                        if (!isMoveOutStabilityRecored)
                        {
                            moveOutStability[_turnNum - 1] = GameControl.ObtainStability();
                            isMoveOutStabilityRecored = true;
                        }

                        player_moveOutTime[_turnNum - 1] += Time.fixedDeltaTime;
                    }
                    else
                    {
                        player_placeTime[_turnNum - 1] += Time.fixedDeltaTime;

                    }
                }
            }
            else if (_tuneGameManager.startChecking)
            {
                player_totalTime[_turnNum - 1] += Time.fixedDeltaTime;

                checkTime[_turnNum - 1] += Time.fixedDeltaTime;
            }
        }
    }

    //data recorded immediately after the checking
    public static void AfterCheckingRecord(int _turnNum)
    {
        if (PhotonNetwork.offlineMode)
            return;

        if (_selectCubeId == 0)
            return;

        if (lastTurnNum == _turnNum)
            return;

        //Debug.Log("Finally recording " + _turnNum);

        maxHeight[_turnNum - 1] = GameControl.MaximunHeight();
        distance[_turnNum - 1] = (GameControl.GetCube(_selectCubeId).transform.position - initialPosi).magnitude;
        FinalStability[_turnNum - 1] = GameControl.ObtainStability();


        isInitialPositionRecorded = false;
        isLastPositionRecorded = false;
        isMoveOutStabilityRecored = false;
        isComplexityRecored = false;
        tempMovement = 0f;
        lastTurnNum = _turnNum;
    }

    //OnDestroy in the future
    public static void WriteData()
    {
        //avoid duplication written
        if (isFileWritten)
            return;
        
        if (PhotonNetwork.offlineMode)
            return;

        if (_streamWriter == null)
            return;

        //if the file was initialized
        if (initialFile)
        {
            Debug.Log("Start writing the file");
            for (int i = 0; i < TurnGameManager.maxTurnNum; i++)
            {
                _streamWriter.WriteLine(turnNum[i] + "\t" + player_index[i] + "\t" + selectedCubeID[i] + "\t" + Math.Round(player_totalTime[i], 3) + "\t" + Math.Round(player_selectTime[i], 3) + "\t" + Math.Round(player_moveOutTime[i], 3) + "\t" + Math.Round(player_placeTime[i], 3) + "\t" + Math.Round(checkTime[i], 3) + "\t" + Math.Round(moveOutStability[i], 3) + "\t" + Math.Round(maxHeight[i], 3) + "\t" + Math.Round(totalMovement[i], 3) + "\t" + Math.Round(distance[i], 3) + "\t" + Math.Round(FinalStability[i], 3) + "\t" + TopComplexity[i]);
            }

            //record if this game ties (used to detect result after all 16 turns are filled)
            _streamWriter.WriteLine("Game tie: " + _tuneGameManager.isGameTie);

            //record if the checkTimeLimit was exceeded last turn to determine who wins
            _streamWriter.WriteLine("ExceedCheckTime: " + _tuneGameManager.exceedLimit);
        }

        _streamWriter.WriteLine();

        _streamWriter.Close();

        isFileWritten = true;

        Debug.Log("Data saved");
    }

    //when the object is destoryed, force to write file
    private void OnDestroy()
    {
        WriteData();
    }
}
