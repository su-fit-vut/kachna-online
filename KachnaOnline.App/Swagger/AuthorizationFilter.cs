// AuthorizationFilter.cs
// Author: Ondřej Ondryáš

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KachnaOnline.App.Swagger
{
    public class AuthorizationFilter : IOperationFilter
    {
        private static IAuthorizeData[] ComputeAuthorizeData(MethodInfo controllerMethod)
        {
            if (controllerMethod is null or { DeclaringType: null })
                return null;

            var allAttributes = controllerMethod.GetCustomAttributes(true)
                .Union(controllerMethod.DeclaringType.GetCustomAttributes(true));

            List<IAuthorizeData> authorizeDatas = null;

            foreach (var t in allAttributes)
            {
                if (t is IAllowAnonymous)
                {
                    return null;
                }

                if (t is IAuthorizeData authorizeData)
                {
                    authorizeDatas ??= new List<IAuthorizeData>();
                    authorizeDatas.Add(authorizeData);
                }
            }

            return authorizeDatas?.ToArray();
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeData = ComputeAuthorizeData(context.MethodInfo);
            if (authorizeData is null)
                return;

            var policiesMessage = "Required policies: ";
            var rolesMessage = "Required roles: ";

            bool hasRoles = false, hasPolicies = false;

            foreach (var data in authorizeData)
            {
                if (data.Roles != null)
                {
                    if (hasRoles)
                    {
                        rolesMessage += " and ";
                    }

                    var roles = data.Roles.Split(',');
                    for (var i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        hasRoles = true;
                        rolesMessage += role.Trim();

                        if (i != roles.Length - 1)
                        {
                            rolesMessage += " or ";
                        }
                    }
                }

                if (data.Policy != null)
                {
                    if (hasPolicies)
                    {
                        policiesMessage += " and ";
                    }

                    policiesMessage += data.Policy;
                    hasPolicies = true;
                }
            }

            if (!hasRoles)
            {
                rolesMessage = "";
            }
            else
            {
                rolesMessage += ". ";
            }

            if (!hasPolicies)
            {
                policiesMessage = "";
            }
            else
            {
                policiesMessage += ".";
            }

            if (hasRoles || hasPolicies)
            {
                if (!operation.Responses.ContainsKey("401"))
                {
                    operation.Responses.Add("401", new OpenApiResponse()
                    {
                        Description = $"The user must be authenticated. {rolesMessage}{policiesMessage}"
                    });
                }

                if (!operation.Responses.ContainsKey("403"))
                {
                    operation.Responses.Add("403", new OpenApiResponse()
                    {
                        Description = $"This user cannot perform this operation. {rolesMessage}{policiesMessage}"
                    });
                }
            }
        }
    }
}
