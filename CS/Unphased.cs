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

        public override string GetDetails()
        {
            return "Temporarily able to interact with creatures and objects unless they're phased.\nCan no longer pass through solids.";
        }

        public override bool Apply(GameObject Object)
        {
            return Object.RemoveEffect<Phased>();
        }

        public override void Remove(GameObject Object)
        {
            // Object.ApplyEffect((Effect)new Phased(9999));
            base.Remove(Object);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            this.Tile = Object.Render.Tile;
            this.RenderString = Object.Render.RenderString;
            Registrar.Register("BeginTakeAction");
            base.Register(Object, Registrar);
        }

        public override bool Render(RenderEvent E)
        {
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction" && this.Duration > 0)
            {
                if (this.Duration != 9999)
                    --this.Duration;
                    if (this.Duration > 0 && this.Object.IsPlayer())
                        Effect.AddPlayerMessage("You will phase back out in " + Duration.Things("round") + ".");
                    // Effect.AddPlayerMessage("You will phase back out in " + Grammar.Cardinal(this.Duration - 1) + " " + (this.Duration - 1 != 1 ? "turns" : "turn") + ".");
            }
            return base.FireEvent(E);
        }
    }
}
