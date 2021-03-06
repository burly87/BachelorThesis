﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;                
    INoiseFilter[] noiseFilters;

    public Elevation elevantion;            // height to calculate hightest and lowest point of Planet to set Colors right

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = CNoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }

        elevantion = new Elevation();
    }


    public float CalculateUnscaledElevation(Vector3 pointOnSphere)
    {
        float firstLayerValyue = 0;         // used to set noise Value of first layer to the rest
        float noise = 0;

        if (noiseFilters.Length > 0)
        {
            // set value to first Layer value
            firstLayerValyue = noiseFilters[0].Evaluate(pointOnSphere);
            if(settings.noiseLayers[0].enabled)
            {
                noise = firstLayerValyue;
            }
        }
         
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if(settings.noiseLayers[i].enabled)
            {
                // check if firstLayer is used as mask if so, set value if not set to 1
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValyue : 1;
                noise += noiseFilters[i].Evaluate(pointOnSphere) * mask;
            }
        }

        //storing lowest and highest elevation of all vertices
        elevantion.AddValue(noise);
        return noise;
    }

    /// <summary>
    /// get back correct elevation with clamp and * by PlanetRadius
    /// </summary>
    public float GetScaledElevation(float unscaledElevation)
    {
        float elevtaion = Mathf.Max(0, unscaledElevation);
        elevtaion = settings.planetRadius * ( 1+elevtaion);
        return elevtaion;
    }
}
