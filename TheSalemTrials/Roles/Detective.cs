using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace TheSalemTrials;

[RegisterTypeInIl2Cpp]
public class Detective : Role
{
    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        Il2CppSystem.Collections.Generic.List<Character> picked = new Il2CppSystem.Collections.Generic.List<Character>();
        Il2CppSystem.Collections.Generic.List<Character> allVillagers = new Il2CppSystem.Collections.Generic.List<Character>();
        allVillagers = Characters.Instance.FilterCharacterType(Gameplay.CurrentCharacters, ECharacterType.Villager);
        //allVillagers = Characters.Instance.RemoveCharacterType<Recluse>(allVillagers);

        allVillagers.Remove(charRef);
        Character pickedEvil = allVillagers[UnityEngine.Random.Range(0, allVillagers.Count)];
        picked.Add(pickedEvil);

        var closestEvil = GetClosestEvilToGood(pickedEvil, charRef);

        info = ConjourInfo(pickedEvil, closestEvil.Item1);
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

    public override ActedInfo GetBluffInfo(Character charRef) // Just gets a fake distance -.-
    {
        string info = "";

        Il2CppSystem.Collections.Generic.List<Character> picked = new Il2CppSystem.Collections.Generic.List<Character>();
        Il2CppSystem.Collections.Generic.List<Character> allVillagers = new Il2CppSystem.Collections.Generic.List<Character>();
        allVillagers = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
        //allVillagers = Characters.Instance.RemoveCharacterType<Recluse>(allVillagers);

        allVillagers.Remove(charRef);
        Character pickedEvil = allVillagers[UnityEngine.Random.Range(0, allVillagers.Count)];
        picked.Add(pickedEvil);

        var closestEvil = GetClosestGoodToRandom(pickedEvil, charRef);

        info = ConjourInfo(pickedEvil, closestEvil.Item1);
        ActedInfo newInfo = new ActedInfo(info, picked);
        return newInfo;
    }

    public (int, Character?) GetClosestEvilToGood(Character pickedEvil, Character chRef) // Scout code but modified.
    {
        int count = 0;
        int savedCount = 100;
        Character? closestEvil = null;

        Il2CppSystem.Collections.Generic.List<Character> myList = new Il2CppSystem.Collections.Generic.List<Character>();
        myList = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
        myList = CharactersHelper.GetSortedListWithCharacterFirst(myList, pickedEvil);

        myList.RemoveAt(0);
        for (int i = 0; i < myList.Count; i++)
        {
            if (myList[i].alignment == EAlignment.Evil)
            {
                closestEvil = myList[i];
                savedCount = count;
                count = 0;
                break;
            }
            count++;
        }
        count = 0;
        for (int i = myList.Count - 1; i > 0; i--)
        {
            if (myList[i].alignment == EAlignment.Evil)
            {
                if (count < savedCount)
                {
                    closestEvil = myList[i];
                    savedCount = count;
                    count = 0;
                }
                break;
            }
            count++;
        }

        return (savedCount, closestEvil);
    }

    public (int, Character?) GetClosestGoodToRandom(Character pickedEvil, Character chRef) // Scout code but modified.
    {
        int count = 0;
        int savedCount = 100;
        Character? closestEvil = null;

        Il2CppSystem.Collections.Generic.List<Character> myList = new Il2CppSystem.Collections.Generic.List<Character>();
        myList = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
        myList = CharactersHelper.GetSortedListWithCharacterFirst(myList, pickedEvil);

        myList.RemoveAt(0);
        for (int i = 0; i < myList.Count; i++)
        {
            if (myList[i].alignment == EAlignment.Good)
            {
                closestEvil = myList[i];
                savedCount = count;
                count = 0;
                break;
            }
            count++;
        }
        count = 0;
        for (int i = myList.Count - 1; i > 0; i--)
        {
            if (myList[i].alignment == EAlignment.Evil)
            {
                if (count < savedCount)
                {
                    closestEvil = myList[i];
                    savedCount = count;
                    count = 0;
                }
                break;
            }
            count++;
        }

        return (savedCount, closestEvil);
    }

    public string ConjourInfo(Character c, int steps)
    {
        string charName = c.GetRegisterAs().name;
        if (c.bluff != null) charName = c.bluff.name;

        if (steps > 20)
            return $"There are no valid Villagers.";
        else if (steps == 0)
            return $"{charName} is\n{steps + 1} card away\nfrom closest Evil\nto them.";
        else
            return $"{charName} is\n{steps + 1} cards away\nfrom closest Evil\nto them.";
    }

    public Detective() : base(ClassInjector.DerivedConstructorPointer<Detective>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }

    public Detective(System.IntPtr ptr) : base(ptr)
    {

    }
}