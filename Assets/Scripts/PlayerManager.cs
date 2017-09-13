using UnityEngine;

public class PlayerManager : MonoBehaviour {

	static string playerPrefabName = "Player";
	public Transform SpawnPosition;

	static Vector3 SpawnPos;
	static Quaternion SpawnRot;

	private void Start()
	{
		SpawnPos = SpawnPosition.position;
		SpawnRot = SpawnPosition.rotation;
	}

	public static void SpawnPlayer()
	{
	    GameObject player =	PhotonNetwork.Instantiate(playerPrefabName, SpawnPos, SpawnRot, 0);

        Debug.Log("Player spawned: " + player);
	}
}
