using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a dramatic entrance effect when the scene loads.
/// Objects fade in, particles burst, and lights pulse.
/// </summary>
public class EntranceEffect : MonoBehaviour
{
    [Header("Timing")]
    public float fadeInDuration = 2f;
    public float delayBetweenElements = 0.1f;

    [Header("Particle Burst")]
    public bool enableBurst = true;
    public int burstParticleCount = 100;
    public float burstRadius = 3f;
    public Color burstColor = new Color(0, 1, 1, 1);

    [Header("Light Flash")]
    public bool enableFlash = true;
    public float flashIntensity = 5f;
    public float flashDuration = 0.5f;

    [Header("References")]
    public Transform monumentTarget;

    private ParticleSystem burstParticles;
    private Light flashLight;

    void Start()
    {
        StartCoroutine(PlayEntranceSequence());
    }

    IEnumerator PlayEntranceSequence()
    {
        // Setup effects
        if (enableBurst) SetupBurstParticles();
        if (enableFlash) SetupFlashLight();

        // Wait a frame for everything to initialize
        yield return null;

        // Play the flash
        if (enableFlash)
        {
            StartCoroutine(PlayFlash());
        }

        // Play the burst
        if (enableBurst && burstParticles != null)
        {
            burstParticles.Play();
        }

        // Fade in scene elements
        StartCoroutine(FadeInElements());
    }

    void SetupBurstParticles()
    {
        GameObject burstObj = new GameObject("EntranceBurst");
        burstObj.transform.SetParent(transform);

        Vector3 burstPosition = monumentTarget != null ?
            monumentTarget.position : new Vector3(0, 1.5f, 6);
        burstObj.transform.position = burstPosition;

        burstParticles = burstObj.AddComponent<ParticleSystem>();

        var main = burstParticles.main;
        main.maxParticles = burstParticleCount;
        main.startLifetime = 2f;
        main.startSpeed = 5f;
        main.startSize = 0.1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = burstColor;
        main.playOnAwake = false;
        main.loop = false;

        var emission = burstParticles.emission;
        emission.enabled = false;

        var burst = new ParticleSystem.Burst(0f, burstParticleCount);
        emission.SetBursts(new ParticleSystem.Burst[] { burst });
        emission.enabled = true;

        var shape = burstParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = burstRadius;

        var colorOverLifetime = burstParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(burstColor, 0.0f),
                new GradientColorKey(Color.white, 0.3f),
                new GradientColorKey(burstColor, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1, 0.0f),
                new GradientAlphaKey(1, 0.5f),
                new GradientAlphaKey(0, 1.0f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = burstParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0, 0.5f);
        sizeCurve.AddKey(0.3f, 1f);
        sizeCurve.AddKey(1, 0);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1, sizeCurve);

        var renderer = burstObj.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        Material particleMat = Resources.Load<Material>("Particle_URP");
        if (particleMat == null)
            particleMat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        renderer.material = particleMat;
    }

    void SetupFlashLight()
    {
        GameObject lightObj = new GameObject("EntranceFlash");
        lightObj.transform.SetParent(transform);

        Vector3 lightPosition = monumentTarget != null ?
            monumentTarget.position : new Vector3(0, 1.5f, 6);
        lightObj.transform.position = lightPosition;

        flashLight = lightObj.AddComponent<Light>();
        flashLight.type = LightType.Point;
        flashLight.color = burstColor;
        flashLight.intensity = 0;
        flashLight.range = 30f;
    }

    IEnumerator PlayFlash()
    {
        if (flashLight == null) yield break;

        float elapsed = 0;

        // Flash up
        while (elapsed < flashDuration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flashDuration * 0.3f);
            flashLight.intensity = Mathf.Lerp(0, flashIntensity, t);
            yield return null;
        }

        // Flash down
        elapsed = 0;
        while (elapsed < flashDuration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flashDuration * 0.7f);
            flashLight.intensity = Mathf.Lerp(flashIntensity, 0, t);
            yield return null;
        }

        flashLight.intensity = 0;
    }

    IEnumerator FadeInElements()
    {
        // Find all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (var renderer in renderers)
        {
            // Skip particles and this object
            if (renderer.GetComponent<ParticleSystem>() != null) continue;

            StartCoroutine(FadeInRenderer(renderer));
            yield return new WaitForSeconds(delayBetweenElements * 0.1f);
        }
    }

    IEnumerator FadeInRenderer(Renderer renderer)
    {
        if (renderer == null) yield break;

        Material mat = renderer.material;
        if (mat == null) yield break;

        // Store original color
        Color originalColor = mat.HasProperty("_BaseColor") ?
            mat.GetColor("_BaseColor") : mat.color;

        // Start transparent
        Color startColor = originalColor;
        startColor.a = 0;

        float elapsed = 0;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;

            Color currentColor = Color.Lerp(startColor, originalColor, t);

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", currentColor);
            else
                mat.color = currentColor;

            yield return null;
        }

        // Ensure final color
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", originalColor);
        else
            mat.color = originalColor;
    }
}
