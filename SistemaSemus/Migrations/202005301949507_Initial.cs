namespace SistemaSemus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EstoqueSemus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PedidoEstoque",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EstoqueSemusID = c.Int(nullable: false),
                        TipoPedido = c.Byte(nullable: false),
                        DataEntrada = c.DateTime(nullable: false),
                        DataFaturado = c.DateTime(),
                        Faturado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EstoqueSemus", t => t.EstoqueSemusID, cascadeDelete: true)
                .Index(t => t.EstoqueSemusID);
            
            CreateTable(
                "dbo.PedidoProduto",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PedidoEstoqueID = c.Int(nullable: false),
                        ProdutoID = c.Int(),
                        ProdutoSemEstoqueID = c.Int(),
                        Descricao = c.String(),
                        Quantidade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PedidoEstoque", t => t.PedidoEstoqueID, cascadeDelete: true)
                .Index(t => t.PedidoEstoqueID);
            
            CreateTable(
                "dbo.ProdutoEstoqueSemus",
                c => new
                    {
                        Produto_ID = c.Int(nullable: false),
                        EstoqueSemus_ID = c.Int(nullable: false),
                        QuantidadeTotal = c.Int(nullable: false),
                        QuantidadeEntrada = c.Int(nullable: false),
                        QuantidadeSaida = c.Int(nullable: false),
                        QuantidadeEmFalta = c.Int(nullable: false),
                        DataEntrada = c.DateTime(nullable: false),
                        DataSaida = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.Produto_ID, t.EstoqueSemus_ID })
                .ForeignKey("dbo.EstoqueSemus", t => t.EstoqueSemus_ID, cascadeDelete: true)
                .ForeignKey("dbo.Produto", t => t.Produto_ID, cascadeDelete: true)
                .Index(t => t.Produto_ID)
                .Index(t => t.EstoqueSemus_ID);
            
            CreateTable(
                "dbo.Produto",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TipoProduto = c.Byte(nullable: false),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Funcao",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Medico",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 13),
                        Nome = c.String(maxLength: 50),
                        Area_Atuacao = c.String(maxLength: 100),
                        DataCadastro = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Receita",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EstoqueSemusID = c.Int(nullable: false),
                        DataCadastro = c.DateTime(nullable: false),
                        MedicoID = c.String(maxLength: 13),
                        PacienteID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EstoqueSemus", t => t.EstoqueSemusID, cascadeDelete: true)
                .ForeignKey("dbo.Medico", t => t.MedicoID)
                .ForeignKey("dbo.Paciente", t => t.PacienteID)
                .Index(t => t.EstoqueSemusID)
                .Index(t => t.MedicoID)
                .Index(t => t.PacienteID);
            
            CreateTable(
                "dbo.Paciente",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Nome = c.String(maxLength: 50),
                        RG = c.String(maxLength: 20),
                        Orgao_Emissor = c.String(maxLength: 50),
                        DataNascimento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Receitado",
                c => new
                    {
                        ProdutoID = c.Int(nullable: false),
                        ReceitaID = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProdutoID)
                .ForeignKey("dbo.Produto", t => t.ProdutoID)
                .ForeignKey("dbo.Receita", t => t.ReceitaID, cascadeDelete: true)
                .Index(t => t.ProdutoID)
                .Index(t => t.ReceitaID);
            
            CreateTable(
                "dbo.PedidoCompraNaoCadastrado",
                c => new
                    {
                        Produto_ID = c.Int(nullable: false),
                        PedidoCompra_ID = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Produto_ID, t.PedidoCompra_ID })
                .ForeignKey("dbo.PedidoCompra", t => t.PedidoCompra_ID, cascadeDelete: true)
                .ForeignKey("dbo.ProdutoNaoCadastrado", t => t.Produto_ID, cascadeDelete: true)
                .Index(t => t.Produto_ID)
                .Index(t => t.PedidoCompra_ID);
            
            CreateTable(
                "dbo.PedidoCompra",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EstoqueSemus_ID = c.Int(nullable: false),
                        TipoProduto = c.Byte(nullable: false),
                        Faturado = c.Boolean(nullable: false),
                        DataEntrada = c.DateTime(nullable: false),
                        DataFaturado = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EstoqueSemus", t => t.EstoqueSemus_ID, cascadeDelete: true)
                .Index(t => t.EstoqueSemus_ID);
            
            CreateTable(
                "dbo.PedidoCompraProduto",
                c => new
                    {
                        Produto_ID = c.Int(nullable: false),
                        PedidoCompra_ID = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Produto_ID, t.PedidoCompra_ID })
                .ForeignKey("dbo.PedidoCompra", t => t.PedidoCompra_ID, cascadeDelete: true)
                .ForeignKey("dbo.Produto", t => t.Produto_ID, cascadeDelete: true)
                .Index(t => t.Produto_ID)
                .Index(t => t.PedidoCompra_ID);
            
            CreateTable(
                "dbo.ProdutoNaoCadastrado",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TipoProduto = c.Byte(nullable: false),
                        Quantidade = c.Int(nullable: false),
                        Descricao = c.String(),
                        DataEntrada = c.DateTime(nullable: false),
                        DataPedido = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Setor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nome = c.String(nullable: false, maxLength: 50),
                        Endereco = c.String(nullable: false),
                        Nascimento = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PedidoCompraNaoCadastrado", "Produto_ID", "dbo.ProdutoNaoCadastrado");
            DropForeignKey("dbo.PedidoCompraNaoCadastrado", "PedidoCompra_ID", "dbo.PedidoCompra");
            DropForeignKey("dbo.PedidoCompraProduto", "Produto_ID", "dbo.Produto");
            DropForeignKey("dbo.PedidoCompraProduto", "PedidoCompra_ID", "dbo.PedidoCompra");
            DropForeignKey("dbo.PedidoCompra", "EstoqueSemus_ID", "dbo.EstoqueSemus");
            DropForeignKey("dbo.Receitado", "ReceitaID", "dbo.Receita");
            DropForeignKey("dbo.Receitado", "ProdutoID", "dbo.Produto");
            DropForeignKey("dbo.Receita", "PacienteID", "dbo.Paciente");
            DropForeignKey("dbo.Receita", "MedicoID", "dbo.Medico");
            DropForeignKey("dbo.Receita", "EstoqueSemusID", "dbo.EstoqueSemus");
            DropForeignKey("dbo.ProdutoEstoqueSemus", "Produto_ID", "dbo.Produto");
            DropForeignKey("dbo.ProdutoEstoqueSemus", "EstoqueSemus_ID", "dbo.EstoqueSemus");
            DropForeignKey("dbo.PedidoProduto", "PedidoEstoqueID", "dbo.PedidoEstoque");
            DropForeignKey("dbo.PedidoEstoque", "EstoqueSemusID", "dbo.EstoqueSemus");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PedidoCompraProduto", new[] { "PedidoCompra_ID" });
            DropIndex("dbo.PedidoCompraProduto", new[] { "Produto_ID" });
            DropIndex("dbo.PedidoCompra", new[] { "EstoqueSemus_ID" });
            DropIndex("dbo.PedidoCompraNaoCadastrado", new[] { "PedidoCompra_ID" });
            DropIndex("dbo.PedidoCompraNaoCadastrado", new[] { "Produto_ID" });
            DropIndex("dbo.Receitado", new[] { "ReceitaID" });
            DropIndex("dbo.Receitado", new[] { "ProdutoID" });
            DropIndex("dbo.Receita", new[] { "PacienteID" });
            DropIndex("dbo.Receita", new[] { "MedicoID" });
            DropIndex("dbo.Receita", new[] { "EstoqueSemusID" });
            DropIndex("dbo.ProdutoEstoqueSemus", new[] { "EstoqueSemus_ID" });
            DropIndex("dbo.ProdutoEstoqueSemus", new[] { "Produto_ID" });
            DropIndex("dbo.PedidoProduto", new[] { "PedidoEstoqueID" });
            DropIndex("dbo.PedidoEstoque", new[] { "EstoqueSemusID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Setor");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProdutoNaoCadastrado");
            DropTable("dbo.PedidoCompraProduto");
            DropTable("dbo.PedidoCompra");
            DropTable("dbo.PedidoCompraNaoCadastrado");
            DropTable("dbo.Receitado");
            DropTable("dbo.Paciente");
            DropTable("dbo.Receita");
            DropTable("dbo.Medico");
            DropTable("dbo.Funcao");
            DropTable("dbo.Produto");
            DropTable("dbo.ProdutoEstoqueSemus");
            DropTable("dbo.PedidoProduto");
            DropTable("dbo.PedidoEstoque");
            DropTable("dbo.EstoqueSemus");
            DropTable("dbo.Claims");
        }
    }
}
