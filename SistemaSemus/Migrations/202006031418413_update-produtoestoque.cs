namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateprodutoestoque : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProdutoEstoqueSemus", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.ProdutoEstoqueSemus", "UserID");
            AddForeignKey("dbo.ProdutoEstoqueSemus", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProdutoEstoqueSemus", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.ProdutoEstoqueSemus", new[] { "UserID" });
            DropColumn("dbo.ProdutoEstoqueSemus", "UserID");
        }
    }
}
