using System;
using UnityEngine;

namespace Masot.Standard.Input.EventArguments
{
    /// <summary>
    /// base class for all other eventArguments
    /// </summary>
    public class EventArgsBase : EventArgs
    {
        //make a method that check whetver anyone inheriting from this base modifies the input or uses it
        //or make an event that fires on input modify and make it abstract or virtual
        public IInputData InputData { get; }

        public EventArgsBase(IInputData inputData)
        {
            Debug.Assert(inputData != null, "Input is null");
            InputData = inputData!;
        }
    }
}
