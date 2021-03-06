USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editProduct]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 更新產品資訊
    日期: 2021-12-13

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editProduct]
				 @f_pId CHAR(36) ,
				 @f_name NVARCHAR(20) ,
				 @f_price INT ,
				 @f_description NVARCHAR(250) ,
				 @f_categoryId INT ,
				 @f_stock INT ,
				 @f_isdel BIT ,
				 @f_isopen BIT ,
				 @f_updatetime DATETIME
AS
	BEGIN
		UPDATE t_products WITH(ROWLOCK)
			   SET f_name = @f_name ,
				   f_price = @f_price ,
				   f_description = @f_description ,
				   f_categoryId = @f_categoryId ,
				   f_stock = @f_stock ,
				   f_isDel = @f_isdel ,
				   f_isOpen = @f_isopen ,
				   f_updateTime = @f_updatetime
		WHERE f_pId = @f_pId
	END
GO
