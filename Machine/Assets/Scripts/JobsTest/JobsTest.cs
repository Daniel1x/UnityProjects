using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System;

public class JobsTest : MonoBehaviour
{
    [SerializeField] private bool useJobs = false;

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = SomeHardTaskJob();
                jobHandles.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandles);
            jobHandles.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                SomeHardTask();
            }
        }
        Debug.Log(1000f * (Time.realtimeSinceStartup - startTime) + " ms.");
    }

    private void SomeHardTask()
    {
        float value = 0.001f;
        for(int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle SomeHardTaskJob()
    {
        HardTaskJob job = new HardTaskJob();
        return job.Schedule();
    }

}

[BurstCompile]
public struct HardTaskJob : IJob
{
    public void Execute()
    {
        float value = 0.001f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
