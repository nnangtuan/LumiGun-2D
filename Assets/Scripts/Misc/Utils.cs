using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void RunAfterDelay(MonoBehaviour monoBehavior, float delay, Action task)
    {
        monoBehavior.StartCoroutine(RunAfterDelayRoutine(delay, task));
    }
    private static IEnumerator RunAfterDelayRoutine(float delay, Action task)
    {
        yield return new WaitForSeconds(delay);
        task.Invoke();
    }
}
