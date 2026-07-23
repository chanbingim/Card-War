using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableGroup
{
    public string Label { get; }
    public Dictionary<string, UnityEngine.Object> Assets { get; } = new();
    public Dictionary<string, AsyncOperationHandle> Handles { get; } = new();

    public AddressableGroup(string label)
    {
        Label = label;
    }
}