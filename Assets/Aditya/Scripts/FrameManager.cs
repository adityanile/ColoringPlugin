using IDZBase.Core.GameTemplates.Coloring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDPaint;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Tools.Image;

public class FrameManager : MonoBehaviour
{
    public PaintManager paintManager;
    public FramesManager FramesManager;

    public void WhenHit()
    {
        paintManager.PaintObject.ProcessInput = true;
        InitBrush(FramesManager.CurrentBrushData, paintManager);
    }

    public void DisableColoring()
    {
        paintManager.PaintObject.ProcessInput = false;
        paintManager.PaintObject.FinishPainting();
    }

    private void InitBrush(BrushData brushData, PaintManager paintManager)
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
            //PaintController.Instance.UseSharedSettings = !_isBucketColoring;
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
