@echo off
echo Concatenating Functions ...
rem --------- Functions -----------

IF EXIST ".\Scripts\Functions\Scaler\All.sql" (
  del ".\Scripts\Functions\Scaler\All.sql"
)

IF EXIST ".\Scripts\Functions\Clr\All.sql" (
  del ".\Scripts\Functions\Clr\All.sql"
)

IF EXIST ".\Scripts\Functions\Table\All.sql" (
  del ".\Scripts\Functions\Table\All.sql"
)

IF EXIST ".\Scripts\Functions\Inline\All.sql" (
  del ".\Scripts\Functions\Inline\All.sql"
)

IF EXIST ".\Scripts\Functions\Aggregate\All.sql" (
  del ".\Scripts\Functions\Aggregate\All.sql"
)

IF EXIST ".\Scripts\Functions\Scaler\*.sql" (
	copy Scripts\Functions\Scaler\*.sql Scripts\Functions\Scaler\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Clr\*.sql" (
	copy Scripts\Functions\Clr\*.sql Scripts\Functions\Clr\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Table\*.sql" (
	copy Scripts\Functions\Table\*.sql Scripts\Functions\Table\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Inline\*.sql" (
	copy Scripts\Functions\Inline\*.sql Scripts\Functions\Inline\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Aggregate\*.sql" (
	copy Scripts\Functions\Aggregate\*.sql Scripts\Functions\Aggregate\All.sql > NUL
)
rem ======== FINAL ==========
IF EXIST ".\Scripts\Functions.sql" (
  del ".\Scripts\Functions.sql"
)

copy	Scripts\Functions\Scaler\All.sql 	+ ^
		Scripts\Functions\Clr\All.sql		+ ^
		Scripts\Functions\Inline\All.sql	+ ^
		Scripts\Functions\Aggregate\All.sql	+ ^
		Scripts\Functions\Table\All.sql Scripts\Functions.sql  > NUL
		
IF EXIST ".\Scripts\Functions\Scaler\All.sql" (
  del ".\Scripts\Functions\Scaler\All.sql"
)

IF EXIST ".\Scripts\Functions\Clr\All.sql" (
  del ".\Scripts\Functions\Clr\All.sql"
)

IF EXIST ".\Scripts\Functions\Table\All.sql" (
  del ".\Scripts\Functions\Table\All.sql"
)

IF EXIST ".\Scripts\Functions\Inline\All.sql" (
  del ".\Scripts\Functions\Inline\All.sql"
)

IF EXIST ".\Scripts\Functions\Aggregate\All.sql" (
  del ".\Scripts\Functions\Aggregate\All.sql"
)

echo Done
