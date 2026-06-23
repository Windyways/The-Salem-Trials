using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Shroud : Role
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
                    CharacterData outcastCharacter = outcasts[UnityEngine.Random.Range(0, outcasts.Count)].dataRef;
                    return outcastCharacter;
                }
            }

            // Bluff as an in-play Villager
            Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>();
            villagers = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            villagers = Characters.Instance.FilterBluffableCharacters(villagers);
            villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Villager);

            CharacterData villageCharacter = villagers[UnityEngine.Random.Range(0, villagers.Count)].dataRef;
            return villageCharacter;
        }
    }
}
