namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodelreceita : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Receita", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Receita", "UserID");
            AddForeignKey("dbo.Receita", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Receita", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Receita", new[] { "UserID" });
            DropColumn("dbo.Receita", "UserID");
        }
    }
}
