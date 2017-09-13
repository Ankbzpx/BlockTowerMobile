using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (!PhotonNetwork.offlineMode)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
	}

    public void ResetScene()
    {
        PhotonNetwork.Destroy(transform.root.gameObject);

        Cube.selectNumEachTurn = 0;
        Cube.cubeFallGround = 0;
        Cube.currentCubeID = 0;
        Cube.globalID = 0;
        TurnGameManager.turnNum = 1;
        Player.playerGlobalID = 0;

        GameControl.PlayerList.Clear();
        GameControl.Cubes.Clear();
        GameControl.playerRegistered = 0;

        SceneManager.LoadScene(2);
    }
}
