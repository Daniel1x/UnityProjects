using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ButterflyAcademyToTarget : Academy
{
    private ButterflyAreaToTarget[] butterflyAreas;

    public override void AcademyReset()
    {
        if (butterflyAreas == null)
        {
            butterflyAreas = FindObjectsOfType<ButterflyAreaToTarget>();
        }

        foreach (ButterflyAreaToTarget butterflyArea in butterflyAreas)
        {
            butterflyArea.targetDistance = resetParameters["target_distance"];
            butterflyArea.catchRange = resetParameters["catch_range"];
            butterflyArea.ResetArea();
        }
    }
}
