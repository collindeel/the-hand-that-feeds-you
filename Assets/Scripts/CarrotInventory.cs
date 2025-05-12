using UnityEngine;

public class CarrotInventory : MonoBehaviour
{
    public int CarrotCount { get; set; }

    [Header("UI Popup Controller")]
    public CarrotPopupController popupController;  // Drag your CarrotPopup here in the Inspector

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && popupController != null)
        {
            popupController.ShowPopup(CarrotCount);
        }
    }
    public void AddCarrot()
    {
        CarrotCount++;
        if (popupController != null)
        {
            popupController.ShowPopup(CarrotCount);
        }
    }
    public void RemoveCarrot()
    {
        CarrotCount--;
        if (popupController != null)
        {
            popupController.ShowPopup(CarrotCount);
        }
    }
}
