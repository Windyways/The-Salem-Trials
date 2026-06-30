using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Cleric : Role
    {
        public Cleric() : base(ClassInjector.DerivedConstructorPointer<Cleric>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }

        public Cleric(System.IntPtr ptr) : base(ptr)
        {

        }

        public override ActedInfo GetInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> picked = new Il2CppSystem.Collections.Generic.List<Character>();
            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterCharacterType(chars, ECharacterType.Villager);
            chars = Characters.Instance.FilterAlignmentCharacters(chars, EAlignment.Good);
            chars = Characters.Instance.FilterCharacterContainsStatus(chars, ECharacterStatus.Corrupted);

            if (chars.Count == 0)
            {
                var info2 = ConjourInfo(null);
                ActedInfo newInfo2 = new ActedInfo(info2, chars);
                return newInfo2;
            }

            Character c = chars[UnityEngine.Random.Range(0, chars.Count)];
            picked.Add(c);
            c.statuses.CheckIfCanCurePoisonAndCure();
            //c.statuses.RemoveStatusIfAble(ECharacterStatus.Corrupted); // idk if this would work.

            var info = ConjourInfo(c);
            ActedInfo newInfo = new ActedInfo(info, picked);
            return newInfo;
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> picked = new Il2CppSystem.Collections.Generic.List<Character>();
            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterCharacterMissingStatus(chars, ECharacterStatus.Corrupted);

            Character c = chars[UnityEngine.Random.Range(0, chars.Count)];
            picked.Add(c);

            var info = ConjourInfo(c);
            ActedInfo newInfo = new ActedInfo(info, picked);
            return newInfo;
        }
        
        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetInfo(charRef));
        }

        public override void BluffAct(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetBluffInfo(charRef));
        }

        public string ConjourInfo(Character? c)
        {
            if (c != null) return $"I Cleansed #{c.id}\nof Corruption!";
            return $"There was no\nCorruption to Cleanse of.";
        }
    }
}
