
USE [master]
GO

CREATE DATABASE [PRN222ASM1]
GO

USE [PRN222ASM1]
GO

-- 1. Vehicle_Category
CREATE TABLE Vehicle_Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

-- 2. DealerType (Lookup)
CREATE TABLE DealerType (
    DealerTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL
);

-- 3. Dealer
CREATE TABLE Dealer (
    DealerID INT IDENTITY(1,1) PRIMARY KEY,
    DealerTypeID INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    FOREIGN KEY (DealerTypeID) REFERENCES DealerType(DealerTypeID)
);

-- 4. Inventory (per dealer)
CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    DealerID INT NOT NULL,
    FOREIGN KEY (DealerID) REFERENCES Dealer(DealerID)
);

-- 5. Vehicle
CREATE TABLE Vehicle (
    VehicleID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    ManufactureDate DATE NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Version NVARCHAR(50),
    Image NVARCHAR(255),
    FOREIGN KEY (CategoryID) REFERENCES Vehicle_Category(CategoryID)
);

-- 6. Vehicle_Inventory (N-N with Quantity)
CREATE TABLE Vehicle_Inventory (
    VehicleID INT NOT NULL,
    InventoryID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    PRIMARY KEY (VehicleID, InventoryID),
    FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID),
    FOREIGN KEY (InventoryID) REFERENCES Inventory(InventoryID)
);

-- 7. Customer
CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Address NVARCHAR(255) NOT NULL
);

-- 8. Appointment
CREATE TABLE Appointment (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    VehicleID INT NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID)
);

-- 9. Users (System users / staff)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    DealerID INT NOT NULL,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL, -- bcrypt/argon2 hash
    Role NVARCHAR(50) NOT NULL,
    FOREIGN KEY (DealerID) REFERENCES Dealer(DealerID)
);

-- 10. Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    UserID INT NOT NULL, -- sales rep / staff who created the order
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- 11. Order_Vehicle (N-N between Orders and Vehicles)
CREATE TABLE Order_Vehicle (
    OrderID INT NOT NULL,
    VehicleID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (OrderID, VehicleID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (VehicleID) REFERENCES Vehicle(VehicleID)
);

-- Create InventoryRequest table
CREATE TABLE [dbo].[InventoryRequest] (
    [RequestID] INT IDENTITY(1,1) NOT NULL,
    [VehicleID] INT NOT NULL,
    [DealerID] INT NOT NULL,
    [RequestedBy] INT NOT NULL,
    [RequestedQuantity] INT NOT NULL,
    [Reason] NVARCHAR(500) NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [RequestDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ProcessedDate] DATETIME NULL,
    [ProcessedBy] INT NULL,
    [AdminComment] NVARCHAR(500) NULL,
    
    CONSTRAINT [PK__Inventor__33A8517A12345678] PRIMARY KEY ([RequestID]),
    
    CONSTRAINT [FK__InventoryRequest__Vehicle] 
        FOREIGN KEY ([VehicleID]) REFERENCES [dbo].[Vehicle]([VehicleID]),
    
    CONSTRAINT [FK__InventoryRequest__Dealer] 
        FOREIGN KEY ([DealerID]) REFERENCES [dbo].[Dealer]([DealerID]),
    
    CONSTRAINT [FK__InventoryRequest__RequestedBy] 
        FOREIGN KEY ([RequestedBy]) REFERENCES [dbo].[Users]([UserID]),
    
    CONSTRAINT [FK__InventoryRequest__ProcessedBy] 
        FOREIGN KEY ([ProcessedBy]) REFERENCES [dbo].[Users]([UserID])
);

-- Add check constraint
ALTER TABLE [dbo].[InventoryRequest] 
ADD CONSTRAINT [CK_InventoryRequest_Quantity] 
CHECK ([RequestedQuantity] > 0);

ALTER TABLE [dbo].[InventoryRequest] 
ADD CONSTRAINT [CK_InventoryRequest_Status] 
CHECK ([Status] IN ('Pending', 'Approved', 'Denied'));

USE [PRN222ASM1];
GO

-- 1. Vehicle_Category
INSERT INTO Vehicle_Category (Name) VALUES
(N'SUV'), (N'Sedan'), (N'Truck'), (N'Coupe'), (N'EV');

-- 2. DealerType
INSERT INTO DealerType (TypeName) VALUES
(N'Authorized Dealer'), (N'Independent Dealer'), (N'Service Center');

-- 3. Dealer
INSERT INTO Dealer (DealerTypeID, Address) VALUES
(1, N'123 Main St, Hanoi'),
(1, N'456 Tran Hung Dao, HCMC'),
(2, N'789 Vo Thi Sau, Danang'),
(3, N'321 Nguyen Hue, Can Tho');

-- 4. Inventory
INSERT INTO Inventory (DealerID) VALUES (1), (1), (2), (3), (4);

-- 5. Vehicle
INSERT INTO Vehicle (CategoryID, Color, Price, ManufactureDate, Model, Version, Image) VALUES
(1, N'Black', 80000, '2023-01-15', N'Toyota Land Cruiser', N'VX', N'landcruiser.jpg'),
(2, N'White', 35000, '2022-06-20', N'Toyota Camry', N'LE', N'camry.jpg'),
(3, N'Red', 55000, '2021-03-05', N'Ford F-150', N'XLT', N'f150.jpg'),
(4, N'Blue', 40000, '2023-05-10', N'Honda Civic Coupe', N'Sport', N'civic.jpg'),
(5, N'Silver', 60000, '2024-02-28', N'Tesla Model Y', N'Long Range', N'modely.jpg');

-- 6. Customer
INSERT INTO Customer (Name, Phone, Email, Address) VALUES
(N'Nguyen Van A', N'0901234567', N'vana@example.com', N'Hanoi'),
(N'Tran Thi B', N'0912345678', N'thib@example.com', N'HCMC'),
(N'Le Van C', N'0923456789', N'vanc@example.com', N'Danang'),
(N'Pham Thi D', N'0934567890', N'thid@example.com', N'Can Tho'),
(N'Do Van E', N'0945678901', N'vane@example.com', N'Hai Phong');

-- 7. Users (must be before Orders and InventoryRequest)
INSERT INTO Users (DealerID, Username, PasswordHash, Role) VALUES
(1, N'admin', N'73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=', N'Admin'),
(2, N'dealer', N'73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=', N'DealerManager'),
(3, N'staff', N'73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=', N'DealerStaff'),
(4, N'staff2', N'73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=', N'DealerStaff');

-- 8. Vehicle_Inventory
INSERT INTO Vehicle_Inventory (VehicleID, InventoryID, Quantity) VALUES
(1, 1, 5),
(2, 1, 3),
(3, 2, 4),
(4, 3, 2),
(5, 4, 6);

-- 9. Appointment
INSERT INTO Appointment (CustomerID, VehicleID, AppointmentDate) VALUES
(1, 1, '2025-10-05 09:00'),
(2, 2, '2025-10-06 10:00'),
(3, 5, '2025-10-07 14:00'),
(4, 3, '2025-10-08 16:00'),
(5, 4, '2025-10-09 11:00');

-- 10. Orders (now works because Customers + Users exist)
INSERT INTO Orders (CustomerID, UserID, OrderDate, TotalAmount, Status) VALUES
(1, 1, GETDATE(), 160000, N'Completed'),
(2, 2, GETDATE(), 35000, N'Pending'),
(3, 3, GETDATE(), 60000, N'Processing');

-- 11. Order_Vehicle (Orders + Vehicles exist now)
INSERT INTO Order_Vehicle (OrderID, VehicleID, Quantity, UnitPrice) VALUES
(1, 1, 2, 80000),
(2, 2, 1, 35000),
(3, 5, 1, 60000);

-- 12. InventoryRequest (Users + Dealers + Vehicles exist now)
INSERT INTO InventoryRequest (VehicleID, DealerID, RequestedBy, RequestedQuantity, Reason, Status) VALUES
(1, 1, 1, 3, N'Increase demand for SUV in Hanoi', N'Pending'),
(2, 2, 2, 5, N'Stock refill for Camry', N'Approved'),
(5, 3, 3, 2, N'Test drive demand for EV', N'Denied');  -- use 'Denied' instead of 'Rejected'

