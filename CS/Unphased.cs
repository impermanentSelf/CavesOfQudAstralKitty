using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Language;
using XRL.Rules;
using XRL.World.Capabilities;

namespace XRL.World.Effects
{
    [Serializable]
    public class Unphased : Effect
    {
        public string RenderString = "a";
        public string Tile;

        public Unphased()
        {
        }

        public Unphased(int _Duration)
          : this()
        {
            this.Duration = _Duration;
        }

        public override bool SameAs(Effect e)
        {
            Unphased unphased = e as Unphased;
            if (unphased.Tile != this.Tile || unphased.RenderString != this.RenderString)
                return false;
            return base.SameAs(e);
        }

		public override bool UseStandardDurationCountdown(){
			return true;
		}

		public override bool WantEvent(int ID, int cascade){
			if (!base.WantEvent(ID, cascade) && ID != SingletonEvent<BeginTakeActionEvent>.ID && (ID != EffectAppliedEvent.ID)){
				return ID == WasDerivedFromEvent.ID;
			}
			return true;
		}

        public override string GetDetails()
        {
            return "Temporarily able to interact with creatures and objects unless they're phased.\nCan no longer pass through solids.";
        }

		public override bool HandleEvent(EffectAppliedEvent E){
			Object.RemoveEffect<Phased>();
			return base.HandleEvent(E);
		}

		public override bool Apply(GameObject Object){
			return Object.RemoveEffect<Phased>();
		}

        public override void Remove(GameObject Object)
        {
            Object.ApplyEffect(new Phased(9999));
            base.Remove(Object);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            this.Tile = Object.Render.Tile;
            this.RenderString = Object.Render.RenderString;
            // Registrar.Register("BeginTakeAction");
            base.Register(Object, Registrar);
        }

        public override bool Render(RenderEvent E)
        {
            return true;
        }

		public override bool HandleEvent(BeginTakeActionEvent E){
			if (this.Duration > 0){
                if (this.Duration != 9999){
                    if (this.Duration > 0 && this.Object.IsPlayer()){
                        Effect.AddPlayerMessage("You will phase back out in " + Duration.Things("round") + ".");
					}
				}
            }
			return base.HandleEvent(E);
		}
    }
}
