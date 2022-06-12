using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void CallDelayed(System.Action action, float delay)
    {
        Game.instance.StartCoroutine(CallDelayed_Coroutine(action, delay));
    }

    static IEnumerator CallDelayed_Coroutine(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
