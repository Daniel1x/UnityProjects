using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ButterflyAcademy : Academy
{
    private ButterflyArea[] butterflyAreas;

    public override void AcademyReset()
    {
        if (butterflyAreas == null)
        {
            butterflyAreas = FindObjectsOfType<ButterflyArea>();
        }

        foreach(ButterflyArea butterflyArea in butterflyAreas)
        {
            butterflyArea.targetSpeed = resetParameters["target_speed"];
            butterflyArea.catchRange = resetParameters["catch_range"];
            butterflyArea.ResetArea();
        }
    }
}
