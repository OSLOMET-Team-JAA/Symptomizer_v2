using Moq;
using System;
using System.Net.Http;
using Xunit;
using Symptomizer_v2.Controllers;
using Symptomizer_v2.DAL;
using Symptomizer_v2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Drawing.Printing;

namespace SymptomizerTestProject
{
    public class UnitTest1
    {
        private const string LoggedIn = "loggedIn";
        private const string NotLoggedIn = "";

        private readonly Mock<IPatientRepository> _mockRep = new Mock<IPatientRepository>();
        private readonly Mock<ILogger<PatientController>> _mockLog = new Mock<ILogger<PatientController>>();

        private readonly Mock<HttpContext> _mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession _mockSession = new MockHttpSession();

        [Fact]
        public async Task FindAllLoggedInOK()
        {
            // Arrange
            var patient1 = new Patient { Id = 1, Firstname = "Per", Lastname = "Hansen", Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Muscle or body aches", Disease = "Flu" };
            var patient2 = new Patient { Id = 2, Firstname = "Ole", Lastname = "Jensen", Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Shortness of breath or difficulty breathing,Muscle or body aches", Disease = "COVID-19" };
            var patient3 = new Patient { Id = 3, Firstname = "Tobi", Lastname = "Larsen", Symptoms = "Fever or chills,Cough", Disease = "Common cold" };

            var patientList = new List<Patient>();
            patientList.Add(patient1);
            patientList.Add(patient2);
            patientList.Add(patient3);

            _mockRep.Setup(p => p.FindAll()).ReturnsAsync(patientList);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.FindAll() as OkObjectResult;

            // Assert 
            // Dereference in result of a possibly null reference, so let's check that result is not a null
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<List<Patient>>((List<Patient>)result.Value, patientList);
        }

        [Fact]
        public async Task FindAllNotLoggedIn()
        {
            // Arrange

            _mockRep.Setup(p => p.FindAll()).ReturnsAsync(It.IsAny<List<Patient>>());

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.FindAll() as UnauthorizedObjectResult;

            // Assert
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Patients not found", result.Value);
        }

        [Fact]
        public async Task AddPatientLoggedInOk()
        {

            // Arrange
            _mockRep.Setup(p => p.AddPatient(It.IsAny<Patient>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.AddPatient(It.IsAny<Patient>()) as OkObjectResult;

            // Assert 
            // Checking that result is not a null
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.True(((int)HttpStatusCode.OK).Equals(result.StatusCode));
            Assert.True(("Patient was created").Equals(result.Value));
        }

        [Fact]
        public async Task AddPatientLoggedInNotOk()
        {
            // Arrange
            _mockRep.Setup(p => p.AddPatient(It.IsAny<Patient>())).ReturnsAsync(false);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.AddPatient(It.IsAny<Patient>()) as BadRequestObjectResult;

            // Assert 
            // Checking that result is not a null
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Patient was not created", result.Value);
        }
        [Fact]
        public async Task AddPatientLoggedInFailModel()
        {
            // Arrange
            var patient1 = new Patient { Id = 1, Firstname = "Per", Lastname = "Hansen", Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Muscle or body aches", Disease = "Flu" };

            _mockRep.Setup(p => p.AddPatient(patient1)).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            patientController.ModelState.AddModelError("Firstname", "Inputs' validation is failed");

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.AddPatient(patient1) as BadRequestObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Inputs' validation is failed", result.Value);
        }

        [Fact]
        public async Task NotAddPatientLoggedIn()
        {
            _mockRep.Setup(p => p.AddPatient(It.IsAny<Patient>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.AddPatient(It.IsAny<Patient>()) as UnauthorizedObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Patient was not created", result.Value);
        }

        [Fact]
        public async Task DeletePatientLoggedInOk()
        {
            // Arrange

            _mockRep.Setup(p => p.DeletePatient(It.IsAny<int>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.DeletePatient(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Patient was deleted", result.Value);
        }

        [Fact]
        public async Task DeletePatientLoggedInNotOk()
        {
            // Arrange

            _mockRep.Setup(p => p.DeletePatient(It.IsAny<int>())).ReturnsAsync(false);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.DeletePatient(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("Patient was not deleted", result.Value);
        }

        [Fact]
        public async Task DeletePatientNotLoggedIn()
        {
            _mockRep.Setup(k => k.DeletePatient(It.IsAny<int>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.DeletePatient(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Cannot delete! Login first!", result.Value);
        }

        [Fact]
        public async Task FindPatientLoggedInOk()
        {
            // Arrange
            var patient1 = new Patient { Id = 1, Firstname = "Per", Lastname = "Hansen", Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Muscle or body aches", Disease = "Flu" };

            _mockRep.Setup(p => p.FindPatient(It.IsAny<int>())).ReturnsAsync(patient1);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.FindPatient(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<Patient>(patient1, (Patient)result.Value);
        }

        [Fact]
        public async Task NotFindPatientLoggedInOk()
        {
            // Arrange

            _mockRep.Setup(p => p.FindPatient(It.IsAny<int>())).ReturnsAsync(() => null);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.FindPatient(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("Patient was not found", result.Value);
        }

        [Fact]
        public async Task NotFindPatientNotLoggedIn()
        {
            _mockRep.Setup(p => p.FindPatient(It.IsAny<int>())).ReturnsAsync(() => null);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.FindPatient(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Patient not found! You must login first!", result.Value);
        }

        [Fact]
        public async Task EditPatientLoggedInOk()
        {
            // Arrange
            _mockRep.Setup(p => p.EditPatient(It.IsAny<Patient>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.EditPatient(It.IsAny<Patient>()) as OkObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Patient's info was changed", result.Value);
        }

        [Fact]
        public async Task EditPatientLoggedInNotOK()
        {
            // Arrange
            _mockRep.Setup(p => p.EditPatient(It.IsAny<Patient>())).ReturnsAsync(false);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.EditPatient(It.IsAny<Patient>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("Patient not found", result.Value);
        }

        [Fact]
        public async Task EditPatientLoggedInFailModel()
        {
            // Arrange
            // Patient is indicated fail with empty firstname here.
            // it doesn't matter, it's the introduction with ModelError below that forces the error
            // It could use It here also It.IsAny<Patient>
            var patient1 = new Patient { Id = 1, Firstname = "Per", Lastname = "Hansen", Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Muscle or body aches", Disease = "Flu" };
            _mockRep.Setup(p => p.EditPatient(patient1)).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            patientController.ModelState.AddModelError("Firstname", "Input's validation failed");

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.EditPatient(patient1) as BadRequestObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Inputs' validation failed", result.Value);
        }

        [Fact]
        public async Task EditPatientNotLoggedIn()
        {
            _mockRep.Setup(p => p.EditPatient(It.IsAny<Patient>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.EditPatient(It.IsAny<Patient>()) as UnauthorizedObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Patient's info was not changed! You must login first!", result.Value);
        }

        [Fact]
        public async Task LogInOk()
        {
            _mockRep.Setup(u => u.LoggIn(It.IsAny<User>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.LoggIn(It.IsAny<User>()) as OkObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True((bool)result.Value);
        }

        [Fact]
        public async Task LoggInFailPasswordOrUser()
        {
            _mockRep.Setup(u => u.LoggIn(It.IsAny<User>())).ReturnsAsync(false);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockSession[LoggedIn] = NotLoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.LoggIn(It.IsAny<User>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.False((bool)result.Value);
        }

        [Fact]
        public async Task LoggInInputFail()
        {
            _mockRep.Setup(u => u.LoggIn(It.IsAny<User>())).ReturnsAsync(true);

            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            patientController.ModelState.AddModelError("Username", "Fail in input validation");

            _mockSession[LoggedIn] = LoggedIn;
            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            var result = await patientController.LoggIn(It.IsAny<User>()) as BadRequestObjectResult;

            // Assert 
            Debug.Assert(result != null, nameof(result) + " != null");
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Fail in input validation", result.Value);
        }

        [Fact]
        public void LoggOut()
        {
            var patientController = new PatientController(_mockRep.Object, _mockLog.Object);

            _mockHttpContext.Setup(s => s.Session).Returns(_mockSession);
            _mockSession[LoggedIn] = LoggedIn;
            patientController.ControllerContext.HttpContext = _mockHttpContext.Object;

            // Act
            patientController.LoggOut();

            // Assert
            Assert.Equal(NotLoggedIn, _mockSession[LoggedIn]);
        }
    }
}


// https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)?redirectedfrom=MSDN