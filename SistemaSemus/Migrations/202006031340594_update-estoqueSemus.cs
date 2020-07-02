namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateestoqueSemus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EstoqueSemus", "UserID", c => c.String(maxLength: 128));
            AddColumn("dbo.EstoqueSemus", "DataCadastro", c => c.DateTime(nullable: false));
            AddColumn("dbo.EstoqueSemus", "DataAtualizacao", c => c.DateTime(nullable: false));
            CreateIndex("dbo.EstoqueSemus", "UserID");
            AddForeignKey("dbo.EstoqueSemus", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EstoqueSemus", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.EstoqueSemus", new[] { "UserID" });
            DropColumn("dbo.EstoqueSemus", "DataAtualizacao");
            DropColumn("dbo.EstoqueSemus", "DataCadastro");
            DropColumn("dbo.EstoqueSemus", "UserID");
        }
    }
}
