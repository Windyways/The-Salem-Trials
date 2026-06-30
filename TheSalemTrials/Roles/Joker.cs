using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace TheSalemTrials
{
    public class Joker : Role
    {
        public Joker() : base(ClassInjector.DerivedConstructorPointer<Joker>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Joker(System.IntPtr ptr) : base(ptr)
        {

        }

        public override ActedInfo GetInfo(Character charRef)
        {
            Role role = infoRoles[UnityEngine.Random.Range(0, infoRoles.Count)];
            MelonLoader.MelonLogger.Msg($"Joker got info as {role.GetType()} (NOT BLUFFING)");
            ActedInfo newInfo = role.GetBluffInfo(charRef);
            return newInfo;
        }

        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetBluffInfo(charRef));
        }
        public override void BluffAct(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetBluffInfo(charRef));
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            Role role = infoRoles[UnityEngine.Random.Range(0, infoRoles.Count)];
            MelonLoader.MelonLogger.Msg($"Joker got info as {role.GetType()}");
            ActedInfo newInfo = role.GetBluffInfo(charRef);
            return newInfo;
        }

        public List<Role> infoRoles = new List<Role>()
        {
                // The Salem Trials
                new Cleric(),
                new Detective(),
                new Psychic(),
                new Sheriff(),
                new Sibyl(),

            /*new Empath(),
            new Scout(),
            new Investigator(),
            new Lookout(),
            new Knitter(),
            new Tracker(),
            new Shugenja(),
            new Noble(),
            new Bishop(),
            new Archivist(),
            new Acrobat2(),*/
        };

        /*public List<Role> infoRoles = new List<Role>() // Poet code
        {
            new Empath(), // Lover
            new Scout(),
            new Investigator(),
            new Lookout(), // Medium
            new Knitter(),
            new Tracker(),
            new Shugenja(), // Enlightened
            new Noble(), // Empress
            new Bishop(),
            new Archivist(), // Gemcrafter
            new Acrobat2(), // Bard

            // The Salem Trials
            new Cleric(),
            new Detective(),
            new Psychic(),
            new Sheriff(),
        */
        // Will add compatibility once i figure out how to check if the user is using the mod.
        // (totally not putting this comment for help or anything *wink* *wink*)
        // Vanilla ones don't even work so forget about it^
        // Will just have it mimic salem roles since they all are fine.

        /*// Demon Bluff Expansion Pack
        new ExpansionPack.w_Arithmetician(),
        new ExpansionPack.w_Balloonist(),
        new ExpansionPack.w_Chiromancer(),
        new ExpansionPack.w_Clairvoyant(),
        new ExpansionPack.w_Detective(),
        new ExpansionPack.w_Introvert(),
        new ExpansionPack.w_Jewelsmith(),
        new ExpansionPack.w_Knave(),
        new ExpansionPack.w_Lamb(),
        new ExpansionPack.w_Paperboy(),
        new ExpansionPack.w_Performer(),
        new ExpansionPack.w_Politician(),
        new ExpansionPack.w_Prince(),
        new ExpansionPack.w_Ranger(),
        new ExpansionPack.w_Sentinel(),
        new ExpansionPack.w_Sheriff(),
        new ExpansionPack.w_Spy(),

        // Riddler Mod
        new RiddlerMod.Riddler(),
        new RiddlerMod.Mathematician(),
        new RiddlerMod.Director(),
        new RiddlerMod.Scanner(),
        new RiddlerMod.Obsessor(),
        new RiddlerMod.Lawyer(),
        new RiddlerMod.Psychic(),
        new RiddlerMod.Weaver(),
        new RiddlerMod.Engineer(),
        new RiddlerMod.Governor(),
        new RiddlerMod.Cowboy(),
        new RiddlerMod.Officer(),
        new RiddlerMod.Surveyor(),
        new RiddlerMod.Tracker(),
        new RiddlerMod.Pioneer(),
        new RiddlerMod.Astronaut(),
        new RiddlerMod.Sharpshooter(),

        // Maybe i'll add compatibility to more mods later. This will do for now.*/
    };
}
