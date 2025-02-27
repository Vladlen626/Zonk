using System;
using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class SyncChildObjects : NetworkBehaviour
{
    [Serializable]
    public struct ChildTransformData
    {
        public string childName;
        public TransformData transformData;
    }

    [SyncVar(hook = nameof(OnChildTransformsUpdated))]
    private List<ChildTransformData> childTransformsData;
    [SerializeField] private List<Transform> childTransforms;

    void Start()
    {
        if (childTransformsData == null)
        {
            childTransformsData = new List<ChildTransformData>();
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            UpdateChildTransformsData();
        }
    }

    void UpdateChildTransformsData()
    {
        List<ChildTransformData> newData = new List<ChildTransformData>();

        foreach (var child in childTransforms)
        {
            newData.Add(new ChildTransformData
            {
                childName = child.name,
                transformData = new TransformData
                {
                    position = child.localPosition,
                    rotation = child.localRotation,
                    scale = child.localScale
                }
            });
        }
        
        CmdUpdateChildTransforms(newData);
    }

    [Command]
    void CmdUpdateChildTransforms(List<ChildTransformData> newData)
    {
        childTransformsData = newData;
    }

    void OnChildTransformsUpdated(List<ChildTransformData> oldData, List<ChildTransformData> newData)
    {
        foreach (var data in newData)
        {
            Transform child = childTransforms.Find(t => t.name == data.childName);
            if (child != null)
            {
                child.localPosition = data.transformData.position;
                child.localRotation = data.transformData.rotation;
                child.localScale = data.transformData.scale;
            }
        }
    }
}