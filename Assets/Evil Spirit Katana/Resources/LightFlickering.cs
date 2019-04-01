using UnityEngine;
using Random = UnityEngine.Random;
using System;

namespace OldOperaParticle {
    public class LightFlickering : MonoBehaviour
    {
        public float FlickerFrequency = 1;
        public float FlickerRange = 1;
        private float Intensity;
        private float rand;
        private Light pointlight;

        private void Start()
        {
            Intensity = GetComponent<Light>().intensity;
            rand = Random.value;
            pointlight = GetComponent<Light>();
        }

        private void Update()
        {
            if (pointlight.isActiveAndEnabled == true)
            {
                pointlight.intensity = Intensity + 1f * (Mathf.PerlinNoise(rand + Time.time, rand + Time.time * FlickerFrequency) * FlickerRange);
            }
        }
    }
}
