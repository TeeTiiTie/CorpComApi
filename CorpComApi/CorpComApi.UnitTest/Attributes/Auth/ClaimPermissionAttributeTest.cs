using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using CorpComApi.Attributes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CorpComApi.UnitTest.Attributes.Auth
{
    [TestFixture]
    public class ClaimPermissionAttributeTest
    {
        private HttpContext _httpContext;
        private const string _jwtToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImUwMjVjMWJlLTMzMTItNGQ4My1hNDFmLTk1NzM0MThiZTI3ZSIsInR5cCI6ImF0K2p3dCJ9.eyJuYmYiOjE2ODQ2ODU5MTQsImV4cCI6MTY4NDY4OTUxNCwiaXNzIjoiaHR0cHM6Ly9vYXV0aGxvZ2luLnNpYW1zbWlsZS5jby50aCIsImF1ZCI6WyJwcmVtaXVtX29yZGVyX2FwaSIsInByZW1pdW1fYXBpIiwic3NzZG9jX2FwaSIsIm9jcl9hcGkiLCJzc2JfY2hlcXVlX2FwaSIsInNzYl9kaXJlY3R0cmFuc2Zlcl9hcGkiLCJzc3NwYV9hcGkiXSwiY2xpZW50X2lkIjoiMGQyYzdlZjEtZmQxYS00YWRiLThmN2EtZmExNTkwOTg1NDcwIiwic3ViIjoiYTJjMjhmY2MtNzhmNC00ZGFjLWJhYzgtNGQwZTkyZTkzZTM3IiwiYXV0aF90aW1lIjoxNjgxMDkyODQ4LCJpZHAiOiJsb2NhbCIsInJvbGUiOlsiRGV2ZWxvcGVyIiwiU21pbGVDbGFpbVBheUJhY2tfT3BlcmF0aW9uIiwiUHJlbWl1bV9PcmRlcl9BZG1pbiIsInByZW1pdW1fc3RhdGVtZW50X2FwaV9hZG1pbiIsIlByb3NwZWN0X0FkbWluIiwiUGV0dHlDYXNoX0FkbWluIiwiUHJlbWl1bV9BZG1pbiJdLCJwZXJtaXNzaW9uIjpbInByZW1pdW1fb3JkZXJfYXBpOmludm9pY2VfbW9uaXRvciIsInByZW1pdW1fYXBpOmJpbGxfcGF5bWVudF9mdWxsIiwicHJlbWl1bV9hcGk6Y29sbGVjdGlvbl9mdWxsIiwicHJlbWl1bV9hcGk6cmVjb3ZlcnlfZnVsbCIsInByZW1pdW1fYXBpOnJlZnVuZF9mdWxsIiwicHJlbWl1bV9hcGk6c21zX2Z1bGwiLCJwcmVtaXVtX2FwaTpkZWJ0X2Z1bGwiLCJwcmVtaXVtX2FwaTpyZXBvcnRfZnVsbCIsInByZW1pdW1fYXBpOmRpcmVjdF9kZWJpdF9hZG1pbiIsInNzYl9jaGVxdWVfYXBpOmNoZXF1ZV91c2VyIiwic3NiX2NoZXF1ZV9hcGk6Y2hlcXVlX2Z1bGwiLCJzc2JfZGlyZWN0dHJhbnNmZXJfYXBpOmRpcmVjdHRyYW5zZmVyX3VzZXIiLCJzc2JfZGlyZWN0dHJhbnNmZXJfYXBpOmRpcmVjdHRyYW5zZmVyX2Z1bGwiLCJwcmVtaXVtX29yZGVyX2FwaTplbmRvcnNlX2JpbGxfYWRtaW4iLCJwcmVtaXVtX29yZGVyX2FwaTplbmRvcnNlX2JpbGxfZnVsbCIsInByZW1pdW1fb3JkZXJfYXBpOnJlcG9ydHBhX2Z1bGwiXSwidXNlcl9pZCI6ODk3NCwiZW1wbG95ZWVfY29kZSI6IjA2MjM5IiwiZW1wbG95ZWVfZmlyc3RuYW1lIjoi4Lin4Lij4Lij4LiT4LiY4Lix4LiKIiwiZW1wbG95ZWVfbGFzdG5hbWUiOiLguIrguLHguYnguJnguYHguIjguYjguKEiLCJlbXBsb3llZV9icmFuY2hpZCI6NzAsImVtcGxveWVlX2JyYW5jaG5hbWUiOiLguKrguLPguJnguLHguIHguIfguLLguJnguYPguKvguI3guYgiLCJqdGkiOiI1NTA4NTFFNDMxOUE5N0U4OEVEQjI5NzkwNjBGRjgxNSIsInNpZCI6IkU3MEQxNDZGMzQyQ0RDM0JCOUI2MTU2Q0E3MjRDQjNCIiwiaWF0IjoxNjg0Njg1OTE0LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwicm9sZXMiLCJwcmVtaXVtX29yZGVyX2FwaSIsInByZW1pdW1fYXBpIiwic3NzcGFfYXBpIiwic3NzZG9jX2FwaSIsIm9jcl9hcGkiLCJzc2JfY2hlcXVlX2FwaSIsInNzYl9kaXJlY3R0cmFuc2Zlcl9hcGkiLCJlbWFpbCJdLCJhbXIiOlsicHdkIl19.ZL5UTSsWiP2c-h8Q2XjLBr6e1a4oaQxYrmjzip-WzAKiA3_Cej5jxhsfyz_lpy0rVX-12D0VFOXtJzpP5_ELbdDWmiVgUbyF8wajpaoGSk9q7Bf9Jqh6dnjliuYoTEwC-hS835SOF1xB1vQRCo9GkGj4xTmmQpdNwjPcCW1nHmNa10sKeK30bHqALrhGoUFBK9ew1dFb6AO5Kmoi1jETaaxAq-YbOR_a0fn3Up2XH6VEOmo00RwkF2Ez5WT-pPN0whrGAeJ0-1eDCpBhwQJJjn_cYOsmId0BGN3VVD_GMkuaw6Yxu04PZHzwRVKm8wrvvrcVCsRnYeV46qWm0Bp9jw";

        /*
         * "permission": [
         *     "premium_order_api:invoice_monitor",
         *     "premium_api:bill_payment_full",
         *     "premium_api:collection_full",
         *     "premium_api:recovery_full",
         *     "premium_api:refund_full",
         *     "premium_api:sms_full",
         *     "premium_api:debt_full",
         *     "premium_api:report_full",
         *     "premium_api:direct_debit_admin",
         *     "ssb_cheque_api:cheque_user",
         *     "ssb_cheque_api:cheque_full",
         *     "ssb_directtransfer_api:directtransfer_user",
         *     "ssb_directtransfer_api:directtransfer_full",
         *     "premium_order_api:endorse_bill_admin",
         *     "premium_order_api:endorse_bill_full",
         *     "premium_order_api:reportpa_full"
         * ],
         */

        [SetUp]
        public void SetUp()
        {
            var jwtData = new JwtSecurityTokenHandler().ReadJwtToken(_jwtToken);

            // Mock HttpContext

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.Request.Headers["Authorization"]).Returns("Bearer " + _jwtToken);
            mockHttpContext.Setup(m => m.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(jwtData.Claims)));
            mockHttpContext.Setup(m => m.User.Identity.Name).Returns(jwtData.Subject);
            mockHttpContext.Setup(m => m.User.Identity.IsAuthenticated).Returns(true);

            _httpContext = mockHttpContext.Object;
        }

        private string CheckResult(IActionResult? actionResult)
        {
            if (actionResult is null) return "Null";
            return actionResult.ToString();
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission without condition,
        /// the user must have some permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithPermission_Success()
        {
            // Arrange

            Console.WriteLine("Attribute Example : [ClaimPermission(\"premium_order_api:invoice_monitor\", \"premium_api:bill_payment_full\")]");

            // [ClaimPermission("premium_order_api:invoice_monitor", "premium_api:bill_payment_full")]
            var authorizeAttribute = new ClaimPermissionAttribute("premium_order_api:invoice_monitor", "premium_api:bill_payment_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());

            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.Null);
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission without condition,
        /// the user must doesn't have permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithoutPermission_Forbid()
        {
            // Arrange

            Console.WriteLine("Attribute Example : [ClaimPermission(\"premium_order_api:invoice_full\")]");

            // [ClaimPermission("premium_order_api:invoice_full")]
            var authorizeAttribute = new ClaimPermissionAttribute("premium_order_api:invoice_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());
            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.TypeOf<ForbidResult>());
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission with condition Any,
        /// the user must have some permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithSomePermission_AnyPermission_Success()
        {
            // Arrange

            Console.WriteLine("Attribute Example : [ClaimPermission(ClaimPermissionCondition.Any, \"premium_order_api:invoice_monitor\", \"premium_order_api:invoice_full\")]");

            // [ClaimPermission(ClaimPermissionCondition.Any, "premium_order_api:invoice_monitor", "premium_order_api:invoice_full")]
            var authorizeAttribute = new ClaimPermissionAttribute(ClaimPermissionCondition.Any, "premium_order_api:invoice_monitor", "premium_order_api:invoice_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());
            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.Null);
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission with condition Any,
        /// the user must doesn't have permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithoutPermission_AnyPermission_Forbid()
        {
            // Arrange

            Console.WriteLine("Attribute Example : [ClaimPermission(ClaimPermissionCondition.Any, \"premium_order_api:invoice_admin\", \"premium_order_api:invoice_full\")]");

            // [ClaimPermission(ClaimPermissionCondition.Any, "premium_order_api:invoice_admin", "premium_order_api:invoice_full")]
            var authorizeAttribute = new ClaimPermissionAttribute(ClaimPermissionCondition.Any, "premium_order_api:invoice_admin", "premium_order_api:invoice_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());
            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.TypeOf<ForbidResult>());
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission with condition All,
        /// the user must have all permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithAllPermission_AllPermission_Success()
        {
            // Arrange

            Console.WriteLine("Attribute Example : [ClaimPermission(ClaimPermissionCondition.All, \"premium_order_api:invoice_monitor\", \"premium_api:report_full\")]");

            // [ClaimPermission(ClaimPermissionCondition.All, "premium_order_api:invoice_monitor", "premium_api:report_full")]
            var authorizeAttribute = new ClaimPermissionAttribute(ClaimPermissionCondition.All, "premium_order_api:invoice_monitor", "premium_api:report_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());
            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.Null);
        }

        /// <summary>
        /// ClaimPermissionAttribute test for permission with condition All,
        /// the user must doesn't have all permissions to pass the authorization.
        /// </summary>
        [Test]
        public void UserWithSomePermission_AllPermission_Forbid()
        {
            // Arrange
            // [ClaimPermission(ClaimPermissionCondition.All, "premium_order_api:invoice_monitor", "premium_api:report_full")]
            var authorizeAttribute = new ClaimPermissionAttribute(ClaimPermissionCondition.All, "premium_order_api:invoice_monitor", "premium_api:invoice_full");

            var authorizationFilterContext = new AuthorizationFilterContext(
                                             new ActionContext(_httpContext, new RouteData(), new ActionDescriptor()),
                                             new List<IFilterMetadata>());

            // Act
            authorizeAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Console.WriteLine($"Result : {CheckResult(authorizationFilterContext.Result)}");
            Assert.That(authorizationFilterContext.Result, Is.TypeOf<ForbidResult>());
        }
    }
}