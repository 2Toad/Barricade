/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Barricade
{
    /// <summary>
    /// Adds claim based security to the decorated class or method.
    /// When a claim is not specified, via the constructor, authentication 
    /// is still required but authorization is skipped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ClaimAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// The claim required for access to the protected resource.
        /// </summary>
        public IClaim Claim { get; private set; }

        /// <summary>
        /// Creates an instance of the claim based filter.
        /// </summary>
        public ClaimAttribute() { }

        /// <summary>
        /// Creates an instance of the claim based filter.
        /// </summary>
        /// <param name="type">The claim type.</param>
        /// <param name="value">The value of the claim <paramref name="type"/>.</param>
        public ClaimAttribute(string type, string value)
        {
            Claim = new Claim {Type = type, Value = value};
        }

        /// <summary>
        /// Called when a process requests authorization.
        /// Authorization is denied if the user is not authenticated, or if the user does not have 
        /// the necessary <see cref="Claim"/> (if defined).
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext)) return;

            var controller = (IClaimController)actionContext.ControllerContext.Controller;
            var status = SecurityContext.IsAuthorized(actionContext.Request.Headers.Authorization, Claim, controller.GetUserByAccessToken);
            if (status != HttpStatusCode.OK) actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(status, "Unauthorized");
        }

        /// <summary>
        /// Skips authorization if the <see cref="AllowAnonymousAttribute"/> is being used.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <returns><c>true</c>if the attribute is present; otherwise <c>false</c>.</returns>
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {            
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() 
                || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}