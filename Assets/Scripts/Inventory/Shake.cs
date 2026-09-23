using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //[SerializeField] AnimationCurve curve;
    //[SerializeField] float shakeDuration;
    //public bool isShaking;

    //void Update()
    //{
    //    if (isShaking)
    //    {
    //        isShaking = false;
    //        StartCoroutine(Shaking());
    //    }
    //}

    //private IEnumerator Shaking()
    //{
    //        Vector3 startPos = transform.position; 
    //        float elapsedTime = 0;

    //        while (elapsedTime < shakeDuration)
    //        {
    //            elapsedTime += Time.deltaTime;
    //            float strength = curve.Evaluate(elapsedTime / shakeDuration);
    //            transform.position = startPos + Random.insideUnitSphere * strength;
    //            yield return null;
    //        }

    //        transform.position = startPos;

    //}
}
