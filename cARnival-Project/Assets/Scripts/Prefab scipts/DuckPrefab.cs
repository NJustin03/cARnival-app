
using UnityEngine;

public class DuckPrefab : MonoBehaviour
{
    public TextPrefabScript Text;

    private void Awake()
    {
        MeshRenderer m = GetComponentInChildren<MeshRenderer>();
        m.material = CosmeticManager.duckMaterial.materials[0];
    }
    public void ConfigureDuck(string word)
    {
        SetActive(true);
        Text.Text = word;
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

}
