using UnityEngine;
using UnityEngine.Rendering;

namespace GPHive.Core
{
    public class LevelSettings : MonoBehaviour
    {
        public LevelConfig levelConfig;

        private void OnEnable()
        {
            if (levelConfig.enableVolume)
                FindObjectOfType<Volume>().profile = levelConfig.volumeProfile;


            if (levelConfig.changeSkybox)
                RenderSettings.skybox = levelConfig.skybox;

            RenderSettings.fog = levelConfig.fog;

            if (levelConfig.fog)
            {
                RenderSettings.fogColor = levelConfig.fogColor;

                if (levelConfig.fogMode == FogMode.Linear)
                {
                    RenderSettings.fogStartDistance = levelConfig.fogStart;
                    RenderSettings.fogEndDistance = levelConfig.fogEnd;
                }
                else
                    RenderSettings.fogDensity = levelConfig.fogDensity;
            }
        }

        private void Reset()
        {
            levelConfig.fogMode = FogMode.Linear;
        }
    }
}