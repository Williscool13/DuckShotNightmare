using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleScaleOffset : MonoBehaviour
{
    Vector3 baseScale = Vector3.one;
    [SerializeField] public bool parentParentScale = false;
    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;


        Vector3 parentScale = transform.parent.localScale;
        if (parentParentScale) { parentScale = transform.parent.parent.localScale; }

        Vector3 targetScale = new Vector3(
            baseScale.x / parentScale.x,
            baseScale.y / parentScale.y,
            baseScale.z / parentScale.z
            );
        transform.localScale = targetScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 parentScale = transform.parent.localScale;
        if (parentParentScale) { parentScale = transform.parent.parent.localScale; }

        Vector3 targetScale = new Vector3(
            baseScale.x / parentScale.x,
            baseScale.y / parentScale.y,
            baseScale.z / parentScale.z
            );
        transform.localScale = targetScale;
    }
}
