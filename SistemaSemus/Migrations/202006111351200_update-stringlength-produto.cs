namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestringlengthproduto : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Produto", "Descricao", c => c.String(maxLength: 80));
            AlterColumn("dbo.Produto", "Categoria", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Produto", "Categoria", c => c.String(maxLength: 50));
            AlterColumn("dbo.Produto", "Descricao", c => c.String(maxLength: 100));
        }
    }
}
