
using UnityEngine;

public class DuckPrefab : MonoBehaviour
{
    public TextPrefabScript Text;
    public MeshRenderer DuckColor;

    public void ConfigureDuck(string word, Material material)
    {
        SetActive(true);
        Text.Text = word;
        DuckColor.material = material;

    }

    public void SetActive(bool active) => gameObject.SetActive(active);

}
