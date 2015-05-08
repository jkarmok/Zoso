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
using DirectionalLight = DigitalRune.Graphics.DirectionalLight;


namespace DigitalRune.Samples
{
  // Implements a deferred lighting render pipeline, supporting lights and shadows,
  // Screen Space Ambient Occlusion (SSAO), High Dynamic Range (HDR) lighting, sky
  // rendering, post-processing, ...
  // The intermediate render targets (G-buffer, light buffer, shadow masks) can be
  // visualized for debugging.
  // Beginners can use this graphics screen as it is. Advanced developers can adapt
  // the render pipeline to their needs.
  sealed class DeferredGraphicsScreen : GraphicsScreen, IDisposable
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private readonly SpriteBatch _spriteBatch;
    private readonly MeshRenderer _meshRenderer;
    private readonly DecalRenderer _decalRenderer;
    private readonly BillboardRenderer _billboardRenderer;
    private readonly CloudMapRenderer _cloudMapRenderer;
    private readonly ShadowMapRenderer _shadowMapRenderer;
    private readonly ShadowMaskRenderer _shadowMaskRenderer;
    private readonly GBufferRenderer _gBufferRenderer;
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

    public LightBufferRenderer LightBufferRenderer { get; private set; }
    public SceneRenderer AlphaBlendSceneRenderer { get; private set; }

    // The active camera used to render the scene. This property must be set by
    // the samples. The default value is null.
    public CameraNode ActiveCameraNode { get; set; }

    public Scene Scene { get; private set; }

    public PostProcessorChain PostProcessors { get; private set; }

    // A debug renderer which can be used by the samples and game objects.
    // (Note: DebugRenderer.Clear() is not called automatically.)
    public DebugRenderer DebugRenderer { get; private set; }

    public bool VisualizeIntermediateRenderTargets { get; set; }

    public bool EnableLod { get; set; }

    public bool EnableSoftParticles
    {
      get { return _billboardRenderer.EnableSoftParticles; }
      set { _billboardRenderer.EnableSoftParticles = value; }
    }

    public bool EnableOffscreenParticles
    {
      get { return _billboardRenderer.EnableOffscreenRendering; }
      set { _billboardRenderer.EnableOffscreenRendering = value; }
    }

    public bool DrawReticle { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    public DeferredGraphicsScreen(IServiceLocator services)
      : base(services.GetInstance<IGraphicsService>())
    {
      _spriteBatch = new SpriteBatch(GraphicsService.GraphicsDevice);

      // Let's create the necessary scene node renderers:
      // The current sample contains MeshNodes (opaque and transparent), DecalNodes
      // and ParticleSystemNodes (transparent).
      _meshRenderer = new MeshRenderer();
      _decalRenderer = new DecalRenderer(GraphicsService);
      _billboardRenderer = new BillboardRenderer(GraphicsService, 2048)
      {
        EnableSoftParticles = true,

        // If you have an extreme amount of particles that cover the entire screen,
        // you can turn on offscreen rendering to improve performance.
        //EnableOffscreenRendering = true,
      };

      // The _alphaBlendSceneRenderer combines all renderers for transparent
      // (= alpha blended) objects.
      AlphaBlendSceneRenderer = new SceneRenderer();
      AlphaBlendSceneRenderer.Renderers.Add(_meshRenderer);
      AlphaBlendSceneRenderer.Renderers.Add(_billboardRenderer);

      // Renderer for cloud maps. (Only necessary if LayeredCloudMaps are used.)
      _cloudMapRenderer = new CloudMapRenderer(GraphicsService);

      // Shadows
      _shadowMapRenderer = new ShadowMapRenderer(_meshRenderer);
      _shadowMaskRenderer = new ShadowMaskRenderer(GraphicsService, 2);

      // Renderers which create the intermediate render targets:
      // Those 2 renderers are implemented in this sample. Those functions could
      // be implemented directly in this class but we have created separate classes
      // to make the code more readable.
      _gBufferRenderer = new GBufferRenderer(GraphicsService, _meshRenderer, _decalRenderer);
      LightBufferRenderer = new LightBufferRenderer(GraphicsService);

      // Other specialized renderers:
      _lensFlareRenderer = new LensFlareRenderer(GraphicsService, _spriteBatch);
      _skyRenderer = new SkyRenderer(GraphicsService);
      _fogRenderer = new FogRenderer(GraphicsService);
      _internalDebugRenderer = new DebugRenderer(GraphicsService, _spriteBatch, null);
      _rebuildZBufferRenderer = new RebuildZBufferRenderer(GraphicsService);

      Scene = new Scene();

      // This screen needs a HDR filter to map high dynamic range values back to
      // low dynamic range (LDR).
      PostProcessors = new PostProcessorChain(GraphicsService);
      PostProcessors.Add(new HdrFilter(GraphicsService)
      {
        EnableBlueShift = true,
        BlueShiftCenter = 0.00007f,
        BlueShiftRange = 0.5f,
        BlueShiftColor = new Vector3F(0, 0, 2f),
        MinExposure = 0,
        MaxExposure = 10,
        BloomIntensity = 1,
        BloomThreshold = 0.6f,
      });

      // Use 2D texture for reticle.
      var contentManager = services.GetInstance<ContentManager>();
      _reticle = contentManager.Load<Texture2D>("Reticle");

      // Use the sprite font of the GUI.
      var uiContentManager = services.GetInstance<ContentManager>("UIContent");
      var spriteFont = uiContentManager.Load<SpriteFont>("Default");
      DebugRenderer = new DebugRenderer(GraphicsService, _spriteBatch, spriteFont)
      {
        DefaultColor = new Color(0, 0, 0),
        DefaultTextPosition = new Vector2F(10),
      };

      EnableLod = true;
    }


    public void Dispose()
    {
      _spriteBatch.Dispose();
      _meshRenderer.Dispose();
      _decalRenderer.Dispose();
      _billboardRenderer.Dispose();
      AlphaBlendSceneRenderer.Dispose();
      _shadowMapRenderer.Dispose();
      _shadowMaskRenderer.Dispose();
      LightBufferRenderer.Dispose();
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

    // Updates the graphics screen. - This method is called by GraphicsManager.Update().
    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // The Scene class has an Update() method which must be call once per frame.
      Scene.Update(deltaTime);
    }


    // Renders the graphics screen. - This method is called in GraphicsManager.Render().
    protected override void OnRender(RenderContext context)
    {
      // Abort if no active camera is set.
      if (ActiveCameraNode == null)
        return;

      var renderTargetPool = GraphicsService.RenderTargetPool;
      var graphicsDevice = GraphicsService.GraphicsDevice;
      var screenRenderTarget = context.RenderTarget;
      var viewport = context.Viewport;

      // All intermediate render targets have the size of the target viewport.
      int width = context.Viewport.Width;
      int height = context.Viewport.Height;
      context.Viewport = new Viewport(0, 0, width, height);

      // Our scene and the camera must be set in the render context. This info is
      // required by many renderers.
      context.Scene = Scene;
      context.CameraNode = ActiveCameraNode;

      // LOD (level of detail) settings are also specified in the context.
      context.LodCameraNode = ActiveCameraNode;
      context.LodHysteresis = 0.5f;
      context.LodBias = EnableLod ? 1.0f : 0.0f;
      context.LodBlendingEnabled = false;

      // Get all scene nodes which overlap the camera frustum.
      CustomSceneQuery sceneQuery = Scene.Query<CustomSceneQuery>(ActiveCameraNode, context);

      // Generate cloud maps.
      // (Note: Only necessary if LayeredCloudMaps are used. If the cloud maps are
      // static and the settings do not change, it is not necessary to generate the
      // cloud maps in every frame. But in this example we use animated cloud maps.)
      _cloudMapRenderer.Render(sceneQuery.SkyNodes, context);

      // ----- G-Buffer Pass
      // The GBufferRenderer creates context.GBuffer0 and context.GBuffer1.
      _gBufferRenderer.Render(sceneQuery.RenderableNodes, sceneQuery.DecalNodes, context);

      // ----- Shadow Pass
      // The ShadowMapRenderer renders the shadow maps which are stored in the light nodes.
      context.RenderPass = "ShadowMap";
      _shadowMapRenderer.Render(sceneQuery.Lights, context);
      context.RenderPass = null;

      // The ShadowMaskRenderer renders the shadows and stores them in one or more render
      // targets ("shadows masks").
      _shadowMaskRenderer.Render(sceneQuery.Lights, context);

      // In this render pipeline we do not need most shadow maps anymore and can
      // recycle them. The exception is the DirectionalLight shadow map which
      // might still be needed for forward rendering of alpha-blended objects.
      foreach (var node in sceneQuery.Lights)
      {
        var lightNode = (LightNode)node;
        if (lightNode.Shadow != null && !(lightNode.Light is DirectionalLight))
        {
          renderTargetPool.Recycle(lightNode.Shadow.ShadowMap);
          lightNode.Shadow.ShadowMap = null;
        }
      }

      // ----- Light Buffer Pass
      // The LightBufferRenderer creates context.LightBuffer0 (diffuse light) and
      // context.LightBuffer1 (specular light).
      LightBufferRenderer.Render(sceneQuery.Lights, context);

      // ----- Material Pass
      // In the material pass we render all meshes and decals into a single full-screen
      // render target. The shaders combine the material properties (diffuse texture, etc.)
      // with the light buffer info.
      context.RenderTarget = renderTargetPool.Obtain2D(new RenderTargetFormat(width, height, false, SurfaceFormat.HdrBlendable, DepthFormat.Depth24Stencil8));
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

      // The meshes rendered in the last step might use additional floating-point
      // textures (e.g. the light buffers) in the different graphics texture stages.
      // We reset the texture stages (setting all GraphicsDevice.Textures to null),
      // otherwise XNA might throw exceptions.
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
      AlphaBlendSceneRenderer.Render(sceneQuery.RenderableNodes, context, RenderOrder.BackToFront);
      context.RenderPass = null;
      graphicsDevice.ResetTextures();

      // The shadow maps could be used by some shaders of the alpha-blended
      // objects - but now, we can recycle all shadow maps.
      foreach (var node in sceneQuery.Lights)
      {
        var lightNode = (LightNode)node;
        if (lightNode.Shadow != null)
        {
          renderTargetPool.Recycle(lightNode.Shadow.ShadowMap);
          lightNode.Shadow.ShadowMap = null;
        }
      }

      // ----- Post Processors
      // The post-processors modify the scene image and the result is written into
      // the final render target - which is usually the back  buffer (but this could
      // also be another off-screen render target used in another graphics screen).
      context.SourceTexture = context.RenderTarget;
      context.RenderTarget = screenRenderTarget;
      context.Viewport = viewport;
      PostProcessors.Process(context);

      renderTargetPool.Recycle((RenderTarget2D)context.SourceTexture);
      context.SourceTexture = null;

      // ----- Lens Flares
      _lensFlareRenderer.Render(sceneQuery.LensFlareNodes, context);

      // ----- Optional: Restore the Z-Buffer
      // Currently, the hardware depth buffer is not initialized with useful data because
      // every time we change the render target, XNA deletes the depth buffer. If we want
      // the debug rendering to use correct depth buffer, we can restore the depth buffer
      // using the RebuildZBufferRenderer. If we remove this step, then the DebugRenderer
      // graphics will overlay the whole 3D scene.
      _rebuildZBufferRenderer.Render(context, true);

      // ----- Debug Output
      // Render debug info added by game objects.
      DebugRenderer.Render(context);

      // ----- Draw Reticle
      if (DrawReticle)
      {
        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        _spriteBatch.Draw(
          _reticle,
          new Vector2(viewport.Width / 2 - _reticle.Width / 2, viewport.Height / 2 - _reticle.Height / 2),
          Color.Black);
        _spriteBatch.End();
      }

      // Render intermediate render targets for debugging.
      // We do not use the public DebugRenderer here because the public DebugRenderer
      // might not be cleared every frame (the game logic can choose how it wants to
      // use the public renderer).
      if (VisualizeIntermediateRenderTargets)
      {
        _internalDebugRenderer.DrawTexture(context.GBuffer0, new Rectangle(0, 0, 200, 200));
        _internalDebugRenderer.DrawTexture(context.GBuffer1, new Rectangle(200, 0, 200, 200));
        _internalDebugRenderer.DrawTexture(context.LightBuffer0, new Rectangle(400, 0, 200, 200));
        _internalDebugRenderer.DrawTexture(context.LightBuffer1, new Rectangle(600, 0, 200, 200));
        for (int i = 0; i < _shadowMaskRenderer.ShadowMasks.Count; i++)
        {
          var shadowMask = _shadowMaskRenderer.ShadowMasks[i];
          if (shadowMask != null)
            _internalDebugRenderer.DrawTexture(shadowMask, new Rectangle((i) * 200, 200, 200, 200));
        }

        _internalDebugRenderer.Render(context);
        _internalDebugRenderer.Clear();
      }

      // ----- Clean-up
      // It is very important to give every intermediate render target back to the
      // render target pool!
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
      context.Scene = null;
      context.CameraNode = null;
      context.LodHysteresis = 0;
      context.LodCameraNode = null;
      context.RenderPass = null;
    }
    #endregion
  }
}
