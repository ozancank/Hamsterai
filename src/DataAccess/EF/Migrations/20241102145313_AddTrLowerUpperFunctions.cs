using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddTrLowerUpperFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION tr_lower(input TEXT)
                RETURNS TEXT AS $$
                BEGIN
                    RETURN lower(replace(input, 'İ', 'i'));
                END;
                $$ LANGUAGE plpgsql;
                ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION tr_upper(input TEXT)
                RETURNS TEXT AS $$
                BEGIN
                    RETURN upper(replace(input, 'ı', 'I'));
                END;
                $$ LANGUAGE plpgsql;
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS tr_lower(TEXT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS tr_upper(TEXT);");
        }
    }
}