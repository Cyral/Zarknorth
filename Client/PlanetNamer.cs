using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyral.Extensions;

namespace ZarknorthClient
{
    public class PlanetNamer
    {
        private static List<string> Prefixes = new List<string>() { "Val", "Rhas", "Los", "Phi", "Der", "Shi", "Xo", "Ker", "Delta", "Beta", "Alpha", "Astro", "Cosmo", "Alo", "Dre", "Illumo", "Proxima", "Epi", "Ceti", "Aurora", "Procryon", "Antares", "Maia", "Wespe", "Alkaid", "Hydra", "Leo", "Theta", "Light", "Dark", "Gaia", "Nynx", "Dux", "Haliya", "Myn", "Great", "Surrex", "Sol", "Prime", "Orion", "Region", "Grim", "Dephax", "Praxion", "Zero", "Main", "Mark", "Sky", "Black", "Spynx", "Frost", "Halcyon", "Powion", "Virgil", "Rust", "Ghis", "Kyro", "Endo", "Core", "Ivy", "Ano", "Hex", "Proxima", "Xero", "Lane", "Mana", "Orbit", "Nothar", "Tor", "Yao", "Dek", "Ralt", "Vos", "Maor", "Vrog", "Le", "Bravo", "Fox", "Delta", "Proxima" };
        private static List<string> GreekSuffixes = new List<string>() { "Alpha","Beta","Gamma","Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega"};
        private static List<string> Name1 = new List<string>() { "Aten", "Arctu", "Alo", "Azra", "Bre", "Blo", "Belwa", "Chi", "Cree", "Drasi", "Dulo", "Denva", "Elo", "Illu", "Andro", "Rava", "Wo", "Kuro", "Litu", "Cy", "Ky", "Mello", "Right", "Chora", "Hori", "Silver", "Copper", "Hell", "Ash", "Dai", "Day", "Transa", "Le", "Siki", "Sikara", "Dune", "Moa", "Vite", "Edge", "Alura", "Luste", "Uni", "He", "Ade", "Kill", "Davi", "Fairie", "Stale", "Quarti", "A", "E", "I", "O", "U", "Y", "Ja", "De", "Le", "Mc", "Co", "Eco", "Globa", "Entry", "Pro", "Cable", "Cyrus", "Enji", "Oceo", "Bio", "Ni", "Hex", "Now", "Ju", "Ano", "Kayne", "Taze", "Hoessa", "Sharpe", "Emeri", "Uno", "Zen", "Xoru", "Klato", "Azdore", "Yuti", "Athera", "Mata", "Gredo", "Virda", "Badou", "Nete", "Nep", "Ulta", "Seti", "Aloe", "Mona", "Luna", };
        private static List<string> NameInput = new List<string>() { "Ken", "Mos", "Let", "Lyte", "Tol", "Zin", "Frost", "Vek", "Zor", "Lin", "Lion", "Ran", "Zoh", "Gita", "Tam", "Beal", "Space", "Aurora", "Aura", "Aurora", "Aura", "Wave", "Kit", "Cygni", "Cygni", "Pi", "Path", "Ton", "Bash", "Rent", "West", "North", "Meda", "Shema", "Pine", "Ter", "Zark", "Wood", "Heed", "Ron", "Gon", "Cille", "Quita", "Visa", "Noia", "Die", "Verse", "Zon", "Vis", "Dia", "Gion", "Mium", "Sam", "Pher", "Lios", "Mas", "Roy", "Mex", "Mite", "Jax", "Nox", "Core", "Chin", "Knight", "Night", "Tar", "Con", "Ram", "Reach", "Dove", "Mate", "Leaf", "Das", "Leonis", "Might", "Soul", "Fine", "Luct", "Dow", "Bris", "Field", "World", "Net", "Bal", "Pros", "Per", "Lumin", "Lost", "Lite", "Xam", "Shaw", "Ford", "Zeus", "Titan", "Toe", "Hami", "Lex", "Rex", "Zazan", "Lok", "Twande", "Ralve", "Metro", "Centuri", "Argei", "Leigh", "Forn" };
        private static List<string> Name3 = new List<string>() { "Minor", "Major", "Great", "Core", "Prime", "X", "HD", };
        private const double PrefixChance = .5;
        private const double Body2Chance = .75;
        private const double Body3Chance = .13f;
        private const double NumberInCaseOfPrefix = .35f;
        private const double DashChance = .3f;

        private static Random random;

        static PlanetNamer()
        {
            random = new Random();
        }

        public static string NamePlanet()
        {
            //Choose a body
            string Name = "";
            if (random.NextDouble() < PrefixChance)
            {
                if (random.NextDouble() < NumberInCaseOfPrefix)
                {
                    Name += (random.NextBoolean() ? random.Next(0, 100) : random.Next(1000, 10000));
                    Name += random.NextDouble() < DashChance ? "-" : " ";
                }
                else
                    Name += Prefixes[random.Next(0, Prefixes.Count)] + " ";
            }
            string Body1 = "";
            if (random.NextDouble() > Body2Chance) //Single body
            {
                if (random.Next(0, 2) == 2) //Choose random list
                {
                    Name += Name1[random.Next(0, Name1.Count)];
                }
                else
                {
                    Name += NameInput[random.Next(0, NameInput.Count)];
                }
            }
            else //Multibody
            {
                int name = random.Next(0, Name1.Count);
                Name += Name1[name];
                Body1 += Name1[name];
                //Put the second word in lowercase, unless it is 2 letters, like DeAstro or McRazox.
                int rand= random.Next(0, NameInput.Count);
                if (Body1.Length == 2 && random.NextDouble() > .5)
                    Name += NameInput[rand];
                else
                    Name += NameInput[rand].ToLower();
            }
            if (random.NextDouble() < Body3Chance)
            {
                Name += " " + Name3[random.Next(0, Name3.Count)];
            }
            return Name;
        }
        public static string AddSuffix(NameStyle nameStyle, string name, int i)
        {
            if (i < 1)
                return string.Empty;

            StringBuilder sb = new StringBuilder(name);
            sb.Append(" ");

            switch (nameStyle)
            {
                case NameStyle.Numerical:
                    sb.Append(i);
                    break;
                case NameStyle.Alphabetical:
                    sb.Append((Char)((65) + (i - 1)));
                    break;
                case NameStyle.Greek:
                    sb.Append(GreekSuffixes[i - 1]);
                    break;
                case NameStyle.Roman:
                    sb.Append(i.ToRoman());
                    break;
            }

            return sb.ToString().Trim();
        }
        public enum NameStyle
        {
            Roman,
            Greek,
            Numerical,
            Alphabetical,
        }
    }
}
