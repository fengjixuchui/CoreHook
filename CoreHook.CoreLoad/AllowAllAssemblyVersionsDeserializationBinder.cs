﻿using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace CoreHook.CoreLoad
{
    /// <summary>
    /// When not using the GAC, the BinaryFormatter fails to recognise the InParam
    /// when attempting to deserialise. 
    /// 
    /// A custom DeserializationBinder works around this (see http://spazzarama.com/2009/06/25/binary-deserialize-unable-to-find-assembly/)
    /// </summary>
    internal sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        private Assembly _assembly;

        public AllowAllAssemblyVersionsDeserializationBinder()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public AllowAllAssemblyVersionsDeserializationBinder(Assembly assembly)
        {
            _assembly = assembly;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            try
            {
                // 1. First try to bind without overriding assembly
                typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            }
            catch
            {
                // 2. Failed to find assembly or type, now try with overridden assembly
                typeToDeserialize = _assembly.GetType(typeName);
            }

            return typeToDeserialize;
        }
    }
}
