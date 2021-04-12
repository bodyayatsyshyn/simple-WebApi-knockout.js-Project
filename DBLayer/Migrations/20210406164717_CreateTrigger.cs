using Microsoft.EntityFrameworkCore.Migrations;

namespace DBLayer.Migrations
{
    public partial class CreateTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create trigger DetailsChangedTrigger 
                                    on Details
                                    after insert, update, delete
                                    as
                                    IF EXISTS (SELECT 1 FROM inserted)
                                    BEGIN
                                      IF EXISTS (SELECT 1 FROM deleted)
                                        BEGIN
                                          update Invoice
                                          set Sum = (select SUM(Sum) from Details where InvoiceId = (select InvoiceId from inserted))
                                          where InvoiceId = (select InvoiceId from inserted);
                                        END
                                      ELSE
                                        BEGIN
                                          update Invoice
                                          set Sum = (select SUM(Sum) from Details where InvoiceId = (select InvoiceId from inserted))
                                          where InvoiceId = (select InvoiceId from inserted);
                                        END
                                      END
                                    ELSE
                                    BEGIN
                                        update Invoice
                                        set Sum = (select SUM(Sum) from Details where InvoiceId = (select InvoiceId from deleted))
                                        where InvoiceId = (select InvoiceId from deleted);
                                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop trigger DetailsChangedTrigger");
        }
    }
}
