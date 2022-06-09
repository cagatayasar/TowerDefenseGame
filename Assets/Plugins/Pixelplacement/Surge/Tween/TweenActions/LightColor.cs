﻿/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
{
    class LightColor : TweenBase
    {
        //Public Properties:
        public Color EndValue {get; private set;}

        //Private Variables:
        Light _target;
        Color _start;

        //Constructor:
        public LightColor (Light target, Color endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Func<float, float> easeFunc, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.LightColor, target.GetInstanceID (), duration, delay, obeyTimescale, curve, easeFunc, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.color;
            return true;
        }

        protected override void Operation (float percentage)
        {
            Color calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.color = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.color = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.color = EndValue;
            EndValue = _start;
            _start = _target.color;
        }
    }
}