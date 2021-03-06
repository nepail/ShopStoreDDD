USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_fr_getProducts]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得產品資訊
    日期: 2021-11-16

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_fr_getProducts]
				 @isopen INT
AS
	IF @isopen = 1
		BEGIN
			SELECT a.f_id , a.f_pId , a.f_name , a.f_price , a.f_description , a.f_categoryId , c.f_name AS categoryName , a.f_stock , a.f_isOpen , a.f_isDel , a.f_createTime
			FROM t_products a WITH (NOLOCK) JOIN t_categories c WITH (NOLOCK) ON c.f_id = a.f_categoryId
			WHERE a.f_isOpen = @isopen
		END
	ELSE
		BEGIN
			SELECT a.f_id , a.f_pId , a.f_name , a.f_price , a.f_description , a.f_categoryId , c.f_name AS categoryName , a.f_stock , a.f_isOpen , a.f_isDel , a.f_createTime
			FROM t_products a WITH (NOLOCK) JOIN t_categories c WITH (NOLOCK) ON c.f_id = a.f_categoryId
		END
GO
