using UnityEngine;
using Photon.Pun;

namespace GorillaRigging{
    public class RemoteRigController : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks
    {

        protected RigLink headLink;
        protected RigLink bodyLink;
        protected RigLink handRight;
        protected RigLink handLeft;
        protected RigLink footLeft;
        protected RigLink footRight;

        public bool setRigWeights {get; private set;}
        public bool isBeingControlled {get; private set;}
        public int ownerID;
        public float sizeRatio;
        public RigController ControllingRig;

        private void Update()
        {
            if(!setRigWeights) return;

            if(isBeingControlled && photonView.IsMine)
            {
                SelfControlledLogic();
            }
            else if(isBeingControlled && !photonView.IsMine)
            {
                RemoteControlledLogic();
            }
            else
            {
                UncontrolledLogic();
            }
        }

        private void SelfControlledLogic()
        {
            
        }

        private void RemoteControlledLogic()
        {
            
        }

        private void UncontrolledLogic()
        {
            
        }

        public void SetupRemoteRig(RigLink head, RigLink body, RigLink handR, RigLink handL, RigLink footL, RigLink footR)
        {
            if(!setRigWeights)
            {
                headLink = head;
                bodyLink = body;
                handRight = handR;
                handLeft = handL;
                footLeft = footL;
                footRight = footR;
                setRigWeights=true;
            }
        }

        public void RequestControl()
        {
            if (photonView.IsMine)
            {
                return;
            }

            photonView.RequestOwnership();
        }

        public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
        {
            if (targetView != photonView)
            {
                return;
            }

            RigController requestingRig = PhotonRoomManagerMain.instance.FindVRRigControllerForPlayer(requestingPlayer);

            if(requestingRig == null) return;

            if(!isBeingControlled)
            {
                ControllingRig = requestingRig;
                photonView.TransferOwnership(requestingPlayer);
            }
            else if(isBeingControlled && ControllingRig == null)
            {
                ControllingRig = requestingRig;
                photonView.TransferOwnership(requestingPlayer);
            }

        }

        public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
        {
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
        {
        }

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(ownerID);
                stream.SendNext(isBeingControlled);
            }
            else
            {
                ownerID = (int)stream.ReceiveNext();
                isBeingControlled = (bool)stream.ReceiveNext();
            }
        }
    }
}