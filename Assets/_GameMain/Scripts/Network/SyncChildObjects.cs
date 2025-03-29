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

    private void Start()
    {
        childTransformsData ??= new List<ChildTransformData>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            UpdateChildTransformsData();
        }
    }

    void UpdateChildTransformsData()
    {
        var newData = new List<ChildTransformData>();

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
    private void CmdUpdateChildTransforms(List<ChildTransformData> newData)
    {
        childTransformsData = newData;
    }

    private void OnChildTransformsUpdated(List<ChildTransformData> oldData, List<ChildTransformData> newData)
    {
        if (isLocalPlayer) return;

        foreach (var data in newData)
        {
            var child = childTransforms.Find(t => t.name == data.childName);
            if (child == null) continue;
            child.localPosition = data.transformData.position;
            child.localRotation = data.transformData.rotation;
            child.localScale = data.transformData.scale;
        }
    }
}