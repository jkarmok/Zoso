using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Interpolation;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DirectionalLight = DigitalRune.Graphics.DirectionalLight;
using MathHelper = Microsoft.Xna.Framework.MathHelper;


namespace DigitalRune.Samples
{
  // This game object creates a sky using 
  // - a milky way skybox
  // - the 9110 stars which are visible from earth,
  // - sun and moon with simulated position and moon phase
  // - atmospheric scattering to create the sky colors
  // - cloud layers
  // This game object also creates ambient and direct light for the scene using 
  // simulated sunlight and moonlight. 
  // If the scene contains a Fog, the fog color is also updated by the simulation.
  // If the scene contains a GodRayFilter, the sun direction of the filter is also updated.
  [Controls(@"Dynamic Sky
  Hold <Up>/<Down> to change the time of day.")]
  public class DynamicSkyObject : GameObject
  {
    private readonly IServiceLocator _services;

    // This game object computes physically based light intensities. Since our material
    // properties and all other lights in this application are not physically based, 
    // we scale the light intensities down.
    private const float MilkyWayLightScale = 0.005f;
    private const float StarfieldLightScale = 0.4f;
    private const float ScatteringSkyLightScale = 0.000005f;
    private const float SunlightScale = 0.000001f;
    private const float MoonlightScale = SunlightScale * 100f;

    private static readonly Vector3F LightPollution = new Vector3F(0.004f);

    private IGraphicsService _graphicsService;
    private IInputService _inputService;
    private IScene _scene;

    private CameraObject _cameraObject;

    private SkyboxNode _milkyWayNode;
    private StarfieldNode _starfieldNode;
    private SkyObjectNode _sunNode;
    private SkyObjectNode _moonNode;
    private ScatteringSkyNode _scatteringSkyNode;
    private LightNode _ambientLightNode;
    private LightNode _sunlightNode;
    private LightNode _moonlightNode;

    private bool _enableCloudLayer;
    private CloudLayerNode _cloudLayerNode0;
    private CloudLayerNode _cloudLayerNode1;
    private LayeredCloudMap _cloudMap0;
    private LayeredCloudMap _cloudMap1;

    // We group all scene nodes of this game object under one scene node. (Optional.)
    private SceneNode _groupNode;

    private Ephemeris _ephemeris;
#if XBOX
    private DateTime _time;
#else
    private DateTimeOffset _time;
#endif


    public Vector3F SunDirection { get; private set; }

    public Vector3F SunLight { get; private set; }

    public Vector3F AmbientLight { get; private set; }


    public DynamicSkyObject(IServiceLocator services)
      : this(services, true)
    {
    }


    public DynamicSkyObject(IServiceLocator services, bool enableCloudLayer)
    {
      _services = services;
      Name = "DynamicSky";
      _enableCloudLayer = enableCloudLayer;
    }


    protected override void OnLoad()
    {
      _inputService = _services.GetInstance<IInputService>();
      _graphicsService = _services.GetInstance<IGraphicsService>();
      _scene = _services.GetInstance<IScene>();
      var content = _services.GetInstance<ContentManager>();

      var gameObjectService = _services.GetInstance<IGameObjectService>();
      _cameraObject = (CameraObject)gameObjectService.Objects["Camera"];

      // This scene node is used as the parent of all other scene nodes created here.
      _groupNode = new SceneNode
      {
        Name = "ScatteringSkyGroup",
        Children = new SceneNodeCollection()
      };
      _scene.Children.Add(_groupNode);

      // Add a skybox with milky way cube map.
      _milkyWayNode = new SkyboxNode(content.Load<TextureCube>("Sky/MilkyWay"))
      {
        Color = new Vector3F(MilkyWayLightScale),
      };
      _groupNode.Children.Add(_milkyWayNode);

      // Add a starfield with all visible stars from a star catalog.
      InitializeStarfield();
      _groupNode.Children.Add(_starfieldNode);

      // Add a node which draws a sun disk.
      _sunNode = new SkyObjectNode
      {
        GlowExponent0 = 4000,
      };
      _groupNode.Children.Add(_sunNode);

      // Add a node which draws a moon texture including moon phase.
      _moonNode = new SkyObjectNode
      {
        Texture = new PackedTexture(content.Load<Texture2D>("Sky/Moon")),
        LightWrap = 0.1f,
        LightSmoothness = 1,
        AngularDiameter = new Vector2F(MathHelper.ToRadians(5)),

        // Disable glow.
        GlowColor0 = new Vector3F(0),
        GlowColor1 = new Vector3F(0),
      };
      _groupNode.Children.Add(_moonNode);

      // Add a ScatteringSky which renders the sky colors using a physically-based model.
      _scatteringSkyNode = new ScatteringSkyNode
      {
        SunIntensity = 1,

        // Set a base color to get a dark blue instead of a pitch black night.
        BaseHorizonColor = new Vector3F(0.043f, 0.090f, 0.149f) * 0.01f,
        BaseZenithColor = new Vector3F(0.024f, 0.051f, 0.102f) * 0.01f,
        BaseColorShift = 0.5f
      };
      _groupNode.Children.Add(_scatteringSkyNode);

      if (_enableCloudLayer)
      {
        // Add a CloudLayerNode which renders dynamic clouds.
        // This layer represents dense, lit clouds at low altitude.
        _cloudMap0 = new LayeredCloudMap
        {
          Density = 15,
          Coverage = 0.5f,
          Size = 2048,
        };
        _cloudLayerNode0 = new CloudLayerNode(_cloudMap0)
        {
          SkyCurvature = 0.9f,
          TextureMatrix = CreateScale(0.5f),
          ForwardScatterExponent = 10,
          ForwardScatterScale = 10f,
          ForwardScatterOffset = 0.3f,
          NumberOfSamples = 16,
          HorizonFade = 0.03f,
        };
        // Define the layers which are combined to a single CloudMap.Texture in the CloudMapRenderer.
        var scale = CreateScale(0.2f);
        float animationScale = 0.01f;
        animationScale = 0; // Uncomment to enable formation and dissolution of clouds.
        _cloudMap0.Layers[0] = new CloudMapLayer(null, scale * CreateScale(1), -0.5f, 1, 1 * animationScale);
        _cloudMap0.Layers[1] = new CloudMapLayer(null, scale * CreateScale(1.7f), -0.5f, 1f / 2f, 5 * animationScale);
        _cloudMap0.Layers[2] = new CloudMapLayer(null, scale * CreateScale(3.97f), -0.5f, 1f / 4f, 13 * animationScale);
        _cloudMap0.Layers[3] = new CloudMapLayer(null, scale * CreateScale(8.1f), -0.5f, 1f / 8f, 27 * animationScale);
        _cloudMap0.Layers[4] = new CloudMapLayer(null, scale * CreateScale(16, 17), -0.5f, 1f / 16f, 43 * animationScale);
        _cloudMap0.Layers[5] = new CloudMapLayer(null, scale * CreateScale(32, 31), -0.5f, 1f / 32f, 77 * animationScale);
        _cloudMap0.Layers[6] = new CloudMapLayer(null, scale * CreateScale(64, 67), -0.5f, 1f / 64f,
                                                 127 * animationScale);
        _cloudMap0.Layers[7] = new CloudMapLayer(null, scale * CreateScale(128, 127), -0.5f, 1f / 64f,
                                                 200 * animationScale);
        _groupNode.Children.Add(_cloudLayerNode0);

        // Add a second CloudLayerNode which renders dynamic clouds.
        // This layer represents light clouds at high altitude.
        _cloudMap1 = new LayeredCloudMap
        {
          Density = 2,
          Coverage = 0.4f,
          Size = 1024,
          Seed = 77777,
        };
        _cloudLayerNode1 = new CloudLayerNode(_cloudMap1)
        {
          SkyCurvature = 0.9f,
          TextureMatrix = CreateScale(0.5f),
          ForwardScatterExponent = 10,
          ForwardScatterScale = 10f,
          ForwardScatterOffset = 0f,
          NumberOfSamples = 0, // No samples to disable lighting calculations.
          HorizonFade = 0.03f,
          Alpha = 0.5f, // Make these clouds more transparent.
        };
        scale = CreateScale(0.25f);
        _cloudMap1.Layers[0] = new CloudMapLayer(null, scale * CreateScale(1), -0.5f, 1, 0);
        _cloudMap1.Layers[1] = new CloudMapLayer(null, scale * CreateScale(1.7f), -0.5f, 1f / 2f, 0);
        _cloudMap1.Layers[2] = new CloudMapLayer(null, scale * CreateScale(3.97f), -0.5f, 1f / 4f, 0);
        _cloudMap1.Layers[3] = new CloudMapLayer(null, scale * CreateScale(8.1f), -0.5f, 1f / 8f, 0);
        _cloudMap1.Layers[4] = new CloudMapLayer(null, scale * CreateScale(16, 17), -0.5f, 1f / 16f, 0);
        _cloudMap1.Layers[5] = new CloudMapLayer(null, scale * CreateScale(32, 31), -0.5f, 1f / 32f, 0);
        _cloudMap1.Layers[6] = new CloudMapLayer(null, scale * CreateScale(64, 67), -0.5f, 1f / 64f, 0);
        _cloudMap1.Layers[7] = new CloudMapLayer(null, scale * CreateScale(128, 127), -0.5f, 1f / 128f, 0);
        _groupNode.Children.Add(_cloudLayerNode1);
      }

      // The following scene nodes inherit from SkyNode and are rendered by a SkyRenderer
      // in the DeferredLightingScreen. We have to set the DrawOrder to render them from
      // back to front.
      _milkyWayNode.DrawOrder = 0;
      _starfieldNode.DrawOrder = 1;
      _sunNode.DrawOrder = 2;
      _moonNode.DrawOrder = 3;
      _scatteringSkyNode.DrawOrder = 4;
      if (_enableCloudLayer)
      {
        _cloudLayerNode1.DrawOrder = 5;
        _cloudLayerNode0.DrawOrder = 6;
      }

      // Add an ambient light.
      var ambientLight = new AmbientLight
      {
        HemisphericAttenuation = 1,
      };
      _ambientLightNode = new LightNode(ambientLight)
      {
        Name = "Ambient",
      };
      _groupNode.Children.Add(_ambientLightNode);

      // Add a directional light for the sun.
      _sunlightNode = new LightNode(new DirectionalLight())
      {
        Name = "Sunlight",
        Priority = 10,   // This is the most important light.

        // This light uses Cascaded Shadow Mapping.
        Shadow = new CascadedShadow
        {
          PreferredSize = 1024,
          DepthBiasScale = new Vector4F(0.99f),
          DepthBiasOffset = new Vector4F(0),
          FadeOutDistance = 20,
          MaxDistance = 30,
          MinLightDistance = 100,
          ShadowFog = 0.5f,
          JitterResolution = 3000,
          SplitDistribution = 0.7f
        }
      };
      _groupNode.Children.Add(_sunlightNode);

      // Add a directional light for the moonlight.
      _moonlightNode = new LightNode(new DirectionalLight())
      {
        Name = "Moonlight",
        Priority = 5,
      };
      _groupNode.Children.Add(_moonlightNode);

      // The Ephemeris class computes the positions of the sun and the moon as seen
      // from any place on the earth.
      _ephemeris = new Ephemeris();

      // Seattle, Washington
      _ephemeris.Latitude = 47;
      _ephemeris.Longitude = 122;
      _ephemeris.Altitude = 100;

#if XBOX
      _time = DateTime.Now;
#else
      _time = DateTimeOffset.Now;
#endif

      // Update the positions of sky objects and the lights.
      UpdateSky();
    }


    /// <summary>
    /// Creates the texture matrix for scaling texture coordinates.
    /// </summary>
    /// <param name="s">The uniform scale factor.</param>
    /// <returns>The texture matrix.</returns>
    private static Matrix33F CreateScale(float s)
    {
      return CreateScale(s, s);
    }


    /// <summary>
    /// Creates the texture matrix for scaling the texture coordinates.
    /// </summary>
    /// <param name="su">The scale factor for u texture coordinates.</param>
    /// <param name="sv">The scale factor for v texture coordinates.</param>
    /// <returns>The texture matrix.</returns>
    private static Matrix33F CreateScale(float su, float sv)
    {
      return new Matrix33F(
        su, 0, 0,
        0, sv, 0,
        0, 0, 1);
    }


    private void InitializeStarfield()
    {
      // Load star positions and luminance from file with 9110 predefined stars.
      const int numberOfStars = 9110;
      var stars = new Star[numberOfStars];
      using (var reader = new BinaryReader(File.OpenRead("Content\\Sky\\Stars.bin")))
      {
        for (int i = 0; i < numberOfStars; i++)
        {
          stars[i].Position.X = reader.ReadSingle();
          stars[i].Position.Y = reader.ReadSingle();
          stars[i].Position.Z = reader.ReadSingle();

          // To avoid flickering, the star size should be >= 2.8 pix.
          stars[i].Size = 2.8f;

          stars[i].Color.X = reader.ReadSingle() * StarfieldLightScale;
          stars[i].Color.Y = reader.ReadSingle() * StarfieldLightScale;
          stars[i].Color.Z = reader.ReadSingle() * StarfieldLightScale;
        }

        Debug.Assert(reader.PeekChar() == -1, "End of file should be reached.");
      }

      // Note: to improve performance, we could sort stars by brightness and 
      // remove less important stars.

      _starfieldNode = new StarfieldNode();
      _starfieldNode.Color = new Vector3F(1);
      _starfieldNode.Stars = stars;
    }


    protected override void OnUnload()
    {
      _cameraObject = null;

      // Detach and dispose the grouping scene node with all children.
      _groupNode.Parent.Children.Remove(_ambientLightNode);
      _groupNode.Dispose(false);

      // Dispose cloud textures.
      if (_enableCloudLayer)
      {
        _cloudMap0.Dispose();
        _cloudMap1.Dispose();
      }

      // Set all references to null.
      _groupNode = null;
      _milkyWayNode = null;
      _starfieldNode = null;
      _sunNode = null;
      _moonNode = null;
      _scatteringSkyNode = null;
      _ambientLightNode = null;
      _sunlightNode = null;
      _moonlightNode = null;
      _cloudLayerNode0 = null;
      _cloudLayerNode1 = null;
      _cloudMap0 = null;
      _cloudMap1 = null;
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // <Up> / <Down> -> Change time of day.
      var dateDelta = TimeSpan.FromHours(0.02);
      if (_inputService.IsDown(Keys.Up))
      {
        _time += dateDelta;
        UpdateSky();
      }
      else if (_inputService.IsDown(Keys.Down))
      {
        _time -= dateDelta;
        UpdateSky();
      }

      // Scroll cloud textures to simulate wind.
      if (_enableCloudLayer)
      {
        for (int i = 0; i < 8; i++)
        {
          var matrix = _cloudMap0.Layers[i].TextureMatrix;

          // Increase translation to scroll texture. Every second layers scrolls into the
          // orthogonal direction to create a chaotic animation.
          if (i % 2 == 0)
            matrix.M02 += (float)deltaTime.TotalSeconds * (1) * 0.002f * matrix.M00;
          else
            matrix.M12 += (float)deltaTime.TotalSeconds * (1) * 0.002f * matrix.M11;

          _cloudMap0.Layers[i].TextureMatrix = matrix;
        }
        _cloudMap1 = (LayeredCloudMap)_cloudLayerNode1.CloudMap;
        for (int i = 0; i < 8; i++)
        {
          var matrix = _cloudMap1.Layers[i].TextureMatrix;

          // Increase translation to scroll texture. Every second layers scrolls into the
          // orthogonal direction to create a chaotic animation.
          if (i % 2 == 0)
            matrix.M02 += (float)deltaTime.TotalSeconds * (1) * 0.001f * matrix.M00;
          else
            matrix.M12 += (float)deltaTime.TotalSeconds * (1) * 0.001f * matrix.M11;

          _cloudMap1.Layers[i].TextureMatrix = matrix;
        }
      }
    }


    private void UpdateSky()
    {
      // Update ephemeris model.
#if XBOX
      _ephemeris.Time = new DateTimeOffset(_time.Ticks, TimeSpan.Zero);
#else
      _ephemeris.Time = _time;
#endif
      _ephemeris.Update();

      // Update rotation of milky way. We also need to add an offset because the 
      // cube map texture is rotated.
      _milkyWayNode.PoseWorld = new Pose(
        (Matrix33F)_ephemeris.EquatorialToWorld.Minor
        * Matrix33F.CreateRotationZ(ConstantsF.PiOver2 + -0.004f)
        * Matrix33F.CreateRotationX(ConstantsF.PiOver2 + -0.002f));

      // Update rotation of stars.
      _starfieldNode.PoseWorld = new Pose((Matrix33F)_ephemeris.EquatorialToWorld.Minor);

      // Update direction of sun.
      SunDirection = (Vector3F)_ephemeris.SunDirectionRefracted;
      var sunUp = SunDirection.Orthonormal1;
      _sunNode.LookAt(SunDirection, sunUp);

      // Update direction of moon.
      var moonDirection = (Vector3F)_ephemeris.MoonPosition.Normalized;
      var moonUp = (Vector3F)_ephemeris.EquatorialToWorld.TransformDirection(Vector3D.Up);
      _moonNode.LookAt(moonDirection, moonUp);

      // The moon needs to know the sun position and brightness to compute the moon phase.
      _moonNode.SunDirection = (Vector3F)_ephemeris.SunPosition.Normalized;
      _moonNode.SunLight = Ephemeris.ExtraterrestrialSunlight * SunlightScale;

      // The ScatteringSky needs the sun direction and brightness to compute the sky colors.
      _scatteringSkyNode.SunDirection = SunDirection;
      _scatteringSkyNode.SunColor = Ephemeris.ExtraterrestrialSunlight * ScatteringSkyLightScale;

      // Update the light directions.
      if (!_enableCloudLayer)
        _sunlightNode.LookAt(-SunDirection, sunUp); 

      _moonlightNode.LookAt(-moonDirection, moonUp);

      // The ScatteringSkyNode can compute the actual sunlight.
      SunLight = _scatteringSkyNode.GetSunlight();
      // Undo the ScatteringSkyLightScale and apply a custom scale for the sunlight.
      SunLight = SunLight / ScatteringSkyLightScale * SunlightScale;

      // The ScatteringSkyNode can also compute the ambient light by sampling the 
      // sky hemisphere.
      AmbientLight = _scatteringSkyNode.GetAmbientLight(256);
      AmbientLight = AmbientLight / ScatteringSkyLightScale * SunlightScale;

      // Desaturate the ambient light to avoid very blue shadows.
      AmbientLight = InterpolationHelper.Lerp(
        new Vector3F(Vector3F.Dot(AmbientLight, GraphicsHelper.LuminanceWeights)),
        AmbientLight,
        0.5f);

      // The Ephemeris model can compute the actual moonlight.
      Vector3F moonlight, moonAmbient;
      Ephemeris.GetMoonlight(
        _scatteringSkyNode.ObserverAltitude,
        2.2f,
        _ephemeris.MoonPosition,
        (float)_ephemeris.MoonPhaseAngle,
        out moonlight,
        out moonAmbient);
      moonlight *= MoonlightScale;
      moonAmbient *= MoonlightScale;

      var directionalLight = ((DirectionalLight)_sunlightNode.Light);
      directionalLight.Color = SunLight;
      ((DirectionalLight)_moonlightNode.Light).Color = moonlight;
      ((AmbientLight)_ambientLightNode.Light).Color = AmbientLight + moonAmbient + LightPollution;

      // Use the sunlight color to create a bright sun disk.
      var sunDiskColor = SunLight;
      sunDiskColor.TryNormalize();
      _sunNode.GlowColor0 = sunDiskColor * Ephemeris.ExtraterrestrialSunlight.Length * SunlightScale * 10;

      if (_enableCloudLayer)
      {
        // Update lighting info of cloud layer nodes.
        _cloudLayerNode0.SunDirection = SunDirection;
        _cloudLayerNode0.SunLight = SunLight;
        _cloudLayerNode0.AmbientLight = AmbientLight;
        // The second cloud layer uses simple unlit clouds (only ambient lighting).
        _cloudLayerNode1.SunDirection = SunDirection;
        _cloudLayerNode1.SunLight = Vector3F.Zero;
        _cloudLayerNode1.AmbientLight = AmbientLight + SunLight;

        // Use the cloud map as the texture for the directional light to create cloud shadows.
        directionalLight.Texture = _cloudMap0.Texture;
        // Compute a texture offset so that the current sun position and the projected cloud 
        // shadows match.
        // Since sky dome is always centered on the camera, position the sunlight node in 
        // line with the camera.
        var cameraPosition = _cameraObject.CameraNode.PoseWorld.Position;
        var upVector = Vector3F.AreNumericallyEqual(SunDirection, Vector3F.UnitZ) ? Vector3F.UnitY : Vector3F.UnitZ;
        _sunlightNode.LookAt(cameraPosition + SunDirection, cameraPosition, upVector);
        // Choose a scale for the cloud shadows.
        directionalLight.TextureScale = new Vector2F(-1000);
        // Get the scaled texture offset for the sun direction.
        directionalLight.TextureOffset = _cloudLayerNode0.GetTextureCoordinates(SunDirection) *
                                         directionalLight.TextureScale;
      }

      // The ScatteringSkyNode can also estimate a fog color by sampling the horizon colors.
      Vector3F fogColor = _scatteringSkyNode.GetFogColor(256);
      // Desaturate the fog color.
      fogColor = InterpolationHelper.Lerp(
        new Vector3F(Vector3F.Dot(fogColor, GraphicsHelper.LuminanceWeights)),
        fogColor,
        0.3f);

      // Find any FogNode in the scene and update its fog color.
      foreach (var fogNode in ((Scene)_scene).GetSubtree().OfType<FogNode>())
      {
        var fog = fogNode.Fog;
        fog.Color0 = fog.Color1 = new Vector4F(fogColor, 1);

        // We can use the ScatteringSymmetry to create a highlight in the sky direction.
        // To make the fog color more interesting we set higher forward scattering for
        // red and less for blue.
        fog.ScatteringSymmetry = new Vector3F(0.26f, 0.24f, 0.22f);
      }

      // TODO: If the fog is dense, reduce the direct light and increase the ambient light.
    }
  }
}
