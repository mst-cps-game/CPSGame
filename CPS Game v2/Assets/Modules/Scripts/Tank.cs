﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Module
{
    public Pump OutFlowingPump;

    public GameObject overFlowSprite;

    [Range(1, 10)]
    public int TankCapacity = 5;

    public override int Capacity { get { return TankCapacity; } }

    /// <summary>
    /// Check if inpump is on. Bring in unit of water from previous
    /// Adds water if incoming pump is on
    /// </summary>
    protected override void OnFlow()
    {
        if(this.PreviousModule)
        {
            int inFlow = (this.InFlowingPump.On) ? this.PreviousModule.Fill : 0;
            this.Fill += inFlow;
            this.PreviousModule.Fill -= inFlow;
        }
    }

    //create object behind tank to look like spilled water
    protected override void OnOverflow()
    {
        // Current Issue: Will not appear when run.
        overFlowSprite.SetActive(true);
    }
}