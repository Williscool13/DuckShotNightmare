using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTarget : MonoBehaviour, ITarget
{

    [SerializeField] GameObject dialogueBubbleObject;
    [SerializeField] BoxCollider2D dialogueCollider;
    [SerializeField] GameObject bulletHolePrefab;
    [SerializeField] Sprite bulletHoleSprite;

    public bool Alive => true;
    public TargetType TargetType { get { return TargetType.Dialogue; } }

    public void Hit(int damage, Vector2 impactPosition) {
        Debug.Log("Bullet on dialogue target hit");
        SpawnBulletHole(impactPosition);
    }

    void SpawnBulletHole(Vector2 impact) {
        Vector2 size = dialogueCollider.bounds.size;

        GameObject gameObject = Instantiate(bulletHolePrefab, dialogueBubbleObject.transform);
        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
        Vector2 rendSize = rend.bounds.size * (Vector2) DivideVector3s(gameObject.transform.localScale, dialogueBubbleObject.transform.localScale);
        // add bullet size offset
        Vector2 min = (Vector2)dialogueBubbleObject.transform.position - size / 2;
        Vector2 max = (Vector2)dialogueBubbleObject.transform.position + size / 2;

        impact.x = Mathf.Clamp(impact.x, min.x + rendSize.x, max.x - rendSize.x);
        impact.y = Mathf.Clamp(impact.y, min.y + rendSize.y, max.y - rendSize.y);


        //GameObject gameObject = Instantiate(bulletHolePrefab, dialogueBubbleObject.transform);
        gameObject.transform.position = impact;

        gameObject.GetComponent<SpriteRotationManager>().SetFrontFace(bulletHoleSprite);
        gameObject.AddComponent<BulletHoleScaleOffset>();
    }

    Vector3 DivideVector3s(Vector3 A, Vector3 B) {
        return new Vector3(
            A.x / B.x,
            A.y / B.y,
            A.z / B.z
            );
    }
}
