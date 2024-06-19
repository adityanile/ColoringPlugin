using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XDPaint;

public class CoverManager : MonoBehaviour
{
    public GameObject basePiece;
    public Collider2D collider;
    private PolygonCollider2D polyCollider;

    private Vector3 screenPoint;
    private Vector3 offset;

    public Vector3 finalPos;


    private void Start()
    {
        if (gameObject.CompareTag("Cover"))
        {
            collider = GetComponent<Collider2D>();

            if (collider)
                collider.enabled = false;
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            polyCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            basePiece = collision.gameObject;

        // Deactivate Base Piece to avoid overcoloring
        var pm = basePiece.GetComponent<PaintManager>();
        pm.PaintObject.ProcessInput = false;
        pm.PaintObject.FinishPainting();
    }

    void OnMouseDown()
    {        
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        if (gameObject.CompareTag("Cover"))
        {
            polyCollider.enabled = false;
            if (collider)
                collider.enabled = true;
            var pm = gameObject.GetComponent<PaintManager>();
            pm.PaintObject.ProcessInput = true;
        }

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = finalPos;
    }

}
