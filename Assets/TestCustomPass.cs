using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DrawCommand
{
    public CustomPassInjectionPoint camEvt;

    public CommandBuffer cmdBuf;

    static public Dictionary<Camera, List<DrawCommand>> cBuffersRendering;

    public static void OnCommandRendered(DrawCommand cmd)
    {
    }
}

public class TestCustomPass : CustomPass
{
    CommandBuffer myCB;

protected override void Execute(CustomPassContext context)
{
    context.renderContext.ExecuteCommandBuffer(context.cmd);
    context.cmd.Clear();

    if (DrawCommand.cBuffersRendering.TryGetValue(context.hdCamera.camera, out List<DrawCommand> cmds))
    {
        for (int i = 0; i < cmds.Count; i++)
        {
            if (cmds[i].camEvt == injectionPoint)
            {
                Debug.Log($"Actually execute {context.hdCamera.camera.name}");
                context.renderContext.ExecuteCommandBuffer(cmds[i].cmdBuf);
                DrawCommand.OnCommandRendered(cmds[i]);
            }
        }
    }
}

    protected override void Execute(ScriptableRenderContext context, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
    {
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        if (DrawCommand.cBuffersRendering.TryGetValue(hdCamera.camera, out List<DrawCommand> cmds))
        {
            for (int i = 0; i < cmds.Count; i++)
            {
                if (cmds[i].camEvt == injectionPoint)
                {
                    Debug.Log($"Actually execute {hdCamera.camera.name}");
                    context.ExecuteCommandBuffer(cmds[i].cmdBuf); // this seems to render into the Distortion pass for some reason :(
                    DrawCommand.OnCommandRendered(cmds[i]);
                }
            }
        }
    }
}
