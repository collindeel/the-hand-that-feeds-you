using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [Header("Skyboxes by Episode")]
    [SerializeField] Material[] skyboxMaterials;

    [Header("Default Skybox")]
    [SerializeField] Material defaultSkybox;

    [Header("Lighting")]
    [SerializeField] Light directionalLight; // Your main sun light

    [Header("Episode 3–4 Settings")]
    [SerializeField] Color darkFogColor = new Color32(0x1A, 0x10, 0x18, 0xFF); // #1A1018
    [SerializeField] float darkFogDensity = 0.02f;
    [SerializeField] float darkLightIntensity = 0.1f;
    [SerializeField] Color darkSkyColor = new Color32(26, 11, 50, 255);
    [SerializeField] Color darkEquatorColor = new Color32(20, 15, 80, 255);
    [SerializeField] Color darkGroundColor = new Color32(0, 0, 0, 255);


    void OnEnable() => EpisodeEvents.OnEpisodeChanged += HandleEpisodeChange;
    void OnDisable() => EpisodeEvents.OnEpisodeChanged -= HandleEpisodeChange;

    void HandleEpisodeChange(EpisodeChangedArgs args)
    {
        int ep = args.episode;

        // Set Skybox
        if (ep >= 0 && ep < skyboxMaterials.Length && skyboxMaterials[ep] != null)
            RenderSettings.skybox = skyboxMaterials[ep];
        else if (defaultSkybox != null)
            RenderSettings.skybox = defaultSkybox;

        // Set fog mode
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        // Set Fog & Lighting
        if (ep == 3 || ep == 4)
        {
            RenderSettings.fogColor = darkFogColor;
            RenderSettings.fogDensity = darkFogDensity;

            RenderSettings.ambientSkyColor = darkSkyColor;
            RenderSettings.ambientEquatorColor = darkEquatorColor;
            RenderSettings.ambientGroundColor = darkGroundColor;

            if (directionalLight != null)
                directionalLight.intensity = darkLightIntensity;
        }


        RenderSettings.fog = true;
        DynamicGI.UpdateEnvironment();
    }
}
