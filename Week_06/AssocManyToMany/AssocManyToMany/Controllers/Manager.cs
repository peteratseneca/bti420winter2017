using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using AssocManyToMany.Models;

namespace AssocManyToMany.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private DataContext ds = new DataContext();

        // AutoMapper components
        MapperConfiguration config;
        public IMapper mapper;

        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                // Attention 01 - Object mapper definitions

                cfg.CreateMap<Models.Employee, Controllers.EmployeeBase>();
                cfg.CreateMap<Models.Employee, Controllers.EmployeeWithJobDuties>();

                // Attention 02 - For the employee edit form
                cfg.CreateMap<Controllers.EmployeeBase, Controllers.EmployeeEditJobDutiesForm>();

                cfg.CreateMap<Models.JobDuty, Controllers.JobDutyBase>();
                cfg.CreateMap<Models.JobDuty, Controllers.JobDutyWithEmployees>();

                // Attention 03 - For the job duty edit form
                cfg.CreateMap<Controllers.JobDutyBase, Controllers.JobDutyEditEmployeesForm>();
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }

        // Add methods below
        // Controllers will call these methods
        // Ensure that the methods accept and deliver ONLY view model objects and collections
        // The collection return type is almost always IEnumerable<T>

        // Suggested naming convention: Entity + task/action
        // For example:
        // ProductGetAll()
        // ProductGetById()
        // ProductAdd()
        // ProductEdit()
        // ProductDelete()

        // ############################################################
        // Employee

        public IEnumerable<EmployeeBase> EmployeeGetAll()
        {
            return mapper.Map<IEnumerable<EmployeeBase>>(ds.Employees.OrderBy(e => e.Name));
        }

        public IEnumerable<EmployeeWithJobDuties> EmployeeGetAllWithJobDuties()
        {
            return mapper.Map<IEnumerable<EmployeeWithJobDuties>>
                (ds.Employees.Include("JobDuties").OrderBy(e => e.Name));
        }

        public EmployeeWithJobDuties EmployeeGetByIdWithDetail(int id)
        {
            // Attempt to fetch the object
            var o = ds.Employees.Include("JobDuties").SingleOrDefault(e => e.Id == id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<EmployeeWithJobDuties>(o);
        }

        // Attention 11 - Edit an employee's job duties
        public EmployeeWithJobDuties EmployeeEditJobDuties(EmployeeEditJobDuties newItem)
        {
            // Attempt to fetch the object

            // When editing an object with a to-many collection,
            // and you wish to edit the collection,
            // MUST fetch its associated collection
            var o = ds.Employees.Include("JobDuties")
                .SingleOrDefault(e => e.Id == newItem.Id);

            if (o == null)
            {
                // Problem - object was not found, so return
                return null;
            }
            else
            {
                // Update the object with the incoming values

                // First, clear out the existing collection
                o.JobDuties.Clear();

                // Then, go through the incoming items
                // For each one, add to the fetched object's collection
                foreach (var item in newItem.JobDutyIds)
                {
                    var a = ds.JobDuties.Find(item);
                    o.JobDuties.Add(a);
                }
                // Save changes
                ds.SaveChanges();

                return mapper.Map<EmployeeWithJobDuties>(o);
            }
        }

        // ############################################################
        // JobDuty

        public IEnumerable<JobDutyBase> JobDutyGetAll()
        {
            return mapper.Map<IEnumerable<JobDutyBase>>(ds.JobDuties.OrderBy(j => j.Name));
        }

        public IEnumerable<JobDutyWithEmployees> JobDutyGetAllWithEmployees()
        {
            return mapper.Map<IEnumerable<JobDutyWithEmployees>>
                (ds.JobDuties.Include("Employees").OrderBy(j => j.Name));
        }

        public JobDutyWithEmployees JobDutyGetByIdWithDetail(int id)
        {
            // Attempt to fetch the object
            var o = ds.JobDuties.Include("Employees").SingleOrDefault(j => j.Id == id);

            // Return the result, or null if not found
            return (o == null) ? null : mapper.Map<JobDutyWithEmployees>(o);
        }

        // Attention 11 - Edit a job duty's list of employees
        public JobDutyWithEmployees JobDutyEditEmployees(JobDutyEditEmployees newItem)
        {
            // Attempt to fetch the object

            // When editing an object with a to-many collection,
            // and you wish to edit the collection,
            // MUST fetch its associated collection
            var o = ds.JobDuties.Include("Employees")
                .SingleOrDefault(e => e.Id == newItem.Id);

            if (o == null)
            {
                // Problem - object was not found, so return
                return null;
            }
            else
            {
                // Update the object with the incoming values

                // First, clear out the existing collection
                o.Employees.Clear();

                // Then, go through the incoming items
                // For each one, add to the fetched object's collection
                foreach (var item in newItem.EmployeeIds)
                {
                    var a = ds.Employees.Find(item);
                    o.Employees.Add(a);
                }
                // Save changes
                ds.SaveChanges();

                return mapper.Map<JobDutyWithEmployees>(o);
            }
        }
    }
}