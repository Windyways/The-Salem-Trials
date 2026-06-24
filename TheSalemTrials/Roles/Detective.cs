using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using MelonLoader;
using System;
using System.ComponentModel.Design;
using UnityEngine;

namespace TheSalemTrials;

[RegisterTypeInIl2Cpp]
public class Detective : Role
{
    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        Il2CppSystem.Collections.Generic.List<Character> allVillagers = new Il2CppSystem.Collections.Generic.List<Character>();
        allVillagers = Characters.Instance.FilterCharacterType(Gameplay.CurrentCharacters, ECharacterType.Villager);
        //allVillagers = Characters.Instance.RemoveCharacterType<Recluse>(allVillagers);

        allVillagers.Remove(charRef);
        Character pickedEvil = allVillagers[UnityEngine.Random.Range(0, allVillagers.Count)];

        var closestEvil = GetClosestEvilToEvil(pickedEvil, charRef);

        info = ConjourInfo(pickedEvil.GetRegisterAs().name, closestEvil.Item1);
        ActedInfo newInfo = new ActedInfo(info);
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

        Il2CppSystem.Collections.Generic.List<Character> allVillagers = new Il2CppSystem.Collections.Generic.List<Character>();
        allVillagers = Characters.Instance.FilterCharacterType(Gameplay.CurrentCharacters, ECharacterType.Villager);
        //allVillagers = Characters.Instance.RemoveCharacterType<Recluse>(allVillagers);

        allVillagers.Remove(charRef);
        Character pickedEvil = allVillagers[UnityEngine.Random.Range(0, allVillagers.Count)];

        var closestEvil = GetClosestEvilToEvil(pickedEvil, charRef);

        int fakeDist = closestEvil.Item1;
        if (fakeDist == 1) fakeDist = 2;
        else fakeDist -= 1;

        info = ConjourInfo(pickedEvil.GetRegisterAs().name, fakeDist);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public (int, Character?) GetClosestEvilToEvil(Character pickedEvil, Character chRef)
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

    public string ConjourInfo(string charName, int steps)
    {
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