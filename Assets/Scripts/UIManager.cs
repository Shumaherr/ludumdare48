using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI peopleText;
    [SerializeField] private Timer timer;

    public Timer Timer => timer;

    // Start is called before the first frame update
    void Start()
    {
        energyText.text = GameManager.Instance.Energy.ToString();
        stoneText.text = GameManager.Instance.Stone.ToString();
        GameManager.Instance.OnStoneChange += ChangeStone;
        GameManager.Instance.OnEnergyChange += ChangeEnergy;
        GameManager.Instance.OnPeopleChange += ChangePeople;
    }

    private void ChangePeople(int value)
    {
        peopleText.text = value.ToString();
    }

    private void ChangeEnergy(int value)
    {
        energyText.text = value.ToString();
    }

    private void ChangeStone(int value)
    {
        stoneText.text = value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
