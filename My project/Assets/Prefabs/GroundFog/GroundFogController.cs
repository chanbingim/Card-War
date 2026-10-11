using UnityEngine;
using UnityEngine.Rendering;

namespace Haness.VFX
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class GroundFogController : MonoBehaviour
    {
        [Header("Required Assets")]
        [SerializeField] private Texture2D fogTexture;

        [Header("Area")]
        [SerializeField] private Vector3 areaSize = new(18f, 4f, 0.2f);
        [SerializeField, Min(1)] private int maxParticles = 42;

        [Header("Appearance")]
        [SerializeField] private Color fogColor = new(0.78f, 0.86f, 0.9f, 0.32f);
        [SerializeField] private Vector2 particleSize = new(3.5f, 6.5f);
        [SerializeField] private Vector2 lifetime = new(12f, 20f);
        [SerializeField] private int sortingOrder = 10;

        [Header("Motion")]
        [SerializeField] private Vector2 windXY = new(0.12f, 0.035f);
        [SerializeField, Range(0f, 1f)] private float turbulence = 0.18f;

        private const string ChildName = "Fog Particles (Generated)";
        private ParticleSystem particles;
        private Material runtimeMaterial;

        private void OnEnable() => Rebuild();
        private void OnValidate() => Rebuild();

        private void OnDisable()
        {
            if (runtimeMaterial == null) return;
            if (Application.isPlaying) Destroy(runtimeMaterial);
            else DestroyImmediate(runtimeMaterial);
        }

        [ContextMenu("Rebuild Ground Fog")]
        public void Rebuild()
        {
            EnsureParticleSystem();
            ConfigureParticleSystem();
            ConfigureRenderer();

            if (!particles.isPlaying)
                particles.Play();
        }

        private void EnsureParticleSystem()
        {
            Transform child = transform.Find(ChildName);
            if (child == null)
            {
                var go = new GameObject(ChildName);
                go.transform.SetParent(transform, false);
                child = go.transform;
            }

            particles = child.GetComponent<ParticleSystem>();
            if (particles == null)
                particles = child.gameObject.AddComponent<ParticleSystem>();
        }

        private void ConfigureParticleSystem()
        {
            var main = particles.main;
            main.loop = true;
            main.prewarm = true;
            main.playOnAwake = true;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = maxParticles;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime.x, lifetime.y);
            main.startSpeed = 0f;
            main.startSize = new ParticleSystem.MinMaxCurve(particleSize.x, particleSize.y);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
            main.startColor = fogColor;

            var emission = particles.emission;
            emission.enabled = true;
            emission.rateOverTime = maxParticles / Mathf.Max(1f, (lifetime.x + lifetime.y) * 0.5f);

            var shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = areaSize;

            var velocity = particles.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = windXY.x;
            velocity.y = windXY.y;
            velocity.z = 0f;

            var noise = particles.noise;
            noise.enabled = turbulence > 0.001f;
            noise.quality = ParticleSystemNoiseQuality.Medium;
            noise.strength = turbulence;
            noise.frequency = 0.16f;
            noise.scrollSpeed = 0.06f;
            noise.damping = true;

            var color = particles.colorOverLifetime;
            color.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.18f),
                    new GradientAlphaKey(0.82f, 0.72f),
                    new GradientAlphaKey(0f, 1f)
                });
            color.color = gradient;
        }

        private void ConfigureRenderer()
        {
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            // This project uses a 2D Renderer and an orthographic camera looking
            // along +Z, so horizontal XZ billboards are edge-on and invisible.
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortMode = ParticleSystemSortMode.Distance;
            renderer.sortingFudge = 1f;
            renderer.sortingOrder = sortingOrder;

            Shader shader = Shader.Find("Haness/GroundFogParticle");
            if (shader == null) return;

            if (runtimeMaterial == null || runtimeMaterial.shader != shader)
            {
                if (runtimeMaterial != null)
                {
                    if (Application.isPlaying) Destroy(runtimeMaterial);
                    else DestroyImmediate(runtimeMaterial);
                }

                runtimeMaterial = new Material(shader)
                {
                    name = "Ground Fog Runtime Material",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            runtimeMaterial.SetTexture("_BaseMap", fogTexture);
            renderer.sharedMaterial = runtimeMaterial;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.45f, 0.8f, 1f, 0.35f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, areaSize);
        }
    }
}
