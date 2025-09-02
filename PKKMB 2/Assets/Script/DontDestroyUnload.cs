using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyUnload : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(DelayDontDestroy());
    }

    private IEnumerator DelayDontDestroy()
    {
        // tunggu 6 detik
        yield return new WaitForSeconds(6f);

        // ambil semua GameObject aktif di scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject everyObject in allObjects)
        {
            if (everyObject.name == "Directional Light")
            {
                 continue;
            }
            DontDestroyOnLoad(everyObject);
        }
    }
}
