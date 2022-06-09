/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
{
    class Rotation : TweenBase
    {
        //Public Properties:
        public Vector3 EndValue {get; private set;}

        //Private Variables:
        Transform _target;
        Vector3 _start;

        //Constructor:
        public Rotation (Transform target, Vector3 endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Func<float, float> easeFunc, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.Rotation, target.GetInstanceID (), duration, delay, obeyTimescale, curve, easeFunc, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.eulerAngles;
            return true;
        }

        protected override void Operation (float percentage)
        {
            Quaternion calculatedValue = Quaternion.Euler (TweenUtilities.LinearInterpolateRotational (_start, EndValue, percentage));
            _target.rotation = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.eulerAngles = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.eulerAngles = EndValue;
            EndValue = _start;
            _start = _target.eulerAngles;
        }
    }
}