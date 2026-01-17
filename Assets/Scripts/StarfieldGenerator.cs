using UnityEngine;

/// <summary>
/// Generates a starfield background for the void/space effect.
/// Creates small glowing points that give depth to the infinite void.
/// </summary>
public class StarfieldGenerator : MonoBehaviour
{
    [Header("Star Settings")]
    public int starCount = 1000;
    public float starFieldRadius = 100f;
    public float minStarSize = 0.02f;
    public float maxStarSize = 0.1f;

    [Header("Colors")]
    public Color starColorPrimary = new Color(0, 1, 1, 1);
    public Color starColorSecondary = new Color(1, 0, 0.8f, 1);
    public Color starColorWhite = new Color(1, 1, 1, 1);

    [Header("Animation")]
    public bool twinkle = true;
    public float twinkleSpeed = 2f;

    private Transform starsParent;
    private Material starMaterial;

    void Start()
    {
        GenerateStarfield();
    }

    public void GenerateStarfield()
    {
        ClearStars();

        starsParent = new GameObject("Starfield_Generated").transform;
        starsParent.SetParent(transform);
        starsParent.localPosition = Vector3.zero;

        // Create star material
        starMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        starMaterial.SetFloat("_Surface", 0); // Opaque

        for (int i = 0; i < starCount; i++)
        {
            CreateStar(i);
        }
    }

    void ClearStars()
    {
        if (starsParent != null)
            DestroyImmediate(starsParent.gameObject);
    }

    void CreateStar(int index)
    {
        GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        star.name = $"Star_{index}";
        star.transform.SetParent(starsParent);

        // Random position on sphere surface
        Vector3 randomDir = Random.onUnitSphere;
        float distance = Random.Range(starFieldRadius * 0.5f, starFieldRadius);
        star.transform.localPosition = randomDir * distance;

        // Random size
        float size = Random.Range(minStarSize, maxStarSize);
        star.transform.localScale = Vector3.one * size;

        // Remove collider
        Destroy(star.GetComponent<Collider>());

        // Create unique material instance with random color
        Material mat = new Material(starMaterial);
        Color[] colors = { starColorPrimary, starColorSecondary, starColorWhite };
        Color starColor = colors[Random.Range(0, colors.Length)];
        starColor.a = Random.Range(0.5f, 1f);
        mat.SetColor("_BaseColor", starColor);
        star.GetComponent<Renderer>().material = mat;

        // Add twinkle effect
        if (twinkle)
        {
            var twinkler = star.AddComponent<StarTwinkle>();
            twinkler.twinkleSpeed = twinkleSpeed + Random.Range(-1f, 1f);
            twinkler.phaseOffset = Random.Range(0, Mathf.PI * 2);
        }
    }
}

/// <summary>
/// Makes stars twinkle by modulating their scale
/// </summary>
public class StarTwinkle : MonoBehaviour
{
    public float twinkleSpeed = 2f;
    public float phaseOffset = 0f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * twinkleSpeed + phaseOffset) + 1f) / 2f;
        float scale = Mathf.Lerp(0.5f, 1.2f, t);
        transform.localScale = originalScale * scale;
    }
}
