namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedforeignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FuncaoID", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "SetorID", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "FuncaoID");
            CreateIndex("dbo.AspNetUsers", "SetorID");
            AddForeignKey("dbo.AspNetUsers", "FuncaoID", "dbo.Funcao", "ID", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUsers", "SetorID", "dbo.Setor", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "SetorID", "dbo.Setor");
            DropForeignKey("dbo.AspNetUsers", "FuncaoID", "dbo.Funcao");
            DropIndex("dbo.AspNetUsers", new[] { "SetorID" });
            DropIndex("dbo.AspNetUsers", new[] { "FuncaoID" });
            DropColumn("dbo.AspNetUsers", "SetorID");
            DropColumn("dbo.AspNetUsers", "FuncaoID");
        }
    }
}
