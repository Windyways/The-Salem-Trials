using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace TheSalemTrials
{
    public class Inquisitor : Role
    {
        public override Il2CppSystem.Collections.Generic.List<SpecialRule> GetRules()
        {
            Il2CppSystem.Collections.Generic.List<SpecialRule> sr = new Il2CppSystem.Collections.Generic.List<SpecialRule>();
            sr.Add(new NightModeRule(4));
            return sr;
        }

        Character chRef;
        private Il2CppSystem.Action action1;
        private Il2CppSystem.Action action2;
        private Il2CppSystem.Action action3;
        public override ActedInfo GetInfo(Character charRef)
        {
            return new ActedInfo("", null);
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            return new ActedInfo("", null);
        }

        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger == ETriggerPhase.Start)
            {                
                // --- Heretics 1 ---
                Il2CppSystem.Collections.Generic.List<Character> evils = new Il2CppSystem.Collections.Generic.List<Character>();
                evils = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
                evils = Characters.Instance.FilterAlignmentCharacters(evils, EAlignment.Evil);
                evils = Characters.Instance.FilterCharacterMissingStatus(evils, HereticStatus.heretic);
                evils = Characters.Instance.RemoveCharacterType<Inquisitor>(evils);

                Il2CppSystem.Collections.Generic.List<Character> goods = new Il2CppSystem.Collections.Generic.List<Character>();
                goods = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
                goods = Characters.Instance.FilterAlignmentCharacters(goods, EAlignment.Good);
                goods = Characters.Instance.FilterCharacterMissingStatus(goods, HereticStatus.heretic);
                goods = Characters.Instance.RemoveCharacterType<Inquisitor>(goods);

                if (evils.Count > 0)
                {
                    Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];
                    evil.statuses.AddStatus(HereticStatus.heretic, charRef);

                    evils.Remove(evil);
                }

                if (goods.Count > 0)
                {
                    Character good = goods[UnityEngine.Random.Range(0, goods.Count)];
                    good.statuses.AddStatus(HereticStatus.heretic, charRef);

                    goods.Remove(good);
                }

                // --- Heretics 2 ---
                if (evils.Count > 0)
                {
                    Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];
                    evil.statuses.AddStatus(HereticStatus.heretic, charRef);
                }

                if (goods.Count > 0)
                {
                    Character good = goods[UnityEngine.Random.Range(0, goods.Count)];
                    good.statuses.AddStatus(HereticStatus.heretic, charRef);
                }
            }

            if (trigger != ETriggerPhase.Day) return;
            chRef = charRef;
            CharacterPicker.Instance.StartPickCharacters(1, charRef);
            CharacterPicker.OnCharactersPicked += action1;
            CharacterPicker.OnStopPick += action2;
        }

        public override void BluffAct(ETriggerPhase trigger, Character charRef)
        {
            if (trigger == ETriggerPhase.Start)
            {
                // --- Heretics 1 ---
                Il2CppSystem.Collections.Generic.List<Character> evils = new Il2CppSystem.Collections.Generic.List<Character>();
                evils = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
                evils = Characters.Instance.FilterAlignmentCharacters(evils, EAlignment.Evil);
                evils = Characters.Instance.FilterCharacterMissingStatus(evils, HereticStatus.heretic);
                evils = Characters.Instance.RemoveCharacterType<Inquisitor>(evils);

                Il2CppSystem.Collections.Generic.List<Character> goods = new Il2CppSystem.Collections.Generic.List<Character>();
                goods = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
                goods = Characters.Instance.FilterAlignmentCharacters(goods, EAlignment.Good);
                goods = Characters.Instance.FilterCharacterMissingStatus(goods, HereticStatus.heretic);
                goods = Characters.Instance.RemoveCharacterType<Inquisitor>(goods);

                if (evils.Count > 0)
                {
                    Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];
                    evil.statuses.AddStatus(HereticStatus.heretic, charRef);

                    evils.Remove(evil);
                }

                if (goods.Count > 0)
                {
                    Character good = goods[UnityEngine.Random.Range(0, goods.Count)];
                    good.statuses.AddStatus(HereticStatus.heretic, charRef);

                    goods.Remove(good);
                }

                // --- Heretics 2 ---
                if (evils.Count > 0)
                {
                    Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];
                    evil.statuses.AddStatus(HereticStatus.heretic, charRef);
                }

                if (goods.Count > 0)
                {
                    Character good = goods[UnityEngine.Random.Range(0, goods.Count)];
                    good.statuses.AddStatus(HereticStatus.heretic, charRef);
                }
            }

            if (trigger != ETriggerPhase.Day) return;
            chRef = charRef;
            CharacterPicker.Instance.StartPickCharacters(1, charRef);
            CharacterPicker.OnCharactersPicked += action1;
            CharacterPicker.OnStopPick += action2;
        }

        private void CharacterPicked()
        {
            CharacterPicker.OnCharactersPicked -= action1;
            CharacterPicker.OnStopPick -= action2;

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars.Add(CharacterPicker.PickedCharacters[0]);

            string info = ConjourInfo(chars[0], chars[0].IsHeretic(), chars[0].alignment == EAlignment.Good);
            onActed?.Invoke(new ActedInfo(info, chars));
        }

        private void CharacterPickedLiar()
        {
            CharacterPicker.OnCharactersPicked -= action3;
            CharacterPicker.OnStopPick -= action2;

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars.Add(CharacterPicker.PickedCharacters[0]);

            string info = ConjourInfo(chars[0], !chars[0].IsHeretic(), chars[0].alignment != EAlignment.Good);
            onActed?.Invoke(new ActedInfo(info, chars));
        }

        private void StopPick()
        {
            CharacterPicker.OnCharactersPicked -= action1;
            CharacterPicker.OnStopPick -= action2;
            CharacterPicker.OnCharactersPicked -= action3;
        }

        public Inquisitor() : base(ClassInjector.DerivedConstructorPointer<Inquisitor>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
            action1 = new System.Action(CharacterPicked);
            action2 = new System.Action(StopPick);
            action3 = new System.Action(CharacterPickedLiar);
        }

        public Inquisitor(System.IntPtr ptr) : base(ptr)
        {
            action1 = new System.Action(CharacterPicked);
            action2 = new System.Action(StopPick);
            action3 = new System.Action(CharacterPickedLiar);
        }

        public string ConjourInfo(Character c, bool heretic, bool isGood)
        {
            string keyword = isGood ? "a Good" : "an Evil";

            if (heretic) return $"#{c.id}\nis {keyword} Heretic!";
            return $"#{c.id}\nis not a Heretic.";
        }
    }
}

public static class HereticStatus // Code base from SnakeCharmer from Demon Bluff Extension Pack.
{
    public static ECharacterStatus heretic = (ECharacterStatus)1615919000; // Changed the ID though so no worries!

    [HarmonyPatch(typeof(Character), nameof(Character.RevealAllReal))]
    public static class pvt
    {
        public static void Postfix(Character __instance)
        {
            if (__instance.statuses.Contains(heretic))
            {
                __instance.chName.text = __instance.dataRef.name.ToUpper() + "<color=#511432><size=18>\n<Heretic></color></size>";
            }
        }
    }
}