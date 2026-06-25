using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Shroud : Role // Ngl i love this one, proud of it too:pleading:
    {
        public Shroud() : base(ClassInjector.DerivedConstructorPointer<Shroud>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Shroud(System.IntPtr ptr) : base(ptr)
        {

        }

        public override CharacterData GetBluffIfAble(Character charRef)
        {
            /*// --- Test Behaviour ---
            // Bluff as an in-play Minion (TEST)
            Il2CppSystem.Collections.Generic.List<Character> minions = new Il2CppSystem.Collections.Generic.List<Character>();
            minions = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            minions = Characters.Instance.FilterBluffableCharacters(minions);
            minions = Characters.Instance.FilterCharacterType(minions, ECharacterType.Minion);

            if (minions.Count > 0)
            {
                Character minionCharacter = minions[UnityEngine.Random.Range(0, minions.Count)];
                if (minionCharacter != null) return minionCharacter.dataRef;
            }*/

            // --- Normal Behaviour ---
            int diceRoll = Calculator.RollDice(10);
            if (diceRoll < 5)
            {
                // Bluff as an in-play Outcast
                Il2CppSystem.Collections.Generic.List<Character> outcasts = new Il2CppSystem.Collections.Generic.List<Character>();
                outcasts = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
                outcasts = Characters.Instance.FilterBluffableCharacters(outcasts);
                outcasts = Characters.Instance.FilterCharacterType(outcasts, ECharacterType.Outcast);

                if (outcasts.Count > 0)
                {
                    Character outcastCharacter = outcasts[UnityEngine.Random.Range(0, outcasts.Count)];
                    if (outcastCharacter != null) return outcastCharacter.dataRef;
                }
            }

            // Bluff as an in-play Villager
            Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>();
            villagers = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            villagers = Characters.Instance.FilterBluffableCharacters(villagers);
            villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Villager);

            Character villageCharacter = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            if (villageCharacter != null) return villageCharacter.dataRef;

            return base.GetBluffIfAble(charRef); // Need a fallback on this someday. This is OK-ish for now...
        }
    }
}
