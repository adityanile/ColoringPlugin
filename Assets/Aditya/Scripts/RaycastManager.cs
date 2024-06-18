using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;

public class RaycastManager : MonoBehaviour
{
    RaycastHit hits;
    Ray ray;

    [SerializeField]
    private bool didIt = false;
    public GameObject lastCollided;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                didIt = false;
            }
        }

        if (Physics.Raycast(ray, out hits))
        {
            if (!hits.collider.name.Equals("Bg"))
            {
                if (!didIt)
                {
                    didIt = true;
                    lastCollided = hits.collider.gameObject;

                    //var frameManager = hits.collider.GetComponent<FrameManager>();
                    //frameManager.WhenHit();
                    return;
                }

                // If Afterward it collides with new object then
                if (!hits.collider.name.Equals(lastCollided.name))
                {
                    lastCollided.GetComponent<PaintManager>().enabled = false;
                    //var lastframeManager = lastCollided.GetComponent<FrameManager>();
                    //lastframeManager.DisableColoring();
                    lastCollided = hits.collider.gameObject;

                    //var frameManager = hits.collider.GetComponent<FrameManager>();
                    //frameManager.WhenHit();

                    hits.collider.GetComponent<PaintManager>().enabled = true;

                    return;
                }
            }

            ray = new Ray();
        }

    }

}
