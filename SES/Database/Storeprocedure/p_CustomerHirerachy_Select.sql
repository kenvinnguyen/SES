
CREATE PROCEDURE [dbo].[p_CustomerHirerachy_Select]
	@CustomerID nvarchar(15) = ''
AS
	SELECT	a.CustomerHirerachyID
			,a.CustomerHirerachyName
			,a.CustomerHirerachyIndex
	FROM	CustomerHirerachy a
				INNER JOIN CustomerHirerachyDetail b ON b.CustomerHirerachyID = a.CustomerHirerachyID AND b.CustomerID = @CustomerID
	WHERE	a.Status = 1
	ORDER BY a.CustomerHirerachyIndex

