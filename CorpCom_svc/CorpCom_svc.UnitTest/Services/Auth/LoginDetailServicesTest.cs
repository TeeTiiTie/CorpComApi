using CorpCom_svc.Services.Auth;

namespace CorpCom_svc.UnitTest.Services.Auth
{
    public class LoginDetailServicesTest
    {
        private const string _jwtToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImUwMjVjMWJlLTMzMTItNGQ4My1hNDFmLTk1NzM0MThiZTI3ZSIsInR5cCI6ImF0K2p3dCJ9.eyJuYmYiOjE2ODQ2NzY4NDEsImV4cCI6MTY4NDY4MDQ0MSwiaXNzIjoiaHR0cHM6Ly9vYXV0aGxvZ2luLnNpYW1zbWlsZS5jby50aCIsImNsaWVudF9pZCI6IjBmMTU3N2VmLTBjYWYtNGIwMy1iZmRiLTY0MTgyZGIyMTJlNiIsInN1YiI6ImEyYzI4ZmNjLTc4ZjQtNGRhYy1iYWM4LTRkMGU5MmU5M2UzNyIsImF1dGhfdGltZSI6MTY4MTA5Mjg0OCwiaWRwIjoibG9jYWwiLCJyb2xlIjpbIkRldmVsb3BlciIsIlNtaWxlQ2xhaW1QYXlCYWNrX09wZXJhdGlvbiIsIlByZW1pdW1fT3JkZXJfQWRtaW4iLCJwcmVtaXVtX3N0YXRlbWVudF9hcGlfYWRtaW4iLCJQcm9zcGVjdF9BZG1pbiIsIlBldHR5Q2FzaF9BZG1pbiIsIlByZW1pdW1fQWRtaW4iXSwidXNlcl9pZCI6ODk3NCwiZW1wbG95ZWVfY29kZSI6IjA2MjM5IiwiZW1wbG95ZWVfZmlyc3RuYW1lIjoi4Lin4Lij4Lij4LiT4LiY4Lix4LiKIiwiZW1wbG95ZWVfbGFzdG5hbWUiOiLguIrguLHguYnguJnguYHguIjguYjguKEiLCJlbXBsb3llZV9icmFuY2hpZCI6NzAsImVtcGxveWVlX2JyYW5jaG5hbWUiOiLguKrguLPguJnguLHguIHguIfguLLguJnguYPguKvguI3guYgiLCJVc2VybmFtZSI6IjA2MjM5IiwiRW1wbG95ZWVfSUQiOjExNzYyLCJFbXBsb3llZUNvZGUiOiIwNjIzOSIsIlBlcnNvbl9JRCI6MTkyNjcsIkZ1bGxOYW1lIjoi4Lin4Lij4Lij4LiT4LiY4Lix4LiKIOC4iuC4seC5ieC4meC5geC4iOC5iOC4oSIsIkZpcnN0TmFtZSI6IuC4p-C4o-C4o-C4k-C4mOC4seC4iiIsIkxhc3ROYW1lIjoi4LiK4Lix4LmJ4LiZ4LmB4LiI4LmI4LihIiwiQnJhbmNoX0lEIjo3MCwiQnJhbmNoRGV0YWlsIjoi4Liq4Liz4LiZ4Lix4LiB4LiH4Liy4LiZ4LmD4Lir4LiN4LmIIiwiQXJlYV9JRCI6OSwiQXJlYURldGFpbCI6IuC4quC4s-C4meC4seC4geC4h-C4suC4meC5g-C4q-C4jeC5iCIsIkRlcGFydG1lbnRfSUQiOjcsIkRlcGFydG1lbnREZXRhaWwiOiLguJ7guLHguJLguJnguLLguKPguLDguJrguJoiLCJFbXBsb3llZVRlYW1fSUQiOjE4MSwiRW1wbG95ZWVUZWFtRGV0YWlsIjoi4Lie4LiZ4Lix4LiB4LiH4Liy4LiZ4LmD4Lir4Lih4LmIIiwiRW1wbG95ZWVQb3NpdGlvbl9JRCI6MzQsIkVtcGxveWVlUG9zaXRpb25EZXRhaWwiOiLguJzguLnguYnguIrguYjguKfguKLguJzguLnguYnguIjguLHguJTguIHguLLguKPguYHguJzguJnguIEiLCJqdGkiOiI5RjlBMzUwRjdCMEJFQ0QyNDlBOTU0OTc2NUMyNjZEQyIsInNpZCI6IkU3MEQxNDZGMzQyQ0RDM0JCOUI2MTU2Q0E3MjRDQjNCIiwiaWF0IjoxNjg0Njc2ODQxLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZW1haWwiLCJyb2xlcyIsImVtcGxveWVlX3Byb2ZpbGUiLCJlbXBsb3llZV9icmFuY2giLCJlbXBsb3llZV9kZXBhcnRtZW50IiwiZW1wbG95ZWVfdGVhbSIsImVtcGxveWVlX3Bvc2l0aW9uIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.vqjllxCvEVf_3a1hjF6r6DVZyDQ2hUd14YK7M0EOJ-N6SCAciV1rE3ClYRtlg1EmBMFpsxv_sba1w20Ss9L46e7Ack2_3HrwgTnefq9t96NE0hdbjnskAAWo8CCz6fppTLJn2-wHsq6Z6MXx3jkbgB1ZEqO4L0Tnwpe_W--0Sqk88HE-Bk5oIisV-715ma5Kto3VyqvFi6H9glwuvWQYD_Gcq8vC-mNUKNUxh8a1hkchuiQZc2HqgKFeBUdMl7v0Ms86YS8naCtbTx2S0VGNJFzlaQyFLYvHggOMbLEVTthywcPn8ajXRCue3npYp-1EYRfx8aOnoiguivX7D8Lyag";

        private string _rawjsonResult = "{\r\n  \"nbf\": 1684676841,\r\n  \"exp\": 1684680441,\r\n  \"iss\": \"https://oauthlogin.siamsmile.co.th\",\r\n  \"client_id\": \"0f1577ef-0caf-4b03-bfdb-64182db212e6\",\r\n  \"sub\": \"a2c28fcc-78f4-4dac-bac8-4d0e92e93e37\",\r\n  \"auth_time\": 1681092848,\r\n  \"idp\": \"local\",\r\n  \"role\": [\r\n    \"Developer\",\r\n    \"SmileClaimPayBack_Operation\",\r\n    \"Premium_Order_Admin\",\r\n    \"premium_statement_api_admin\",\r\n    \"Prospect_Admin\",\r\n    \"PettyCash_Admin\",\r\n    \"Premium_Admin\"\r\n  ],\r\n  \"user_id\": 8974,\r\n  \"employee_code\": \"06239\",\r\n  \"employee_firstname\": \"วรรณธัช\",\r\n  \"employee_lastname\": \"ชั้นแจ่ม\",\r\n  \"employee_branchid\": 70,\r\n  \"employee_branchname\": \"สำนักงานใหญ่\",\r\n  \"Username\": \"06239\",\r\n  \"Employee_ID\": 11762,\r\n  \"EmployeeCode\": \"06239\",\r\n  \"Person_ID\": 19267,\r\n  \"FullName\": \"วรรณธัช ชั้นแจ่ม\",\r\n  \"FirstName\": \"วรรณธัช\",\r\n  \"LastName\": \"ชั้นแจ่ม\",\r\n  \"Branch_ID\": 70,\r\n  \"BranchDetail\": \"สำนักงานใหญ่\",\r\n  \"Area_ID\": 9,\r\n  \"AreaDetail\": \"สำนักงานใหญ่\",\r\n  \"Department_ID\": 7,\r\n  \"DepartmentDetail\": \"พัฒนาระบบ\",\r\n  \"EmployeeTeam_ID\": 181,\r\n  \"EmployeeTeamDetail\": \"พนักงานใหม่\",\r\n  \"EmployeePosition_ID\": 34,\r\n  \"EmployeePositionDetail\": \"ผู้ช่วยผู้จัดการแผนก\",\r\n  \"jti\": \"9F9A350F7B0BECD249A9549765C266DC\",\r\n  \"sid\": \"E70D146F342CDC3BB9B6156CA724CB3B\",\r\n  \"iat\": 1684676841,\r\n  \"scope\": [\r\n    \"openid\",\r\n    \"profile\",\r\n    \"email\",\r\n    \"roles\",\r\n    \"employee_profile\",\r\n    \"employee_branch\",\r\n    \"employee_department\",\r\n    \"employee_team\",\r\n    \"employee_position\",\r\n    \"offline_access\"\r\n  ],\r\n  \"amr\": [\r\n    \"pwd\"\r\n  ]\r\n}";

        public LoginDetailServicesTest()
        {
        }

        [Test]
        public void GetClaimTest()
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);

            // Act
            var user = loginDetailServices.GetClaim();

            // Assert
            Assert.That(user, Is.Not.Null);

            Assert.That(user.EmployeeCode, Is.EqualTo("06239"));
            Assert.That(user.Firstname, Is.EqualTo("วรรณธัช"));
            Assert.That(user.Lastname, Is.EqualTo("ชั้นแจ่ม"));
            Assert.That(user.BranchId, Is.EqualTo(70));
            Assert.That(user.Branchname, Is.EqualTo("สำนักงานใหญ่"));
            Assert.That(user.UserId, Is.EqualTo(8974));
        }

        [Test]
        public void TokenTest()
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);

            // Act
            var token = loginDetailServices.Token;

            // Assert
            Assert.That(token, Is.EqualTo(_jwtToken));
        }

        [TestCase("Developer")]
        [TestCase("SmileClaimPayBack_Operation")]
        [TestCase("Premium_Order_Admin")]
        [TestCase("premium_statement_api_admin")]
        [TestCase("Prospect_Admin")]
        [TestCase("PettyCash_Admin")]
        [TestCase("Premium_Admin")]
        public void RoleTest(string expectedRole)
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);

            // Act
            var role = loginDetailServices.Roles;

            // Assert
            Assert.That(expectedRole, Is.AnyOf(role));
        }

        [Test]
        public void PermissionTest()
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);
            // Act
            var permission = loginDetailServices.Permissions;

            // Assert
            Assert.That(permission, Is.Empty);
        }

        [Test]
        public void IsLoginTest()
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);

            // Act
            var isLogin = loginDetailServices.IsLogin;

            // Assert
            Assert.That(isLogin, Is.False);
        }

        [TestCase(LoginDetailClaim.Subject, "a2c28fcc-78f4-4dac-bac8-4d0e92e93e37")]
        [TestCase(LoginDetailClaim.EmployeeCode, "06239")]
        [TestCase(LoginDetailClaim.FirstName, "วรรณธัช")]
        [TestCase(LoginDetailClaim.LastName, "ชั้นแจ่ม")]
        [TestCase(LoginDetailClaim.BranchDetail, "สำนักงานใหญ่")]
        [TestCase(LoginDetailClaim.Username, "06239")]
        [TestCase(LoginDetailClaim.EmployeeCode, "06239")]
        [TestCase(LoginDetailClaim.FullName, "วรรณธัช ชั้นแจ่ม")]
        [TestCase(LoginDetailClaim.BranchDetail, "สำนักงานใหญ่")]
        [TestCase(LoginDetailClaim.AreaDetail, "สำนักงานใหญ่")]
        [TestCase(LoginDetailClaim.DepartmentDetail, "พัฒนาระบบ")]
        [TestCase(LoginDetailClaim.EmployeeTeamDetail, "พนักงานใหม่")]
        [TestCase(LoginDetailClaim.EmployeePositionDetail, "ผู้ช่วยผู้จัดการแผนก")]
        public void LoginDetailClaim_StringTest(string claimType, string expect)
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);

            // Act
            var claim = loginDetailServices.GetClaim<string>(claimType);

            // Assert
            Assert.That(claim, Is.EqualTo(expect));
        }

        [TestCase(LoginDetailClaim.UserId, 8974)]
        [TestCase(LoginDetailClaim.EmployeeId, 11762)]
        [TestCase(LoginDetailClaim.PersonId, 19267)]
        [TestCase(LoginDetailClaim.BranchId, 70)]
        [TestCase(LoginDetailClaim.AreaId, 9)]
        [TestCase(LoginDetailClaim.DepartmentId, 7)]
        [TestCase(LoginDetailClaim.EmployeeTeamId, 181)]
        [TestCase(LoginDetailClaim.EmployeePositionId, 34)]
        public void LoginDetailClaim_IntTest(string claimType, int expect)
        {
            // Arrange
            var loginDetailServices = new LoginDetailServices(_jwtToken);
            // Act
            var claim = loginDetailServices.GetClaim<int>(claimType);
            // Assert
            Assert.That(claim, Is.EqualTo(expect));
        }
    }
}