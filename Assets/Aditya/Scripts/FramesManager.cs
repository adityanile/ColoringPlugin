using IDZBase.Core.GameTemplates.Coloring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    private void Start()
    {
        StartCoroutine(Initailise());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    IEnumerator Initailise()
    {
        foreach (var part in paintables)
        {
            var paintManager = part.GetComponent<PaintManager>();
            registeredPM.Add(paintManager);

            var eventTrigger = part.AddComponent<EventTrigger>();

            // Initially for covers deactivate painting
            if (part.gameObject.CompareTag("Cover"))
            {
                paintManager.PaintObject.ProcessInput = false;
                paintManager.PaintObject.FinishPainting();
            }

            var pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            pointerDownEntry.callback.AddListener(eventData =>
            {
                OnPointerDown.Invoke(registeredPM);

                foreach (var pm in registeredPM)
                {
                    InitBrush(CurrentBrushData, pm);
                }
            });

            eventTrigger.triggers.Add(pointerDownEntry);

            yield return new WaitUntil(() => paintManager.Initialized);
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

            PaintController.Instance.Brush.SetColor(brushData.BrushColor);
            PaintController.Instance.Brush.Size = brushData.BrushSize;

            paintManager.SetPaintMode(PaintMode.Default);
            var settings = ((BrushTool)paintManager.ToolsManager.CurrentTool).Settings;
            settings.UsePattern = false;
        }
    }

}
