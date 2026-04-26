using System;
using XRL.Language;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts.Mutation{
    [Serializable]
    public class AstralClaws : BaseDefaultEquipmentMutation{

        public override bool GeneratesEquipment(){
            return true;
        }

        public override string GetDescription(){
            return "You have sharp claws and a ferocious bite.";
        }

        public string GetBaseDamage(){
            return GetBaseDamage(Level);
        }

        public string GetBaseDamage(int Level){
            return (1 + (Level-1)/2) + "d2";
        }

        public int GetBonusPenetration(){
            return GetBonusPenetration(Level);
        }

        public int GetBonusPenetration(int Level){
            return (2 + Level/3);
        }

        public int DismemberChance(){
            return DismemberChance(Level);
        }

        public int DismemberChance(int Level){
            return 3 * Level;
        }

        public override string GetLevelText(int Level){
            return "Claws and bite do {{rules|" + GetBaseDamage(Level) + "}} damage, have {{rules|" + GetBonusPenetration(Level) + "}} bonus penetration, and have a {{rules|" + DismemberChance(Level) + "%}} chance to dismember";
        }

        public override void OnRegenerateDefaultEquipment(Body body){
            foreach (BodyPart ClawPart in body.GetPart("Astral Kitty Foot")){
                MeleeWeapon ClawWeapon = ClawPart.DefaultBehavior.GetPart<MeleeWeapon>();
                ClawWeapon.BaseDamage = GetBaseDamage();
                ClawWeapon.PenBonus = GetBonusPenetration();

                ClawPart.DefaultBehavior.GetPart<ModSerrated>().Chance = DismemberChance();
            }

            foreach (BodyPart BitePart in body.GetPart("Astral Kitty Jaw")){
                MeleeWeapon BiteWeapon = BitePart.DefaultBehavior.GetPart<MeleeWeapon>();
                BiteWeapon.BaseDamage = GetBaseDamage();
                BiteWeapon.PenBonus = GetBonusPenetration();

                BitePart.DefaultBehavior.GetPart<ModSerrated>().Chance = DismemberChance();
            }

            base.OnRegenerateDefaultEquipment(body);
        }

        // public void AddClaws(GameObject GO){
        //     // if (LeftClaw == null){
        //     //     LeftClaw = GameObject.Create("Kitty_Astral_Claw");
        //     // }
        //     //
        //     // if (RightClaw == null){
        //     //     RightClaw = GameObject.Create("Kitty_Astral_Claw");
        //     // }
        //     if (ClawParts == null){
        //         ClawParts = ParentObject.Body.GetPart(ClawPartType);
        //     }
        //
        //     if (Bite == null){
        //         Bite = GameObject.Create("Kitty_Astral_Bite");
        //     }
        //
        //
        // }

        public override bool Mutate(GameObject GO, int Level){
            // AddClaws(GO);
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO){
            return base.Unmutate(GO);
        }

    }
}
