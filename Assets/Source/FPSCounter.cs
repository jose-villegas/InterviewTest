using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public enum DeltaTimeType
    {
        Smooth,
        Unscaled
    }

    [SerializeField] public TMP_Text label;
    [SerializeField] public DeltaTimeType DeltaType = DeltaTimeType.Smooth;

    private Dictionary<int, string> CachedNumberStrings = new();

    private int[] frameRateSamples;
    private int cacheNumbersAmount = 300;
    private int averageFromAmount = 30;
    private int averageCounter;
    private int currentAveraged;

    void Awake()
    {
        // Cache strings and create array
        {
            for (int i = 0; i < cacheNumbersAmount; i++)
            {
                CachedNumberStrings[i] = i.ToString();
            }

            frameRateSamples = new int[averageFromAmount];
        }
    }

    void Update()
    {
        // Sample
        {
            var currentFrame = (int) Math.Round(1f / DeltaType switch
            {
                DeltaTimeType.Smooth => Time.smoothDeltaTime,
                DeltaTimeType.Unscaled => Time.unscaledDeltaTime,
                _ => Time.unscaledDeltaTime
            });
            frameRateSamples[averageCounter] = currentFrame;
        }

        // Average
        {
            var average = 0f;

            foreach (var frameRate in frameRateSamples)
            {
                average += frameRate;
            }

            currentAveraged = (int) Math.Round(average / averageFromAmount);
            averageCounter = (averageCounter + 1) % averageFromAmount;
        }

        // Assign to UI
        {
            label.text = currentAveraged switch
            {
                var x when x >= 0 && x < cacheNumbersAmount => CachedNumberStrings[x],
                var x when x >= cacheNumbersAmount => $"> {cacheNumbersAmount}",
                var x when x < 0 => "< 0",
                _ => "?"
            };
        }
    }
}