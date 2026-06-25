using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Starspawn : Role
    {
        public Starspawn() : base(ClassInjector.DerivedConstructorPointer<Starspawn>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Starspawn(System.IntPtr ptr) : base(ptr)
        {

        }

        public override CharacterData GetBluffIfAble(Character charRef)
        {
            // Bluff as an in-play Outcast
            Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>();
            villagers = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            villagers = Characters.Instance.FilterBluffableCharacters(villagers);
            villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Outcast);

            Character villageCharacter = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            if (villageCharacter == null) return base.GetBluffIfAble(charRef);

            return villageCharacter.dataRef;
        }
    }
}
