using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SellController : MonoBehaviour
{
    public delegate void ItemWasSold(GameObject go);
    public event ItemWasSold Notify;
    [SerializeField] private CollectController _collectController;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _sellWaitTime;
    private bool _canSell;
    private int _counter;
    private float _itemMoveSpeed = 0.1f;


    private void OnTriggerEnter(Collider other)
    {
        _canSell = true;
        if (_collectController.CarryingItems.Count > 0)
        {
            _counter = _collectController.CarryingItems.Count - 1;
            StartCoroutine(SellItems());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        _canSell = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _canSell = false;
    }

    private IEnumerator SellItems()
    {
        while(_canSell && _counter >= 0)
        {
            GameObject go = _collectController.CarryingItems[_counter];
            StartCoroutine(MoveItemToSellZone(go.transform));
            _collectController.RemoveItemFromList(go);
            _counter -= 1;
            yield return new WaitForSeconds(_sellWaitTime);
        }
    }

    private IEnumerator MoveItemToSellZone(Transform trns)
    {
        bool IsMoving = true;
        trns.parent = null;
        while (IsMoving)
        {
           trns.position = Vector3.MoveTowards(trns.position, transform.position, _itemMoveSpeed);
            if (trns.position == transform.position)
            {
                IsMoving = false;
            }
            yield return new WaitForEndOfFrame();
        }
        _collectController.ChangeCarryingPoint(-(trns.GetComponent<MeshRenderer>().bounds.size.y));
        Notify.Invoke(trns.gameObject);
        if (_collectController.CarryingItems.Count == 0)
            _playerController.ChangeCarryingStatus(false);
    }
}
