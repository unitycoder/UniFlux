/*
Copyright (c) 2023 Xavier Arpa López Thomas Peter ('Kingdox')

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
namespace Kingdox.UniFlux.Core.Internal
{
    ///<summary>
    /// This class represents an implementation of an IFlux interface with a TKey key and an action without parameters.
    ///</summary>
    internal sealed class ActionFlux<TKey> :  IFlux<TKey, Action>
    {
        /// <summary>
        /// A dictionary that stores functions with no parameters
        /// </summary>
        // internal Dictionary<TKey, Action> dictionary = new Dictionary<TKey, Action>();
        // internal Dictionary<TKey, List<Action>> dictionary = new Dictionary<TKey, List<Action>>();
        internal Dictionary<TKey, HashSet<Action>> dictionary = new Dictionary<TKey, HashSet<Action>>();
        /// <summary>
        /// A Read Only dictionary wich contains dictionary field
        /// </summary>
        // internal readonly IReadOnlyDictionary<TKey, Action> dictionary_read = null;
        internal readonly IReadOnlyDictionary<TKey, HashSet<Action>> dictionary_read = null;
        /// <summary>
        /// Constructor of ActionFLux
        /// </summary>
        public ActionFlux()
        {
            dictionary_read = dictionary;
        }
        ///<summary>
        /// Subscribes an event to the action dictionary if the given condition is met
        ///</summary>
        ///<param name="condition">Condition that must be true to subscribe the event</param>
        ///<param name="key">Key of the event to subscribe</param>
        ///<param name="action">Action to execute when the event is triggered</param>
        void IStore<TKey, Action>.Store(in bool condition, TKey key, Action action)
        {
            if(dictionary_read.TryGetValue(key, out var values))
            {
                if (condition) values.Add(action);
                else values.Remove(action);
            }
            else if (condition) dictionary.Add(key, new HashSet<Action>(){action});
            // if(dictionary_read.TryGetValue(key, out var values))
            // {
            //     if (condition) values.Add(action);
            //     else values.Remove(action);
            // }
            // else if (condition) dictionary.Add(key, new List<Action>(){action});
            // if(dictionary_read.ContainsKey(key))
            // {
            //     if (condition) dictionary[key] += action;
            //     else dictionary[key] -= action;
            // }
            // else if (condition) dictionary.Add(key, action);
        }
        ///<summary>
        /// Triggers the function stored in the dictionary with the specified key. 
        ///</summary>
        void IFlux<TKey, Action>.Dispatch(TKey key)
        {
            // if(dictionary_read.TryGetValue(key, out var _actions)) _actions?.Invoke();
            // if(dictionary_read.TryGetValue(key, out var _actions)) 
            // {
            //     for (int i = 0; i < _actions.Count; i++) _actions[i].Invoke();
            // }
            if(dictionary_read.TryGetValue(key, out var _actions)) 
            {
                foreach (var item in _actions) item.Invoke();
            }
        }
    }
}
//Hashtable<TAction>