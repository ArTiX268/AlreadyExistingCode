using UnityEngine;

public class ParrallaxBackground : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float parrallaxSpeed;

    private Transform cameraTransform;
    private Vector3 lastCamPos;
    private float textureUnitSizeY;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCamPos = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        textureUnitSizeY *= transform.localScale.y;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCamPos;
        transform.position += deltaMovement * parrallaxSpeed;

        lastCamPos = cameraTransform.position;

        if (cameraTransform.position.y - transform.position.y >= textureUnitSizeY)
        {
            float yOffset = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + yOffset, transform.position.z);
        }
    }
}
