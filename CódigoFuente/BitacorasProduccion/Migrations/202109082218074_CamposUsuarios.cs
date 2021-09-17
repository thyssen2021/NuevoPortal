namespace Portal_2_0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CamposUsuarios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Nombre", c => c.String(maxLength: 25));
            AddColumn("dbo.AspNetUsers", "Apellidos", c => c.String(maxLength: 45));
            AddColumn("dbo.AspNetUsers", "FechaCreacion", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FechaCreacion");
            DropColumn("dbo.AspNetUsers", "Apellidos");
            DropColumn("dbo.AspNetUsers", "Nombre");
        }
    }
}
