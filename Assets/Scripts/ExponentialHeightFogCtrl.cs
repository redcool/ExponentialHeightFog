﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentialHeightFogCtrl : MonoBehaviour
{
    [Range(0.0f,0.05f)]
    public float fogDensity = 0.02f; // This is the global density factor, which can be thought of as the fog layer's thickness.
    [Range(0.001f, 0.1f)]
    public float fogHeightFalloff = 0.2f; // Height density factor, controls how the density increases as height decreases. Smaller values make the transition larger.
    public float fogHeight = 0.0f;
    
    [Range(0.0f, 0.05f)]
    public float fogDensity2 = 0.02f;
    [Range(0.001f, 0.1f)]
    public float fogHeightFalloff2 = 0.2f;
    public float fogHeight2;

    [ColorUsage(false)]
    public Color fogInscatteringColor = new Color(0.447f, 0.639f, 1.0f); // Sets the inscattering color for the fog. Essentially, this is the fog's primary color.
    [Range(0.0f,1.0f)]
    public float fogMaxOpacity = 1.0f; // This controls the maximum opacity of the fog. A value of 1 means the fog will be completely opaque, while 0 means the fog will be essentially invisible.
    [Range(0.0f,5000.0f)]
    public float startDistance = 0.0f; // Distance from the camera that the fog will start.
    [Range(0.0f,20000000.0f)]
    public float fogCutoffDistance = 0.0f;

    public Light dirLight = null;
    [Range(2.0f,64.0f)]
    public float directionalInscatteringExponent = 4.0f; // Controls the size of the directional inscattering cone, which is used to approximate inscattering from a directional light source.

    public float directionalInscatteringStartDistance = 0.0f; // Controls the start distance from the viewer of the directional inscattering, which is used to approximate inscattering from a directional light.
    [ColorUsage(false)]
    public Color directionalInscatteringColor = new Color(0.25f, 0.25f, 0.125f); // Sets the color for directional inscattering, used to approximate inscattering from a directional light. This is similar to adjusting the simulated color of a directional light source.
    [Range(0.0f, 10.0f)]
    public float directionalInscatteringIntensity = 1.0f;

    // Update is called once per frame
    void Update()
    {
        const float USELESS_VALUE = 0.0f;

        var ExponentialFogParameters = new Vector4(RayOriginTerm(fogDensity, fogHeightFalloff, fogHeight), fogHeightFalloff, USELESS_VALUE, startDistance);
        var ExponentialFogParameters2 = new Vector4(RayOriginTerm(fogDensity2, fogHeightFalloff2, fogHeight2), fogHeightFalloff2, fogDensity2, fogHeight2);
        var ExponentialFogParameters3 = new Vector4(fogDensity, fogHeight, USELESS_VALUE, fogCutoffDistance);
        var DirectionalInscatteringColor = new Vector4(
            directionalInscatteringColor.r,
            directionalInscatteringColor.g,
            directionalInscatteringColor.b,
            directionalInscatteringExponent
        );
        var InscatteringLightDirection = new Vector4(
            -dirLight.transform.forward.x,
            -dirLight.transform.forward.y,
            -dirLight.transform.forward.z,
            directionalInscatteringStartDistance
        );
        var ExponentialFogColorParameter = new Vector4(
            directionalInscatteringIntensity * fogInscatteringColor.r,
            directionalInscatteringIntensity * fogInscatteringColor.g,
            directionalInscatteringIntensity * fogInscatteringColor.b,
            1.0f - fogMaxOpacity
        );
        
        Shader.SetGlobalVector(nameof(ExponentialFogParameters), ExponentialFogParameters);
        Shader.SetGlobalVector(nameof(ExponentialFogParameters2), ExponentialFogParameters2);
        Shader.SetGlobalVector(nameof(ExponentialFogParameters3), ExponentialFogParameters3);
        Shader.SetGlobalVector(nameof(DirectionalInscatteringColor), DirectionalInscatteringColor);
        Shader.SetGlobalVector(nameof(InscatteringLightDirection), InscatteringLightDirection);
        Shader.SetGlobalVector(nameof(ExponentialFogColorParameter), ExponentialFogColorParameter);
    }

    private static float RayOriginTerm(float density, float heightFalloff, float heightOffset)
    {
        float exponent = heightFalloff * (Camera.main.transform.position.y - heightOffset);
        return density * Mathf.Pow(2.0f, - exponent);
    }
}
