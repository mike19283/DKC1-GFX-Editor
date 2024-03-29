﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandAloneGFXDKC1
{
    public partial class ROM
    {
        public Dictionary<string, int> palettePointers = new Dictionary<string, int>() 
        {
            ["Donkey Kong 1P"] = 0x3C849A,
            ["Donkey Kong 1P (dimmed back-up)"] = 0x3C84B8,
            ["Donkey Kong 2P"] = 0x3C84D6,
            ["Donkey Kong 2P (dimmed back-up)"] = 0x3C84F4,
            ["Diddy Kong 1P"] = 0x3C8422,
            ["Diddy Kong 1P (dimmed back-up)"] = 0x3C8440,
            ["Diddy Kong 2P"] = 0x3C845E,
            ["Diddy Kong 2P (dimmed back-up)"] = 0x3C847C,
            ["dirt patch, bonus wall"] = 0x3C8226,
            ["DK Island seen from Gang-Plank Galleon"] = 0x3C8262,
            ["Cannonball"] = 0x3C829E,
            ["Necky Boss nut"] = 0x3C82BA,
            ["Necky Boss 1"] = 0x3C82D8,
            ["Necky Boss 2"] = 0x3C82F6,
            ["King K.Rool"] = 0x3C8314,
            ["arrow sign"] = 0x3C834E,
            ["Gnawty Boss 2"] = 0x3C83AA,
            ["Boss Dumb Drum"] = 0x3C83C8,
            ["Rambi, tire"] = 0x3C8404,
            ["Kritter, green"] = 0x3C8512,
            ["Kritter, purple"] = 0x3C8530,
            ["Kritter, brown"] = 0x3C854E,
            ["Kritter, blue"] = 0x3C856C,
            ["Kritter, yellow"] = 0x3C858A,
            ["Kritter, grey"] = 0x3C85A8,
            ["Klump"] = 0x3C85C6,
            ["Expresso"] = 0x3C85E4,
            ["Slippa"] = 0x3C8602,
            ["Winky"] = 0x3C863E,
            ["Army, rope"] = 0x3C865C,
            ["mine cart"] = 0x3C867A,
            ["Squidge"] = 0x3C869A,
            ["Necky"] = 0x3C86D4,
            ["bananas/KONG tokens/DK barrel logo"] = 0x3C86F2,
            ["animal crate, moss-lined barrel"] = 0x3C870E,
            ["barrels (normal, cannons)"] = 0x3C872E,
            ["Klaptrap, green"] = 0x3C876A,
            ["Klaptrap, purple"] = 0x3C87A6,
            ["tire (dark), mincer"] = 0x3C881E,
            ["giant banana"] = 0x3C885A,
            ["red balloon"] = 0x3C8878,
            ["green balloon"] = 0x3C8896,
            ["blue balloon"] = 0x3C88B4,
            ["cave platform"] = 0x3C88D2,
            ["pink butterfly"] = 0x3C894A,
            ["animal tokens"] = 0x3C89A4,
            ["Krusha, blue"] = 0x3C89C2,
            ["Krusha, grey"] = 0x3C89E0,
            ["Gnawty + Gnawty boss"] = 0x3C8A1C,
            ["mine shaft platform"] = 0x3C8A3A,
            ["Manky"] = 0x3C8A58,
            ["Chomps"] = 0x3C8AB2,
            ["Chomps Jr."] = 0x3C8AD0,
            ["Piranha"] = 0x3C8AEE,
            ["Croctopus, blue"] = 0x3C8B2A,
            ["Croctopus, purple"] = 0x3C8B48,
            ["Enguarde"] = 0x3C8B84,
            ["tank platform, green light"] = 0x3C8BA2,
            ["tank platform, orange light"] = 0x3C8BC0,
            ["tank platform, red light"] = 0x3C8BDE,
            ["fuel"] = 0x3C8C38,
            ["Clam"] = 0x3C8C92,
            ["Rockkroc"] = 0x3C8D64,
            ["Zinger, yellow"] = 0x3C8D82,
            ["Zinger, pink"] = 0x3C8DA0,
            ["Zinger, orange"] = 0x3C8DBE,
            ["Zinger, purple"] = 0x3C8DDC,
            ["Zinger, blue"] = 0x3C8DFA,
            ["Zinger, green"] = 0x3C8E18,
            ["blue rope"] = 0x3C8EEA,
            ["Temple bonus wall"] = 0x3C8E54,
            ["ice cave bonus wall"] = 0x3C8F08,
            ["purple rope"] = 0x3C8F26,
            ["iron barrel, oil drum"] = 0x3C8F44,
            ["Squawks"] = 0x3C8F80,
            ["Funky Kong"] = 0x3C8FBC,
            ["Funky Kong's surf board"] = 0x3C8FDA,
            ["Candy Kong"] = 0x3C8FF8,
            ["Cranky Kong"] = 0x3C9016,
            ["Unused Palette #1"] = 0x3cfb4b,
            ["Unused Palette #2"] = 0x3cfb69,
            ["Unused Palette #3"] = 0x3cfb87,
            ["Unused Palette #4"] = 0x3cfba5,
            ["Unused Palette #5"] = 0x3cfbc3,
            ["Unused Palette #6"] = 0x3cfbe1,
            ["Unused Palette #7"] = 0x3cfbff,
            ["Unused Palette #8"] = 0x3cfc1d,
            ["Unused Palette #9"] = 0x3cfc3b,
            ["Unused Palette #10"] = 0x3cfc59,
            ["Unused Palette #11"] = 0x3cfc77,
            ["Unused Palette #12"] = 0x3cfc95,
            ["Unused Palette #13"] = 0x3cfcb3,
            ["Unused Palette #14"] = 0x3cfcd1,
            ["Unused Palette #15"] = 0x3cfcef,
            ["Unused Palette #16"] = 0x3cfd0d,
            ["Unused Palette #17"] = 0x3cfd2b,
            ["Unused Palette #18"] = 0x3cfd49,
            ["Unused Palette #19"] = 0x3cfd67,
            ["Unused Palette #20"] = 0x3cfd85,
            ["Unused Palette #21"] = 0x3cfda3,
            ["Unused Palette #22"] = 0x3cfdc1,
            ["Unused Palette #23"] = 0x3cfddf,
            ["Unused Palette #24"] = 0x3cfdfd,
            ["Unused Palette #25"] = 0x3cfe1b,
            ["Unused Palette #26"] = 0x3cfe39,
            ["Unused Palette #27"] = 0x3cfe57,
            ["Unused Palette #28"] = 0x3cfe75,
            ["Unused Palette #29"] = 0x3cfe93,
            ["Unused Palette #30"] = 0x3cfeb1,
            ["Unused Palette #31"] = 0x3cfecf,
            ["Unused Palette #32"] = 0x3cfeed,
            ["Unused Palette #33"] = 0x3cff0b,
            ["Unused Palette #34"] = 0x3cff29,
            ["Unused Palette #35"] = 0x3cff47,
            ["Unused Palette #36"] = 0x3cff65,
            ["Unused Palette #37"] = 0x3cff83,
            ["Unused Palette #38"] = 0x3cffa1,
            ["Unused Palette #39"] = 0x3cffbf,
            ["Unused Palette #40"] = 0x3cffdd,

        };

        public string[] GetKeys ()
        {
            return palettePointers.Keys.ToArray();
        }

    }
}
