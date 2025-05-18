using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image fillImage;
    public HealthPopupController hpc;

    public void SetHealth(int intPercentage)
    {
        float proportion = intPercentage / 100f;
        fillImage.fillAmount = Mathf.Clamp01(proportion);
        hpc.ShowPopup();
    }
}

