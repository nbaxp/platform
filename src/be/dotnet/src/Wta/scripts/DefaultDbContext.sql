ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `Department` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Number` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Order` int NOT NULL,
    `Path` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ParentId` char(36) COLLATE ascii_general_ci NULL,
    CONSTRAINT `PK_Department` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Department_Department_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Department` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `Dict` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Number` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Order` int NOT NULL,
    `Path` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ParentId` char(36) COLLATE ascii_general_ci NULL,
    CONSTRAINT `PK_Dict` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Dict_Dict_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Dict` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `Job` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Cron` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Disabled` tinyint(1) NOT NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Job` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Permission` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Authorize` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ButtonType` longtext CHARACTER SET utf8mb4 NULL,
    `ClassList` longtext CHARACTER SET utf8mb4 NULL,
    `Command` longtext CHARACTER SET utf8mb4 NULL,
    `Component` longtext CHARACTER SET utf8mb4 NULL,
    `Disabled` tinyint(1) NOT NULL,
    `Hidden` tinyint(1) NOT NULL,
    `Icon` longtext CHARACTER SET utf8mb4 NULL,
    `Method` longtext CHARACTER SET utf8mb4 NULL,
    `NoCache` tinyint(1) NOT NULL,
    `Redirect` longtext CHARACTER SET utf8mb4 NULL,
    `RouterPath` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Schema` longtext CHARACTER SET utf8mb4 NULL,
    `Type` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Url` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Number` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Order` int NOT NULL,
    `Path` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ParentId` char(36) COLLATE ascii_general_ci NULL,
    CONSTRAINT `PK_Permission` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Permission_Permission_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Permission` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `Role` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Number` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` varchar(255) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Role` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Tenant` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Number` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Disabled` tinyint(1) NOT NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` longtext CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Tenant` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `User` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `UserName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Avatar` longtext CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail` longtext CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed` tinyint(1) NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `SecurityStamp` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
    `TwoFactorEnabled` tinyint(1) NOT NULL,
    `AccessFailedCount` int NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint(1) NOT NULL,
    `IsReadOnly` tinyint(1) NOT NULL,
    `DepartmentId` char(36) COLLATE ascii_general_ci NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `CreatedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UpdatedOn` datetime(6) NULL,
    `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `TenantNumber` varchar(255) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_User` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_User_Department_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Department` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `RolePermission` (
    `RoleId` char(36) COLLATE ascii_general_ci NOT NULL,
    `PermissionId` char(36) COLLATE ascii_general_ci NOT NULL,
    `IsReadOnly` tinyint(1) NOT NULL,
    CONSTRAINT `PK_RolePermission` PRIMARY KEY (`RoleId`, `PermissionId`),
    CONSTRAINT `FK_RolePermission_Permission_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `Permission` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_RolePermission_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `UserRole` (
    `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
    `RoleId` char(36) COLLATE ascii_general_ci NOT NULL,
    `IsReadOnly` tinyint(1) NOT NULL,
    CONSTRAINT `PK_UserRole` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_UserRole_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserRole_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE INDEX `IX_Department_ParentId` ON `Department` (`ParentId`);


CREATE UNIQUE INDEX `IX_Department_TenantNumber_Number` ON `Department` (`TenantNumber`, `Number`);


CREATE INDEX `IX_Dict_ParentId` ON `Dict` (`ParentId`);


CREATE UNIQUE INDEX `IX_Dict_TenantNumber_Number` ON `Dict` (`TenantNumber`, `Number`);


CREATE INDEX `IX_Permission_ParentId` ON `Permission` (`ParentId`);


CREATE UNIQUE INDEX `IX_Permission_TenantNumber_Number` ON `Permission` (`TenantNumber`, `Number`);


CREATE UNIQUE INDEX `IX_Role_TenantNumber_Number` ON `Role` (`TenantNumber`, `Number`);


CREATE INDEX `IX_RolePermission_PermissionId` ON `RolePermission` (`PermissionId`);


CREATE UNIQUE INDEX `IX_Tenant_Number` ON `Tenant` (`Number`);


CREATE INDEX `IX_User_DepartmentId` ON `User` (`DepartmentId`);


CREATE UNIQUE INDEX `IX_User_TenantNumber_NormalizedUserName` ON `User` (`TenantNumber`, `NormalizedUserName`);


CREATE INDEX `IX_UserRole_RoleId` ON `UserRole` (`RoleId`);


