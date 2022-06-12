using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityAction = UnityEngine.Events.UnityAction;
using TMPro;
using Pixelplacement;

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
