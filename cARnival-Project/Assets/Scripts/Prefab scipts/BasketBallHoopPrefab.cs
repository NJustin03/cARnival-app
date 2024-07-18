using UnityEngine;

public class BasketBallHoopPrefab : MonoBehaviour
{
    public TextPrefabScript Text;
    public ParticleSystem confetti;
    private AudioSource audioSource;
    public SpawnResultText spawnResultText;

    [SerializeField]
    private LanguageHoopsManager gameManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ConfigureHoop(string word)
    {
        SetActive(true);
        Text.Text = word;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            Debug.Log("Collider Works!!!");
            gameManager.SelectWord(this, out bool isCorrect);
            if (isCorrect)
            {
                confetti.Play();
                isCorrect = false;
                spawnResultText.AnsweredCorrect(spawnResultText.transform.position);
            }
            else
-            {
                spawnResultText.AnsweredIncorrect(spawnResultText.transform.position);
            }
        }
    }
    public void SetActive(bool active) => gameObject.SetActive(active);

    public void PlaySound() => audioSource.Play();

}
