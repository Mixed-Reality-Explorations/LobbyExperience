using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace UnityEngine.XR.iOS
{
    public class ARAnchorComponent : MonoBehaviour
    {

        private string m_AnchorId;

        public string AnchorId { get { return m_AnchorId; } }

        void Start()
        {
            UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent += GameObjectAnchorUpdated;
            UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += AnchorRemoved;
            UnityARSessionNativeInterface.ARUserAnchorAddedEvent += AnchorAdded;
            this.m_AnchorId = UnityARSessionNativeInterface.GetARSessionNativeInterface().AddUserAnchorFromGameObject(this.gameObject).identifierStr;
        }

        public void AnchorAdded(ARUserAnchor anchor)
        {
            Debug.LogFormat("STE ~~~~~~~~~ ARAnchor Added: {0}", anchor.transform.ToString());
        } 

        public void AnchorRemoved(ARUserAnchor anchor)
        {
            if (anchor.identifier.Equals(m_AnchorId))
            {
                Destroy(this.gameObject);
            }
            Debug.Log("STE ~~~~~~~~~ ARAnchor Destroyed");

        }

        void OnDestroy()
        {
            UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent -= GameObjectAnchorUpdated;
            UnityARSessionNativeInterface.ARUserAnchorRemovedEvent -= AnchorRemoved;
            UnityARSessionNativeInterface.GetARSessionNativeInterface().RemoveUserAnchor(this.m_AnchorId);
        }

        private void GameObjectAnchorUpdated(ARUserAnchor anchor)
        {
            if (anchor.identifier.Equals(m_AnchorId))
            {
                transform.position = UnityARMatrixOps.GetPosition(anchor.transform);
                transform.rotation = UnityARMatrixOps.GetRotation(anchor.transform);

                Console.WriteLine("Updated: pos = " + transform.position + m_AnchorId);
            }
        }
    }
}