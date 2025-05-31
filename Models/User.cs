using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace task_4.Models;

/*
id SERIAL PRIMARY KEY,
name VARCHAR(255) NOT NULL,
email VARCHAR(255) UNIQUE NOT NULL,
password TEXT NOT NULL,
last_login TIMESTAMPTZ,
is_blocked BOOLEAN NOT NULL DEFAULT FALSE,
created_at TIMESTAMPTZ NOT NULL DEFAULT now()
 */

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [Column("password")]
    public string Password { get; set; }

    [Column("last_login")]
    public DateTimeOffset? LastLogin { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; } = false;

    [Required]
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
