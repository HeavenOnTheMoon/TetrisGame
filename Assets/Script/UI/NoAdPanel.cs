using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAdPanel : MonoBehaviour
{
    private void OnEnable()
    {
        StopCoroutine(nameof(OpenAdTitle));
        StartCoroutine(nameof(OpenAdTitle));
    }

    IEnumerator OpenAdTitle()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
