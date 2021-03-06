

create PROCEDURE [dbo].[p_get_SalesPerson]		
	@Page int = 1,
	@PageSize int = 100,
	@WhereCondition nvarchar(MAX) = ''
AS
	DECLARE @Sql nvarchar(MAX), @Start int, @End int
	SET @Start = (@PageSize * @Page) - (@PageSize - 1)
	SET @End = @PageSize * @Page
	
	SET @Sql = 
	'
		;WITH FullSet AS
		(
			select sp.*,c.CompanyName from SalesPerson sp
			join Company c
			on sp.CompanyID=c.CompanyID
	)
	 
	,FilteredSet AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY SalesPersonID) AS ''Number''
				,*
		FROM FullSet
		WHERE 1 = 1 '+ @WhereCondition +'
	)
	 
	,CountSet AS
	(
		SELECT COUNT(*) AS ''RowCount''
		FROM FilteredSet
	)
	 
	SELECT b.[RowCount], a.*
	FROM FilteredSet a CROSS APPLY CountSet b
	WHERE Number BETWEEN '+ CAST(@Start AS varchar(10)) +' AND '+ CAST(@End AS varchar(10)) +'
	'
	--print @sql
	EXEC (@Sql)