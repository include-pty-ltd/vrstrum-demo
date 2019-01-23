using Include;
using System.Collections;
using UnityEngine;

public class VRStrum : MonoBehaviour
{
    public bool drawNearCone, drawFrustum; // debug drawing

    private Transform lookTarget; // this is where the camera will look at
    private Vector2 dimensions = Vector2.zero; // dimensions of the view
    private Vector3[] corners = new Vector3[4]; // corners of the frustrum
    private Camera cam; // target camera

    void Start()
    {
        ViewRInterface.OnDeviceConnected.AddListener(OnConnect); // add a listener for viewr clients
    }

    private void OnConnect(DeviceInfo info, GameObject gameObject)
    {
        dimensions = new Vector2(info.ScreenWidth, info.ScreenHeight); // set dimensions to match the viewr device
        lookTarget = gameObject.transform; // set the viewr device's game object to be the look target
        StartCoroutine(AttachOnCamera(gameObject)); // script associated with the viewr camera controller which only gets created a frame after this one
    }

    IEnumerator AttachOnCamera(GameObject gameObject)
    {
        yield return 0;
        ViewRCameraController controller = gameObject.GetComponentInChildren<ViewRCameraController>(); // get camera controllre
        controller.transform.parent = transform; // move the viewr device to this object
        controller.OnStartCamera.AddListener(OnStartCamera); // add a listener for when the camera starts streaming
    }

    private void OnStartCamera(ViewRDataType type, Camera camera)
    {
        if (type == ViewRDataType.Scene)
            cam = camera; // set the scene camera to this object
    }

    void Update()
    {
        if (cam == null) return; // return when we don't have a camera

        Vector3 position = this.transform.position;
        this.transform.rotation = lookTarget.rotation;

        float halfX = dimensions.x / 2000f;
        float halfY = dimensions.y / 2000f;

        corners[0] = lookTarget.transform.position - halfY * lookTarget.transform.up - halfX * lookTarget.transform.right; //Bottom-Left
        corners[1] = lookTarget.transform.position - halfY * lookTarget.transform.up + halfX * lookTarget.transform.right; //Bottom-Right
        corners[2] = lookTarget.transform.position + halfY * lookTarget.transform.up - halfX * lookTarget.transform.right; //Top-Left
        corners[3] = lookTarget.transform.position + halfY * lookTarget.transform.up + halfX * lookTarget.transform.right; //Top-Right
        Vector3 pa, pb, pc, pd;
        pa = corners[0]; //Bottom-Left
        pb = corners[1]; //Bottom-Right
        pc = corners[2]; //Top-Left
        pd = corners[3]; //Top-Right

        Vector3 pe = cam.transform.position;// eye position

        Vector3 vr = (pb - pa).normalized; // right axis of screen
        Vector3 vu = (pc - pa).normalized; // up axis of screen
        Vector3 vn = Vector3.Cross(vr, vu).normalized; // normal vector of screen

        Vector3 va = pa - pe; // from pe to pa
        Vector3 vb = pb - pe; // from pe to pb
        Vector3 vc = pc - pe; // from pe to pc
        Vector3 vd = pd - pe; // from pe to pd

        float n = -lookTarget.InverseTransformPoint(cam.transform.position).z; // distance to the near clip plane (screen)
        float f = cam.farClipPlane; // distance of far clipping plane
        float d = Vector3.Dot(va, vn); // distance from eye to screen
        float l = Vector3.Dot(vr, va) * n / d; // distance to left screen edge from the 'center'
        float r = Vector3.Dot(vr, vb) * n / d; // distance to right screen edge from 'center'
        float b = Vector3.Dot(vu, va) * n / d; // distance to bottom screen edge from 'center'
        float t = Vector3.Dot(vu, vc) * n / d; // distance to top screen edge from 'center'

        Matrix4x4 p = new Matrix4x4(); // Projection matrix
        p[0, 0] = 2.0f * n / (r - l);
        p[0, 2] = (r + l) / (r - l);
        p[1, 1] = 2.0f * n / (t - b);
        p[1, 2] = (t + b) / (t - b);
        p[2, 2] = (f + n) / (n - f);
        p[2, 3] = 2.0f * f * n / (n - f);
        p[3, 2] = -1.0f;

        cam.projectionMatrix = p; // Assign matrix to camera

        if (drawNearCone)
        { //Draw lines from the camera to the corners f the screen
            Debug.DrawRay(cam.transform.position, va, Color.blue);
            Debug.DrawRay(cam.transform.position, vb, Color.blue);
            Debug.DrawRay(cam.transform.position, vc, Color.blue);
            Debug.DrawRay(cam.transform.position, vd, Color.blue);
        }

        if (drawFrustum) DrawFrustum(cam); //Draw actual camera frustum
    }

    Vector3 ThreePlaneIntersection(Plane p1, Plane p2, Plane p3)
    { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
            (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }

    void DrawFrustum(Camera cam)
    {
        Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
        Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = ThreePlaneIntersection(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = ThreePlaneIntersection(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //near corners on the created projection matrix
            Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //far corners on the created projection matrix
            Debug.DrawLine(nearCorners[i], farCorners[i], Color.red, Time.deltaTime, false); //sides of the created projection matrix
        }
    }
}
