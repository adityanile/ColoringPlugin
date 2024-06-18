using IDZBase.Core.GameTemplates.Coloring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XDPaint;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Tools.Image;

public class FramesManager : MonoBehaviour
{
    public List<GameObject> paintables;

    public UnityEvent<List<PaintManager>> OnPointerDown = new();
    public UnityEvent<PaintManager> OnPointerUp = new();
    public BrushData CurrentBrushData;

    public List<PaintManager> registeredPM = new();
    public List<Texture2D> textures;

    private void Start()
    {
        StartCoroutine(Initailise());
    }


    IEnumerator Initailise()
    {
        foreach (var part in paintables)
        {
            var paintManager = part.GetComponent<PaintManager>();
            registeredPM.Add(paintManager);
            //var paintManager = part.AddComponent<PaintManager>();
            //paintManager.ObjectForPainting = part;
            //var material = new Material(Shader.Find("XD Paint/Alpha Mask"));
            //paintManager.Material.SourceMaterial = material;
            //paintManager.Material.ShaderTextureName = "_MainTex";
            part.AddComponent<PolygonCollider2D>();

            //var frameManager = part.GetComponent<FrameManager>();
            //frameManager.paintManager = paintManager;
            //frameManager.FramesManager = this;

            Texture2D tex = part.GetComponent<SpriteRenderer>().sprite.texture;
            textures.Add(tex);

            var eventTrigger = part.AddComponent<EventTrigger>();
            var pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            pointerDownEntry.callback.AddListener(eventData =>
            {
                Debug.Log("Triggered");
                OnPointerDown.Invoke(registeredPM);
                // paintManager.PaintObject.ProcessInput = true;

                foreach (var pm in registeredPM)
                {
                    InitBrush(CurrentBrushData, pm);
                }
            });

            eventTrigger.triggers.Add(pointerDownEntry);

            //var pointerUpEntry = new EventTrigger.Entry
            //{
            //    eventID = EventTriggerType.PointerUp
            //};

            //pointerUpEntry.callback.AddListener(_ =>
            //{
            //    OnPointerUp.Invoke(paintManager);

            //    paintManager.PaintObject.ProcessInput = false;
            //    paintManager.PaintObject.FinishPainting();
            //});

            //eventTrigger.triggers.Add(pointerUpEntry);

            yield return new WaitUntil(() => paintManager.Initialized);
            // paintManager.PaintObject.ProcessInput = false;
        }
    }

    public void InitBrush(BrushData brushData, PaintManager paintManager)
    {
        if (paintManager is null) return;
        if (paintManager.ToolsManager.CurrentTool.Type != PaintTool.Brush) return;
        if (brushData.UsePattern)
        {
            PaintController.Instance.UseSharedSettings = false;
            paintManager.SetPaintMode(PaintMode.Additive);
            paintManager.Brush.SetColor(brushData.BrushColor);
            paintManager.Brush.Size = brushData.BrushSize;
            var settings = ((BrushTool)paintManager.ToolsManager.CurrentTool).Settings;
            settings.UsePattern = true;
            settings.PatternTexture = brushData.PatternTexture;
            settings.PatternScale = brushData.PatternScale;
        }
        else
        {
            PaintController.Instance.UseSharedSettings = true;
            //if (_isBucketColoring)
            //{
            //    paintManager.Brush.SetColor(brushData.BrushColor);
            //}
            //else
            {
                PaintController.Instance.Brush.SetColor(brushData.BrushColor);
                PaintController.Instance.Brush.Size = brushData.BrushSize;
            }

            paintManager.SetPaintMode(PaintMode.Default);
            var settings = ((BrushTool)paintManager.ToolsManager.CurrentTool).Settings;
            settings.UsePattern = false;
        }
    }

}
