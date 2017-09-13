using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamepadUIEvent : MonoBehaviour
{
    //public static bool isJoyStickConnected = false;

    [SerializeField]
    GameObject DefaultUIObject;

    public void SetEventObject()
    {
        //if (isJoyStickConnected)
            EventSystem.current.SetSelectedGameObject(DefaultUIObject);
        //else
        //    return;
    }

    public static void ClearSelectedUI()
    {
            EventSystem.current.SetSelectedGameObject(null);
    }

    //IEnumerator CheckForControllers()
    //{
    //    while (true)
    //    {
    //        var controllers = Input.GetJoystickNames();
    //        if (!isJoyStickConnected && controllers.Length > 0)
    //        {
    //            isJoyStickConnected = true;

    //            if (Cursor.visible)
    //                Cursor.visible = false;
    //            Debug.Log("Connected");
    //        }
    //        else if (isJoyStickConnected && controllers.Length == 0)
    //        {
    //            isJoyStickConnected = false;
    //            if (!Cursor.visible)
    //                Cursor.visible = true;
    //            Debug.Log("Disconnected");
    //        }
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    //void Awake()
    //{
    //    StartCoroutine(CheckForControllers());
    //}
}
