using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _carryingPoint;
    [SerializeField] private float _itemMoveSpeed;

    [SerializeField]public List<GameObject> CarryingItems;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        StartCoroutine(MoveItemToPlayer(other.transform));
    }

    private IEnumerator MoveItemToPlayer(Transform otherTransform)
    {
        otherTransform.parent = transform;
        otherTransform.GetComponent<BoxCollider>().enabled = false;
        _playerController.ChangeCarryingStatus(true);
        while (otherTransform.position != _carryingPoint.transform.position)
        {
            otherTransform.position = Vector3.MoveTowards(otherTransform.position, _carryingPoint.position, _itemMoveSpeed);
            yield return new WaitForEndOfFrame();
        }
        ChangeCarryingPoint(otherTransform.GetComponent<MeshRenderer>().bounds.size.y);
        AddItemToList(otherTransform.gameObject);
        yield return new WaitForEndOfFrame();
    }

    public void ChangeCarryingPoint(float offset)
    {
        _carryingPoint.localPosition = new Vector3(_carryingPoint.localPosition.x, _carryingPoint.localPosition.y + offset, _carryingPoint.localPosition.z);
    }

    private void AddItemToList(GameObject item)
    {
        CarryingItems.Add(item);
    }

    public void RemoveItemFromList(GameObject go)
    {
        CarryingItems.Remove(go);
    }
}
