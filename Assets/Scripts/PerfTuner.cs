using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PerfTier { Low, High }

public static class PerfTuner
{

    static Resolution nativeRes;

    static void CacheNative()
    {
        if (nativeRes.width == 0)
            nativeRes = Screen.currentResolution;
    }

    public static void Apply(PerfTier tier)
    {
        var urp = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        CacheNative();

        switch (tier)
        {
            case PerfTier.Low:
                urp.shadowDistance = 0;
                urp.renderScale = 0.7f;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                Screen.SetResolution(
                    Mathf.RoundToInt(nativeRes.width * 0.8f),
                    Mathf.RoundToInt(nativeRes.height * 0.8f),
                    FullScreenMode.ExclusiveFullScreen,
                    nativeRes.refreshRateRatio);
                break;

            case PerfTier.High:
                urp.shadowDistance = 100;
                urp.renderScale = 1f;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                QualitySettings.shadowDistance = 100;
                urp.renderScale = 1f;
                QualitySettings.anisotropicFiltering =
                    AnisotropicFiltering.ForceEnable;
                Screen.SetResolution(
                    nativeRes.width,
                    nativeRes.height,
                    FullScreenMode.ExclusiveFullScreen,
                    nativeRes.refreshRateRatio);
                break;
        }
    }
}

