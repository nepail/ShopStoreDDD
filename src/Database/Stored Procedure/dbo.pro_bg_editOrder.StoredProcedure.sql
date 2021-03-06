USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editOrder]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 更新訂單狀態或運送狀態
    日期: 2021-12-21

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editOrder]
				 @account VARCHAR(20),
				 @id CHAR(14) ,
				 @status TINYINT ,
				 @shippingMethod TINYINT
AS
	BEGIN
		DECLARE @SQL NVARCHAR(100)
		SET @SQL = 'UPDATE t_orders SET '

		IF @status != 0
			BEGIN
				SET @SQL += 'f_status = ' + CONVERT(VARCHAR , @status) + ' ,'
			END

		IF @shippingMethod != 0
			BEGIN
				SET @SQL += 'f_shippingMethod = ' + CONVERT(VARCHAR , @shippingMethod) + ' ,'
			END

		SET @SQL = LEFT(@SQL , LEN(@SQL) - 1)

		SET @SQL += 'WHERE f_id = ' + @id

		--print @SQL
		EXEC sp_executesql @SQL
	END
GO
