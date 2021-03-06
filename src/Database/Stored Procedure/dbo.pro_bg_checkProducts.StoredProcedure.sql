USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_checkProducts]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 檢查產品庫存
    日期: 2022-01-11

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_checkProducts]

AS
	BEGIN
		SELECT f_id AS Id , f_name AS Name , f_stock AS Stock
		FROM t_products WITH (NOLOCK)
		WHERE f_stock < 20
		ORDER BY Stock
	END
GO
