using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using Photon.Pun;
using System.Text.RegularExpressions;
using GorillaRigging;

namespace GorillaModManager
{
    public class NameTagManagers : MonoBehaviour
    {
        private readonly Dictionary<int, Nametag> tags = new();

		public LayerMask OcclusionSettings;

        public bool visible = true; // DEBUG: force enabled
        private bool cachedState;

        private const int NicknameMaxLength = 12;
        private const int UserIdMaxLength   = 48;

        private static readonly Regex SafeTextRegex =
            new(@"[^a-zA-Z0-9_\-\.\s]", RegexOptions.Compiled);

        private float cleanupTimer;
        private const float cleanupInterval = 1f; // safer cleanup

        private void Update()
        {
            // -------------- DEBUG: RIG CHECK --------------
            var rigs = FindObjectsByType<RigController>();
            //Debug.Log($"[NAMETAGS] Rigs found: {rigs.Length}");

            foreach (var rig in rigs)
            {
                if (rig == null)
                {
                    //Debug.LogWarning("[NAMETAGS] Null rig found — skipped.");
                    continue;
                }

                if (rig.RigStatus != RigController.RigType.Online)
                {
                    //Debug.Log($"[NAMETAGS] Rig '{rig.name}' isn't Online, skipping.");
                    continue;
                }

                Photon.Realtime.Player owner = rig.photonView?.Owner;

                if (owner == null)
                {
                    //Debug.LogWarning($"[NAMETAGS] Rig '{rig.name}' has NO Photon Owner!");
                    continue;
                }

                int actor = owner.ActorNumber;

                if (!tags.ContainsKey(actor) || tags[actor] == null)
                {
                    //Debug.Log($"[NAMETAGS] Creating tag for Actor {actor} (Nick: {owner.NickName}, UserId: {owner.UserId})");

                    Nametag newTag = CreateTag(rig, owner);
                    tags[actor] = newTag;

                    if (newTag == null)
                    {
                        //Debug.LogError($"[NAMETAGS] FAILED to create tag for Actor {actor}");
                    }
                }

                UpdateTagTransform(tags[actor], rig);
            }

            // ---------- Cleanup Timer ----------
            cleanupTimer += Time.deltaTime;
            if (cleanupTimer >= cleanupInterval)
            {
                CleanupMissingPlayers();
                cleanupTimer = 0f;
            }

            // ---------- Visibility Toggle ----------
            if (visible != cachedState)
            {
                //Debug.Log($"[NAMETAGS] Visibility changed: {visible}");

                foreach (var kvp in tags)
                {
                    if (kvp.Value != null)
                        kvp.Value.Toggle(visible);
                }
                cachedState = visible;
            }
        }
        private Nametag CreateTag(RigController rig, Photon.Realtime.Player owner)
        {
            if (rig.playerTextName == null)
            {
                //Debug.LogError("[NAMETAGS] rig.playerTextName is NULL! Cannot create nametag.");
                return null;
            }

            GameObject go = Instantiate(rig.playerTextName, rig.playerTextName.transform.parent).gameObject;
            go.name = $"Nametag Actor {owner.ActorNumber}";
            go.SetActive(true);

            Nametag tag = go.AddComponent<Nametag>();
			NametagOcclusionFade nof = go.AddComponent<NametagOcclusionFade>();

			nof.occlusionMask = OcclusionSettings;

            if (tag.Text == null)
            {
                //Debug.LogError($"[NAMETAGS] Nametag.Text is NULL on newly created tag!");
                return tag;
            }

            tag.Text.supportRichText = true;
            tag.Text.enabled = visible;
            tag.Text.color = Color.white;

            string safeNick = SanitizeName(owner.NickName, NicknameMaxLength);
            string safeId   = SanitizeId(owner.UserId, UserIdMaxLength);

            //Debug.Log($"[NAMETAGS] Sanitized => Nick: '{safeNick}', Id: '{safeId}'");

            tag.Text.text = $"| {safeNick} |\nID: {safeId}";

            return tag;
        }


        private void UpdateTagTransform(Nametag tag, RigController rig)
        {
            if (tag == null)
            {
                //Debug.LogWarning("[NAMETAGS] Tried to update NULL tag.");
                return;
            }

            if (tag.Text == null)
            {
                //Debug.LogError("[NAMETAGS] tag.Text is NULL during transform update.");
                return;
            }

            //tag.transform.position = rig.transform.position + Vector3.up * 1f;
			tag.transform.localPosition = new Vector3(0,150f,0);

            // Proper billboard
            tag.transform.LookAt(PlayerBase.@i.transforms.Camera.transform);
            tag.transform.Rotate(0, 180f, 0);

            tag.transform.localScale = new Vector3(2f, 2f, 2f);

            //Debug.Log($"[NAMETAGS] Updated tag transform for Actor tag.", tag);
        }

        // -------------------------
        //     SANITIZATION
        // -------------------------
        private string SanitizeName(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return "Unknown";

            string stripped = SafeTextRegex.Replace(input, "");

            if (stripped.Length > maxLength)
                stripped = stripped.Substring(0, maxLength);

            //Debug.Log($"[NAMETAGS] Name sanitized from '{input}' -> '{stripped}'");
            return stripped;
        }

        private string SanitizeId(string id, int maxLength)
        {
            if (string.IsNullOrEmpty(id) || id == "null" || id == "0")
            {
                //Debug.Log("[NAMETAGS] ID rejected — null/unverified");
                return "Unverified";
            }

            string stripped = SafeTextRegex.Replace(id, "");

            if (stripped.Length > maxLength)
                stripped = stripped.Substring(0, maxLength);

            //Debug.Log($"[NAMETAGS] ID sanitized from '{id}' -> '{stripped}'");
            return stripped;
        }

        // -------------------------
        //       CLEANUP
        // -------------------------
        private void CleanupMissingPlayers()
        {
            HashSet<int> alive = new();
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                alive.Add(p.ActorNumber);

            List<int> remove = new();

            foreach (var kvp in tags)
            {
                int actor = kvp.Key;
                Nametag tag = kvp.Value;

                if (!alive.Contains(actor))
                {
                    //Debug.Log($"[NAMETAGS] Removing tag for Actor {actor} — disconnected.");
                    if (tag != null)
                        Destroy(tag.gameObject);
                    remove.Add(actor);
                }
                else if (tag == null)
                {
                    //Debug.LogWarning($"[NAMETAGS] Tag missing for Actor {actor}, removing.");
                    remove.Add(actor);
                }
            }

            foreach (int k in remove)
                tags.Remove(k);
        }
    }
}
