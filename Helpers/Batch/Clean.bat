@echo off

rem --------- Tables -----------

IF EXIST ".\Scripts\Tables\Sql\All.sql" (
  del ".\Scripts\Tables\Sql\All.sql"
)

IF EXIST ".\Scripts\Tables\External\All.sql" (
  del ".\Scripts\Tables\External\All.sql"
)

IF EXIST ".\Scripts\Tables\File\All.sql" (
  del ".\Scripts\Tables\File\All.sql"
)

IF EXIST ".\Scripts\Tables\Graph\All.sql" (
  del ".\Scripts\Tables\Graph\All.sql"
)

IF EXIST ".\Scripts\Tables.sql" (
  del ".\Scripts\Tables.sql"
)

rem --------- Views -----------
IF EXIST ".\Scripts\Views\Sql\All.sql" (
  del ".\Scripts\Views\Sql\All.sql"
)
IF EXIST ".\Scripts\Views.sql" (
  del ".\Scripts\Views.sql"
)
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

IF EXIST ".\Scripts\Functions.sql" (
  del ".\Scripts\Functions.sql"
)
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

IF EXIST ".\Scripts\Procedures.sql" (
  del ".\Scripts\Procedures.sql"
)

rem --------- Triggers -----------
IF EXIST ".\Scripts\Triggers\All.sql" (
  del ".\Scripts\Triggers\All.sql"
)

IF EXIST ".\Scripts\Triggers.sql" (
  del ".\Scripts\Triggers.sql"
)

rem --------- Data -----------
IF EXIST ".\Scripts\Data\Seed.sql" (
  del ".\Scripts\Data\Seed.sql"
)

IF EXIST ".\Scripts\Data\Messages.sql" (
  del ".\Scripts\Data\Messages.sql"
)

IF EXIST ".\Scripts\Data\Texts.sql" (
  del ".\Scripts\Data\Texts.sql"
)

IF EXIST ".\Scripts\Data.sql" (
  del ".\Scripts\Data.sql"
)

echo Done
