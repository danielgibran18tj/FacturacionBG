
-- =============================================
-- SEED DATA - BANCO GUAYAQUIL
-- Datos Iniciales del Sistema
-- =============================================
USE [BillingDB]


INSERT INTO [dbo].[SystemSettings] ([SettingKey], [SettingValue], [Description], [DataType], [IsSystem])
VALUES 
    -- Impuestos
    ('IVA_PERCENTAGE', '15.00', 'Porcentaje de IVA a aplicar en facturas', 'decimal', 1),
    
    -- Información de la Empresa
    ('COMPANY_NAME', 'Banco Guayaquil', 'Nombre de la empresa', 'string', 1),
    ('COMPANY_RUC', '0990000000001', 'RUC/Identificación de la empresa', 'string', 1),
    ('COMPANY_ADDRESS', 'Av. 9 de Octubre, Guayaquil, Ecuador', 'Dirección de la empresa', 'string', 0),
    ('COMPANY_PHONE', '042-555-0000', 'Teléfono de la empresa', 'string', 0),
    ('COMPANY_EMAIL', 'contacto@bancobg.com', 'Email de la empresa', 'string', 0),
    
    -- Formato de Factura
    ('INVOICE_NUMBER_LENGTH', '6', 'Longitud del número de factura (ej: 000001)', 'number', 1),
    ('INVOICE_FOOTER_TEXT', 'Gracias por su compra - Banco Guayaquil', 'Texto al pie de la factura', 'string', 0)


SET IDENTITY_INSERT [dbo].[Roles] ON;

INSERT INTO [dbo].[Roles] ([Id], [Name], [Description], [CreatedAt])
VALUES 
    (1, 'Administrator', 'Full system access - Can manage users, settings, and all invoices', GETUTCDATE()),
    (2, 'Seller', 'Can create invoices, manage customers and products', GETUTCDATE()),
    (3, 'Customer', 'Can view their own invoices and account details', GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Roles] OFF;


SET IDENTITY_INSERT [dbo].[Users] ON;

-- Password para todos: Admin123! (hash con BCrypt)
INSERT INTO [dbo].[Users] ([Id], [Username], [Email], [PasswordHash], [FirstName], [LastName], [IsActive], [CreatedAt])
VALUES 
    (1, 'admin', 'admin@bancobg.com', '$2a$11$26cWK.JlxYax.jqpkBWs1evQz37oD4uqbKieNmk5/SNlQRCuP4sw6', 'System', 'Administrator', 1, GETUTCDATE()),
    (2, 'seller1', 'maria.gonzalez@bancobg.com', '$2a$11$26cWK.JlxYax.jqpkBWs1evQz37oD4uqbKieNmk5/SNlQRCuP4sw6', 'María', 'González', 1, GETUTCDATE()),
    (3, 'seller2', 'carlos.ramirez@bancobg.com', '$2a$11$26cWK.JlxYax.jqpkBWs1evQz37oD4uqbKieNmk5/SNlQRCuP4sw6', 'Carlos', 'Ramírez', 1, GETUTCDATE()),
    (4, 'customer_juan', 'juan.perez@email.com', '$2a$11$26cWK.JlxYax.jqpkBWs1evQz37oD4uqbKieNmk5/SNlQRCuP4sw6', 'Juan', 'Pérez', 1, GETUTCDATE()),
    (5, 'customer_maria', 'mf.lopez@email.com', '$2a$11$26cWK.JlxYax.jqpkBWs1evQz37oD4uqbKieNmk5/SNlQRCuP4sw6', 'María Fernanda', 'López', 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Users] OFF;


INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId], [AssignedAt])
VALUES 
    (1, 1, GETUTCDATE()), -- admin -> Administrator
    (2, 2, GETUTCDATE()), -- seller1 -> Seller
    (3, 2, GETUTCDATE()), -- seller2 -> Seller
    (4, 3, GETUTCDATE()), -- customer_juan -> Customer
    (5, 3, GETUTCDATE()); -- customer_maria -> Customer


SET IDENTITY_INSERT [dbo].[Customers] ON;

INSERT INTO [dbo].[Customers] ([Id], [DocumentNumber], [FullName], [Phone], [Email], [Address], [UserId], [IsActive], [CreatedAt])
VALUES 
    -- Clientes con cuenta de usuario (pueden ver sus facturas)
    (1, '0987654321', 'Juan Pérez Zambrano', '0999-555-002', 'juan.perez@email.com', 'Cdla. Kennedy Norte, Guayaquil', 4, 1, GETUTCDATE()),
    (2, '0934567890', 'María Fernanda López', '0987-555-003', 'mf.lopez@email.com', 'Urdesa Central, Guayaquil', 5, 1, GETUTCDATE()),
    
    -- Clientes sin cuenta de usuario
    (3, '0912345678001', 'Comercial Ecuador S.A.', '042-555-001', 'contacto@comercialec.com', 'Av. 9 de Octubre y Malecón, Guayaquil', NULL, 1, GETUTCDATE()),
    (4, '0945678901', 'Empresa Distribuidora XYZ Cía. Ltda.', '042-555-004', 'ventas@xyz.com', 'Parque Industrial, Guayaquil', NULL, 1, GETUTCDATE()),
    (5, '0967890123', 'Tech Solutions Corp.', '042-555-007', 'info@techsolutions.com', 'World Trade Center, Guayaquil', NULL, 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Customers] OFF;



SET IDENTITY_INSERT [dbo].[Products] ON;

INSERT INTO [dbo].[Products] ([Id], [Code], [Name], [Description], [UnitPrice], [Stock], [MinStock], [IsActive], [CreatedAt])
VALUES 
    -- Electrónica
    (1, 'ELEC-001', 'Laptop HP Pavilion 15', 'Intel Core i7 11th Gen, 16GB RAM, 512GB SSD, Windows 11', 899.99, 15, 5, 1, GETUTCDATE()),
    (2, 'ELEC-002', 'Mouse Logitech MX Master 3', 'Mouse inalámbrico ergonómico, 7 botones programables', 99.99, 50, 10, 1, GETUTCDATE()),
    (3, 'ELEC-003', 'Teclado Mecánico Corsair K70', 'Switches Cherry MX, RGB, reposamuñecas desmontable', 149.99, 30, 10, 1, GETUTCDATE()),
    (4, 'ELEC-004', 'Monitor LG 27" 4K', 'UHD 3840x2160, IPS, HDR10, 60Hz', 399.99, 20, 5, 1, GETUTCDATE()),
    
    -- Accesorios
    (5, 'ACC-001', 'Cable HDMI 4K 2m', 'Soporte 4K 60Hz, alta velocidad', 12.99, 100, 30, 1, GETUTCDATE()),
    (6, 'ACC-002', 'Hub USB-C 7 en 1', '3x USB 3.0, HDMI, SD, Ethernet, USB-C PD', 45.99, 50, 15, 1, GETUTCDATE()),
    (7, 'ACC-003', 'Mousepad XXL Gaming', '900x400mm, antideslizante, RGB', 29.99, 60, 20, 1, GETUTCDATE()),
    
    -- Oficina
    (8, 'OFF-001', 'Silla Ergonómica de Oficina', 'Respaldo alto, ajuste lumbar, reposabrazos 3D', 199.99, 10, 3, 1, GETUTCDATE()),
    (9, 'OFF-002', 'Escritorio Ajustable Altura', 'Eléctrico, 120x60cm, altura 72-122cm', 449.99, 5, 2, 1, GETUTCDATE()),
    (10, 'OFF-003', 'Lámpara LED de Escritorio', 'Regulable, 3 modos de luz, puerto USB', 39.99, 30, 10, 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Products] OFF;



SET IDENTITY_INSERT [dbo].[PaymentMethods] ON;

INSERT INTO [dbo].[PaymentMethods] ([Id], [Name], [IsActive], [CreatedAt])
VALUES 
    (1, 'Cash', 1, GETUTCDATE()),
    (2, 'Credit Card', 1, GETUTCDATE()),
    (3, 'Debit Card', 1, GETUTCDATE()),
    (4, 'Bank Transfer', 1, GETUTCDATE()),
    (5, 'Check', 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF;

