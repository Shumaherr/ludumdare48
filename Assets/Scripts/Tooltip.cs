using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
   //private RectTransform bgRectTransform;
   [SerializeField] private TextMeshProUGUI _textMeshPro;
   private RectTransform _rectTransform;
   private RectTransform canvasRectTransform;
   Camera cam;
   Vector3 min, max;

   private void Awake()
   {
      canvasRectTransform = transform.GetComponentInParent<RectTransform>();
      _rectTransform = transform.GetComponent<RectTransform>();
      gameObject.SetActive(false);
      cam = Camera.main;
      min = new Vector3(0, 0, 0);
      max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
   }

   private void SetText(string tooltipText)
   {
      _textMeshPro.SetText(tooltipText);
      _textMeshPro.ForceMeshUpdate();

      Vector2 textSize = _textMeshPro.GetRenderedValues(false);
      Vector2 textPadding = new Vector2(8,0);
      //bgRectTransform.sizeDelta = textSize + textPadding;
   }

   public void ShowTooltip(string tooltipText)
   {
      SetText(tooltipText);
      gameObject.SetActive(true);
   }

   public void HideTooltip()
   {
      gameObject.SetActive(false);
   }

   private void Update()
   {
      Vector2 newPos = Input.mousePosition / canvasRectTransform.localScale.x;
      
      Vector3 position = new Vector3(Input.mousePosition.x + canvasRectTransform.rect.width, Input.mousePosition.y - (canvasRectTransform.rect.height / 2), 0f);
      min = new Vector3(0, 0, 0);
      max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
      //clamp it to the screen size so it doesn't go outside

      /*if (newPos.x + bgRectTransform.rect.width > canvasRectTransform.rect.width)
      {
         newPos.x = canvasRectTransform.rect.width - bgRectTransform.rect.width;
      }
      if (newPos.y + bgRectTransform.rect.height > canvasRectTransform.rect.height)
      {
         newPos.y = canvasRectTransform.rect.height - bgRectTransform.rect.height;
      }*/
      _rectTransform.position = newPos;
   }
}
