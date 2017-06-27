﻿using System;
using System.Linq;
using System.Reflection;

namespace DotApiClient
{
    class WebApiDescription
    {
        public string BaseUrl { get; set; }
        public WebApiMethodDescriptions Methods { get; set; }

        public static WebApiDescription Create(Type contractType)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            CheckContractAttributes(contractType);
            CheckForgottenMethods(contractType);

            var wasAttr = contractType.GetTypeInfo().GetCustomAttribute<WebApiServiceAttribute>();

            var desc = new WebApiDescription
            {
                BaseUrl = wasAttr?.RelPath,
                Methods = new WebApiMethodDescriptions(
                    contractType.GetTypeInfo().FindMembers(
                            MemberTypes.Method,
                            BindingFlags.Instance | BindingFlags.Public,
                            (info, criteria) => info.GetCustomAttribute<EnpointBaseAttribute>() != null,
                            null)
                        .Cast<MethodInfo>()
                        .Select(WebApiMethodDescription.Create)
                )
            };

            CheckWrongMethodAttributes(contractType);

            return desc;
        }

        private static void CheckForgottenMethods(Type contractType)
        {
            if (contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType != typeof(object))
                .Any(m => !Attribute.IsDefined(m, typeof(EnpointBaseAttribute))))
            {
                throw new WebApiContractException("No all of methods is marked as a part of the web api contract");
            }
        }

        private static void CheckContractAttributes(Type contractType)
        {
            if (!Attribute.IsDefined(contractType, typeof(WebApiAttribute)) && 
                !Attribute.IsDefined(contractType, typeof(RestApiAttribute)))
                throw new WebApiContractException("Undefined contract type");
        }

        private static void CheckWrongMethodAttributes(Type contractType)
        {
            var methods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            if (Attribute.IsDefined(contractType, typeof(WebApiAttribute)) &&
                methods.Any(m => Attribute.IsDefined(m, typeof(RestActionAttribute))))
                throw new WebApiContractException();

            if (Attribute.IsDefined(contractType, typeof(RestApiAttribute)) &&
                methods.Any(m => Attribute.IsDefined(m, typeof(ServiceEndpointAttribute))))
                throw new WebApiContractException();
        }
    }
}