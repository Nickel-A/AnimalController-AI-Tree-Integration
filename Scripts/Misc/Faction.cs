using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Events;
using MalbersAnimations.HAP;
using MalbersAnimations.Scriptables;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{


    public class Faction : MonoBehaviour
    {
        public enum AttackStyle
        {
            Unarmed,
            Melee,
            Range
        }
        public enum AttackOrder
        {
            OnSight,
            WhenLeaderDamaged
        }
        public enum MountType
        {
            Horse,
            Wolf,
            Dragon
        }

        public enum Formation
        {
            BehindLine,
            BehindColumn,
            FrontLine,
            Circle,
            HalfCircle
        }

        public bool groupLeader;
        public string groupName;
        public bool whenMountingAssignGroup;
        public bool whenDismountingClearGroup;
        public bool mountable;
        public MountType mountType;
        [HideInInspector]
        public bool taken;
        [HideInInspector]
        public MAnimalAIControl mAnimalAIControl;
        [HideInInspector]
        public MRider mRider;

        [HideInInspector]
        public Mount mount;
        private EnableDisableEvent enableDisableEvent;
        private MalbersInput malbersInput;
        [HideInInspector]
        public MWeaponManager mWeaponManager;
        [HideInInspector]
        public MAnimal mAnimal;
        private MAnimal mAnimalOther;
        private GameObject leader;
        [HideInInspector]
        public bool followingLeader;
        [HideInInspector]
        public BehaviourRunner behaviourRunner;

        //Formations
        private int _nbFollowers;
        public Formation formation;
        public float distanceBetween = 5f;
        public float distanceToLeader = 5f;
        private GameObject[] followersGO;
        [HideInInspector]
        public GameObject cubePrefab;
        private List<MAnimalAIControl> followersAIController = new List<MAnimalAIControl>();
        [HideInInspector]
        public bool inFormation;
        public AttackStyle attackStyle;
        public AttackOrder attackOrder;
        public GameObject enemyInSight;
        public GameObject attackedBy;
        // Start is called before the first frame update
        void Start()
        {
            mAnimalAIControl = gameObject.transform.root.GetComponentInChildren<MAnimalAIControl>();
            mAnimal = gameObject.transform.root.GetComponentInChildren<MAnimal>();
            mRider = gameObject.transform.root.GetComponentInChildren<MRider>();
            mWeaponManager = gameObject.transform.root.GetComponentInChildren<MWeaponManager>();
            behaviourRunner = gameObject.GetComponent<BehaviourRunner>();
            mount = gameObject.transform.root.GetComponentInChildren<Mount>();
        }

        public void OnDamage()
        {
            Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 10);
            foreach (Collider collider in colliders)
            {
                // Check if the collider belongs to a game object you're interested in
                if (collider.gameObject.CompareTag("Animal") && collider.gameObject != this.gameObject)
                {
                    MAnimalAIControl attackerAI = collider.gameObject.GetComponentInChildren<MAnimalAIControl>();
                    if (attackerAI != null)
                    {
                        if (attackerAI.Target !=null)
                        {
                            if (attackerAI.Target.gameObject == this.gameObject)
                            {
                                attackedBy = collider.gameObject;
                            } else if (mount!=null)
                            {
                                if (attackerAI.Target.gameObject == mount.Rider.gameObject)
                                {
                                    attackedBy = collider.gameObject;
                                    Faction riderfaction = mount.Rider.GetComponent<Faction>();
                                    riderfaction.attackedBy = collider.gameObject;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Update is called once per frame
        void Update()
        {

        }

        public int NBFollowers
        {
            get { return _nbFollowers; }
            set
            {
                if (_nbFollowers != value)
                {
                    //Debug.Log("Value changed from " + _nbFollowers + " to " + value);
                    _nbFollowers = value;
                }
            }
        }

        void DestroyPreviousCubes()
        {
            // Check if followersGO array is initialized and contains any cubes
            if (followersGO != null && followersGO.Length > 0)
            {
                // Loop through the array and destroy each cube
                foreach (GameObject cube in followersGO)
                {
                    Destroy(cube);
                }
            }
        }

        public void FollowersTarget()
        {
            if (groupLeader)
            {
                if (NBFollowers > 0)
                {
                    Vector3 startPosition;
                    float totalWidth;
                    float angleIncrement;
                    Quaternion startRotation;
                    Vector3 followerPosition;
                    DestroyPreviousCubes();
                    switch (formation)
                    {
                        case Formation.BehindColumn:
                            startPosition = transform.position - transform.forward * distanceToLeader;
                            followersGO = new GameObject[_nbFollowers];
                            for (int i = 0; i < _nbFollowers; i++)
                            {
                                followerPosition = startPosition - transform.forward * (i * distanceBetween);
                                GameObject cube = Instantiate(cubePrefab, followerPosition, Quaternion.identity);
                                followersGO[i] = cube;
                            }
                            break;
                        case Formation.BehindLine:
                            totalWidth = (_nbFollowers - 1) * distanceBetween;
                            startPosition = transform.position - transform.right * totalWidth / 2;

                            followersGO = new GameObject[_nbFollowers];
                            for (int i = 0; i < _nbFollowers; i++)
                            {
                                followerPosition = startPosition + transform.right * (i * distanceBetween);
                                followerPosition -= transform.forward * distanceToLeader;
                                GameObject cube = Instantiate(cubePrefab, followerPosition, Quaternion.identity);
                                followersGO[i] = cube;
                            }
                            break;
                        case Formation.FrontLine:
                            totalWidth = (_nbFollowers - 1) * distanceBetween;
                            startPosition = transform.position - transform.right * totalWidth / 2;

                            followersGO = new GameObject[_nbFollowers];
                            for (int i = 0; i < _nbFollowers; i++)
                            {
                                followerPosition = startPosition + transform.right * (i * distanceBetween);
                                followerPosition += transform.forward * distanceToLeader;
                                GameObject cube = Instantiate(cubePrefab, followerPosition, Quaternion.identity);
                                followersGO[i] = cube;
                            }
                            break;
                        case Formation.HalfCircle:
                            angleIncrement = 180f / (_nbFollowers - 1);
                            followersGO = new GameObject[_nbFollowers];

                            startRotation = transform.rotation;
                            startPosition = transform.position - startRotation * Vector3.forward * distanceBetween;

                            for (int i = 0; i < _nbFollowers; i++)
                            {
                                float angle = i * angleIncrement;
                                float xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * distanceBetween;
                                float zPos = Mathf.Sin(Mathf.Deg2Rad * angle) * distanceBetween;

                                followerPosition = startPosition + startRotation * (new Vector3(xPos, 0f, zPos));
                                followerPosition += transform.forward * distanceBetween;
                                GameObject cube = Instantiate(cubePrefab, followerPosition, Quaternion.identity);
                                followersGO[i] = cube;
                            }
                            break;
                        case Formation.Circle:
                            angleIncrement = 360f / (_nbFollowers);
                            followersGO = new GameObject[_nbFollowers];

                            startRotation = transform.rotation;
                            startPosition = transform.position - startRotation * Vector3.forward * distanceBetween;

                            for (int i = 0; i < _nbFollowers; i++)
                            {
                                float angle = i * angleIncrement;
                                float xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * distanceBetween;
                                float zPos = Mathf.Sin(Mathf.Deg2Rad * angle) * distanceBetween;

                                followerPosition = startPosition + startRotation * (new Vector3(xPos, 0f, zPos));
                                followerPosition += transform.forward * distanceBetween;
                                GameObject cube = Instantiate(cubePrefab, followerPosition, Quaternion.identity);
                                followersGO[i] = cube;
                            }
                            break;
                    }
                }
            }
        }

        public void RidingDistance()
        {
            ChangeAITargetDistance(3, 5);
        }

        public void NotRidingDistance()
        {
            ChangeAITargetDistance(1, 2);
        }

        public void ChangeAITargetDistance(float stoppingdistance, float slowingdistance)
        {

            AITarget aiTarget;
            aiTarget = mRider.Montura.Animal.gameObject.GetComponent<AITarget>();
            aiTarget.stoppingDistance = stoppingdistance;
            aiTarget.slowingDistance = slowingdistance;
        }
        public void SetFormation()
        {
            followersAIController = new List<MAnimalAIControl>();
            Faction[] factions = FindObjectsOfType<Faction>();
            foreach (Faction faction in factions)
            {
                if (groupName == faction.groupName && faction.followingLeader && faction.inFormation)
                {
                    followersAIController.Add(faction.gameObject.GetComponentInChildren<MAnimalAIControl>());
                }
            }
            NBFollowers = followersAIController.Count;
            FollowersTarget();
            int i = 0;
            foreach (MAnimalAIControl manimalcontrol in followersAIController)
            {
                manimalcontrol.SetTarget(followersGO[i], true);
                followersGO[i].transform.SetParent(this.gameObject.transform);
                //manimalcontrol.SetDestination(followersGO[i].transform.position, true);
                i++;
            }
        }

        public GameObject FindLeader(string groupName)
        {
            Faction[] factions = FindObjectsOfType<Faction>();
            foreach (Faction faction in factions)
            {
                if (groupName == faction.groupName && faction.groupLeader)
                {
                    return faction.gameObject;
                }
            }
            return null;
        }

        public MAnimal FindLeaderAnimal(string groupName)
        {
            Faction[] factions = FindObjectsOfType<Faction>();
            foreach (Faction faction in factions)
            {
                if (groupName == faction.groupName && faction.groupLeader)
                {
                    return faction.gameObject.GetComponent<MAnimal>();
                }
            }
            return null;
        }
    }
}