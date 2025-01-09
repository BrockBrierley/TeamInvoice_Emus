using UnityEngine;

public class PatrolPointVisual : MonoBehaviour
{
    // Light blue color with some transparency
    [SerializeField] private Color gizmoColour = new Color(0.5f, 0.8f, 1f, 0.5f);
    [SerializeField] private float gizmoSize = 0.5f;
    [SerializeField] private bool connectPoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;

        //loop through all the children
        foreach (Transform child in transform)
        {
            //draws a sphere at the patrol points
            Gizmos.DrawSphere(child.position, gizmoSize);

            //draws lines to connect points for visualisation, can toggle with bool
            if (connectPoints && child.GetSiblingIndex() < transform.childCount -1)
            {
                Transform nextChild = transform.GetChild(child.GetSiblingIndex() + 1);
                Gizmos.DrawLine(child.position, nextChild.position);
            }
        }
    }

}
