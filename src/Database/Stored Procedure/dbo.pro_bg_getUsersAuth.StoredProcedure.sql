USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getUsersAuth]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得後台用戶權限
    日期: 2021-12-25

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getUsersAuth]
				 @userId INT
AS
	BEGIN

		DECLARE @groupID INT

		SET @groupID = (SELECT TOP 1 f_groupId
			FROM t_manager_users WITH (NOLOCK)
			WHERE f_id = @userId
		);

		SELECT menu.f_id AS MenuId , menu.f_name AS MenuName , ISNULL(users.f_permissionCode , 0) AS PermissionCode
		FROM (SELECT f_id , f_groupId , f_menuSubId
			FROM t_manager_group_permissions WITH (NOLOCK)
			WHERE f_groupId = @groupID
		) a LEFT JOIN t_manager_users_permissions users WITH (NOLOCK) ON a.f_menuSubId = users.f_menuSubId
							AND
							users.f_userId = @userId INNER JOIN t_manager_menuSub menu WITH (NOLOCK) ON a.f_menuSubId = menu.f_id


	END
GO
