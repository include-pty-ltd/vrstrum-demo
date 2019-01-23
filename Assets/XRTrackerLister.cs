using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class XRTrackerLister : MonoBehaviour {

	// Use this for initialization
	void Start () {
        List<XRNodeState> nodes = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodes);
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < nodes.Count; i++) {
            sb.Append("Tracked Object ")
                .Append(i)
                .Append(": ")
                .Append(InputTracking.GetNodeName(nodes[i].uniqueID))
                .Append(" (")
                .Append(nodes[i].nodeType)
                .Append(")")
                .Append(Environment.NewLine);
        }
        Debug.Log(sb.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
