using UnityEngine;
using XDPaint;
using XDPaint.Core;

public class FlipEffectManager : MonoBehaviour
{
    private Book pageManager;

    public Sprite currentSprite;
    public Sprite flipped;
    public GameObject controller;

    public FramesManager framesManager;

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
        for(int i=0; i < framesManager.registeredPM.Count; i++)
        {
            if (framesManager.registeredPM[i])
            {
                var pm = framesManager.registeredPM[i];
                pm.PaintObject.ProcessInput = false;
                pm.PaintObject.FinishPainting();
            }
            else
            {
                framesManager.registeredPM.RemoveAt(i);
            }
        }
    }
    public void ActivateAllPM()
    {
        for (int i = 0; i < framesManager.registeredPM.Count; i++)
        {
            if (framesManager.registeredPM[i])
            {
                var pm = framesManager.registeredPM[i];
                pm.PaintObject.ProcessInput = true;
            }
            else
            {
                framesManager.registeredPM.RemoveAt(i);
            }
        }
    }

    void UpdateSprite(Sprite currentSprite)
    {
        pageManager.bookPages[2] = currentSprite;
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
