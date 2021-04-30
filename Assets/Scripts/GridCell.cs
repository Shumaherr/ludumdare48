using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private Transform _currentTransform;

    public Transform CurrentTransform
    {
        get => _currentTransform;
        set => _currentTransform = value;
    }

    private void OnSceneGUI()
    {
        Handles.Label(_currentTransform.transform.position,_currentTransform.transform.position.ToString());
    }
}
