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
    private bool _isActive;
    private IEnumerator _coroutine;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (value)
            {
                _coroutine = GiveResource();
                StartCoroutine(_coroutine);
            }
            else
            {
                StopCoroutine(_coroutine);
            }
        }
    }

    public Type Type => type;

    // Start is called before the first frame update
    void Start()
    {
        _isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GiveResource()
    {
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
