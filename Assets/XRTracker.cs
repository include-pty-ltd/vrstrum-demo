using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRTracker : MonoBehaviour
{
    [
        Header("Number which the tracker is currently on."),
        Space(-10),
        Header("This will can when you restart SteamVR"),
        Space(-10),
        Header("This is a backup for serial")
    ]
    public int objectNumber;
    [Header("SteamVR tracker serial")]
    public string serial;

    private ulong uniqueId = 0;
    private List<XRNodeState> states = new List<XRNodeState>();
    private Vector3 pos = new Vector3();
    private Quaternion rot = new Quaternion();

    [Header("Move object's position")]
    public bool position;
    [Header("Move object's rotation")]
    public bool rotation;

    private void Start()
    {
        InputTracking.GetNodeStates(states);
        foreach (var state in states)
        {
            if (InputTracking.GetNodeName(state.uniqueID) == serial)
            {
                uniqueId = state.uniqueID;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputTracking.GetNodeStates(states);

        Debug.Log("There are " + states.Count + " trackers");

        //this is if we found the serial before
        if (uniqueId != 0)
        {
            foreach (var state in states)
            {
                if (state.uniqueID == uniqueId)
                {
                    if (position)
                    {
                        state.TryGetPosition(out pos);
                        transform.localPosition = pos;
                    }
                    if (rotation)
                    {
                        state.TryGetRotation(out rot);
                        transform.localRotation = transform.localRotation;
                    }
                    return;
                }
            }
        }

        //fallback if we don't find the unique id
        if (objectNumber < states.Count)
        {
            if (position)
            {
                states[objectNumber].TryGetPosition(out pos);
                transform.localPosition = pos;
            }
            if (rotation)
            {
                states[objectNumber].TryGetRotation(out rot);
                transform.localRotation = transform.localRotation;
            }
        }
    }
}
