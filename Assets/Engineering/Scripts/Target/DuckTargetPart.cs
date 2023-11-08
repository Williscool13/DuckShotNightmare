using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTargetPart : MonoBehaviour
{

    [SerializeField] Transform[] transforms;
    [SerializeField] DuckPart part;
    [SerializeField] Transform parent;
    public DuckPart DuckPart { get { return part; } }

    List<Vector2> unoccupied = new();
    private void Start() {
        Debug.Assert(transforms.Length > 0, "Duck part needs some position for its bullet hole");
        foreach (Transform trans in transforms) {
            unoccupied.Add(trans.localPosition);
            absPositions.Add(trans.localPosition);
        }
    }

    public Vector2 GetClosestUnoccupiedLocalPosition(Vector2 impactWorldPosition) {

        Vector2 target;

        if (unoccupied.Count > 0) {
            target = FindClosest(unoccupied, impactWorldPosition);
            unoccupied.Remove(target);
        } else {
            target = transforms[0].localPosition;
        }


        return target;

    }

    List<Vector2> absPositions = new();
    public Vector2 GetClosestPosition(Vector2 impactPosition) {
        return FindClosest(absPositions, impactPosition);
    }

    Vector2 FindClosest(List<Vector2> currUnoccupied, Vector2 impact) {

        float closestDist = float.MaxValue;
        Vector2 closestVec = Vector2.zero;
        Vector2 convertedImpact = parent.InverseTransformPoint(impact);
        foreach (Vector2 v in currUnoccupied) {
            float currDist = Vector2.Distance(convertedImpact, v);
            if (currDist < closestDist) {
                closestDist = currDist;
                closestVec = v;
            }
        }

        return closestVec;
    }

}

public enum DuckPart
{
    Head = 0,
    Body = 1,
    Tail = 2
}
