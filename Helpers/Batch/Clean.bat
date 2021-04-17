@echo off

rem --------- Schemas -----------
IF EXIST ".\Scripts\Schemas\Sql\All.sql" (
  del ".\Scripts\Schemas\Sql\All.sql"
)
IF EXIST ".\Scripts\Schemas.sql" (
  del ".\Scripts\Schemas.sql"
)

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

IF EXIST ".\Scripts\Functions.sql" (
  del ".\Scripts\Functions.sql"
)

IF EXIST ".\Scripts\Functions-Base.sql" (
  del ".\Scripts\Functions-Base.sql"
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

rem --------- Relations -----------
IF EXIST ".\Scripts\Relations\All.sql" (
  del ".\Scripts\Relations\All.sql"
)

IF EXIST ".\Scripts\Relations.sql" (
  del ".\Scripts\Relations.sql"
)

rem --------- Indexes -----------
IF EXIST ".\Scripts\Indexes\All.sql" (
  del ".\Scripts\Indexes\All.sql"
)

IF EXIST ".\Scripts\Indexes.sql" (
  del ".\Scripts\Indexes.sql"
)

rem --------- Types -----------
IF EXIST ".\Scripts\Types\Clr\All.sql" (
  del ".\Scripts\Types\Clr\All.sql"
)

IF EXIST ".\Scripts\Types\Scaler\All.sql" (
  del ".\Scripts\Types\Scaler\All.sql"
)

IF EXIST ".\Scripts\Types\Table\All.sql" (
  del ".\Scripts\Types\Table\All.sql"
)

IF EXIST ".\Scripts\Types\XmlSchemaCollection\All.sql" (
  del ".\Scripts\Types\XmlSchemaCollection\All.sql"
)

IF EXIST ".\Scripts\Types.sql" (
  del ".\Scripts\Types.sql"
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

IF EXIST ".\Scripts\All.sql" (
  del ".\Scripts\All.sql"
)

echo Done
