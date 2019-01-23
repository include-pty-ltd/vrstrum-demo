using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRTracker : MonoBehaviour
{
    [
        Header("Number which the tracker is currently on."),
        Space(-10),
        Header("This can change on VR system restarted")
    ]
    public int objectNumber;

    private ulong uniqueId = 0;
    private List<XRNodeState> states = new List<XRNodeState>();
    private Vector3 pos = new Vector3();
    private Quaternion rot = new Quaternion();
    private bool foundSerial = false;
    private bool messageWritten = false;
    private int previousObjectNumber = -1;

    [Header("Move object's position")]
    public bool position;
    [Header("Move object's rotation")]
    public bool rotation;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        InputTracking.GetNodeStates(states);

        // indication that we've changed a value
        if (previousObjectNumber != objectNumber) {
            previousObjectNumber = objectNumber;
            messageWritten = false;
        }

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
        if (objectNumber < states.Count || objectNumber < 0)
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
            if (!messageWritten) {
                Debug.Log("Successfully set to object " + objectNumber);
                messageWritten = true;
            }
        } else if (!messageWritten) {
            Debug.Log("Object number out of range");
            messageWritten = true;
        }
    }
}
