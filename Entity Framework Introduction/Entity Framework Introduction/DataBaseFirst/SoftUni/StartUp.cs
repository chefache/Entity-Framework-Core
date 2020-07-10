using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();
            Console.WriteLine(GetLatestProjects(db)); 
        }

        // Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var employees = context.Employees.ToList();

            foreach (Employee employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} " +
                    $"{employee.JobTitle} {employee.Salary:f2}");         
            }

            return sb.ToString();
        }

        // Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees.Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (Employee employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString();
        }

        // Problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                 .Select(e => new 
                 {
                     e.FirstName,
                     e.LastName,
                     DepartmentName = e.Department.Name,
                     e.Salary
                 })
                 .OrderBy(e => e.Salary)
                 .ThenByDescending(e => e.FirstName)
                 .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from " +
                    $"{employee.DepartmentName} - {employee.Salary:f2}");
            }

            return sb.ToString();
        }


        // Problem 6

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var sb = new StringBuilder();

            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee employeeAddressToChange = context.Employees
                .First(e => e.LastName == "Nakov");

            employeeAddressToChange.Address = newAddress;


            context.SaveChanges();

            List<string> addresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => x.Address.AddressText)
                .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine(address);
            }

            return sb.ToString();
        }


        // Problem 7

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employeess = context.Employees
            .Where(e => e.EmployeesProjects
            .Any(ep => ep.Project.StartDate.Year >= 2001 &&
            ep.Project.StartDate.Year <= 2003))
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager.FirstName,
                ManagerLastName = e.Manager.LastName,
                Projects = e.EmployeesProjects
                .Select(ep => new
                {
                    ProjectName = ep.Project.Name,
                    StartDate = ep.Project.StartDate
                    .ToString("M/d/yyyy h:mm:ss tt",CultureInfo.InvariantCulture),
                    EndDate = ep.Project.EndDate.HasValue ?
                    ep.Project
                    .EndDate
                    .Value
                    .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"
                })             
            });


            foreach (var e in employeess)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString();
        }

        // Problem 8

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var addresses = context.Addresses              
                .Select(a => new 
                {
                    AdressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count()
                })
                .OrderByDescending(e => e.EmployeesCount)
                .ThenBy(t => t.TownName)
                .ThenBy(at => at.AdressText)
                .Take(10);
               

            foreach (var a in addresses)
            {
                sb
                    .AppendLine($"{a.AdressText}, {a.TownName} - {a.EmployeesCount} employees");
            }
            return sb.ToString();
        }


        // Problem 9

        public static string GetEmployee147(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    Lastname = e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                    .Select(ep => ep.Project.Name)
                    .OrderBy(pn => pn)
                    .ToList()
                })
                .Single();

            sb.AppendLine($"{employee.FirstName} {employee.Lastname} - {employee.JobTitle}");

            foreach (var projectName in employee.Projects)
            {
                sb.AppendLine(projectName);
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 10

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastNAme = d.Manager.LastName,
                    DepartmentEmployees = d.Employees
                    .Select(de => new
                    {
                        EmployeeFirstName = de.FirstName,
                        EmployeeLastName = de.LastName,
                        de.JobTitle
                    })
                    .OrderBy(e => e.EmployeeFirstName)
                    .ThenBy(e => e.EmployeeLastName)
                    .ToList()
                })
                
                .ToList();

            foreach (var d in departments)
            {
                sb
                    .AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastNAme}");

                foreach (var e in d.DepartmentEmployees)
                {
                    sb
                        .AppendLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    ProjectName = p.Name,
                    ProjectDescription = p.Description,
                    ProjectStartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })                          
                .ToList();

            foreach (var p in projects)
            {
                sb
                    .AppendLine($"{p.ProjectName}");
                sb
                    .AppendLine($"{p.ProjectDescription}");
                sb
                    .AppendLine($"{p.ProjectStartDate}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
