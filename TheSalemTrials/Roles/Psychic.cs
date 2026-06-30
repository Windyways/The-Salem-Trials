using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Psychic : Role 
    {
        public Psychic() : base(ClassInjector.DerivedConstructorPointer<Psychic>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Psychic(System.IntPtr ptr) : base(ptr)
        {

        }
        public override ActedInfo GetInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> displaychars = new Il2CppSystem.Collections.Generic.List<Character>();
            displaychars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            displaychars = Characters.Instance.FilterRevealedCharacters(displaychars);

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterRevealedCharacters(chars);
            chars = Characters.Instance.FilterAlignmentCharacters(chars, EAlignment.Evil);

            int revealedEvils = chars.Count;

            string info = ConjourInfo(revealedEvils);
            ActedInfo newInfo = new ActedInfo(info, displaychars);
            return newInfo;
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> displaychars = new Il2CppSystem.Collections.Generic.List<Character>();
            displaychars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            displaychars = Characters.Instance.FilterRevealedCharacters(displaychars);

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterRevealedCharacters(chars);
            chars = Characters.Instance.FilterAlignmentCharacters(chars, EAlignment.Evil);

            int revealedEvils = Ext.MakeNumberWrong(chars.Count, 2, 1);

            string info = ConjourInfo(revealedEvils);
            ActedInfo newInfo = new ActedInfo(info, displaychars);

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

        public string ConjourInfo(int revealedEvils)
        {
            if (revealedEvils == 0) return $"There are {revealedEvils}\nRevealed Evils.";
            else if (revealedEvils == 1) return $"There is {revealedEvils}\nRevealed Evil.";
            return $"There are {revealedEvils}\nRevealed Evils.";
        }
    }
}
