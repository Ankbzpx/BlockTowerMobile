using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {

    [SerializeField]
    Text roomNameText;
    public string roomName;

    Button roomButton;

    AudioManager audioManager;

    public bool Updated;

    // Use this for initialization
    void Start () {

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        roomButton = GetComponent<Button>();
        roomButton.onClick.AddListener(() => RoomItemGroup.OnClickJoinRoom(roomName));
    }

    public void PlayHoverSoundForRoomItem()
    {
        audioManager.Play("Ding");
    }

    public void SetRoomNameText(string text)
    {
        roomName = text;
        roomNameText.text = roomName;
    }

    private void OnDestroy()
    {
        roomButton.onClick.RemoveAllListeners();
    }
}
