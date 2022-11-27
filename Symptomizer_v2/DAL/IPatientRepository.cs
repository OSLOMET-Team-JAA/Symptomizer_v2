using System.Collections.Generic;
using System.Threading.Tasks;
using Symptomizer_v2.Models;

namespace Symptomizer_v2.DAL
{
    public interface IPatientRepository
    {
        Task<bool> AddPatient(Patient p);
        Task<List<Patient>> FindAll();
        Task<bool> DeletePatient(int id);
        Task<Patient> FindPatient(int id);
        Task<bool> EditPatient(Patient eP);
        Task<bool> LoggIn(User user);
    }
}