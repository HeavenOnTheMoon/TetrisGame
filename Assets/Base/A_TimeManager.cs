using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  </summary>
public class A_TimeManager : MonoBehaviour
{
    public static A_TimeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine Delay(float delay, System.Action action)
    {
        return StartCoroutine(DelayIE(delay, action));
    }
    IEnumerator DelayIE(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
