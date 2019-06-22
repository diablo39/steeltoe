﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Steeltoe.Stream.Binding
{
    public class BindableProxyGenerator
    {
        public static object CreateProxy(IBindableProxyFactory factory)
        {
            return GenerateProxy(factory);
        }

        internal static object GenerateProxy(IBindableProxyFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var generator = new ProxyGenerator();
            Func<MethodInfo, object> del = (m) => factory.Invoke(m);
            var proxy = generator.CreateInterfaceProxyWithoutTarget(factory.Binding, new BindingInterceptor(del));
            return proxy;
        }

        private class BindingInterceptor : IInterceptor
        {
            private readonly Delegate _impl;

            public BindingInterceptor(Delegate impl)
            {
                _impl = impl;
            }

            public void Intercept(IInvocation invocation)
            {
                var result = this._impl.DynamicInvoke(invocation.Method);
                invocation.ReturnValue = result;
            }
        }
    }
}
