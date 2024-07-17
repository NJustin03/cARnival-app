using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public RectTransform numberPos;

    float lerpDuration = 3;
    float startValue = 0;
    float endValue = 10000;
    float valueToLerp;


    // Start is called before the first frame update
    void Start()
    {
        numberPos = GetComponent<RectTransform>();
        StartCoroutine(Lerp());
    }

    private IEnumerator Lerp()
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            numberPos.localPosition = new Vector3(numberPos.localPosition.x, valueToLerp, numberPos.localPosition.z);
            yield return null;
        }
        valueToLerp = endValue;
        Destroy(gameObject);
    }
}
