--SELECT DISTINCT * FROM Challenge c INNER JOIN AspNetUsers u ON c.UserEmail = u.Email WHERE c.DaysComplete <> 71
SELECT DISTINCT Email FROM AspNetUsers
UNION
SELECT DISTINCT UserEmail As Email FROM Challenge