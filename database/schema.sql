-- Run this once against a fresh SQL Server / LocalDB instance.

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EmsDb')
BEGIN
    CREATE DATABASE EmsDb;
END
GO

USE EmsDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'employees')
BEGIN
    CREATE TABLE employees (
        employee_id   INT           IDENTITY(1,1) PRIMARY KEY,
        first_name    NVARCHAR(100) NOT NULL,
        last_name     NVARCHAR(100) NOT NULL,
        email         NVARCHAR(255) NOT NULL,
        phone         NVARCHAR(20)  NULL,
        department    NVARCHAR(100) NOT NULL,
        position      NVARCHAR(100) NOT NULL,
        hire_date     DATE          NOT NULL,
        salary        DECIMAL(12,2) NOT NULL,
        created_at    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        updated_at    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT uq_employees_email UNIQUE (email)
    );
END
GO

-- Optional seed data for local development
INSERT INTO employees (first_name, last_name, email, phone, department, position, hire_date, salary)
VALUES
    ('Ahmad',   'Razali',   'ahmad.razali@ems.local',   '+60123456789', 'Engineering', 'Senior Engineer',  '2021-03-15', 9500.00),
    ('Siti',    'Norzahra', 'siti.norzahra@ems.local',  '+60198765432', 'HR',          'HR Manager',        '2019-07-01', 8200.00),
    ('Wei',     'Liang',    'wei.liang@ems.local',       NULL,           'Engineering', 'Junior Engineer',  '2023-01-10', 5500.00),
    ('Priya',   'Selvam',   'priya.selvam@ems.local',   '+60112233445', 'Finance',     'Finance Analyst',  '2020-11-22', 7000.00),
    ('Hafizah', 'Othman',   'hafizah.othman@ems.local', '+60167788990', 'Engineering', 'Tech Lead',         '2018-05-30', 12000.00);
GO
