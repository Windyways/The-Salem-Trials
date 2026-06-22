using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Cultist : Role
    {
        public Cultist() : base(ClassInjector.DerivedConstructorPointer<Cultist>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Cultist(System.IntPtr ptr) : base(ptr)
        {

        }

        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Start) return;

            Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>();
            villagers = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Minion);

            Character pickedVillager = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            pickedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

            villagers.Remove(pickedVillager);
            Character replacedVillager = villagers[UnityEngine.Random.Range(0, villagers.Count)];


            replacedVillager.InitWithNoReset(pickedVillager.GetCharacterBluffIfAble());
            replacedVillager.Act(ETriggerPhase.Start);
            replacedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

            /*Il2CppSystem.Collections.Generic.List<Character> viableCharacters = new Il2CppSystem.Collections.Generic.List<Character>();
            viableCharacters = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);

            Il2CppSystem.Collections.Generic.List<CharacterData> notInPlayOutsiders = Gameplay.Instance.GetAscensionAllStartingCharacters();
            notInPlayOutsiders = Characters.Instance.FilterNotInDeckCharactersUnique(notInPlayOutsiders);
            notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Villager);
            notInPlayOutsiders = Characters.Instance.FilterAlignmentCharacters(notInPlayOutsiders, EAlignment.Good);
            if (notInPlayOutsiders.Count == 0)
            {
                notInPlayOutsiders = Gameplay.Instance.GetAllAscensionCharacters();
                notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Villager);
                notInPlayOutsiders = Characters.Instance.FilterAlignmentCharacters(notInPlayOutsiders, EAlignment.Good);
            }

            CharacterData pickedOutsider = notInPlayOutsiders[UnityEngine.Random.Range(0, notInPlayOutsiders.Count - 1)];

            if (notInPlayOutsiders.Count != 0)
            {
                Gameplay.Instance.AddScriptCharacter(ECharacterType.Minion, pickedOutsider);

                viableCharacters = Characters.Instance.FilterAliveCharacters(viableCharacters);
                viableCharacters = Characters.Instance.FilterRealCharacterType(viableCharacters, ECharacterType.Villager);

                Character pickedCharacter = viableCharacters[UnityEngine.Random.Range(0, viableCharacters.Count)];
                pickedCharacter.Init(pickedOutsider);
                viableCharacters.Remove(pickedCharacter);
                notInPlayOutsiders.Remove(pickedOutsider);
            }*/
        }

        public override CharacterData GetBluffIfAble(Character charRef)
        {
            int diceRoll = Calculator.RollDice(10);
            if (diceRoll < 5)
            {
                // 100% Double Claim
                return Characters.Instance.GetRandomDuplicateBluff();
            }
            else
            {
                // Become a new character
                CharacterData bluff = Characters.Instance.GetRandomUniqueBluff();
                Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

                return bluff;
            }
        }
    }
}
