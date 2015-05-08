using System.Collections.Generic;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;

namespace DigitalRune.Samples
{
  // A scene query (see documentation of ISceneQuery) which sorts the queried
  // nodes into lists as required by the DeferredGraphicsScreen.
  // (See class documentation of ISceneQuery for additional information.)
  public class SceneQueryWithLodBlending : ISceneQuery
  {
    public SceneNode ReferenceNode { get; private set; }

    public List<SceneNode> DecalNodes { get; private set; }
    public List<SceneNode> Lights { get; private set; }
    public List<SceneNode> LensFlareNodes { get; private set; }
    public List<SceneNode> SkyNodes { get; private set; }
    public List<SceneNode> FogNodes { get; private set; }

    // All other scene nodes where IsRenderable is true (e.g. MeshNodes).
    public List<SceneNode> RenderableNodes { get; private set; }


    public SceneQueryWithLodBlending()
    {
      DecalNodes = new List<SceneNode>();
      Lights = new List<SceneNode>();
      LensFlareNodes = new List<SceneNode>();
      SkyNodes = new List<SceneNode>();
      FogNodes = new List<SceneNode>();
      RenderableNodes = new List<SceneNode>();
    }


    public void Reset()
    {
      ReferenceNode = null;

      DecalNodes.Clear();
      Lights.Clear();
      LensFlareNodes.Clear();
      SkyNodes.Clear();
      FogNodes.Clear();
      RenderableNodes.Clear();
    }


    public void Set(SceneNode referenceNode, IList<SceneNode> nodes, RenderContext context)
    {
      Reset();
      ReferenceNode = referenceNode;

      if (context.LodCameraNode == null)
      {
        // Simple: No distance culling, no LOD.
        for (int i = 0; i < nodes.Count; i++)
          AddNode(nodes[i]);
      }
      else
      {
        // Advanced: Distance culling and LOD selection.
        // If the scene uses LOD, the scene query needs to evaluate the LOD 
        // conditions. The RenderContext.LodCameraNode serves as reference for 
        // distance calculations.
        for (int i = 0; i < nodes.Count; i++)
          AddNodeEx(nodes[i], context);
      }
    }


    // Sorts scene node into correct list.
    private void AddNode(SceneNode node)
    {
      if (node is DecalNode)
        DecalNodes.Add(node);
      else if (node is LightNode)
        Lights.Add(node);
      else if (node is LensFlareNode)
        LensFlareNodes.Add(node);
      else if (node is SkyNode)
        SkyNodes.Add(node);
      else if (node is FogNode)
        FogNodes.Add(node);
      else if (node.IsRenderable)
        RenderableNodes.Add(node);

      // Unsupported types are simply ignored.
    }


    // Advanced: Distance culling and LOD selection.
    private void AddNodeEx(SceneNode node, RenderContext context)
    {
      bool hasMaxDistance = Numeric.IsPositiveFinite(node.MaxDistance);
      var lodGroupNode = node as LodGroupNode;
      bool isLodNode = (lodGroupNode != null);

      float distance = 0;
      if (hasMaxDistance || isLodNode)
      {
        // ----- Calculate view-normalized distance.
        // The view-normalized distance is the distance between scene node and 
        // camera node corrected by the camera field of view. This metric is used
        // for distance culling and LOD selection.
        var cameraNode = context.LodCameraNode;
        distance = GraphicsHelper.GetViewNormalizedDistance(node, cameraNode);

        // Apply LOD bias. (The LOD bias is a factor that can be used to increase
        // or decrease the viewing distance.)
        distance *= cameraNode.LodBias * context.LodBias;
      }

      // ----- Distance Culling: Check whether scene node is within MaxDistance.
      if (hasMaxDistance && distance >= node.MaxDistance)
        return;   // Ignore scene node.

      // ----- Optional: Fade out scene node near MaxDistance to avoid popping.
      if (context.LodBlendingEnabled && node.SupportsInstanceAlpha())
      {
        float d = node.MaxDistance - distance;
        float alpha = (d < context.LodHysteresis) ? d / context.LodHysteresis : 1;
        node.SetInstanceAlpha(alpha);
      }

      if (isLodNode)
      {
        // ----- Evaluate LOD group.
        var lodSelection = lodGroupNode.SelectLod(context, distance);

        // ----- Optional: LOD Blending.
        if (lodSelection.Next != null
            && context.LodBlendingEnabled
            && lodSelection.Current.SupportsInstanceAlpha()
            && lodSelection.Next.SupportsInstanceAlpha())
        {
          // The LOD group is currently transitioning between two LODs. Both LODs
          // have an "InstanceAlpha" material parameter, i.e. we can create a 
          // smooth transition by blending between both LODs.
          // --> Render both LODs using screen door transparency (stipple patterns).
          // The current LOD (alpha = 1 - t) is faded out and the next LOD is faded 
          // in (alpha = t). 
          // The fade-in uses the regular stipple pattern and the fade-out needs to 
          // use the inverted stipple pattern. If the alpha value is negative the 
          // shader will use the inverted stipple pattern - see effect Material.fx.)
          AddSubtree(lodSelection.Current, context);
          lodSelection.Current.SetInstanceAlpha(-(1 - lodSelection.Transition));

          AddSubtree(lodSelection.Next, context);
          lodSelection.Next.SetInstanceAlpha(lodSelection.Transition);
        }
        else
        {
          // No blending between two LODs. Just show current LOD.
          if (lodSelection.Current.SupportsInstanceAlpha())
            lodSelection.Current.SetInstanceAlpha(1);

          AddSubtree(lodSelection.Current, context);
        }
      }
      else
      {
        // ----- Handle normal nodes.
        AddNode(node);
      }
    }


    // Adds the scene node including children to the lists.
    private void AddSubtree(SceneNode node, RenderContext context)
    {
      if (node.IsEnabled)
      {
        AddNodeEx(node, context);

        if (node.Children != null)
          foreach (var childNode in node.Children)
            AddSubtree(childNode, context);
      }
    }
  }
}