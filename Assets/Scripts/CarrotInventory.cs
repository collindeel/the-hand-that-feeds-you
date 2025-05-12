using UnityEngine;

public class CarrotInventory : MonoBehaviour
{
    private int carrotCount = 0;

    [Header("UI Popup Controller")]
    public CarrotPopupController popupController;  // Drag your CarrotPopup here in the Inspector

    public void AddCarrot()
    {
        carrotCount++;
        Debug.Log("Carrot collected! Total: " + carrotCount);
        if (popupController != null)
        {
            popupController.ShowPopup(carrotCount);
        }

    }
}
