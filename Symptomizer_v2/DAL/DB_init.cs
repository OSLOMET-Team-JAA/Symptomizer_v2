using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Symptomizer_v2.Models;

namespace Symptomizer_v2.DAL
{
    public class DB_init
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PatientContext>();

                // -- We will not keep our database, so we need each time delete it after seeding
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //--- Making data for our database initialization -----------//
                var disease1 = new Diseases { Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Muscle or body aches", DiseaseName = "Flu" };
                var disease2 = new Diseases { Symptoms = "Fever or chills,Cough,Sore throat,High temperature,Shortness of breath or difficulty breathing,Muscle or body aches", DiseaseName = "COVID-19" };

                var patient1 = new Patients { Firstname = "Ole", Lastname = "Hansen", Disease = disease1 };
                var patient2 = new Patients { Firstname = "Per", Lastname = "Jensen", Disease = disease2 };

                //--Add pre-created Patients to context (Database) ----------//
                context.Patients.Add(patient1);
                context.Patients.Add(patient2);

                //--Creating predefined user -------------------------------//
                var user = new Users
                {
                    Username = "Admin"
                };
                var password = "Admin123";
                byte[] salt = PatientRepository.CreateSalt(); //can be used var instead
                byte[] hash = PatientRepository.CreateHash(password, salt); //can be used var instead
                user.Password = hash;
                user.Salt = salt;
                context.Users.Add(user);


                context.SaveChanges();

                //----Some referrences --------------------------------------//
                //https://www.cdc.gov/flu/symptoms/symptoms.htm
                //https://www.cdc.gov/coronavirus/2019-ncov/symptoms-testing/symptoms.html
                //https://oslomet.instructure.com/courses/24253/pages/kunde-ordre-eksempel?module_item_id=452353



            }
        }
    }
}