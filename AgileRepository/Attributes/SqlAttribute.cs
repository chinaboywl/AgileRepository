﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Repository.Data;
using Agile.Repository.Proxy;
using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;

namespace Agile.Repository.Attributes
{
    public abstract class SqlAttribute : AbstractInterceptorAttribute
    {
        private string _connectionName;

        public string ConnectionName
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionName))
                {
                    return _connectionName;
                }
                if (!string.IsNullOrEmpty(AgileRepository.Config.ConnectionName))
                {
                    return AgileRepository.Config.ConnectionName;
                }

                return ConnectionConfig.DefaultConnName;
            }
            set => _connectionName = value;
        }

        public abstract override Task Invoke(AspectContext context, AspectDelegate next);

        public string Provider => string.IsNullOrEmpty(ConnectionName)
            ? DbProviders.Sqlserver
            : ConnectionConfig.GetProviderName(ConnectionName);

        protected Dictionary<string, object> ToParamterDict(ParameterCollection paramters)
        {
            var queryParams = new Dictionary<string, object>();
            foreach (var parameter in paramters)
            {
                queryParams.Add(parameter.Name, parameter.Value);
            }

            return queryParams;
        }

        protected Type[] AgileRepositoryGenericTypeArguments(AspectContext context)
        {
            var gt = context.ServiceMethod.DeclaringType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAgileRepository<>))
                .GenericTypeArguments;

            return gt;
        }
    }
}
