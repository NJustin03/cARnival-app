using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceMeasurer : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_Text text;
    public Transform cam;
    public Transform target;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText((target.position - cam.position).magnitude.ToString());
    }
}
