using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{

    [NodeContent("Check Faction Data", "Animal Controller/Faction/Check Faction Data", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckFactionData : ConditionDecorator
    {
        private Faction faction;
        private Faction factionAttacked;
        public enum CheckFor
        {
            AttackStyle,
            AttackOrder,
            AttackedBy
        }
        public CheckFor checkFor;
        [Hide("checkFor", 0)]
        public Faction.AttackStyle attackStyle;
        [Hide("checkFor", 1)]
        public Faction.AttackOrder attackOrder;
        [Hide("checkFor", 0,1)]
        public bool checkRider;
        [Hide("checkFor", 2)]
        public TransformKey AnimalToCheck;
        public TransformKey AttackedBy;
        private MRider mRider;

        protected override void OnInitialize()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            if (faction.mount != null) mRider = faction.mount.Rider;
        }
        protected override bool CalculateResult()
        {
            switch (checkFor)
            {
                case CheckFor.AttackStyle:
                    {
                        if (checkRider)
                        {
                            return attackStyle == mRider.gameObject.GetComponentInChildren<Faction>().attackStyle;
                        }
                        else
                        {
                            return attackStyle == faction.attackStyle;
                        }
                    }
                case CheckFor.AttackOrder:
                    {
                        if (checkRider)
                        {
                            return attackOrder == mRider.gameObject.GetComponentInChildren<Faction>().attackOrder;
                        }
                        else
                        {
                            return attackOrder == faction.attackOrder;
                        }
                    }
                case CheckFor.AttackedBy:
                    {
                        if (AnimalToCheck.GetValue() != GetOwner().gameObject.transform)
                        {
                            factionAttacked = AnimalToCheck.GetValue().GetComponent<Faction>();
                        }
                        GameObject attby = factionAttacked.attackedBy;
                        if (attby !=null)
                        {
                            AttackedBy.SetValue(attby.transform);
                            return true;
                        }else
                        {
                            return false;
                        }
                    }
            }
            return false;
        }


    }
}
