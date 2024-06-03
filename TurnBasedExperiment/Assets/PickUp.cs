using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 1f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    FirstPersonLook mouseLookScript;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        mouseLookScript = player.GetComponent<FirstPersonLook>();

        if (mouseLookScript == null)
        {
            Debug.LogError("FirstPersonLook component not found on the player GameObject.");
        }
    }

    void LateUpdate()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * pickUpRange, Color.red);
    }
    void Update()
    {
        if (heldObj == null)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * pickUpRange, Color.red);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    Debug.Log("Raycast hit: " + hit.transform.gameObject.name);
                    if (hit.transform.gameObject.tag == "CanPickUp")
                    {
                        Debug.Log("Object has canPickUp tag");
                        PickUpObject(hit.transform.gameObject);
                    }
                    else
                    {
                        Debug.Log("Object does not have canPickUp tag");
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any object");
                }
            }
        }
        else
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true)
            {
                Debug.Log("Throwing the object");
                StopClipping();
                ThrowObject();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Dropping the object");
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj = null;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.position;
    }

    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }

    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            Vector3 offsetDirection = (heldObj.transform.position - transform.position).normalized;
            heldObj.transform.position = transform.position + offsetDirection * 0.5f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * pickUpRange);
    }
}