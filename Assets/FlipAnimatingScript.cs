using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipAnimatingScript : MonoBehaviour
{
    public string textFront;
    public string textBack;
    public Vector3 textScale;

    public AnimationCurve flipCurve;
    float curveAdjustment;

    // Start is called before the first frame update
    void Start()
    {
        textScale = gameObject.GetComponent<RectTransform>().localScale;
        curveAdjustment = Random.Range(0.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flipCurve.Evaluate(Time.time + curveAdjustment) > 0)
        {
            gameObject.GetComponent<Text>().text = textFront;
        }
        else
        {
            gameObject.GetComponent<Text>().text = textBack;
        }

        textScale = new Vector3(1, Mathf.Abs(flipCurve.Evaluate(Time.time + curveAdjustment)), 1);
        gameObject.GetComponent<RectTransform>().localScale = textScale;
    }
}
