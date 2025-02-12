using System;
using System.Collections.Generic;
using System.IO;

public enum FileOperationStatus
{
    EmptyPath,
    FileDoesNotExist,
    Success
}


public static class EmployeesLogic
{
    public static List<EmployeesModel> AllEmployees = DataAccessClass.ReadList<EmployeesModel>("DataSources/employees.json");
    public static FileOperationStatus SaveEmployeeFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return FileOperationStatus.EmptyPath;

        bool success = DataAccessClass.EmpCopyToDestinationLogic(filePath);

        if (!success)
            return FileOperationStatus.FileDoesNotExist;

        return FileOperationStatus.Success;
    }



    public static bool NameLogic(string input)
    {
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    public static EmployeesModel SelectEmployeeId(int id)
    {
        var employee = DataAccessClass.ReadList<EmployeesModel>("DataSources/employees.json");

        return employee.FirstOrDefault(empl => empl.Id == id);
    }

    public static bool SaveChangesLogic(EmployeesModel employee)
    {
        var employees = DataAccessClass.ReadList<EmployeesModel>("DataSources/employees.json");
        var emloyeeToUpdate = employees.FirstOrDefault(f => f.Id == employee.Id);
        if (emloyeeToUpdate != null)
        {
            emloyeeToUpdate.Accepted = employee.Accepted;
            // EmployeesAccess.WriteAll(employees);
            DataAccessClass.WriteList<EmployeesModel>("DataSources/employees.json", employees);
            return true;
        }
        return false;
    }
    public static void OpenFile(string filePath)
    {
        // Determine the operating system
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // If Windows, use 'explorer' to open the file
            System.Diagnostics.Process.Start("explorer", filePath);
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // If macOS or Unix-based, use 'open' to open the file
            System.Diagnostics.Process.Start("open", filePath);
        }
        else
        {
            // If the operating system is unsupported, show an error message
            throw new PlatformNotSupportedException("Unsupported operating system for opening files.");
        }
    }

    public static void SaveEmployee(string name, int age, string cvFileName, int registrationID)
    {
        List<EmployeesModel> employees = DataAccessClass.ReadList<EmployeesModel>("DataSources/employees.json");
        EmployeesModel newEmployee = new EmployeesModel(
            employees.Count() + 1,
            name,
            age,
            // false,
            cvFileName,
            registrationID
        );

        employees.Add(newEmployee);
        DataAccessClass.WriteList<EmployeesModel>("DataSources/employees.json", employees);
    }

    public static EmployeesModel GetEmployeeByID(int id)
    {
        return AllEmployees.FirstOrDefault(f => f.RegistrationID == id);
    }

    public static EmployeesModel GetEmployeeByName(string name)
    {
        return AllEmployees.FirstOrDefault(r => r.Name == name);
    }
}
