using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public RectTransform numberPos;

    float lerpDuration = 3;
    float startValue = 0;
    float endValue = 1;
    float valueToLerp;
    float startPos;


    // Start is called before the first frame update
    void Start()
    {
        numberPos = GetComponent<RectTransform>();
        startPos = numberPos.position.y;
        StartCoroutine(Lerp());
    }

    private IEnumerator Lerp()
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            numberPos.localPosition = new Vector3(numberPos.localPosition.x, valueToLerp + startPos, numberPos.localPosition.z);
            yield return null;
        }
        valueToLerp = endValue;
        Destroy(gameObject);
    }
}
