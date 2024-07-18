
using UnityEngine;

public class DuckPrefab : MonoBehaviour
{
    public TextPrefabScript Text;
    private AudioSource audioSource;
    private void Awake()
    {
        MeshRenderer m = GetComponentInChildren<MeshRenderer>();
        m.material = CosmeticManager.duckMaterial.materials[0];
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    public void ConfigureDuck(string word)
    {
        SetActive(true);
        Text.Text = word;
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

}
