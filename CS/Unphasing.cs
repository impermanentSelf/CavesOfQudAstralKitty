using System;
using System.Collections.Generic;
using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.AI.GoalHandlers;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Unphasing : BaseMutation
    {
        public Guid UnphaseActivatedAbilityID = Guid.Empty;
        public ActivatedAbilityEntry UnphaseActivatedAbility;

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeginTakeAction");
            Registrar.Register("AfterPhaseIn");
            Registrar.Register("AfterPhaseOut");
			Registrar.Register("CommandTogglePhase");
            base.Register(Object, Registrar);
        }

        public override bool WantEvent(int ID, int cascade){
            if (!base.WantEvent(ID, cascade)){
                return (ID == AIGetOffensiveAbilityListEvent.ID || ID == AIGetDefensiveAbilityListEvent.ID);
            }
            return true;
        }

        public override string GetDescription()
        {
            return "You live in an alternate reality, but may phase into reality for brief periods of time.";
        }

        public override string GetLevelText(int Level)
        {
            return "Cooldown: " + GetCooldown(Level) + " rounds\n" + "Duration: " + GetDuration(Level) + " rounds";
        }
        public void SyncAbilities()
        {
            UnphaseActivatedAbility.ToggleState = ParentObject.HasEffect<Unphased>();
        }

        public int GetCooldown(int Level){
            return 103 - 3 * Level;
        }

        public int GetDuration(int Level){
            return 6 + Level;
        }

        public override bool HandleEvent(AIGetOffensiveAbilityListEvent E){
            bool phased = E.Actor.HasEffect<Phased>();
            if (phased != E.Target.HasEffect<Phased>() && (!phased || UnphaseActivatedAbility.Cooldown <= 0) && E.Distance <= 2 && E.Actor.HasLOSTo(E.Target, IncludeSolid: false)){
                E.Add("CommandTogglePhase");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AIGetDefensiveAbilityListEvent E){
            bool phased = E.Actor.HasEffect<Phased>();
            if (phased == E.Target.HasEffect<Phased>() && (!phased || UnphaseActivatedAbility.Cooldown <= 0) && E.Actor.hitpoints <= E.Actor.baseHitpoints*2 / 5){
                E.Add("CommandTogglePhase");
            }
            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction"){
                if (!this.ParentObject.HasEffect<Phased>() && !this.ParentObject.HasEffect<RealityStabilized>() && !this.ParentObject.HasEffect<Unphased>())
                {
                    this.ParentObject.ApplyEffect(new Phased(9999));
                    UnphaseActivatedAbility.ToggleState = false;
                }
                SyncAbilities();
                return true;

			} else if (E.ID == "AfterPhaseOut" || E.ID == "AfterPhaseIn"){
				SyncAbilities();
			} else if (E.ID == "CommandTogglePhase")
            {
                if (ParentObject.HasEffect<Unphased>()){
                    // Phase Out
                    if (!this.ParentObject.FireEvent(Event.New("InitiateRealityDistortionLocal", "Object", ParentObject, "Mutation", this), E)){
                        return false;
                    }
                    ParentObject.RemoveEffect<Unphased>();
                    ParentObject.PlayWorldSound("Sounds/Abilities/sfx_ability_mutation_phase");
                    // ParentObject.ApplyEffect(new Phased(9999));
                    SyncAbilities();
                    return true;
                } else{
                    //Phase In
                    ParentObject.ApplyEffect(new Unphased(GetDuration(Level) + 1));
                    CooldownMyActivatedAbility(UnphaseActivatedAbilityID, GetCooldown(Level));
                    SyncAbilities();
                    return true;
                }
                // return ParentObject.FireEvent(Event.New(this.ParentObject.HasEffect<Unphased>() ? "CommandPhase" : "CommandUnphase"));
            }
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.ParentObject.ApplyEffect((Effect)new Phased(9999));
            ActivatedAbilities part = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;
            if (part != null)
            {
                this.UnphaseActivatedAbilityID = AddMyActivatedAbility(
                    Name: "Unphase",
                    Command: "CommandTogglePhase",
                    Class: "Physical Mutation",
                    Description: "Peer behind the curtain from the other side.",
                    Icon: "°",
                    IsRealityDistortionBased: true,
                    Toggleable: true,
                    DefaultToggleState: false,
                    ActiveToggle: true,
                    IsAttack: false
                );
                this.UnphaseActivatedAbility = part.AbilityByGuid[this.UnphaseActivatedAbilityID];
            }
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            if (this.UnphaseActivatedAbilityID != Guid.Empty)
            {
                (GO.GetPart("ActivatedAbilities") as ActivatedAbilities).RemoveAbility(this.UnphaseActivatedAbilityID);
                this.UnphaseActivatedAbilityID = Guid.Empty;
            }
            return base.Unmutate(GO);
        }
    }
}
