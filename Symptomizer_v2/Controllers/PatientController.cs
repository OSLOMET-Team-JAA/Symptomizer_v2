using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Symptomizer_v2.DAL;
using Symptomizer_v2.Models;

namespace Symptomizer_v2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private IPatientRepository _db;
        private ILogger<PatientController> _log;
        private const string _loggedIn = "loggedIn";
        private const string _NotLoggedIn = "";

        public PatientController(IPatientRepository db, ILogger<PatientController> log)
        {
            _db = db;
            _log = log;
        }
        [HttpPost]
        public async Task<ActionResult> AddPatient(Patient p)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                return Unauthorized("Patient was not created");
            }
            if (ModelState.IsValid)
            {
                bool returnOk = await _db.AddPatient(p);
                if (!returnOk)
                {
                    _log.LogInformation("Patient was not created");
                    return BadRequest("Patient was not created");
                }

                return Ok("Patient was created");
            }
            _log.LogInformation("Inputs' validation is failed");
            return BadRequest("Inputs' validation is failed");
        }

        [HttpGet]
        public async Task<ActionResult> FindAll()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                return Unauthorized("Patients not found");
            }
            List<Patient> allPatients = await _db.FindAll();
            //We can also use "var allPatients" instead of List<> (List<> usage not required)
            return Ok(allPatients);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                return Unauthorized("Cannot delete! Login first!");
            }
            bool returnOk = await _db.DeletePatient(id);
            if (!returnOk)
            {
                _log.LogInformation("Patient was not deleted");
                return NotFound("Patient was not deleted");
            }
            return Ok("Patient was deleted");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindPatient(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                return Unauthorized("Patient not found! You must login first!");
            }
            Patient foundPatient = await _db.FindPatient(id);
            if (foundPatient == null)
            {
                _log.LogInformation("Patient was not found");
                return NotFound("Patient was not found");
            }
            return Ok(foundPatient);
        }

        [HttpPut]
        public async Task<ActionResult> EditPatient(Patient eP)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                return Unauthorized("Patient's info was not changed! You must login first!");
            }
            if (ModelState.IsValid)
            {
                bool returnOk = await _db.EditPatient(eP);
                if (!returnOk)
                {
                    _log.LogInformation("Patient not found");
                    return NotFound("Patient not found");
                }
                return Ok("Patient's info was changed");
            }
            _log.LogInformation("Input's validation failed");
            return BadRequest("Inputs' validation failed");
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoggIn(User user)
        {
            if (ModelState.IsValid)
            {
                bool returnOk = await _db.LoggIn(user);
                if (!returnOk)
                {
                    _log.LogInformation("Failed to logg in with user");
                    HttpContext.Session.SetString(_loggedIn, _NotLoggedIn);
                    return Ok(false);
                }
                HttpContext.Session.SetString(_loggedIn, _loggedIn);
                return Ok(true);
            }
            _log.LogInformation("Fail in input validation");
            return BadRequest("Fail in input validation");
        }
        
        [HttpPost("logout")]
        public void LoggOut()
        {
            HttpContext.Session.SetString(_loggedIn, _NotLoggedIn);
        }
    }
}

//----Some referrences --------------------------------------//
// https://oslomet.instructure.com/courses/24253/pages/sessions?module_item_id=452360