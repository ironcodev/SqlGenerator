@echo off

echo Concatenating Procedures ...

rem --------- Procedures -----------

IF EXIST ".\Scripts\Procedures\Sql\All.sql" (
  del ".\Scripts\Procedures\Sql\All.sql"
)

IF EXIST ".\Scripts\Procedures\Clr\All.sql" (
  del ".\Scripts\Procedures\Clr\All.sql"
)

IF EXIST ".\Scripts\Procedures\Extended\All.sql" (
  del ".\Scripts\Procedures\Extended\All.sql"
)

IF EXIST ".\Scripts\Procedures\Sql\*.sql" (
	copy Scripts\Procedures\Sql\*.sql Scripts\Procedures\Sql\All.sql > NUL
)
IF EXIST ".\Scripts\Procedures\Clr\*.sql" (
	copy Scripts\Procedures\Clr\*.sql Scripts\Procedures\Clr\All.sql > NUL
)
IF EXIST ".\Scripts\Procedures\Extended\*.sql" (
	copy Scripts\Procedures\Extended\*.sql Scripts\Procedures\Extended\All.sql > NUL
)

rem ======== FINAL ==========

IF EXIST ".\Scripts\Procedures.sql" (
  del ".\Scripts\Procedures.sql"
)

copy	Scripts\Procedures\Sql\All.sql 	+ ^
		Scripts\Procedures\Clr\All.sql	+ ^
		Scripts\Procedures\Extended\All.sql Scripts\Procedures.sql  > NUL
		
IF EXIST ".\Scripts\Procedures\Sql\All.sql" (
  del ".\Scripts\Procedures\Sql\All.sql"
)

IF EXIST ".\Scripts\Procedures\Clr\All.sql" (
  del ".\Scripts\Procedures\Clr\All.sql"
)

IF EXIST ".\Scripts\Procedures\Extended\All.sql" (
  del ".\Scripts\Procedures\Extended\All.sql"
)

echo Done
