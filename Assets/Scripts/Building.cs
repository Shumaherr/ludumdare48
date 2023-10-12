using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Type
{
    House,
    PowerPlant,
    Mine
}

public class Building : MonoBehaviour
{
    [SerializeField] private Type type;
    [SerializeField] private int costsStone;
    [SerializeField] private List<UnityEngine.Rendering.Universal.Light2D> activeLights;
    [SerializeField] private string tooltipText;
    
    private string _tooltip;

    public string Tooltip => _tooltip;

    public int CostsStone => costsStone;

    public int CostsPeople => costsPeople;

    [SerializeField] private int costsPeople;

    private bool _isActive;
    private IEnumerator _coroutine;
    private bool _coroutineFlag;
    private GameObject tooltipGO;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (value)
            {
                foreach (var light in activeLights)
                {
                    light.enabled = true;
                }
                _coroutine = GiveResource();
                StartCoroutine(_coroutine);
            }
            else
            {
                foreach (var light in activeLights)
                {
                    light.enabled = false;
                }
                if (_coroutineFlag)
                {
                    StopCoroutine(_coroutine);
                    _coroutineFlag = false;
                }
                   
            }
        }
    }

    public Type Type => type;

    // Start is called before the first frame update
    void Start()
    {
        _isActive = false;
        _tooltip = $"{type}\nCosts stone: {costsStone}\nService staff: {costsPeople}\n{tooltipText}";
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GiveResource()
    {
        _coroutineFlag = true;
        while (_isActive)
        {
            switch (Type)
            {
                case Type.House:
                    break;
                case Type.PowerPlant:
                    GameManager.Instance.Energy += 10;
                    Utils.CreateWorldTextPopup("+10", transform.position, 1.0f);
                    break;
                case Type.Mine:
                    GameManager.Instance.Stone += 10;
                    Utils.CreateWorldTextPopup("+10", transform.position, 1.0f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    private void OnDestroy()
    {
        StopCoroutine("GiveResource");
    }
    
}