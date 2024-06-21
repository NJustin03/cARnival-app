using UnityEngine;

public class BasketBallHoopPrefab : MonoBehaviour
{
    public TextPrefabScript Text;

    public void ConfigureHoop(string word)
    {
        SetActive(true);
        Text.Text = word;
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

}
