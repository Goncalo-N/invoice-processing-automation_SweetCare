INSERT INTO [dbo].[suppliersName] (supplierName, supplierContactName, supplierContactEmail, supplierContactPhone, supplierContactLang, supplierLeavePending, supplierReturnsContactName, supplierReturnsContactEmail, workdaysToDeliver, discountPP, discountAffectIva, cobraIva, daysToPay, contributeToLeadTime, isActive, obsPrivate, obsShared)
VALUES
('LEX', 'John Doe', 'johndoe@lex.com', '555-0101', 'EN', 0, 'Jane Doe', 'janedoe@lex.com', 5, 5.0, 0, 1, 30, 1, 1, 'Private notes about LEX', 'Shared notes about LEX'),
('Roger & Gallet', 'Alexis Duval', 'alexis@rogergallet.com', '555-0202', 'FR', 0, 'Martine Dupont', 'martine@rogergallet.com', 3, 7.0, 1, 1, 45, 1, 1, 'Private notes about Roger & Gallet', 'Shared notes about Roger & Gallet');
