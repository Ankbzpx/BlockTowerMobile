using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelUI : MonoBehaviour {

    public int _maxTurn = 36;
    public float _timeTurn = 90;
    public float _checkTime = 3;
    public int _dropDownValue = 0;

    [SerializeField]
    Slider maxTurnSlider;
    [SerializeField]
    Text maxTurnNumText;

    [SerializeField]
    Slider timeTurnSlider;
    [SerializeField]
    Text timeTurnNumText;

    [SerializeField]
    Slider checkTimeSlider;
    [SerializeField]
    Text checkTimeNumText;

    [SerializeField]
    Dropdown modeChooseDropDown;


    void Update()
    {

        _maxTurn = (int)maxTurnSlider.value * 2;
        maxTurnNumText.text = _maxTurn.ToString();

        _timeTurn = timeTurnSlider.value;
        timeTurnNumText.text = _timeTurn.ToString();

        _checkTime = checkTimeSlider.value;
        checkTimeNumText.text = _checkTime.ToString();

        _dropDownValue = modeChooseDropDown.value;
    }

    public void ForceMode(int index)
    {
        modeChooseDropDown.value = index;
        modeChooseDropDown.interactable = false;
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}
