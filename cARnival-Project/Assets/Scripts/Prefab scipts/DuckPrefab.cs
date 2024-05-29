
using UnityEngine;

public class DuckPrefab : MonoBehaviour
{
    public TextPrefabScript Text;

    public void ConfigureDuck(string word, Material material)
    {
        SetActive(true);
        Text.Text = word;
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

}
