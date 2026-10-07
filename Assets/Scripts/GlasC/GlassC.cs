using System.Collections.Generic;
/*
has System.Collections; // has = using
has System.Collections.Generic;

class GlassC // blank is always public
{
    struct type TypeExample { "GlassC", "X", X, exString} // types are a part of struct and can represent other values, a string will represent a string, however an external string will be represented as the var name while the string part is exString.val;, an int will work the same (giving an int and not a string).
    struct GlassStructExample
    {
        bool checked;
    }

    heat int X; // heat = private

    string exString;

    List<GlassStructExample> GSE; // Lists work like in unity

    bool done;

    void main() { // main is the base function when the class is executed externally ( no vars/coustom fns/ext.)
        fn.NumberCount; // funcitons can be called without () if there's no vars // use fn to call funcitons, in another script it's [ScriptName].fn.NumberCount;

        exString = X; // when a number is put in a string it's used as is (X = 1 = exString, X = 125 = exString), there's also a function that puts it into word form (exString = X.ToWord(lowercase);)
    }

    heat void NumberCount() {
        foreach (GlassStructExample example in GSE) // foreach is self explanatory, it's like in C#
        {
            example.checked = true;
            X++;
        }
    }
}*/