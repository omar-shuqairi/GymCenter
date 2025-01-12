using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aboutuspage> Aboutuspages { get; set; }

    public virtual DbSet<Contactform> Contactforms { get; set; }

    public virtual DbSet<Contactu> Contactus { get; set; }

    public virtual DbSet<Creditcard> Creditcards { get; set; }

    public virtual DbSet<Footer> Footers { get; set; }

    public virtual DbSet<Homepage> Homepages { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Siteinfo> Siteinfos { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<Whychooseu> Whychooseus { get; set; }

    public virtual DbSet<Workoutplan> Workoutplans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("DATA SOURCE=localhost:1521;USER ID=c##omarshuqairi3;PASSWORD=Test321;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##OMARSHUQAIRI3")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Aboutuspage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008692");

            entity.ToTable("ABOUTUSPAGE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Backgroundvideoimg)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BACKGROUNDVIDEOIMG");
            entity.Property(e => e.Paragraph1)
                .HasColumnType("CLOB")
                .HasColumnName("PARAGRAPH1");
            entity.Property(e => e.Paragraph2)
                .HasColumnType("CLOB")
                .HasColumnName("PARAGRAPH2");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TITLE");
            entity.Property(e => e.Videourl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VIDEOURL");
        });

        modelBuilder.Entity<Contactform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008698");

            entity.ToTable("CONTACTFORM");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Guestcomment)
                .HasColumnType("CLOB")
                .HasColumnName("GUESTCOMMENT");
            entity.Property(e => e.Guestemail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GUESTEMAIL");
            entity.Property(e => e.Guestname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GUESTNAME");
        });

        modelBuilder.Entity<Contactu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008694");

            entity.ToTable("CONTACTUS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Locationn)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOCATIONN");
            entity.Property(e => e.Mapurl)
                .HasColumnType("CLOB")
                .HasColumnName("MAPURL");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TITLE");
        });

        modelBuilder.Entity<Creditcard>(entity =>
        {
            entity.HasKey(e => e.Cardid).HasName("SYS_C008680");

            entity.ToTable("CREDITCARD");

            entity.HasIndex(e => e.Cardnumber, "SYS_C008681").IsUnique();

            entity.Property(e => e.Cardid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CARDID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER")
                .HasColumnName("BALANCE");
            entity.Property(e => e.Cardnumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("CARDNUMBER");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CVV");
            entity.Property(e => e.Expirydate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRYDATE");
        });

        modelBuilder.Entity<Footer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008696");

            entity.ToTable("FOOTER");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Paragraph)
                .HasColumnType("CLOB")
                .HasColumnName("PARAGRAPH");
            entity.Property(e => e.Tip1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TIP1");
            entity.Property(e => e.Tip2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TIP2");
        });

        modelBuilder.Entity<Homepage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008688");

            entity.ToTable("HOMEPAGE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Title1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TITLE1");
            entity.Property(e => e.Title2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TITLE2");
            entity.Property(e => e.Titlebtn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TITLEBTN");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Memberid).HasName("SYS_C008671");

            entity.ToTable("MEMBERS");

            entity.Property(e => e.Memberid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBERID");
            entity.Property(e => e.Planid)
                .HasColumnType("NUMBER")
                .HasColumnName("PLANID");
            entity.Property(e => e.SubscriptionEnd)
                .HasColumnType("DATE")
                .HasColumnName("SUBSCRIPTION_END");
            entity.Property(e => e.SubscriptionStart)
                .HasColumnType("DATE")
                .HasColumnName("SUBSCRIPTION_START");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Plan).WithMany(p => p.Members)
                .HasForeignKey(d => d.Planid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PLAN_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Members)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MEMBER_USER_ID");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("SYS_C008683");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.Paymentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENTID");
            entity.Property(e => e.Amountpaid)
                .HasColumnType("NUMBER")
                .HasColumnName("AMOUNTPAID");
            entity.Property(e => e.Invoicepath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("INVOICEPATH");
            entity.Property(e => e.Paymentdate)
                .HasDefaultValueSql("sysdate")
                .HasColumnType("DATE")
                .HasColumnName("PAYMENTDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PAYMENTS_USER_ID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("SYS_C008660");

            entity.ToTable("ROLE");

            entity.Property(e => e.Roleid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<Siteinfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008686");

            entity.ToTable("SITEINFO");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.LogoImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOGO_IMAGE_PATH");
            entity.Property(e => e.SharedImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SHARED_IMAGE_PATH");
            entity.Property(e => e.SiteImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SITE_IMAGE_PATH");
            entity.Property(e => e.Sitename)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SITENAME");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Testimonialid).HasName("SYS_C008675");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.Testimonialid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIALID");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CONTENT");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("STATUS");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_USER_ID_TESTIMONIALS");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("SYS_C008658");

            entity.ToTable("USERS");

            entity.Property(e => e.Userid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LNAME");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Loginid).HasName("SYS_C008662");

            entity.ToTable("USER_LOGIN");

            entity.HasIndex(e => e.Username, "SYS_C008663").IsUnique();

            entity.Property(e => e.Loginid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("LOGINID");
            entity.Property(e => e.Passwordd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORDD");
            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Username)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Role).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ROLE_ID");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_USER_ID");
        });

        modelBuilder.Entity<Whychooseu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008690");

            entity.ToTable("WHYCHOOSEUS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.IconClass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ICON_CLASS");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TITLE");
        });

        modelBuilder.Entity<Workoutplan>(entity =>
        {
            entity.HasKey(e => e.Planid).HasName("SYS_C008669");

            entity.ToTable("WORKOUTPLANS");

            entity.Property(e => e.Planid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PLANID");
            entity.Property(e => e.Durationinmonths)
                .HasColumnType("NUMBER")
                .HasColumnName("DURATIONINMONTHS");
            entity.Property(e => e.ExerciseRoutines)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EXERCISE_ROUTINES");
            entity.Property(e => e.Goals)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("GOALS");
            entity.Property(e => e.Planname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PLANNAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER")
                .HasColumnName("PRICE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
