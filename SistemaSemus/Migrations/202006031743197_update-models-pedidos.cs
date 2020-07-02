namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodelspedidos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PedidoCompra", "UserID", c => c.String(maxLength: 128));
            AddColumn("dbo.PedidoEstoque", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.PedidoCompra", "UserID");
            CreateIndex("dbo.PedidoEstoque", "UserID");
            AddForeignKey("dbo.PedidoCompra", "UserID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PedidoEstoque", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PedidoEstoque", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.PedidoCompra", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.PedidoEstoque", new[] { "UserID" });
            DropIndex("dbo.PedidoCompra", new[] { "UserID" });
            DropColumn("dbo.PedidoEstoque", "UserID");
            DropColumn("dbo.PedidoCompra", "UserID");
        }
    }
}
