using System.Collections.Generic;
using UnityEngine;
using XDPaint;

public class FlipEffectManager : MonoBehaviour
{
    private Book pageManager;

    public Sprite currentSprite;
    public Sprite flipped;
    public GameObject controller;

    public FramesManager framesManager;
    private CoverManager coverManager;

    private List<PaintManager> deactivatedPM;

    private void Start()
    {
        pageManager = gameObject.GetComponentInChildren<Book>();
        framesManager = GameObject.Find("Colorable").GetComponent<FramesManager>();

        UpdateSprite(currentSprite);
        pageManager.bookPages[1] = flipped;

        controller.SetActive(false);
    }

    public void ApplyTexture(Texture2D currentTexture)
    {
        var sprt = Sprite.Create(currentTexture, currentSprite.rect, currentSprite.pivot);
        UpdateSprite(sprt);
    }

    // Activating n deactivating paintainingManager to avoid overpainting the object while peeling
    public void DeactivateAllPM()
    {
        deactivatedPM = new();

        for (int i = 0; i < framesManager.registeredPM.Count; i++)
        {
            if (framesManager.registeredPM[i])
            {
                var pm = framesManager.registeredPM[i];

                if (pm.PaintObject.ProcessInput)
                {
                    pm.PaintObject.ProcessInput = false;
                    pm.PaintObject.FinishPainting();
                    deactivatedPM.Add(pm);
                }
            }
            else
            {
                framesManager.registeredPM.RemoveAt(i);
            }
        }
    }
    public void ActivateAllPM()
    {
        for (int i = 0; i < deactivatedPM.Count; i++)
        {
            if (deactivatedPM[i])
            {
                var pm = deactivatedPM[i];
                pm.PaintObject.ProcessInput = true;
            }
        }
    }

    void UpdateSprite(Sprite currentSprite)
    {
        pageManager.bookPages[2] = currentSprite;
    }

    public void EnablePiece()
    {
        if (!coverManager)
        {
            coverManager = transform.parent.GetComponent<CoverManager>();
        }

        if (coverManager.basePiece)
        {
            var pm = coverManager.basePiece.GetComponent<PaintManager>();
            pm.PaintObject.ProcessInput = true;
        }
    }

    public void DisablePiece()
    {
        if (!coverManager)
        {
            coverManager = GetComponent<CoverManager>();
        }

        if (coverManager.basePiece)
        {
            var pm = coverManager.basePiece.GetComponent<PaintManager>();
            pm.PaintObject.ProcessInput = false;
            pm.PaintObject.FinishPainting();
        }
    }

    public void DisableOriginalObj()
    {
        transform.parent.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void EnableLayer()
    {
        controller.SetActive(true);
    }

    public void EnableOriginalObj()
    {
        transform.parent.GetComponent<SpriteRenderer>().enabled = true;
    }
    public void DisableLayer()
    {
        controller.SetActive(false);
    }
}
