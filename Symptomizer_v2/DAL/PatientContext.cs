﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Symptomizer_v2.DAL
{
    public class Patients
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public virtual Diseases Disease { get; set; }
    }

    public class Diseases
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Symptoms { get; set; }
        public string DiseaseName { get; set; }
        public virtual List<Patients> Patients { get; set; }
    }

    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
    }

    public class PatientContext : DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Patients> Patients { get; set; }
        public DbSet<Diseases> Diseases { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}

//----Some referrences --------------------------------------//
// https://oslomet.instructure.com/courses/24253/pages/sessions?module_item_id=452360