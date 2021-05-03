using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI oreText;
    // Start is called before the first frame update
    void Start()
    {
        energyText.text = GameManager.Instance.Energy.ToString();
        oreText.text = GameManager.Instance.Stone.ToString();
        GameManager.Instance.OnStoneChange += ChangeStone;
        GameManager.Instance.OnEnergyChange += ChangeEnergy;
    }

    private void ChangeEnergy(int value)
    {
        energyText.text = value.ToString();
    }

    private void ChangeStone(int value)
    {
        oreText.text = value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
