import("stdfaust.lib");


// Violin model
violinModel(stringLength,bowPressure,bowVelocity,bowPosition) = pm.endChain(modelChain)
with{
    stringTuning = 0.08;
    stringL = stringLength-stringTuning;
    modelChain = pm.chain(
        violinNuts :
        bowedString(stringL,bowPressure,bowVelocity,bowPosition) :
        pm.violinBridge :
        violinBody :
        pm.out
    );
};

// Violin string
bowedString(stringLength, bowPressure, bowVelocity, bowPosition) = 
pm.chain(
    pm.stringSegment(maxStringLength,ntbd) :
    pm.violinBow(bowPressure, bowVelocity) :
    pm.stringSegment(maxStringLength,btbd)
)
with{
        maxStringLength = pm.maxLength;
        ntbd = stringLength*bowPosition;     // upper portion of the string length
        btbd = stringLength*(1-bowPosition); // lower portion of the string length
};


// String segment with interpolation
stringSegment(maxStringLength, length) = guide, guide, _
with {
    guide = interpDelay(nMax, 0.01, n, gate);
    nMax = pm.maxLength : pm.l2s;
    n = length : pm.l2s/2;
};



// Violin body
violinBody = reflectance, transmittance, _
with{
    transmittance = fi.resonbp(Res,2,1);
    reflectance = _;
};

violinNuts = pm.lTermination(-1*pm.bridgeFilter(0.7,0.1),pm.basicBlock);

// Interface
freq = hslider("freq",440, 300, 1000, 0.1) : si.smoo;
L = 340/(freq);

// V = hslider("Velocity", 0.2, 0, 1, 0.01) : si.smoo;
Pos = hslider("Possition", 0.2, 0, 1, 0.01) : si.smoo;
oscRes = hslider("Resonance Oscilation", 0.5, 0, 1, 0.01) : si.smoo;
// Res =500;
Res = 1.5*freq + freq*oscRes*os.osc(0.5);
gain = hslider("gain", 0, -96, 0, 0.1) : ba.db2linear : si.smoo;
gate = 1;



velocityAttack = hslider("Velocity attack",1,0.1,5,0.01);
velocityOsc = hslider("Bowing frequency",3,0,10,0.01);

P = hslider("preasure", 0.2, 0, 1, 0.01) * en.ar(0.01,2,gate)*os.osc(velocityOsc);
s = os.osc(velocityOsc*en.asr(2,1,2,gate));
bowingInput = en.asr(velocityAttack,gain/2,2,gate)*s;


// process 
process =  violinModel(L,P,bowingInput,Pos)<: _,_;
//process =  violinModel(L,P,bowingInput,Pos),violinModel(L*2/3,P,bowingInput,Pos):>_/2<: _,_;


// interpolate Delay
interpDelay(m,interp,del,t) = _ <: de.fdelay4(m,del0)*xfade, de.fdelay4(m,del1)*(1-xfade) :> _
with{
    switch = t' : ba.impulsify : +~%(2);
    del0 = del : ba.sAndH(1-switch);
    del1 = del : ba.sAndH(switch);
    xfade = en.asr(interp,1,interp,switch);
};


    