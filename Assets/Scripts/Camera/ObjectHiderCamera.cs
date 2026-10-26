using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Credit to this YouTube tutorial for the basic setup: https://www.youtube.com/watch?v=mOqHVMS7-Nw
public class ObjectHiderCamera : MonoBehaviour
{
    public GameObject player;

    private HashSet<ObjectFader> objectFaders = new HashSet<ObjectFader>();

    void Update()
    {
        if (player == null) return;

        Vector3 playerDir = player.transform.position - transform.position;
        float playerDistance = playerDir.magnitude;
        playerDir.Normalize();
        Ray ray = new Ray(transform.position, playerDir);

        RaycastHit[] hits = Physics.RaycastAll(ray);

        HashSet<ObjectFader> curObjectFaders = new HashSet<ObjectFader>();

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject == null) continue;
            if (hit.collider.gameObject == player) continue;

            ObjectFader fader = hit.collider.gameObject.GetComponent<ObjectFader>();
            if (fader != null && hit.distance < playerDistance)
                curObjectFaders.Add(fader);
        }

        // fade new objects
        foreach (ObjectFader fader in curObjectFaders.Except(objectFaders))
            fader.Fade();

        // unfade old objects
        foreach (ObjectFader fader in objectFaders.Except(curObjectFaders))
            fader.Unfade();

        objectFaders = curObjectFaders;
    }
}
