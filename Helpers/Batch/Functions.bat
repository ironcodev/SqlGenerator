@echo off
echo Concatenating Functions ...
rem --------- Functions -----------
rem ---------- All (clean) ------------
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

rem ---------- Base (clean) ------------

IF EXIST ".\Scripts\Functions\Scaler\Base\All.sql" (
  del ".\Scripts\Functions\Scaler\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Clr\Base\All.sql" (
  del ".\Scripts\Functions\Clr\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Table\Base\All.sql" (
  del ".\Scripts\Functions\Table\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Inline\Base\All.sql" (
  del ".\Scripts\Functions\Inline\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Aggregate\Base\All.sql" (
  del ".\Scripts\Functions\Aggregate\Base\All.sql"
)

rem ---------- All (create) ------------

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

rem ---------- Base (create) ------------

IF EXIST ".\Scripts\Functions\Scaler\Base\*.sql" (
	copy Scripts\Functions\Scaler\Base\*.sql Scripts\Functions\Scaler\Base\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Clr\Base\*.sql" (
	copy Scripts\Functions\Clr\Base\*.sql Scripts\Functions\Clr\Base\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Table\Base\*.sql" (
	copy Scripts\Functions\Table\Base\*.sql Scripts\Functions\Table\Base\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Inline\Base\*.sql" (
	copy Scripts\Functions\Inline\Base\*.sql Scripts\Functions\Inline\Base\All.sql > NUL
)
IF EXIST ".\Scripts\Functions\Aggregate\Base\*.sql" (
	copy Scripts\Functions\Aggregate\Base\*.sql Scripts\Functions\Aggregate\Base\All.sql > NUL
)
rem ======== FINAL ==========
IF EXIST ".\Scripts\Functions.sql" (
  del ".\Scripts\Functions.sql"
)

IF EXIST ".\Scripts\Functions-Base.sql" (
  del ".\Scripts\Functions-Base.sql"
)

copy	Scripts\Functions\Scaler\All.sql 	+ ^
		Scripts\Functions\Clr\All.sql		+ ^
		Scripts\Functions\Inline\All.sql	+ ^
		Scripts\Functions\Aggregate\All.sql	+ ^
		Scripts\Functions\Table\All.sql Scripts\Functions.sql  > NUL

copy	Scripts\Functions\Scaler\Base\All.sql 	+ ^
		Scripts\Functions\Clr\Base\All.sql		+ ^
		Scripts\Functions\Inline\Base\All.sql	+ ^
		Scripts\Functions\Aggregate\Base\All.sql	+ ^
		Scripts\Functions\Table\Base\All.sql Scripts\Functions-Base.sql  > NUL
		
rem ---------- All (clean) ------------
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

rem ---------- Base (clean) ------------

IF EXIST ".\Scripts\Functions\Scaler\Base\All.sql" (
  del ".\Scripts\Functions\Scaler\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Clr\Base\All.sql" (
  del ".\Scripts\Functions\Clr\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Table\Base\All.sql" (
  del ".\Scripts\Functions\Table\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Inline\Base\All.sql" (
  del ".\Scripts\Functions\Inline\Base\All.sql"
)

IF EXIST ".\Scripts\Functions\Aggregate\Base\All.sql" (
  del ".\Scripts\Functions\Aggregate\Base\All.sql"
)

echo Done
