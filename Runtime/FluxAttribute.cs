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
using System.Linq;
using System.Reflection;
namespace Kingdox.UniFlux
{
    ///<summary>
    /// Class FluxAttribute, a custom attribute that mark a method to be subscribed in a flux.
    /// AllowMultiple is false to keep legibility
    ///</summary>
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class FluxAttribute : System.Attribute
    {
        ///<summary>
        /// Key provided to the attribute's constructor.
        ///</summary>
        public readonly object key;
        ///<summary>
        /// Constructor of the FluxAttribute class that takes a key as a parameter.
        ///</summary>
        public FluxAttribute(object key)
        {
            this.key = key;
        }
    }
}
namespace Kingdox.UniFlux.Core.Internal
{
    ///<summary>
    /// static class that ensure to handle the FluxAttribute
    ///</summary>
    internal static class FluxAttributeExtension
    {
        internal static readonly Type m_type_monoflux = typeof(MonoFlux);
        //
        internal static readonly Type m_type_flux = typeof(Core.Internal.Flux<>);
        internal static readonly Type m_type_flux_delegate = typeof(Action);
        internal static readonly string m_type_flux_method = nameof(Core.Internal.Flux<object>.Store);
        //
        internal static readonly Type m_type_fluxparam = typeof(Core.Internal.FluxParam<,>);
        internal static readonly Type m_type_fluxparam_delegate = typeof(Action<>);
        internal static readonly string m_type_fluxparam_method = nameof(Core.Internal.FluxParam<object,object>.Store);
        //
        internal static readonly Type m_type_fluxreturn = typeof(Core.Internal.FluxReturn<,>);
        internal static readonly Type m_type_fluxreturn_delegate = typeof(Func<>);
        internal static readonly string m_type_fluxreturn_method = nameof(Core.Internal.FluxReturn<object,object>.Store);
        //
        internal static readonly Type m_type_fluxparamreturn = typeof(Core.Internal.FluxParamReturn<,,>);
        internal static readonly Type m_type_fluxparamreturn_delegate = typeof(Func<,>);
        internal static readonly string m_type_fluxparamreturn_method = nameof(Core.Internal.FluxParamReturn<object,object,object>.Store);
        //
        ///<summary>
        /// typeof(void)
        ///</summary>
        internal static readonly Type m_type_void = typeof(void);
        ///<summary>
        /// Dictionary to cache each MonoFlux instance's methods
        ///</summary>
        internal static readonly Dictionary<MonoFlux, List<MethodInfo>> m_monofluxes = new Dictionary<MonoFlux, List<MethodInfo>>();
        ///<summary>
        /// Dictionary to cache the FluxAttribute of each MethodInfo
        ///</summary>
        internal static readonly Dictionary<MethodInfo, FluxAttribute> m_methods = new Dictionary<MethodInfo, FluxAttribute>();
        ///<summary>
        /// Allows subscribe methods using `FluxAttribute` by reflection
        /// ~ where magic happens ~
        ///</summary>
        internal static void Subscribe(this MonoFlux monoflux, in bool condition)
        {
            if (!m_monofluxes.ContainsKey(monoflux))
            {
                m_monofluxes.Add(
                    monoflux, 
                    monoflux.gameObject.GetComponent(m_type_monoflux).GetType().GetMethods((BindingFlags)(-1)).Where(method => 
                    {
                        if(System.Attribute.GetCustomAttributes(method).FirstOrDefault((_att) => _att is FluxAttribute) is FluxAttribute _attribute)
                        {
                            if(!m_methods.ContainsKey(method)) m_methods.Add(method, _attribute); // ADD <Method, Attribute>!
                            return true;
                        } 
                        else return false;
                    }).ToList()
                );
            }
            //
            List<MethodInfo> methods = m_monofluxes[monoflux];
            //
            for (int i = 0; i < methods.Count; i++) 
            {
                var _Parameters = methods[i].GetParameters();
                #if UNITY_EDITOR
                    if(_Parameters.Length > 1) // Auth Params is 0 or 1
                    {
                        throw new System.Exception($"Error '{methods[i].Name}' : Theres more than one parameter, please set 1 or 0 parameter. (if you need to add more than 1 argument use Tuples or create a struct, record o class...)");
                    }
                #endif
                                      // Activity
                Type keyType = m_methods[methods[i]].key.GetType();
                Type fluxType;
                Type delegateType;
                string methodName;
                //
                switch ((_Parameters.Length.Equals(1), !methods[i].ReturnType.Equals(m_type_void)))
                {
                    case (false, false): 
                        fluxType = typeof(Core.Internal.Flux<>).MakeGenericType(keyType);
                        delegateType = typeof(Action);
                        methodName = nameof(Core.Internal.Flux<object>.SubscribeAction);
                    break;
                    case (true, false): 
                        fluxType = typeof(Core.Internal.Flux<,>).MakeGenericType(keyType, _Parameters[0].ParameterType);
                        delegateType = typeof(Action<>).MakeGenericType(_Parameters[0].ParameterType); 
                        methodName = nameof(Core.Internal.Flux<object,object>.SubscribeActionParam);
                    break;
                    case (false, true): 
                        fluxType = typeof(Core.Internal.Flux<,>).MakeGenericType(keyType, methods[i].ReturnType);
                        delegateType = typeof(Func<>).MakeGenericType(methods[i].ReturnType); 
                        methodName = nameof(Core.Internal.Flux<object,object>.SubscribeFunc);
                    break;
                    case (true, true): 
                        fluxType = typeof(Core.Internal.Flux<,,>).MakeGenericType(keyType, _Parameters[0].ParameterType, methods[i].ReturnType);
                        delegateType = typeof(Func<,>).MakeGenericType(_Parameters[0].ParameterType, methods[i].ReturnType); 
                        methodName = nameof(Core.Internal.Flux<object,object,object>.SubscribeFuncParam);
                    break;
                }
                //Execute
                fluxType.GetMethod(methodName).Invoke(
                    null, 
                    new object[]
                    {
                        m_methods[methods[i]].key,
                        methods[i].CreateDelegate(delegateType, monoflux),
                        condition
                    }   
                );
                
                // Optimized 😎👍
                /*
                switch ((_Parameters.Length.Equals(1), !methods[i].ReturnType.Equals(m_type_void)))
                {
                    case (false, false): // Flux
                        m_type_flux.MakeGenericType(m_methods[methods[i]].key.GetType()).GetMethod(m_type_flux_method).Invoke( null, new object[]{ m_methods[methods[i]].key, methods[i].CreateDelegate(m_type_flux_delegate, monoflux), condition});
                    break;
                    case (true, false): // FluxParam
                        m_type_fluxparam.MakeGenericType(m_methods[methods[i]].key.GetType(), _Parameters[0].ParameterType).GetMethod(m_type_fluxparam_method).Invoke( null, new object[]{ m_methods[methods[i]].key, methods[i].CreateDelegate(m_type_fluxparam_delegate.MakeGenericType(_Parameters[0].ParameterType), monoflux), condition});
                    break;
                    case (false, true): //FluxReturn
                        m_type_fluxreturn.MakeGenericType(m_methods[methods[i]].key.GetType(), methods[i].ReturnType).GetMethod(m_type_fluxreturn_method).Invoke( null, new object[]{ m_methods[methods[i]].key, methods[i].CreateDelegate(m_type_fluxreturn_delegate.MakeGenericType(methods[i].ReturnType), monoflux), condition});
                    break;
                    case (true, true): //FluxParamReturn
                        m_type_fluxparamreturn.MakeGenericType(m_methods[methods[i]].key.GetType(), _Parameters[0].ParameterType, methods[i].ReturnType).GetMethod(m_type_fluxparamreturn_method).Invoke( null, new object[]{ m_methods[methods[i]].key, methods[i].CreateDelegate(m_type_fluxparamreturn_delegate.MakeGenericType(_Parameters[0].ParameterType, methods[i].ReturnType), monoflux), condition});
                    break;
                }
                */
            }
        }
    }
}
//TODO: C# 11 allow Attribute<T>, instead of object key
