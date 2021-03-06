USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_fr_addProduct]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增產品
    日期: 2021-11-16

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_fr_addProduct]	
	@pId CHAR(36),
	@name NVARCHAR(10),
	@price INT,	
	@description NVARCHAR(250),	
	@categoryId INT,
	@stock INT,
	@isDel BIT,
	@isOpen BIT,
	@updateTime DATETIME,
	@createTime DATETIME
AS
	BEGIN
		INSERT INTO [ShoppingDB].[dbo].[t_products] (f_pId, f_name, f_price, f_description, f_categoryId, f_stock, f_isdel, f_isOpen, f_createTime)
		VALUES (@pId, @name, @price, @description, @categoryId, @stock, @isDel, @isOpen, @createTime)
	END
GO
