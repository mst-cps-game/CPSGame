﻿using Assets.Interfaces.Modules;
using Assets.Modules.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the water flow between modules
/// </summary>
public class WaterFlowController : MonoBehaviour
{
    public Reservoir Reservoir;
    public int MapCapacity = 0;

    private Module firstModule;

    private void Start()
    {
        Module currMod = Reservoir;

        // Count the map capacity
        while (currMod != null)
        {
            if (currMod is IHaveCapacity)
                MapCapacity += (currMod as IHaveCapacity).MaxCapacity;
            else
                MapCapacity += 1;

            currMod = currMod.NextModule;
        }
    }

    /// <summary>
    /// Makes time move (tick) forward for the modules.  Ticking time forward allows for the water to flow through
    /// the system.
    /// </summary>
    public List<WaterObject> TickModules()
    {
        List<WaterObject> waterLeaving = new List<WaterObject>();

        // Flow water through the reservoir, to start flow through everything else
        try
        {
            for (int i = 0; i < MapCapacity; i++)
            {
                this.Reservoir.Tick();
                WaterObject water = this.Reservoir.OnFlow(new WaterObject());

                var currentModule = this.Reservoir.NextModule;
                while (currentModule != null)
                {
                    currentModule.Tick();
                    water = currentModule.OnFlow(water);

                    currentModule = currentModule.NextModule;
                }

                if (water != null)
                    waterLeaving.Add(water.Copy()); // Add the water that came from the last module
            }
        }
        catch (Exception e)
        {

        }

        return waterLeaving;
    }

    /// <summary>
    /// Starts ticking time forward for the modules in regular intervals
    /// </summary>
    /// <param name="secondsBetweenTicks">The amount of time in between ticks</param>
    public void StartWaterFlow(float secondsBetweenTicks)
    {
        this.InvokeRepeating("TickModules", 0.1f, secondsBetweenTicks);
    }
}
