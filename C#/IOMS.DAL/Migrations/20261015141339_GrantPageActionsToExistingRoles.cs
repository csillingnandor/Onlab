using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOMS.DAL.Migrations
{
    // Adatmigráció: az oldalakon belüli műveletek (AppActions) bevezetése előtt az oldal-jogosultság
    // minden műveletet is jelentett. Hogy a meglévő szerepkörök ugyanúgy működjenek, minden oldalukhoz
    // megkapják az összes műveletet. (A lista szándékosan itt rögzített, a migráció pillanatnyi állapotát tükrözi.)
    /// <inheritdoc />
    public partial class GrantPageActionsToExistingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                SELECT rc.RoleId, 'action', a.Action
                FROM AspNetRoleClaims rc
                JOIN (VALUES
                    ('products', 'products.create'),
                    ('products', 'products.edit'),
                    ('products', 'products.delete'),
                    ('customer-orders', 'customer-orders.create'),
                    ('customer-orders', 'customer-orders.ship'),
                    ('customer-orders', 'customer-orders.deliver'),
                    ('customer-orders', 'customer-orders.cancel'),
                    ('supplier-orders', 'supplier-orders.create'),
                    ('supplier-orders', 'supplier-orders.receive'),
                    ('customers', 'customers.create')
                ) AS a(Page, Action) ON rc.ClaimValue = a.Page
                WHERE rc.ClaimType = 'page'
                  AND NOT EXISTS (
                      SELECT 1 FROM AspNetRoleClaims e
                      WHERE e.RoleId = rc.RoleId AND e.ClaimType = 'action' AND e.ClaimValue = a.Action);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoleClaims WHERE ClaimType = 'action';");
        }
    }
}
