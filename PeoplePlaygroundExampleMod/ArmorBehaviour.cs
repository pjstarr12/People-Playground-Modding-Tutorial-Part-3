using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Mod
{
    public class ArmorBehaviour : MonoBehaviour
    {
        private bool equipped;
        public string armorPiece;
        public int armorTier;
        public float stabResistance;
        private bool blockingStab;

        // dont do this
        public string otherSprite;
        public string otherName;
        public string otherPiece;
        public float otherResist;
        public int otherTier;

        void Start()
        {
            GetComponent<PhysicalBehaviour>().HoldingPositions = new Vector3[0];
        }
        public void SpawnOtherParts()
        {
            GameObject lower = Instantiate(ModAPI.FindSpawnable(otherName).Prefab, transform.position, transform.rotation);
            lower.name = ModAPI.FindSpawnable(otherName).name;
            lower.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(otherSprite);
            ArmorBehaviour armor1 = lower.AddComponent<ArmorBehaviour>();
            armor1.stabResistance = otherResist;
            armor1.armorPiece = otherPiece;
            armor1.armorTier = otherTier;
            lower.FixColliders();
        }
        void Update()
        {
            if (equipped && GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>() && GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>().CurrentlyHolding)
            {
                GripBehaviour grip = GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>();
                Nocollide(grip.CurrentlyHolding.gameObject);
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            LimbBehaviour limb = collision.gameObject.GetComponent<LimbBehaviour>();
            ArmorBehaviour arm = collision.gameObject.GetComponent<ArmorBehaviour>();
            if (arm)
            {
                Nocollide(arm.gameObject);
            }
            if (limb)
            {
                Nocollide(limb.gameObject);
                // Bodypart sections are Torso, Head, Arms, and Legs
                // Bodyparts are UpperBody, MiddleBody, LowerBody etc.
                Debug.Log(limb.gameObject.ToString());
                Debug.Log(armorPiece);
                if (!equipped && limb.gameObject.ToString() == armorPiece + " (UnityEngine.GameObject)")
                {
                    Debug.Log("attach");
                    Attach(limb);
                }
            }
            if (equipped && collision.gameObject.GetComponent<Rigidbody2D>().velocity.x < stabResistance && !limb)
            {
                transform.parent = limb.transform;
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<FixedJoint2D>().enabled = false;
            }
            else if (equipped)
            {
                transform.parent = null;
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<FixedJoint2D>().enabled = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (GetComponent<Rigidbody2D>().isKinematic == true)
            {
                transform.parent = null;
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<FixedJoint2D>().enabled = true;
            }
        }
        public void Nocollide(GameObject col)
        {
            NoCollide noCol = gameObject.AddComponent<NoCollide>();
            noCol.NoCollideSetA = GetComponents<Collider2D>();
            noCol.NoCollideSetB = col.GetComponents<Collider2D>();
        }
        public void Attach(LimbBehaviour limb)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sortingOrder = limb.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            sr.sortingLayerName = limb.gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            equipped = true;
            GetComponent<Rigidbody2D>().isKinematic = true;
            transform.parent = limb.transform;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
            transform.parent = null;
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.dampingRatio = 1;
            joint.frequency = 0;
            joint.connectedBody = limb.GetComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }
}
