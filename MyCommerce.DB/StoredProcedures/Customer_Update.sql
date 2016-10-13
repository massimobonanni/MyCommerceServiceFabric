CREATE PROCEDURE [dbo].[Customer_Update]
	@userName nvarchar(50),
	@firstName nvarchar(50),
	@lastName nvarchar(50),
	@isEnabled bit
AS
	
	update Customers
	set FirstName=@firstName,
		LastName =@lastName,
		IsEnabled=@isEnabled
	where UserName=@userName

RETURN 0
