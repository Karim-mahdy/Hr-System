-- Create the stored procedure
CREATE PROCEDURE sp_CalculateEmployeeSalaryReport
@EmployeeId INT,
@Month INT,
@Year INT
AS
BEGIN
  DECLARE @EmployeeName nvarchar(max)
  DECLARE @Department nvarchar(max)
  DECLARE @Salary FLOAT
  DECLARE @AttendanceDays INT
  DECLARE @AbsenceDays INT
  DECLARE @AdditionalPerHour FLOAT
  DECLARE @HourlyDiscount FLOAT
  DECLARE @TotalDiscount FLOAT
  DECLARE @TotalAdditional FLOAT
  DECLARE @NetSalary FLOAT

-- Retrieve Employee Information
SELECT @EmployeeName = CONCAT(e.FirstName, ' ', e.LastName), 
       @Department = d.DeptName,
       @Salary = e.Salary
FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.Id
WHERE e.Id = @EmployeeId;


  -- Calculate Number of attendance days
  SELECT @AttendanceDays = COUNT(*) 
  FROM Attendances a
  WHERE a.EmployeeId = @EmployeeId
    AND MONTH(a.Date) = @Month
    AND YEAR(a.Date) = @Year
    AND a.ArrivalTime IS NOT NULL
    AND a.Date NOT IN (
      SELECT Day
      FROM PublicHolidays
    )

  -- Calculate the number of days in the month
  DECLARE @DaysInMonth INT
  SET @DaysInMonth = DAY(EOMONTH(DATEFROMPARTS(@Year, @Month, 1)))

  -- Calculate the number of public holidays in the month
  DECLARE @PublicHolidaysCount INT
  SELECT @PublicHolidaysCount = COUNT(*)
  FROM PublicHolidays
  WHERE MONTH(Day) = @Month AND YEAR(Day) = @Year

-- Calculate the number of weekend days for each month
DECLARE @WeekendDaysCount INT
 
 --- Calculate the number of weekend days for each week in the month
DECLARE @GeneralSettingsId INT;
-- Determine the GeneralSettingsId based on the EmployeeId
SELECT @GeneralSettingsId = Id
FROM GeneralSettings
WHERE (@EmployeeId IS NOT NULL AND EmployeeId = @EmployeeId)
   OR ( EmployeeId IS NULL);

SELECT @WeekendDaysCount = COUNT(*)
FROM Weekend w
 WHERE w.GeneralSettingsId = @GeneralSettingsId
 
-- Subtract the count of weekend days from the total days in the month

SET @AbsenceDays = @DaysInMonth - @AttendanceDays - @PublicHolidaysCount - (@WeekendDaysCount * 4)
-- Now @WeekendDaysCount contains the total number of weekend days in the month


 
 -- Calculate Additional per hour and Hourly discount
  SELECT @AdditionalPerHour = (e.Salary / 22.0 / 8.0 /60 ) * 
    CASE
      WHEN gs.EmployeeId IS NOT NULL THEN gs.OvertimeHour
      ELSE (SELECT TOP 1 OvertimeHour FROM GeneralSettings WHERE EmployeeId IS NULL)
    END,
    @HourlyDiscount = (e.Salary / 22.0 / 8.0 /60) * 
    CASE
      WHEN gs.EmployeeId IS NOT NULL THEN gs.DiscountHour
      ELSE (SELECT TOP 1 DiscountHour FROM GeneralSettings WHERE EmployeeId IS NULL)
    END
  FROM Employees e
  LEFT JOIN GeneralSettings gs ON e.Id = gs.EmployeeId
  WHERE e.Id = @EmployeeId


-- Calculate Total Discount
SELECT @TotalDiscount = @HourlyDiscount * (
  SUM(
    CASE
      WHEN e.ArrivalTime < a.ArrivalTime THEN DATEDIFF(MINUTE, e.ArrivalTime, a.ArrivalTime)
      ELSE 0
    END
    +
    CASE
      WHEN e.LeaveTime > a.LeaveTime THEN DATEDIFF(MINUTE, a.LeaveTime, e.LeaveTime)
      ELSE 0
    END
  )
)
FROM Employees e
INNER JOIN Attendances a ON e.Id = a.EmployeeId
WHERE e.Id = @EmployeeId
  AND MONTH(a.Date) = @Month
  AND YEAR(a.Date) = @Year;

-- Calculate Total Additional
SELECT @TotalAdditional = @AdditionalPerHour * (
  SUM(
    CASE
      WHEN e.LeaveTime < a.LeaveTime THEN DATEDIFF(MINUTE, e.LeaveTime, a.LeaveTime)
      ELSE 0
    END
  )
)
FROM Employees e
INNER JOIN Attendances a ON e.Id = a.EmployeeId
WHERE e.Id = @EmployeeId
  AND MONTH(a.Date) = @Month
  AND YEAR(a.Date) = @Year;



  -- Calculate Net Salary
  SELECT @NetSalary = (e.Salary / @AttendanceDays ) - @TotalDiscount + @TotalAdditional 
  FROM Employees e
  WHERE e.Id = @EmployeeId
  
 
 -- Return the calculated values
SELECT 
    @EmployeeName as EmployeeName,
	@Department as Department,
	@Salary as  Salary,
	@AttendanceDays AS AttendanceDays,
    @AbsenceDays AS AbsenceDays,
    CAST(@AdditionalPerHour * 60 AS DECIMAL(10, 2)) AS AdditionalPerHour,
    CAST(@HourlyDiscount * 60 AS DECIMAL(10, 2)) AS HourlyDiscount,
    CAST(@TotalDiscount AS DECIMAL(10, 2)) AS TotalDiscount,
    CAST(@TotalAdditional AS DECIMAL(10, 2)) AS TotalAdditional,
    CAST(@NetSalary AS DECIMAL(10, 2)) AS NetSalary;
	
end
-- Replace @EmployeeId, @Month, and @Year with the desired values
EXEC sp_CalculateEmployeeSalaryReport @EmployeeId = 1, @Month =11, @Year = 2023;
