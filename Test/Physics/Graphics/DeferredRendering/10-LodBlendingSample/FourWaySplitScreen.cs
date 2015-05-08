using System;
using DigitalRune.Graphics;
using DigitalRune.Graphics.PostProcessing;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRune.Samples.Graphics
{
  // This graphics screen renders 4 views using the same camera. Each view uses
  // different LOD settings:
  //
  //   TOP, LEFT:                        TOP, RIGHT:
  //     LODs are highlighted.             LODs are highlighted.
  //     LOD blending is disabled.         LOD blending is enabled.
  //                                    
  //   BOTTOM, LEFT:                     BOTTOM, RIGHT:
  //     Original LODs are rendered.       Original LODs are rendered.
  //     LOD blending is disabled.         LOD blending is enabled.
  //
  // Other than that, rendering is the same as in the DeferredGraphicsScreen.
  sealed class FourWaySplitScreen : GraphicsScreen
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private readonly SpriteBatch _spriteBatch;
    private readonly MeshRenderer _meshRenderer;
    private readonly SceneRenderer _transparentSceneRenderer;
    private readonly DecalRenderer _decalRenderer;
    private readonly BillboardRenderer _billboardRenderer;
    private readonly CloudMapRenderer _cloudMapRenderer;
    private readonly ShadowMapRenderer _shadowMapRenderer;
    private readonly ShadowMaskRenderer _shadowMaskRenderer;
    private readonly GBufferRenderer _gBufferRenderer;
    private readonly LightBufferRenderer _lightBufferRenderer;
    private readonly LensFlareRenderer _lensFlareRenderer;
    private readonly SkyRenderer _skyRenderer;
    private readonly FogRenderer _fogRenderer;
    private readonly DebugRenderer _internalDebugRenderer;
    private readonly RebuildZBufferRenderer _rebuildZBufferRenderer;
    private readonly Texture2D _reticle;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    public CameraNode ActiveCameraNode { get; set; }
    public Scene Scene { get; private set; }
    public PostProcessorChain PostProcessors { get; private set; }

    // A debug renderer which can be used by the samples and game objects.
    // (Note: DebugRenderer.Clear() is not called automatically.)
    public DebugRenderer DebugRenderer { get; private set; }

    public bool DrawReticle { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    public FourWaySplitScreen(IServiceLocator services)
      : base(services.GetInstance<IGraphicsService>())
    {
      _spriteBatch = new SpriteBatch(GraphicsService.GraphicsDevice);
      _meshRenderer = new MeshRenderer();
      _decalRenderer = new DecalRenderer(GraphicsService);
      _billboardRenderer = new BillboardRenderer(GraphicsService, 2048)
      {
        EnableSoftParticles = true,
      };
      _transparentSceneRenderer = new SceneRenderer();
      _transparentSceneRenderer.Renderers.Add(_meshRenderer);
      _transparentSceneRenderer.Renderers.Add(_billboardRenderer);
      _cloudMapRenderer = new CloudMapRenderer(GraphicsService);
      _shadowMapRenderer = new ShadowMapRenderer(_meshRenderer);
      _shadowMaskRenderer = new ShadowMaskRenderer(GraphicsService, 2);
      _gBufferRenderer = new GBufferRenderer(GraphicsService, _meshRenderer, _decalRenderer);
      _lightBufferRenderer = new LightBufferRenderer(GraphicsService);
      _lensFlareRenderer = new LensFlareRenderer(GraphicsService, _spriteBatch);
      _skyRenderer = new SkyRenderer(GraphicsService);
      _fogRenderer = new FogRenderer(GraphicsService);
      _internalDebugRenderer = new DebugRenderer(GraphicsService, _spriteBatch, null);
      _rebuildZBufferRenderer = new RebuildZBufferRenderer(GraphicsService);

      Scene = new Scene();

      PostProcessors = new PostProcessorChain(GraphicsService);
      PostProcessors.Add(new HdrFilter(GraphicsService)
      {
        MinExposure = 0,
        MaxExposure = 4,
        BloomIntensity = 1,
        BloomThreshold = 0.6f,
      });

      var contentManager = services.GetInstance<ContentManager>();
      _reticle = contentManager.Load<Texture2D>("Reticle");

      // Use the sprite font of the GUI.
      var uiContentManager = services.GetInstance<ContentManager>("UIContent");
      var spriteFont = uiContentManager.Load<SpriteFont>("Default");
      DebugRenderer = new DebugRenderer(GraphicsService, spriteFont)
      {
        DefaultColor = new Color(0, 0, 0),
        DefaultTextPosition = new Vector2F(10),
      };
    }


    public void Dispose()
    {
      _spriteBatch.Dispose();
      _meshRenderer.Dispose();
      _decalRenderer.Dispose();
      _billboardRenderer.Dispose();
      _transparentSceneRenderer.Dispose();
      _shadowMapRenderer.Dispose();
      _shadowMaskRenderer.Dispose();
      _lightBufferRenderer.Dispose();
      _lensFlareRenderer.Dispose();
      _skyRenderer.Dispose();
      _fogRenderer.Dispose();
      _internalDebugRenderer.Dispose();
      Scene.Dispose(false);
      PostProcessors.Dispose();
      DebugRenderer.Dispose();
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // Update the scene - this must be called each frame.
      Scene.Update(deltaTime);
    }


    protected override void OnRender(RenderContext context)
    {
      if (ActiveCameraNode == null)
        return;

      var renderTargetPool = GraphicsService.RenderTargetPool;
      var graphicsDevice = GraphicsService.GraphicsDevice;
      var originalRenderTarget = context.RenderTarget;
      var fullViewport = context.Viewport;

      // Get a render target for the first camera. Use half the width and height.
      int halfWidth = fullViewport.Width / 2;
      int halfHeight = fullViewport.Height / 2;
      var format = new RenderTargetFormat(context.RenderTarget)
      {
        Width = halfWidth,
        Height = halfHeight
      };

      var renderTarget0 = renderTargetPool.Obtain2D(format);
      var renderTarget1 = renderTargetPool.Obtain2D(format);
      var renderTarget2 = renderTargetPool.Obtain2D(format);
      var viewport0 = new Viewport(0, 0, halfWidth, halfHeight);
      var viewport1 = new Viewport(halfWidth, 0, halfWidth, halfHeight);
      var viewport2 = new Viewport(0, halfHeight, halfWidth, halfHeight);

      context.Scene = Scene;
      context.CameraNode = ActiveCameraNode;
      context.LodCameraNode = context.CameraNode;
      context.LodHysteresis = 0.5f;

      // Reduce detail level by increasing the LOD bias.
      context.LodBias = 2.0f;

      for (int i = 0; i < 4; i++)
      {
        Viewport halfViewport;
        RenderTarget2D currentRenderTarget;
        if (i == 0)
        {
          // TOP, LEFT
          currentRenderTarget = renderTarget0;
          halfViewport = new Viewport(0, 0, viewport0.Width, viewport0.Height);
          context.LodBlendingEnabled = false;
        }
        else if (i == 1)
        {
          // TOP, RIGHT
          currentRenderTarget = renderTarget1;
          halfViewport = new Viewport(0, 0, viewport1.Width, viewport1.Height);
          context.LodBlendingEnabled = true;
        }
        else if (i == 2)
        {
          // BOTTOM, LEFT
          currentRenderTarget = renderTarget2;
          halfViewport = new Viewport(0, 0, viewport2.Width, viewport2.Height);
          context.LodBlendingEnabled = false;
        }
        else
        {
          // BOTTOM, RIGHT
          currentRenderTarget = originalRenderTarget;
          halfViewport = new Viewport(fullViewport.X + halfWidth, fullViewport.Y + halfHeight, halfWidth, halfHeight);
          context.LodBlendingEnabled = true;
        }

        var sceneQuery = Scene.Query<SceneQueryWithLodBlending>(context.CameraNode, context);

        if (i == 0 || i == 1)
        {
          // TOP
          for (int j = 0; j < sceneQuery.RenderableNodes.Count; j++)
            if (sceneQuery.RenderableNodes[j].UserFlags == 1)
              sceneQuery.RenderableNodes[j] = null;
        }
        else
        {
          // BOTTOM
          for (int j = 0; j < sceneQuery.RenderableNodes.Count; j++)
            if (sceneQuery.RenderableNodes[j].UserFlags == 2)
              sceneQuery.RenderableNodes[j] = null;
        }

        // Cloud maps need to be updated only once.
        if (i == 0)
          _cloudMapRenderer.Render(sceneQuery.SkyNodes, context);

        // ----- G-Buffer Pass
        _gBufferRenderer.Render(sceneQuery.RenderableNodes, sceneQuery.DecalNodes, context);

        // ----- Shadow Pass
        context.RenderPass = "ShadowMap";
        _shadowMapRenderer.Render(sceneQuery.Lights, context);
        context.RenderPass = null;

        context.Viewport = halfViewport;
        _shadowMaskRenderer.Render(sceneQuery.Lights, context);

        // Recycle shadow maps.
        foreach (var node in sceneQuery.Lights)
        {
          var lightNode = (LightNode)node;
          if (lightNode.Shadow != null)
          {
            renderTargetPool.Recycle(lightNode.Shadow.ShadowMap);
            lightNode.Shadow.ShadowMap = null;
          }
        }

        // ----- Light Buffer Pass
        _lightBufferRenderer.Render(sceneQuery.Lights, context);

        // ----- Material Pass
        context.RenderTarget = renderTargetPool.Obtain2D(new RenderTargetFormat(
          context.Viewport.Width,
          context.Viewport.Height,
          false,
          SurfaceFormat.HdrBlendable,
          DepthFormat.Depth24Stencil8));
        graphicsDevice.SetRenderTarget(context.RenderTarget);
        context.Viewport = graphicsDevice.Viewport;
        graphicsDevice.Clear(Color.Black);
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        graphicsDevice.BlendState = BlendState.Opaque;
        context.RenderPass = "Material";
        _meshRenderer.Render(sceneQuery.RenderableNodes, context);
        _decalRenderer.Render(sceneQuery.DecalNodes, context);
        context.RenderPass = null;
        graphicsDevice.ResetTextures();

        // ----- Occlusion Queries
        _lensFlareRenderer.UpdateOcclusion(sceneQuery.LensFlareNodes, context);

        // ----- Sky
        _skyRenderer.Render(sceneQuery.SkyNodes, context);

        // ----- Fog
        _fogRenderer.Render(sceneQuery.FogNodes, context);

        // ----- Forward Rendering of Alpha-Blended Meshes and Particles
        graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        context.RenderPass = "AlphaBlend";
        _transparentSceneRenderer.Render(sceneQuery.RenderableNodes, context, RenderOrder.BackToFront);
        context.RenderPass = null;
        graphicsDevice.ResetTextures();

        // ----- Lens Flares
        _lensFlareRenderer.Render(sceneQuery.LensFlareNodes, context);

        // ----- Post Processors
        context.SourceTexture = context.RenderTarget;
        context.RenderTarget = currentRenderTarget;
        context.Viewport = halfViewport;
        PostProcessors.Process(context);

        renderTargetPool.Recycle((RenderTarget2D)context.SourceTexture);
        context.SourceTexture = null;

        // ----- Optional: Restore the Z-Buffer
        _rebuildZBufferRenderer.Render(context, true);

        // ----- Debug Output
        DebugRenderer.Render(context);

        // ----- Draw Reticle
        if (DrawReticle)
        {
          _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
          _spriteBatch.Draw(
            _reticle,
            new Vector2(halfViewport.Width / 2 - _reticle.Width / 2, halfViewport.Height / 2 - _reticle.Height / 2),
            Color.Black);
          _spriteBatch.End();
        }

        // ----- Clean-up
        renderTargetPool.Recycle(context.GBuffer0);
        context.GBuffer0 = null;
        renderTargetPool.Recycle(context.GBuffer1);
        context.GBuffer1 = null;
        renderTargetPool.Recycle((RenderTarget2D)context.Data[RenderContextKeys.DepthBufferHalf]);
        context.Data.Remove(RenderContextKeys.DepthBufferHalf);
        renderTargetPool.Recycle(context.LightBuffer0);
        context.LightBuffer0 = null;
        renderTargetPool.Recycle(context.LightBuffer1);
        context.LightBuffer1 = null;
        _shadowMaskRenderer.RecycleShadowMasks();

        sceneQuery.Reset();
      }

      // ----- Copy screens.
      // Copy the previous screens from the temporary render targets into the back buffer.
      context.Viewport = fullViewport;
      graphicsDevice.Viewport = fullViewport;

      _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
      _spriteBatch.Draw(renderTarget0, viewport0.Bounds, Color.White);
      _spriteBatch.Draw(renderTarget1, viewport1.Bounds, Color.White);
      _spriteBatch.Draw(renderTarget2, viewport2.Bounds, Color.White);
      _spriteBatch.End();

      renderTargetPool.Recycle(renderTarget0);
      renderTargetPool.Recycle(renderTarget1);
      renderTargetPool.Recycle(renderTarget2);

      context.Scene = null;
      context.CameraNode = null;
      context.LodCameraNode = null;
      context.RenderPass = null;
    }
    #endregion
  }
}
