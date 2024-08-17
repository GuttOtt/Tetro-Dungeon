using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Hoverable : MonoBehaviour
    {
        [SerializeField] public GameObject _object;
        public void OnMouseEnter()
        {
            _object.SetActive(true);
        }
        public void OnMouseExit()
        {
            _object.SetActive(false);
        }
    }
}
