using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Symptomizer_v2.DAL;
using Symptomizer_v2.Models;

namespace Symptomizer_v2.DAL
{
    public class PatientRepository : IPatientRepository //Implementing all methods from interface
    {
        private readonly PatientContext _db;
        private ILogger<PatientRepository> _log;
        public PatientRepository(PatientContext db, ILogger<PatientRepository> log)
        {
            _db = db;
            _log = log;
        }
        public async Task<bool> AddPatient(Patient p)
        {
            try
            {
                var newPatient = new Patients
                {
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                };
                var findSymptoms = await _db.Diseases.FindAsync(p.Symptoms);
                if (findSymptoms == null)
                {
                    var newDisease = new Diseases
                    {
                        Symptoms = p.Symptoms,
                        DiseaseName = p.Disease
                    };
                    newPatient.Disease = newDisease;
                }
                else
                {
                    newPatient.Disease = findSymptoms;
                }
                _db.Patients.Add(newPatient);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }
        public async Task<List<Patient>> FindAll()
        {
            try
            {
                List<Patient> patients = await _db.Patients.Select(p => new Patient
                {
                    Id = p.Id,
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    Symptoms = p.Disease.Symptoms,
                    Disease = p.Disease.DiseaseName,
                }).ToListAsync();
                return patients;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return null;
            }
        }
        public async Task<bool> DeletePatient(int id)
        {
            try
            {
                Patients patient = await _db.Patients.FindAsync(id);
                _db.Patients.Remove(patient);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }
        public async Task<Patient> FindPatient(int id)
        {
            try
            {
                Patients patient = await _db.Patients.FindAsync(id);
                var foundPatient = new Patient()
                {
                    Id = patient.Id,
                    Firstname = patient.Firstname,
                    Lastname = patient.Lastname,
                    Symptoms = patient.Disease.Symptoms,
                    Disease = patient.Disease.DiseaseName
                };
                return foundPatient;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return null;
            }
        }

        public async Task<bool> EditPatient(Patient eP)
        {
            try
            {
                var editPatient = await _db.Patients.FindAsync(eP.Id);
                if (editPatient.Disease.Symptoms != eP.Symptoms)
                {
                    var findSimptoms = await _db.Diseases.FindAsync(eP.Symptoms);
                    if (findSimptoms == null)
                    {
                        var newDisease = new Diseases()
                        {
                            Symptoms = eP.Symptoms,
                            DiseaseName = eP.Disease
                        };
                        editPatient.Disease = newDisease;
                    }
                    else
                    {
                        editPatient.Disease.Symptoms = eP.Symptoms;
                    }
                }
                editPatient.Firstname = eP.Firstname;
                editPatient.Lastname = eP.Lastname;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
            //return true;
        }

        public static byte[] CreateHash(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1000,
                numBytesRequested: 32);
        }
        
        public static byte[] CreateSalt()
        {
            var csp = new RNGCryptoServiceProvider();
            var salt = new byte[24];
            csp.GetBytes(salt);
            return salt;
        }
        
        public async Task<bool> LoggIn(User user)
        {
            try
            {
                Users foundUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                byte[] hash = CreateHash(user.Password, foundUser.Salt);
                bool ok = hash.SequenceEqual(foundUser.Password);
                if (ok)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }
    }
}