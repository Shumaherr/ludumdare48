using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Building building;

    private string _tooltip;
    // Start is called before the first frame update
    void Start()
    {
        _tooltip = $"{building.Type}\nCosts stone: {building.CostsStone}\nService staff: {building.CostsPeople}\n";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.ShowTooltip(_tooltip);

    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.HideTooltip(gameObject.name);

    }
}
