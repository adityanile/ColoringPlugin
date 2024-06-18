using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XDPaint;
using XDPaint.Controllers;

public class MainManager : MonoBehaviour
{   
    public Transform texturePane;
    public Transform colorPane;
    public Transform brushPane;

    public FramesManager framesManager;
    public int patternScale = 3;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform t in colorPane)
        {
            var btn = t.GetComponent<Button>();
            var clr = t.GetComponent<Image>().color;

            btn.onClick.AddListener(() =>
            {
                framesManager.CurrentBrushData.UsePattern = false;
                framesManager.CurrentBrushData.BrushColor = clr;
            });
        }

        foreach(Transform tex in texturePane)
        {
            var btn = tex.GetComponent<Button>();
            var texture = tex.GetComponent<Image>().sprite.texture;

            btn.onClick.AddListener(() =>
            {
                framesManager.CurrentBrushData.UsePattern = true;
                framesManager.CurrentBrushData.PatternTexture = texture;
                framesManager.CurrentBrushData.PatternScale = new Vector2(3f, 3f);
            });

        }

        foreach (Transform brsh in brushPane)
        {
            var btn = brsh.GetComponent<Button>();
            var texture = brsh.GetComponent<Image>().sprite.texture;

            btn.onClick.AddListener(() =>
            {
                PaintController.Instance.Brush.SourceTexture = texture;
            });

        }

    }
}
