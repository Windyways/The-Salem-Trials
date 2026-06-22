using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Pestilence : Role
    {
        public Pestilence() : base(ClassInjector.DerivedConstructorPointer<Pestilence>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Pestilence(System.IntPtr ptr) : base(ptr)
        {

        }

        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger == ETriggerPhase.Start)
            {
                PoisonRandomVillager(charRef);
            }
        }

        private void PoisonRandomVillager(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>();
            villagers = Characters.Instance.FilterCharacterType(Gameplay.CurrentCharacters, ECharacterType.Villager);
            villagers = Characters.Instance.FilterCharacterMissingStatus(villagers, ECharacterStatus.Corrupted);

            if (villagers.Count <= 0) return;

            Character randomCharacter = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            randomCharacter.statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
        }
    }
}
