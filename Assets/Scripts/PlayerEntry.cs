using UnityEngine;
using TMPro;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text rankText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;

    public void Set(string rank, string name, int score)
    {
        rankText.text = rank;
        nameText.text = name;
        scoreText.text = score.ToString();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
