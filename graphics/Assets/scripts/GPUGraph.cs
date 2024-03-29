using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    
    [SerializeField]
    ComputeShader computeShader = default;


    static readonly int
       positionsID = Shader.PropertyToID("_Positions"),
       resolutionID = Shader.PropertyToID("_Resolution"),
       stepID = Shader.PropertyToID("_Step"),
       timeID = Shader.PropertyToID("_Time"),
       transitionProgressID = Shader.PropertyToID("_TransitionProgress");




    [SerializeField]
    Material material = default;

    [SerializeField]
    Mesh mesh = default;

    const int maxResolution = 1000;

    [SerializeField, Range (10, maxResolution)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function = default;

    public enum TransitionMode { Cycle, Random}

    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    


    ComputeBuffer positionsBuffer;
   
   

    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;


    
   

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
           
        }
      
        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionID, resolution);
        computeShader.SetFloat(stepID, step);
        computeShader.SetFloat(timeID, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressID,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        var kernelIndex = (int)function + (int) (transitioning ? transitionFunction : function) * FunctionLibrary.FunctionCount ;
        computeShader.SetBuffer(kernelIndex, positionsID, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsID, positionsBuffer);
        material.SetFloat(stepID, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    void PickNextFunction()
        {
            function = transitionMode == TransitionMode.Cycle ? FunctionLibrary.GetNextFunctionName(function) : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }
    


}
