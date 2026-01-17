using UnityEngine;

/// <summary>
/// Refined cyberspace environment generator with intentional composition,
/// color depth, smooth animations, and professional polish.
/// </summary>
public class CyberSceneGenerator : MonoBehaviour
{
    [Header("Materials")]
    public Material neonCyan;
    public Material neonMagenta;
    public Material neonWhite;
    public Material particleMaterial;

    [Header("Composition Layers")]
    [Tooltip("Near objects: within arm's reach, high detail")]
    public int nearLayerCount = 8;
    [Tooltip("Mid objects: main visual interest zone")]
    public int midLayerCount = 15;
    [Tooltip("Far objects: background depth, larger scale")]
    public int farLayerCount = 12;

    [Header("Central Monument")]
    public float monumentScale = 2.5f;
    public Vector3 monumentPosition = new Vector3(0, 1.8f, 5);

    [Header("Animation - Smooth")]
    public float rotationSpeed = 12f;
    public float floatSpeed = 0.4f;
    public float floatAmplitude = 0.3f;

    [Header("Particles - Ambient Dust")]
    public int dustParticleCount = 300;
    public float dustSize = 0.04f;

    [Header("Particles - Energy Flow")]
    public int energyParticleCount = 150;
    public float energySize = 0.08f;

    [Header("Light Rays")]
    public int rayCount = 8;
    public float rayLength = 50f;

    private Transform objectsParent;
    private Transform monumentParent;
    private Transform particlesParent;
    private Transform raysParent;
    private Transform lightsParent;

    void Start()
    {
        GenerateScene();
    }

    public void GenerateScene()
    {
        ClearScene();
        LoadParticleMaterialIfNeeded();
        CreateParents();
        GenerateMonument();
        GenerateCompositionLayers();
        GenerateDustParticles();
        GenerateEnergyFlow();
        GenerateLightRays();
        SetupAtmosphericLighting();
    }

    void LoadParticleMaterialIfNeeded()
    {
        if (particleMaterial == null)
        {
            particleMaterial = Resources.Load<Material>("Particle_URP");
            if (particleMaterial == null)
            {
                Debug.LogWarning("Particle_URP material not found in Resources. Creating fallback.");
                particleMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            }
        }
    }

    void ClearScene()
    {
        if (objectsParent != null) DestroyImmediate(objectsParent.gameObject);
        if (monumentParent != null) DestroyImmediate(monumentParent.gameObject);
        if (particlesParent != null) DestroyImmediate(particlesParent.gameObject);
        if (raysParent != null) DestroyImmediate(raysParent.gameObject);
        if (lightsParent != null) DestroyImmediate(lightsParent.gameObject);
    }

    void CreateParents()
    {
        objectsParent = new GameObject("Objects_Generated").transform;
        objectsParent.SetParent(transform);

        monumentParent = new GameObject("Monument_Generated").transform;
        monumentParent.SetParent(transform);

        particlesParent = new GameObject("Particles_Generated").transform;
        particlesParent.SetParent(transform);

        raysParent = new GameObject("LightRays_Generated").transform;
        raysParent.SetParent(transform);

        lightsParent = new GameObject("Lights_Generated").transform;
        lightsParent.SetParent(transform);
    }

    void GenerateMonument()
    {
        // Outer frame - wireframe cube aesthetic
        CreateMonumentFrame();

        // Inner core - glowing sphere
        GameObject innerCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        innerCore.name = "Monument_Core";
        innerCore.transform.SetParent(monumentParent);
        innerCore.transform.localPosition = monumentPosition;
        innerCore.transform.localScale = Vector3.one * (monumentScale * 0.6f);
        Destroy(innerCore.GetComponent<Collider>());
        if (neonWhite != null)
            innerCore.GetComponent<Renderer>().material = neonWhite;

        var coreAnim = innerCore.AddComponent<SmoothFloat>();
        coreAnim.floatAmplitude = 0.15f;
        coreAnim.floatSpeed = 0.5f;

        // Rotating rings - gyroscope style
        CreateGyroscopeRings();

        // Orbiting accent pieces
        CreateOrbitingAccents();
    }

    void CreateMonumentFrame()
    {
        float size = monumentScale;
        float thickness = 0.08f;

        // 12 edges of a cube frame
        Vector3[] edgePositions = {
            // Bottom square
            new Vector3(0, -size/2, -size/2), new Vector3(0, -size/2, size/2),
            new Vector3(-size/2, -size/2, 0), new Vector3(size/2, -size/2, 0),
            // Top square
            new Vector3(0, size/2, -size/2), new Vector3(0, size/2, size/2),
            new Vector3(-size/2, size/2, 0), new Vector3(size/2, size/2, 0),
            // Vertical edges
            new Vector3(-size/2, 0, -size/2), new Vector3(size/2, 0, -size/2),
            new Vector3(-size/2, 0, size/2), new Vector3(size/2, 0, size/2),
        };

        Vector3[] edgeScales = {
            // Bottom square
            new Vector3(thickness, thickness, size), new Vector3(thickness, thickness, size),
            new Vector3(size, thickness, thickness), new Vector3(size, thickness, thickness),
            // Top square
            new Vector3(thickness, thickness, size), new Vector3(thickness, thickness, size),
            new Vector3(size, thickness, thickness), new Vector3(size, thickness, thickness),
            // Vertical edges
            new Vector3(thickness, size, thickness), new Vector3(thickness, size, thickness),
            new Vector3(thickness, size, thickness), new Vector3(thickness, size, thickness),
        };

        GameObject frameParent = new GameObject("Monument_Frame");
        frameParent.transform.SetParent(monumentParent);
        frameParent.transform.localPosition = monumentPosition;

        for (int i = 0; i < 12; i++)
        {
            GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            edge.name = $"Frame_Edge_{i}";
            edge.transform.SetParent(frameParent.transform);
            edge.transform.localPosition = edgePositions[i];
            edge.transform.localScale = edgeScales[i];
            Destroy(edge.GetComponent<Collider>());

            if (neonCyan != null)
                edge.GetComponent<Renderer>().material = neonCyan;
        }

        // Slow elegant rotation
        var frameAnim = frameParent.AddComponent<SmoothRotate>();
        frameAnim.rotationSpeed = new Vector3(0, rotationSpeed * 0.5f, 0);
        frameAnim.useEasing = true;
    }

    void CreateGyroscopeRings()
    {
        float[] ringScales = { 1.8f, 2.2f, 2.6f };
        Vector3[] ringRotations = { new Vector3(0, 0, 0), new Vector3(60, 0, 0), new Vector3(0, 0, 60) };
        float[] speeds = { 1f, -0.7f, 0.5f };

        for (int i = 0; i < 3; i++)
        {
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ring.name = $"Gyro_Ring_{i}";
            ring.transform.SetParent(monumentParent);
            ring.transform.localPosition = monumentPosition;
            ring.transform.localRotation = Quaternion.Euler(ringRotations[i]);
            ring.transform.localScale = new Vector3(
                monumentScale * ringScales[i],
                0.03f,
                monumentScale * ringScales[i]
            );
            Destroy(ring.GetComponent<Collider>());

            Material ringMat = (i % 2 == 0) ? neonMagenta : neonCyan;
            if (ringMat != null)
                ring.GetComponent<Renderer>().material = ringMat;

            var ringAnim = ring.AddComponent<SmoothRotate>();
            ringAnim.rotationSpeed = new Vector3(
                rotationSpeed * speeds[i] * 0.3f,
                rotationSpeed * speeds[i],
                rotationSpeed * speeds[i] * 0.2f
            );
            ringAnim.useEasing = true;
        }
    }

    void CreateOrbitingAccents()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject accent = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            accent.name = $"Orbit_Accent_{i}";
            accent.transform.SetParent(monumentParent);

            float angle = i * 90f * Mathf.Deg2Rad;
            float radius = monumentScale * 2f;
            accent.transform.localPosition = monumentPosition + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            accent.transform.localScale = Vector3.one * 0.25f;
            Destroy(accent.GetComponent<Collider>());

            Material[] mats = { neonCyan, neonMagenta, neonWhite, neonCyan };
            if (mats[i] != null)
                accent.GetComponent<Renderer>().material = mats[i];

            var orbit = accent.AddComponent<SmoothOrbit>();
            orbit.center = monumentPosition;
            orbit.orbitSpeed = 25f + i * 8f;
            orbit.bobAmplitude = 0.2f;
            orbit.bobSpeed = 0.8f;
        }
    }

    void GenerateCompositionLayers()
    {
        // NEAR LAYER: Small, detailed, within reach (2-4m)
        GenerateLayer("Near", nearLayerCount, 2f, 4f, 0.15f, 0.4f,
            new Vector2(-1f, 2.5f), neonMagenta, 0.7f);

        // MID LAYER: Main visual zone (5-12m)
        GenerateLayer("Mid", midLayerCount, 5f, 12f, 0.3f, 1.2f,
            new Vector2(-2f, 6f), null, 0.5f);

        // FAR LAYER: Background depth (15-30m)
        GenerateLayer("Far", farLayerCount, 15f, 30f, 1f, 3f,
            new Vector2(-5f, 12f), neonCyan, 0.3f);
    }

    void GenerateLayer(string layerName, int count, float minDist, float maxDist,
        float minScale, float maxScale, Vector2 heightRange, Material preferredMat, float animSpeed)
    {
        GameObject layerParent = new GameObject($"Layer_{layerName}");
        layerParent.transform.SetParent(objectsParent);

        for (int i = 0; i < count; i++)
        {
            // Distribute in front hemisphere
            float angle = Random.Range(-120f, 120f) * Mathf.Deg2Rad;
            float distance = Random.Range(minDist, maxDist);
            float height = Random.Range(heightRange.x, heightRange.y);

            Vector3 position = new Vector3(
                Mathf.Sin(angle) * distance,
                height,
                Mathf.Cos(angle) * distance + 5f
            );

            // Alternate shapes
            PrimitiveType shape = (i % 3 == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube;
            float scale = Random.Range(minScale, maxScale);

            GameObject obj = GameObject.CreatePrimitive(shape);
            obj.name = $"{layerName}_{i}";
            obj.transform.SetParent(layerParent.transform);
            obj.transform.localPosition = position;
            obj.transform.localRotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360)
            );
            obj.transform.localScale = Vector3.one * scale;
            Destroy(obj.GetComponent<Collider>());

            // Color based on layer depth
            Material mat = preferredMat;
            if (mat == null)
            {
                Material[] mats = { neonCyan, neonMagenta, neonWhite };
                mat = mats[Random.Range(0, mats.Length)];
            }
            if (mat != null)
                obj.GetComponent<Renderer>().material = mat;

            // Smooth animation
            var anim = obj.AddComponent<SmoothFloat>();
            anim.rotateSpeed = new Vector3(
                Random.Range(-8f, 8f) * animSpeed,
                Random.Range(-8f, 8f) * animSpeed,
                Random.Range(-8f, 8f) * animSpeed
            );
            anim.floatSpeed = floatSpeed * animSpeed;
            anim.floatAmplitude = floatAmplitude * (1f - (distance / maxDist) * 0.5f);
            anim.phaseOffset = Random.Range(0f, Mathf.PI * 2);
        }
    }

    void GenerateDustParticles()
    {
        GameObject dustObj = new GameObject("Dust_Particles");
        dustObj.transform.SetParent(particlesParent);
        dustObj.transform.localPosition = new Vector3(0, 2, 8);

        var ps = dustObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = dustParticleCount;
        main.startLifetime = 12f;
        main.startSpeed = 0.1f;
        main.startSize = dustSize;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Subtle color variation
        Gradient startGradient = new Gradient();
        startGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.3f, 0.8f, 1f), 0),
                new GradientColorKey(new Color(1f, 0.3f, 0.8f), 1)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.3f, 0),
                new GradientAlphaKey(0.3f, 1)
            }
        );
        main.startColor = new ParticleSystem.MinMaxGradient(startGradient);

        var emission = ps.emission;
        emission.rateOverTime = dustParticleCount / 12f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(30, 15, 30);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.05f, 0.05f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.05f, 0.05f);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient lifeGradient = new Gradient();
        lifeGradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0),
                new GradientColorKey(Color.white, 1)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(0.4f, 0.2f),
                new GradientAlphaKey(0.4f, 0.8f),
                new GradientAlphaKey(0, 1)
            }
        );
        colorOverLifetime.color = lifeGradient;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.3f;
        noise.frequency = 0.3f;

        var renderer = dustObj.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = particleMaterial;
    }

    void GenerateEnergyFlow()
    {
        GameObject energyObj = new GameObject("Energy_Flow");
        energyObj.transform.SetParent(particlesParent);
        energyObj.transform.localPosition = monumentPosition;

        var ps = energyObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = energyParticleCount;
        main.startLifetime = 4f;
        main.startSpeed = 2f;
        main.startSize = energySize;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0, 1, 1, 0.8f);

        var emission = ps.emission;
        emission.rateOverTime = energyParticleCount / 4f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = monumentScale;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1, 1, 1), 0),
                new GradientColorKey(new Color(0, 1, 1), 0.3f),
                new GradientColorKey(new Color(1, 0, 0.8f), 0.7f),
                new GradientColorKey(new Color(0, 1, 1), 1)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(1, 0.1f),
                new GradientAlphaKey(0.8f, 0.6f),
                new GradientAlphaKey(0, 1)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0, 0.3f);
        sizeCurve.AddKey(0.2f, 1f);
        sizeCurve.AddKey(0.8f, 0.8f);
        sizeCurve.AddKey(1, 0);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1, sizeCurve);

        var trails = ps.trails;
        trails.enabled = true;
        trails.lifetime = 0.3f;
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(0.5f, 0f);

        var renderer = energyObj.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.trailMaterial = particleMaterial;
        renderer.material = particleMaterial;
    }

    void GenerateLightRays()
    {
        for (int i = 0; i < rayCount; i++)
        {
            GameObject ray = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ray.name = $"LightRay_{i}";
            ray.transform.SetParent(raysParent);

            float angle = (i / (float)rayCount) * 360f * Mathf.Deg2Rad;
            float distance = 20f;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * distance,
                15f,
                Mathf.Sin(angle) * distance + 10f
            );

            ray.transform.localPosition = position;
            ray.transform.LookAt(monumentPosition);
            ray.transform.localScale = new Vector3(0.15f, 0.15f, rayLength);
            Destroy(ray.GetComponent<Collider>());

            // Gradient transparency material
            Material rayMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            Color rayColor = (i % 2 == 0) ?
                new Color(0, 1, 1, 0.08f) :
                new Color(1, 0, 0.8f, 0.06f);
            rayMat.SetColor("_BaseColor", rayColor);
            rayMat.SetFloat("_Surface", 1);
            ray.GetComponent<Renderer>().material = rayMat;

            // Subtle sway animation
            var rayAnim = ray.AddComponent<SmoothFloat>();
            rayAnim.floatAmplitude = 0.5f;
            rayAnim.floatSpeed = 0.2f;
            rayAnim.phaseOffset = i * 0.5f;
        }
    }

    void SetupAtmosphericLighting()
    {
        // Main monument light - cyan
        CreatePointLight("MonumentLight_Main", monumentPosition, new Color(0, 1, 1), 2.5f, 20f, true);

        // Accent light - magenta
        CreatePointLight("MonumentLight_Accent", monumentPosition + Vector3.up * 2, new Color(1, 0, 0.8f), 1.5f, 15f, true);

        // Rim lights for depth
        CreatePointLight("RimLight_Left", new Vector3(-8, 3, 10), new Color(0, 0.8f, 1), 1f, 12f, false);
        CreatePointLight("RimLight_Right", new Vector3(8, 3, 10), new Color(1, 0.2f, 0.8f), 1f, 12f, false);
    }

    void CreatePointLight(string name, Vector3 position, Color color, float intensity, float range, bool pulse)
    {
        GameObject lightObj = new GameObject(name);
        lightObj.transform.SetParent(lightsParent);
        lightObj.transform.localPosition = position;

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;

        if (pulse)
        {
            var pulser = lightObj.AddComponent<SmoothPulse>();
            pulser.minIntensity = intensity * 0.7f;
            pulser.maxIntensity = intensity * 1.3f;
            pulser.pulseSpeed = 0.8f;
        }
    }
}

// ============================================
// SMOOTH ANIMATION COMPONENTS
// ============================================

/// <summary>
/// Smooth floating and rotating with easing
/// </summary>
public class SmoothFloat : MonoBehaviour
{
    public Vector3 rotateSpeed = Vector3.zero;
    public float floatSpeed = 0.5f;
    public float floatAmplitude = 0.3f;
    public float phaseOffset = 0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Smooth rotation
        if (rotateSpeed != Vector3.zero)
            transform.Rotate(rotateSpeed * Time.deltaTime);

        // Eased floating (sine wave)
        if (floatAmplitude > 0)
        {
            float t = (Time.time * floatSpeed) + phaseOffset;
            float easedY = Mathf.Sin(t) * floatAmplitude;
            transform.position = new Vector3(startPosition.x, startPosition.y + easedY, startPosition.z);
        }
    }
}

/// <summary>
/// Smooth rotation with optional easing
/// </summary>
public class SmoothRotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 15, 0);
    public bool useEasing = false;

    void Update()
    {
        Vector3 speed = rotationSpeed;
        if (useEasing)
        {
            // Subtle speed variation for organic feel
            float easeFactor = 1f + Mathf.Sin(Time.time * 0.5f) * 0.1f;
            speed *= easeFactor;
        }
        transform.Rotate(speed * Time.deltaTime);
    }
}

/// <summary>
/// Smooth orbital motion around a center point
/// </summary>
public class SmoothOrbit : MonoBehaviour
{
    public Vector3 center;
    public float orbitSpeed = 30f;
    public float bobAmplitude = 0.2f;
    public float bobSpeed = 1f;

    private float angle;
    private float radius;
    private float baseY;

    void Start()
    {
        Vector3 offset = transform.position - center;
        radius = new Vector2(offset.x, offset.z).magnitude;
        baseY = transform.position.y;
        angle = Mathf.Atan2(offset.z, offset.x);
    }

    void Update()
    {
        // Eased orbit (slight speed variation)
        float easedSpeed = orbitSpeed * (1f + Mathf.Sin(Time.time * 0.3f) * 0.05f);
        angle += easedSpeed * Mathf.Deg2Rad * Time.deltaTime;

        float x = center.x + Mathf.Cos(angle) * radius;
        float z = center.z + Mathf.Sin(angle) * radius;

        // Smooth bobbing
        float y = baseY + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;

        transform.position = new Vector3(x, y, z);
        transform.Rotate(Vector3.up, easedSpeed * 0.3f * Time.deltaTime);
    }
}

/// <summary>
/// Smooth light intensity pulsing
/// </summary>
public class SmoothPulse : MonoBehaviour
{
    public float minIntensity = 1f;
    public float maxIntensity = 2f;
    public float pulseSpeed = 1f;

    private Light targetLight;

    void Start()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (targetLight != null)
        {
            // Eased pulse (sine wave)
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) / 2f;
            // Apply easing curve for smoother transitions
            t = t * t * (3f - 2f * t); // Smoothstep
            targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
    }
}
